"""
Genereert alle documentatiediagrammen als PNG met matplotlib mathtext.

Uitvoer in docs/:
  architecture.png              Volledige stack
  stap1.1_physics_formula.png   P(t)-grafiek + formule
  stap1.2_risk_ncw.png          NCW-grafiek + formule
  stap1.3_optimization.png      Optimalisatieformuleringen + verificatie
  database_mapping.png          MDB -> SQLite -> FloodOpt mapping
  stap1.4_smoke_test.png        Smoke-test flow + resultaten
  stap2.1_api.png               FastAPI endpoints + request-flow
  stap2.2_database.png          Repository-pattern + schema
  geo_stack.png                 GeoPandas + Leaflet flow

Gebruik:
    python scripts/generate_docs.py
"""

import matplotlib

matplotlib.use("Agg")
import matplotlib.pyplot as plt
import matplotlib.patches as mpatches
import matplotlib.gridspec as gridspec
import numpy as np
import math
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


# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------


def _box(ax, x, y, w, h, fc, ec, lw=1.5, alpha=1.0, radius=0.15):
    """Draw a rounded rectangle.  x, y = bottom-left corner."""
    patch = mpatches.FancyBboxPatch(
        (x, y),
        w,
        h,
        boxstyle=f"round,pad={radius}",
        facecolor=fc,
        edgecolor=ec,
        linewidth=lw,
        alpha=alpha,
        zorder=3,
    )
    ax.add_patch(patch)
    return patch


def _arrow(ax, x0, y0, x1, y1, color=GREY, lw=1.8, style="-|>"):
    ax.annotate(
        "",
        xy=(x1, y1),
        xytext=(x0, y0),
        arrowprops=dict(arrowstyle=style, color=color, lw=lw),
        zorder=4,
    )


# ---------------------------------------------------------------------------
# 1. Architectuur
# ---------------------------------------------------------------------------


def make_architecture() -> None:
    """Volledige stack: Frontend -> FastAPI -> Database/Geo -> floodopt-core."""
    fig, ax = plt.subplots(figsize=(12, 11), facecolor=BG)
    ax.set_xlim(0, 12)
    ax.set_ylim(0, 13)
    ax.axis("off")
    ax.set_facecolor(BG)

    fig.suptitle(
        "FloodOpt  --  Volledige Architectuur",
        fontsize=16,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )

    # ---- row y-centres (top to bottom) ----
    Y_FE = 11.2  # Frontend
    Y_API = 9.0  # FastAPI
    Y_DB = 7.0  # Database + Geo (side by side)
    Y_OPT = 5.0  # Optimization Layer
    Y_RISK = 3.1  # Risk Layer
    Y_PHYS = 1.2  # Physics Layer

    BOX_H = 1.3  # standard box height
    FULL_W = 11.0  # full-width box
    X0 = 0.5  # left margin

    # ---- Frontend ----
    _box(ax, X0, Y_FE - BOX_H / 2, FULL_W, BOX_H, "#0077b6", "#023e8a", lw=2)
    ax.text(
        6.0,
        Y_FE + 0.25,
        "Frontend  --  React + Vite + Leaflet",
        ha="center",
        va="center",
        fontsize=13,
        fontweight="bold",
        color=WHITE,
    )
    ax.text(
        6.0,
        Y_FE - 0.22,
        "Kaartviewer (GeoJSON)  |  Scenario-editor  |  Resultaten-dashboard",
        ha="center",
        va="center",
        fontsize=10,
        color="#cce8ff",
    )
    ax.text(
        11.2,
        Y_FE + 0.25,
        "Stap 4 (gepland)",
        ha="right",
        va="center",
        fontsize=9,
        color=WHITE,
        fontstyle="italic",
    )

    # arrow FE -> API
    _arrow(ax, 6.0, Y_FE - BOX_H / 2, 6.0, Y_API + BOX_H / 2, color=GREY)
    ax.text(
        6.3,
        (Y_FE - BOX_H / 2 + Y_API + BOX_H / 2) / 2,
        "HTTP / GeoJSON",
        va="center",
        fontsize=9,
        color=GREY,
    )

    # ---- FastAPI ----
    _box(ax, X0, Y_API - BOX_H / 2, FULL_W, BOX_H, ORANGE, "#c76b00", lw=2)
    ax.text(
        6.0,
        Y_API + 0.25,
        "FastAPI  --  dunne HTTP-schil",
        ha="center",
        va="center",
        fontsize=13,
        fontweight="bold",
        color=DARK,
    )
    ax.text(
        3.5,
        Y_API - 0.22,
        "POST /optimize   POST /scenarios   POST /trajectories",
        ha="center",
        va="center",
        fontsize=9,
        color="#333",
    )
    ax.text(
        8.5,
        Y_API - 0.22,
        "GET /results/{id}   GET /trajectories/{id}/geojson",
        ha="center",
        va="center",
        fontsize=9,
        color="#333",
    )
    ax.text(
        11.2,
        Y_API + 0.25,
        "Stap 2.x (klaar)",
        ha="right",
        va="center",
        fontsize=9,
        color=DARK,
        fontstyle="italic",
    )

    # arrow API -> Database (left)
    _arrow(ax, 3.2, Y_API - BOX_H / 2, 3.0, Y_DB + 0.7, color=GREY)
    ax.text(
        2.2,
        (Y_API - BOX_H / 2 + Y_DB + 0.7) / 2,
        "SQLAlchemy",
        ha="center",
        fontsize=8,
        color=GREY,
    )

    # arrow API -> Geo (right)
    _arrow(ax, 8.8, Y_API - BOX_H / 2, 9.0, Y_DB + 0.7, color="#0077b6")
    ax.text(
        9.8,
        (Y_API - BOX_H / 2 + Y_DB + 0.7) / 2,
        "GeoPandas",
        ha="center",
        fontsize=8,
        color="#0077b6",
    )

    # ---- Database (left) ----
    DB_W = 5.0
    _box(ax, X0, Y_DB - 0.75, DB_W, 1.5, "#f1f3f5", GREY, lw=1.5)
    ax.text(
        X0 + DB_W / 2,
        Y_DB + 0.4,
        "Database",
        ha="center",
        va="center",
        fontsize=11,
        fontweight="bold",
        color=DARK,
    )
    ax.text(
        X0 + DB_W / 2,
        Y_DB + 0.0,
        "SQLite  (dev, ingebouwd in Python)",
        ha="center",
        va="center",
        fontsize=9,
        color="#333",
    )
    ax.text(
        X0 + DB_W / 2,
        Y_DB - 0.38,
        "PostgreSQL  (prod, via DATABASE_URL)",
        ha="center",
        va="center",
        fontsize=8,
        color=GREY,
    )
    ax.text(
        X0 + DB_W - 0.2,
        Y_DB + 0.4,
        "Stap 2.2 (klaar)",
        ha="right",
        va="center",
        fontsize=8,
        color=GREY,
        fontstyle="italic",
    )

    # ---- Geo (right) ----
    GEO_X = 6.5
    GEO_W = 5.0
    _box(ax, GEO_X, Y_DB - 0.75, GEO_W, 1.5, "#e8f4fd", "#0077b6", lw=1.5)
    ax.text(
        GEO_X + GEO_W / 2,
        Y_DB + 0.4,
        "Geo-verwerking",
        ha="center",
        va="center",
        fontsize=11,
        fontweight="bold",
        color="#023e8a",
    )
    ax.text(
        GEO_X + GEO_W / 2,
        Y_DB + 0.0,
        "GeoPandas:  WKT / shapefile  ->  GeoJSON",
        ha="center",
        va="center",
        fontsize=9,
        color="#0077b6",
    )
    ax.text(
        GEO_X + GEO_W / 2,
        Y_DB - 0.38,
        "Geen PostGIS nodig voor MVP",
        ha="center",
        va="center",
        fontsize=8,
        color=GREY,
    )
    ax.text(
        GEO_X + GEO_W - 0.2,
        Y_DB + 0.4,
        "Stap 4.2 (gepland)",
        ha="right",
        va="center",
        fontsize=8,
        color="#0077b6",
        fontstyle="italic",
    )

    # arrow DB/Geo -> core
    _arrow(ax, 6.0, Y_DB - 0.75, 6.0, Y_OPT + BOX_H / 2 + 0.1, color=GREY)
    ax.text(
        6.3,
        (Y_DB - 0.75 + Y_OPT + BOX_H / 2) / 2,
        "roept aan",
        va="center",
        fontsize=9,
        color=GREY,
    )

    # ---- floodopt-core layers ----
    core_layers = [
        (
            Y_OPT,
            BLUE,
            "#3a0ca3",
            "Optimization Layer",
            r"$\min \sum_i c_i x_i \;\;\mathrm{s.t.}\;\; \sum_i h_i x_i \geq h_{\min}$",
            "BruteForce  +  Pyomo / HiGHS",
            "Stap 1.3 (klaar)",
        ),
        (
            Y_RISK,
            PURPLE,
            "#560bad",
            "Risk Layer",
            r"$\mathrm{NCW} = \sum_{s=0}^{T-1} P(s)\cdot V_0 \cdot e^{(\gamma-\delta)\,s}$",
            "SimpleRiskCalculator",
            "Stap 1.2 (klaar)",
        ),
        (
            Y_PHYS,
            GREEN,
            "#05b484",
            "Physics Layer",
            r"$P(t) = P_0 \cdot e^{\alpha \eta t} \cdot e^{-\alpha \Delta h}$",
            "SimpleDikeOverflow",
            "Stap 1.1 (klaar)",
        ),
    ]

    for y_c, fc, ec, title, formula, sub, badge in core_layers:
        _box(ax, X0, y_c - BOX_H / 2, FULL_W, BOX_H, fc, ec, lw=2)
        ax.text(
            6.0,
            y_c + 0.30,
            title,
            ha="center",
            va="center",
            fontsize=12,
            fontweight="bold",
            color=WHITE,
        )
        ax.text(
            6.0,
            y_c - 0.10,
            formula,
            ha="center",
            va="center",
            fontsize=10,
            color=WHITE,
            alpha=0.95,
        )
        ax.text(
            6.0,
            y_c - 0.48,
            sub,
            ha="center",
            va="center",
            fontsize=9,
            color=WHITE,
            alpha=0.80,
        )
        ax.text(
            11.2,
            y_c + 0.30,
            badge,
            ha="right",
            va="center",
            fontsize=9,
            color=WHITE,
            fontstyle="italic",
        )

    # arrows between core layers
    for y_top, y_bot in [
        (Y_OPT - BOX_H / 2, Y_RISK + BOX_H / 2),
        (Y_RISK - BOX_H / 2, Y_PHYS + BOX_H / 2),
    ]:
        _arrow(ax, 6.0, y_top, 6.0, y_bot, color=WHITE, lw=1.5)

    plt.tight_layout(rect=[0, 0, 1, 0.98])
    plt.savefig(OUT / "architecture.png", dpi=150, bbox_inches="tight")
    plt.close()
    print("  architecture.png")


