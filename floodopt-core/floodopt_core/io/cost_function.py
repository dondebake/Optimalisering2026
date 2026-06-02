"""Kostenfunctie-parameters voor de continue optimizer.

Formule (exponentieel, conform OptimaliseRing 2.3.2):
    IC(u, W) = (C + b * u) * exp(lambda * (u + W)) * (1 + Omega / delta2)

Waarbij:
    u     = dijkverhoging [m]
    W     = som eerdere dijkverhogingen [m]  (= 0 bij eerste investering)
    C     = vaste kosten [M EUR]
    b     = variabele lineaire kosten [M EUR/m]
    lam   = exponentiële schaalparameter [1/m]
    Omega = jaarlijkse onderhoudsfractie [-]  (Omega=0.002 in 2011-database)
"""

from __future__ import annotations

from pydantic import BaseModel, Field


class CostFunction(BaseModel):
    """Parameters voor de exponentiële kostenfunctie per traject."""

    C: float = Field(gt=0, description="Vaste kosten [M EUR]  (C_exp in DB)")
    b: float = Field(ge=0, description="Variabele lineaire kosten [M EUR/m]  (b_exp in DB)")
    lam: float = Field(ge=0, description="Exponentiële schaalparameter [1/m]  (lambda_exp_per_m in DB)")
    omega: float = Field(
        default=0.002, ge=0, description="Jaarlijkse onderhoudsfractie [-]  (Omega in DB)"
    )

    def investment_cost(self, delta_h: float, W: float, delta2: float) -> float:
        """Bereken investeringskosten incl. onderhoud [M EUR].

        Args:
            delta_h: Dijkverhoging [m]
            W:       Cumulatieve eerdere verhogingen [m]
            delta2:  Discontovoet investeringen [1/jaar]
        """
        import math

        maintenance_factor = 1 + self.omega / delta2 if delta2 > 0 else 1.0
        return (self.C + self.b * delta_h) * math.exp(self.lam * (delta_h + W)) * maintenance_factor
