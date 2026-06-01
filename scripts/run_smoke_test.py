"""
Stap 1.4 — Integratie smoke test (CLI).

End-to-end run zonder API of database:
  Traject → Scenario → Maatregelen → Optimizer → Resultaat

Verificatie:
  1. Runt zonder errors
  2. BruteForce.solve() == PyomoOptimizer.solve() voor alle objectives
  3. Rekentijd BruteForce vs. Pyomo gelogd

Gebruik:
    python scripts/run_smoke_test.py

Exitcode:
    0  alle objectives matchen
    1  afwijking gedetecteerd (stop de ontwikkeling, analyseer oorzaak)
"""

from __future__ import annotations

import sys
import time

# Windows-terminals gebruiken standaard cp1252; forceer UTF-8 voor speciale tekens.
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
from dataclasses import dataclass

from floodopt_core.io.models import Measure, MeasureType, Scenario, Trajectory
from floodopt_core.optimization import (
    BruteForceOptimizer,
    ObjectiveType,
    OptimizationResult,
    PyomoOptimizer,
)
from floodopt_core.physics.simple_dike_overflow import SimpleDikeOverflow
from floodopt_core.risk.protocols import RiskParams
from floodopt_core.risk.simple_risk_calculator import SimpleRiskCalculator

# ---------------------------------------------------------------------------
# Testcase: realistisch Rijnmond-achtig traject (MVP parameters)
# ---------------------------------------------------------------------------

TRAJECTORY = Trajectory(
    id="T_rijnmond_mvp",
    norm=1e-3,  # 1/1000 per jaar — DPVL-norm dijkring 14
    length=45.0,  # km
    p0=5e-3,  # huidige faalkans 1/200 per jaar
    alpha=4.0,  # schaalparameter [1/m]
    base_year=2017,
)

SCENARIO = Scenario(
    id="S_Wplus",
    climate="W+",
    q_design=16_000.0,  # m³/s — Rijn maatgevende afvoer
    h_design=11.5,  # m+NAP
    eta=0.003,  # klimaatstijging 3 mm/jaar
)

CANDIDATES: list[Measure] = [
    Measure(
        id="M01",
        type=MeasureType.DIKE_REINFORCEMENT,
        effect=0.50,
        cost=2_000_000.0,
        year=2025,
        location="km  0-15",
    ),
    Measure(
        id="M02",
        type=MeasureType.DIKE_REINFORCEMENT,
        effect=0.30,
        cost=1_000_000.0,
        year=2025,
        location="km 15-28",
    ),
    Measure(
        id="M03",
        type=MeasureType.DIKE_REINFORCEMENT,
        effect=0.30,
        cost=1_200_000.0,
        year=2028,
        location="km 28-38",
    ),
    Measure(
        id="M04",
        type=MeasureType.DIKE_REINFORCEMENT,
        effect=0.20,
        cost=500_000.0,
        year=2025,
        location="km 38-42",
    ),
    Measure(
        id="M05",
        type=MeasureType.DIKE_REINFORCEMENT,
        effect=0.15,
        cost=800_000.0,
        year=2030,
        location="km 42-45",
    ),
]

RISK_PARAMS = RiskParams(
    base_damage=5_000_000_000.0,  # 5 miljard € (Rijnmond)
    discount_rate=0.04,
    gamma=0.02,
    time_horizon=100,
)

BUDGET_MAX_RR = 2_000_000.0  # € budget voor MAX_RISK_REDUCTION


# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------


@dataclass
class RunResult:
    objective: ObjectiveType
    bf_result: OptimizationResult
    py_result: OptimizationResult
    bf_time: float
    py_time: float

    @property
    def match(self) -> bool:
        return self.bf_result.selected_ids == self.py_result.selected_ids


def run_both(
    bf: BruteForceOptimizer,
    py: PyomoOptimizer,
    objective: ObjectiveType,
    budget: float | None = None,
) -> RunResult:
    t0 = time.perf_counter()
    bf_result = bf.solve(
        TRAJECTORY, SCENARIO, CANDIDATES, RISK_PARAMS, objective, budget
    )
    bf_time = time.perf_counter() - t0

    t0 = time.perf_counter()
    py_result = py.solve(
        TRAJECTORY, SCENARIO, CANDIDATES, RISK_PARAMS, objective, budget
    )
    py_time = time.perf_counter() - t0

    return RunResult(objective, bf_result, py_result, bf_time, py_time)