# ---------------------------------------------------------------------------
# 2. Physics Layer P(t)
# ---------------------------------------------------------------------------


def make_physics() -> None:
    P0, ALPHA, ETA = 1e-3, 5.0, 0.003
    t = np.linspace(0, 100, 400)

    P_no = P0 * np.exp(ALPHA * ETA * t)
    P_05 = P0 * np.exp(ALPHA * ETA * t) * np.exp(-ALPHA * 0.5)
    P_10 = P0 * np.exp(ALPHA * ETA * t) * np.exp(-ALPHA * 1.0)

    fig = plt.figure(figsize=(11, 8), facecolor=BG)
    gs = gridspec.GridSpec(
        2,
        1,
        height_ratios=[3, 1],
        hspace=0.45,
        top=0.92,
        bottom=0.10,
        left=0.10,
        right=0.96,
    )
    ax = fig.add_subplot(gs[0])
    ax_form = fig.add_subplot(gs[1])

    ax.plot(
        t, P_no * 1e3, color=RED, lw=2.5, label=r"$\Delta h = 0$ m  (geen maatregelen)"
    )
    ax.plot(t, P_05 * 1e3, color=ORANGE, lw=2.5, ls="--", label=r"$\Delta h = 0.5$ m")
    ax.plot(t, P_10 * 1e3, color=GREEN, lw=2.5, ls=":", label=r"$\Delta h = 1.0$ m")

    # reference line P = 1e-3 (initial value)
    ax.axhline(1.0, color=RED, lw=0.8, ls="-", alpha=0.3)

    # testcase markers
    v_case2 = P0 * math.exp(ALPHA * ETA * 50) * 1e3
    v_case3 = P0 * math.exp(-ALPHA * 1.0) * 1e3
    ax.scatter([0], [P0 * 1e3], color=RED, s=60, zorder=5)
    ax.scatter(
        [50],
        [v_case2],
        color=RED,
        marker="^",
        s=70,
        zorder=5,
        label=r"TC2: $t=50$ jaar,  $P \approx 2.12 \times 10^{-3}$",
    )
    ax.scatter(
        [0],
        [v_case3],
        color=GREEN,
        marker="s",
        s=60,
        zorder=5,
        label=r"TC3: $\Delta h = 1$ m,  $P \approx 6.7 \times 10^{-6}$",
    )

    ax.annotate(
        r"$P_0 = 10^{-3}$",
        xy=(0, P0 * 1e3),
        xytext=(10, 1.4),
        fontsize=10,
        color=RED,
        arrowprops=dict(arrowstyle="->", color=RED, lw=1.2),
    )

    ax.set_xlabel(r"$t$  [jaar na basisjaar 2017]", fontsize=12)
    ax.set_ylabel(r"$P(t)$   [$\times 10^{-3}$ jaar$^{-1}$]", fontsize=12)
    ax.set_title(
        "Stap 1.1  --  SimpleDikeOverflow",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        pad=10,
    )
    ax.legend(fontsize=10, framealpha=0.9, loc="upper left")
    ax.set_xlim(0, 100)
    ax.set_ylim(bottom=0)

    # formula panel
    ax_form.axis("off")
    ax_form.set_facecolor(WHITE)
    formula_text = (
        r"$P(t) = P_0 \cdot e^{\alpha \eta t} \cdot e^{-\alpha \Delta h}$"
        "\n\n"
        r"$P_0 = 10^{-3}$ jaar$^{-1}$"
        r"     $\alpha = 5.0$ m$^{-1}$"
        r"     $\eta = 0.003$ m/jaar (W+)"
        r"     $\Delta h = \sum_i h_i$  [m]"
    )
    ax_form.text(
        0.5,
        0.55,
        formula_text,
        ha="center",
        va="center",
        fontsize=11,
        transform=ax_form.transAxes,
        bbox=dict(
            boxstyle="round,pad=0.5", facecolor="#e8f0fe", edgecolor=BLUE, linewidth=1.5
        ),
    )
    ax_form.text(
        0.98,
        0.05,
        "Identiek aan OptimaliseRing 2.3.2 (HKV, 2013)  |  7/7 tests OK  rel_tol = 1e-9",
        ha="right",
        va="bottom",
        fontsize=8,
        color=GREY,
        transform=ax_form.transAxes,
    )

    plt.savefig(OUT / "stap1.1_physics_formula.png", dpi=150, bbox_inches="tight")
    plt.close()
    print("  stap1.1_physics_formula.png")


# ---------------------------------------------------------------------------
# 3. Risk Layer NCW
# ---------------------------------------------------------------------------


