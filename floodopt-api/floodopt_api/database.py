"""SQLAlchemy ORM-modellen en engine-factory voor FloodOpt.

Backend: SQLite (standaard, ingebouwd in Python).
Overgang naar PostgreSQL via DATABASE_URL als dat later nodig is.

Schema:
    scenarios              — hydraulische scenario's
    trajectories           — dijktrajecten
    optimization_results   — optimalisatieresultaten per job_id
"""

from __future__ import annotations

import os
from pathlib import Path

from sqlalchemy import Float, Integer, JSON, String, create_engine
from sqlalchemy.orm import DeclarativeBase, MappedColumn, Session, mapped_column

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
    status: MappedColumn[str] = mapped_column(String, nullable=False, default="pending")
    objective: MappedColumn[str] = mapped_column(String, nullable=False)
    solver: MappedColumn[str] = mapped_column(String, nullable=False)
    # Nullable: None totdat de worker klaar is (status "done")
    selected_measure_ids: MappedColumn[list[str] | None] = mapped_column(
        JSON, nullable=True
    )
    total_ncw: MappedColumn[float | None] = mapped_column(Float, nullable=True)
    risk_ncw: MappedColumn[float | None] = mapped_column(Float, nullable=True)
    investment_npv: MappedColumn[float | None] = mapped_column(Float, nullable=True)
    objective_value: MappedColumn[float | None] = mapped_column(Float, nullable=True)


def create_engine_from_url(url: str):  # type: ignore[no-untyped-def]
    kwargs: dict = {}
    if url.startswith("sqlite"):
        kwargs["connect_args"] = {"check_same_thread": False}
    else:
        kwargs["pool_pre_ping"] = True
    return create_engine(url, **kwargs)


def init_schema(url: str) -> None:
    """Maak het schema aan (idempotent via CREATE TABLE IF NOT EXISTS)."""
    engine = create_engine_from_url(url)
    Base.metadata.create_all(engine)


def get_effective_url() -> str:
    return os.getenv("DATABASE_URL", DEFAULT_URL)


def make_session(url: str) -> Session:
    engine = create_engine_from_url(url)
    return Session(engine)
