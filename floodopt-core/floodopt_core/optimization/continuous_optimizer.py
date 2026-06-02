"""Continue optimalisatie van dijkingstrategie — tijdstip én hoogte.

Conform OptimaliseRing 2.3.2 (HKV, 2013): simultane optimalisatie van
T_i (tijdstip) en u_i (hoogte) van N opeenvolgende investeringen.

Kostenfunctie (exponentieel):
    IC(u_i, W_i) = (C + b*u_i) * exp(lambda*(u_i + W_i)) * (1 + Omega/delta2)

NCW-doelfunctie:
    K = sum_{s=0}^{T-1} P(s)*V(s)*exp(-delta1*s) + sum_i IC_i * exp(-delta2*(T_i - t0))
"""

from __future__ import annotations

import math
from dataclasses import dataclass

import numpy as np
from scipy.optimize import minimize

from floodopt_core.io.cost_function import CostFunction
from floodopt_core.io.models import Scenario, Trajectory
from floodopt_core.risk.protocols import RiskParams


@dataclass
class Investment:
    """Één geplande dijkverhoging in de optimale strategie."""

    year: int
    delta_h: float
    W: float
    cost_meur: float
    cost_A_meur: float
    cost_BC_meur: float


@dataclass
class ContinuousResult:
    """Resultaat van de continue optimalisatie."""

    investments: list[Investment]
    total_ncw: float
    risk_ncw: float
    investment_npv: float
    objective_value: float


def _p(s: float, p0: float, alpha: float, eta: float, cumulative_dh: float) -> float:
    return p0 * math.exp(alpha * eta * s - alpha * cumulative_dh)


def _ncw_for_strategy(
    t_offsets: list[float],
    delta_hs: list[float],
    p0: float,
    alpha: float,
    eta: float,
    v0: float,
    gamma: float,
    delta1: float,
    delta2: float,
    T: int,
    cost_fn: CostFunction,
) -> tuple[float, float, float]:
    """Bereken (totale NCW, risico-NCW, investerings-NPV) voor een strategie."""
    investments_sorted = sorted(zip(t_offsets, delta_hs), key=lambda x: x[0])

    # Risico-NCW (discrete sommatie)
    risk_ncw = 0.0
    for s in range(T):
        cumulative_dh = sum(dh for (t, dh) in investments_sorted if t <= s)
        p_s = _p(s, p0, alpha, eta, cumulative_dh)
        v_s = v0 * math.exp(gamma * s)
        risk_ncw += p_s * v_s * math.exp(-delta1 * s)

    # Investerings-NPV (met W voor elke investering)
    inv_npv = 0.0
    W = 0.0
    for (t, dh) in investments_sorted:
        ic = cost_fn.investment_cost(dh, W, delta2)
        inv_npv += ic * math.exp(-delta2 * t) * 1e6  # M EUR -> EUR
        W += dh

    return risk_ncw + inv_npv, risk_ncw, inv_npv


def _ncw_no_measures(
    p0: float, alpha: float, eta: float, v0: float,
    gamma: float, delta1: float, T: int,
) -> float:
    """NCW₀ — verwachte schade zonder maatregelen."""
    beta = alpha * eta + gamma - delta1
    if abs(beta) < 1e-10:
        geo = float(T)
    else:
        geo = (math.exp(beta * T) - 1) / (math.exp(beta) - 1)
    return v0 * p0 * geo


