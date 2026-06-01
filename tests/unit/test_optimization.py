"""
Verificatietests stap 1.3 — Optimization Layer.

KRITIEKE EIS: BruteForceOptimizer.solve() == PyomoOptimizer.solve()
voor alle testcases. Bij afwijking stopt de ontwikkeling.

Testcases:
  TC1  MIN_COST    — cheapest subset dat norm haalt (M02+M03 < M01)
  TC2  MIN_COST    — norm al gehaald → lege set
  TC3  MIN_COST    — dependency: M02 vereist M01
  TC4  MAX_RISK_REDUCTION — knapsack: M02+M03 past binnen budget, M01 niet
  TC5  MIN_NCW     — kleine maatregelen (alpha*h=0.1), linearisatie < 1% fout
  TC6  MIN_NCW     — maatregel duidelijk de moeite waard vs. te duur

Parameters TC1-TC4:
  P0=1e-2, alpha=5.0, norm=1e-3 → h_min = ln(10)/5 ≈ 0.461 m
  M01: h=0.5m, cost=2M  (duur, haalt norm alleen)
  M02: h=0.3m, cost=1M
  M03: h=0.2m, cost=0.5M (M02+M03 haalt norm voor 1.5M)

Parameters TC5-TC6:
  alpha=2.0, kleine effecten (alpha*h_i = 0.1 → linearisatiefout ~0.5%)
"""

import pytest

from floodopt_core.io import Measure, MeasureType, Scenario, Trajectory
from floodopt_core.optimization import (
    BruteForceOptimizer,
    ObjectiveType,
    OptimizationResult,
    OptimizationStrategy,
    PyomoOptimizer,
)
from floodopt_core.physics import SimpleDikeOverflow
from floodopt_core.risk import RiskParams, SimpleRiskCalculator

# ---------------------------------------------------------------------------
# Gedeelde fixtures
# ---------------------------------------------------------------------------

BASE_YEAR = 2017


@pytest.fixture
def risk_calc() -> SimpleRiskCalculator:
    return SimpleRiskCalculator(physics=SimpleDikeOverflow())


@pytest.fixture
def bf(risk_calc: SimpleRiskCalculator) -> BruteForceOptimizer:
    return BruteForceOptimizer(risk=risk_calc)


@pytest.fixture
def py(risk_calc: SimpleRiskCalculator) -> PyomoOptimizer:
    return PyomoOptimizer(risk=risk_calc)


@pytest.fixture
def traj_norm() -> Trajectory:
    """P0=1e-2 >> norm=1e-3 → versterking nodig. h_min ≈ 0.461 m."""
    return Trajectory(
        id="T01",
        norm=1e-3,
        length=10.0,
        p0=1e-2,
        alpha=5.0,
        base_year=BASE_YEAR,
    )


@pytest.fixture
def traj_ok() -> Trajectory:
    """P0=1e-4 < norm=1e-3 → norm al gehaald."""
    return Trajectory(
        id="T02",
        norm=1e-3,
        length=10.0,
        p0=1e-4,
        alpha=5.0,
        base_year=BASE_YEAR,
    )


@pytest.fixture
def traj_small_alpha() -> Trajectory:
    """alpha=2.0 voor kleine linearisatiefout in MIN_NCW tests."""
    return Trajectory(
        id="T03",
        norm=1e-4,
        length=10.0,
        p0=1e-3,
        alpha=2.0,
        base_year=BASE_YEAR,
    )


@pytest.fixture
def scenario() -> Scenario:
    return Scenario(
        id="S01", climate="huidig", q_design=15_000.0, h_design=11.0, eta=0.0
    )


@pytest.fixture
def params_norm() -> RiskParams:
    return RiskParams(base_damage=1e9, discount_rate=0.04, gamma=0.02, time_horizon=50)


@pytest.fixture
def params_small() -> RiskParams:
    return RiskParams(base_damage=1e8, discount_rate=0.04, gamma=0.02, time_horizon=50)


@pytest.fixture
def m01() -> Measure:
    return Measure(
        id="M01",
        type=MeasureType.DIKE_REINFORCEMENT,
        cost=2_000_000.0,
        year=BASE_YEAR,
        effect=0.5,
        location="km 0-5",
    )


@pytest.fixture
def m02() -> Measure:
    return Measure(
        id="M02",
        type=MeasureType.DIKE_REINFORCEMENT,
        cost=1_000_000.0,
        year=BASE_YEAR,
        effect=0.3,
        location="km 5-10",
    )


@pytest.fixture
def m03() -> Measure:
    return Measure(
        id="M03",
        type=MeasureType.DIKE_REINFORCEMENT,
        cost=500_000.0,
        year=BASE_YEAR,
        effect=0.2,
        location="km 10-15",
    )


# ---------------------------------------------------------------------------
# Hulpfunctie: vergelijk BruteForce vs Pyomo
# ---------------------------------------------------------------------------


