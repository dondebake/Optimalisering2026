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

import json
import uuid
from contextlib import asynccontextmanager
from pathlib import Path
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
    payload = {
        "trajectory": trajectory.model_dump(),
        "scenario": scenario.model_dump(),
        "candidates": [m.model_dump() for m in request.candidates],
        "risk_params": request.risk_params.model_dump(),
        "objective": request.objective.value,
        "budget": request.budget,
        "solver": request.solver,
        "cost_function": request.cost_function.model_dump()
        if request.cost_function
        else None,
    }

    pending = OptimizeResponse(
        job_id=job_id,
        status="pending",
        trajectory_id=request.trajectory_id,
        scenario_id=request.scenario_id,
        objective=request.objective,
        solver=request.solver,
        input_payload=payload,
    )
    repos.save_result(pending)

    celery_app.send_task(
        "floodopt_worker.tasks.run_optimization", args=[job_id, payload]
    )

    return pending


# ---------------------------------------------------------------------------
# GET /geo/trajectories
# ---------------------------------------------------------------------------


_DIJKRINGDELEN_GEOJSON = (
    Path(__file__).parent.parent.parent
    / "tests"
    / "validation"
    / "dijkringdelen.geojson"
).resolve()


@app.get("/geo/dijkringdelen")
def geo_dijkringdelen() -> dict:
    """Dijkringdelen (2011) als GeoJSON met P₀-waarden — gegenereerd via convert_dijkringdelen.py."""
    if not _DIJKRINGDELEN_GEOJSON.exists():
        raise HTTPException(
            503,
            detail="dijkringdelen.geojson ontbreekt — draai eerst: python scripts/convert_dijkringdelen.py",
        )
    return json.loads(_DIJKRINGDELEN_GEOJSON.read_text(encoding="utf-8"))


@app.get("/geo/trajectories")
def geo_trajectories(repos: Repos, year: int = 2050) -> dict:
    """Retourneer alle opgeslagen trajecten als GeoJSON FeatureCollection.

    Query-parameter `year` (standaard 2050) bepaalt welke P-waarde uit de
    tijdreeks als `p_year` in de feature-properties wordt opgenomen. Deze
    waarde wordt door de kaart gebruikt voor de kleurcodering.
    """
    trajectories = repos.get_all_trajectories()

    # Meest recente resultaat per traject (voor kleurcodering)
    all_results = repos.get_all_results()
    latest_by_traj: dict[str, object] = {}
    for r in reversed(all_results):  # get_all_results geeft nieuwste eerst
        if r.trajectory_id not in latest_by_traj:
            latest_by_traj[r.trajectory_id] = r

    features = []
    for t in trajectories:
        p_year: float | None = None
        result = latest_by_traj.get(t.id)
        if result is not None and result.p_series:  # type: ignore[union-attr]
            for point in result.p_series:  # type: ignore[union-attr]
                if int(point["year"]) == year:
                    p_year = point["p"]
                    break

        features.append(
            {
                "type": "Feature",
                "geometry": t.geometry,
                "properties": {
                    "id": t.id,
                    "norm": t.norm,
                    "length": t.length,
                    "p0": t.p0,
                    "base_year": t.base_year,
                    "p_year": p_year,
                    "year": year,
                },
            }
        )
    return {"type": "FeatureCollection", "features": features}


# ---------------------------------------------------------------------------
# GET /results/{job_id}
# ---------------------------------------------------------------------------


@app.get("/results", response_model=list[OptimizeResponse])
def list_results(repos: Repos) -> list[OptimizeResponse]:
    """Retourneer alle optimalisatieresultaten, nieuwste eerst."""
    return repos.get_all_results()


@app.get("/results/{job_id}", response_model=OptimizeResponse)
def get_result(job_id: str, repos: Repos) -> OptimizeResponse:
    """Haal het resultaat van een optimalisatierun op via job_id."""
    result = repos.get_result(job_id)
    if result is None:
        raise HTTPException(404, detail=f"Resultaat '{job_id}' niet gevonden")
    return result


@app.delete("/results/{job_id}", status_code=204)
def delete_result(job_id: str, repos: Repos) -> None:
    """Verwijder een optimalisatieresultaat."""
    if not repos.delete_result(job_id):
        raise HTTPException(404, detail=f"Resultaat '{job_id}' niet gevonden")


