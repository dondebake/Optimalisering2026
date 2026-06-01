# FloodOpt

Open-source platform voor optimalisatie van dijkversterking en rivierverruiming.

## Doel

FloodOpt optimaliseert waterveiligheidsstrategieën op traject-, dijkring- en systeemniveau door dijkversterking en rivierverruiming te combineren. Het platform ondersteunt probabilistische risico's, meerdere klimaatscenario's, meerdere faalmechanismen en robuuste optimalisatie.

Toekomstige toepassingen: HWBP-analyses, MKBA, asset management, klimaatstresstesten voor financiële instellingen, portefeuille-optimalisatie.

## Architectuur

```
┌──────────────────────────────────────────────────────┐
│  Frontend  React + Vite + Leaflet                    │
│  Kaarten: GeoJSON van API  |  Resultaten-dashboard   │
└────────────────────┬─────────────────────────────────┘
                     │ HTTP / GeoJSON
┌────────────────────▼─────────────────────────────────┐
│  FastAPI  (dunne HTTP-schil, geen business logic)    │
│  POST /optimize  →  floodopt-core                    │
│  GET  /trajectories/{id}/geojson  →  GeoPandas       │
└──────┬─────────────────────────────────┬─────────────┘
       │ SQLAlchemy                      │ GeoPandas
┌──────▼──────────┐          ┌───────────▼────────────┐
│  SQLite (dev)   │          │  Geometrie             │
│  PostgreSQL     │          │  WKT / GeoJSON         │
│  (prod, opt.)   │          │  shapefiles            │
└─────────────────┘          └────────────────────────┘
                     │
┌────────────────────▼─────────────────────────────────┐
│  floodopt-core  (strikte laagscheiding)              │
│                                                      │
│  Optimization Layer  min Σcᵢxᵢ  s.t. Σhᵢxᵢ ≥ h_min │
│         ↓  roept aan                                 │
│  Risk Layer      NCW = Σ P(s)·V₀·e^((γ−δ)s)         │
│         ↓  roept aan                                 │
│  Physics Layer   P(t) = P₀·e^(αηt)·e^(−αΔh)         │
└──────────────────────────────────────────────────────┘
```

Zie `docs/architecture.png` voor het volledige architectuurdiagram.

## Tech stack

| Component | Technologie | Reden |
|---|---|---|
| Rekenkernel | Python (`floodopt-core`) | Pure Python, geen framework-lock-in |
| Optimizer | Pyomo 6 + HiGHS (MILP) | Open-source, schaalt naar grote N |
| Backend API | FastAPI | Snel, automatische Swagger UI |
| Database | **SQLite** (dev) → PostgreSQL optioneel (prod) | Nul installatie voor development |
| Geo-verwerking | **GeoPandas** + GeoJSON | Server-side geometrie zonder PostGIS |
| Kaarten (frontend) | **Leaflet** + React + Vite | Leest GeoJSON direct van API |
| Queue | Redis + Celery *(stap 2.3)* | Async optimalisaties |
| Documentatie | matplotlib (LaTeX-formules) | Reproduceerbaar via `generate_docs.py` |

### Geo-stack: waarom GeoPandas + Leaflet in plaats van PostGIS

| | GeoPandas + Leaflet | PostGIS |
|---|---|---|
| Installatie | Nul (Python-package) | Docker of server vereist |
| Dijkvak-alignementen | ✓ via WKT in SQLite + GeoPandas | ✓ native |
| GeoJSON naar Leaflet | ✓ GeoPandas → `to_json()` | ✓ ST_AsGeoJSON() |
| Ruimtelijke DB-queries | ✗ (niet nodig voor MVP) | ✓ |
| Prod upgrade pad | `DATABASE_URL` → PostgreSQL + PostGIS | direct |

