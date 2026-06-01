"""
Database round-trip tests — stap 2.2.

Gebruikt SQLite (in-memory) — geen PostgreSQL of Docker vereist.

Verificatie:
  1. Scenario opslaan → ophalen → identiek
  2. Traject opslaan → ophalen → identiek
  3. Optimalisatieresultaat persistent na session.expire_all()
  4. Upsert: zelfde id overschrijft bestaande rij
  5. API-endpoints slaan op in SQLite en halen op via job_id

Productie (PostgreSQL):
  DATABASE_URL=postgresql://floodopt:floodopt@localhost:5432/floodopt
  docker compose up -d postgres && python scripts/init_db.py
"""

from __future__ import annotations

import pytest
from sqlalchemy import create_engine
from sqlalchemy.orm import Session

from floodopt_api.database import Base
from floodopt_api.models import OptimizeResponse
from floodopt_api.repositories import OrmRepositories
from floodopt_core.optimization.protocols import ObjectiveType

# Referentiedata uit stap 1.4 smoke test
from scripts.run_smoke_test import CANDIDATES, RISK_PARAMS, SCENARIO, TRAJECTORY

SQLITE_TEST_URL = "sqlite:///:memory:"


@pytest.fixture(scope="module")
def engine():
    eng = create_engine(SQLITE_TEST_URL, connect_args={"check_same_thread": False})
    Base.metadata.create_all(eng)
    yield eng
    Base.metadata.drop_all(eng)
    eng.dispose()


@pytest.fixture
def db_session(engine):
    with Session(engine) as session:
        yield session


@pytest.fixture(autouse=True)
def clean_tables(db_session):
    yield
    from floodopt_api.database import OptimizationResultORM, ScenarioORM, TrajectoryORM

    db_session.query(OptimizationResultORM).delete()
    db_session.query(ScenarioORM).delete()
    db_session.query(TrajectoryORM).delete()
    db_session.commit()


@pytest.fixture
def repos(db_session):
    return OrmRepositories(db_session)


# ---------------------------------------------------------------------------
# Verificatie-eisen stap 2.2
# ---------------------------------------------------------------------------


def test_scenario_roundtrip(repos):
    """Scenario opslaan → ophalen → identiek."""
    repos.save_scenario(SCENARIO)
    retrieved = repos.get_scenario(SCENARIO.id)
    assert retrieved is not None
    assert retrieved.id == SCENARIO.id
    assert retrieved.climate == SCENARIO.climate
    assert retrieved.eta == SCENARIO.eta
    assert retrieved.q_design == SCENARIO.q_design
    assert retrieved.h_design == SCENARIO.h_design


def test_trajectory_roundtrip(repos):
    """Traject opslaan → ophalen → identiek."""
    repos.save_trajectory(TRAJECTORY)
    retrieved = repos.get_trajectory(TRAJECTORY.id)
    assert retrieved is not None
    assert retrieved.id == TRAJECTORY.id
    assert retrieved.p0 == TRAJECTORY.p0
    assert retrieved.alpha == TRAJECTORY.alpha
    assert retrieved.norm == TRAJECTORY.norm
    assert retrieved.base_year == TRAJECTORY.base_year


def test_result_persistent_after_cache_clear(repos, db_session):
    """Resultaat is persistent: nog opvraagbaar na session.expire_all()."""
    result = OptimizeResponse(
        job_id="test-sqlite-persist-001",
        trajectory_id=TRAJECTORY.id,
        scenario_id=SCENARIO.id,
        objective=ObjectiveType.MIN_COST,
        solver="brute_force",
        selected_measure_ids=["M02", "M04"],
        total_ncw=234_913_345.0,
        risk_ncw=233_824_122.0,
        investment_npv=1_089_224.0,
        objective_value=1_089_224.0,
    )
    repos.save_result(result)
    db_session.expire_all()  # leeg ORM-cache, simuleert nieuwe sessie

    retrieved = repos.get_result("test-sqlite-persist-001")
    assert retrieved is not None
    assert retrieved.job_id == "test-sqlite-persist-001"
    assert set(retrieved.selected_measure_ids) == {"M02", "M04"}
    assert retrieved.investment_npv == pytest.approx(1_089_224.0, rel=1e-6)
    assert retrieved.total_ncw == pytest.approx(234_913_345.0, rel=1e-6)


def test_get_nonexistent_returns_none(repos):
    assert repos.get_scenario("ONBEKEND") is None
    assert repos.get_trajectory("ONBEKEND") is None
    assert repos.get_result("ONBEKEND") is None


def test_scenario_upsert(repos):
    """Opslaan van hetzelfde id overschrijft de bestaande rij."""
    repos.save_scenario(SCENARIO)
    updated = SCENARIO.model_copy(update={"eta": 0.005})
    repos.save_scenario(updated)
    retrieved = repos.get_scenario(SCENARIO.id)
    assert retrieved is not None
    assert retrieved.eta == pytest.approx(0.005)


def test_api_with_sqlite_backend():
    """POST /optimize via de API slaat resultaat op in SQLite (StaticPool)."""
    from sqlalchemy.pool import StaticPool

    from fastapi.testclient import TestClient

    from floodopt_api.main import app, get_repositories
    from floodopt_api.repositories import OrmRepositories

    # StaticPool zorgt dat alle sessies dezelfde in-memory verbinding delen
    eng = create_engine(
        "sqlite:///:memory:",
        connect_args={"check_same_thread": False},
        poolclass=StaticPool,
    )
    Base.metadata.create_all(eng)

    def override():
        with Session(eng) as session:
            yield OrmRepositories(session)

    original = app.dependency_overrides.copy()
    app.dependency_overrides[get_repositories] = override
    client = TestClient(app)

    try:
        client.post(
            "/scenarios",
            content=SCENARIO.model_dump_json(),
            headers={"Content-Type": "application/json"},
        )
        client.post(
            "/trajectories",
            content=TRAJECTORY.model_dump_json(),
            headers={"Content-Type": "application/json"},
        )

        payload = {
            "trajectory_id": TRAJECTORY.id,
            "scenario_id": SCENARIO.id,
            "candidates": [m.model_dump() for m in CANDIDATES],
            "risk_params": RISK_PARAMS.model_dump(),
            "objective": "min_cost",
            "solver": "brute_force",
        }
        resp = client.post("/optimize", json=payload)
        assert resp.status_code == 201
        data = resp.json()
        assert set(data["selected_measure_ids"]) == {"M02", "M04"}

        job_id = data["job_id"]
        resp2 = client.get(f"/results/{job_id}")
        assert resp2.status_code == 200
        assert resp2.json()["job_id"] == job_id
    finally:
        app.dependency_overrides.update(original)
        Base.metadata.drop_all(eng)