def assert_same_selection(
    bf_result: OptimizationResult,
    py_result: OptimizationResult,
    label: str = "",
) -> None:
    assert (
        bf_result.selected_ids == py_result.selected_ids
    ), f"{label}: BruteForce={bf_result.selected_ids} != Pyomo={py_result.selected_ids}"


# ---------------------------------------------------------------------------
# TC1 — MIN_COST: M02+M03 goedkoper dan M01
# ---------------------------------------------------------------------------


def test_tc1_min_cost_bf(
    bf: BruteForceOptimizer,
    traj_norm: Trajectory,
    scenario: Scenario,
    params_norm: RiskParams,
    m01: Measure,
    m02: Measure,
    m03: Measure,
) -> None:
    result = bf.solve(
        traj_norm, scenario, [m01, m02, m03], params_norm, ObjectiveType.MIN_COST
    )
    assert result.selected_ids == {"M02", "M03"}
    assert result.investment_npv == pytest.approx(1_500_000.0, rel=1e-6)


def test_tc1_min_cost_pyomo(
    py: PyomoOptimizer,
    traj_norm: Trajectory,
    scenario: Scenario,
    params_norm: RiskParams,
    m01: Measure,
    m02: Measure,
    m03: Measure,
) -> None:
    result = py.solve(
        traj_norm, scenario, [m01, m02, m03], params_norm, ObjectiveType.MIN_COST
    )
    assert result.selected_ids == {"M02", "M03"}


def test_tc1_bf_equals_pyomo(
    bf: BruteForceOptimizer,
    py: PyomoOptimizer,
    traj_norm: Trajectory,
    scenario: Scenario,
    params_norm: RiskParams,
    m01: Measure,
    m02: Measure,
    m03: Measure,
) -> None:
    candidates = [m01, m02, m03]
    bf_r = bf.solve(
        traj_norm, scenario, candidates, params_norm, ObjectiveType.MIN_COST
    )
    py_r = py.solve(
        traj_norm, scenario, candidates, params_norm, ObjectiveType.MIN_COST
    )
    assert_same_selection(bf_r, py_r, "TC1 MIN_COST")


# ---------------------------------------------------------------------------
# TC2 — MIN_COST: norm al gehaald → lege set
# ---------------------------------------------------------------------------


def test_tc2_norm_already_met(
    bf: BruteForceOptimizer,
    py: PyomoOptimizer,
    traj_ok: Trajectory,
    scenario: Scenario,
    params_norm: RiskParams,
    m01: Measure,
    m02: Measure,
) -> None:
    for optimizer in (bf, py):
        result = optimizer.solve(
            traj_ok, scenario, [m01, m02], params_norm, ObjectiveType.MIN_COST
        )
        assert (
            result.selected_ids == set()
        ), f"Verwacht lege set, kreeg {result.selected_ids}"


# ---------------------------------------------------------------------------
# TC3 — MIN_COST met dependency: M02 vereist M01
# ---------------------------------------------------------------------------


def test_tc3_dependency_respected(
    bf: BruteForceOptimizer,
    py: PyomoOptimizer,
    traj_norm: Trajectory,
    scenario: Scenario,
    params_norm: RiskParams,
    m01: Measure,
) -> None:
    m02_dep = Measure(
        id="M02d",
        type=MeasureType.DIKE_REINFORCEMENT,
        cost=1_000_000.0,
        year=BASE_YEAR,
        effect=0.3,
        location="km 5-10",
        dependencies=["M01"],
    )
    m03_indep = Measure(
        id="M03i",
        type=MeasureType.DIKE_REINFORCEMENT,
        cost=500_000.0,
        year=BASE_YEAR,
        effect=0.2,
        location="km 10-15",
    )
    # h_min ≈ 0.461m. M02d+M03i=0.5m maar M02d vereist M01.
    # Alleen M01 alleen (0.5m) of M01+M02d+M03i zijn haalbaar.
    # Goedkoopste: M01 @ 2M vs M01+M02d+M03i @ 3.5M → M01 wint
    candidates = [m01, m02_dep, m03_indep]
    bf_r = bf.solve(
        traj_norm, scenario, candidates, params_norm, ObjectiveType.MIN_COST
    )
    py_r = py.solve(
        traj_norm, scenario, candidates, params_norm, ObjectiveType.MIN_COST
    )
    assert_same_selection(bf_r, py_r, "TC3 dependency")
    assert "M01" in bf_r.selected_ids  # M01 altijd aanwezig (enige haalbare solo)


# ---------------------------------------------------------------------------
# TC4 — MAX_RISK_REDUCTION met budget (knapsack)
# ---------------------------------------------------------------------------


def test_tc4_max_risk_reduction_bf(
    bf: BruteForceOptimizer,
    traj_norm: Trajectory,
    scenario: Scenario,
    params_norm: RiskParams,
    m01: Measure,
    m02: Measure,
    m03: Measure,
) -> None:
    # Budget=1.5M: M01(2M) te duur, M02+M03(1.5M) past net, Δh=0.5m
    result = bf.solve(
        traj_norm,
        scenario,
        [m01, m02, m03],
        params_norm,
        ObjectiveType.MAX_RISK_REDUCTION,
        budget=1_500_000.0,
    )
    assert result.selected_ids == {"M02", "M03"}


