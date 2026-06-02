# FloodOpt

Open-source platform voor optimalisatie van dijkversterking en rivierverruiming.

## Doel

FloodOpt optimaliseert waterveiligheidsstrategieën op traject-, dijkring- en systeemniveau door dijkversterking en rivierverruiming te combineren. Het platform ondersteunt probabilistische risico's, meerdere klimaatscenario's, meerdere faalmechanismen en robuuste optimalisatie.

Toekomstige toepassingen: HWBP-analyses, MKBA, asset management, klimaatstresstesten voor financiële instellingen, portefeuille-optimalisatie.

## Architectuur

```
┌──────────────────────────────────────────────────────────┐
│  Frontend  React + Vite + Tailwind + Leaflet             │
│  Dashboard  |  OptimizeForm  |  Results + P(t)-grafiek   │
│  Kaart: GeoJSON van API, gekleurd per P-klasse (2050)    │
└────────────────────┬─────────────────────────────────────┘
                     │ HTTP / GeoJSON
┌────────────────────▼─────────────────────────────────────┐
│  FastAPI  (dunne HTTP-schil, geen business logic)        │
│  POST /optimize        → 202 + job_id  (async)           │
│  GET  /results         → lijst alle jobs                 │
│  GET  /results/{id}    → status polling                  │
│  GET  /geo/trajectories→ GeoJSON FeatureCollection       │
└──────┬───────────────────────┬────────────────────────────┘
       │ SQLAlchemy             │ task.delay()
┌──────▼──────────┐   ┌────────▼───────────────────────────┐
│  SQLite (dev)   │   │  Redis  (message broker)            │
│  PostgreSQL     │   │  taakwachtrij + job-status          │
│  (prod, opt.)   │   └────────┬───────────────────────────┘
└─────────────────┘            │ Celery worker
                      ┌────────▼───────────────────────────┐
                      │  floodopt-worker                    │
                      │  run_optimization task              │
                      │  pending → running → done           │
                      │  + compute_p_series (P(t), Pmidden) │
                      └────────┬───────────────────────────┘
                               │
┌──────────────────────────────▼───────────────────────────┐
│  floodopt-core  (strikte laagscheiding)                  │
│                                                          │
│  Optimization Layer  min Σcᵢxᵢ  s.t. Σhᵢxᵢ ≥ h_min     │
│         ↓  roept aan                                     │
│  Risk Layer      NCW = Σ P(s)·V₀·e^((γ−δ)s)             │
│         ↓  roept aan                                     │
│  Physics Layer   P(t) = P₀·e^(αηt)·e^(−αΔh)             │
│         ↓  roept aan                                     │
│  P-tijdreeks     P(t) + Pmidden per maatregelinterval    │
└──────────────────────────────────────────────────────────┘
```

Zie `docs/architecture.png` voor het volledige architectuurdiagram.

## Tech stack

| Component | Technologie | Reden |
|---|---|---|
| Rekenkernel | Python (`floodopt-core`) | Pure Python, geen framework-lock-in |
| Optimizer | Pyomo 6 + HiGHS (MILP) | Open-source, schaalt naar grote N |
| Backend API | FastAPI | Snel, automatische Swagger UI |
| Database | **SQLite** (dev) → PostgreSQL optioneel (prod) | Nul installatie voor development |
| Geometrie | GeoJSON in SQLite (JSON-kolom) | Geen PostGIS nodig voor MVP |
| Kaarten (frontend) | **Leaflet** + react-leaflet | Leest GeoJSON direct van API |
| Queue | Redis + Celery | Async optimalisaties — POST /optimize retourneert direct job_id |
| Grafiek | Recharts | P(t)-tijdreeks + Pmidden in de browser |
| Documentatie | matplotlib (LaTeX-formules) | Reproduceerbaar via `generate_docs.py` |

### Async queue: waarom Celery + Redis in plaats van synchroon

Het synchrone MVP (`POST /optimize` blokkeert tot resultaat) breekt op drie plekken zodra problemen groeien:

| Probleem | Gevolg |
|---|---|
| HTTP-verbinding time-out (30–60 sec) | Client krijgt fout; resultaat gaat verloren |
| FastAPI-thread geblokkeerd tijdens berekening | Andere requests wachten |
| Geen herstelbaarheid bij crash | Hele job verloren |

Met de async queue:

