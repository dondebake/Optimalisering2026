"""
Integratie smoke test — stap 1.4.

Verificeert de volledige keten zonder API of database:
    Trajectory -> Scenario -> Measures -> Optimizer -> OptimizationResult

Eisen:
    1. Runt zonder exceptions
    2. BruteForce.solve() == PyomoOptimizer.solve() voor alle objectives
    3. Rekentijd gelogd (BruteForce < 1s voor N=5, T=100)
"""

import math
import time

import pytest

from floodopt_core.io.models import MeasureType
from floodopt_core.optimization import (
    BruteForceOptimizer,
    ObjectiveType,
    PyomoOptimizer,
)
from floodopt_core.physics.simple_dike_overflow import SimpleDikeOverflow
from floodopt_core.risk.simple_risk_calculator import SimpleRiskCalculator

# Importeer de testcase-parameters rechtstreeks uit het smoke-test script.
from scripts.run_smoke_test import (
    BUDGET_MAX_RR,
    CANDIDATES,
    RISK_PARAMS,
    SCENARIO,
    TRAJECTORY,
    main,
    run_both,
)


@pytest.fixture(scope="module")
def optimizers():
    physics = SimpleDikeOverflow()
    risk = SimpleRiskCalculator(physics=physics)
    return BruteForceOptimizer(risk=risk), PyomoOptimizer(risk=risk)


# ---------------------------------------------------------------------------
# Eis 1: runt zonder errors
# ---------------------------------------------------------------------------


def test_smoke_main_exits_zero():
    """Het smoke-test script mag alleen met exitcode 0 eindigen."""
    assert main() == 0


# ---------------------------------------------------------------------------
# Eis 2: BruteForce == Pyomo voor alle objectives
# ---------------------------------------------------------------------------


def test_min_cost_match(optimizers):
    bf, py = optimizers
    run = run_both(bf, py, ObjectiveType.MIN_COST)
    assert run.match, f"MIN_COST afwijking: BF={run.bf_result.selected_ids} != Pyomo={run.py_result.selected_ids}"


def test_min_cost_selects_cheapest_feasible(optimizers):
    """Optimaal voor MIN_COST: {M02, M04} kost 1.5M < M01 alleen (2M)."""
    bf, _ = optimizers
    run = run_both(bf, _, ObjectiveType.MIN_COST)
    assert run.bf_result.selected_ids == {"M02", "M04"}


def test_min_cost_norm_satisfied(optimizers):
    """Geselecteerde maatregelen halen de norm."""
    bf, _ = optimizers
    run = run_both(bf, _, ObjectiveType.MIN_COST)
    delta_h = sum(m.effect for m in run.bf_result.selected_measures)
    pf_after = TRAJECTORY.p0 * math.exp(-TRAJECTORY.alpha * delta_h)
    assert (
        pf_after <= TRAJECTORY.norm
    ), f"Norm niet gehaald: P(t0)={pf_after:.2e} > norm={TRAJECTORY.norm:.2e}"


def test_max_risk_reduction_match(optimizers):
    bf, py = optimizers
    run = run_both(bf, py, ObjectiveType.MAX_RISK_REDUCTION, budget=BUDGET_MAX_RR)
    assert run.match, f"MAX_RISK_REDUCTION afwijking: BF={run.bf_result.selected_ids} != Pyomo={run.py_result.selected_ids}"


def test_max_risk_reduction_within_budget(optimizers):
    """{M02, M03, M04} maximaliseert Δh=0.8m binnen €2M budget."""
    bf, _ = optimizers
    run = run_both(bf, _, ObjectiveType.MAX_RISK_REDUCTION, budget=BUDGET_MAX_RR)
    assert run.bf_result.selected_ids == {"M02", "M03", "M04"}
    assert (
        run.bf_result.investment_npv <= BUDGET_MAX_RR + 1.0
    )  # klein numeriek tolerantie


def test_min_ncw_match(optimizers):
    bf, py = optimizers
    run = run_both(bf, py, ObjectiveType.MIN_NCW)
    assert run.match, f"MIN_NCW afwijking: BF={run.bf_result.selected_ids} != Pyomo={run.py_result.selected_ids}"


def test_min_ncw_decreases_with_measures(optimizers):
    """Totale NCW met optimale maatregelen < NCW zonder maatregelen."""
    bf, _ = optimizers
    run = run_both(bf, _, ObjectiveType.MIN_NCW)
    physics = SimpleDikeOverflow()
    risk = SimpleRiskCalculator(physics=physics)
    baseline = risk.compute(TRAJECTORY, SCENARIO, [], RISK_PARAMS).ncw
    assert (
        run.bf_result.total_ncw < baseline
    ), f"NCW daalt niet: {run.bf_result.total_ncw:.0f} >= baseline {baseline:.0f}"


# ---------------------------------------------------------------------------
# Eis 3: rekentijd
# ---------------------------------------------------------------------------


def test_brute_force_timing_n5(optimizers):
    """BruteForce N=5, T=100 moet binnen 1 seconde klaar zijn."""
    bf, _ = optimizers
    t0 = time.perf_counter()
    bf.solve(TRAJECTORY, SCENARIO, CANDIDATES, RISK_PARAMS, ObjectiveType.MIN_NCW)
    elapsed = time.perf_counter() - t0
    assert elapsed < 1.0, f"BruteForce te langzaam: {elapsed:.2f}s (grens: 1.0s)"


# ---------------------------------------------------------------------------
# Integriteitschecks
# ---------------------------------------------------------------------------


def test_no_physics_in_optimizer():
    """Optimizer-bronbestanden bevatten geen eigenhandig geschreven faalkansformule."""
    import inspect
    from floodopt_core.optimization import brute_force, pyomo_optimizer

    for module in (brute_force, pyomo_optimizer):
        src = inspect.getsource(module)
        assert (
            "math.exp" not in src or "discount" in src or "investment" in src
        ), f"{module.__name__} lijkt een faalkansformule te bevatten"


def test_all_measures_are_candidates():
    """Alle kandidaat-IDs zijn uniek."""
    ids = [m.id for m in CANDIDATES]
    assert len(ids) == len(set(ids)), "Dubbele maatregel-IDs gevonden"


def test_measure_types_valid():
    for m in CANDIDATES:
        assert m.type == MeasureType.DIKE_REINFORCEMENT
        assert m.effect > 0
        assert m.cost > 0