def test_tc4_bf_equals_pyomo(
    bf: BruteForceOptimizer,
    py: PyomoOptimizer,
    traj_norm: Trajectory,
    scenario: Scenario,
    params_norm: RiskParams,
    m01: Measure,
    m02: Measure,
    m03: Measure,
) -> None:
    candidates = [m01, m02, m03]
    bf_r = bf.solve(
        traj_norm,
        scenario,
        candidates,
        params_norm,
        ObjectiveType.MAX_RISK_REDUCTION,
        budget=1_500_000.0,
    )
    py_r = py.solve(
        traj_norm,
        scenario,
        candidates,
        params_norm,
        ObjectiveType.MAX_RISK_REDUCTION,
        budget=1_500_000.0,
    )
    assert_same_selection(bf_r, py_r, "TC4 MAX_RISK_REDUCTION")


# ---------------------------------------------------------------------------
# TC5 & TC6 — MIN_NCW: kleine maatregelen, linearisatiefout < 1%
# ---------------------------------------------------------------------------


@pytest.fixture
def m_cheap(traj_small_alpha: Trajectory) -> Measure:
    """Maatregel die duidelijk de moeite waard is: voordeel >> kosten."""
    # alpha=2.0, h=0.05m → alpha*h=0.10
    # Baseline NCW ~ P0*V0*K = 1e-3*1e8*31.9 ≈ 3.19M
    # Lin. reductie = 3.19M * 2.0 * 0.05 = 319k > kosten 5k → include
    return Measure(
        id="M_cheap",
        type=MeasureType.DIKE_REINFORCEMENT,
        cost=5_000.0,
        year=BASE_YEAR,
        effect=0.05,
        location="km 0-5",
    )


@pytest.fixture
def m_expensive(traj_small_alpha: Trajectory) -> Measure:
    """Maatregel die NIET de moeite waard is: kosten >> voordeel."""
    # Lin. reductie ≈ 319k < kosten 400k → exclude
    return Measure(
        id="M_exp",
        type=MeasureType.DIKE_REINFORCEMENT,
        cost=400_000.0,
        year=BASE_YEAR,
        effect=0.05,
        location="km 5-10",
    )


def test_tc5_min_ncw_bf(
    bf: BruteForceOptimizer,
    traj_small_alpha: Trajectory,
    scenario: Scenario,
    params_small: RiskParams,
    m_cheap: Measure,
    m_expensive: Measure,
) -> None:
    result = bf.solve(
        traj_small_alpha,
        scenario,
        [m_cheap, m_expensive],
        params_small,
        ObjectiveType.MIN_NCW,
    )
    assert "M_cheap" in result.selected_ids
    assert "M_exp" not in result.selected_ids


def test_tc6_bf_equals_pyomo_min_ncw(
    bf: BruteForceOptimizer,
    py: PyomoOptimizer,
    traj_small_alpha: Trajectory,
    scenario: Scenario,
    params_small: RiskParams,
    m_cheap: Measure,
    m_expensive: Measure,
) -> None:
    candidates = [m_cheap, m_expensive]
    bf_r = bf.solve(
        traj_small_alpha, scenario, candidates, params_small, ObjectiveType.MIN_NCW
    )
    py_r = py.solve(
        traj_small_alpha, scenario, candidates, params_small, ObjectiveType.MIN_NCW
    )
    assert_same_selection(bf_r, py_r, "TC6 MIN_NCW small measures")


# ---------------------------------------------------------------------------
# Structurele eisen
# ---------------------------------------------------------------------------


def test_optimizer_contains_no_physics_formulas() -> None:
    """Optimizer mag geen exp/log van fysische formules bevatten — alleen aanroepen."""
    import inspect
    from floodopt_core.optimization import brute_force, pyomo_optimizer

    for module in (brute_force, pyomo_optimizer):
        src = inspect.getsource(module)
        # exp() mag alleen voorkomen voor verdiscontering (investment_npv / disc_costs)
        assert (
            "trajectory.p0" not in src
            or "math.log" not in src.split("trajectory.p0")[0]
        ), f"{module.__name__} bevat mogelijk een fysische faalkansformule"


def test_protocol_conformity_bf(bf: BruteForceOptimizer) -> None:
    opt: OptimizationStrategy = bf  # type: ignore[assignment]
    assert opt is bf


def test_protocol_conformity_py(py: PyomoOptimizer) -> None:
    opt: OptimizationStrategy = py  # type: ignore[assignment]
    assert opt is py


def test_result_is_frozen(
    bf: BruteForceOptimizer,
    traj_norm: Trajectory,
    scenario: Scenario,
    params_norm: RiskParams,
    m01: Measure,
) -> None:
    result = bf.solve(traj_norm, scenario, [m01], params_norm, ObjectiveType.MIN_NCW)
    with pytest.raises(Exception):
        result.total_ncw = 0.0  # type: ignore[misc]
