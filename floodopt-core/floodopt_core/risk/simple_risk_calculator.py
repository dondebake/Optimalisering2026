import math

from floodopt_core.io.models import Measure, Scenario, Trajectory
from floodopt_core.physics.protocols import PhysicsModel
from floodopt_core.risk.protocols import RiskParams, RiskResult


class SimpleRiskCalculator:
    """MVP risico-calculator: verwachte schade en NCW via discrete jaarlijkse sommatie.

    Formules:
        S(s)  = P(s) × V(s)
        P(s)  = P0 · exp(α·η·s) · exp(−α·Δh)         [Physics Layer]
        V(s)  = V0 · exp(γ·s)                          [schade groeit met economie]
        NCW   = Σ_{s=0}^{T-1} S(s) · exp(−δ·s)

    Waarbij s = jaren na basisjaar (trajectory.base_year).

    Bevat geen optimizer-logica — puur risico-aggregatie.
    """

    def __init__(self, physics: PhysicsModel) -> None:
        self._physics = physics

    def compute(
        self,
        trajectory: Trajectory,
        scenario: Scenario,
        measures: list[Measure],
        params: RiskParams,
    ) -> RiskResult:
        base_year = trajectory.base_year

        # Verwachte schade in basisjaar
        pf_with = self._physics.compute(trajectory, scenario, measures, base_year).pf_overflow
        pf_without = self._physics.compute(trajectory, scenario, [], base_year).pf_overflow

        ed_with = pf_with * params.base_damage
        ed_without = pf_without * params.base_damage
        risk_reduction = ed_without - ed_with

        # NCW: discrete sommatie over tijdshorizon
        ncw = 0.0
        for s in range(params.time_horizon):
            year = base_year + s
            pf_s = self._physics.compute(trajectory, scenario, measures, year).pf_overflow
            v_s = params.base_damage * math.exp(params.gamma * s)
            discount = math.exp(-params.discount_rate * s)
            ncw += pf_s * v_s * discount

        return RiskResult(
            expected_damage_t0=ed_with,
            risk_reduction=risk_reduction,
            ncw=ncw,
        )
