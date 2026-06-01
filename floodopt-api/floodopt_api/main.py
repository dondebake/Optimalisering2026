"""FloodOpt API — stap 2.1 MVP.

Vier endpoints, geen business logic in deze laag:
    POST /scenarios           scenario opslaan
    POST /trajectories        traject opslaan
    POST /optimize            optimalisatie uitvoeren (synchroon voor MVP)
    GET  /results/{job_id}    resultaat ophalen

Business logic zit volledig in floodopt-core.
Async queue (Celery) volgt in stap 2.3.
"""

from __future__ import annotations

import uuid

from fastapi import FastAPI, HTTPException

from floodopt_api import store
from floodopt_api.models import OptimizeRequest, OptimizeResponse
from floodopt_core.io.models import Scenario, Trajectory
from floodopt_core.optimization.brute_force import BruteForceOptimizer
from floodopt_core.optimization.pyomo_optimizer import PyomoOptimizer
from floodopt_core.physics.simple_dike_overflow import SimpleDikeOverflow
from floodopt_core.risk.simple_risk_calculator import SimpleRiskCalculator

app = FastAPI(
    title="FloodOpt API",
    description=(
        "Optimalisatie van dijkversterkingsstrategieën. "
        "Physics en Risk Layer zitten in floodopt-core; "
        "deze API is een dunne HTTP-schil."
    ),
    version="0.1.0",
)

# Optimizer-instanties — gedeeld over requests (stateless berekeningen)
_physics = SimpleDikeOverflow()
_risk = SimpleRiskCalculator(physics=_physics)
_optimizers = {
    "brute_force": BruteForceOptimizer(risk=_risk),
    "pyomo": PyomoOptimizer(risk=_risk),
}


# ---------------------------------------------------------------------------
# POST /scenarios
# ---------------------------------------------------------------------------


@app.post("/scenarios", status_code=201, response_model=Scenario)
def create_scenario(scenario: Scenario) -> Scenario:
    """Sla een hydraulisch scenario op."""
    store._scenarios[scenario.id] = scenario
    return scenario


@app.get("/scenarios/{scenario_id}", response_model=Scenario)
def get_scenario(scenario_id: str) -> Scenario:
    scenario = store._scenarios.get(scenario_id)
    if scenario is None:
        raise HTTPException(404, detail=f"Scenario '{scenario_id}' niet gevonden")
    return scenario


# ---------------------------------------------------------------------------
# POST /trajectories
# ---------------------------------------------------------------------------


@app.post("/trajectories", status_code=201, response_model=Trajectory)
def create_trajectory(trajectory: Trajectory) -> Trajectory:
    """Sla een dijktraject op. Maatregelen worden via /optimize meegegeven."""
    store._trajectories[trajectory.id] = trajectory
    return trajectory


@app.get("/trajectories/{trajectory_id}", response_model=Trajectory)
def get_trajectory(trajectory_id: str) -> Trajectory:
    trajectory = store._trajectories.get(trajectory_id)
    if trajectory is None:
        raise HTTPException(404, detail=f"Traject '{trajectory_id}' niet gevonden")
    return trajectory


# ---------------------------------------------------------------------------
# POST /optimize
# ---------------------------------------------------------------------------


@app.post("/optimize", status_code=201, response_model=OptimizeResponse)
def optimize(request: OptimizeRequest) -> OptimizeResponse:
    """Voer een optimalisatie uit en sla het resultaat op.

    MVP: synchroon (geen Celery). Status is altijd 'completed'.
    Stap 2.3 voegt async job-queue toe.
    """
    trajectory = store._trajectories.get(request.trajectory_id)
    if trajectory is None:
        raise HTTPException(
            404, detail=f"Traject '{request.trajectory_id}' niet gevonden"
        )

    scenario = store._scenarios.get(request.scenario_id)
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
    store._results[job_id] = response
    return response


# ---------------------------------------------------------------------------
# GET /results/{job_id}
# ---------------------------------------------------------------------------


@app.get("/results/{job_id}", response_model=OptimizeResponse)
def get_result(job_id: str) -> OptimizeResponse:
    """Haal het resultaat van een optimalisatierun op via job_id."""
    result = store._results.get(job_id)
    if result is None:
        raise HTTPException(404, detail=f"Resultaat '{job_id}' niet gevonden")
    return result
