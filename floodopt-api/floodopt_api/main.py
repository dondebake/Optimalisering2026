"""FloodOpt API — stap 2.1 / 2.2 / 2.3.

Endpoints:
    POST /scenarios           scenario opslaan
    POST /trajectories        traject opslaan
    POST /optimize            optimalisatie inplannen (202 + job_id)
    GET  /results/{job_id}    status + resultaat opvragen

DATABASE_URL env-variabele bepaalt de backend:
    niet ingesteld  →  SQLite (floodopt.db naast de projectroot, geen install)
    ingesteld       →  PostgreSQL (productie, meerdere workers)

REDIS_URL env-variabele bepaalt de Celery-broker:
    niet ingesteld  →  redis://localhost:6379/0

Business logic zit volledig in floodopt-core.
De worker (floodopt_worker.tasks.run_optimization) voert de berekening uit.
"""

from __future__ import annotations

import uuid
from contextlib import asynccontextmanager
from typing import Annotated, Generator

from fastapi import Depends, FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware

from floodopt_api.celery_app import celery_app
from floodopt_api.database import get_effective_url, init_schema, make_session
from floodopt_api.models import OptimizeRequest, OptimizeResponse
from floodopt_api.repositories import OrmRepositories, Repositories
from floodopt_core.io.models import Scenario, Trajectory


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
        "POST /optimize geeft direct een job_id terug; "
        "de Celery worker voert de berekening asynchroon uit."
    ),
    version="0.3.0",
    lifespan=lifespan,
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:5173"],
    allow_methods=["*"],
    allow_headers=["*"],
)


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


@app.post("/optimize", status_code=202, response_model=OptimizeResponse)
def optimize(request: OptimizeRequest, repos: Repos) -> OptimizeResponse:
    """Plan een optimalisatie in en retourneer direct een job_id (202 Accepted).

    Status-flow: pending → running → done
    Prik de status op via GET /results/{job_id}.
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

    job_id = str(uuid.uuid4())
    pending = OptimizeResponse(
        job_id=job_id,
        status="pending",
        trajectory_id=request.trajectory_id,
        scenario_id=request.scenario_id,
        objective=request.objective,
        solver=request.solver,
    )
    repos.save_result(pending)

    payload = {
        "trajectory": trajectory.model_dump(),
        "scenario": scenario.model_dump(),
        "candidates": [m.model_dump() for m in request.candidates],
        "risk_params": request.risk_params.model_dump(),
        "objective": request.objective.value,
        "budget": request.budget,
        "solver": request.solver,
    }
    celery_app.send_task(
        "floodopt_worker.tasks.run_optimization", args=[job_id, payload]
    )

    return pending


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
