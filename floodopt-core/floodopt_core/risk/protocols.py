from dataclasses import dataclass
from typing import Protocol

from pydantic import BaseModel, Field

from floodopt_core.io.models import Measure, Scenario, Trajectory


class RiskParams(BaseModel):
    """Parameters voor de Risk Layer berekening."""

    base_damage: float = Field(gt=0, description="Basisschade bij overstroming V0 [€]")
    discount_rate: float = Field(gt=0, le=1, description="Discontovoet δ [1/jaar, bijv. 0.04]")
    gamma: float = Field(ge=0, description="Economische groeivoet γ [1/jaar, bijv. 0.02]")
    time_horizon: int = Field(gt=0, le=500, description="Tijdshorizon T [jaar]")


@dataclass(frozen=True)
class RiskResult:
    """Output van een RiskCalculator berekening."""

    expected_damage_t0: float
    """Verwachte schade in het basisjaar met maatregelen: P(t0) × V0 [€/jaar]."""

    risk_reduction: float
    """Risicoreductie t.o.v. geen maatregelen: (P0 − P(t0,Δh)) × V0 [€/jaar]."""

    ncw: float
    """NCW verwachte schade over tijdshorizon: Σ P(t)·V(t)·exp(−δ·s) [€]."""


class RiskCalculator(Protocol):
    """Protocol voor Risk Layer implementaties.

    Elke implementatie berekent verwachte schade en NCW voor een traject,
    scenario en set maatregelen. Bevat geen optimizer-logica.
    """

    def compute(
        self,
        trajectory: Trajectory,
        scenario: Scenario,
        measures: list[Measure],
        params: RiskParams,
    ) -> RiskResult: ...
