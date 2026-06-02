"""Request- en response-modellen voor de FloodOpt API.

Geen business logic — alleen datastructuren voor HTTP-communicatie.
De kern-berekeningen zitten in floodopt-core.
"""

from __future__ import annotations

from typing import Literal

from pydantic import BaseModel, Field

from floodopt_core.io.models import Measure
from floodopt_core.optimization.protocols import ObjectiveType
from floodopt_core.risk.protocols import RiskParams


class CostFunctionParams(BaseModel):
    """Parameters voor de exponentiële kostenfunctie (voor de continue optimizer)."""

    C: float = Field(gt=0, description="Vaste kosten [M EUR]")
    b: float = Field(ge=0, description="Variabele lineaire kosten [M EUR/m]")
    lam: float = Field(ge=0, description="Exponentiële schaalparameter [1/m]")
    omega: float = Field(default=0.002, ge=0, description="Jaarlijkse onderhoudsfractie")


class OptimizeRequest(BaseModel):
    """Verzoek om een optimalisatie uit te voeren."""

    trajectory_id: str
    scenario_id: str
    candidates: list[Measure] = Field(min_length=0, description="Kandidaatmaatregelen (leeg bij continuous solver)")
    risk_params: RiskParams
    objective: ObjectiveType = ObjectiveType.MIN_NCW
    budget: float | None = Field(
        default=None, description="Budget [€] voor MAX_RISK_REDUCTION"
    )
    solver: Literal["brute_force", "pyomo", "continuous"] = Field(
        default="brute_force",
        description="'brute_force' = exact referentie, 'pyomo' = HiGHS MILP, 'continuous' = continue optimalisatie",
    )
    cost_function: CostFunctionParams | None = Field(
        default=None, description="Kostenfunctie-parameters voor de continue optimizer"
    )


class OptimizeResponse(BaseModel):
    """Resultaat van een optimalisatierun.

    Velden total_ncw t/m objective_value zijn None zolang status != 'done'.
    """

    job_id: str
    status: Literal["pending", "running", "done", "failed"] = "pending"
    trajectory_id: str
    scenario_id: str
    objective: ObjectiveType
    solver: str
    selected_measure_ids: list[str] = []
    total_ncw: float | None = Field(
        default=None, description="NCW_risico + NCW_investering [€]"
    )
    risk_ncw: float | None = Field(default=None, description="NCW verwachte schade [€]")
    investment_npv: float | None = Field(
        default=None, description="Gedisconteerde investeringskosten [€]"
    )
    objective_value: float | None = Field(
        default=None, description="Waarde van de geoptimaliseerde doelfunctie"
    )
    p_series: list[dict] | None = Field(
        default=None,
        description="P(t)-tijdreeks: [{year, p, p_mid}, …] voor elk jaar in de horizon",
    )
    error_message: str | None = Field(
        default=None,
        description="Foutmelding als status='failed'",
    )
    investments: list[dict] | None = Field(
        default=None,
        description="Investeringsschema van de continue optimizer [{year, delta_h, W, cost_meur, ...}]",
    )
    input_payload: dict | None = Field(
        default=None,
        description="Volledige invoer van de berekening (trajectory, scenario, candidates, risk_params)",
    )