def make_risk_ncw() -> None:
    P0, ALPHA, V0, GAMMA, DELTA, T = 1e-3, 5.0, 1e9, 0.02, 0.04, 50
    s = np.arange(T)

    def S(dh: float) -> np.ndarray:
        return P0 * np.exp(-ALPHA * dh) * V0 * np.exp((GAMMA - DELTA) * s)

    def ncw(dh: float) -> float:
        return float(np.sum(S(dh)))

    S0 = S(0.0)
    S05 = S(0.5)
    S10 = S(1.0)

    fig = plt.figure(figsize=(12, 9), facecolor=BG)
    gs = gridspec.GridSpec(
        2,
        2,
        height_ratios=[2.5, 1],
        hspace=0.55,
        wspace=0.40,
        top=0.92,
        bottom=0.08,
        left=0.09,
        right=0.97,
    )
    ax = fig.add_subplot(gs[0, :])
    ax_table = fig.add_subplot(gs[1, 0])
    ax_form = fig.add_subplot(gs[1, 1])

    ax.fill_between(s, S0 / 1e6, alpha=0.10, color=RED)
    ax.plot(
        s,
        S0 / 1e6,
        color=RED,
        lw=2.5,
        label=r"$\Delta h = 0$ m    (NCW $\approx$ 31.9 M€)",
    )
    ax.plot(
        s,
        S05 / 1e6,
        color=ORANGE,
        lw=2.5,
        ls="--",
        label=r"$\Delta h = 0.5$ m  (NCW $\approx$ 2.6 M€)",
    )
    ax.plot(
        s,
        S10 / 1e6,
        color=GREEN,
        lw=2.5,
        ls=":",
        label=r"$\Delta h = 1.0$ m  (NCW $\approx$ 0.22 M€)",
    )

    ax.annotate(
        f"NCW = {ncw(0)/1e6:.1f} M€",
        xy=(25, S0[25] / 1.8e6),
        fontsize=11,
        color=RED,
        fontstyle="italic",
    )

    ax.set_xlabel(r"$s$  [jaar na basisjaar]", fontsize=12)
    ax.set_ylabel(r"$S(s) = P(s) \cdot V(s)$   [M€/jaar]", fontsize=12)
    ax.set_title(
        "Stap 1.2  --  Risk Layer: verwachte schade en NCW",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        pad=10,
    )
    ax.legend(fontsize=10, framealpha=0.9, loc="upper right")

    # table
    ax_table.axis("off")
    ax_table.set_title(
        "Verificatietabel  (T = 50 jaar)", fontsize=10, fontweight="bold", pad=8
    )
    headers = ["Dh [m]", "S(s=0) [EUR]", "NCW [EUR]", "Reductie"]
    rows_data = [
        ["0", "1 000 000", "31 923 000", "--"],
        ["0.5", "   82 085", " 2 620 000", "-91.8%"],
        ["1.0", "    6 738", "   215 000", "-99.3%"],
    ]
    tbl = ax_table.table(
        cellText=rows_data,
        colLabels=headers,
        loc="center",
        cellLoc="center",
    )
    tbl.auto_set_font_size(False)
    tbl.set_fontsize(9)
    tbl.scale(1, 1.8)
    for (r, c), cell in tbl.get_celld().items():
        if r == 0:
            cell.set_facecolor(BLUE)
            cell.set_text_props(color=WHITE, fontweight="bold")
        elif c == 3 and r > 0:
            cell.set_text_props(color="#059669", fontweight="bold")
        elif r % 2 == 0 and r > 0:
            cell.set_facecolor("#f8f9fa")

    # formula
    ax_form.axis("off")
    ax_form.set_title("Formule", fontsize=10, fontweight="bold", pad=8)
    ax_form.text(
        0.5,
        0.70,
        r"$\mathrm{NCW} = \sum_{s=0}^{T-1} P(s)\cdot V_0\cdot e^{(\gamma-\delta)\,s}$",
        ha="center",
        va="center",
        fontsize=13,
        transform=ax_form.transAxes,
        bbox=dict(
            boxstyle="round,pad=0.4",
            facecolor="#f3e8ff",
            edgecolor=PURPLE,
            linewidth=1.5,
        ),
    )
    ax_form.text(
        0.5,
        0.35,
        r"$V(s) = V_0 e^{\gamma s}$"
        "\n"
        r"$V_0 = 10^9$ EUR     $\gamma = 0.02$     $\delta = 0.04$     $T = 50$ jaar",
        ha="center",
        va="center",
        fontsize=10,
        transform=ax_form.transAxes,
    )
    ax_form.text(
        0.5,
        0.05,
        "10/10 tests OK  |  rel_tol = 1e-9",
        ha="center",
        va="bottom",
        fontsize=8,
        color=GREY,
        transform=ax_form.transAxes,
    )

    plt.savefig(OUT / "stap1.2_risk_ncw.png", dpi=150, bbox_inches="tight")
    plt.close()
    print("  stap1.2_risk_ncw.png")


# ---------------------------------------------------------------------------
# 4. Optimization Layer
# ---------------------------------------------------------------------------


