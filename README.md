# FloodOpt

Open-source platform voor optimalisatie van dijkversterking en rivierverruiming.

## Doel

FloodOpt optimaliseert waterveiligheidsstrategieën op traject-, dijkring- en systeemniveau door dijkversterking en rivierverruiming te combineren. Het platform ondersteunt probabilistische risico's, meerdere klimaatscenario's, meerdere faalmechanismen en robuuste optimalisatie.

Toekomstige toepassingen: HWBP-analyses, MKBA, asset management, klimaatstresstesten voor financiële instellingen, portefeuille-optimalisatie.

## Architectuur

Drie strikte lagen — de optimizer bevat nooit fysica:

```
Optimization Layer
      ↓
  Risk Layer
      ↓
Physics Layer
```

| Laag | Verantwoordelijkheid | Kernformule | Output |
|---|---|---|---|
| Physics | Hydraulica, geotechniek | $P(t) = P_0 \cdot e^{\alpha\eta t} \cdot e^{-\alpha\Delta h}$ | Faalkansen, kruinhoogten |
| Risk | Schade, NCW | $\mathrm{NCW} = \sum_{s=0}^{T-1} P(s)\cdot V_0\cdot e^{(\gamma-\delta)s}$ | Verwachte schade, risico NCW |
| Optimization | Maatregelstrategie via Pyomo | $\min \sum c_i x_i \;\text{s.t.}\; \sum h_i x_i \geq h_{\min}$ | Optimale maatregelencombinatie |

Zie `docs/architecture.png` voor het volledige architectuurdiagram.

## Tech stack

| Component | Technologie |
|---|---|
| Rekenkernel | Python (`floodopt-core`) |
| Optimizer | Pyomo 6 + HiGHS (MILP) / Ipopt (NLP) |
| Backend API | FastAPI *(gepland fase 2)* |
| Database | PostgreSQL + PostGIS *(gepland fase 2)* |
| Queue | Redis + Celery *(gepland fase 2)* |
| Frontend | React + Vite + Leaflet / MapLibre *(gepland fase 4)* |
| Documentatie | matplotlib (formules + diagrammen) |

## Projectstructuur

```
floodopt/
├── floodopt-core/           # rekenkernel — geen FastAPI, pure Python
│   └── floodopt_core/
│       ├── io/              # Pydantic datamodellen (Measure, Scenario, Trajectory)
│       ├── physics/         # PhysicsModel Protocol + SimpleDikeOverflow
│       ├── risk/            # RiskCalculator Protocol + SimpleRiskCalculator
│       ├── optimization/    # OptimizationStrategy Protocol + BruteForce + Pyomo
│       └── utils/
├── floodopt-api/            # FastAPI backend (gepland)
├── floodopt-worker/         # Celery workers (gepland)
├── floodopt-frontend/       # React + Vite (gepland)
├── tests/
│   ├── unit/                # 46 tests — alle lagen
│   ├── integration/
│   └── validation/          # optimalise_ring_2011.sqlite (referentiedata)
├── docs/                    # Diagrammen (PNG) + SVG bronbestanden
└── scripts/                 # generate_docs.py, convert_mdb_to_sqlite.py
```

## Ontwikkelprincipes

- **Strikte laagscheiding** — optimizer roept altijd de rekenkernel aan; bevat nooit fysische formules
- **Modulaire interfaces** — elke laag implementeert een Protocol; implementaties zijn vervangbaar
- **Dubbele validatie** — elke optimalisatie wordt geverifieerd met een brute-force referentie
- **Stapsgewijze uitbreiding** — MVP eerst, complexiteit per fase toevoegen

## Documentatie

Alle diagrammen worden gegenereerd met `python scripts/generate_docs.py`:

| Diagram | Bestand |
|---|---|
| Architectuur | `docs/architecture.png` |
| Physics Layer — $P(t)$ formule | `docs/stap1.1_physics_formula.png` |
| Risk Layer — NCW berekening | `docs/stap1.2_risk_ncw.png` |
| Optimization Layer — BruteForce vs Pyomo | `docs/stap1.3_optimization.png` |
| Database mapping MDB → FloodOpt | `docs/database_mapping.png` |

## Fasering

| Fase | Status | Inhoud |
|---|---|---|
| 0 — Tooling | ✅ Klaar | Projectstructuur, packaging, CI-tooling |
| 1 — MVP rekenkernel | ✅ Klaar | Physics, Risk, Optimization, CLI smoke test |
| 2 — Backend & API | 🔄 2.2 klaar | FastAPI ✅, PostgreSQL ✅, Celery ⏳ |
| 3 — Uitbreidingen | ⏳ Gepland | FORM/Monte Carlo, lengte-effecten, rivierverruiming |
| 4 — Frontend | ⏳ Gepland | React + Vite, kaartviewer |

## Validatiestrategie