| | Synchroon (MVP) | Async (Celery + Redis) |
|---|---|---|
| `POST /optimize` retourneert | Resultaat (na berekening) | `job_id` + `status: pending` (<5 ms) |
| Berekening tijdsduur | Blokkeert HTTP-verbinding | Ontkoppeld van API |
| Schaling workers | ✗ (één API-thread) | ✓ meerdere Celery-workers |
| Crash-herstel | ✗ | ✓ Celery retry-mechanisme |
| Tests zonder Redis | ✓ | ✓ `task_always_eager=True` |

**SQLite → PostgreSQL bij meerdere workers:** SQLite ondersteunt geen concurrent schrijven vanuit meerdere processen. Bij één worker (dev) werkt SQLite prima; schakel over naar PostgreSQL zodra je meerdere Celery-workers draait.

## Projectstructuur

```
floodopt/
├── floodopt-core/           # rekenkernel — geen FastAPI, pure Python
│   └── floodopt_core/
│       ├── io/              # Pydantic datamodellen (Measure, Scenario, Trajectory)
│       ├── physics/         # SimpleDikeOverflow + compute_p_series
│       ├── risk/            # RiskCalculator Protocol + SimpleRiskCalculator
│       ├── optimization/    # OptimizationStrategy Protocol + BruteForce + Pyomo
│       └── utils/
├── floodopt-api/            # FastAPI backend
│   └── floodopt_api/
│       ├── main.py          # endpoints + DI
│       ├── database.py      # SQLAlchemy ORM + migraties (SQLite / PostgreSQL)
│       └── repositories.py  # Memory + ORM implementaties
├── floodopt-worker/         # Celery worker
│   └── floodopt_worker/
│       └── tasks.py         # run_optimization + compute_p_series
├── floodopt-frontend/       # React + Vite + Leaflet
│   └── src/
│       ├── pages/           # Dashboard, OptimizeForm, Results
│       ├── components/      # MapView, PSeriesChart, JobList, StatusBadge
│       └── api/             # Typed fetch-client
├── tests/
│   ├── unit/                # 46 tests — alle lagen
│   ├── integration/         # 44 tests — CLI, API, database, worker
│   └── validation/          # optimalise_ring_2011.sqlite (referentiedata)
├── docs/                    # PNG-diagrammen (via generate_docs.py) + SVG bronnen
├── scripts/                 # generate_docs.py, run_smoke_test.py, init_db.py
└── start.bat                # Start Redis + API + Worker + Frontend in 4 terminals
```

## Opstarten

```bat
start.bat
```

Opent vier terminals automatisch:

| Terminal | Proces | URL |
|---|---|---|
| 1 | Redis 7-alpine (Docker) | localhost:6379 |
| 2 | FastAPI + uvicorn | http://localhost:8000/docs |
| 3 | Celery worker (`--pool=solo`) | — |
| 4 | Vite dev server | http://localhost:5173 |

## Ontwikkelprincipes

- **Strikte laagscheiding** — optimizer bevat nooit fysische formules; API bevat geen business logic
- **Modulaire interfaces** — elke laag implementeert een Protocol; implementaties zijn vervangbaar
- **Dubbele validatie** — elke optimalisatie geverifieerd met brute-force referentie
- **Installatie-arm** — SQLite + in-memory tests; geen Docker vereist voor development
- **Stapsgewijze uitbreiding** — MVP eerst, complexiteit pas toevoegen als nodig

## Documentatie

Alle diagrammen worden gegenereerd met `python scripts/generate_docs.py`:

| Diagram | Bestand |
|---|---|
| Volledige architectuur (rekenkernel + API + geo + frontend) | `docs/architecture.png` |
| Celery + Redis taak-flow (visueel) | `docs/celery_flow.png` |
| Physics Layer — $P(t)$ formule + curves | `docs/stap1.1_physics_formula.png` |
| Risk Layer — NCW berekening | `docs/stap1.2_risk_ncw.png` |
| Optimization Layer — BruteForce vs Pyomo | `docs/stap1.3_optimization.png` |
| Smoke test — end-to-end verificatie | `docs/stap1.4_smoke_test.png` |
| FastAPI service — endpoints + flow | `docs/stap2.1_api.png` |
| Database — SQLite/PostgreSQL + repository-pattern | `docs/stap2.2_database.png` |
| Async queue — Celery + Redis statustabel | `docs/stap2.3_worker.png` |
| Geo-stack — GeoJSON in SQLite + Leaflet kleurcodering | `docs/geo_stack.png` |
| Frontend stap 4.1–4.4 — componenten en features | `docs/stap4_frontend.png` |
| Database mapping MDB → FloodOpt | `docs/database_mapping.png` |