def make_optimization() -> None:
    fig = plt.figure(figsize=(14, 10), facecolor=BG)
    fig.suptitle(
        "Stap 1.3  --  Optimization Layer:  BruteForce = Pyomo / HiGHS",
        fontsize=15,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )
    gs = gridspec.GridSpec(
        2,
        2,
        hspace=0.55,
        wspace=0.45,
        top=0.93,
        bottom=0.05,
        left=0.05,
        right=0.97,
    )

    # ---------- Panel 1: objective formulations ----------
    ax1 = fig.add_subplot(gs[0, 0])
    ax1.axis("off")
    ax1.set_title(
        "Optimalisatieformuleringen", fontsize=12, fontweight="bold", color=DARK, pad=10
    )

    objectives = [
        (
            "MIN_COST",
            BLUE,
            r"$\min\;\sum_i c_i x_i$",
            r"s.t. $\sum_i h_i x_i \geq h_{\min}$",
            r"$h_{\min} = \ln(P_0/\mathrm{norm})\,/\,\alpha$",
        ),
        (
            "MAX_RISK_REDUCTION",
            GREEN,
            r"$\max\;\sum_i h_i x_i$",
            r"s.t. $\sum_i c_i x_i \leq B$",
            "(0/1-knapsack)",
        ),
        (
            "MIN_NCW  (lineair)",
            ORANGE,
            r"$\min\;\sum_i (c_i - C\alpha h_i)\,x_i$",
            r"$C\alpha h_i \approx \Delta\mathrm{NCW}_i$",
            r"geldig voor $\alpha h_i < 0.5$",
        ),
    ]

    y_positions = [0.88, 0.55, 0.22]
    for (label, color, line1, line2, line3), yp in zip(objectives, y_positions):
        # label
        ax1.text(
            0.03,
            yp + 0.06,
            label,
            transform=ax1.transAxes,
            fontsize=10,
            fontweight="bold",
            color=color,
        )
        # formula box
        box_text = line1 + "\n" + line2 + "\n" + line3
        ax1.text(
            0.03,
            yp - 0.14,
            box_text,
            transform=ax1.transAxes,
            fontsize=10,
            color="#212529",
            bbox=dict(
                boxstyle="round,pad=0.35",
                facecolor=color + "22",
                edgecolor=color,
                linewidth=1.2,
            ),
            va="top",
        )

    # ---------- Panel 2: NCW vs Dh ----------
    ax2 = fig.add_subplot(gs[0, 1])
    P0, ALPHA, V0, GAMMA, DELTA, T_HOR = 1e-3, 5.0, 1e9, 0.02, 0.04, 50
    C = P0 * V0 * sum(math.exp((GAMMA - DELTA) * s) for s in range(T_HOR))
    dh = np.linspace(0, 2, 300)
    ncw_exact = C * np.exp(-ALPHA * dh)
    ncw_linear = C * (1 - ALPHA * dh)
    ncw_linear = np.clip(ncw_linear, 0, None)

    ax2.plot(
        dh,
        ncw_exact / 1e6,
        color=BLUE,
        lw=2.5,
        label=r"Exact: $C \cdot e^{-\alpha\Delta h}$",
    )
    ax2.plot(
        dh,
        ncw_linear / 1e6,
        color=ORANGE,
        lw=2.0,
        ls="--",
        label=r"Lineair: $C(1 - \alpha\Delta h)$",
    )
    ax2.axvline(0.5, color=GREY, lw=1, ls=":")
    ax2.text(
        0.53,
        ax2.get_ylim()[1] * 0.1 if False else 2.0,
        r"$\alpha\Delta h = 0.5$" + "\n(grens linearisatie)",
        fontsize=8,
        color=GREY,
        va="bottom",
    )
    ax2.set_xlabel(r"$\Delta h$  [m]", fontsize=11)
    ax2.set_ylabel(r"NCW$_\mathrm{risico}$  [M€]", fontsize=11)
    ax2.set_title(r"NCW als functie van $\Delta h$", fontsize=12, color=DARK, pad=10)
    ax2.legend(fontsize=10, loc="upper right")
    ax2.set_xlim(0, 2)
    ax2.set_ylim(bottom=0)

    # ---------- Panel 3: TC1 bar chart ----------
    ax3 = fig.add_subplot(gs[1, 0])
    ax3.set_title(
        "TC1  --  MIN_COST:  optimale maatregelenkeuze", fontsize=11, color=DARK, pad=10
    )

    h_min = math.log(1e-2 / 1e-3) / 5.0  # ~0.461 m
    measures = [
        ("M01", 0.50, 2.0, RED),
        ("M02", 0.30, 1.0, BLUE),
        ("M03", 0.20, 0.5, GREEN),
        ("M02+M03", 0.50, 1.5, PURPLE),
    ]
    names = [m[0] for m in measures]
    costs = [m[2] for m in measures]
    dh_v = [m[1] for m in measures]
    colors = [m[3] for m in measures]

    bars = ax3.bar(
        range(len(names)),
        costs,
        color=colors,
        alpha=0.85,
        edgecolor=WHITE,
        linewidth=1.5,
    )
    ax3.set_xticks(range(len(names)))
    ax3.set_xticklabels(names, fontsize=10)
    ax3.axhline(1.5, color=PURPLE, lw=1.5, ls="--", alpha=0.6)
    ax3.set_ylabel("Investering  [M€]", fontsize=10)
    ax3.set_ylim(0, 2.8)
    ax3.set_title(
        "TC1  --  MIN_COST:  optimale maatregelenkeuze", fontsize=11, color=DARK, pad=10
    )
    ax3.set_title("")  # already set via suptitle equivalent above

    for bar, h in zip(bars, dh_v):
        ok_str = "OK" if h >= h_min else "X"
        ok_clr = "#059669" if h >= h_min else RED
        label = f"Dh={h:.2f}m  {ok_str}"
        ax3.text(
            bar.get_x() + bar.get_width() / 2,
            bar.get_height() + 0.07,
            label,
            ha="center",
            fontsize=9,
            color=ok_clr,
            fontweight="bold",
        )

    ax3.text(
        3,
        1.62,
        "Optimum\n1.5 M€",
        ha="center",
        fontsize=9,
        color=PURPLE,
        fontweight="bold",
    )
    ax3.text(
        0.5,
        0.04,
        f"hmin = {h_min:.3f} m     BruteForce = Pyomo: {{M02, M03}} OK",
        ha="center",
        fontsize=9,
        color="#059669",
        fontweight="bold",
        transform=ax3.transAxes,
    )

    # ---------- Panel 4: verification table ----------
    ax4 = fig.add_subplot(gs[1, 1])
    ax4.axis("off")
    ax4.set_title(
        "Verificatieresultaten  (6 testcases)", fontsize=11, color=DARK, pad=10
    )

    test_results = [
        ("TC1", "MIN_COST", "M02+M03 goedkoopst", True),
        ("TC2", "MIN_COST", "Norm al gehaald -> {}", True),
        ("TC3", "MIN_COST", "Dependency M02 -> M01", True),
        ("TC4", "MAX_RR", "Knapsack binnen budget", True),
        ("TC5", "MIN_NCW", "alpha*h=0.1,  fout < 1%", True),
        ("TC6", "MIN_NCW", "Duur vs. voordelig", True),
    ]

    # column header
    col_x = [0.03, 0.18, 0.48, 0.85]
    header_y = 0.93
    for txt, cx in zip(["TC", "Objective", "Scenario", "Status"], col_x):
        ax4.text(
            cx,
            header_y,
            txt,
            transform=ax4.transAxes,
            fontsize=9,
            fontweight="bold",
            color=DARK,
        )
    ax4.plot([0.01, 0.99], [0.89, 0.89], color=GREY, lw=0.8, transform=ax4.transAxes)

    row_h = 0.12
    for i, (tc, obj, desc, ok) in enumerate(test_results):
        y = header_y - row_h * (i + 1) - 0.01
        bg = "#f0fdf4" if i % 2 == 0 else WHITE
        ax4.add_patch(
            mpatches.FancyBboxPatch(
                (0.01, y - 0.03),
                0.97,
                row_h - 0.01,
                transform=ax4.transAxes,
                boxstyle="round,pad=0.01",
                facecolor=bg,
                edgecolor="none",
            )
        )
        ax4.text(
            col_x[0], y + 0.02, tc, transform=ax4.transAxes, fontsize=9, color=DARK
        )
        ax4.text(
            col_x[1], y + 0.02, obj, transform=ax4.transAxes, fontsize=8, color=BLUE
        )
        ax4.text(
            col_x[2], y + 0.02, desc, transform=ax4.transAxes, fontsize=8, color="#333"
        )
        status = "BF = Pyomo  OK" if ok else "AFWIJKING  X"
        color = "#059669" if ok else RED
        ax4.text(
            col_x[3],
            y + 0.02,
            status,
            transform=ax4.transAxes,
            fontsize=8,
            fontweight="bold",
            color=color,
        )

    ax4.text(
        0.5,
        0.02,
        "46/46 tests geslaagd  |  mypy schoon",
        ha="center",
        fontsize=9,
        color="#059669",
        fontweight="bold",
        transform=ax4.transAxes,
    )

    plt.savefig(OUT / "stap1.3_optimization.png", dpi=150, bbox_inches="tight")
    plt.close()
    print("  stap1.3_optimization.png")


# ---------------------------------------------------------------------------
# 5. Database mapping
# ---------------------------------------------------------------------------