PostGIS is beschikbaar zodra de query-complexiteit het rechtvaardigt — de `docker-compose.yml` en `init_schema()` staan al klaar.

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
├── floodopt-api/            # FastAPI backend
│   └── floodopt_api/
│       ├── main.py          # endpoints + DI
│       ├── database.py      # SQLAlchemy ORM (SQLite / PostgreSQL)
│       └── repositories.py  # Memory + Postgres implementaties
├── floodopt-worker/         # Celery workers (stap 2.3)
├── floodopt-frontend/       # React + Vite + Leaflet (stap 4)
├── tests/
│   ├── unit/                # 46 tests — alle lagen
│   ├── integration/         # 38 tests — CLI, API, database
│   └── validation/          # optimalise_ring_2011.sqlite (referentiedata)
├── docs/                    # PNG-diagrammen (via generate_docs.py) + SVG bronnen
└── scripts/                 # generate_docs.py, run_smoke_test.py, init_db.py
```

## Ontwikkelprincipes

- **Strikte laagscheiding** — optimizer bevat nooit fysische formules; API bevat geen business logic
- **Modulaire interfaces** — elke laag implementeert een Protocol; implementaties zijn vervangbaar
- **Dubbele validatie** — elke optimalisatie geverifieerd met brute-force referentie
- **Installatie-arm** — SQLite + in-memory tests; geen Docker vereist voor development
- **Stapsgewijze uitbreiding** — MVP eerst, PostGIS / Celery / frontend pas als nodig

## Documentatie

Alle diagrammen worden gegenereerd met `python scripts/generate_docs.py`:

| Diagram | Bestand |
|---|---|
| Volledige architectuur (rekenkernel + API + geo + frontend) | `docs/architecture.png` |
| Physics Layer — $P(t)$ formule + curves | `docs/stap1.1_physics_formula.png` |
| Risk Layer — NCW berekening | `docs/stap1.2_risk_ncw.png` |
| Optimization Layer — BruteForce vs Pyomo | `docs/stap1.3_optimization.png` |
| Smoke test — end-to-end verificatie | `docs/stap1.4_smoke_test.png` |
| FastAPI service — endpoints + flow | `docs/stap2.1_api.png` |
| Database — SQLite/PostgreSQL + repository-pattern | `docs/stap2.2_database.png` |
| Geo-stack — GeoPandas + Leaflet + GeoJSON | `docs/geo_stack.png` |
| Database mapping MDB → FloodOpt | `docs/database_mapping.png` |

## Fasering

| Fase | Status | Inhoud |
|---|---|---|
| 0 — Tooling | ✅ Klaar | Projectstructuur, packaging, pre-commit |
| 1 — MVP rekenkernel | ✅ Klaar | Physics, Risk, Optimization, CLI smoke test |
| 2 — Backend & API | 🔄 2.2 klaar | FastAPI ✅, SQLite ✅, Celery ⏳ |
| 3 — Uitbreidingen rekenkernel | ⏳ Gepland | FORM/Monte Carlo, lengte-effecten, rivierverruiming |
| 4 — Frontend | ⏳ Gepland | React + Vite + Leaflet, GeoPandas GeoJSON |

## Validatiestrategie

Referentiedataset: `tests/validation/optimalise_ring_2011.sqlite` — afgeleid van OptimaliseRing v2.3.2 (HKV, 2013), 103 dijkringen, 176 trajecten.

| Niveau | Methode | Status |
|---|---|---|
| Unit | pytest per functie | ✅ 46/46 |
| Integratie CLI | smoke test end-to-end | ✅ 12/12 |
| Integratie API | TestClient alle endpoints | ✅ 20/20 |
| Integratie DB | SQLite round-trip | ✅ 6/6 |
| Kritiek | BruteForce == Pyomo | ✅ 6/6 testcases |
| Regressie | CI bij elke commit | ⏳ Gepland |

---

## Stappenplan

Volgorde: `0.1 → 0.2 → 1.1 → 1.2 → 1.3 → 1.4 → 2.1 → 2.2 → 2.3 → 3.x → 4`

Na elke stap: verificatie en validatie voor er verder wordt gegaan.

### Fase 0 — Projectstructuur & Tooling

#### Stap 0.1 ✅ — Repository & package layout
Mapstructuur, tooling (`pytest`, `ruff`, `mypy`, `pre-commit`), editable install.

#### Stap 0.2 ✅ — Data model
Pydantic v2 models: `Measure` (effect [m]), `Scenario` (eta [m/j]), `Trajectory` (p0, alpha, base_year).

---

### Fase 1 — MVP rekenkernel (traject-niveau)

#### Stap 1.1 ✅ — Physics Layer

$$P(t) = P_0 \cdot e^{\alpha \eta t} \cdot e^{-\alpha \Delta h}$$

`SimpleDikeOverflow` achter `PhysicsModel` Protocol. Formule identiek aan OptimaliseRing 2.3.2 (HKV, 2013).

#### Stap 1.2 ✅ — Risk Layer

$$\mathrm{NCW} = \sum_{s=0}^{T-1} P(s) \cdot V_0 \cdot e^{(\gamma - \delta)\,s}$$

`SimpleRiskCalculator` achter `RiskCalculator` Protocol.

#### Stap 1.3 ✅ — Optimization Layer

| Objective | Formulering | Solver |
|---|---|---|
| `MIN_COST` | $\min \sum c_i x_i \;\text{s.t.}\; \sum h_i x_i \geq h_{\min}$ | HiGHS MILP (exact) |
| `MAX_RISK_REDUCTION` | $\max \sum h_i x_i \;\text{s.t.}\; \sum c_i x_i \leq B$ | HiGHS MILP (exact) |
| `MIN_NCW` | $\min \sum (c_i - C\alpha h_i)\,x_i$ | HiGHS MILP (lineaire benadering) |

`BruteForce.solve() == PyomoOptimizer.solve()` voor alle 6 testcases ✅

#### Stap 1.4 ✅ — Integratie smoke test (CLI)

```bash
python scripts/run_smoke_test.py
```

---

### Fase 2 — Backend & API

#### Stap 2.1 ✅ — FastAPI service
```
POST /scenarios        POST /trajectories
POST /optimize         GET  /results/{job_id}
GET  /docs             (Swagger UI)
```

#### Stap 2.2 ✅ — Database

**SQLite (standaard — nul installatie):**
```bash
uvicorn floodopt_api.main:app --reload
# → schrijft naar floodopt.db
```

**PostgreSQL (productie — optioneel):**
```bash
docker compose up -d postgres && python scripts/init_db.py
DATABASE_URL=postgresql://floodopt:floodopt@localhost:5432/floodopt \
  uvicorn floodopt_api.main:app --reload