def fmt_euro(v: float) -> str:
    return f"€{v:>15,.0f}"


def fmt_ids(ids: frozenset[str]) -> str:
    return "{" + ", ".join(sorted(ids)) + "}" if ids else "{}"


def print_run(run: RunResult, extra_label: str = "") -> None:
    obj_name = run.objective.value.upper()
    print(f"\n{'─'*60}")
    print(f"  {obj_name}{extra_label}")
    print(f"{'─'*60}")

    speed = run.bf_time / run.py_time if run.py_time > 0 else float("inf")

    rows = [
        ("BruteForce", run.bf_result, run.bf_time),
        ("Pyomo/HiGHS", run.py_result, run.py_time),
    ]
    for label, res, t in rows:
        ids = fmt_ids(res.selected_ids)
        print(f"  {label:<14} geselecteerd = {ids}")
        print(f"  {'':14} NCW_risico   = {fmt_euro(res.risk_ncw)}")
        print(f"  {'':14} investering  = {fmt_euro(res.investment_npv)}")
        print(f"  {'':14} totaal NCW   = {fmt_euro(res.total_ncw)}")
        print(f"  {'':14} tijd         = {t*1000:6.1f} ms")
        print()

    if run.match:
        ratio = f"  (BruteForce {speed:.1f}× langzamer)" if speed > 1 else ""
        print(f"  MATCH ✓{ratio}")
    else:
        print("  AFWIJKING ✗")
        print(f"  BruteForce: {fmt_ids(run.bf_result.selected_ids)}")
        print(f"  Pyomo:      {fmt_ids(run.py_result.selected_ids)}")


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------


def main() -> int:
    physics = SimpleDikeOverflow()
    risk = SimpleRiskCalculator(physics=physics)
    bf = BruteForceOptimizer(risk=risk)
    py = PyomoOptimizer(risk=risk)

    import math

    h_min = math.log(TRAJECTORY.p0 / TRAJECTORY.norm) / TRAJECTORY.alpha

    print("=" * 60)
    print("  FloodOpt — Stap 1.4 Smoke Test")
    print("=" * 60)
    print(f"\n  Traject  : {TRAJECTORY.id}")
    print(f"  norm     : 1/{int(1/TRAJECTORY.norm):,} per jaar")
    print(
        f"  P0       : 1/{int(1/TRAJECTORY.p0):,} per jaar  ->  h_min = {h_min:.3f} m"
    )
    print(f"  Scenario : {SCENARIO.climate}  (η = {SCENARIO.eta} m/jaar)")
    print(f"  Schade   : {fmt_euro(RISK_PARAMS.base_damage)}")
    print(
        f"  Horizon  : {RISK_PARAMS.time_horizon} jaar  δ={RISK_PARAMS.discount_rate}  γ={RISK_PARAMS.gamma}"
    )
    print(f"\n  Kandidaatmaatregelen ({len(CANDIDATES)}):")
    for m in CANDIDATES:
        print(
            f"    {m.id}  Δh={m.effect:.2f}m  {fmt_euro(m.cost)}  jaar={m.year}  {m.location}"
        )

    runs = [
        run_both(bf, py, ObjectiveType.MIN_COST),
        run_both(bf, py, ObjectiveType.MAX_RISK_REDUCTION, budget=BUDGET_MAX_RR),
        run_both(bf, py, ObjectiveType.MIN_NCW),
    ]

    labels = [
        "  —  goedkoopste set die norm haalt",
        f"  —  max Δh binnen budget {fmt_euro(BUDGET_MAX_RR)}",
        "  —  min totale NCW  [lineaire benadering Pyomo]",
    ]
    for run, label in zip(runs, labels):
        print_run(run, label)

    print(f"\n{'='*60}")
    failed = [r for r in runs if not r.match]
    if not failed:
        total_bf = sum(r.bf_time for r in runs)
        total_py = sum(r.py_time for r in runs)
        print(f"  SMOKE TEST GESLAAGD — alle {len(runs)} objectives matchen ✓")
        print(
            f"  Totaaltijd  BruteForce: {total_bf*1000:.0f} ms  |  Pyomo: {total_py*1000:.0f} ms"
        )
    else:
        print(f"  SMOKE TEST MISLUKT — {len(failed)} afwijking(en) gedetecteerd ✗")
        print("  Ontwikkeling gestopt — analyseer oorzaak voor verder te gaan.")
    print("=" * 60)

    return 0 if not failed else 1


if __name__ == "__main__":
    sys.exit(main())
