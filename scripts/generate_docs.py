"""
Genereert alle documentatiediagrammen als PNG met matplotlib.

Uitvoer in docs/:
  architecture.png              Volledige stack
  stap1.1_physics_formula.png   P(t)-grafiek + formule
  stap1.2_risk_ncw.png          NCW-grafiek + formule
  stap1.3_optimization.png      Optimalisatieformuleringen + verificatie
  database_mapping.png          MDB -> SQLite -> FloodOpt mapping
  stap1.4_smoke_test.png        Smoke-test resultaten
  stap2.1_api.png               FastAPI endpoints + request-flow
  stap2.2_database.png          Repository-pattern + schema
  stap2.3_worker.png            Celery + Redis async queue
  geo_stack.png                 GeoJSON + Leaflet flow (stap 4.2)
  stap4_frontend.png            Frontend overzicht stap 4.1-4.4

Gebruik:
    python scripts/generate_docs.py

Design rules:
  - NO FancyBboxPatch with text inside -- use ax.set_facecolor() instead
  - For tables: always ax.table(), never manual text loops
  - Text annotations: always ax.annotate() with explicit xytext offset
  - NO emoji characters -- use (klaar), (gepland), OK, X
  - Every figure: fig.tight_layout(rect=[0, 0, 1, 0.95]) before savefig
  - dpi=150 for all saves
"""

import warnings
import matplotlib

matplotlib.use("Agg")
import matplotlib.patches as mpatches
import matplotlib.pyplot as plt
import matplotlib.gridspec as gridspec
import numpy as np
from pathlib import Path

OUT = Path(__file__).parent.parent / "docs"
OUT.mkdir(exist_ok=True)

BLUE = "#4361ee"
PURPLE = "#7209b7"
GREEN = "#06d6a0"
RED = "#e63946"
ORANGE = "#f4a261"
GREY = "#6c757d"
BG = "#f8f9fa"
WHITE = "#ffffff"
DARK = "#1a1a2e"

plt.rcParams.update(
    {
        "font.family": "DejaVu Sans",
        "mathtext.fontset": "dejavusans",
        "axes.spines.top": False,
        "axes.spines.right": False,
        "figure.facecolor": BG,
        "axes.facecolor": WHITE,
    }
)

# ax.table() axes are intentionally not tight_layout-compatible; suppress the warning.
warnings.filterwarnings(
    "ignore",
    message="This figure includes Axes that are not compatible with tight_layout",
    category=UserWarning,
)


# ---------------------------------------------------------------------------
# Helper: style an ax.table() consistently
# ---------------------------------------------------------------------------


def _style_table(tbl, n_cols, header_color=DARK, alt_color="#f0f2f5"):
    """Apply standard styling to an ax.table() object."""
    tbl.auto_set_font_size(False)
    tbl.set_fontsize(9)
    tbl.scale(1, 1.8)
    tbl.auto_set_column_width(range(n_cols))
    for (row, col), cell in tbl.get_celld().items():
        cell.set_edgecolor("#cccccc")
        cell.set_linewidth(0.5)
        if row == 0:
            cell.set_facecolor(header_color)
            cell.set_text_props(color=WHITE, fontweight="bold", ha="left")
        elif row % 2 == 0:
            cell.set_facecolor(alt_color)
            cell.set_text_props(ha="left")
        else:
            cell.set_facecolor(WHITE)
            cell.set_text_props(ha="left")


# ---------------------------------------------------------------------------
# 1. Architecture
# ---------------------------------------------------------------------------


def make_architecture() -> None:
    """Volledige stack als tabel + beschrijvingstekst."""
    fig = plt.figure(figsize=(12, 7), facecolor=BG)

    fig.suptitle(
        "FloodOpt -- Volledige Architectuur",
        fontsize=15,
        fontweight="bold",
        color=DARK,
        y=0.98,
    )

    gs = gridspec.GridSpec(2, 1, figure=fig, height_ratios=[5, 1], hspace=0.15)

    # --- Top: stack table ---
    ax_tbl = fig.add_subplot(gs[0])
    ax_tbl.axis("off")
    ax_tbl.set_facecolor(BG)

    col_labels = ["Laag", "Technologie / Module", "Status"]
    rows = [
        [
            "Frontend",
            "React + Vite + Tailwind + Leaflet + Recharts",
            "Stap 4.1-4.4 (klaar)",
        ],
        [
            "FastAPI",
            "POST /optimize -> 202  |  GET /results  |  GET /geo/trajectories",
            "Stap 2.1 (klaar)",
        ],
        [
            "Async Queue",
            "Redis (broker) + Celery worker  pending->running->done + p_series",
            "Stap 2.3 (klaar)",
        ],
        [
            "Database",
            "SQLite: scenarios, trajectories (geometry), optimization_results (p_series)",
            "Stap 2.2 (klaar)",
        ],
        [
            "Geometrie",
            "GeoJSON in SQLite JSON-kolom  |  GET /geo/trajectories?year=2050",
            "Stap 4.2 (klaar)",
        ],
        ["Optimization Layer", "ContinuousOptimizer (SLSQP) + BruteForce + Pyomo/HiGHS", "Stap 1.3 + 3.1 (klaar)"],
        ["Risk Layer", "NCW = Sum P(s)*V0*e^((gamma-delta)*s)", "Stap 1.2 (klaar)"],
        [
            "Physics Layer",
            "P(t) = P0*e^(alpha*eta*t)*e^(-alpha*Dh)  +  compute_p_series()",
            "Stap 1.1 + 4.4 (klaar)",
        ],
    ]

    tbl = ax_tbl.table(
        cellText=rows,
        colLabels=col_labels,
        loc="center",
        cellLoc="left",
    )
    _style_table(tbl, n_cols=3)

    # Row-specific background colors for layer identity
    row_colors = [
        BLUE,    # Frontend
        ORANGE,  # FastAPI
        RED,     # Async Queue
        GREY,    # Database
        GREY,    # Geometrie
        BLUE,    # Optimization
        PURPLE,  # Risk
        GREEN,   # Physics
    ]
    for data_row_idx, color in enumerate(row_colors):
        cell = tbl[(data_row_idx + 1, 0)]  # +1 because row 0 is header
        cell.set_facecolor(color)
        cell.set_text_props(color=WHITE, fontweight="bold", ha="left")

    # --- Bottom: description text ---
    ax_txt = fig.add_subplot(gs[1])
    ax_txt.axis("off")
    ax_txt.set_facecolor(BG)
    ax_txt.text(
        0.5,
        0.6,
        "Tech-stack: Python 3.12 | Pyomo 6.x | HiGHS | SQLAlchemy | FastAPI | GeoPandas | React/Vite/Leaflet",
        ha="center",
        va="center",
        fontsize=10,
        color=GREY,
        transform=ax_txt.transAxes,
    )
    ax_txt.text(
        0.5,
        0.15,
        "Fase 1+2+3.1+4 (stap 4.1-4.8) klaar -- 90/90 tests.  "
        "start.bat: Redis + FastAPI + Celery + Vite (4 terminals).",
        ha="center",
        va="center",
        fontsize=9,
        color=GREY,
        transform=ax_txt.transAxes,
    )

    fig.tight_layout(rect=[0, 0, 1, 0.95])
    fig.savefig(OUT / "architecture.png", dpi=150, bbox_inches="tight")
    plt.close(fig)
    print("  architecture.png")


