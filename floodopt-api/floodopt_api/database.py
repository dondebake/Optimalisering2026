"""SQLAlchemy ORM-modellen en engine-factory voor FloodOpt.

Schema:
    scenarios              — hydraulische scenario's
    trajectories           — dijktrajecten
    optimization_results   — optimalisatieresultaten per job_id

PostGIS-extensie wordt bij initialisatie aangemaakt (voor toekomstig gebruik van
geometrie-kolommen, bijv. dijkvak-alignementen in stap 3.x).
"""

from __future__ import annotations

import os

from sqlalchemy import Float, Integer, JSON, String, create_engine, text
from sqlalchemy.orm import DeclarativeBase, MappedColumn, Session, mapped_column


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


def create_engine_from_url(url: str):  # type: ignore[no-untyped-def]
    return create_engine(url, pool_pre_ping=True)


def init_schema(url: str) -> None:
    """Maak het schema aan (inclusief PostGIS-extensie)."""
    engine = create_engine_from_url(url)
    with engine.connect() as conn:
        conn.execute(text("CREATE EXTENSION IF NOT EXISTS postgis"))
        conn.commit()
    Base.metadata.create_all(engine)


def get_default_url() -> str | None:
    return os.getenv("DATABASE_URL")


def make_session(url: str) -> Session:
    engine = create_engine_from_url(url)
    return Session(engine)
