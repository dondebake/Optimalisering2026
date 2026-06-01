import math

from floodopt_core.io.models import Measure, Scenario, Trajectory
from floodopt_core.physics.protocols import PhysicsResult


class SimpleDikeOverflow:
    """Analytisch faalkansmodel gebaseerd op de exponentiële benadering.

    P(t) = P0 · exp(α · η · t) · exp(−α · Δh)

    Waarbij:
        P0    faalkans in basisjaar [1/jaar]          (trajectory.p0)
        α     schaalparameter                [1/m]    (trajectory.alpha)
        η     klimaatstijging waterstand     [m/jaar] (scenario.eta)
        t     jaren na basisjaar             [jaar]   (year - trajectory.base_year)
        Δh    totale kruinhoogteverhoging    [m]      (som van measure.effect)

    Formule identiek aan OptimaliseRing 2.3.2 (HKV, 2013).
    Bevat geen optimizer-logica — puur fysisch model.
    """

    def compute(
        self,
        trajectory: Trajectory,
        scenario: Scenario,
        measures: list[Measure],
        year: int,
    ) -> PhysicsResult:
        t = year - trajectory.base_year
        delta_h = sum(m.effect for m in measures)
        pf = (
            trajectory.p0
            * math.exp(trajectory.alpha * scenario.eta * t)
            * math.exp(-trajectory.alpha * delta_h)
        )
        return PhysicsResult(pf_overflow=pf, h_crest=delta_h)