# ---------------------------------------------------------------------------
# GET /validation/*  — OptimaliseRing referentiedata (readonly)
# ---------------------------------------------------------------------------


@app.get("/validation/dijkringen")
def validation_dijkringen() -> list[dict]:
    """Lijst van 103 dijkringen uit de OptimaliseRing 2011 referentiedatabase."""
    from floodopt_api.validation import get_dijkringen

    return get_dijkringen()


@app.get("/validation/reference/{dijkring}/{deel}")
def validation_reference(dijkring: str, deel: float) -> dict:
    """Alle schade- en economische scenario's voor een dijkring/deel combinatie.

    Retourneert:
    - schade_scenarios: Laag / Verwacht / Hoog in M EUR (V₀)
    - gamma_scenarios: alle CPB-groeiscenario's (γ)
    """
    from floodopt_api.validation import get_reference_data

    return get_reference_data(dijkring, deel)


@app.get("/validation/trajectories")
def validation_trajectories(dijkring: str | None = None) -> list[dict]:
    """Trajecten (klimaat_id=1) uit de referentiedatabase, optioneel gefilterd op dijkring."""
    from floodopt_api.validation import get_trajectories

    return get_trajectories(dijkring)


@app.post(
    "/validation/optimize/{dijkring}/{deel}/{traject}",
    status_code=202,
    response_model=OptimizeResponse,
)
def validation_optimize(
    dijkring: str,
    deel: float,
    traject: float,
    repos: Repos,
) -> OptimizeResponse:
    """Start een FloodOpt-optimalisatie op een OptimaliseRing-referentietraject.

    Gebruikt 3 standaard kandidaatmaatregelen (Dh = 0.5 / 1.0 / 1.5 m).
    Risk-parameters zijn de FloodOpt-standaardwaarden.
    """
    from floodopt_api.validation import get_trajectory
    from floodopt_core.io.models import Measure, Scenario, Trajectory
    from floodopt_core.optimization.protocols import ObjectiveType
    from floodopt_core.risk.protocols import RiskParams

    ref = get_trajectory(dijkring, deel, traject)
    if ref is None:
        raise HTTPException(
            404, detail=f"Traject {dijkring}-{deel}-{traject} niet gevonden"
        )

    traj_id = f"ref-{dijkring}-{int(deel)}-{int(traject)}"
    scen_id = f"ref-scen-{dijkring}"

    traj = Trajectory(
        id=traj_id,
        norm=ref["norm_per_jaar"],
        length=10.0,
        p0=ref["p0_per_jaar"],
        alpha=ref["alpha_per_m"],
        base_year=2023,
    )
    scen = Scenario(
        id=scen_id,
        climate="huidig",
        q_design=1000.0,
        h_design=5.0,
        eta=ref["eta_m_per_jaar"],
    )
    repos.save_trajectory(traj)
    repos.save_scenario(scen)

    candidates = [
        Measure(
            id="V1",
            type="dike_reinforcement",
            cost=2_000_000,
            year=2030,
            effect=0.5,
            location="vak-1",
        ),
        Measure(
            id="V2",
            type="dike_reinforcement",
            cost=5_000_000,
            year=2040,
            effect=1.0,
            location="vak-2",
        ),
        Measure(
            id="V3",
            type="dike_reinforcement",
            cost=10_000_000,
            year=2050,
            effect=1.5,
            location="vak-3",
        ),
    ]
    risk_params = RiskParams(
        base_damage=1e9, discount_rate=0.04, gamma=0.02, time_horizon=100
    )

    job_id = str(uuid.uuid4())
    pending = OptimizeResponse(
        job_id=job_id,
        status="pending",
        trajectory_id=traj_id,
        scenario_id=scen_id,
        objective=ObjectiveType.MIN_COST,
        solver="brute_force",
    )
    repos.save_result(pending)

    payload = {
        "trajectory": traj.model_dump(),
        "scenario": scen.model_dump(),
        "candidates": [m.model_dump() for m in candidates],
        "risk_params": risk_params.model_dump(),
        "objective": ObjectiveType.MIN_COST.value,
        "budget": None,
        "solver": "brute_force",
    }
    celery_app.send_task(
        "floodopt_worker.tasks.run_optimization", args=[job_id, payload]
    )
    return pending
