from dataclasses import dataclass
from typing import Protocol

from floodopt_core.io.models import Measure, Scenario, Trajectory


@dataclass(frozen=True)
class PhysicsResult:
    """Output van een PhysicsModel berekening."""

    pf_overflow: float
    """Overstrominskans op het gevraagde jaar [1/jaar]."""

    h_crest: float
    """Totale kruinhoogteverhoging door maatregelen Σ Δh [m]."""


class PhysicsModel(Protocol):
    """Protocol voor Physics Layer implementaties.

    Elke implementatie berekent de overstrominskans op een gegeven jaar voor
    een traject, scenario en set maatregelen. Bevat geen optimizer-logica.
    """

    def compute(
        self,
        trajectory: Trajectory,
        scenario: Scenario,
        measures: list[Measure],
        year: int,
    ) -> PhysicsResult: ...
