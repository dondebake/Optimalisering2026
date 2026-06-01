"""
Database round-trip tests — stap 2.2.

Verificatie:
  1. Scenario opslaan → ophalen → identiek
  2. Traject opslaan → ophalen → identiek
  3. Optimalisatieresultaat persistent na nieuwe sessie
  4. Round-trip via de API met DATABASE_URL (PostgreSQL backend)

Vereist een draaiende PostgreSQL-instantie.
Skip automatisch als de database niet bereikbaar is.

Start database:
    docker compose up -d postgres_test
    # of: DATABASE_TEST_URL=postgresql://... pytest tests/integration/test_database.py
"""

from __future__ import annotations

import os

import pytest

TEST_URL = os.getenv(
    "DATABASE_TEST_URL",
    "postgresql://floodopt:floodopt@localhost:5433/floodopt_test",
)


def postgres_available() -> bool:
    try:
        import psycopg2

        conn = psycopg2.connect(TEST_URL)
        conn.close()
        return True
    except Exception:
        return False


requires_postgres = pytest.mark.skipif(
    not postgres_available(),
    reason="PostgreSQL niet bereikbaar — start met: docker compose up -d postgres_test",
)


# ---------------------------------------------------------------------------
# Fixtures
# ---------------------------------------------------------------------------


@pytest.fixture(scope="module")
def db_session():
    """SQLAlchemy-sessie op de testdatabase. Schema wordt voor de module aangemaakt."""
    from floodopt_api.database import Base, create_engine_from_url

    engine = create_engine_from_url(TEST_URL)
    from sqlalchemy import text

    with engine.connect() as conn:
        try:
            conn.execute(text("CREATE EXTENSION IF NOT EXISTS postgis"))
            conn.commit()
        except Exception:
            conn.rollback()

    Base.metadata.create_all(engine)

    from sqlalchemy.orm import Session

    with Session(engine) as session:
        yield session

    Base.metadata.drop_all(engine)
    engine.dispose()


@pytest.fixture(autouse=True)
def clean_tables(db_session):
    """Leeg alle tabellen voor elke test (binnen dezelfde module-scope sessie)."""
    from floodopt_api.database import OptimizationResultORM, ScenarioORM, TrajectoryORM

    yield
    db_session.query(OptimizationResultORM).delete()
    db_session.query(ScenarioORM).delete()
    db_session.query(TrajectoryORM).delete()
    db_session.commit()


@pytest.fixture
def repos(db_session):
    from floodopt_api.repositories import PostgresRepositories

    return PostgresRepositories(db_session)


# Referentiedata
from scripts.run_smoke_test import (  # noqa: E402
    CANDIDATES,
    RISK_PARAMS,
    SCENARIO,
    TRAJECTORY,
)


# ---------------------------------------------------------------------------
# Stap 2.2 verificatie-eisen
# ---------------------------------------------------------------------------


@requires_postgres
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


@requires_postgres
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


@requires_postgres
def test_result_persistent_after_new_session(repos, db_session):
    """Optimalisatieresultaat is persistent: nog opvraagbaar na nieuwe sessie."""
    from floodopt_api.models import OptimizeResponse
    from floodopt_core.optimization.protocols import ObjectiveType

    result = OptimizeResponse(
        job_id="test-job-persistent-001",
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
    db_session.expire_all()  # Leeg de ORM-cache — simuleert nieuwe sessie

    retrieved = repos.get_result("test-job-persistent-001")
    assert retrieved is not None
    assert retrieved.job_id == "test-job-persistent-001"
    assert set(retrieved.selected_measure_ids) == {"M02", "M04"}
    assert retrieved.investment_npv == pytest.approx(1_089_224.0, rel=1e-6)
    assert retrieved.total_ncw == pytest.approx(234_913_345.0, rel=1e-6)


@requires_postgres
def test_get_nonexistent_returns_none(repos):
    assert repos.get_scenario("ONBEKEND") is None
    assert repos.get_trajectory("ONBEKEND") is None
    assert repos.get_result("ONBEKEND") is None


@requires_postgres
def test_scenario_upsert(repos):
    """Opslaan van hetzelfde id overschrijft de bestaande rij."""
    repos.save_scenario(SCENARIO)
    updated = SCENARIO.model_copy(update={"eta": 0.005})
    repos.save_scenario(updated)
    retrieved = repos.get_scenario(SCENARIO.id)
    assert retrieved is not None
    assert retrieved.eta == pytest.approx(0.005)


@requires_postgres
def test_api_with_postgres_backend(db_session):
    """POST /optimize via de API met PostgreSQL-backend."""
    import os

    from fastapi.testclient import TestClient

    os.environ["DATABASE_URL"] = TEST_URL

    from floodopt_api.database import Base, create_engine_from_url

    engine = create_engine_from_url(TEST_URL)
    Base.metadata.create_all(engine)

    from floodopt_api.main import app

    client = TestClient(app)

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

    # Resultaat opvraagbaar via GET /results/{job_id}
    job_id = data["job_id"]
    resp2 = client.get(f"/results/{job_id}")
    assert resp2.status_code == 200
    assert resp2.json()["job_id"] == job_id

    del os.environ["DATABASE_URL"]
