import json

import pytest

from floodopt_core.io import Measure, MeasureType, Scenario, Trajectory


# --- fixtures ---


@pytest.fixture
def measure() -> Measure:
    return Measure(
        id="M01",
        type=MeasureType.DIKE_REINFORCEMENT,
        cost=1_500_000.0,
        year=2030,
        effect=0.5,
        location="km 5-10",
        dependencies=[],
    )


@pytest.fixture
def scenario() -> Scenario:
    return Scenario(id="S01", climate="W+", q_design=16_000.0, h_design=12.5, eta=0.003)


@pytest.fixture
def trajectory(measure: Measure) -> Trajectory:
    return Trajectory(
        id="T01",
        norm=1e-4,
        length=42.5,
        p0=1e-3,
        alpha=5.0,
        base_year=2017,
        measures=[measure],
    )


# --- Measure ---


def test_measure_create(measure: Measure) -> None:
    assert measure.id == "M01"
    assert measure.type == MeasureType.DIKE_REINFORCEMENT
    assert measure.cost == 1_500_000.0
    assert measure.year == 2030
    assert measure.effect == 0.5
    assert measure.dependencies == []


def test_measure_effect_in_meters(measure: Measure) -> None:
    assert measure.effect == 0.5  # 0.5 m kruinhoogteverhoging


def test_measure_json_roundtrip(measure: Measure) -> None:
    json_str = measure.model_dump_json()
    restored = Measure.model_validate(json.loads(json_str))
    assert restored == measure


def test_measure_with_dependencies() -> None:
    m = Measure(
        id="M02",
        type=MeasureType.DIKE_REINFORCEMENT,
        cost=800_000.0,
        year=2035,
        effect=0.3,
        location="km 10-15",
        dependencies=["M01"],
    )
    assert m.dependencies == ["M01"]


def test_measure_invalid_cost() -> None:
    with pytest.raises(Exception):
        Measure(
            id="M_bad",
            type=MeasureType.DIKE_REINFORCEMENT,
            cost=-1.0,
            year=2030,
            effect=0.5,
            location="km 0-5",
        )


def test_measure_invalid_effect() -> None:
    with pytest.raises(Exception):
        Measure(
            id="M_bad",
            type=MeasureType.DIKE_REINFORCEMENT,
            cost=100_000.0,
            year=2030,
            effect=0.0,
            location="km 0-5",
        )


# --- Scenario ---


def test_scenario_create(scenario: Scenario) -> None:
    assert scenario.id == "S01"
    assert scenario.climate == "W+"
    assert scenario.q_design == 16_000.0
    assert scenario.h_design == 12.5
    assert scenario.eta == 0.003


def test_scenario_eta_zero_allowed() -> None:
    s = Scenario(
        id="S_no_climate", climate="huidig", q_design=10_000.0, h_design=10.0, eta=0.0
    )
    assert s.eta == 0.0


def test_scenario_json_roundtrip(scenario: Scenario) -> None:
    json_str = scenario.model_dump_json()
    restored = Scenario.model_validate(json.loads(json_str))
    assert restored == scenario


def test_scenario_invalid_discharge() -> None:
    with pytest.raises(Exception):
        Scenario(id="S_bad", climate="huidig", q_design=0.0, h_design=10.0, eta=0.002)


# --- Trajectory ---


def test_trajectory_create(trajectory: Trajectory, measure: Measure) -> None:
    assert trajectory.id == "T01"
    assert trajectory.norm == 1e-4
    assert trajectory.length == 42.5
    assert trajectory.p0 == 1e-3
    assert trajectory.alpha == 5.0
    assert trajectory.base_year == 2017
    assert len(trajectory.measures) == 1
    assert trajectory.measures[0] == measure


def test_trajectory_json_roundtrip(trajectory: Trajectory) -> None:
    json_str = trajectory.model_dump_json()
    restored = Trajectory.model_validate(json.loads(json_str))
    assert restored == trajectory


def test_trajectory_empty_measures() -> None:
    t = Trajectory(id="T02", norm=1e-3, length=10.0, p0=5e-3, alpha=3.0, base_year=2017)
    assert t.measures == []


def test_trajectory_invalid_norm() -> None:
    with pytest.raises(Exception):
        Trajectory(
            id="T_bad", norm=0.0, length=10.0, p0=1e-3, alpha=5.0, base_year=2017
        )


def test_trajectory_invalid_p0() -> None:
    with pytest.raises(Exception):
        Trajectory(
            id="T_bad", norm=1e-4, length=10.0, p0=0.0, alpha=5.0, base_year=2017
        )


def test_trajectory_invalid_alpha() -> None:
    with pytest.raises(Exception):
        Trajectory(
            id="T_bad", norm=1e-4, length=10.0, p0=1e-3, alpha=0.0, base_year=2017
        )
