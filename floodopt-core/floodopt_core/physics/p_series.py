"""P(t)-tijdreeks voor de optimale dijkversterkingsstrategie.

Voor elk jaar in [base_year, base_year + time_horizon] wordt de
overstromingskans berekend gegeven de geselecteerde maatregelen.

Daarnaast wordt per maatregelinterval (epoch) de midden-overstromingskans
Pmidden = sqrt(P_start * P_end) berekend — identiek aan OptimaliseRing 2.3.2.
"""

from __future__ import annotations

import math

from floodopt_core.io.models import Measure, Scenario, Trajectory
from floodopt_core.physics.simple_dike_overflow import SimpleDikeOverflow

_physics = SimpleDikeOverflow()


def _p(
    trajectory: Trajectory,
    scenario: Scenario,
    active: list[Measure],
    year: int,
) -> float:
    return _physics.compute(trajectory, scenario, active, year).pf_overflow


def compute_p_series(
    trajectory: Trajectory,
    scenario: Scenario,
    selected_measures: list[Measure],
    time_horizon: int,
) -> list[dict[str, float]]:
    """Bereken P(t) en Pmidden voor elk jaar in de planningshorizon.

    Returns:
        Lijst van {"year": int, "p": float, "p_mid": float}.
        p_mid is de geometrische mean van P aan het begin en einde van het
        huidige maatregelinterval — stapsgewijs dalend na elke maatregel.
    """
    sorted_measures = sorted(selected_measures, key=lambda m: m.year)
    end_year = trajectory.base_year + time_horizon

    # Epoch-grenzen: elk jaar waarop een maatregel in werking treedt
    measure_years = sorted({m.year for m in sorted_measures})
    boundaries = [trajectory.base_year] + measure_years + [end_year + 1]

    # Pmidden per epoch (constant binnen een interval)
    epoch_p_mid: list[float] = []
    for i in range(len(boundaries) - 1):
        e_start = boundaries[i]
        e_end = boundaries[i + 1] - 1
        active_start = [m for m in sorted_measures if m.year <= e_start]
        active_end = [m for m in sorted_measures if m.year <= e_end]
        p_s = _p(trajectory, scenario, active_start, e_start)
        p_e = _p(trajectory, scenario, active_end, max(e_start, e_end))
        epoch_p_mid.append(math.sqrt(p_s * p_e))

    def epoch_idx(year: int) -> int:
        for i in range(len(boundaries) - 1):
            if boundaries[i] <= year < boundaries[i + 1]:
                return i
        return len(boundaries) - 2

    series: list[dict[str, float]] = []
    for year in range(trajectory.base_year, end_year + 1):
        active = [m for m in sorted_measures if m.year <= year]
        series.append(
            {
                "year": float(year),
                "p": _p(trajectory, scenario, active, year),
                "p_mid": epoch_p_mid[epoch_idx(year)],
            }
        )

    return series
