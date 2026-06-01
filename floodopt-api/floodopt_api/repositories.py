"""Repository-laag: abstracte interface + twee implementaties.

MemoryRepositories   — in-memory (voor tests, geen DB vereist)
PostgresRepositories — SQLAlchemy + psycopg2 (productie)

De API-laag (main.py) gebruikt uitsluitend de abstracte interface;
de implementatie wordt via FastAPI dependency injection gekozen.
"""

from __future__ import annotations

from typing import Protocol

from sqlalchemy.orm import Session

from floodopt_api.database import (
    OptimizationResultORM,
    ScenarioORM,
    TrajectoryORM,
)
from floodopt_api.models import OptimizeResponse
from floodopt_core.io.models import Scenario, Trajectory


# ---------------------------------------------------------------------------
# Abstracte interface (Protocol)
# ---------------------------------------------------------------------------


class Repositories(Protocol):
    def save_scenario(self, s: Scenario) -> None: ...
    def get_scenario(self, id: str) -> Scenario | None: ...

    def save_trajectory(self, t: Trajectory) -> None: ...
    def get_trajectory(self, id: str) -> Trajectory | None: ...

    def save_result(self, r: OptimizeResponse) -> None: ...
    def get_result(self, job_id: str) -> OptimizeResponse | None: ...
    def update_status(self, job_id: str, status: str) -> None: ...


# ---------------------------------------------------------------------------
# In-memory implementatie (tests, development zonder DB)
# ---------------------------------------------------------------------------


class MemoryRepositories:
    """Volatile opslag — geen persistentie na herstart.
    Gebruikt als fallback als DATABASE_URL niet is ingesteld.
    """

    def __init__(self) -> None:
        self._scenarios: dict[str, Scenario] = {}
        self._trajectories: dict[str, Trajectory] = {}
        self._results: dict[str, OptimizeResponse] = {}

    def save_scenario(self, s: Scenario) -> None:
        self._scenarios[s.id] = s

    def get_scenario(self, id: str) -> Scenario | None:
        return self._scenarios.get(id)

    def save_trajectory(self, t: Trajectory) -> None:
        self._trajectories[t.id] = t

    def get_trajectory(self, id: str) -> Trajectory | None:
        return self._trajectories.get(id)

    def save_result(self, r: OptimizeResponse) -> None:
        self._results[r.job_id] = r

    def get_result(self, job_id: str) -> OptimizeResponse | None:
        return self._results.get(job_id)

    def update_status(self, job_id: str, status: str) -> None:
        if job_id in self._results:
            self._results[job_id] = self._results[job_id].model_copy(
                update={"status": status}
            )

    def clear(self) -> None:
        self._scenarios.clear()
        self._trajectories.clear()
        self._results.clear()


# ---------------------------------------------------------------------------
# PostgreSQL implementatie
# ---------------------------------------------------------------------------


class OrmRepositories:
    """Persistente opslag via SQLAlchemy (SQLite standaard, PostgreSQL optioneel)."""

    def __init__(self, session: Session) -> None:
        self._s = session

    def save_scenario(self, s: Scenario) -> None:
        orm = ScenarioORM(
            id=s.id,
            climate=s.climate,
            q_design=s.q_design,
            h_design=s.h_design,
            eta=s.eta,
        )
        self._s.merge(orm)
        self._s.commit()

    def get_scenario(self, id: str) -> Scenario | None:
        row = self._s.get(ScenarioORM, id)
        if row is None:
            return None
        return Scenario(
            id=row.id,
            climate=row.climate,
            q_design=row.q_design,
            h_design=row.h_design,
            eta=row.eta,
        )

    def save_trajectory(self, t: Trajectory) -> None:
        orm = TrajectoryORM(
            id=t.id,
            norm=t.norm,
            length=t.length,
            p0=t.p0,
            alpha=t.alpha,
            base_year=t.base_year,
        )
        self._s.merge(orm)
        self._s.commit()

    def get_trajectory(self, id: str) -> Trajectory | None:
        row = self._s.get(TrajectoryORM, id)
        if row is None:
            return None
        return Trajectory(
            id=row.id,
            norm=row.norm,
            length=row.length,
            p0=row.p0,
            alpha=row.alpha,
            base_year=row.base_year,
        )

    def save_result(self, r: OptimizeResponse) -> None:
        orm = OptimizationResultORM(
            job_id=r.job_id,
            trajectory_id=r.trajectory_id,
            scenario_id=r.scenario_id,
            status=r.status,
            objective=r.objective.value,
            solver=r.solver,
            selected_measure_ids=r.selected_measure_ids or None,
            total_ncw=r.total_ncw,
            risk_ncw=r.risk_ncw,
            investment_npv=r.investment_npv,
            objective_value=r.objective_value,
        )
        self._s.merge(orm)
        self._s.commit()

    def get_result(self, job_id: str) -> OptimizeResponse | None:
        row = self._s.get(OptimizationResultORM, job_id)
        if row is None:
            return None
        return OptimizeResponse(
            job_id=row.job_id,
            trajectory_id=row.trajectory_id,
            scenario_id=row.scenario_id,
            status=row.status,  # type: ignore[arg-type]
            objective=row.objective,  # type: ignore[arg-type]
            solver=row.solver,
            selected_measure_ids=row.selected_measure_ids or [],
            total_ncw=row.total_ncw,
            risk_ncw=row.risk_ncw,
            investment_npv=row.investment_npv,
            objective_value=row.objective_value,
        )

    def update_status(self, job_id: str, status: str) -> None:
        row = self._s.get(OptimizationResultORM, job_id)
        if row is not None:
            row.status = status
            self._s.commit()
