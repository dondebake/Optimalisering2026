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

| Laag | Verantwoordelijkheid | Output |
|---|---|---|
| Physics | Fysische modellen (hydraulica, geotechniek, rivieren) | Faalkansen, kruinhoogten |
| Risk | Schade, NCW, risico-indicatoren | Verwachte schade, risico NCW |
| Optimization | Zoekt optimale strategie via Pyomo | Optimale maatregelencombinatie |

## Tech stack

| Component | Technologie |
|---|---|
| Rekenkernel | Python (`floodopt-core`) |
| Backend API | FastAPI |
| Database | PostgreSQL + PostGIS |
| Queue | Redis + Celery |
| Optimizer | Pyomo + HiGHS / CBC / Ipopt |
| Frontend | React + Vite + Leaflet / MapLibre |

## Projectstructuur

```
floodopt/
├── floodopt-core/       # rekenkernel — geen FastAPI, pure Python
│   ├── physics/         # hydraulica, geotechniek, riviermodellen
│   ├── risk/            # faalkansen, schade, NCW
│   ├── optimization/    # Pyomo-modellen, solver-abstractie
│   ├── io/              # datamodellen (Measure, Scenario, Trajectory)
│   └── utils/
├── floodopt-api/        # FastAPI backend
├── floodopt-worker/     # Celery workers voor zware berekeningen
├── floodopt-frontend/   # React + Vite
└── tests/
    ├── unit/
    ├── integration/
    └── validation/      # brute-force vs. optimizer vergelijkingen
```

## Ontwikkelprincipes

- **Strikte laagscheiding** — optimizer roept altijd de rekenkernel aan
- **Modulaire interfaces** — elke laag implementeert een Protocol; implementaties zijn vervangbaar
- **Dubbele validatie** — elke optimalisatie wordt geverifieerd met een brute-force referentie
- **Stapsgewijze uitbreiding** — MVP eerst, complexiteit per fase toevoegen

## Fasering

| Fase | Inhoud |
|---|---|
| 1 (MVP) | Traject-niveau, eenvoudige schade/kosten, enkele maatregelen |
| 2 | Probabilistische sterkte (FORM / Monte Carlo), klimaatscenario's |
| 3 | Lengte-effecten, correlaties, robuuste optimalisatie |
| 4 | Rivierverruiming, hydraulische interacties, trajectoverschrijdende effecten |

## Validatiestrategie

Elke optimalisatie wordt gevalideerd door de uitkomst te vergelijken met een brute-force berekening over dezelfde maatregelencombinaties. Bij afwijking stopt de ontwikkeling totdat de oorzaak is vastgesteld.

| Niveau | Methode |
|---|---|
| Unit | pytest per functie |
| Laag | Protocol-conformiteit via mypy |
| Integratie | brute-force == optimizer voor alle testcases |
| Regressie | CI runt validatie bij elke commit |

---

## Stappenplan

Volgorde: `0.1 → 0.2 → 1.1 → 1.2 → 1.3 → 1.4 → 2.1 → 2.2 → 2.3 → 3.x → 4`

Na elke stap: verificatie en validatie voor er verder wordt gegaan.

### Fase 0 — Projectstructuur & Tooling

#### Stap 0.1 — Repository & package layout
- Mapstructuur aanmaken (zie Projectstructuur)
- Tooling: `uv`/`poetry`, `pytest`, `ruff`, `mypy`, `pre-commit`

**Verificatie:**
- `floodopt-core` importeerbaar als package
- Alle submodules aanwezig (`__init__.py`)
- `pytest` runt zonder fouten (0 tests, 0 errors)

#### Stap 0.2 — Data model
Generieke datastructuren als Pydantic models in `floodopt-core/io/`:
- `Measure` — id, type, cost, year, effect, location, dependencies
- `Scenario` — id, climate, q_design, h_design
- `Trajectory` — id, norm, length, measures

**Verificatie:**
- Unit tests: aanmaken en serialiseren
- `mypy` geeft geen errors
- JSON round-trip (serialize → deserialize → gelijk)

---

### Fase 1 — MVP rekenkernel (traject-niveau)

#### Stap 1.1 — Physics Layer
- Abstracte `PhysicsModel` Protocol interface
- MVP-implementatie: `SimpleDikeOverflow` (analytisch, kruinhoogte vs. waterstand)
- Output: `PhysicsResult` (pf_overflow, h_crest)

**Verificatie:**
- Handmatige berekening voor 3 testcases klopt
- Protocol-conformiteit via mypy
- Geen optimizer-logica in physics

#### Stap 1.2 — Risk Layer
- Abstracte `RiskCalculator` Protocol interface
- MVP: `pf × schade_per_overstroming`, NCW-berekening
- Output: `RiskResult` (expected_damage, risk_reduction, ncw)

**Verificatie:**
- Handmatige NCW-berekening voor referentiecasus
- NCW neemt af naarmate meer maatregelen worden genomen

#### Stap 1.3 — Optimization Layer
Beide implementaties achter hetzelfde `OptimizationStrategy` Protocol:

1. `BruteForceOptimizer` — itereert alle combinaties (referentie)
2. `PyomoOptimizer` — algebraïsch model met HiGHS solver

Ondersteunde objective functions: `min_ncw`, `min_cost`, `max_risk_reduction`

**Verificatie (kritiek):**
- Voor alle testcases: `BruteForce.solve() == PyomoOptimizer.solve()`
- Bij afwijking: bouwen stopt, oorzaak wordt geanalyseerd
- Optimizer bevat zelf geen fysische formules

#### Stap 1.4 — Integratie smoke test (CLI)
End-to-end run zonder API of database: traject → optimizer → resultaat via CLI-script.

**Verificatie:**
- Runt zonder errors
- Resultaat identiek aan brute-force
- Rekentijd brute-force vs. Pyomo gelogd

---

### Fase 2 — Backend & API

#### Stap 2.1 — FastAPI service
Endpoints MVP:
```
POST /scenarios
POST /trajectories
POST /optimize
GET  /results/{job_id}
```

**Verificatie:**
- Swagger UI beschikbaar op `/docs`
- `POST /optimize` geeft zelfde resultaat als CLI (stap 1.4)
- Geen business logic in API-laag

#### Stap 2.2 — Database (PostgreSQL + PostGIS)
Schema: trajecten, maatregelen, scenarios, resultaten.

**Verificatie:**
- Round-trip: scenario opslaan → ophalen → identiek
- Optimalisatieresultaten persistent opgeslagen

#### Stap 2.3 — Async queue (Redis + Celery)
**Verificatie:**
- `POST /optimize` geeft direct `job_id` terug
- Status: `pending` → `running` → `done`
- Worker crasht niet bij ongeldige input

---

### Fase 3 — Uitbreidingen rekenkernel

| Stap | Inhoud | Wat blijft ongewijzigd |
|---|---|---|
| 3.1 | Probabilistische sterkte: FORM / Monte Carlo | Optimizer, Risk Layer |
| 3.2 | Lengte-effecten & correlaties | Optimizer, Physics Layer |
| 3.3 | Rivierverruiming & hydraulische interacties | Optimizer, Risk Layer |

Nieuwe implementaties achter bestaande Protocols — optimizer hoeft niet aangepast te worden.

---

### Fase 4 — Frontend

React + Vite, Leaflet/MapLibre voor kaarten. Start na werkende API (stap 2.1).

Componenten: kaartviewer, scenario-editor, maatregel-editor, resultaten-dashboards.

---

## Licentie

Open source — licentie volgt.