# ---------------------------------------------------------------------------
# 2. Physics
# ---------------------------------------------------------------------------


def make_physics() -> None:
    """P(t) lijn-grafiek voor 3 deltah-waarden + formulepaneel."""
    P0 = 1e-3
    alpha = 5.0
    eta = 0.003
    t = np.linspace(0, 100, 400)

    dh_cases = [0.0, 0.5, 1.0]
    colors = [BLUE, ORANGE, RED]
    labels = ["Dh = 0.0 m", "Dh = 0.5 m", "Dh = 1.0 m"]

    fig = plt.figure(figsize=(12, 8), facecolor=BG)
    gs = gridspec.GridSpec(2, 1, figure=fig, height_ratios=[7, 3], hspace=0.35)

    # --- Top panel: P(t) graph ---
    ax = fig.add_subplot(gs[0])
    ax.set_facecolor(WHITE)

    curves = []
    for dh, col, lbl in zip(dh_cases, colors, labels):
        Pt = P0 * np.exp(alpha * eta * t) * np.exp(-alpha * dh)
        ax.plot(t, Pt * 1e3, color=col, lw=2, label=lbl)
        curves.append(Pt)

    ax.set_xlabel("Tijd t  [jaar]", fontsize=11)
    ax.set_ylabel("P(t)  [x 10$^{-3}$ / jaar]", fontsize=11)
    ax.set_title(
        "Overstromingskans P(t) vs tijd", fontsize=12, fontweight="bold", color=DARK
    )
    ax.legend(loc="upper left", fontsize=10, framealpha=0.85)

    # Annotate test points -- xytext far from the curves to avoid overlap
    test_times = [25, 50, 75]
    ann_offsets = [(40, 25), (30, -35), (-40, 30)]
    for ti, (dx, dy) in zip(test_times, ann_offsets):
        idx = np.searchsorted(t, ti)
        y0 = curves[0][idx] * 1e3
        ax.annotate(
            f"t={ti}j\nP={curves[0][idx]*1e3:.3f}",
            xy=(t[idx], y0),
            xytext=(dx, dy),
            textcoords="offset points",
            fontsize=8,
            color=BLUE,
            arrowprops=dict(arrowstyle="->", color=BLUE, lw=0.8),
            bbox=dict(boxstyle="round,pad=0.3", fc=WHITE, ec=BLUE, lw=0.8, alpha=0.85),
        )

    # --- Bottom panel: formula ---
    ax_f = fig.add_subplot(gs[1])
    ax_f.axis("off")
    ax_f.set_facecolor("#eef2ff")

    ax_f.text(
        0.5,
        0.72,
        r"$P(t) = P_0 \cdot e^{\alpha \eta t} \cdot e^{-\alpha \Delta h}$",
        ha="center",
        va="center",
        fontsize=18,
        transform=ax_f.transAxes,
        color=DARK,
    )
    ax_f.text(
        0.5,
        0.32,
        r"Parameters:  $P_0 = 10^{-3}$ /jaar,  $\alpha = 5.0$ /m,  $\eta = 0.003$ m/jaar",
        ha="center",
        va="center",
        fontsize=11,
        transform=ax_f.transAxes,
        color=GREY,
    )

    fig.suptitle(
        "Stap 1.1 -- Physics Layer",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )
    fig.tight_layout(rect=[0, 0, 1, 0.95])
    fig.savefig(OUT / "stap1.1_physics_formula.png", dpi=150, bbox_inches="tight")
    plt.close(fig)
    print("  stap1.1_physics_formula.png")


# ---------------------------------------------------------------------------
# 3. Risk / NCW
# ---------------------------------------------------------------------------


def make_risk_ncw() -> None:
    """S(s) grafiek voor 3 Dh-waarden + NCW-tabel + formulepaneel."""
    P0 = 1e-3
    alpha = 5.0
    V0 = 1e9
    gamma = 0.02
    delta = 0.04
    T = 50
    eta = 0.0

    s_arr = np.arange(0, T)

    dh_cases = [0.0, 0.5, 1.0]
    colors = [BLUE, ORANGE, RED]
    labels = ["Dh = 0.0 m", "Dh = 0.5 m", "Dh = 1.0 m"]

    fig = plt.figure(figsize=(13, 8), facecolor=BG)
    gs = gridspec.GridSpec(
        2,
        2,
        figure=fig,
        width_ratios=[6, 4],
        height_ratios=[1, 1],
        hspace=0.45,
        wspace=0.35,
    )

    # --- Left (spanning both rows): S(s) graph ---
    ax_l = fig.add_subplot(gs[:, 0])
    ax_l.set_facecolor(WHITE)

    ncw_values = []
    s_curves = []
    for dh, col, lbl in zip(dh_cases, colors, labels):
        Ps = P0 * np.exp(alpha * eta * s_arr) * np.exp(-alpha * dh)
        Ss = Ps * V0 * np.exp((gamma - delta) * s_arr)
        ncw = float(np.sum(Ss))
        ncw_values.append(ncw)
        s_curves.append(Ss)
        ax_l.plot(s_arr, Ss / 1e6, color=col, lw=2, label=lbl)

    ax_l.set_xlabel("Tijdstap s  [jaar]", fontsize=11)
    ax_l.set_ylabel("Schadeterm S(s)  [M EUR]", fontsize=11)
    ax_l.set_title(
        "Jaarlijkse schadeterm S(s) = P(s) * V0 * e^((gamma-delta)*s)",
        fontsize=10,
        fontweight="bold",
        color=DARK,
    )
    ax_l.legend(loc="upper right", fontsize=10, framealpha=0.85)

    # Annotate NCW sum -- place label safely away from all curves
    ncw0_meur = ncw_values[0] / 1e6
    ax_l.annotate(
        f"NCW (Dh=0) = {ncw0_meur:.1f} M EUR",
        xy=(35, s_curves[0][35] / 1e6),
        xytext=(5, -45),
        textcoords="offset points",
        fontsize=9,
        color=BLUE,
        arrowprops=dict(arrowstyle="->", color=BLUE, lw=0.8),
        bbox=dict(boxstyle="round,pad=0.3", fc=WHITE, ec=BLUE, lw=0.8, alpha=0.9),
    )

    # --- Top right: NCW results table ---
    ax_tr = fig.add_subplot(gs[0, 1])
    ax_tr.axis("off")
    ax_tr.set_facecolor(BG)
    ax_tr.set_title("NCW resultaten", fontsize=10, fontweight="bold", color=DARK, pad=6)

    ncw_base = ncw_values[0]
    tbl_rows = []
    for dh, ncw in zip(dh_cases, ncw_values):
        s0_val = P0 * np.exp(-alpha * dh) * V0
        reductie = (ncw_base - ncw) / ncw_base * 100 if ncw_base != 0 else 0.0
        tbl_rows.append(
            [
                f"{dh:.1f} m",
                f"{s0_val/1e6:.2f} M EUR",
                f"{ncw/1e6:.2f} M EUR",
                f"{reductie:.1f}%",
            ]
        )

    tbl = ax_tr.table(
        cellText=tbl_rows,
        colLabels=["Dh", "S(0)", "NCW", "Reductie"],
        loc="center",
        cellLoc="left",
    )
    _style_table(tbl, n_cols=4)

    # --- Bottom right: formula panel ---
    ax_br = fig.add_subplot(gs[1, 1])
    ax_br.axis("off")
    ax_br.set_facecolor("#f0eeff")

    ax_br.text(
        0.5,
        0.72,
        r"$\mathrm{NCW}=\sum_{s=0}^{T-1} P(s)\cdot V_0\cdot e^{(\gamma-\delta)s}$",
        ha="center",
        va="center",
        fontsize=14,
        transform=ax_br.transAxes,
        color=DARK,
    )
    ax_br.text(
        0.5,
        0.28,
        (r"$V_0=10^9$ EUR,  $\gamma=0.02$,  $\delta=0.04$,  $T=50$ jaar"),
        ha="center",
        va="center",
        fontsize=9,
        transform=ax_br.transAxes,
        color=GREY,
    )

    fig.suptitle(
        "Stap 1.2 -- Risk Layer  (NCW)",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )
    fig.tight_layout(rect=[0, 0, 1, 0.95])
    fig.savefig(OUT / "stap1.2_risk_ncw.png", dpi=150, bbox_inches="tight")
    plt.close(fig)
    print("  stap1.2_risk_ncw.png")


