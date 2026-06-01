"""
Genereert alle documentatiediagrammen als PNG met matplotlib mathtext.

Uitvoer in docs/:
  architecture.png           Drielagenarchitectuur
  stap1.1_physics_formula.png  P(t)-grafiek + formule
  stap1.2_risk_ncw.png       NCW-grafiek + formule
  stap1.3_optimization.png   Optimalisatievergelijking BruteForce vs Pyomo
  database_mapping.png       MDB → SQLite → FloodOpt mapping

Gebruik:
    python scripts/generate_docs.py
"""

import math
from pathlib import Path

import matplotlib

matplotlib.use("Agg")  # headless — geen Tk/display vereist
import matplotlib.pyplot as plt
import matplotlib.patches as mpatches
import matplotlib.gridspec as gridspec
import numpy as np

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
# 1. Architectuur
# ---------------------------------------------------------------------------


def make_architecture() -> None:
    fig, ax = plt.subplots(figsize=(9, 6), facecolor=BG)
    ax.set_xlim(0, 10)
    ax.set_ylim(0, 10)
    ax.axis("off")
    ax.set_facecolor(BG)

    fig.suptitle(
        "FloodOpt — Architectuur", fontsize=16, fontweight="bold", color=DARK, y=0.97
    )
    ax.text(
        5,
        9.3,
        "Drie strikte lagen — de optimizer bevat nooit fysica",
        ha="center",
        fontsize=10,
        color=GREY,
    )

    layers = [
        (
            7.5,
            BLUE,
            "#3a0ca3",
            "Optimization Layer",
            "Zoekt optimale maatregelencombinatie via Pyomo",
            r"$\min \sum c_i x_i \;|\; \text{BruteForce} \cdot\text{(ref)} + \text{Pyomo/HiGHS}$",
            "Stap 1.3 ✓",
        ),
        (
            4.8,
            PURPLE,
            "#560bad",
            "Risk Layer",
            r"Verwachte schade, NCW",
            r"$\mathrm{NCW} = \sum_{s=0}^{T-1} P(s)\cdot V_0\cdot e^{(\gamma-\delta)s}$",
            "Stap 1.2 ✓",
        ),
        (
            2.1,
            GREEN,
            "#05b484",
            "Physics Layer",
            r"Faalkans, kruinhoogte",
            r"$P(t) = P_0\cdot e^{\alpha\eta t}\cdot e^{-\alpha\Delta h}$",
            "Stap 1.1 ✓",
        ),
    ]

    for y_center, facecolor, edgecolor, title, subtitle, formula, badge in layers:
        rect = mpatches.FancyBboxPatch(
            (0.5, y_center - 1.1),
            9,
            2.2,
            boxstyle="round,pad=0.1",
            facecolor=facecolor,
            edgecolor=edgecolor,
            linewidth=2,
        )
        ax.add_patch(rect)
        ax.text(
            5,
            y_center + 0.7,
            title,
            ha="center",
            va="center",
            fontsize=13,
            fontweight="bold",
            color=WHITE,
        )
        ax.text(
            5,
            y_center + 0.05,
            subtitle,
            ha="center",
            va="center",
            fontsize=10,
            color=WHITE,
            alpha=0.9,
        )
        ax.text(
            5,
            y_center - 0.55,
            formula,
            ha="center",
            va="center",
            fontsize=10,
            color=WHITE,
            alpha=0.85,
        )
        ax.text(
            9.3,
            y_center + 0.7,
            badge,
            ha="right",
            va="center",
            fontsize=9,
            color=WHITE,
            fontstyle="italic",
        )

    # Pijlen
    for y_arrow in [6.6, 3.9]:
        ax.annotate(
            "",
            xy=(5, y_arrow - 0.35),
            xytext=(5, y_arrow + 0.25),
            arrowprops=dict(arrowstyle="-|>", color=GREY, lw=2),
        )
        ax.text(5.3, y_arrow - 0.05, "roept aan", fontsize=9, color=GREY)

    plt.tight_layout()
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

    fig = plt.figure(figsize=(11, 7), facecolor=BG)
    gs = gridspec.GridSpec(2, 1, height_ratios=[3, 1], hspace=0.35)
    ax = fig.add_subplot(gs[0])
    ax_form = fig.add_subplot(gs[1])

    ax.plot(
        t, P_no * 1e3, color=RED, lw=2.5, label=r"$\Delta h = 0$ (geen maatregelen)"
    )
    ax.plot(
        t,
        P_05 * 1e3,
        color=ORANGE,
        lw=2.5,
        ls="--",
        label=r"$\Delta h = 0.5\,\mathrm{m}$",
    )
    ax.plot(
        t,
        P_10 * 1e3,
        color=GREEN,
        lw=2.5,
        ls=":",
        label=r"$\Delta h = 1.0\,\mathrm{m}$",
    )

    ax.axhline(1.0, color=RED, lw=1, ls="-", alpha=0.4)
    ax.scatter([0], [P0 * 1e3], color=RED, s=60, zorder=5)
    ax.scatter(
        [50],
        [P0 * math.exp(ALPHA * ETA * 50) * 1e3],
        color=RED,
        marker="^",
        s=70,
        zorder=5,
        label=r"Case 2: $t{=}50,\ P{=}2.12\times10^{-3}$",
    )
    ax.scatter(
        [0],
        [P0 * math.exp(-ALPHA * 1.0) * 1e3],
        color=GREEN,
        marker="s",
        s=60,
        zorder=5,
        label=r"Case 3: $\Delta h{=}1\,\mathrm{m},\ P{=}6.7\times10^{-6}$",
    )

    ax.set_xlabel(r"$t$ [jaar na basisjaar 2017]", fontsize=12)
    ax.set_ylabel(r"$P(t)\ [\times 10^{-3}\ \mathrm{jaar}^{-1}]$", fontsize=12)
    ax.set_title(
        "Stap 1.1 — SimpleDikeOverflow", fontsize=14, fontweight="bold", color=DARK
    )
    ax.legend(fontsize=10, framealpha=0.9)
    ax.set_xlim(0, 100)
    ax.set_ylim(bottom=0)

    # Annotaties testcases
    ax.annotate(
        r"$P_0 = 10^{-3}$",
        xy=(0, P0 * 1e3),
        xytext=(8, 1.15),
        fontsize=10,
        color=RED,
        arrowprops=dict(arrowstyle="->", color=RED, lw=1.2),
    )

    # Formule-panel
    ax_form.axis("off")
    ax_form.set_facecolor(WHITE)
    formula_text = (
        r"$P(t) = P_0 \cdot e^{\alpha \eta t} \cdot e^{-\alpha \Delta h}$"
        "\n\n"
        r"$P_0 = 10^{-3}\ \mathrm{jaar}^{-1}$   "
        r"$\alpha = 5.0\ \mathrm{m}^{-1}$   "
        r"$\eta = 0.003\ \mathrm{m/jaar}\ (\mathrm{W+})$   "
        r"$\Delta h = \sum_i h_i\ [\mathrm{m}]$"
    )
    ax_form.text(
        0.5,
        0.5,
        formula_text,
        ha="center",
        va="center",
        fontsize=12,
        transform=ax_form.transAxes,
        bbox=dict(
            boxstyle="round,pad=0.4", facecolor="#e8f0fe", edgecolor=BLUE, linewidth=1.5
        ),
    )
    ax_form.text(
        0.98,
        0.05,
        "Identiek aan OptimaliseRing 2.3.2 (HKV, 2013)  |  7/7 tests ✓  rel_tol = 1e-9",
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

    def S(delta_h: float) -> np.ndarray:
        pf = P0 * np.exp(-ALPHA * delta_h)
        return pf * V0 * np.exp((GAMMA - DELTA) * s)

    def ncw(delta_h: float) -> float:
        return float(np.sum(S(delta_h)))

    S0 = S(0.0)
    S05 = S(0.5)
    S10 = S(1.0)

    fig = plt.figure(figsize=(11, 8), facecolor=BG)
    gs = gridspec.GridSpec(2, 2, height_ratios=[2.5, 1], hspace=0.4, wspace=0.35)
    ax = fig.add_subplot(gs[0, :])
    ax_table = fig.add_subplot(gs[1, 0])
    ax_form = fig.add_subplot(gs[1, 1])

    ax.fill_between(s, S0 / 1e6, alpha=0.12, color=RED, label="_nolegend_")
    ax.plot(
        s,
        S0 / 1e6,
        color=RED,
        lw=2.5,
        label=r"$\Delta h=0$  (NCW $\approx 31.9\,\mathrm{M€}$)",
    )
    ax.plot(
        s,
        S05 / 1e6,
        color=ORANGE,
        lw=2.5,
        ls="--",
        label=r"$\Delta h=0.5\,\mathrm{m}$  (NCW $\approx 2.6\,\mathrm{M€}$)",
    )
    ax.plot(
        s,
        S10 / 1e6,
        color=GREEN,
        lw=2.5,
        ls=":",
        label=r"$\Delta h=1.0\,\mathrm{m}$  (NCW $\approx 0.22\,\mathrm{M€}$)",
    )

    ax.annotate(
        f"NCW ≈ {ncw(0)/1e6:.1f} M€",
        xy=(25, S0[25] / 2e6),
        fontsize=11,
        color=RED,
        fontstyle="italic",
    )

    ax.set_xlabel(r"$s$ [jaar na basisjaar]", fontsize=12)
    ax.set_ylabel(r"$S(s) = P(s)\cdot V(s)\ [\mathrm{M€/jaar}]$", fontsize=12)
    ax.set_title(
        "Stap 1.2 — Risk Layer: Verwachte schade & NCW",
        fontsize=14,
        fontweight="bold",
        color=DARK,
    )
    ax.legend(fontsize=10, framealpha=0.9)

    # Tabel
    ax_table.axis("off")
    headers = [r"$\Delta h$", r"$S(s{=}0)$", "NCW", "Reductie"]
    rows = [
        ["0 m", "1 000 000 €", "31 923 000 €", "—"],
        ["0.5 m", "82 085 €", "2 620 000 €", "−91.8 %"],
        ["1.0 m", "6 738 €", "215 000 €", "−99.3 %"],
    ]
    tbl = ax_table.table(
        cellText=rows, colLabels=headers, loc="center", cellLoc="center"
    )
    tbl.auto_set_font_size(False)
    tbl.set_fontsize(9)
    tbl.scale(1, 1.6)
    for (r, c), cell in tbl.get_celld().items():
        if r == 0:
            cell.set_facecolor(BLUE)
            cell.set_text_props(color="white", fontweight="bold")
        elif c == 3 and r > 0:
            cell.set_text_props(color="#059669", fontweight="bold")
    ax_table.set_title("Verificatietabel (T=50 jaar)", fontsize=10, pad=4)

    # Formule
    ax_form.axis("off")
    ax_form.text(
        0.5,
        0.65,
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
        0.2,
        r"$V(s)=V_0 e^{\gamma s}$   $V_0=10^9\,€$"
        "\n"
        r"$\gamma=0.02$   $\delta=0.04$   $T=50\,\mathrm{jaar}$",
        ha="center",
        va="center",
        fontsize=10,
        transform=ax_form.transAxes,
    )
    ax_form.text(
        0.5,
        -0.05,
        "10/10 tests ✓  rel_tol = 1e-9",
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
    fig, axes = plt.subplots(2, 2, figsize=(13, 9), facecolor=BG)
    fig.suptitle(
        "Stap 1.3 — Optimization Layer: BruteForce ≡ Pyomo/HiGHS",
        fontsize=15,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )

    # --- Panel 1: Probleemformulering ---
    ax = axes[0, 0]
    ax.axis("off")
    ax.set_title(
        "Optimalisatieformuleringen", fontsize=12, fontweight="bold", color=DARK
    )
    formulas = [
        (
            r"MIN_COST",
            BLUE,
            r"$\min\;\sum_i c_i x_i$"
            + "\n"
            + r"$\text{s.t.}\;\sum_i h_i x_i \geq h_{\min}$"
            + "\n"
            + r"$h_{\min} = \ln(P_0/\text{norm})\,/\,\alpha$",
        ),
        (
            r"MAX_RISK_REDUCTION",
            GREEN,
            r"$\max\;\sum_i h_i x_i$"
            + "\n"
            + r"$\text{s.t.}\;\sum_i c_i x_i \leq B$"
            + "\n"
            + r"$(0/1\text{-knapsack})$",
        ),
        (
            r"MIN_NCW  (lineair)",
            ORANGE,
            r"$\min\;\sum_i (c_i - C\alpha h_i)\,x_i$"
            + "\n"
            + r"$C\cdot\alpha\cdot h_i \approx \Delta\mathrm{NCW}_i$"
            + "\n"
            + r"geldig voor $\alpha h_i < 0.5$",
        ),
    ]
    y_pos = [0.82, 0.50, 0.18]
    for (label, color, formula), yp in zip(formulas, y_pos):
        ax.text(
            0.02,
            yp + 0.07,
            label,
            transform=ax.transAxes,
            fontsize=10,
            fontweight="bold",
            color=color,
        )
        ax.text(
            0.02,
            yp - 0.10,
            formula,
            transform=ax.transAxes,
            fontsize=10,
            color="#212529",
            bbox=dict(
                boxstyle="round,pad=0.3",
                facecolor=color + "22",
                edgecolor=color,
                linewidth=1.2,
            ),
        )

    # --- Panel 2: NCW als functie van Δh ---
    ax2 = axes[0, 1]
    P0, ALPHA, V0, GAMMA, DELTA, T_HOR = 1e-3, 5.0, 1e9, 0.02, 0.04, 50
    C = P0 * V0 * sum(math.exp((GAMMA - DELTA) * s) for s in range(T_HOR))
    dh = np.linspace(0, 2, 300)
    ncw_exact = C * np.exp(-ALPHA * dh)
    ncw_linear = C * (1 - ALPHA * dh)

    ax2.plot(
        dh,
        ncw_exact / 1e6,
        color=BLUE,
        lw=2.5,
        label="Exact: $C\\cdot e^{-\\alpha\\Delta h}$",
    )
    ax2.plot(
        dh,
        ncw_linear / 1e6,
        color=ORANGE,
        lw=2.0,
        ls="--",
        label="Lineair: $C(1-\\alpha\\Delta h)$",
    )
    ax2.axvline(0.5, color=GREY, lw=1, ls=":")
    ax2.text(
        0.52,
        C * 0.6 / 1e6,
        r"$\alpha\Delta h = 0.5$" + "\n(grens linearis.)",
        fontsize=8,
        color=GREY,
    )
    ax2.set_xlabel(r"$\Delta h$ [m]", fontsize=11)
    ax2.set_ylabel(r"$\mathrm{NCW}_\mathrm{risico}\ [\mathrm{M€}]$", fontsize=11)
    ax2.set_title(r"NCW als functie van $\Delta h$", fontsize=12, color=DARK)
    ax2.legend(fontsize=10)
    ax2.set_xlim(0, 2)
    ax2.set_ylim(bottom=0)

    # --- Panel 3: Testcase TC1 visualisatie ---
    ax3 = axes[1, 0]
    ax3.set_title("TC1 — MIN_COST: optimale maatregelenkeuze", fontsize=11, color=DARK)
    ax3.axis("off")

    measures = [
        ("M01", 0.5, 2.0, RED),
        ("M02", 0.3, 1.0, BLUE),
        ("M03", 0.2, 0.5, GREEN),
        ("M02+M03", 0.5, 1.5, PURPLE),
    ]
    h_min = math.log(1e-2 / 1e-3) / 5.0  # ≈ 0.461 m

    bar_ax = ax3.inset_axes([0.05, 0.12, 0.90, 0.75])
    names = [m[0] for m in measures]
    costs = [m[2] for m in measures]
    colors = [m[3] for m in measures]
    heights = [m[1] for m in measures]

    bars = bar_ax.bar(
        range(len(names)),
        costs,
        color=colors,
        alpha=0.85,
        edgecolor="white",
        linewidth=1.5,
    )
    bar_ax.set_xticks(range(len(names)))
    bar_ax.set_xticklabels(names, fontsize=9)
    bar_ax.axhline(1.5, color=PURPLE, lw=1.5, ls="--", alpha=0.6)
    bar_ax.set_ylabel("Investering [M€]", fontsize=10)
    bar_ax.set_ylim(0, 2.6)

    for bar, h in zip(bars, heights):
        ok = "✓" if h >= h_min else "✗"
        clr = "#059669" if h >= h_min else RED
        bar_ax.text(
            bar.get_x() + bar.get_width() / 2,
            bar.get_height() + 0.05,
            f"Δh={h:.1f}m {ok}",
            ha="center",
            fontsize=9,
            color=clr,
            fontweight="bold",
        )

    bar_ax.text(
        3,
        1.62,
        "Optimum\n1.5 M€",
        ha="center",
        fontsize=9,
        color=PURPLE,
        fontweight="bold",
    )
    bar_ax.text(
        1.5,
        0.08,
        rf"$h_{{\min}}={h_min:.3f}\,\mathrm{{m}}$",
        fontsize=9,
        color=DARK,
        ha="center",
    )

    ax3.text(
        0.5,
        0.02,
        "BruteForce == Pyomo: {M02, M03} ✓",
        ha="center",
        fontsize=10,
        color="#059669",
        fontweight="bold",
        transform=ax3.transAxes,
    )

    # --- Panel 4: Verificatiesamenvatting ---
    ax4 = axes[1, 1]
    ax4.axis("off")
    ax4.set_title("Verificatieresultaten (kritiek)", fontsize=11, color=DARK)

    test_results = [
        ("TC1", "MIN_COST", "M02+M03 goedkoopst", True),
        ("TC2", "MIN_COST", "Norm al gehaald → {}", True),
        ("TC3", "MIN_COST", "Dependency M02→M01", True),
        ("TC4", "MAX_RR", "Knapsack binnen budget", True),
        ("TC5", "MIN_NCW", "αh=0.1, fout < 1%", True),
        ("TC6", "MIN_NCW", "Duur vs. voordelig", True),
    ]
    y_start = 0.88
    ax4.text(
        0.02,
        y_start + 0.05,
        "TC",
        fontsize=9,
        fontweight="bold",
        transform=ax4.transAxes,
        color=DARK,
    )
    ax4.text(
        0.16,
        y_start + 0.05,
        "Objective",
        fontsize=9,
        fontweight="bold",
        transform=ax4.transAxes,
        color=DARK,
    )
    ax4.text(
        0.46,
        y_start + 0.05,
        "Scenario",
        fontsize=9,
        fontweight="bold",
        transform=ax4.transAxes,
        color=DARK,
    )
    ax4.text(
        0.86,
        y_start + 0.05,
        "Status",
        fontsize=9,
        fontweight="bold",
        transform=ax4.transAxes,
        color=DARK,
    )
    ax4.plot(
        [0.01, 0.99],
        [y_start + 0.02, y_start + 0.02],
        transform=ax4.transAxes,
        color=GREY,
        lw=0.5,
    )

    for i, (tc, obj, desc, ok) in enumerate(test_results):
        y = y_start - 0.12 * (i + 1)
        bg = "#f0fdf4" if i % 2 == 0 else WHITE
        ax4.add_patch(
            mpatches.FancyBboxPatch(
                (0.01, y - 0.04),
                0.97,
                0.10,
                transform=ax4.transAxes,
                boxstyle="round,pad=0.01",
                facecolor=bg,
                edgecolor="none",
            )
        )
        ax4.text(0.03, y + 0.01, tc, fontsize=9, transform=ax4.transAxes, color=DARK)
        ax4.text(0.16, y + 0.01, obj, fontsize=8, transform=ax4.transAxes, color=BLUE)
        ax4.text(
            0.46, y + 0.01, desc, fontsize=8, transform=ax4.transAxes, color="#333"
        )
        status = "BF == Pyomo ✓" if ok else "AFWIJKING ✗"
        color = "#059669" if ok else RED
        ax4.text(
            0.86,
            y + 0.01,
            status,
            fontsize=8,
            fontweight="bold",
            transform=ax4.transAxes,
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

    plt.tight_layout()
    plt.savefig(OUT / "stap1.3_optimization.png", dpi=150, bbox_inches="tight")
    plt.close()
    print("  stap1.3_optimization.png")


# ---------------------------------------------------------------------------
# 5. Database mapping
# ---------------------------------------------------------------------------


def make_database_mapping() -> None:
    fig, ax = plt.subplots(figsize=(13, 7), facecolor=BG)
    ax.set_xlim(0, 13)
    ax.set_ylim(0, 7)
    ax.axis("off")
    ax.set_facecolor(BG)

    fig.suptitle(
        "OptimaliseRing DB  →  FloodOpt datamodel",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )
    ax.text(
        6.5,
        6.55,
        "optimalise_ring_2011.sqlite  |  103 dijkringen  |  176 trajecten  |  3348 klimaatrecords",
        ha="center",
        fontsize=10,
        color=GREY,
    )

    # Left: MDB tables
    mdb_tables = [
        ("Dijkringen", "103", "Id, Naam, Terugkeertijd"),
        ("DijkringTrajecten", "176", "H0 [cm+NAP], Factor"),
        (
            "Klimaat_...DataTraject",
            "3348",
            r"$\alpha$ [1/cm], $P_0$ [1/j], $\eta$ [cm/j]",
        ),
        (
            "ParametersKostenfunctieData",
            "183",
            r"$\lambda$ [1/cm], $C_\mathrm{exp}$, $b$, $\Omega$",
        ),
        ("SchadeFunctieData", "372", r"$\nu$, $\zeta$, $\psi$"),
        ("EconomischScenarioData", "868", r"$\gamma$ (economische groei)"),
        ("RamingVoorSlachtoffersData", "372", "Slachtoffers, Getroffenen"),
    ]

    header_rect = mpatches.FancyBboxPatch(
        (0.2, 5.7),
        4.8,
        0.45,
        boxstyle="round,pad=0.05",
        facecolor="#495057",
        edgecolor="none",
    )
    ax.add_patch(header_rect)
    ax.text(
        2.6,
        5.92,
        "OptimaliseRing MDB (origineel)",
        ha="center",
        fontsize=11,
        fontweight="bold",
        color=WHITE,
    )

    for i, (name, rows, cols) in enumerate(mdb_tables):
        y = 5.3 - i * 0.68
        color = "#fff3cd" if name == "Klimaat_...DataTraject" else "#f1f3f5"
        r = mpatches.FancyBboxPatch(
            (0.2, y - 0.22),
            4.8,
            0.44,
            boxstyle="round,pad=0.03",
            facecolor=color,
            edgecolor="#dee2e6",
            linewidth=0.8,
        )
        ax.add_patch(r)
        ax.text(0.4, y + 0.10, name, fontsize=9, fontweight="bold", color="#212529")
        ax.text(4.8, y + 0.10, rows, fontsize=8, color=GREY, ha="right")
        ax.text(0.4, y - 0.09, cols, fontsize=8, color="#495057")

    # Conversie box
    conv = mpatches.FancyBboxPatch(
        (0.2, 0.25),
        4.8,
        0.90,
        boxstyle="round,pad=0.05",
        facecolor="#e8f4fd",
        edgecolor="#90c8f0",
        linewidth=1.2,
    )
    ax.add_patch(conv)
    ax.text(
        2.6,
        1.05,
        "Eenheidsconversie (SQLite views)",
        ha="center",
        fontsize=10,
        fontweight="bold",
        color=BLUE,
    )
    ax.text(
        0.5,
        0.78,
        r"$\alpha:\times 100\ (1/\mathrm{cm}\to 1/\mathrm{m})$",
        fontsize=9,
        color="#0a58ca",
    )
    ax.text(
        0.5,
        0.58,
        r"$\eta:\div 100\ (\mathrm{cm/j}\to \mathrm{m/j})$",
        fontsize=9,
        color="#0a58ca",
    )
    ax.text(
        0.5,
        0.38,
        r"$H_0:\div 100\ (\mathrm{cm+NAP}\to \mathrm{m+NAP})$",
        fontsize=9,
        color="#0a58ca",
    )

    # Arrow
    ax.annotate(
        "",
        xy=(7.5, 3.2),
        xytext=(5.2, 3.2),
        arrowprops=dict(arrowstyle="-|>", color=BLUE, lw=2.5),
    )
    ax.text(6.35, 3.45, "SQLite\nviews", ha="center", fontsize=9, color=BLUE)

    # Right: FloodOpt models
    header_rect2 = mpatches.FancyBboxPatch(
        (7.8, 5.7),
        4.9,
        0.45,
        boxstyle="round,pad=0.05",
        facecolor=BLUE,
        edgecolor="none",
    )
    ax.add_patch(header_rect2)
    ax.text(
        10.25,
        5.92,
        "FloodOpt datamodel [m]",
        ha="center",
        fontsize=11,
        fontweight="bold",
        color=WHITE,
    )

    floodopt_panels = [
        (
            BLUE,
            4.5,
            "Trajectory",
            [
                r"norm $= 1/\mathrm{Terugkeertijd}$",
                r"$p_0 = P_0$ [1/jaar]",
                r"$\alpha = \alpha_\mathrm{MDB}\times 100$ [1/m]",
                r"base\_year, length, id",
            ],
            "v_trajecten_floodopt (3168 rijen)",
        ),
        (
            PURPLE,
            2.6,
            "Scenario",
            [r"climate = Klimaat naam", r"$\eta = \eta_\mathrm{MDB}/100$ [m/jaar]"],
            "18 klimaatscenarios",
        ),
        (
            GREEN,
            1.1,
            "Measure + RiskParams",
            [
                r"effect [m] = $\Delta h$",
                r"$\gamma$ via EconomischScenario",
                r"base\_damage via SchadeFunctie",
            ],
            "Beschikbaar voor stap 1.3 validatie",
        ),
    ]

    for color, y_c, title, fields, note in floodopt_panels:
        h = 0.36 * len(fields) + 0.55
        r = mpatches.FancyBboxPatch(
            (7.8, y_c - h / 2),
            4.9,
            h,
            boxstyle="round,pad=0.05",
            facecolor=color + "22",
            edgecolor=color,
            linewidth=1.5,
        )
        ax.add_patch(r)
        ax.text(
            8.0, y_c + h / 2 - 0.28, title, fontsize=10, fontweight="bold", color=color
        )
        for j, field in enumerate(fields):
            ax.text(
                8.0, y_c + h / 2 - 0.58 - j * 0.30, field, fontsize=8, color="#1e3a5f"
            )
        ax.text(
            8.0, y_c - h / 2 + 0.12, note, fontsize=7, color=GREY, fontstyle="italic"
        )

    plt.tight_layout()
    plt.savefig(OUT / "database_mapping.png", dpi=150, bbox_inches="tight")
    plt.close()
    print("  database_mapping.png")


# ---------------------------------------------------------------------------
# 6. Stap 1.4 — Smoke test resultaten
# ---------------------------------------------------------------------------


def make_smoke_test() -> None:
    """Visualiseert de end-to-end smoke test: traject -> optimizer -> resultaat."""
    fig = plt.figure(figsize=(13, 8), facecolor=BG)
    gs = gridspec.GridSpec(2, 2, hspace=0.45, wspace=0.35)

    ax_flow = fig.add_subplot(gs[0, :])  # end-to-end flow
    ax_time = fig.add_subplot(gs[1, 0])  # rekentijden
    ax_table = fig.add_subplot(gs[1, 1])  # resultaten per objective

    fig.suptitle(
        "Stap 1.4 — Smoke Test: Traject → Optimizer → Resultaat",
        fontsize=14,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )

    # --- Flow diagram ---
    ax_flow.set_xlim(0, 13)
    ax_flow.set_ylim(0, 3)
    ax_flow.axis("off")

    boxes = [
        (
            0.4,
            1.5,
            2.0,
            1.8,
            BLUE,
            "Trajectory\n& Scenario",
            r"$P_0=1/200$, $\alpha=4$, $\eta=0.003$",
        ),
        (
            3.0,
            1.5,
            2.0,
            1.8,
            GREEN,
            "5 Kandidaat-\nmaatregelen",
            "Δh: 0.15–0.50 m\nkosten: 0.5–2 M€",
        ),
        (
            5.8,
            1.5,
            2.0,
            1.8,
            PURPLE,
            "Risk Layer\n(NCW berekening)",
            r"$\mathrm{NCW}=\sum P(s)\cdot V_0\cdot e^{(\gamma-\delta)s}$",
        ),
        (8.6, 1.5, 2.0, 1.8, RED, "BruteForce\n& Pyomo", r"$2^5=32$ combinaties"),
        (11.4, 1.5, 1.4, 1.8, "#059669", "Resultaat\n✓ MATCH", "3/3 objectives"),
    ]

    for x, y, w, h, color, title, sub in boxes:
        rect = mpatches.FancyBboxPatch(
            (x - w / 2, y - h / 2),
            w,
            h,
            boxstyle="round,pad=0.12",
            facecolor=color + "33",
            edgecolor=color,
            linewidth=2,
        )
        ax_flow.add_patch(rect)
        ax_flow.text(
            x,
            y + 0.25,
            title,
            ha="center",
            va="center",
            fontsize=9,
            fontweight="bold",
            color=color,
        )
        ax_flow.text(
            x, y - 0.30, sub, ha="center", va="center", fontsize=7, color="#333333"
        )

    # Pijlen
    for x_start, x_end in [(1.4, 2.0), (4.0, 4.8), (6.8, 7.6), (9.6, 10.4)]:
        ax_flow.annotate(
            "",
            xy=(x_end, 1.5),
            xytext=(x_start, 1.5),
            arrowprops=dict(arrowstyle="-|>", color=GREY, lw=2),
        )

    # --- Rekentijden ---
    objectives = ["MIN_COST", "MAX_RISK_RED.", "MIN_NCW"]
    bf_times = [9.6, 5.2, 6.3]  # ms uit smoke test output
    py_times = [184.6, 55.6, 7.6]

    x_pos = np.arange(len(objectives))
    w = 0.35
    ax_time.bar(x_pos - w / 2, bf_times, w, label="BruteForce", color=RED, alpha=0.85)
    ax_time.bar(x_pos + w / 2, py_times, w, label="Pyomo/HiGHS", color=BLUE, alpha=0.85)

    for i, (b, p) in enumerate(zip(bf_times, py_times)):
        ax_time.text(i - w / 2, b + 1.5, f"{b:.0f}", ha="center", fontsize=8, color=RED)
        ax_time.text(
            i + w / 2, p + 1.5, f"{p:.0f}", ha="center", fontsize=8, color=BLUE
        )

    ax_time.set_xticks(x_pos)
    ax_time.set_xticklabels(objectives, fontsize=9)
    ax_time.set_ylabel("Rekentijd [ms]", fontsize=10)
    ax_time.set_title("Rekentijden  (N=5 maatregelen)", fontsize=11, color=DARK)
    ax_time.legend(fontsize=9)
    ax_time.set_ylim(0, 220)
    ax_time.text(
        0.98,
        0.96,
        "BruteForce sneller voor N≤5\nPyomo schaalt beter voor grote N",
        ha="right",
        va="top",
        fontsize=8,
        color=GREY,
        transform=ax_time.transAxes,
        bbox=dict(boxstyle="round,pad=0.3", facecolor=WHITE, edgecolor=GREY),
    )

    # --- Resultaten tabel ---
    ax_table.axis("off")
    ax_table.set_title("Verificatieresultaten", fontsize=11, color=DARK)
    headers = ["Objective", "Optimum", "Waarde", "Match"]
    rows = [
        ["MIN_COST", "{M02, M04}", "€ 1,089,224", "BF = Py ✓"],
        ["MAX_RISK_R.", "{M02, M03, M04}", "Δh = 0.80 m", "BF = Py ✓"],
        ["MIN_NCW", "{alle 5}", "NCW = 9.0 M€", "BF = Py ✓"],
    ]
    tbl = ax_table.table(
        cellText=rows, colLabels=headers, loc="center", cellLoc="center"
    )
    tbl.auto_set_font_size(False)
    tbl.set_fontsize(9)
    tbl.scale(1, 2.0)
    for (r, c), cell in tbl.get_celld().items():
        if r == 0:
            cell.set_facecolor(DARK)
            cell.set_text_props(color="white", fontweight="bold")
        elif c == 3:
            cell.set_text_props(color="#059669", fontweight="bold")
        elif r % 2 == 0:
            cell.set_facecolor("#f0fdf4")

    ax_table.text(
        0.5,
        0.05,
        "58/58 tests geslaagd  ·  exitcode 0  ·  mypy schoon",
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
# 7. Stap 2.1 — FastAPI service
# ---------------------------------------------------------------------------


def make_api() -> None:
    """Visualiseert de FastAPI-architectuur en endpoint-flow."""
    fig, axes = plt.subplots(1, 2, figsize=(14, 7), facecolor=BG)
    fig.suptitle(
        "Stap 2.1 — FastAPI service: dunne HTTP-schil om floodopt-core",
        fontsize=13,
        fontweight="bold",
        color=DARK,
        y=0.99,
    )

    # --- Panel 1: Request-flow diagram ---
    ax = axes[0]
    ax.set_xlim(0, 10)
    ax.set_ylim(0, 10)
    ax.axis("off")
    ax.set_title("Request-flow (stap 2.1 MVP)", fontsize=11, color=DARK)

    # Client box
    client_rect = mpatches.FancyBboxPatch(
        (0.3, 8.0),
        2.2,
        1.2,
        boxstyle="round,pad=0.1",
        facecolor="#e8f0fe",
        edgecolor=BLUE,
        linewidth=2,
    )
    ax.add_patch(client_rect)
    ax.text(
        1.4,
        8.65,
        "Client /\nSwagger UI",
        ha="center",
        va="center",
        fontsize=9,
        fontweight="bold",
        color=BLUE,
    )

    # API box
    api_rect = mpatches.FancyBboxPatch(
        (3.8, 7.5),
        2.8,
        2.2,
        boxstyle="round,pad=0.1",
        facecolor="#fff3cd",
        edgecolor="#f4a261",
        linewidth=2,
    )
    ax.add_patch(api_rect)
    ax.text(
        5.2,
        9.3,
        "FastAPI",
        ha="center",
        fontsize=10,
        fontweight="bold",
        color="#856404",
    )
    endpoints = [
        "POST /scenarios",
        "POST /trajectories",
        "POST /optimize",
        "GET  /results/{id}",
    ]
    for i, ep in enumerate(endpoints):
        ax.text(
            5.2,
            8.8 - i * 0.35,
            ep,
            ha="center",
            fontsize=8,
            color="#495057",
            fontfamily="monospace",
        )

    # Core box
    core_rect = mpatches.FancyBboxPatch(
        (3.8, 3.5),
        2.8,
        3.2,
        boxstyle="round,pad=0.1",
        facecolor="#d1fae5",
        edgecolor=GREEN,
        linewidth=2,
    )
    ax.add_patch(core_rect)
    ax.text(
        5.2,
        6.4,
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
        ax.text(5.2, 5.9 - i * 0.55, item, ha="center", fontsize=9, color="#065f46")

    # Store box
    store_rect = mpatches.FancyBboxPatch(
        (7.5, 5.5),
        2.2,
        2.2,
        boxstyle="round,pad=0.1",
        facecolor="#f1f3f5",
        edgecolor=GREY,
        linewidth=1.5,
        linestyle="--",
    )
    ax.add_patch(store_rect)
    ax.text(
        8.6,
        6.9,
        "Store\n(in-memory)",
        ha="center",
        fontsize=9,
        color=GREY,
        fontweight="bold",
    )
    ax.text(
        8.6,
        6.3,
        "scenarios{}",
        ha="center",
        fontsize=8,
        color=GREY,
        fontfamily="monospace",
    )
    ax.text(
        8.6,
        5.9,
        "trajectories{}",
        ha="center",
        fontsize=8,
        color=GREY,
        fontfamily="monospace",
    )
    ax.text(
        8.6,
        5.5,
        "results{}",
        ha="center",
        fontsize=8,
        color=GREY,
        fontfamily="monospace",
    )
    ax.text(
        8.6,
        5.1,
        "→ PostgreSQL\n  (stap 2.2)",
        ha="center",
        fontsize=8,
        color="#aaa",
        fontstyle="italic",
    )

    # Pijlen
    ax.annotate(
        "",
        xy=(3.8, 8.65),
        xytext=(2.5, 8.65),
        arrowprops=dict(arrowstyle="-|>", color=BLUE, lw=2),
    )
    ax.annotate(
        "",
        xy=(5.2, 7.5),
        xytext=(5.2, 6.7),
        arrowprops=dict(arrowstyle="-|>", color="#856404", lw=2),
    )
    ax.annotate(
        "",
        xy=(7.5, 6.5),
        xytext=(6.6, 6.5),
        arrowprops=dict(arrowstyle="-|>", color=GREY, lw=1.5),
    )

    ax.text(3.1, 8.80, "HTTP", fontsize=8, color=BLUE)
    ax.text(5.3, 7.05, "roept aan", fontsize=8, color="#856404")
    ax.text(6.65, 6.65, "lees/schrijf", fontsize=8, color=GREY)

    ax.text(
        5.2,
        2.8,
        "Geen business logic in API-laag ✓",
        ha="center",
        fontsize=10,
        color="#059669",
        fontweight="bold",
        bbox=dict(boxstyle="round,pad=0.4", facecolor="#f0fdf4", edgecolor="#059669"),
    )

    # --- Panel 2: Endpoint-tabel ---
    ax2 = axes[1]
    ax2.axis("off")
    ax2.set_title("Endpoints & verificatie (20/20 tests ✓)", fontsize=11, color=DARK)

    headers = ["Method", "Pad", "Status", "Verificatie"]
    rows = [
        ["POST", "/scenarios", "201", "roundtrip ✓"],
        ["GET", "/scenarios/{id}", "200/404", "404 test ✓"],
        ["POST", "/trajectories", "201", "roundtrip ✓"],
        ["GET", "/trajectories/{id}", "200/404", "404 test ✓"],
        ["POST", "/optimize", "201", "= stap 1.4 ✓"],
        ["GET", "/results/{job_id}", "200/404", "= POST resp ✓"],
        ["GET", "/docs", "200", "Swagger UI ✓"],
    ]

    tbl = ax2.table(
        cellText=rows,
        colLabels=headers,
        loc="upper center",
        cellLoc="center",
        bbox=[0.0, 0.45, 1.0, 0.52],
    )
    tbl.auto_set_font_size(False)
    tbl.set_fontsize(9)
    tbl.scale(1, 1.8)
    for (r, c), cell in tbl.get_celld().items():
        if r == 0:
            cell.set_facecolor(DARK)
            cell.set_text_props(color="white", fontweight="bold")
        elif c == 0:
            color = BLUE if rows[r - 1][0] == "POST" else GREEN
            cell.set_text_props(color=color, fontweight="bold")
        elif c == 3:
            cell.set_text_props(color="#059669")
        elif r % 2 == 0:
            cell.set_facecolor("#f8f9fa")

    # Sleutelverificaties
    verifs = [
        ("POST /optimize MIN_COST", "{M02, M04} investering €1,089,224", True),
        ("POST /optimize MAX_RR", "{M02, M03, M04} Δh=0.80m", True),
        ("POST /optimize MIN_NCW", "{alle 5} NCW=€9.0M", True),
        ("BruteForce == Pyomo", "via API beide solvers", True),
        ("Geen physics in API", "math.exp afwezig in main.py", True),
    ]
    y0 = 0.38
    ax2.text(
        0.5,
        y0,
        "Kritieke verificaties vs. stap 1.4:",
        ha="center",
        fontsize=10,
        fontweight="bold",
        color=DARK,
        transform=ax2.transAxes,
    )
    for i, (label, detail, ok) in enumerate(verifs):
        y = y0 - 0.07 * (i + 1)
        icon = "✓" if ok else "✗"
        color = "#059669" if ok else RED
        ax2.text(
            0.03,
            y,
            f"{icon}  {label}",
            fontsize=9,
            color=color,
            fontweight="bold",
            transform=ax2.transAxes,
        )
        ax2.text(
            0.03,
            y - 0.035,
            f"   {detail}",
            fontsize=8,
            color="#495057",
            transform=ax2.transAxes,
        )

    ax2.text(
        0.5,
        0.02,
        "78/78 tests geslaagd  ·  Swagger /docs ✓  ·  mypy schoon",
        ha="center",
        fontsize=10,
        color="#059669",
        fontweight="bold",
        transform=ax2.transAxes,
    )

    plt.tight_layout()
    plt.savefig(OUT / "stap2.1_api.png", dpi=150, bbox_inches="tight")
    plt.close()
    print("  stap2.1_api.png")


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
    print(f"\nKlaar — alle PNG's opgeslagen in {OUT}/")