def _optimize_n(
    N: int,
    p0: float, alpha: float, eta: float,
    v0: float, gamma: float, delta1: float, delta2: float,
    T: int, cost_fn: CostFunction,
    u_min: float = 0.1,
    u_max: float = 3.0,
) -> tuple[float, list[float], list[float]]:
    """Optimaliseer voor precies N investeringen. Geeft (NCW, t_offsets, delta_hs)."""

    def objective(x: np.ndarray) -> float:
        t_raw = x[:N]
        u_raw = x[N:]
        # Sorteer tijdstippen en zorg voor ongelijkheid
        t_sorted = np.sort(t_raw)
        total, _, _ = _ncw_for_strategy(
            list(t_sorted), list(u_raw), p0, alpha, eta,
            v0, gamma, delta1, delta2, T, cost_fn,
        )
        return total

    # Beginsituatie: gelijkmatig verdeeld
    t0 = np.array([T * (i + 1) / (N + 1) for i in range(N)], dtype=float)
    u0 = np.array([0.5] * N, dtype=float)
    x0 = np.concatenate([t0, u0])

    # Grenzen
    t_bounds = [(1.0, float(T - 1))] * N
    u_bounds = [(u_min, u_max)] * N
    bounds = t_bounds + u_bounds

    # Beperkingen: T_{i+1} >= T_i + min_gap
    min_gap = max(5, T // (N * 4))
    constraints = [
        {
            "type": "ineq",
            "fun": lambda x, i=i: np.sort(x[:N])[i + 1] - np.sort(x[:N])[i] - min_gap,
        }
        for i in range(N - 1)
    ]

    result = minimize(
        objective, x0,
        method="SLSQP",
        bounds=bounds,
        constraints=constraints,
        options={"ftol": 1e-6, "maxiter": 500, "disp": False},
    )

    t_opt = sorted(result.x[:N])
    u_opt = list(result.x[N:])
    return result.fun, t_opt, u_opt


class ContinuousOptimizer:
    """Continue optimalisatie van dijkingstrategie — tijdstip én hoogte.

    Itereert N = 1 … max_investments en kiest de N met de laagste NCW.
    Kostenfunctie-parameters (C, b, λ, Ω) zijn verplicht als input.
    """

    def __init__(self, max_investments: int = 3, u_min: float = 0.1, u_max: float = 3.0) -> None:
        self.max_investments = max_investments
        self.u_min = u_min
        self.u_max = u_max

    def solve(
        self,
        trajectory: Trajectory,
        scenario: Scenario,
        cost_fn: CostFunction,
        risk_params: RiskParams,
    ) -> ContinuousResult:
        p0 = trajectory.p0
        alpha = trajectory.alpha
        eta = scenario.eta
        v0 = risk_params.base_damage
        gamma = risk_params.gamma
        delta1 = risk_params.discount_rate
        delta2 = risk_params.discount_rate
        T = risk_params.time_horizon
        base_year = trajectory.base_year

        best_ncw = float("inf")
        best_ts: list[float] = []
        best_us: list[float] = []

        for N in range(1, self.max_investments + 1):
            ncw, ts, us = _optimize_n(
                N, p0, alpha, eta, v0, gamma,
                delta1, delta2, T, cost_fn,
                self.u_min, self.u_max,
            )
            if ncw < best_ncw - 1e3:  # 1 EUR verbetering = de moeite waard
                best_ncw = ncw
                best_ts = ts
                best_us = us

        # Bereken NCW-componenten en kostenopsplitsing
        total, risk_ncw, inv_npv = _ncw_for_strategy(
            best_ts, best_us, p0, alpha, eta,
            v0, gamma, delta1, delta2, T, cost_fn,
        )

        ncw0 = _ncw_no_measures(p0, alpha, eta, v0, gamma, delta1, T)

        # Bouw Investment-objecten
        investments: list[Investment] = []
        W = 0.0
        for t_offset, dh in sorted(zip(best_ts, best_us)):
            year = base_year + int(round(t_offset))
            cost = cost_fn.investment_cost(dh, W, delta2)

            # Kostenopsplitsing A/(B+C):
            # A = kosten die ook gemaakt zouden worden bij nul klimaat + nul groei
            cost_A = cost_fn.investment_cost(dh, W, delta2) if W == 0.0 else 0.0

            investments.append(
                Investment(
                    year=year,
                    delta_h=round(dh, 3),
                    W=round(W, 3),
                    cost_meur=round(cost, 3),
                    cost_A_meur=round(cost_A, 3),
                    cost_BC_meur=round(cost - cost_A, 3),
                )
            )
            W += dh

        return ContinuousResult(
            investments=investments,
            total_ncw=total,
            risk_ncw=risk_ncw,
            investment_npv=inv_npv,
            objective_value=total,
        )