# ---------------------------------------------------------------------------
# 4. Optimization
# ---------------------------------------------------------------------------


def make_optimization() -> None:
    """Drie objectiefformuleringen + NCW-grafiek + verificatietabel."""
    fig = plt.figure(figsize=(15, 12), facecolor=BG)
    gs = gridspec.GridSpec(
        3,
        2,
        figure=fig,
        width_ratios=[5, 7],
        height_ratios=[1, 1, 1],
        hspace=0.55,
        wspace=0.35,
    )

    fig.suptitle(
        "Stap 1.3 -- Optimalisatieformuleringen",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )

    # ---- Formula card helper ----
    def _formula_card(ax, bg_color, title, formula, constraint, note):
        ax.set_facecolor(bg_color + "22")
        for spine in ax.spines.values():
            spine.set_visible(False)
        ax.set_xticks([])
        ax.set_yticks([])
        ax.set_title(title, fontsize=11, fontweight="bold", color=DARK, pad=6)
        ax.text(
            0.05,
            0.70,
            formula,
            ha="left",
            va="top",
            fontsize=13,
            transform=ax.transAxes,
            color=DARK,
        )
        ax.text(
            0.05,
            0.38,
            constraint,
            ha="left",
            va="top",
            fontsize=11,
            transform=ax.transAxes,
            color=DARK,
        )
        ax.text(
            0.05,
            0.12,
            note,
            ha="left",
            va="top",
            fontsize=9,
            transform=ax.transAxes,
            color=GREY,
        )

    # Row 0 col 0: MIN_COST
    ax00 = fig.add_subplot(gs[0, 0])
    _formula_card(
        ax00,
        BLUE,
        "MIN_COST  --  Minimale investering",
        r"$\min \sum_{i \in S} C_i$",
        r"$\mathrm{s.t.}\ \mathrm{NCW}(S) \leq \mathrm{NCW}_{norm}$",
        "Kies goedkoopste combinatie die norm haalt",
    )

    # Row 1 col 0: MAX_RISK_REDUCTION
    ax10 = fig.add_subplot(gs[1, 0])
    _formula_card(
        ax10,
        PURPLE,
        "MAX_RISK_RED  --  Maximale risicoreductie",
        r"$\max\ \mathrm{NCW}_0 - \mathrm{NCW}(S)$",
        r"$\mathrm{s.t.}\ \sum C_i \leq B$",
        "Maximaliseer risicoreductie binnen budget B",
    )

    # Row 2 col 0: MIN_NCW
    ax20 = fig.add_subplot(gs[2, 0])
    _formula_card(
        ax20,
        GREEN,
        "MIN_NCW  --  Minimale totale NCW",
        r"$\min\ \mathrm{NCW}(S) + \sum_{i \in S} C_i$",
        r"$S \subseteq \{M_1, \ldots, M_n\}$",
        "Minimaliseer som van kosten en restrisico",
    )

    # ---- NCW vs Dh graph (spans rows 0 and 1, col 1) ----
    ax_ncw = fig.add_subplot(gs[0:2, 1])
    ax_ncw.set_facecolor(WHITE)

    P0 = 1e-3
    alpha = 5.0
    V0 = 1e9
    gamma = 0.02
    delta = 0.04
    T = 50
    dh_range = np.linspace(0, 2.5, 200)

    def _ncw(dh):
        s = np.arange(T)
        Ps = P0 * np.exp(-alpha * dh)
        return float(np.sum(Ps * V0 * np.exp((gamma - delta) * s)))

    ncw_arr = np.array([_ncw(dh) for dh in dh_range]) / 1e6

    ax_ncw.plot(dh_range, ncw_arr, color=BLUE, lw=2.5)
    ax_ncw.set_xlabel("Dijkverhoging Dh  [m]", fontsize=11)
    ax_ncw.set_ylabel("NCW  [M EUR]", fontsize=11)
    ax_ncw.set_title(
        "NCW als functie van dijkverhoging Dh",
        fontsize=11,
        fontweight="bold",
        color=DARK,
    )
    ax_ncw.fill_between(dh_range, ncw_arr, alpha=0.12, color=BLUE)

    # Annotate a few reference points
    for dh_pt, offset in [(0.5, (30, 20)), (1.0, (25, -40)), (1.5, (-55, -35))]:
        ncw_pt = _ncw(dh_pt) / 1e6
        ax_ncw.annotate(
            f"Dh={dh_pt:.1f}m\n{ncw_pt:.1f} M EUR",
            xy=(dh_pt, ncw_pt),
            xytext=offset,
            textcoords="offset points",
            fontsize=8,
            color=DARK,
            arrowprops=dict(arrowstyle="->", color=GREY, lw=0.8),
            bbox=dict(boxstyle="round,pad=0.25", fc=WHITE, ec=GREY, lw=0.7, alpha=0.9),
        )

    # ---- Verification table (row 2, col 1) ----
    ax_ver = fig.add_subplot(gs[2, 1])
    ax_ver.axis("off")
    ax_ver.set_facecolor(BG)
    ax_ver.set_title(
        "Verificatie BruteForce vs Pyomo",
        fontsize=10,
        fontweight="bold",
        color=DARK,
        pad=6,
    )

    ver_rows = [
        ["MIN_COST", "M02, M04", "1,089,224 EUR", "BruteForce", "OK"],
        ["MIN_COST", "M02, M04", "1,089,224 EUR", "Pyomo", "OK"],
        ["MAX_RISK_RED", "M02, M03, M04", "1,862,067 EUR", "BruteForce", "OK"],
        ["MAX_RISK_RED", "M02, M03, M04", "1,862,067 EUR", "Pyomo", "OK"],
        ["MIN_NCW", "alle 5", "3,789,982 EUR", "BruteForce", "OK"],
        ["MIN_NCW", "alle 5", "3,789,982 EUR", "Pyomo", "OK"],
    ]

    tbl = ax_ver.table(
        cellText=ver_rows,
        colLabels=["Objectief", "Maatregelen", "Investering", "Methode", "Status"],
        loc="center",
        cellLoc="left",
    )
    _style_table(tbl, n_cols=5)

    # Color the Status column green
    for row_idx in range(1, len(ver_rows) + 1):
        cell = tbl[(row_idx, 4)]
        cell.set_facecolor("#d4edda")
        cell.set_text_props(color="#155724", fontweight="bold", ha="center")

    fig.tight_layout(rect=[0, 0, 1, 0.95])
    fig.savefig(OUT / "stap1.3_optimization.png", dpi=150, bbox_inches="tight")
    plt.close(fig)
    print("  stap1.3_optimization.png")