Elke optimalisatie wordt gevalideerd door de uitkomst te vergelijken met een brute-force berekening over dezelfde maatregelencombinaties. Bij afwijking stopt de ontwikkeling totdat de oorzaak is vastgesteld.

Referentiedataset: `tests/validation/optimalise_ring_2011.sqlite` — afgeleid van OptimaliseRing v2.3.2 (HKV, 2013), 103 dijkringen, 176 trajecten.

| Niveau | Methode | Status |
|---|---|---|
| Unit | pytest per functie | ✅ 46/46 geslaagd |
| Integratie | CLI smoke test end-to-end | ✅ 12/12 geslaagd |
| Laag | Protocol-conformiteit via mypy | ✅ Schoon |
| Integratie | BruteForce == Optimizer voor alle testcases | ✅ 6/6 testcases |
| Regressie | CI runt validatie bij elke commit | ⏳ Gepland |

---

## Stappenplan

Volgorde: `0.1 → 0.2 → 1.1 → 1.2 → 1.3 → 1.4 → 2.1 → 2.2 → 2.3 → 3.x → 4`

Na elke stap: verificatie en validatie voor er verder wordt gegaan.

### Fase 0 — Projectstructuur & Tooling

#### Stap 0.1 — Repository & package layout ✅
- Mapstructuur aangemaakt, tooling geïnstalleerd (`pytest`, `ruff`, `mypy`, `pre-commit`)
- `floodopt-core` als editable package geïnstalleerd

#### Stap 0.2 — Data model ✅
Pydantic v2 models in `floodopt_core/io/`:
- `Measure` — id, type, cost, year, **effect** [m], location, dependencies
- `Scenario` — id, climate, q_design, h_design, **eta** [m/jaar]
- `Trajectory` — id, norm, length, **p0**, **alpha**, **base_year**, measures

---

### Fase 1 — MVP rekenkernel (traject-niveau)

#### Stap 1.1 — Physics Layer ✅

$$P(t) = P_0 \cdot e^{\alpha \eta t} \cdot e^{-\alpha \Delta h}$$

Implementatie: `SimpleDikeOverflow` achter `PhysicsModel` Protocol.
Formule identiek aan OptimaliseRing 2.3.2 (HKV, 2013).

#### Stap 1.2 — Risk Layer ✅

$$\mathrm{NCW} = \sum_{s=0}^{T-1} P(s) \cdot V_0 \cdot e^{(\gamma - \delta)\,s}$$

Implementatie: `SimpleRiskCalculator` achter `RiskCalculator` Protocol.

#### Stap 1.3 — Optimization Layer ✅
Beide implementaties achter `OptimizationStrategy` Protocol:

1. `BruteForceOptimizer` — itereert alle $2^N$ combinaties (referentie, exact)
2. `PyomoOptimizer` — MILP via Pyomo + HiGHS

| Objective | Formulering | Solver |
|---|---|---|
| `MIN_COST` | $\min \sum c_i x_i \;\text{s.t.}\; \sum h_i x_i \geq h_{\min}$ | HiGHS MILP (exact) |
| `MAX_RISK_REDUCTION` | $\max \sum h_i x_i \;\text{s.t.}\; \sum c_i x_i \leq B$ | HiGHS MILP (exact) |
| `MIN_NCW` | $\min \sum (c_i - C\alpha h_i)\,x_i$ | HiGHS MILP (lineaire benadering) |

**Kritieke verificatie:** `BruteForce.solve() == PyomoOptimizer.solve()` voor alle 6 testcases ✅

#### Stap 1.4 — Integratie smoke test (CLI) ✅

```bash
python scripts/run_smoke_test.py
```

End-to-end run zonder API of database: traject → optimizer → resultaat.

| Objective | Optimum | Waarde | Match |
|---|---|---|---|
| `MIN_COST` | {M02, M04} | € 1,089,224 | BF = Py ✓ |
| `MAX_RISK_REDUCTION` | {M02, M03, M04} | Δh = 0.80 m | BF = Py ✓ |
| `MIN_NCW` | {alle 5} | NCW = € 9.0M | BF = Py ✓ |

---

### Fase 2 — Backend & API ⏳

#### Stap 2.1 — FastAPI service
```
POST /scenarios
POST /trajectories
POST /optimize
GET  /results/{job_id}
```

#### Stap 2.2 — Database (PostgreSQL + PostGIS)

#### Stap 2.3 — Async queue (Redis + Celery)

---

### Fase 3 — Uitbreidingen rekenkernel ⏳

| Stap | Inhoud | Wat blijft ongewijzigd |
|---|---|---|
| 3.1 | Probabilistische sterkte: FORM / Monte Carlo | Optimizer, Risk Layer |
| 3.2 | Lengte-effecten & correlaties | Optimizer, Physics Layer |
| 3.3 | Rivierverruiming & hydraulische interacties | Optimizer, Risk Layer |

---

### Fase 4 — Frontend ⏳

React + Vite, Leaflet/MapLibre voor kaarten.

---

## Licentie

Open source — licentie volgt.
