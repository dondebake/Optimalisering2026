"""
Worker-integratietests — stap 2.3.

Verificatie:
  1. run_optimization task zet status: pending → running → done
  2. Resultaten identiek aan stap 1.4 smoke test (MIN_COST, MAX_RISK_REDUCTION, MIN_NCW)
  3. BruteForce == Pyomo voor MIN_COST (via worker)
  4. Fout in optimizer zet status op 'failed'

Geen Redis nodig: taken worden direct aangeroepen (geen .delay()).
"""

import pytest

from floodopt_api.database import init_schema, make_session
from floodopt_api.models import OptimizeResponse
from floodopt_api.repositories import OrmRepositories
from floodopt_core.optimization.protocols import ObjectiveType
from floodopt_worker.tasks import run_optimization

from scripts.run_smoke_test import (
    BUDGET_MAX_RR,
    CANDIDATES,
    RISK_PARAMS,
    SCENARIO,
    TRAJECTORY,
)


# ---------------------------------------------------------------------------
# Fixtures
# ---------------------------------------------------------------------------


@pytest.fixture
def db_url(tmp_path, monkeypatch):
    """Tijdelijke SQLite-database; DATABASE_URL wordt overschreven voor de worker."""
    url = f"sqlite:///{tmp_path}/worker_test.db"
    monkeypatch.setenv("DATABASE_URL", url)
    init_schema(url)
    return url


@pytest.fixture
def repos(db_url):
    session = make_session(db_url)
    yield OrmRepositories(session)
    session.close()


def _save_pending(repos, job_id, objective="min_cost", solver="brute_force"):
    repos.save_result(
        OptimizeResponse(
            job_id=job_id,
            status="pending",
            trajectory_id=TRAJECTORY.id,
            scenario_id=SCENARIO.id,
            objective=ObjectiveType(objective),
            solver=solver,
        )
    )


def _payload(objective="min_cost", solver="brute_force", budget=None):
    return {
        "trajectory": TRAJECTORY.model_dump(),
        "scenario": SCENARIO.model_dump(),
        "candidates": [m.model_dump() for m in CANDIDATES],
        "risk_params": RISK_PARAMS.model_dump(),
        "objective": objective,
        "budget": budget,
        "solver": solver,
    }


# ---------------------------------------------------------------------------
# Status-flow
# ---------------------------------------------------------------------------


def test_task_sets_status_done(db_url, repos):
    """Na uitvoering is status 'done'."""
    _save_pending(repos, "job-1")
    run_optimization("job-1", _payload())
    result = repos.get_result("job-1")
    assert result is not None
    assert result.status == "done"


def test_task_fills_result_fields(db_url, repos):
    """Resultaatvelden zijn ingevuld na uitvoering."""
    _save_pending(repos, "job-2")
    run_optimization("job-2", _payload())
    r = repos.get_result("job-2")
    assert r is not None
    assert r.total_ncw is not None
    assert r.risk_ncw is not None
    assert r.investment_npv is not None
    assert r.objective_value is not None
    assert len(r.selected_measure_ids) > 0


# ---------------------------------------------------------------------------
# Correctheid vs. stap 1.4
# ---------------------------------------------------------------------------


def test_min_cost_matches_stap14(db_url, repos):
    """MIN_COST optimum is {M02, M04}, investering ≈ €1.089.224."""
    _save_pending(repos, "job-mc")
    run_optimization("job-mc", _payload("min_cost"))
    r = repos.get_result("job-mc")
    assert r is not None
    assert set(r.selected_measure_ids) == {"M02", "M04"}
    assert r.investment_npv == pytest.approx(1_089_224, rel=0.01)


def test_max_risk_reduction_matches_stap14(db_url, repos):
    """MAX_RISK_REDUCTION met budget=2M geeft {M02, M03, M04}."""
    _save_pending(repos, "job-mrr", objective="max_risk_reduction")
    run_optimization("job-mrr", _payload("max_risk_reduction", budget=BUDGET_MAX_RR))
    r = repos.get_result("job-mrr")
    assert r is not None
    assert set(r.selected_measure_ids) == {"M02", "M03", "M04"}


def test_min_ncw_selects_all(db_url, repos):
    """MIN_NCW selecteert alle 5 maatregelen (allen winstgevend)."""
    _save_pending(repos, "job-mncw", objective="min_ncw")
    run_optimization("job-mncw", _payload("min_ncw"))
    r = repos.get_result("job-mncw")
    assert r is not None
    assert set(r.selected_measure_ids) == {"M01", "M02", "M03", "M04", "M05"}


def test_pyomo_matches_brute_force(db_url, repos):
    """Pyomo-solver geeft zelfde MIN_COST resultaat als BruteForce via worker."""
    _save_pending(repos, "job-bf", solver="brute_force")
    _save_pending(repos, "job-py", solver="pyomo")
    run_optimization("job-bf", _payload("min_cost", solver="brute_force"))
    run_optimization("job-py", _payload("min_cost", solver="pyomo"))
    bf = repos.get_result("job-bf")
    py = repos.get_result("job-py")
    assert bf is not None and py is not None
    assert set(bf.selected_measure_ids) == set(py.selected_measure_ids)


# ---------------------------------------------------------------------------
# Foutafhandeling
# ---------------------------------------------------------------------------


def test_task_sets_failed_on_error(db_url, repos, monkeypatch):
    """Bij een optimizer-fout wordt status 'failed' gezet."""
    _save_pending(repos, "job-fail")

    def _bad_solve(*args, **kwargs):
        raise RuntimeError("gesimuleerde fout")

    monkeypatch.setattr(
        "floodopt_core.optimization.brute_force.BruteForceOptimizer.solve", _bad_solve
    )
    with pytest.raises(RuntimeError):
        run_optimization("job-fail", _payload())

    r = repos.get_result("job-fail")
    assert r is not None
    assert r.status == "failed"