# ---------------------------------------------------------------------------
# 5. Database mapping (MDB -> FloodOpt)
# ---------------------------------------------------------------------------


def make_database_mapping() -> None:
    """MDB-tabellen en FloodOpt-modellen als twee naast elkaar staande tabellen."""
    fig = plt.figure(figsize=(13, 8), facecolor=BG)

    fig.suptitle(
        "Database Mapping  --  MDB -> SQLite -> FloodOpt",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )

    gs = gridspec.GridSpec(
        2,
        2,
        figure=fig,
        width_ratios=[1, 1],
        height_ratios=[12, 1],
        hspace=0.15,
        wspace=0.3,
    )

    # --- Left: MDB tables ---
    ax_l = fig.add_subplot(gs[0, 0])
    ax_l.axis("off")
    ax_l.set_facecolor(BG)
    ax_l.set_title(
        "MDB-brontabellen", fontsize=11, fontweight="bold", color=DARK, pad=8
    )

    mdb_rows = [
        ["Dijkringen", "103", "Id, Naam, Terugkeertijd"],
        ["DijkringTrajecten", "176", "H0 [cm], Factor"],
        ["Klimaat_...DataTraject", "3348", "Alpha [1/cm], P0 [1/j], Eta [cm/j]"],
        ["ParametersKostenfunctieData", "183", "Lambda, C_exp, b_exp, Omega"],
        ["SchadeFunctieData", "372", "Nu, Zeta, Psi"],
        ["EconomischScenarioData", "868", "Gamma"],
        ["RamingVoorSlachtoffersData", "372", "Slachtoffers, Getroffenen"],
    ]

    tbl_l = ax_l.table(
        cellText=mdb_rows,
        colLabels=["Tabel", "Rijen", "Sleutelvelden"],
        loc="center",
        cellLoc="left",
    )
    _style_table(tbl_l, n_cols=3)

    # --- Right: FloodOpt models ---
    ax_r = fig.add_subplot(gs[0, 1])
    ax_r.axis("off")
    ax_r.set_facecolor(BG)
    ax_r.set_title(
        "FloodOpt Modellen", fontsize=11, fontweight="bold", color=DARK, pad=8
    )

    floodopt_rows = [
        ["Trajectory", "p0 (= P0_overstromingskans)", "1/jaar"],
        ["Trajectory", "alpha (= Alpha * 100)", "1/m"],
        ["Trajectory", "base_year, norm, length", "-"],
        ["Scenario", "eta (= Eta / 100)", "m/jaar"],
        ["Scenario", "climate, q_design, h_design", "-"],
        ["Measure", "effect (Dh)", "m"],
        ["RiskParams", "base_damage, gamma, delta, T", "-"],
    ]

    tbl_r = ax_r.table(
        cellText=floodopt_rows,
        colLabels=["Model", "Veld", "Eenheid"],
        loc="center",
        cellLoc="left",
    )
    _style_table(tbl_r, n_cols=3)

    # --- Bottom: unit conversion note ---
    ax_note = fig.add_subplot(gs[1, :])
    ax_note.axis("off")
    ax_note.set_facecolor(BG)
    ax_note.text(
        0.5,
        0.5,
        "Eenheidconversies: Alpha [1/cm] * 100 = alpha [1/m]  |  "
        "Eta [cm/jaar] / 100 = eta [m/jaar]  |  "
        "H0 [cm] / 100 = h0 [m]",
        ha="center",
        va="center",
        fontsize=9,
        color=GREY,
        transform=ax_note.transAxes,
    )

    fig.tight_layout(rect=[0, 0, 1, 0.95])
    fig.savefig(OUT / "database_mapping.png", dpi=150, bbox_inches="tight")
    plt.close(fig)
    print("  database_mapping.png")


# ---------------------------------------------------------------------------
# 6. Smoke test
# ---------------------------------------------------------------------------


