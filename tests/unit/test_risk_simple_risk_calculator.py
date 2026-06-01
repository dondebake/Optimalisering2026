"""
Handmatige testcases voor SimpleRiskCalculator.

Formules:
    S(s) = P(s) × V(s)
    P(s) = P0 · exp(α·η·s) · exp(−α·Δh)
    V(s) = V0 · exp(γ·s)
    NCW  = Σ_{s=0}^{T-1} S(s) · exp(−δ·s)

Basisparameters (eta=0 voor analytisch verifieerbare resultaten):
    P0=1e-3, alpha=5.0, eta=0.0, base_year=2017
    V0=1e9 €, gamma=0.02, delta=0.04, T=50
"""

import math

import pytest

from floodopt_core.io import Measure, MeasureType, Scenario, Trajectory
from floodopt_core.physics import SimpleDikeOverflow
from floodopt_core.risk import RiskCalculator, RiskParams, SimpleRiskCalculator

P0 = 1e-3
ALPHA = 5.0
BASE_YEAR = 2017
V0 = 1_000_000_000.0  # 1 miljard €
GAMMA = 0.02
DELTA = 0.04
T = 50


@pytest.fixture
def trajectory() -> Trajectory:
    return Trajectory(
        id="T_risk",
        norm=1e-4,
        length=10.0,
        p0=P0,
        alpha=ALPHA,
        base_year=BASE_YEAR,
    )


@pytest.fixture
def scenario_no_climate() -> Scenario:
    """eta=0 maakt P(t) tijdsonafhankelijk → analytisch verifieerbaar."""
    return Scenario(
        id="S_huidig", climate="huidig", q_design=15_000.0, h_design=11.0, eta=0.0
    )


@pytest.fixture
def scenario_climate() -> Scenario:
    return Scenario(
        id="S_Wplus", climate="W+", q_design=15_000.0, h_design=11.0, eta=0.003
    )


@pytest.fixture
def params() -> RiskParams:
    return RiskParams(base_damage=V0, discount_rate=DELTA, gamma=GAMMA, time_horizon=T)


@pytest.fixture
def measure_1m() -> Measure:
    return Measure(
        id="M01",
        type=MeasureType.DIKE_REINFORCEMENT,
        cost=2_000_000.0,
        year=2020,
        effect=1.0,
        location="km 0-10",
    )


@pytest.fixture
def calculator() -> SimpleRiskCalculator:
    return SimpleRiskCalculator(physics=SimpleDikeOverflow())


# --- expected_damage_t0 ---


def test_expected_damage_no_measures(
    calculator: SimpleRiskCalculator,
    trajectory: Trajectory,
    scenario_no_climate: Scenario,
    params: RiskParams,
) -> None:
    """Zonder maatregelen: verwachte schade t0 = P0 × V0."""
    result = calculator.compute(trajectory, scenario_no_climate, [], params)
    expected = P0 * V0  # 1e-3 × 1e9 = 1e6 €/jaar
    assert math.isclose(result.expected_damage_t0, expected, rel_tol=1e-9)


def test_expected_damage_with_measure(
    calculator: SimpleRiskCalculator,
    trajectory: Trajectory,
    scenario_no_climate: Scenario,
    params: RiskParams,
    measure_1m: Measure,
) -> None:
    """Met 1m verhoging: verwachte schade t0 = P0 · exp(-α·1) · V0."""
    result = calculator.compute(trajectory, scenario_no_climate, [measure_1m], params)
    expected = P0 * math.exp(-ALPHA * 1.0) * V0
    assert math.isclose(result.expected_damage_t0, expected, rel_tol=1e-9)


# --- risk_reduction ---


def test_risk_reduction_no_measures_is_zero(
    calculator: SimpleRiskCalculator,
    trajectory: Trajectory,
    scenario_no_climate: Scenario,
    params: RiskParams,
) -> None:
    """Zonder maatregelen: geen risicoreductie."""
    result = calculator.compute(trajectory, scenario_no_climate, [], params)
    assert math.isclose(result.risk_reduction, 0.0, abs_tol=1e-6)


