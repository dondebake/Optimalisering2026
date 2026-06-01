"""
API-integratietests — stap 2.1 / 2.2 / 2.3.

Verificatie:
  1. Swagger UI beschikbaar op /docs
  2. POST /scenarios, POST /trajectories, POST /optimize, GET /results/{id} werken
  3. POST /optimize retourneert 202 + status 'pending' (async queue)
  4. Geen business logic in de API-laag (gecontroleerd structureel)
  5. 404-afhandeling voor ontbrekende resources

Correctheid van de optimalisatieresultaten wordt getest in test_worker.py.
"""

from unittest.mock import patch

import pytest
from fastapi.testclient import TestClient

from floodopt_api.main import app, get_repositories
from floodopt_api.repositories import MemoryRepositories

# Referentiedata uit stap 1.4 smoke test
from scripts.run_smoke_test import (
    BUDGET_MAX_RR,
    CANDIDATES,
    RISK_PARAMS,
    SCENARIO,
    TRAJECTORY,
)

# Gedeelde in-memory repos voor de test-sessie (via FastAPI dependency override)
_test_repos = MemoryRepositories()


def _override():
    yield _test_repos


app.dependency_overrides[get_repositories] = _override
client = TestClient(app)


@pytest.fixture(autouse=True)
def reset_store():
    """Leeg de in-memory repos voor elke test."""
    _test_repos.clear()
    yield
    _test_repos.clear()


# ---------------------------------------------------------------------------
# Hulpfuncties
# ---------------------------------------------------------------------------


def _post_scenario():
    return client.post(
        "/scenarios",
        content=SCENARIO.model_dump_json(),
        headers={"Content-Type": "application/json"},
    )


def _post_trajectory():
    return client.post(
        "/trajectories",
        content=TRAJECTORY.model_dump_json(),
        headers={"Content-Type": "application/json"},
    )


def _post_optimize(
    objective: str = "min_cost",
    solver: str = "brute_force",
    budget: float | None = None,
):
    payload = {
        "trajectory_id": TRAJECTORY.id,
        "scenario_id": SCENARIO.id,
        "candidates": [m.model_dump() for m in CANDIDATES],
        "risk_params": RISK_PARAMS.model_dump(),
        "objective": objective,
        "solver": solver,
        "budget": budget,
    }
    return client.post("/optimize", json=payload)


# ---------------------------------------------------------------------------
# 1. Swagger UI
# ---------------------------------------------------------------------------


def test_swagger_ui_available():
    """GET /docs geeft 200 terug — Swagger UI is beschikbaar."""
    resp = client.get("/docs")
    assert resp.status_code == 200


def test_openapi_json_available():
    resp = client.get("/openapi.json")
    assert resp.status_code == 200
    schema = resp.json()
    assert "FloodOpt" in schema["info"]["title"]
    paths = schema["paths"]
    assert "/scenarios" in paths
    assert "/trajectories" in paths
    assert "/optimize" in paths
    assert "/results/{job_id}" in paths


# ---------------------------------------------------------------------------
# 2. POST /scenarios
# ---------------------------------------------------------------------------


def test_post_scenario_returns_201():
    resp = _post_scenario()
    assert resp.status_code == 201


def test_post_scenario_roundtrip():
    _post_scenario()
    resp = client.get(f"/scenarios/{SCENARIO.id}")
    assert resp.status_code == 200
    data = resp.json()
    assert data["id"] == SCENARIO.id
    assert data["climate"] == SCENARIO.climate
    assert data["eta"] == SCENARIO.eta


def test_get_scenario_not_found():
    resp = client.get("/scenarios/ONBEKEND")
    assert resp.status_code == 404


# ---------------------------------------------------------------------------
# 3. POST /trajectories
# ---------------------------------------------------------------------------


def test_post_trajectory_returns_201():
    resp = _post_trajectory()
    assert resp.status_code == 201


def test_post_trajectory_roundtrip():
    _post_trajectory()
    resp = client.get(f"/trajectories/{TRAJECTORY.id}")
    assert resp.status_code == 200
    data = resp.json()
    assert data["id"] == TRAJECTORY.id
    assert data["p0"] == TRAJECTORY.p0
    assert data["alpha"] == TRAJECTORY.alpha


def test_get_trajectory_not_found():
    resp = client.get("/trajectories/ONBEKEND")
    assert resp.status_code == 404