def make_smoke_test() -> None:
    """Smoke-test resultaten: timing barh + resultaattabel."""
    fig = plt.figure(figsize=(13, 7), facecolor=BG)

    fig.suptitle(
        "Stap 1.4 -- Smoke Test  (N=5 maatregelen, T=100 jaar)",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )

    gs = gridspec.GridSpec(
        2,
        2,
        figure=fig,
        width_ratios=[1, 1],
        height_ratios=[10, 1],
        hspace=0.15,
        wspace=0.35,
    )

    # --- Left: timing bar chart ---
    ax_bar = fig.add_subplot(gs[0, 0])
    ax_bar.set_facecolor(WHITE)

    objectives = ["MIN_COST", "MAX_RISK_RED", "MIN_NCW"]
    bf_times = [3, 5, 13]  # ms
    py_times = [54, 87, 107]  # ms

    y_pos = np.arange(len(objectives))
    height = 0.35

    ax_bar.barh(
        y_pos + height / 2, bf_times, height=height, color=BLUE, label="BruteForce"
    )
    ax_bar.barh(
        y_pos - height / 2, py_times, height=height, color=ORANGE, label="Pyomo"
    )

    ax_bar.set_yticks(y_pos)
    ax_bar.set_yticklabels(objectives, fontsize=10)
    ax_bar.set_xlabel("Rekentijd  [ms]", fontsize=10)
    ax_bar.set_title(
        "Rekentijd per objectief", fontsize=11, fontweight="bold", color=DARK
    )
    ax_bar.legend(loc="lower right", fontsize=9, framealpha=0.85)

    # Value labels
    for i, (bf, py) in enumerate(zip(bf_times, py_times)):
        ax_bar.text(
            bf + 0.5, i + height / 2, f"{bf} ms", va="center", fontsize=8, color=BLUE
        )
        ax_bar.text(
            py + 0.5, i - height / 2, f"{py} ms", va="center", fontsize=8, color=ORANGE
        )

    # --- Right: results table ---
    ax_tbl = fig.add_subplot(gs[0, 1])
    ax_tbl.axis("off")
    ax_tbl.set_facecolor(BG)
    ax_tbl.set_title(
        "Smoke-test resultaten", fontsize=11, fontweight="bold", color=DARK, pad=8
    )

    result_rows = [
        ["MIN_COST", "M02, M04", "1,089,224 EUR", "OK"],
        ["MAX_RISK_RED", "M02, M03, M04", "1,862,067 EUR", "OK"],
        ["MIN_NCW", "alle 5", "3,789,982 EUR", "OK"],
    ]

    tbl = ax_tbl.table(
        cellText=result_rows,
        colLabels=["Objectief", "Optimum", "Investering", "Match"],
        loc="center",
        cellLoc="left",
    )
    _style_table(tbl, n_cols=4)

    # Color Match column green
    for row_idx in range(1, len(result_rows) + 1):
        cell = tbl[(row_idx, 3)]
        cell.set_facecolor("#d4edda")
        cell.set_text_props(color="#155724", fontweight="bold", ha="center")

    # --- Bottom: footer ---
    ax_foot = fig.add_subplot(gs[1, :])
    ax_foot.axis("off")
    ax_foot.set_facecolor(BG)
    ax_foot.text(
        0.5,
        0.5,
        "BruteForce totaal: 21 ms  |  Pyomo totaal: 248 ms  |  Exitcode 0",
        ha="center",
        va="center",
        fontsize=10,
        color=GREY,
        transform=ax_foot.transAxes,
    )

    fig.tight_layout(rect=[0, 0, 1, 0.95])
    fig.savefig(OUT / "stap1.4_smoke_test.png", dpi=150, bbox_inches="tight")
    plt.close(fig)
    print("  stap1.4_smoke_test.png")


# ---------------------------------------------------------------------------
# 7. API
# ---------------------------------------------------------------------------


def make_api() -> None:
    """FastAPI request-flow + endpoints-overzicht."""
    fig = plt.figure(figsize=(13, 8), facecolor=BG)

    fig.suptitle(
        "Stap 2.1 -- FastAPI  (90/90 tests geslaagd)",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )

    gs = gridspec.GridSpec(
        2,
        2,
        figure=fig,
        width_ratios=[9, 11],
        height_ratios=[12, 1],
        hspace=0.15,
        wspace=0.35,
    )

    # --- Left: request flow table ---
    ax_l = fig.add_subplot(gs[0, 0])
    ax_l.axis("off")
    ax_l.set_facecolor(BG)
    ax_l.set_title("Request-flow", fontsize=11, fontweight="bold", color=DARK, pad=8)

    flow_rows = [
        ["1", "Client / Swagger", "HTTP request"],
        ["2", "FastAPI", "Valideer request (Pydantic)"],
        ["3", "get_repositories()", "Kies SQLite of PostgreSQL"],
        ["4", "OrmRepositories", "Sla op / haal op"],
        ["5", "floodopt-core", "Bereken (Physics/Risk/Optimizer)"],
        ["6", "FastAPI", "Return JSON response"],
    ]

    tbl_l = ax_l.table(
        cellText=flow_rows,
        colLabels=["Stap", "Component", "Actie"],
        loc="center",
        cellLoc="left",
    )
    _style_table(tbl_l, n_cols=3)

    # --- Right: endpoints table ---
    ax_r = fig.add_subplot(gs[0, 1])
    ax_r.axis("off")
    ax_r.set_facecolor(BG)
    ax_r.set_title("API-endpoints", fontsize=11, fontweight="bold", color=DARK, pad=8)

    endpoint_rows = [
        ["POST", "/optimize", "202", "async + job_id + input_payload"],
        ["GET", "/results", "200", "lijst alle runs"],
        ["GET", "/results/{job_id}", "200/404", "polling + input_payload"],
        ["DELETE", "/results/{job_id}", "204/404", "optimistic delete"],
        ["GET", "/geo/trajectories", "200", "GeoJSON + p_year"],
        ["GET", "/geo/dijkringdelen", "200", "2011 dijkringdelen + P0"],
        ["GET", "/validation/reference/{dr}/{deel}", "200", "V0 + gamma scenarios"],
    ]

    tbl_r = ax_r.table(
        cellText=endpoint_rows,
        colLabels=["Method", "Endpoint", "Status", "Verificatie"],
        loc="center",
        cellLoc="left",
    )
    _style_table(tbl_r, n_cols=4)

    # Color POST green, GET blue
    method_colors = {
        "POST": ("#d4edda", "#155724"),
        "GET": ("#cce5ff", "#004085"),
        "DELETE": ("#f8d7da", "#721c24"),
    }
    for row_idx in range(1, len(endpoint_rows) + 1):
        method = endpoint_rows[row_idx - 1][0]
        if method in method_colors:
            fc, tc = method_colors[method]
            cell = tbl_r[(row_idx, 0)]
            cell.set_facecolor(fc)
            cell.set_text_props(color=tc, fontweight="bold", ha="center")

    # --- Footer ---
    ax_foot = fig.add_subplot(gs[1, :])
    ax_foot.axis("off")
    ax_foot.set_facecolor(BG)
    ax_foot.text(
        0.5,
        0.5,
        "90/90 tests geslaagd  |  Swagger /docs OK  |  DATABASE_URL bepaalt backend",
        ha="center",
        va="center",
        fontsize=10,
        color=GREY,
        transform=ax_foot.transAxes,
    )

    fig.tight_layout(rect=[0, 0, 1, 0.95])
    fig.savefig(OUT / "stap2.1_api.png", dpi=150, bbox_inches="tight")
    plt.close(fig)
    print("  stap2.1_api.png")


# ---------------------------------------------------------------------------
# 8. Database / Repository pattern
# ---------------------------------------------------------------------------