## Validatiestrategie

Referentiedataset: `tests/validation/optimalise_ring_2011.sqlite` — afgeleid van OptimaliseRing v2.3.2 (HKV, 2013), 103 dijkringen, 176 trajecten.

| Niveau | Methode | Status |
|---|---|---|
| Unit | pytest per functie | ✅ 46/46 |
| Integratie CLI | smoke test end-to-end | ✅ 12/12 |
| Integratie API | TestClient alle endpoints | ✅ 19/19 |
| Integratie DB | SQLite round-trip | ✅ 6/6 |
| Integratie Worker | Celery taken direct aangeroepen | ✅ 7/7 |
| Kritiek | BruteForce == Pyomo | ✅ 6/6 testcases |
| Totaal | | ✅ 90/90 |
| Regressie | CI bij elke commit | ⏳ Gepland |

## Fasering

| Fase | Status | Inhoud |
|---|---|---|
| 0 — Tooling | ✅ Klaar | Projectstructuur, packaging, pre-commit |
| 1 — MVP rekenkernel | ✅ Klaar | Physics, Risk, Optimization, CLI smoke test |
| 2 — Backend & API | ✅ Klaar | FastAPI, SQLite, Celery + Redis |
| 3 — Uitbreidingen rekenkernel | ⏳ Gepland | FORM/Monte Carlo, lengte-effecten, rivierverruiming |
| 4 — Frontend | 🚧 In uitvoering | 4.1–4.5 ✅ · 4.6 dijkring-niveau |
| D — Data-actualisatie 2026 | ⏳ Gepland | NBPW WFS, WBI2023, KNMI 2023, HWBP, SSM2 |

---

## Stappenplan

### Fase 0 — Projectstructuur & Tooling

#### Stap 0.1 ✅ — Repository & package layout
Mapstructuur, tooling (`pytest`, `ruff`, `mypy`, `pre-commit`), editable install.

