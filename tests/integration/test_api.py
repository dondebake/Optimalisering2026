"""
API-integratietests — stap 2.1.

Verificatie:
  1. Swagger UI beschikbaar op /docs
  2. POST /scenarios, POST /trajectories, POST /optimize, GET /results/{id} werken
  3. POST /optimize geeft zelfde resultaat als stap 1.4 smoke test
  4. Geen business logic in de API-laag (gecontroleerd structureel)
  5. 404-afhandeling voor ontbrekende resources
"""

import pytest
from fastapi.testclient import TestClient

from floodopt_api.main import _memory_repos, app

# Referentiedata uit stap 1.4 smoke test
from scripts.run_smoke_test import (
    BUDGET_MAX_RR,
    CANDIDATES,
    RISK_PARAMS,
    SCENARIO,
    TRAJECTORY,
)

client = TestClient(app)


@pytest.fixture(autouse=True)
def reset_store():
    """Leeg de in-memory repos voor elke test."""
    _memory_repos.clear()
    yield
    _memory_repos.clear()


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
    # Alle 4 endpoints aanwezig
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
# 4. POST /optimize — correctheid vs. stap 1.4
# ---------------------------------------------------------------------------


def test_optimize_requires_existing_trajectory():
    _post_scenario()
    # Geen trajectory → 404
    resp = _post_optimize()
    assert resp.status_code == 404
    assert "Traject" in resp.json()["detail"]


def test_optimize_requires_existing_scenario():
    _post_trajectory()
    # Geen scenario → 404
    resp = _post_optimize()
    assert resp.status_code == 404
    assert "Scenario" in resp.json()["detail"]


def test_optimize_min_cost_returns_201():
    _post_scenario()
    _post_trajectory()
    resp = _post_optimize("min_cost")
    assert resp.status_code == 201


def test_optimize_min_cost_matches_stap14():
    """POST /optimize MIN_COST moet zelfde resultaat geven als stap 1.4 smoke test."""
    _post_scenario()
    _post_trajectory()
    resp = _post_optimize("min_cost")
    assert resp.status_code == 201
    data = resp.json()
    # Stap 1.4: optimum is {M02, M04}
    assert set(data["selected_measure_ids"]) == {"M02", "M04"}
    assert data["investment_npv"] == pytest.approx(1_089_224, rel=0.01)


def test_optimize_max_risk_reduction_matches_stap14():
    """POST /optimize MAX_RISK_REDUCTION met budget=2M moet {M02, M03, M04} geven."""
    _post_scenario()
    _post_trajectory()
    resp = _post_optimize("max_risk_reduction", budget=BUDGET_MAX_RR)
    assert resp.status_code == 201
    data = resp.json()
    assert set(data["selected_measure_ids"]) == {"M02", "M03", "M04"}


def test_optimize_min_ncw_selects_all():
    """POST /optimize MIN_NCW selecteert alle 5 maatregelen (allen winstgevend)."""
    _post_scenario()
    _post_trajectory()
    resp = _post_optimize("min_ncw")
    assert resp.status_code == 201
    data = resp.json()
    assert set(data["selected_measure_ids"]) == {"M01", "M02", "M03", "M04", "M05"}


def test_optimize_pyomo_solver():
    """Pyomo-solver geeft zelfde resultaat als BruteForce voor MIN_COST."""
    _post_scenario()
    _post_trajectory()
    bf_data = _post_optimize("min_cost", solver="brute_force").json()
    py_data = _post_optimize("min_cost", solver="pyomo").json()
    assert set(bf_data["selected_measure_ids"]) == set(py_data["selected_measure_ids"])


def test_optimize_response_contains_job_id():
    _post_scenario()
    _post_trajectory()
    resp = _post_optimize("min_cost")
    data = resp.json()
    assert "job_id" in data
    assert len(data["job_id"]) == 36  # UUID-formaat


def test_optimize_response_status_completed():
    _post_scenario()
    _post_trajectory()
    data = _post_optimize("min_cost").json()
    assert data["status"] == "completed"


# ---------------------------------------------------------------------------
# 5. GET /results/{job_id}
# ---------------------------------------------------------------------------


def test_get_results_returns_same_as_optimize():
    _post_scenario()
    _post_trajectory()
    opt_data = _post_optimize("min_cost").json()
    job_id = opt_data["job_id"]

    result_data = client.get(f"/results/{job_id}").json()
    assert result_data["job_id"] == job_id
    assert result_data["selected_measure_ids"] == opt_data["selected_measure_ids"]
    assert result_data["total_ncw"] == opt_data["total_ncw"]


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
