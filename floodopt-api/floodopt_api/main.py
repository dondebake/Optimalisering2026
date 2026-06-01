"""FloodOpt API — stap 2.1 / 2.2.

Endpoints:
    POST /scenarios           scenario opslaan
    POST /trajectories        traject opslaan
    POST /optimize            optimalisatie uitvoeren
    GET  /results/{job_id}    resultaat ophalen

DATABASE_URL env-variabele bepaalt de backend:
    niet ingesteld  →  SQLite (floodopt.db naast de projectroot, geen install)
    ingesteld       →  PostgreSQL + PostGIS (productie)

Business logic zit volledig in floodopt-core.
Async queue (Celery) volgt in stap 2.3.
"""

from __future__ import annotations

import uuid
from contextlib import asynccontextmanager
from typing import Annotated, Generator

from fastapi import Depends, FastAPI, HTTPException

from floodopt_api.database import get_effective_url, init_schema, make_session
from floodopt_api.models import OptimizeRequest, OptimizeResponse
from floodopt_api.repositories import OrmRepositories, Repositories
from floodopt_core.io.models import Scenario, Trajectory
from floodopt_core.optimization.brute_force import BruteForceOptimizer
from floodopt_core.optimization.pyomo_optimizer import PyomoOptimizer
from floodopt_core.physics.simple_dike_overflow import SimpleDikeOverflow
from floodopt_core.risk.simple_risk_calculator import SimpleRiskCalculator


@asynccontextmanager
async def lifespan(app: FastAPI):  # type: ignore[type-arg]
    """Schema aanmaken bij opstart (idempotent)."""
    init_schema(get_effective_url())
    yield


app = FastAPI(
    title="FloodOpt API",
    description=(
        "Optimalisatie van dijkversterkingsstrategieën. "
        "Physics en Risk Layer zitten in floodopt-core; "
        "deze API is een dunne HTTP-schil. "
        "Standaard: SQLite. Stel DATABASE_URL in voor PostgreSQL."
    ),
    version="0.2.0",
    lifespan=lifespan,
)

# Optimizer-instanties — stateless, gedeeld over requests
_physics = SimpleDikeOverflow()
_risk = SimpleRiskCalculator(physics=_physics)
_optimizers = {
    "brute_force": BruteForceOptimizer(risk=_risk),
    "pyomo": PyomoOptimizer(risk=_risk),
}


# ---------------------------------------------------------------------------
# Dependency: repositories
# ---------------------------------------------------------------------------


def get_repositories() -> Generator[Repositories, None, None]:
    """Opent een sessie naar de geconfigureerde database (SQLite of PostgreSQL)
    en sluit deze na het request.
    """
    session = make_session(get_effective_url())
    try:
        yield OrmRepositories(session)
    finally:
        session.close()


Repos = Annotated[Repositories, Depends(get_repositories)]


# ---------------------------------------------------------------------------
# POST /scenarios
# ---------------------------------------------------------------------------


@app.post("/scenarios", status_code=201, response_model=Scenario)
def create_scenario(scenario: Scenario, repos: Repos) -> Scenario:
    """Sla een hydraulisch scenario op."""
    repos.save_scenario(scenario)
    return scenario


@app.get("/scenarios/{scenario_id}", response_model=Scenario)
def get_scenario(scenario_id: str, repos: Repos) -> Scenario:
    scenario = repos.get_scenario(scenario_id)
    if scenario is None:
        raise HTTPException(404, detail=f"Scenario '{scenario_id}' niet gevonden")
    return scenario


# ---------------------------------------------------------------------------
# POST /trajectories
# ---------------------------------------------------------------------------


@app.post("/trajectories", status_code=201, response_model=Trajectory)
def create_trajectory(trajectory: Trajectory, repos: Repos) -> Trajectory:
    """Sla een dijktraject op. Maatregelen worden via /optimize meegegeven."""
    repos.save_trajectory(trajectory)
    return trajectory


@app.get("/trajectories/{trajectory_id}", response_model=Trajectory)
def get_trajectory(trajectory_id: str, repos: Repos) -> Trajectory:
    trajectory = repos.get_trajectory(trajectory_id)
    if trajectory is None:
        raise HTTPException(404, detail=f"Traject '{trajectory_id}' niet gevonden")
    return trajectory


# ---------------------------------------------------------------------------
# POST /optimize
# ---------------------------------------------------------------------------


@app.post("/optimize", status_code=201, response_model=OptimizeResponse)
def optimize(request: OptimizeRequest, repos: Repos) -> OptimizeResponse:
    """Voer een optimalisatie uit en sla het resultaat op.

    MVP: synchroon (geen Celery). Status is altijd 'completed'.
    Stap 2.3 voegt async job-queue toe.
    """
    trajectory = repos.get_trajectory(request.trajectory_id)
    if trajectory is None:
        raise HTTPException(
            404, detail=f"Traject '{request.trajectory_id}' niet gevonden"
        )

    scenario = repos.get_scenario(request.scenario_id)
    if scenario is None:
        raise HTTPException(
            404, detail=f"Scenario '{request.scenario_id}' niet gevonden"
        )

    optimizer = _optimizers[request.solver]
    result = optimizer.solve(
        trajectory,
        scenario,
        request.candidates,
        request.risk_params,
        request.objective,
        request.budget,
    )

    job_id = str(uuid.uuid4())
    response = OptimizeResponse(
        job_id=job_id,
        trajectory_id=request.trajectory_id,
        scenario_id=request.scenario_id,
        objective=request.objective,
        solver=request.solver,
        selected_measure_ids=sorted(result.selected_ids),
        total_ncw=result.total_ncw,
        risk_ncw=result.risk_ncw,
        investment_npv=result.investment_npv,
        objective_value=result.objective_value,
    )
    repos.save_result(response)
    return response


# ---------------------------------------------------------------------------
# GET /results/{job_id}
# ---------------------------------------------------------------------------


@app.get("/results/{job_id}", response_model=OptimizeResponse)
def get_result(job_id: str, repos: Repos) -> OptimizeResponse:
    """Haal het resultaat van een optimalisatierun op via job_id."""
    result = repos.get_result(job_id)
    if result is None:
        raise HTTPException(404, detail=f"Resultaat '{job_id}' niet gevonden")
    return result