def make_database_mapping() -> None:
    fig, ax = plt.subplots(figsize=(14, 8), facecolor=BG)
    ax.set_xlim(0, 14)
    ax.set_ylim(0, 8)
    ax.axis("off")
    ax.set_facecolor(BG)

    fig.suptitle(
        "OptimaliseRing DB  -->  FloodOpt datamodel",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )
    ax.text(
        7.0,
        7.55,
        "optimalise_ring_2011.sqlite  |  103 dijkringen  |  176 trajecten  |  3348 klimaatrecords",
        ha="center",
        fontsize=10,
        color=GREY,
    )

    # ---- LEFT: MDB tables header ----
    _box(ax, 0.2, 6.95, 5.2, 0.50, "#495057", "#495057", lw=0, radius=0.08)
    ax.text(
        2.8,
        7.22,
        "OptimaliseRing MDB  (origineel)",
        ha="center",
        fontsize=11,
        fontweight="bold",
        color=WHITE,
    )

    mdb_tables = [
        ("Dijkringen", "103", "Id, Naam, Terugkeertijd"),
        ("DijkringTrajecten", "176", "H0 [cm+NAP], Factor"),
        (
            "Klimaat_...DataTraject",
            "3348",
            r"$\alpha$ [1/cm],  $P_0$ [1/j],  $\eta$ [cm/j]",
        ),
        (
            "ParametersKostenfunctieData",
            "183",
            r"$\lambda$ [1/cm],  $C_\mathrm{exp}$,  $b$,  $\Omega$",
        ),
        ("SchadeFunctieData", "372", r"$\nu$,  $\zeta$,  $\psi$"),
        ("EconomischScenarioData", "868", r"$\gamma$  (economische groei)"),
        ("RamingVoorSlachtoffersData", "372", "Slachtoffers, Getroffenen"),
    ]

    ROW_H = 0.62
    for i, (name, rows, cols) in enumerate(mdb_tables):
        y_bot = 6.90 - (i + 1) * ROW_H
        fc = "#fff3cd" if name == "Klimaat_...DataTraject" else "#f1f3f5"
        _box(ax, 0.2, y_bot, 5.2, ROW_H - 0.06, fc, "#dee2e6", lw=0.8, radius=0.06)
        ax.text(
            0.45,
            y_bot + ROW_H * 0.62,
            name,
            fontsize=9,
            fontweight="bold",
            color="#212529",
            va="center",
        )
        ax.text(
            5.20,
            y_bot + ROW_H * 0.62,
            rows,
            fontsize=8,
            color=GREY,
            ha="right",
            va="center",
        )
        ax.text(
            0.45, y_bot + ROW_H * 0.22, cols, fontsize=8, color="#495057", va="center"
        )

    # unit conversion box
    _box(ax, 0.2, 0.20, 5.2, 1.00, "#e8f4fd", "#90c8f0", lw=1.2, radius=0.08)
    ax.text(
        2.8,
        1.08,
        "Eenheidsconversie  (SQLite views)",
        ha="center",
        fontsize=10,
        fontweight="bold",
        color=BLUE,
    )
    ax.text(
        0.50,
        0.82,
        r"$\alpha: \times 100$  (1/cm  ->  1/m)",
        fontsize=9,
        color="#0a58ca",
    )
    ax.text(
        0.50, 0.58, r"$\eta: \div 100$  (cm/j  ->  m/j)", fontsize=9, color="#0a58ca"
    )
    ax.text(
        0.50, 0.34, r"$H_0: \div 100$  (cm+NAP  ->  m+NAP)", fontsize=9, color="#0a58ca"
    )

    # ---- ARROW (centre) ----
    _arrow(ax, 5.6, 3.8, 7.8, 3.8, color=BLUE, lw=2.5)
    ax.text(6.7, 4.10, "SQLite\nviews", ha="center", fontsize=9, color=BLUE)

    # ---- RIGHT: FloodOpt models header ----
    _box(ax, 8.3, 6.95, 5.3, 0.50, BLUE, BLUE, lw=0, radius=0.08)
    ax.text(
        10.95,
        7.22,
        "FloodOpt datamodel  [m]",
        ha="center",
        fontsize=11,
        fontweight="bold",
        color=WHITE,
    )

    floodopt_panels = [
        (
            BLUE,
            5.8,
            "Trajectory",
            [
                r"norm = 1 / Terugkeertijd",
                r"$p_0 = P_0$  [1/jaar]",
                r"$\alpha = \alpha_\mathrm{MDB} \times 100$  [1/m]",
                "base_year, length, id",
            ],
            "v_trajecten_floodopt  (3168 rijen)",
        ),
        (
            PURPLE,
            3.6,
            "Scenario",
            [r"climate = Klimaat naam", r"$\eta = \eta_\mathrm{MDB} / 100$  [m/jaar]"],
            "18 klimaatscenarios",
        ),
        (
            GREEN,
            1.8,
            "Measure + RiskParams",
            [
                r"effect [m] = $\Delta h$",
                r"$\gamma$ via EconomischScenario",
                "base_damage via SchadeFunctie",
            ],
            "Beschikbaar voor stap 1.3 validatie",
        ),
    ]

    for color, y_top, title, fields, note in floodopt_panels:
        n_fields = len(fields)
        box_h = 0.42 * n_fields + 0.80
        y_bot = y_top - box_h
        _box(ax, 8.3, y_bot, 5.3, box_h, color + "22", color, lw=1.5, radius=0.08)
        ax.text(
            8.55,
            y_top - 0.30,
            title,
            fontsize=10,
            fontweight="bold",
            color=color,
            va="center",
        )
        for j, field in enumerate(fields):
            ax.text(
                8.55,
                y_top - 0.68 - j * 0.38,
                field,
                fontsize=8,
                color="#1e3a5f",
                va="center",
            )
        ax.text(
            8.55,
            y_bot + 0.14,
            note,
            fontsize=7,
            color=GREY,
            fontstyle="italic",
            va="center",
        )

    plt.tight_layout(rect=[0, 0, 1, 0.97])
    plt.savefig(OUT / "database_mapping.png", dpi=150, bbox_inches="tight")
    plt.close()
    print("  database_mapping.png")


# ---------------------------------------------------------------------------
# 6. Stap 1.4 - Smoke test
# ---------------------------------------------------------------------------


def make_smoke_test() -> None:
    fig = plt.figure(figsize=(14, 9), facecolor=BG)
    fig.suptitle(
        "Stap 1.4  --  Smoke Test:  Trajectory  ->  Optimizer  ->  Resultaat",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )
    gs = gridspec.GridSpec(
        2,
        2,
        height_ratios=[1, 1.2],
        hspace=0.55,
        wspace=0.40,
        top=0.93,
        bottom=0.06,
        left=0.05,
        right=0.97,
    )

    ax_flow = fig.add_subplot(gs[0, :])
    ax_time = fig.add_subplot(gs[1, 0])
    ax_table = fig.add_subplot(gs[1, 1])

    # ---- flow diagram ----
    ax_flow.set_xlim(0, 14)
    ax_flow.set_ylim(0, 3.2)
    ax_flow.axis("off")

    flow_items = [
        (
            1.2,
            "Trajectory\n& Scenario",
            r"$P_0 = 1/200$,  $\alpha = 4$,  $\eta = 0.003$",
            BLUE,
        ),
        (
            3.8,
            "5 Kandidaat-\nmaatregelen",
            "Dh: 0.15 - 0.50 m\nkosten: 0.5 - 2 M€",
            GREEN,
        ),
        (
            6.4,
            "Risk Layer\n(NCW berekening)",
            r"$\mathrm{NCW} = \sum P(s) \cdot V_0 \cdot e^{(\gamma-\delta)s}$",
            PURPLE,
        ),
        (9.0, "BruteForce\n& Pyomo", r"$2^5 = 32$ combinaties", RED),
        (11.8, "Resultaat\nMATCH", "3/3 objectives OK", "#059669"),
    ]

    BOX_W, BOX_H_F = 2.0, 2.2
    for x, title, sub, color in flow_items:
        _box(
            ax_flow,
            x - BOX_W / 2,
            0.5,
            BOX_W,
            BOX_H_F,
            color + "33",
            color,
            lw=2,
            radius=0.12,
        )
        ax_flow.text(
            x,
            1.95,
            title,
            ha="center",
            va="center",
            fontsize=9,
            fontweight="bold",
            color=color,
        )
        ax_flow.text(
            x, 0.95, sub, ha="center", va="center", fontsize=7.5, color="#333333"
        )

    arrow_xs = [(2.1, 2.8), (4.7, 5.4), (7.3, 8.0), (10.0, 10.8)]
    for x0, x1 in arrow_xs:
        _arrow(ax_flow, x0, 1.60, x1, 1.60, color=GREY, lw=2)

    # ---- timing bar chart ----
    objectives = ["MIN_COST", "MAX_RISK_R.", "MIN_NCW"]
    bf_times = [9.6, 5.2, 6.3]
    py_times = [184.6, 55.6, 7.6]

    x_pos = np.arange(len(objectives))
    W = 0.35
    ax_time.bar(x_pos - W / 2, bf_times, W, label="BruteForce", color=RED, alpha=0.85)
    ax_time.bar(x_pos + W / 2, py_times, W, label="Pyomo/HiGHS", color=BLUE, alpha=0.85)

    for i, (b, p) in enumerate(zip(bf_times, py_times)):
        ax_time.text(
            i - W / 2, b + 2.5, f"{b:.0f} ms", ha="center", fontsize=8, color=RED
        )
        ax_time.text(
            i + W / 2, p + 2.5, f"{p:.0f} ms", ha="center", fontsize=8, color=BLUE
        )

    ax_time.set_xticks(x_pos)
    ax_time.set_xticklabels(objectives, fontsize=9)
    ax_time.set_ylabel("Rekentijd  [ms]", fontsize=10)
    ax_time.set_title(
        "Rekentijden  (N = 5 maatregelen)", fontsize=11, color=DARK, pad=10
    )
    ax_time.legend(fontsize=9, loc="upper right")
    ax_time.set_ylim(0, 230)
    ax_time.text(
        0.98,
        0.97,
        "BruteForce sneller voor N <= 5\nPyomo schaalt beter voor grote N",
        ha="right",
        va="top",
        fontsize=8,
        color=GREY,
        transform=ax_time.transAxes,
        bbox=dict(boxstyle="round,pad=0.3", facecolor=WHITE, edgecolor=GREY),
    )

    # ---- results table ----
    ax_table.axis("off")
    ax_table.set_title("Verificatieresultaten", fontsize=11, color=DARK, pad=10)
    headers = ["Objective", "Optimum", "Waarde", "Match"]
    rows_data = [
        ["MIN_COST", "{M02, M04}", "EUR 1 089 224", "BF = Py  OK"],
        ["MAX_RISK_R.", "{M02, M03, M04}", "Dh = 0.80 m", "BF = Py  OK"],
        ["MIN_NCW", "{alle 5}", "NCW = 9.0 M", "BF = Py  OK"],
    ]
    tbl = ax_table.table(
        cellText=rows_data,
        colLabels=headers,
        loc="center",
        cellLoc="center",
        bbox=[0.0, 0.30, 1.0, 0.60],
    )
    tbl.auto_set_font_size(False)
    tbl.set_fontsize(9)
    tbl.scale(1, 2.0)
    for (r, c), cell in tbl.get_celld().items():
        if r == 0:
            cell.set_facecolor(DARK)
            cell.set_text_props(color=WHITE, fontweight="bold")
        elif c == 3:
            cell.set_text_props(color="#059669", fontweight="bold")
        elif r % 2 == 0:
            cell.set_facecolor("#f0fdf4")

    ax_table.text(
        0.5,
        0.10,
        "58/58 tests geslaagd  |  exitcode 0  |  mypy schoon",
        ha="center",
        fontsize=9,
        color="#059669",
        fontweight="bold",
        transform=ax_table.transAxes,
    )

    plt.savefig(OUT / "stap1.4_smoke_test.png", dpi=150, bbox_inches="tight")
    plt.close()
    print("  stap1.4_smoke_test.png")


