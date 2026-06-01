"""In-memory opslag voor MVP (stap 2.1). Vervangen door PostgreSQL in stap 2.2."""

from __future__ import annotations

from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from floodopt_api.models import OptimizeResponse
    from floodopt_core.io.models import Scenario, Trajectory

_scenarios: dict[str, Scenario] = {}
_trajectories: dict[str, Trajectory] = {}
_results: dict[str, OptimizeResponse] = {}


def reset() -> None:
    """Leeg de store — alleen voor testgebruik."""
    _scenarios.clear()
    _trajectories.clear()
    _results.clear()
