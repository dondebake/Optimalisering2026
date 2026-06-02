"""Celery-taken voor FloodOpt.

Taak:
    run_optimization(job_id, payload)
        Voert een optimalisatie uit in de achtergrond.
        Leest trajectory/scenario/candidates uit `payload` (JSON-serialiseerbare dict).
        Schrijft resultaat terug naar de database via OrmRepositories.

Status-flow: pending → running → done  (of → failed bij uitzondering)
"""

from __future__ import annotations

from floodopt_api.celery_app import celery_app


@celery_app.task(name="floodopt_worker.tasks.run_optimization")
def run_optimization(job_id: str, payload: dict) -> None:  # type: ignore[type-arg]
    """Voer de optimalisatie uit en sla het resultaat op in de DB."""
    from floodopt_api.database import get_effective_url, make_session
    from floodopt_api.models import OptimizeResponse
    from floodopt_api.repositories import OrmRepositories
    from floodopt_core.io.models import Measure, Scenario, Trajectory
    from floodopt_core.optimization.brute_force import BruteForceOptimizer
    from floodopt_core.optimization.protocols import ObjectiveType
    from floodopt_core.optimization.pyomo_optimizer import PyomoOptimizer
    from floodopt_core.physics.p_series import compute_p_series
    from floodopt_core.physics.simple_dike_overflow import SimpleDikeOverflow
    from floodopt_core.risk.protocols import RiskParams
    from floodopt_core.risk.simple_risk_calculator import SimpleRiskCalculator

    session = make_session(get_effective_url())
    repos = OrmRepositories(session)

    try:
        repos.update_status(job_id, "running")

        trajectory = Trajectory.model_validate(payload["trajectory"])
        scenario = Scenario.model_validate(payload["scenario"])
        candidates = [Measure.model_validate(m) for m in payload["candidates"]]
        risk_params = RiskParams.model_validate(payload["risk_params"])
        objective = ObjectiveType(payload["objective"])
        budget: float | None = payload.get("budget")
        solver_name: str = payload["solver"]

        physics = SimpleDikeOverflow()
        risk_calc = SimpleRiskCalculator(physics=physics)
        optimizers = {
            "brute_force": BruteForceOptimizer(risk=risk_calc),
            "pyomo": PyomoOptimizer(risk=risk_calc),
        }
        result = optimizers[solver_name].solve(
            trajectory, scenario, candidates, risk_params, objective, budget
        )

        selected_measures = [m for m in candidates if m.id in result.selected_ids]
        p_series = compute_p_series(
            trajectory, scenario, selected_measures, risk_params.time_horizon
        )

        repos.save_result(
            OptimizeResponse(
                job_id=job_id,
                status="done",
                trajectory_id=trajectory.id,
                scenario_id=scenario.id,
                objective=objective,
                solver=solver_name,
                selected_measure_ids=sorted(result.selected_ids),
                total_ncw=result.total_ncw,
                risk_ncw=result.risk_ncw,
                investment_npv=result.investment_npv,
                objective_value=result.objective_value,
                p_series=p_series,
                input_payload=payload,
            )
        )
    except Exception:
        repos.update_status(job_id, "failed")
        raise
    finally:
        session.close()