# ---------------------------------------------------------------------------
# 7. Stap 2.1 - FastAPI service
# ---------------------------------------------------------------------------


def make_api() -> None:
    fig = plt.figure(figsize=(15, 8), facecolor=BG)
    fig.suptitle(
        "Stap 2.1  --  FastAPI service:  dunne HTTP-schil om floodopt-core",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )
    gs = gridspec.GridSpec(
        1,
        2,
        wspace=0.40,
        top=0.92,
        bottom=0.05,
        left=0.04,
        right=0.98,
    )
    ax_flow = fig.add_subplot(gs[0, 0])
    ax_table = fig.add_subplot(gs[0, 1])

    # ---- left: request flow ----
    ax_flow.set_xlim(0, 10)
    ax_flow.set_ylim(0, 10.5)
    ax_flow.axis("off")
    ax_flow.set_title("Request-flow  (stap 2.1 MVP)", fontsize=11, color=DARK, pad=10)

    # Client
    _box(ax_flow, 0.3, 8.5, 2.4, 1.2, "#e8f0fe", BLUE, lw=2, radius=0.10)
    ax_flow.text(
        1.5,
        9.12,
        "Client /\nSwagger UI",
        ha="center",
        va="center",
        fontsize=9,
        fontweight="bold",
        color=BLUE,
    )

    # FastAPI
    _box(ax_flow, 3.5, 7.5, 3.0, 2.5, "#fff3cd", ORANGE, lw=2, radius=0.10)
    ax_flow.text(
        5.0,
        9.65,
        "FastAPI",
        ha="center",
        fontsize=10,
        fontweight="bold",
        color="#856404",
    )
    endpoints_left = [
        "POST /scenarios",
        "POST /trajectories",
        "POST /optimize",
        "GET  /results/{id}",
    ]
    for i, ep in enumerate(endpoints_left):
        ax_flow.text(
            5.0,
            9.15 - i * 0.40,
            ep,
            ha="center",
            fontsize=8,
            color="#495057",
            fontfamily="monospace",
        )

    # floodopt-core
    _box(ax_flow, 3.5, 3.5, 3.0, 3.2, "#d1fae5", GREEN, lw=2, radius=0.10)
    ax_flow.text(
        5.0,
        6.30,
        "floodopt-core",
        ha="center",
        fontsize=10,
        fontweight="bold",
        color="#064e3b",
    )
    core_items = [
        r"Physics: $P(t)$",
        "Risk: NCW",
        "Optimizer: MILP",
        "In-memory store",
    ]
    for i, item in enumerate(core_items):
        ax_flow.text(
            5.0, 5.80 - i * 0.55, item, ha="center", fontsize=9, color="#065f46"
        )

    # Store
    _box(ax_flow, 7.2, 5.2, 2.4, 2.4, "#f1f3f5", GREY, lw=1.5, radius=0.10)
    ax_flow.text(
        8.4,
        7.25,
        "Store\n(in-memory)",
        ha="center",
        fontsize=9,
        color=GREY,
        fontweight="bold",
    )
    store_items = ["scenarios{}", "trajectories{}", "results{}"]
    for i, s in enumerate(store_items):
        ax_flow.text(
            8.4,
            6.55 - i * 0.40,
            s,
            ha="center",
            fontsize=8,
            color=GREY,
            fontfamily="monospace",
        )
    ax_flow.text(
        8.4,
        5.40,
        "-> PostgreSQL\n   (stap 2.2)",
        ha="center",
        fontsize=8,
        color="#aaa",
        fontstyle="italic",
    )

    # arrows
    _arrow(ax_flow, 2.7, 9.10, 3.5, 9.10, color=BLUE, lw=2)
    ax_flow.text(3.1, 9.25, "HTTP", fontsize=8, color=BLUE)

    _arrow(ax_flow, 5.0, 7.50, 5.0, 6.70, color="#856404", lw=2)
    ax_flow.text(5.15, 7.10, "roept aan", fontsize=8, color="#856404")

    _arrow(ax_flow, 6.5, 6.00, 7.2, 6.00, color=GREY, lw=1.5)
    ax_flow.text(6.55, 6.15, "lees/schrijf", fontsize=8, color=GREY)

    ax_flow.text(
        5.0,
        2.60,
        "Geen business logic in API-laag  OK",
        ha="center",
        fontsize=10,
        color="#059669",
        fontweight="bold",
        bbox=dict(boxstyle="round,pad=0.4", facecolor="#f0fdf4", edgecolor="#059669"),
    )

    # ---- right: endpoint table + verifications ----
    ax_table.axis("off")
    ax_table.set_title(
        "Endpoints en verificatie  (20/20 tests OK)", fontsize=11, color=DARK, pad=10
    )

    ep_headers = ["Method", "Pad", "Status", "Verificatie"]
    ep_rows = [
        ["POST", "/scenarios", "201", "roundtrip OK"],
        ["GET", "/scenarios/{id}", "200/404", "404 test OK"],
        ["POST", "/trajectories", "201", "roundtrip OK"],
        ["GET", "/trajectories/{id}", "200/404", "404 test OK"],
        ["POST", "/optimize", "201", "= stap 1.4 OK"],
        ["GET", "/results/{job_id}", "200/404", "= POST resp OK"],
        ["GET", "/docs", "200", "Swagger UI OK"],
    ]
    tbl = ax_table.table(
        cellText=ep_rows,
        colLabels=ep_headers,
        loc="upper center",
        cellLoc="center",
        bbox=[0.0, 0.52, 1.0, 0.46],
    )
    tbl.auto_set_font_size(False)
    tbl.set_fontsize(9)
    tbl.scale(1, 1.8)
    for (r, c), cell in tbl.get_celld().items():
        if r == 0:
            cell.set_facecolor(DARK)
            cell.set_text_props(color=WHITE, fontweight="bold")
        elif c == 0:
            clr = BLUE if ep_rows[r - 1][0] == "POST" else GREEN
            cell.set_text_props(color=clr, fontweight="bold")
        elif c == 3:
            cell.set_text_props(color="#059669")
        elif r % 2 == 0:
            cell.set_facecolor("#f8f9fa")

    # key verifications
    verifs = [
        ("POST /optimize  MIN_COST", "{M02, M04}  investering EUR 1 089 224"),
        ("POST /optimize  MAX_RR", "{M02, M03, M04}  Dh = 0.80 m"),
        ("POST /optimize  MIN_NCW", "{alle 5}  NCW = EUR 9.0 M"),
        ("BruteForce = Pyomo", "via API beide solvers geverifieerd"),
        ("Geen physics in API", "math.exp afwezig in main.py"),
    ]
    ax_table.text(
        0.5,
        0.49,
        "Kritieke verificaties vs. stap 1.4:",
        ha="center",
        fontsize=10,
        fontweight="bold",
        color=DARK,
        transform=ax_table.transAxes,
    )
    for i, (label, detail) in enumerate(verifs):
        y = 0.42 - i * 0.08
        ax_table.text(
            0.03,
            y,
            f"OK  {label}",
            fontsize=9,
            color="#059669",
            fontweight="bold",
            transform=ax_table.transAxes,
        )
        ax_table.text(
            0.03,
            y - 0.035,
            f"    {detail}",
            fontsize=8,
            color="#495057",
            transform=ax_table.transAxes,
        )

    ax_table.text(
        0.5,
        0.02,
        "78/78 tests geslaagd  |  Swagger /docs OK  |  mypy schoon",
        ha="center",
        fontsize=10,
        color="#059669",
        fontweight="bold",
        transform=ax_table.transAxes,
    )

    plt.savefig(OUT / "stap2.1_api.png", dpi=150, bbox_inches="tight")
    plt.close()
    print("  stap2.1_api.png")


