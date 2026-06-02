"""Leest OptimaliseRing 2011 referentiedata voor het validatie-dashboard.

Bron: tests/validation/optimalise_ring_2011.sqlite
Views:  v_dijkringen_floodopt  (103 dijkringen, norm in 1/jaar)
        v_trajecten_floodopt   (176 trajecten x 18 klimaatscenario's)

Klimaat_id=1 = basisscenario (huidige omstandigheden).
"""

from __future__ import annotations

import sqlite3
from pathlib import Path

_DB = (
    Path(__file__).parent.parent.parent
    / "tests"
    / "validation"
    / "optimalise_ring_2011.sqlite"
).resolve()


def _conn() -> sqlite3.Connection:
    conn = sqlite3.connect(str(_DB))
    conn.row_factory = sqlite3.Row
    return conn


def get_dijkringen() -> list[dict]:
    with _conn() as con:
        rows = con.execute(
            """
            SELECT d.Dijkring, d.Naam, d.norm_per_jaar,
                   COUNT(*) AS n_trajecten
            FROM v_dijkringen_floodopt d
            LEFT JOIN v_trajecten_floodopt t
                   ON t.Dijkring = d.Dijkring AND t.klimaat_id = 1
            GROUP BY d.Dijkring, d.Naam, d.norm_per_jaar
            ORDER BY CAST(d.Dijkring AS INTEGER)
            """
        ).fetchall()
    return [dict(r) for r in rows]


_TRAJ_SQL = """
    SELECT t.Dijkring, t.DijkringDeel, t.DijkringTraject, t.Naam,
           t.alpha_per_m, t.p0_per_jaar, t.eta_m_per_jaar,
           d.norm_per_jaar,
           k.lambda_exp_per_m, k.C_exp, k.b_exp, k.Omega
    FROM v_trajecten_floodopt t
    JOIN v_dijkringen_floodopt d ON d.Dijkring = t.Dijkring
    LEFT JOIN v_kostenfunctie_floodopt k
           ON k.Dijkring = t.Dijkring
          AND k.DijkringDeel = t.DijkringDeel
          AND k.DijkringTraject = t.DijkringTraject
    WHERE t.klimaat_id = 1
"""


def get_trajectories(dijkring_id: str | None = None) -> list[dict]:
    with _conn() as con:
        if dijkring_id:
            rows = con.execute(
                _TRAJ_SQL + " AND t.Dijkring = ? ORDER BY t.DijkringDeel, t.DijkringTraject",
                (dijkring_id,),
            ).fetchall()
        else:
            rows = con.execute(
                _TRAJ_SQL
                + " ORDER BY CAST(t.Dijkring AS INTEGER), t.DijkringDeel, t.DijkringTraject"
            ).fetchall()
    return [dict(r) for r in rows]


def get_trajectory(dijkring: str, deel: float, traject: float) -> dict | None:
    with _conn() as con:
        row = con.execute(
            _TRAJ_SQL + " AND t.Dijkring = ? AND t.DijkringDeel = ? AND t.DijkringTraject = ?",
            (dijkring, deel, traject),
        ).fetchone()
    return dict(row) if row else None