#### Stap 0.2 ✅ — Data model
Pydantic v2 models: `Measure` (effect [m]), `Scenario` (eta [m/j]), `Trajectory` (p0, alpha, base_year, geometry).

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
POST /scenarios          POST /trajectories
POST /optimize           GET  /results
GET  /results/{job_id}   GET  /geo/trajectories
GET  /docs               (Swagger UI)
```

#### Stap 2.2 ✅ — Database

**SQLite (standaard — nul installatie):**
```bash
uvicorn floodopt_api.main:app --reload
# → schrijft naar floodopt.db
```

**PostgreSQL (productie — optioneel):**
```bash
DATABASE_URL=postgresql://user:pw@host/db uvicorn floodopt_api.main:app --reload
python scripts/init_db.py
```

Schema: `scenarios`, `trajectories` (+ geometry JSON), `optimization_results` (+ p_series JSON).

#### Stap 2.3 ✅ — Async queue (Redis + Celery)

`POST /optimize` geeft direct `job_id` terug (202). Status: `pending → running → done`.

```bat
start.bat   ← start Redis + API + Worker + Frontend
```

---

### Fase 3 — Uitbreidingen rekenkernel ⏳

| Stap | Inhoud | Wat blijft ongewijzigd |
|---|---|---|
| 3.1 | Probabilistische sterkte: FORM / Monte Carlo | Optimizer, Risk Layer |
| 3.2 | Lengte-effecten & correlaties | Optimizer, Physics Layer |
| 3.3 | Rivierverruiming & hydraulische interacties | Optimizer, Risk Layer |

Nieuwe implementaties achter bestaande Protocols — optimizer hoeft niet aangepast te worden.

---

### Fase 4 — Frontend

**React + Vite + Tailwind CSS + Leaflet + Recharts**

#### Stap 4.1 ✅ — Frontend scaffold
React + Vite + TypeScript + Tailwind CSS. TanStack Query voor API-calls en polling.
OptimizeForm (traject, scenario, maatregelen) → `POST /optimize` → Results-pagina met polling.

#### Stap 4.2 ✅ — Kaartviewer

`Trajectory.geometry` (optioneel GeoJSON) opgeslagen in SQLite.
`GET /geo/trajectories` geeft FeatureCollection terug.
Leaflet-kaart op Dashboard; "Laad Rijnmond-voorbeeld" knop POST-et een traject met LineString.

#### Stap 4.3 ✅ — Job-overzicht

`GET /results` lijst-endpoint. `JobList` component op Dashboard met automatische polling
(2 s bij actieve jobs, 15 s bij rust).

#### Stap 4.4 ✅ — P(t)-grafiek conform OptimaliseRing

`compute_p_series()` in `floodopt_core/physics/p_series.py` — berekent P(t) en Pmidden
per maatregelinterval (Pmidden = √(P_start · P_end), conform OptimaliseRing 2.3.2).

Worker slaat `p_series` op na elke optimalisatie.
`PSeriesChart` (Recharts) toont P (groen), Pmidden (blauw gestippeld) en Pwet (zwarte lijn).
`GET /geo/trajectories?year=2050` voegt `p_year` toe; kaart kleurt trajecten per
OptimaliseRing-klasse-indeling (< 1/113.000 t/m > 1/800).

#### Stap 4.x ⏳ — Jobs verwijderen

- `DELETE /results/{job_id}` endpoint
- `delete_result()` in Protocol + beide implementaties
- Verwijder-knop in `JobList` (met bevestiging), cache-invalidatie

#### Stap 4.5 ⏳ — Validatie-dashboard

Laad de 176 trajecten uit `tests/validation/optimalise_ring_2011.sqlite`, run FloodOpt erop en vergelijk de uitkomsten met de OptimaliseRing-referentieresultaten.

- `GET /validation/trajectories` — readonly verbinding met referentie-SQLite
- `ValidationDashboard` pagina — tabel van alle 176 trajecten (dijkring, norm, p0, α, lengte)
- "Optimaliseer" knop per traject → POST /optimize → Results-pagina
- Vergelijkingstabel: FloodOpt vs OptimaliseRing (afhankelijk van beschikbare referentieresultaten in SQLite)

---

### Fase D — Data-actualisatie 2011 → 2026

De 2011-data (OptimaliseRing SQLite) is uitsluitend een testbed. Productiedata volgt via onderstaande stappen.

**Terminologiewijziging:** dijkringen/dijkringdelen zijn vervallen. In 2026 zijn er **normtrajecten** (ook: dijktrajecten) — elk met eigen norm, geometrie en parameters. Dit vereenvoudigt het FloodOpt-datamodel.

#### Stap D1 ⏳ — Normtrajecten laden (NBPW WFS)
Geometrie, ID's en normen via WFS:
```
https://geo.rijkswaterstaat.nl/services/ogc/wvp/ows/wfs
```
Script: `scripts/load_nbpw_trajectories.py` (GeoPandas + owslib)

#### Stap D2 ⏳ — P₀ en α kalibreren
P₀ uit geaggregeerde WBI2023-beoordelingsresultaten (Nationaal Georegister, alle waterschappen + RWS).
α uit HYDRA-NL of geschaald van 2011-waarden.

> **Let op:** WBI2023-kansen staan ter discussie (conservatisme). FloodOpt vereist daarom altijd de mogelijkheid om P₀ per traject handmatig te overschrijven.

#### Stap D3 ⏳ — KNMI 2023 klimaatscenario's (η)
Vier scenario's (W / W+ / WH / WH+). Zeespiegelstijging per regio per scenario.

#### Stap D4 ⏳ — Maatregelen (HWBP)
HWBP-projectenlijst: maatregel-type, effect Δh [m], kostenraming, planningsjaar per normtraject.

#### Stap D5 ⏳ — Economische parameters
Discontovoet 2,25% reëel (Rijksbegroting 2022). Schade V₀ uit SSM2/WaterSchadeSchatter. Economische groei γ uit CPB 2024.

#### Stap D6 ⏳ — Validatie 2026
Vergelijk FloodOpt-resultaten met HWBP-prioritering en WBI2023-beoordelingsresultaten op 5–10 trajecten.

---

#### Stap 4.6 ⏳ — Dijkring-niveau

Een dijkring is een verzameling trajecten. Optimaliseer alle trajecten van een dijkring en toon het gecombineerde resultaat op de kaart.

- `DijkRing` model: id, name, `trajectory_ids: list[str]`
- `POST /dijkringen`, `GET /dijkringen`, `POST /optimize-dijkring`
- Worker dispatcht één taak per traject; `GET /dijkring-results/{id}` aggregeert status
- `DijkRingForm` + `DijkRingResults` pagina's
- Kaart toont alle trajecten van een dijkring gekleurd per P(2050)-klasse
- MVP: trajectory-level optimizer per traject (onafhankelijk); gezamenlijke MILP is Fase 3

---

## Licentie

Open source — licentie volgt.