def test_risk_reduction_with_measure(
    calculator: SimpleRiskCalculator,
    trajectory: Trajectory,
    scenario_no_climate: Scenario,
    params: RiskParams,
    measure_1m: Measure,
) -> None:
    """Risicoreductie = (P0 − P0·exp(−α·Δh)) × V0."""
    result = calculator.compute(trajectory, scenario_no_climate, [measure_1m], params)
    expected = P0 * (1.0 - math.exp(-ALPHA * 1.0)) * V0
    assert math.isclose(result.risk_reduction, expected, rel_tol=1e-9)


# --- NCW handmatige berekening ---


def test_ncw_no_measures_manual(
    calculator: SimpleRiskCalculator,
    trajectory: Trajectory,
    scenario_no_climate: Scenario,
    params: RiskParams,
) -> None:
    """
    eta=0, Δh=0 → P(s) = P0 voor alle s.
    NCW = P0 · V0 · Σ_{s=0}^{T-1} exp((γ−δ)·s)
        = 1e-3 · 1e9 · Σ exp(-0.02·s)  over 50 jaar
    """
    result = calculator.compute(trajectory, scenario_no_climate, [], params)

    expected_ncw = P0 * V0 * sum(math.exp((GAMMA - DELTA) * s) for s in range(T))
    assert math.isclose(result.ncw, expected_ncw, rel_tol=1e-9)


def test_ncw_with_measure_manual(
    calculator: SimpleRiskCalculator,
    trajectory: Trajectory,
    scenario_no_climate: Scenario,
    params: RiskParams,
    measure_1m: Measure,
) -> None:
    """
    eta=0, Δh=1.0m → P(s) = P0 · exp(-α·1.0) voor alle s.
    NCW = P0 · exp(-α) · V0 · Σ exp((γ−δ)·s)
    """
    result = calculator.compute(trajectory, scenario_no_climate, [measure_1m], params)

    p_with = P0 * math.exp(-ALPHA * 1.0)
    expected_ncw = p_with * V0 * sum(math.exp((GAMMA - DELTA) * s) for s in range(T))
    assert math.isclose(result.ncw, expected_ncw, rel_tol=1e-9)


# --- NCW daalt bij meer maatregelen (kerneis stap 1.2) ---


def test_ncw_decreases_with_more_measures(
    calculator: SimpleRiskCalculator,
    trajectory: Trajectory,
    scenario_no_climate: Scenario,
    params: RiskParams,
) -> None:
    """NCW neemt af naarmate meer maatregelen worden genomen."""

    def make_measure(dh: float, idx: int) -> Measure:
        return Measure(
            id=f"M{idx:02d}",
            type=MeasureType.DIKE_REINFORCEMENT,
            cost=1_000_000.0,
            year=2020,
            effect=dh,
            location="km 0-10",
        )

    ncw_0 = calculator.compute(trajectory, scenario_no_climate, [], params).ncw
    ncw_05 = calculator.compute(
        trajectory, scenario_no_climate, [make_measure(0.5, 1)], params
    ).ncw
    ncw_10 = calculator.compute(
        trajectory, scenario_no_climate, [make_measure(1.0, 2)], params
    ).ncw

    assert ncw_0 > ncw_05 > ncw_10


# --- NCW stijgt door klimaat (zonder maatregelen) ---


def test_ncw_higher_with_climate(
    calculator: SimpleRiskCalculator,
    trajectory: Trajectory,
    scenario_no_climate: Scenario,
    scenario_climate: Scenario,
    params: RiskParams,
) -> None:
    """Klimaatstijging (eta > 0) verhoogt de NCW."""
    ncw_no_climate = calculator.compute(trajectory, scenario_no_climate, [], params).ncw
    ncw_climate = calculator.compute(trajectory, scenario_climate, [], params).ncw
    assert ncw_climate > ncw_no_climate


# --- Protocol-conformiteit ---


def test_protocol_conformity(calculator: SimpleRiskCalculator) -> None:
    risk_calc: RiskCalculator = calculator  # type: ignore[assignment]
    assert risk_calc is calculator


def test_result_is_frozen(
    calculator: SimpleRiskCalculator,
    trajectory: Trajectory,
    scenario_no_climate: Scenario,
    params: RiskParams,
) -> None:
    result = calculator.compute(trajectory, scenario_no_climate, [], params)
    with pytest.raises(Exception):
        result.ncw = 0.0  # type: ignore[misc]