# ---------------------------------------------------------------------------
# 8. Stap 2.2 - Database architectuur
# ---------------------------------------------------------------------------


def make_database() -> None:
    fig = plt.figure(figsize=(15, 8), facecolor=BG)
    fig.suptitle(
        "Stap 2.2  --  Database:  SQLite (dev)  ->  PostgreSQL optioneel (prod)",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )
    gs = gridspec.GridSpec(
        1,
        2,
        wspace=0.40,
        top=0.92,
        bottom=0.05,
        left=0.04,
        right=0.98,
    )
    ax_pat = fig.add_subplot(gs[0, 0])
    ax_schema = fig.add_subplot(gs[0, 1])

    # ---- left: repository pattern ----
    ax_pat.set_xlim(0, 10)
    ax_pat.set_ylim(0, 10)
    ax_pat.axis("off")
    ax_pat.set_title(
        "Repository-pattern  (afhankelijkheidsinjectie)",
        fontsize=11,
        color=DARK,
        pad=10,
    )

    # FastAPI box (top, full width)
    _box(ax_pat, 1.0, 8.30, 8.0, 1.20, ORANGE + "33", ORANGE, lw=2, radius=0.12)
    ax_pat.text(
        5.0,
        9.15,
        "FastAPI  (main.py)",
        ha="center",
        va="center",
        fontsize=10,
        fontweight="bold",
        color="#856404",
    )
    ax_pat.text(
        5.0,
        8.65,
        "DATABASE_URL niet ingesteld  ->  SQLite\n"
        "DATABASE_URL ingesteld  ->  PostgreSQL",
        ha="center",
        va="center",
        fontsize=8.5,
        color="#555",
    )

    # MemoryRepositories
    _box(ax_pat, 0.5, 5.70, 4.0, 2.0, "#f1f3f5", "#ced4da", lw=1.5, radius=0.10)
    ax_pat.text(
        2.5,
        7.35,
        "MemoryRepositories",
        ha="center",
        va="center",
        fontsize=9,
        fontweight="bold",
        color=DARK,
    )
    ax_pat.text(
        2.5, 6.90, "In-memory dict", ha="center", va="center", fontsize=8, color="#555"
    )
    ax_pat.text(
        2.5,
        6.55,
        "Gebruikt in tests",
        ha="center",
        va="center",
        fontsize=8,
        color="#555",
    )
    ax_pat.text(
        2.5,
        6.20,
        "(FastAPI dep. override)",
        ha="center",
        va="center",
        fontsize=8,
        color=GREY,
    )

    # PostgresRepositories
    _box(ax_pat, 5.5, 5.70, 4.0, 2.0, GREEN + "22", GREEN, lw=1.5, radius=0.10)
    ax_pat.text(
        7.5,
        7.35,
        "PostgresRepositories",
        ha="center",
        va="center",
        fontsize=9,
        fontweight="bold",
        color="#064e3b",
    )
    ax_pat.text(
        7.5,
        6.90,
        "SQLAlchemy Session",
        ha="center",
        va="center",
        fontsize=8,
        color="#555",
    )
    ax_pat.text(
        7.5,
        6.55,
        "Werkt met SQLite",
        ha="center",
        va="center",
        fontsize=8,
        color="#555",
    )
    ax_pat.text(
        7.5, 6.20, "en PostgreSQL", ha="center", va="center", fontsize=8, color="#555"
    )

    # SQLite box
    _box(ax_pat, 0.5, 3.20, 4.0, 1.80, "#f1f3f5", "#ced4da", lw=1.5, radius=0.10)
    ax_pat.text(
        2.5,
        4.70,
        "SQLite  (floodopt.db)",
        ha="center",
        va="center",
        fontsize=9,
        fontweight="bold",
        color=DARK,
    )
    ax_pat.text(
        2.5,
        4.25,
        "Ingebouwd in Python",
        ha="center",
        va="center",
        fontsize=8,
        color="#555",
    )
    ax_pat.text(
        2.5,
        3.85,
        "Geen installatie vereist",
        ha="center",
        va="center",
        fontsize=8,
        color="#555",
    )

    # PostgreSQL box
    _box(ax_pat, 5.5, 3.20, 4.0, 1.80, "#e8f4fd", "#0077b6", lw=1.5, radius=0.10)
    ax_pat.text(
        7.5,
        4.70,
        "PostgreSQL  (optioneel)",
        ha="center",
        va="center",
        fontsize=9,
        fontweight="bold",
        color="#023e8a",
    )
    ax_pat.text(
        7.5,
        4.25,
        "docker compose up -d postgres",
        ha="center",
        va="center",
        fontsize=8,
        color="#0077b6",
        fontfamily="monospace",
    )
    ax_pat.text(
        7.5,
        3.85,
        "DATABASE_URL=postgresql://...",
        ha="center",
        va="center",
        fontsize=8,
        color="#0077b6",
        fontfamily="monospace",
    )

    # arrows
    _arrow(ax_pat, 3.0, 8.30, 2.5, 7.70, color=GREY, lw=1.5)
    ax_pat.text(1.5, 8.10, "tests", fontsize=8, color=GREY, ha="center")

    _arrow(ax_pat, 7.0, 8.30, 7.5, 7.70, color="#059669", lw=1.5)
    ax_pat.text(8.3, 8.10, "productie", fontsize=8, color="#059669", ha="center")

    _arrow(ax_pat, 2.5, 5.70, 2.5, 5.00, color=GREY, lw=1.5)
    _arrow(ax_pat, 7.5, 5.70, 7.5, 5.00, color="#059669", lw=1.5)

    ax_pat.text(
        5.0,
        2.50,
        "84/84 tests geslaagd  |  geen Docker vereist",
        ha="center",
        fontsize=10,
        color="#059669",
        fontweight="bold",
    )

    # ---- right: schema ----
    ax_schema.axis("off")
    ax_schema.set_title("Database-schema", fontsize=11, color=DARK, pad=10)

    tables = [
        (
            "scenarios",
            [
                "id             TEXT   PK",
                "climate        TEXT",
                "q_design       REAL",
                "h_design       REAL",
                "eta            REAL",
            ],
            GREEN,
        ),
        (
            "trajectories",
            [
                "id             TEXT   PK",
                "norm           REAL",
                "length         REAL",
                "p0             REAL",
                "alpha          REAL",
                "base_year      INT",
            ],
            BLUE,
        ),
        (
            "optimization_results",
            [
                "job_id         TEXT   PK",
                "trajectory_id  TEXT",
                "scenario_id    TEXT",
                "objective      TEXT",
                "solver         TEXT",
                "selected_ids   JSON",
                "total_ncw      REAL",
                "investment_npv REAL",
            ],
            PURPLE,
        ),
    ]

    y_cursor = 0.97
    for name, cols, color in tables:
        n = len(cols)
        box_h = 0.055 * (n + 1.8)
        y_bot = y_cursor - box_h
        ax_schema.add_patch(
            mpatches.FancyBboxPatch(
                (0.02, y_bot),
                0.96,
                box_h,
                transform=ax_schema.transAxes,
                boxstyle="round,pad=0.01",
                facecolor=color + "22",
                edgecolor=color,
                linewidth=1.5,
            )
        )
        ax_schema.text(
            0.50,
            y_cursor - 0.023,
            name,
            ha="center",
            fontsize=10,
            fontweight="bold",
            color=color,
            transform=ax_schema.transAxes,
        )
        for i, col in enumerate(cols):
            ax_schema.text(
                0.07,
                y_cursor - 0.053 * (i + 1.2),
                col,
                fontsize=8.5,
                color="#333",
                transform=ax_schema.transAxes,
                fontfamily="monospace",
            )
        y_cursor = y_bot - 0.035

    ax_schema.text(
        0.50,
        0.025,
        "SQLite: check_same_thread=False  |  PostgreSQL: pool_pre_ping=True",
        ha="center",
        fontsize=8,
        color=GREY,
        transform=ax_schema.transAxes,
    )

    plt.savefig(OUT / "stap2.2_database.png", dpi=150, bbox_inches="tight")
    plt.close()
    print("  stap2.2_database.png")


