"""
Handmatige testcases voor SimpleDikeOverflow.

Formule: P(t) = P0 · exp(α · η · t) · exp(−α · Δh)

Parameters gebruikt in alle cases:
    P0        = 1e-3   [1/jaar]
    alpha     = 5.0    [1/m]
    eta       = 0.003  [m/jaar]  (W+ klimaatscenario, ~3 mm/jaar)
    base_year = 2017
"""

import math

import pytest

from floodopt_core.io import Measure, MeasureType, Scenario, Trajectory
from floodopt_core.physics import PhysicsModel, SimpleDikeOverflow

P0 = 1e-3
ALPHA = 5.0
ETA = 0.003
BASE_YEAR = 2017


@pytest.fixture
def trajectory() -> Trajectory:
    return Trajectory(
        id="T_test",
        norm=1e-4,
        length=10.0,
        p0=P0,
        alpha=ALPHA,
        base_year=BASE_YEAR,
    )


@pytest.fixture
def scenario() -> Scenario:
    return Scenario(
        id="S_Wplus", climate="W+", q_design=15_000.0, h_design=11.0, eta=ETA
    )


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
def model() -> SimpleDikeOverflow:
    return SimpleDikeOverflow()


# --- Case 1: basisjaar, geen maatregelen → P = P0 ---


def test_case1_base_year_no_measures(
    model: SimpleDikeOverflow, trajectory: Trajectory, scenario: Scenario
) -> None:
    """t=0, Δh=0 → P(t) moet exact gelijk zijn aan P0."""
    result = model.compute(trajectory, scenario, measures=[], year=BASE_YEAR)
    expected = P0  # 1e-3
    assert math.isclose(result.pf_overflow, expected, rel_tol=1e-9)
    assert result.h_crest == 0.0


# --- Case 2: 50 jaar klimaatstijging, geen maatregelen ---


def test_case2_50_years_climate_no_measures(
    model: SimpleDikeOverflow, trajectory: Trajectory, scenario: Scenario
) -> None:
    """t=50, Δh=0 → P = P0 · exp(α·η·50) = 1e-3 · exp(0.75) ≈ 2.117e-3."""
    result = model.compute(trajectory, scenario, measures=[], year=BASE_YEAR + 50)
    expected = P0 * math.exp(ALPHA * ETA * 50)  # ≈ 2.117e-3
    assert math.isclose(result.pf_overflow, expected, rel_tol=1e-9)
    assert result.h_crest == 0.0


# --- Case 3: basisjaar, 1 m verhoging → sterke kansreductie ---


def test_case3_base_year_with_1m_measure(
    model: SimpleDikeOverflow,
    trajectory: Trajectory,
    scenario: Scenario,
    measure_1m: Measure,
) -> None:
    """t=0, Δh=1.0 m → P = P0 · exp(-α·1.0) = 1e-3 · exp(-5) ≈ 6.738e-6."""
    result = model.compute(trajectory, scenario, measures=[measure_1m], year=BASE_YEAR)
    expected = P0 * math.exp(-ALPHA * 1.0)  # ≈ 6.738e-6
    assert math.isclose(result.pf_overflow, expected, rel_tol=1e-9)
    assert result.h_crest == 1.0


# --- Gecombineerd: klimaat + maatregel ---


def test_climate_and_measure_combined(
    model: SimpleDikeOverflow,
    trajectory: Trajectory,
    scenario: Scenario,
    measure_1m: Measure,
) -> None:
    """t=50, Δh=1.0 m → P = P0 · exp(α·η·50) · exp(-α·1.0)."""
    result = model.compute(
        trajectory, scenario, measures=[measure_1m], year=BASE_YEAR + 50
    )
    expected = P0 * math.exp(ALPHA * ETA * 50) * math.exp(-ALPHA * 1.0)
    assert math.isclose(result.pf_overflow, expected, rel_tol=1e-9)


# --- Twee maatregelen → effecten optellen ---


def test_two_measures_additive_effect(
    model: SimpleDikeOverflow, trajectory: Trajectory, scenario: Scenario
) -> None:
    """Twee maatregelen van elk 0.5 m geven zelfde resultaat als één van 1.0 m."""
    m_half = Measure(
        id="M_half",
        type=MeasureType.DIKE_REINFORCEMENT,
        cost=1_000_000.0,
        year=2020,
        effect=0.5,
        location="km 0-5",
    )
    result_two = model.compute(
        trajectory, scenario, measures=[m_half, m_half], year=BASE_YEAR
    )
    result_one = model.compute(
        trajectory,
        scenario,
        measures=[
            Measure(
                id="M_1m",
                type=MeasureType.DIKE_REINFORCEMENT,
                cost=2_000_000.0,
                year=2020,
                effect=1.0,
                location="km 0-10",
            )
        ],
        year=BASE_YEAR,
    )
    assert math.isclose(result_two.pf_overflow, result_one.pf_overflow, rel_tol=1e-9)
    assert result_two.h_crest == 1.0


# --- Protocol-conformiteit ---


def test_protocol_conformity(model: SimpleDikeOverflow) -> None:
    """SimpleDikeOverflow voldoet aan het PhysicsModel Protocol (runtime check)."""
    # mypy verifieert dit statisch; hier controleren we runtime assignability
    physics_model: PhysicsModel = model  # type: ignore[assignment]
    assert physics_model is model


def test_result_is_frozen(
    model: SimpleDikeOverflow, trajectory: Trajectory, scenario: Scenario
) -> None:
    """PhysicsResult is immutable (frozen dataclass)."""
    result = model.compute(trajectory, scenario, measures=[], year=BASE_YEAR)
    with pytest.raises(Exception):
        result.pf_overflow = 0.0  # type: ignore[misc]
