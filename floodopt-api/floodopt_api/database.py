"""SQLAlchemy ORM-modellen en engine-factory voor FloodOpt.

Schema:
    scenarios              — hydraulische scenario's
    trajectories           — dijktrajecten
    optimization_results   — optimalisatieresultaten per job_id

Backend:
    SQLite (standaard, geen installatie vereist)
        DATABASE_URL niet ingesteld → sqlite:///floodopt.db
    PostgreSQL + PostGIS (productie)
        DATABASE_URL=postgresql://user:pw@host/db

PostGIS-extensie wordt alleen aangemaakt bij een PostgreSQL-verbinding.
"""

from __future__ import annotations

import os
from pathlib import Path

from sqlalchemy import Float, Integer, JSON, String, create_engine, text
from sqlalchemy.orm import DeclarativeBase, MappedColumn, Session, mapped_column

# Standaard SQLite-pad (relatief aan de projectroot)
_DEFAULT_SQLITE = (Path(__file__).parent.parent.parent / "floodopt.db").resolve()
DEFAULT_URL = f"sqlite:///{_DEFAULT_SQLITE}"


class Base(DeclarativeBase):
    pass


class ScenarioORM(Base):
    __tablename__ = "scenarios"

    id: MappedColumn[str] = mapped_column(String, primary_key=True)
    climate: MappedColumn[str] = mapped_column(String, nullable=False)
    q_design: MappedColumn[float] = mapped_column(Float, nullable=False)
    h_design: MappedColumn[float] = mapped_column(Float, nullable=False)
    eta: MappedColumn[float] = mapped_column(Float, nullable=False)


class TrajectoryORM(Base):
    __tablename__ = "trajectories"

    id: MappedColumn[str] = mapped_column(String, primary_key=True)
    norm: MappedColumn[float] = mapped_column(Float, nullable=False)
    length: MappedColumn[float] = mapped_column(Float, nullable=False)
    p0: MappedColumn[float] = mapped_column(Float, nullable=False)
    alpha: MappedColumn[float] = mapped_column(Float, nullable=False)
    base_year: MappedColumn[int] = mapped_column(Integer, nullable=False)


class OptimizationResultORM(Base):
    __tablename__ = "optimization_results"

    job_id: MappedColumn[str] = mapped_column(String, primary_key=True)
    trajectory_id: MappedColumn[str] = mapped_column(String, nullable=False)
    scenario_id: MappedColumn[str] = mapped_column(String, nullable=False)
    status: MappedColumn[str] = mapped_column(
        String, nullable=False, default="completed"
    )
    objective: MappedColumn[str] = mapped_column(String, nullable=False)
    solver: MappedColumn[str] = mapped_column(String, nullable=False)
    selected_measure_ids: MappedColumn[list[str]] = mapped_column(JSON, nullable=False)
    total_ncw: MappedColumn[float] = mapped_column(Float, nullable=False)
    risk_ncw: MappedColumn[float] = mapped_column(Float, nullable=False)
    investment_npv: MappedColumn[float] = mapped_column(Float, nullable=False)
    objective_value: MappedColumn[float] = mapped_column(Float, nullable=False)


def _is_postgres(url: str) -> bool:
    return url.startswith("postgresql")


def create_engine_from_url(url: str):  # type: ignore[no-untyped-def]
    kwargs: dict = {"pool_pre_ping": True}
    if not _is_postgres(url):
        # SQLite vereist connect_args voor gebruik buiten de aanmaak-thread
        kwargs["connect_args"] = {"check_same_thread": False}
    return create_engine(url, **kwargs)


def init_schema(url: str) -> None:
    """Maak het schema aan.

    Bij PostgreSQL: PostGIS-extensie aanmaken (voor geometrie stap 3.x).
    Bij SQLite: alleen tabellen aanmaken, geen extensies.
    """
    engine = create_engine_from_url(url)
    if _is_postgres(url):
        with engine.connect() as conn:
            conn.execute(text("CREATE EXTENSION IF NOT EXISTS postgis"))
            conn.commit()
    Base.metadata.create_all(engine)


def get_effective_url() -> str:
    """Geeft DATABASE_URL terug als ingesteld, anders de SQLite-standaard."""
    return os.getenv("DATABASE_URL", DEFAULT_URL)


def make_session(url: str) -> Session:
    engine = create_engine_from_url(url)
    return Session(engine)