def make_database() -> None:
    """Repository-pattern tabel + schema tabel + test-breakdown barh."""
    fig = plt.figure(figsize=(13, 7), facecolor=BG)

    fig.suptitle(
        "Stap 2.2 -- Database  (Repository Pattern)",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )

    gs = gridspec.GridSpec(
        3,
        2,
        figure=fig,
        width_ratios=[1, 1],
        height_ratios=[5, 4, 1],
        hspace=0.5,
        wspace=0.35,
    )

    # --- Left top: repository pattern ---
    ax_rep = fig.add_subplot(gs[0, 0])
    ax_rep.axis("off")
    ax_rep.set_facecolor(BG)
    ax_rep.set_title(
        "Repository Pattern", fontsize=10, fontweight="bold", color=DARK, pad=6
    )

    rep_rows = [
        [
            "In-memory",
            "MemoryRepositories",
            "Tests (autouse fixture)",
            "Geen DB vereist",
        ],
        [
            "SQLite (dev)",
            "OrmRepositories",
            "Standaard (ingebouwd)",
            "DATABASE_URL niet ingesteld",
        ],
        [
            "PostgreSQL (prod)",
            "OrmRepositories",
            "DATABASE_URL=postgresql://...",
            "Optioneel",
        ],
    ]

    tbl_rep = ax_rep.table(
        cellText=rep_rows,
        colLabels=["Backend", "Klasse", "Gebruik", "Status"],
        loc="center",
        cellLoc="left",
    )
    _style_table(tbl_rep, n_cols=4)

    # --- Left bottom: schema table ---
    ax_sch = fig.add_subplot(gs[1, 0])
    ax_sch.axis("off")
    ax_sch.set_facecolor(BG)
    ax_sch.set_title(
        "Database Schema", fontsize=10, fontweight="bold", color=DARK, pad=6
    )

    schema_rows = [
        ["scenarios", "id, climate, q_design, h_design, eta"],
        ["trajectories", "id, norm, length, p0, alpha, base_year, geometry (JSON)"],
        [
            "optimization_results",
            "job_id, trajectory_id, objective, status, selected_ids, p_series (JSON)",
        ],
    ]

    tbl_sch = ax_sch.table(
        cellText=schema_rows,
        colLabels=["Tabel", "Sleutelvelden"],
        loc="center",
        cellLoc="left",
    )
    _style_table(tbl_sch, n_cols=2)

    # --- Right: test breakdown bar chart ---
    ax_bar = fig.add_subplot(gs[0:2, 1])
    ax_bar.set_facecolor(WHITE)

    categories = [
        "Unit tests",
        "CLI integration",
        "API integration",
        "DB round-trip",
        "Worker integration",
    ]
    counts = [46, 12, 19, 6, 7]
    colors = [BLUE, ORANGE, GREEN, PURPLE, RED]

    y_pos = np.arange(len(categories))
    bars = ax_bar.barh(y_pos, counts, color=colors, height=0.55, alpha=0.9)

    ax_bar.set_yticks(y_pos)
    ax_bar.set_yticklabels(categories, fontsize=10)
    ax_bar.set_xlabel("Aantal tests", fontsize=10)
    ax_bar.set_title(
        f"Test-verdeling  (totaal: {sum(counts)} tests)",
        fontsize=11,
        fontweight="bold",
        color=DARK,
    )

    for bar, count in zip(bars, counts):
        ax_bar.text(
            bar.get_width() + 0.3,
            bar.get_y() + bar.get_height() / 2,
            str(count),
            va="center",
            fontsize=10,
            color=DARK,
        )

    ax_bar.set_xlim(0, max(counts) * 1.2)

    # --- Footer ---
    ax_foot = fig.add_subplot(gs[2, :])
    ax_foot.axis("off")
    ax_foot.set_facecolor(BG)
    ax_foot.text(
        0.5,
        0.5,
        "90/90 tests geslaagd  |  SQLite standaard  |  PostgreSQL via DATABASE_URL",
        ha="center",
        va="center",
        fontsize=10,
        color=GREY,
        transform=ax_foot.transAxes,
    )

    fig.tight_layout(rect=[0, 0, 1, 0.95])
    fig.savefig(OUT / "stap2.2_database.png", dpi=150, bbox_inches="tight")
    plt.close(fig)
    print("  stap2.2_database.png")


# ---------------------------------------------------------------------------
# 9. Worker (stap 2.3)
# ---------------------------------------------------------------------------


def make_worker() -> None:
    """Celery + Redis async queue: status-flow + componenten-tabel."""
    fig = plt.figure(figsize=(13, 7), facecolor=BG)

    fig.suptitle(
        "Stap 2.3 -- Async Queue  (Redis + Celery)  90/90 tests",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )

    gs = gridspec.GridSpec(
        2,
        2,
        figure=fig,
        width_ratios=[1, 1],
        height_ratios=[12, 1],
        hspace=0.15,
        wspace=0.35,
    )

    # --- Left: status flow table ---
    ax_l = fig.add_subplot(gs[0, 0])
    ax_l.axis("off")
    ax_l.set_facecolor(BG)
    ax_l.set_title("Status-flow", fontsize=11, fontweight="bold", color=DARK, pad=8)

    flow_rows = [
        ["1", "Client", "POST /optimize  (traject + scenario + maatregelen)"],
        ["2", "FastAPI", "Sla pending-record op in SQLite"],
        ["3", "FastAPI", "celery_app.send_task()  ->  202 + job_id"],
        ["4", "Redis", "Taak in wachtrij  (broker)"],
        ["5", "Celery worker", "update_status('running')"],
        ["6", "Celery worker", "optimizer.solve()  ->  selected_measures"],
        ["7", "Celery worker", "compute_p_series()  ->  P(t) + Pmidden per jaar"],
        ["8", "Celery worker", "save_result(status='done', p_series=[...])"],
        ["9", "Client", "GET /results/{job_id}  ->  status + p_series + NCW"],
    ]

    tbl_l = ax_l.table(
        cellText=flow_rows,
        colLabels=["Stap", "Component", "Actie"],
        loc="center",
        cellLoc="left",
    )
    _style_table(tbl_l, n_cols=3)

    # --- Right: component table ---
    ax_r = fig.add_subplot(gs[0, 1])
    ax_r.axis("off")
    ax_r.set_facecolor(BG)
    ax_r.set_title("Componenten", fontsize=11, fontweight="bold", color=DARK, pad=8)

    comp_rows = [
        ["celery_app.py", "floodopt-api", "Celery-instantie + REDIS_URL configuratie"],
        [
            "tasks.py",
            "floodopt-worker",
            "run_optimization: pending->running->done/failed",
        ],
        ["docker-compose.yml", "project root", "Redis 7-alpine op poort 6379"],
        ["start.bat", "project root", "Redis + API + Worker + Frontend in 4 terminals"],
        ["DATABASE_URL", "env-var", "SQLite (1 worker) of PostgreSQL (multi)"],
        ["REDIS_URL", "env-var", "redis://localhost:6379/0 (standaard)"],
    ]

    tbl_r = ax_r.table(
        cellText=comp_rows,
        colLabels=["Bestand", "Package", "Inhoud"],
        loc="center",
        cellLoc="left",
    )
    _style_table(tbl_r, n_cols=3)

    # --- Footer ---
    ax_foot = fig.add_subplot(gs[1, :])
    ax_foot.axis("off")
    ax_foot.set_facecolor(BG)
    ax_foot.text(
        0.5,
        0.5,
        "Tests draaien zonder Redis (task_always_eager)  |  "
        "SQLite bij 1 worker  |  PostgreSQL bij meerdere workers",
        ha="center",
        va="center",
        fontsize=10,
        color=GREY,
        transform=ax_foot.transAxes,
    )

    fig.tight_layout(rect=[0, 0, 1, 0.95])
    fig.savefig(OUT / "stap2.3_worker.png", dpi=150, bbox_inches="tight")
    plt.close(fig)
    print("  stap2.3_worker.png")