# ---------------------------------------------------------------------------
# 4. POST /optimize — async gedrag (stap 2.3)
# ---------------------------------------------------------------------------


def test_optimize_requires_existing_trajectory():
    _post_scenario()
    resp = _post_optimize()
    assert resp.status_code == 404
    assert "Traject" in resp.json()["detail"]


def test_optimize_requires_existing_scenario():
    _post_trajectory()
    resp = _post_optimize()
    assert resp.status_code == 404
    assert "Scenario" in resp.json()["detail"]


def test_optimize_returns_202():
    """POST /optimize retourneert 202 Accepted (taak ingepland, niet klaar)."""
    _post_scenario()
    _post_trajectory()
    with patch("floodopt_api.main.celery_app.send_task"):
        resp = _post_optimize("min_cost")
    assert resp.status_code == 202


def test_optimize_response_status_pending():
    """Directe response bevat status 'pending' — berekening loopt nog."""
    _post_scenario()
    _post_trajectory()
    with patch("floodopt_api.main.celery_app.send_task"):
        data = _post_optimize("min_cost").json()
    assert data["status"] == "pending"


def test_optimize_response_contains_job_id():
    _post_scenario()
    _post_trajectory()
    with patch("floodopt_api.main.celery_app.send_task"):
        data = _post_optimize("min_cost").json()
    assert "job_id" in data
    assert len(data["job_id"]) == 36  # UUID-formaat


def test_optimize_response_result_fields_none_while_pending():
    """Resultaatvelden zijn None zolang de worker nog niet klaar is."""
    _post_scenario()
    _post_trajectory()
    with patch("floodopt_api.main.celery_app.send_task"):
        data = _post_optimize("min_cost").json()
    assert data["total_ncw"] is None
    assert data["selected_measure_ids"] == []


def test_optimize_dispatches_celery_task():
    """POST /optimize moet de Celery-taak versturen met job_id en payload."""
    _post_scenario()
    _post_trajectory()
    with patch("floodopt_api.main.celery_app.send_task") as mock_send:
        resp = _post_optimize("min_cost")
    mock_send.assert_called_once()
    task_name = mock_send.call_args[0][0]
    assert task_name == "floodopt_worker.tasks.run_optimization"
    assert resp.json()["job_id"] in str(mock_send.call_args)


def test_optimize_multiple_objectives_all_return_202():
    """MIN_COST, MAX_RISK_REDUCTION en MIN_NCW geven allemaal 202."""
    _post_scenario()
    _post_trajectory()
    with patch("floodopt_api.main.celery_app.send_task"):
        for obj in ("min_cost", "max_risk_reduction", "min_ncw"):
            budget = BUDGET_MAX_RR if obj == "max_risk_reduction" else None
            resp = _post_optimize(obj, budget=budget)
            assert resp.status_code == 202, f"{obj} gaf {resp.status_code}"


# ---------------------------------------------------------------------------
# 5. GET /results/{job_id}
# ---------------------------------------------------------------------------


def test_get_results_returns_pending_immediately():
    """GET /results na POST geeft dezelfde pending job terug."""
    _post_scenario()
    _post_trajectory()
    with patch("floodopt_api.main.celery_app.send_task"):
        opt_data = _post_optimize("min_cost").json()
    job_id = opt_data["job_id"]

    result_data = client.get(f"/results/{job_id}").json()
    assert result_data["job_id"] == job_id
    assert result_data["status"] == "pending"
    assert result_data["trajectory_id"] == TRAJECTORY.id
    assert result_data["scenario_id"] == SCENARIO.id


def test_get_results_not_found():
    resp = client.get("/results/onbekend-job-id")
    assert resp.status_code == 404


# ---------------------------------------------------------------------------
# 6. API-laag bevat geen business logic
# ---------------------------------------------------------------------------


def test_no_physics_formulas_in_api():
    """De API-laag (main.py, models.py) importeert geen faalkansformules."""
    import inspect

    from floodopt_api import main, models

    for module in (main, models):
        src = inspect.getsource(module)
        assert (
            "math.exp" not in src
        ), f"{module.__name__} bevat math.exp — verplaats naar floodopt-core"
        assert (
            "alpha" not in src or "import" in src.split("alpha")[0].splitlines()[-1]
        ), f"{module.__name__} bevat mogelijk een faalkansparameter"
