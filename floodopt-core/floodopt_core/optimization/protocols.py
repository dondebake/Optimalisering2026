from __future__ import annotations

import math
from dataclasses import dataclass
from enum import Enum
from typing import Protocol

from floodopt_core.io.models import Measure, Scenario, Trajectory
from floodopt_core.risk.protocols import RiskParams


class ObjectiveType(str, Enum):
    MIN_COST = "min_cost"
    """Minimaliseer investeringskosten, onder eis dat norm gehaald wordt."""

    MIN_NCW = "min_ncw"
    """Minimaliseer totale NCW = NCW_risico + NCW_investering."""

    MAX_RISK_REDUCTION = "max_risk_reduction"
    """Maximaliseer risicoreductie binnen beschikbaar budget."""


@dataclass(frozen=True)
class OptimizationResult:
    """Output van een OptimizationStrategy berekening."""

    selected_measures: tuple[Measure, ...]
    """Geselecteerde maatregelen."""

    total_ncw: float
    """NCW_risico + NCW_investering [€]."""

    risk_ncw: float
    """NCW verwachte schade over tijdshorizon [€]."""

    investment_npv: float
    """NCW investeringen (gedisconteerd) [€]."""

    objective_value: float
    """Waarde van de geoptimaliseerde doelfunctie."""

    @property
    def selected_ids(self) -> frozenset[str]:
        return frozenset(m.id for m in self.selected_measures)


def investment_npv(
    measures: list[Measure],
    base_year: int,
    discount_rate: float,
) -> float:
    """Berekent gedisconteerde investeringskosten voor een set maatregelen."""
    return sum(m.cost * math.exp(-discount_rate * max(0.0, m.year - base_year)) for m in measures)


class OptimizationStrategy(Protocol):
    """Protocol voor Optimization Layer implementaties.

    Elke implementatie bevat geen fysische formules — die worden via de
    Risk Layer (en Physics Layer) aangeroepen.
    """

    def solve(
        self,
        trajectory: Trajectory,
        scenario: Scenario,
        candidates: list[Measure],
        risk_params: RiskParams,
        objective: ObjectiveType = ObjectiveType.MIN_NCW,
        budget: float | None = None,
    ) -> OptimizationResult: ...