# ---------------------------------------------------------------------------
# 10. Geo stack
# ---------------------------------------------------------------------------


def make_geo_stack() -> None:
    """GeoJSON in SQLite + Leaflet kleurcodering (stap 4.2)."""
    fig = plt.figure(figsize=(13, 7), facecolor=BG)

    fig.suptitle(
        "Stap 4.2 -- Geo Stack  (GeoJSON + Leaflet)",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )

    gs = gridspec.GridSpec(
        2,
        2,
        figure=fig,
        width_ratios=[9, 11],
        height_ratios=[12, 1],
        hspace=0.15,
        wspace=0.35,
    )

    # --- Left: flow table ---
    ax_l = fig.add_subplot(gs[0, 0])
    ax_l.axis("off")
    ax_l.set_facecolor(BG)
    ax_l.set_title("GeoJSON-flow", fontsize=11, fontweight="bold", color=DARK, pad=8)

    flow_rows = [
        ["1", "POST /trajectories", "Traject + geometry (GeoJSON dict) opslaan"],
        ["2", "SQLite", "geometry opgeslagen als JSON-kolom"],
        ["3", "GET /geo/trajectories", "FeatureCollection van alle trajecten"],
        ["4", "?year=2050", "p_year per feature uit p_series van laatste job"],
        ["5", "Leaflet (React)", "GeoJSON renderen op kaart"],
        ["6", "Kleurcodering", "P-klasse 1/113000 t/m >1/800 (conform OptimaliseRing)"],
    ]

    tbl_l = ax_l.table(
        cellText=flow_rows,
        colLabels=["Stap", "Component", "Wat"],
        loc="center",
        cellLoc="left",
    )
    _style_table(tbl_l, n_cols=3)

    # --- Right: P-klasse tabel ---
    ax_r = fig.add_subplot(gs[0, 1])
    ax_r.axis("off")
    ax_r.set_facecolor(BG)
    ax_r.set_title(
        "Klasse-indeling kaart (conform OptimaliseRing)",
        fontsize=11,
        fontweight="bold",
        color=DARK,
        pad=8,
    )

    klasse_rows = [
        ["< 1/113.000", "Cyaan", "Zeer veilig"],
        ["1/113.000 - 1/57.000", "Blauw", ""],
        ["1/57.000 - 1/28.000", "Paars", ""],
        ["1/28.000 - 1/14.000", "Roze", ""],
        ["1/14.000 - 1/6.300", "Rood", "Aandacht"],
        ["1/6.300 - 1/2.800", "Oranje", ""],
        ["1/2.800 - 1/1.600", "Geel", ""],
        ["1/1.600 - 1/800", "Lichtgroen", ""],
        ["> 1/800", "Donkergroen", "Urgent"],
    ]

    tbl_r = ax_r.table(
        cellText=klasse_rows,
        colLabels=["Overstromingskans", "Kaartkleur", "Prioriteit"],
        loc="center",
        cellLoc="left",
    )
    _style_table(tbl_r, n_cols=3)

    # --- Footer ---
    ax_foot = fig.add_subplot(gs[1, :])
    ax_foot.axis("off")
    ax_foot.set_facecolor(BG)
    ax_foot.text(
        0.5,
        0.5,
        "Geen GeoPandas/PostGIS nodig voor MVP -- "
        "GeoJSON als JSON-kolom in SQLite | Leaflet rendert direct",
        ha="center",
        va="center",
        fontsize=10,
        color=GREY,
        transform=ax_foot.transAxes,
    )

    fig.tight_layout(rect=[0, 0, 1, 0.95])
    fig.savefig(OUT / "geo_stack.png", dpi=150, bbox_inches="tight")
    plt.close(fig)
    print("  geo_stack.png")


# ---------------------------------------------------------------------------
# 11. Celery + Redis flow diagram
# ---------------------------------------------------------------------------


def make_celery_flow() -> None:
    """Visueel taak-flow diagram: FastAPI -> Celery -> Redis -> worker -> result."""
    fig, ax = plt.subplots(figsize=(14, 3.8), facecolor=BG)
    ax.set_xlim(0, 14)
    ax.set_ylim(0, 3.8)
    ax.axis("off")
    ax.set_facecolor(BG)

    fig.suptitle(
        "Celery + Redis -- Task Queue Flow  (stap 2.3)",
        fontsize=13,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )

    # ---- helpers ----
    def _box(cx, cy, w, h, text, fc=WHITE, fs=8.5):
        ax.add_patch(
            mpatches.FancyBboxPatch(
                (cx - w / 2, cy - h / 2),
                w,
                h,
                boxstyle="round,pad=0.06",
                facecolor=fc,
                edgecolor=DARK,
                linewidth=1.2,
                zorder=2,
            )
        )
        ax.text(
            cx,
            cy,
            text,
            ha="center",
            va="center",
            fontsize=fs,
            color=DARK,
            fontweight="bold",
            multialignment="center",
            linespacing=1.4,
            zorder=3,
        )

    def _cylinder(cx, cy, w, h, text, fc="#d6eaf8"):
        rx, ry = cx - w / 2, cy - h / 2
        eh = h * 0.30
        # body
        ax.add_patch(
            mpatches.Rectangle(
                (rx, ry), w, h, facecolor=fc, edgecolor=fc, linewidth=0, zorder=2
            )
        )
        ax.plot([rx, rx], [ry, ry + h], color=DARK, lw=1.2, zorder=3)
        ax.plot([rx + w, rx + w], [ry, ry + h], color=DARK, lw=1.2, zorder=3)
        # top ellipse
        ax.add_patch(
            mpatches.Ellipse(
                (cx, ry + h),
                w,
                eh,
                facecolor=fc,
                edgecolor=DARK,
                linewidth=1.2,
                zorder=4,
            )
        )
        # bottom arc
        ax.add_patch(
            mpatches.Arc(
                (cx, ry),
                w,
                eh,
                theta1=0,
                theta2=180,
                color=DARK,
                linewidth=1.2,
                zorder=3,
            )
        )
        ax.text(
            cx,
            cy,
            text,
            ha="center",
            va="center",
            fontsize=8.5,
            color=DARK,
            fontweight="bold",
            multialignment="center",
            linespacing=1.4,
            zorder=5,
        )

    def _arrow(x1, y1, x2, y2, label="", lx=None, ly=None, la="center"):
        ax.annotate(
            "",
            xy=(x2, y2),
            xytext=(x1, y1),
            arrowprops=dict(arrowstyle="-|>", color=DARK, lw=1.1, mutation_scale=13),
            zorder=1,
        )
        if label:
            lx = lx if lx is not None else (x1 + x2) / 2
            ly = ly if ly is not None else (y1 + y2) / 2 + 0.13
            ax.text(
                lx,
                ly,
                label,
                ha=la,
                va="bottom",
                fontsize=7.5,
                color=GREY,
                style="italic",
            )

    # ---- layout ----
    TOP = 2.65
    BOT = 0.95
    BH = 1.0
    BW = 1.7
    CW = 1.45
    CH = 0.88

    XA = 0.85  # FastAPI
    XC = 3.1  # Celery client
    XB = 5.35  # Redis broker
    XW = 7.65  # Celery worker
    XF = 10.85  # Python function / run_optimization
    XR = 10.85  # Redis result backend

    # elements
    _box(
        XA,
        (TOP + BOT) / 2,
        1.5,
        2.6,
        "FastAPI\nPOST /optimize\nGET /results",
        fc="#fef9e7",
    )
    _box(XC, TOP, BW, BH, "Celery\nclient")
    _cylinder(XB, TOP, CW, CH, "Redis\nbroker\n/ queue")
    _box(XW, TOP, BW, BH, "Celery\nworker")
    _box(XF, TOP, 2.4, BH, "run_optimization\n(optimizer\n+ p_series)", fc="#eafaf1")
    _cylinder(XR, BOT, CW, CH, "Redis\nresult\nbackend")

    # arrows -- top flow
    _arrow(XA + 0.75, TOP, XC - BW / 2, TOP, "taak plaatsen")
    _arrow(XC + BW / 2, TOP, XB - CW / 2, TOP, "message: taak + args")
    _arrow(XB + CW / 2, TOP, XW - BW / 2, TOP, "pakt taak op")
    _arrow(XW + BW / 2, TOP, XF - 1.2, TOP, "voert uit")

    # Celery worker -> Redis result backend
    _arrow(
        XW + 0.3,
        TOP - BH / 2,
        XR - CW / 2,
        BOT + CH / 2 + 0.12,
        "status + resultaat",
        lx=(XW + 0.3 + XR - CW / 2) / 2 + 0.25,
        ly=(TOP - BH / 2 + BOT + CH / 2 + 0.12) / 2 + 0.04,
        la="left",
    )

    # FastAPI -> Redis result backend (poll)
    _arrow(
        XA + 0.75, BOT + 0.18, XR - CW / 2, BOT + 0.18, "poll resultaat", ly=BOT + 0.32
    )

    # Redis result backend -> FastAPI (result)
    _arrow(
        XR - CW / 2,
        BOT - 0.18,
        XA + 0.75,
        BOT - 0.18,
        "ready / failed / result",
        ly=BOT - 0.38,
    )

    fig.savefig(OUT / "celery_flow.png", dpi=150, bbox_inches="tight")
    plt.close(fig)
    print("  celery_flow.png")


