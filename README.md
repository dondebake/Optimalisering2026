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

## Licentie

Open source — licentie volgt.
