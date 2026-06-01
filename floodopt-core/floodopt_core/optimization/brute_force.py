from __future__ import annotations

from itertools import combinations

from floodopt_core.io.models import Measure, Scenario, Trajectory
from floodopt_core.optimization.protocols import (
    ObjectiveType,
    OptimizationResult,
    investment_npv,
)
from floodopt_core.risk.protocols import RiskCalculator, RiskParams


def _dependencies_satisfied(subset: list[Measure], all_ids: frozenset[str]) -> bool:
    """Controleert of alle afhankelijkheden binnen de subset vervuld zijn."""
    subset_ids = frozenset(m.id for m in subset)
    return all(dep in subset_ids for m in subset for dep in m.dependencies)


class BruteForceOptimizer:
    """Referentie-optimizer: itereert alle 2^N combinaties en kiest de beste.

    Bevat geen fysische formules — Physics en Risk Layer worden via
    de RiskCalculator aangeroepen.

    Tijdcomplexiteit: O(2^N · T) waarbij T de tijdshorizon is.
    Bruikbaar tot N ≈ 15 voor validatiedoeleinden.
    """

    def __init__(self, risk: RiskCalculator) -> None:
        self._risk = risk

    def solve(
        self,
        trajectory: Trajectory,
        scenario: Scenario,
        candidates: list[Measure],
        risk_params: RiskParams,
        objective: ObjectiveType = ObjectiveType.MIN_NCW,
        budget: float | None = None,
    ) -> OptimizationResult:
        all_ids = frozenset(m.id for m in candidates)
        best: OptimizationResult | None = None
        best_obj = float("inf")

        # Itereer alle deelverzamelingen (inclusief lege set)
        for r in range(len(candidates) + 1):
            for subset_tuple in combinations(candidates, r):
                subset = list(subset_tuple)

                if not _dependencies_satisfied(subset, all_ids):
                    continue

                npv = investment_npv(subset, trajectory.base_year, risk_params.discount_rate)

                # Budget-filter voor MAX_RISK_REDUCTION
                if objective == ObjectiveType.MAX_RISK_REDUCTION and budget is not None:
                    if npv > budget:
                        continue

                risk_result = self._risk.compute(trajectory, scenario, subset, risk_params)
                total = risk_result.ncw + npv

                if objective == ObjectiveType.MIN_COST:
                    # Alleen subsets die de norm halen (P ≤ norm)
                    pf = risk_result.expected_damage_t0 / risk_params.base_damage
                    if pf > trajectory.norm:
                        continue
                    obj = npv

                elif objective == ObjectiveType.MAX_RISK_REDUCTION:
                    obj = -risk_result.risk_reduction  # minimaliseer = maximaliseer

                else:  # MIN_NCW
                    obj = total

                if obj < best_obj:
                    best_obj = obj
                    best = OptimizationResult(
                        selected_measures=subset_tuple,
                        total_ncw=total,
                        risk_ncw=risk_result.ncw,
                        investment_npv=npv,
                        objective_value=-best_obj
                        if objective == ObjectiveType.MAX_RISK_REDUCTION
                        else best_obj,
                    )

        if best is None:
            # Geen haalbare oplossing gevonden (bijv. norm niet haalbaar)
            risk_result = self._risk.compute(trajectory, scenario, [], risk_params)
            return OptimizationResult(
                selected_measures=(),
                total_ncw=risk_result.ncw,
                risk_ncw=risk_result.ncw,
                investment_npv=0.0,
                objective_value=float("inf"),
            )

        return best