# ---------------------------------------------------------------------------
# 9. Geo-stack
# ---------------------------------------------------------------------------


def make_geo_stack() -> None:
    fig = plt.figure(figsize=(14, 9), facecolor=BG)
    fig.suptitle(
        "Geo-stack:  GeoPandas (server) + Leaflet (frontend)  --  geen PostGIS vereist",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )
    gs = gridspec.GridSpec(
        2,
        1,
        height_ratios=[1.4, 1],
        hspace=0.50,
        top=0.93,
        bottom=0.04,
        left=0.04,
        right=0.97,
    )
    ax_flow = fig.add_subplot(gs[0])
    ax_table = fig.add_subplot(gs[1])

    # ---- top: flow ----
    ax_flow.set_xlim(0, 14)
    ax_flow.set_ylim(0, 4.5)
    ax_flow.axis("off")

    flow_boxes = [
        (
            1.4,
            2.2,
            2.4,
            3.6,
            "#f1f3f5",
            GREY,
            "#023e8a",
            "Brondata",
            [
                "Shapefiles (dijkringen)",
                "optimalise_ring_2011.sqlite",
                "WKT in SQLite (trajectories)",
            ],
        ),
        (
            4.6,
            2.2,
            2.6,
            3.6,
            "#e8f4fd",
            "#0077b6",
            "#023e8a",
            "GeoPandas",
            [
                "gpd.read_file(shapefile)",
                "gdf.to_json()  ->  GeoJSON",
                "Ruimtelijke joins",
                "Lengteberekeningen",
                "Clippen op dijkring",
            ],
        ),
        (
            7.8,
            2.2,
            2.4,
            3.6,
            "#fff3cd",
            ORANGE,
            "#856404",
            "FastAPI",
            [
                "GET /trajectories/{id}/geojson",
                "Content-Type:",
                "application/geo+json",
                "Stap 4.2 (gepland)",
            ],
        ),
        (
            11.0,
            2.2,
            2.4,
            3.6,
            "#dbeafe",
            "#0077b6",
            "#023e8a",
            "Leaflet",
            [
                "Dijkvakken op kaart",
                "Kleurgradiënt NCW",
                "P(t) per segment",
                "Klikbare maatregelen",
            ],
        ),
    ]

    for cx, cy, bw, bh, fc, ec, tc, title, items in flow_boxes:
        _box(ax_flow, cx - bw / 2, cy - bh / 2, bw, bh, fc, ec, lw=2, radius=0.12)
        ax_flow.text(
            cx,
            cy + bh / 2 - 0.30,
            title,
            ha="center",
            va="center",
            fontsize=11,
            fontweight="bold",
            color=tc,
        )
        for i, s in enumerate(items):
            ax_flow.text(
                cx,
                cy + bh / 2 - 0.70 - i * 0.42,
                s,
                ha="center",
                va="center",
                fontsize=8,
                color="#333",
            )

    arrow_midpoints = [
        (2.6, 3.3, "#0077b6"),
        (5.9, 3.3, ORANGE),
        (9.1, 3.3, "#0077b6"),
    ]
    gap = 0.35
    for cx, bw, color in arrow_midpoints:
        x0 = cx + bw / 2 + gap * 0.2
        x1 = cx + bw / 2 + gap * 0.8
        _arrow(ax_flow, x0, 2.2, x1, 2.2, color=color, lw=2.0)

    ax_flow.text(3.5, 2.35, "read", ha="center", fontsize=9, color=GREY)
    ax_flow.text(6.75, 2.35, "GeoJSON", ha="center", fontsize=9, color="#0077b6")
    ax_flow.text(9.95, 2.35, "HTTP", ha="center", fontsize=9, color=ORANGE)

    # ---- bottom: comparison table ----
    ax_table.axis("off")
    ax_table.set_title(
        "Waarom GeoPandas + Leaflet in plaats van PostGIS",
        fontsize=11,
        fontweight="bold",
        color=DARK,
        pad=10,
    )

    tbl_headers = ["Aspect", "GeoPandas + Leaflet", "PostGIS"]
    tbl_rows = [
        ["Installatie", "Nul  (pip install geopandas)", "Docker / server vereist"],
        ["Dijkvak-geometrie", "WKT in SQLite + GeoPandas", "native geometry kolom"],
        ["GeoJSON naar Leaflet", "gdf.to_json()", "ST_AsGeoJSON()"],
        ["Ruimtelijke query", "Python-code  (voldoende MVP)", "SQL spatial functions"],
        ["Prod upgrade pad", "DATABASE_URL -> PostgreSQL", "altijd beschikbaar"],
    ]
    tbl = ax_table.table(
        cellText=tbl_rows,
        colLabels=tbl_headers,
        loc="center",
        cellLoc="center",
        bbox=[0.0, 0.02, 1.0, 0.90],
    )
    tbl.auto_set_font_size(False)
    tbl.set_fontsize(9.5)
    tbl.scale(1, 1.9)
    for (r, c), cell in tbl.get_celld().items():
        if r == 0:
            cell.set_facecolor(DARK)
            cell.set_text_props(color=WHITE, fontweight="bold")
        elif c == 0:
            cell.set_text_props(fontweight="bold", color=DARK)
        elif c == 1:
            cell.set_text_props(color="#059669")
        elif r % 2 == 0:
            cell.set_facecolor("#f0fdf4")

    plt.savefig(OUT / "geo_stack.png", dpi=150, bbox_inches="tight")
    plt.close()
    print("  geo_stack.png")


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
    make_geo_stack()
    print(f"\nKlaar -- alle PNG's opgeslagen in {OUT}/")