```

Schema: `scenarios`, `trajectories`, `optimization_results`.
PostGIS-extensie aangemaakt bij PostgreSQL (voor geometrie stap 4.2).

#### Stap 2.3 ⏳ — Async queue (Redis + Celery)

`POST /optimize` geeft direct `job_id` terug. Status: `pending → running → done`.
SQLite → PostgreSQL switch verplicht bij meerdere Celery-workers (concurrent writes).

---

### Fase 3 — Uitbreidingen rekenkernel ⏳

| Stap | Inhoud | Wat blijft ongewijzigd |
|---|---|---|
| 3.1 | Probabilistische sterkte: FORM / Monte Carlo | Optimizer, Risk Layer |
| 3.2 | Lengte-effecten & correlaties | Optimizer, Physics Layer |
| 3.3 | Rivierverruiming & hydraulische interacties | Optimizer, Risk Layer |

Nieuwe implementaties achter bestaande Protocols — optimizer hoeft niet aangepast te worden.

---

### Fase 4 — Frontend ⏳

**React + Vite + Leaflet**

#### Stap 4.1 — Frontend setup
React + Vite, Leaflet voor interactieve kaarten, TanStack Query voor API-calls.

#### Stap 4.2 — Kaartviewer (GeoPandas + Leaflet)

Dijkvak-geometrie (WKT in SQLite) → GeoPandas → GeoJSON → Leaflet:

```
API endpoint:  GET /trajectories/{id}/geojson
Server:        GeoPandas leest WKT, geeft GeoJSON terug
Frontend:      Leaflet rendert dijkvakken op kaart
               Kleurgradiënt op basis van P(t) of NCW
```

#### Stap 4.3 — Scenario-editor & resultaten-dashboard

Maatregel-editor, scenario-selectie, NCW-grafieken, vergelijking BruteForce vs Pyomo.

---

## Licentie

Open source — licentie volgt.