# ---------------------------------------------------------------------------
# 12. Frontend (stap 4.1-4.4)
# ---------------------------------------------------------------------------


def make_frontend() -> None:
    """Frontend overzicht: componenten, paginas en features (stap 4.1-4.4)."""
    fig = plt.figure(figsize=(14, 8), facecolor=BG)

    fig.suptitle(
        "Stap 4.1-4.4 -- Frontend  (React + Vite + Leaflet + Recharts)",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )

    gs = gridspec.GridSpec(
        2,
        2,
        figure=fig,
        width_ratios=[1, 1],
        height_ratios=[11, 1],
        hspace=0.2,
        wspace=0.35,
    )

    # --- Left: paginas en componenten ---
    ax_l = fig.add_subplot(gs[0, 0])
    ax_l.axis("off")
    ax_l.set_facecolor(BG)
    ax_l.set_title(
        "Paginas en componenten", fontsize=11, fontweight="bold", color=DARK, pad=8
    )

    comp_rows = [
        ["Dashboard", "Kaart + JobList + Rijnmond-voorbeeld knop"],
        ["OptimizeForm", "Traject, scenario, maatregelen, doelfunctie"],
        ["Results", "StatusBadge + PSeriesChart + financiele samenvatting"],
        ["MapView", "react-leaflet kaart, P-klasse kleurcodering"],
        ["PSeriesChart", "Recharts: P (groen), Pmidden (blauw), Pwet (zwart)"],
        ["JobList", "Tabel alle jobs, polling 2 s actief / 15 s idle"],
        ["StatusBadge", "Kleurgecodeerde badge: pending/running/done/failed"],
    ]

    tbl_l = ax_l.table(
        cellText=comp_rows,
        colLabels=["Component", "Inhoud"],
        loc="center",
        cellLoc="left",
    )
    _style_table(tbl_l, n_cols=2)

    # --- Right: stap-tabel ---
    ax_r = fig.add_subplot(gs[0, 1])
    ax_r.axis("off")
    ax_r.set_facecolor(BG)
    ax_r.set_title(
        "Stappen en features", fontsize=11, fontweight="bold", color=DARK, pad=8
    )

    stap_rows = [
        ["4.1", "Scaffold", "Vite + React + Tailwind + TanStack Query + CORS"],
        ["4.2", "Kaart + geometrie", "GeoJSON SQLite, Leaflet, P-klasse kleurcodering"],
        ["4.3–4.5", "Job-overzicht + delete", "GET /results, polling, optimistic delete"],
        ["4.4", "P(t)-grafiek", "compute_p_series: P + Pmidden per epoch"],
        ["4.6", "Validatie + V0/gamma", "2011-DB: scenario-selectie V0 en gamma"],
        ["4.7", "Dashboard + Runs", "Kaartlayout, TrajectoryPanel, RunsPage, OptimizeModal"],
        ["4.8", "Results compleet", "input_payload opgeslagen, Opnieuw-knoppen"],
    ]

    tbl_r = ax_r.table(
        cellText=stap_rows,
        colLabels=["Stap", "Onderdeel", "Wat is gebouwd"],
        loc="center",
        cellLoc="left",
    )
    _style_table(tbl_r, n_cols=3)

    # --- Footer ---
    ax_foot = fig.add_subplot(gs[1, :])
    ax_foot.axis("off")
    ax_foot.set_facecolor(BG)
    ax_foot.text(
        0.5,
        0.5,
        "Tech: Vite + React 19 + TypeScript + Tailwind v4 + TanStack Query + "
        "react-leaflet + Recharts  |  Stap 4.1-4.8 klaar",
        ha="center",
        va="center",
        fontsize=9,
        color=GREY,
        transform=ax_foot.transAxes,
    )

    fig.tight_layout(rect=[0, 0, 1, 0.95])
    fig.savefig(OUT / "stap4_frontend.png", dpi=150, bbox_inches="tight")
    plt.close(fig)
    print("  stap4_frontend.png")


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

if __name__ == "__main__":
    print("Diagrammen genereren...")
    make_architecture()
    make_physics()
    make_risk_ncw()
    make_optimization()
    make_database_mapping()
    make_smoke_test()
    make_api()
    make_database()
    make_worker()
    make_celery_flow()
    make_geo_stack()
    make_frontend()
    print(f"\nKlaar -- alle PNG's opgeslagen in {OUT}/")
