from __future__ import annotations

import math

import pyomo.environ as pyo  # type: ignore[import-untyped]

from floodopt_core.io.models import Measure, Scenario, Trajectory
from floodopt_core.optimization.protocols import (
    ObjectiveType,
    OptimizationResult,
    investment_npv,
)
from floodopt_core.risk.protocols import RiskCalculator, RiskParams


class PyomoOptimizer:
    """Optimizer via Pyomo MILP met HiGHS solver.

    Objective-implementaties:
      MIN_COST:           exact MILP — minimaliseer investering s.t. norm gehaald
      MAX_RISK_REDUCTION: exact MILP — maximaliseer Δh binnen budget (knapsack)
      MIN_NCW:            lineaire benadering (eerste-orde Taylor van exp(-α·Δh))
                          Geldig voor kleine α·Δh per maatregel (< 0.5).

    Bevat geen fysische formules — Risk Layer wordt via RiskCalculator aangeroepen.
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
        N = len(candidates)

        if N == 0:
            risk_result = self._risk.compute(trajectory, scenario, [], risk_params)
            return OptimizationResult(
                selected_measures=(),
                total_ncw=risk_result.ncw,
                risk_ncw=risk_result.ncw,
                investment_npv=0.0,
                objective_value=risk_result.ncw,
            )

        disc_costs = [
            m.cost * math.exp(-risk_params.discount_rate * max(0.0, m.year - trajectory.base_year))
            for m in candidates
        ]
        effects = [m.effect for m in candidates]

        model = pyo.ConcreteModel()
        model.I = pyo.RangeSet(0, N - 1)
        model.x = pyo.Var(model.I, domain=pyo.Binary)

        # Afhankelijkheidsconstraints: x_j <= x_i  als maatregel j maatregel i vereist
        dep_idx = 0
        for j, m in enumerate(candidates):
            for dep_id in m.dependencies:
                for i, c in enumerate(candidates):
                    if c.id == dep_id:
                        model.add_component(
                            f"dep_{dep_idx}",
                            pyo.Constraint(expr=model.x[j] <= model.x[i]),
                        )
                        dep_idx += 1

        if objective == ObjectiveType.MIN_COST:
            self._build_min_cost(model, trajectory, disc_costs, effects, N)

        elif objective == ObjectiveType.MAX_RISK_REDUCTION:
            self._build_max_risk_reduction(model, disc_costs, effects, N, budget)

        else:  # MIN_NCW — lineaire benadering
            self._build_min_ncw_linear(
                model, trajectory, scenario, candidates, risk_params, disc_costs, effects, N
            )

        solver = pyo.SolverFactory("highs")
        solver.solve(model, tee=False)

        selected = [candidates[i] for i in range(N) if pyo.value(model.x[i]) > 0.5]
        npv = investment_npv(selected, trajectory.base_year, risk_params.discount_rate)
        risk_result = self._risk.compute(trajectory, scenario, selected, risk_params)
        total = risk_result.ncw + npv

        if objective == ObjectiveType.MIN_COST:
            obj_val = npv
        elif objective == ObjectiveType.MAX_RISK_REDUCTION:
            obj_val = risk_result.risk_reduction
        else:
            obj_val = total

        return OptimizationResult(
            selected_measures=tuple(selected),
            total_ncw=total,
            risk_ncw=risk_result.ncw,
            investment_npv=npv,
            objective_value=obj_val,
        )

    # --- Private builders ---

    def _build_min_cost(
        self,
        model: pyo.ConcreteModel,
        trajectory: Trajectory,
        disc_costs: list[float],
        effects: list[float],
        N: int,
    ) -> None:
        """MILP: min Σ c_i·x_i  s.t.  Σ h_i·x_i >= h_min, x_i ∈ {0,1}."""
        if trajectory.p0 > trajectory.norm:
            h_min = math.log(trajectory.p0 / trajectory.norm) / trajectory.alpha
        else:
            h_min = 0.0

        model.obj = pyo.Objective(
            expr=sum(disc_costs[i] * model.x[i] for i in range(N)),
            sense=pyo.minimize,
        )
        model.norm_constr = pyo.Constraint(
            expr=sum(effects[i] * model.x[i] for i in range(N)) >= h_min
        )

    def _build_max_risk_reduction(
        self,
        model: pyo.ConcreteModel,
        disc_costs: list[float],
        effects: list[float],
        N: int,
        budget: float | None,
    ) -> None:
        """MILP: max Σ h_i·x_i  s.t.  Σ c_i·x_i <= budget  (0/1-knapsack)."""
        model.obj = pyo.Objective(
            expr=sum(effects[i] * model.x[i] for i in range(N)),
            sense=pyo.maximize,
        )
        if budget is not None:
            model.budget_constr = pyo.Constraint(
                expr=sum(disc_costs[i] * model.x[i] for i in range(N)) <= budget
            )

    def _build_min_ncw_linear(
        self,
        model: pyo.ConcreteModel,
        trajectory: Trajectory,
        scenario: Scenario,
        candidates: list[Measure],
        risk_params: RiskParams,
        disc_costs: list[float],
        effects: list[float],
        N: int,
    ) -> None:
        """Lineair MILP via eerste-orde Taylor: NCW ≈ C·(1 - α·Δh).

        Nettokosten per maatregel: c_i - C·α·h_i
        Selecteer maatregel als voordeel > investering.
        Geldig voor α·h_i << 1 per maatregel.
        """
        baseline_ncw = self._risk.compute(trajectory, scenario, [], risk_params).ncw
        alpha = trajectory.alpha
        lin_reductions = [baseline_ncw * alpha * effects[i] for i in range(N)]

        model.obj = pyo.Objective(
            expr=sum((disc_costs[i] - lin_reductions[i]) * model.x[i] for i in range(N)),
            sense=pyo.minimize,
        )
