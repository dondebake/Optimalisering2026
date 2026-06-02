# Development Log

## Architectuurkeuzes en context

### Actuele technologiekeuzes

| Component | Keuze | Reden |
|---|---|---|
| Database | **SQLite** (standaard, ingebouwd) | Nul installatie; geen Docker vereist |
| DB upgrade pad | PostgreSQL optioneel via `DATABASE_URL` | `init_schema()` werkt met beide |
| Geometrie | GeoJSON als JSON-kolom in SQLite | Geen PostGIS/GeoPandas nodig voor MVP |
| Frontend kaarten | **Leaflet** (React + Vite) | Leest GeoJSON direct van de API |
| Optimizer | Pyomo + HiGHS (MILP) | Open-source, exact voor MIN_COST/MAX_RR |
| Queue | Redis + Celery | Async optimalisaties zonder HTTP-timeout |
| Grafiek | Recharts | P(t)-tijdreeks in de browser |

### Implementatiestatus 2011-referentiedata

#### Geïmplementeerd

| Data | Tabel/View | Gebruik | Status |
|---|---|---|---|
| Dijkringnamen + normen | `v_dijkringen_floodopt` | Validatie-dashboard dropdown | ✅ |
| P₀, α, η (basisscenario) | `v_trajecten_floodopt` (klimaat_id=1) | Physics, kaartkleur, formulier pre-fill | ✅ |
| Kostenfunctie λ, C, b | `v_kostenfunctie_floodopt` | Kandidaatmaatregelen bij pre-fill | ✅ |
| Geometrie dijkringdelen | `dijkringdelen.shp` (RD New → WGS84) | Kaart — gekleurde lijnen op P₀ | ✅ |

#### Nog niet geïmplementeerd (prioriteit)

| Prioriteit | Data | Tabel | Probleem |
|---|---|---|---|
| 🔴 | Schadeparameters (Zeta, Nu, Psi, Inwoners) | `v_schade_floodopt` | V₀ hardcoded €1 mrd — NCW incorrect |
| 🔴 | Economische groei γ per dijkringdeel | `EconomischScenarioData` | γ hardcoded 2 % — NCW incorrect |
| 🟡 | Klimaatscenario-keuze in UI | `v_trajecten_floodopt` (id 1–18) | Gebruiker voert η handmatig in |
| 🟡 | Omega (onderhoudsfactor) in NCW | `v_kostenfunctie_floodopt` | Jaarlijkse onderhoudskosten ontbreken |
| 🟢 | BeginJaar (2015 vs hardcoded 2023) | `BeginJaar` | Klein tijdsverschil |
| 🟢 | 57 grijze dijkringdelen op kaart | shapefile DIJKRINGNU met letters | Visueel, geen rekeneffect |

**Technische details ontbrekende data:**

- **V₀**: moet berekend worden als `Zeta * oppervlak * Psi + Nu * bebouwingswaarde` per dijkringdeel.
  `v_schade_floodopt` bevat Zeta (€/ha), Nu, Psi, Inwoners per dijkring/scenariocombinatie.
- **γ**: `EconomischScenarioData` geeft γ per dijkringdeel voor 6 economische groeiscenario's (0,4 %–2,6 %).
  In OptimaliseRing 2011 werd het scenario gekozen door de gebruiker; nu hardcoded 2 %.
- **Klimaatscenario's**: 18 scenario's in de DB; id=1–2 hebben η=0 (geen zeespiegelstijging),
  id=3–18 hebben η ≈ 0,002 m/jr. Gebruiker kan nu alleen handmatig η invullen in het formulier.
- **Omega**: `IC_jaarlijks = Omega × IC_investering`. Niet meegenomen in huidige NCW-integratie.
- **Grijze dijkringdelen**: DIJKRINGNU-waarden als `"34-a"`, `"13-a"` (buitendijkse compartimenten)
  matchen niet op numerieke Dijkring-ID. Oplossing: expliciete mappingtabel.

---

### Data-context: 2011 vs 2026

| 2011 (OptimaliseRing) | 2026 (NBPW / WBI2023) | Toelichting |
|---|---|---|
| Dijkring | — | Begrip vervallen als beheereenheid |
| DijkringDeel | — | Begrip vervallen |
| DijkringTraject | **Normtraject** (of dijktraject) | 1-op-1; plattere structuur |
| Terugkeertijd [jaar] | Norm [1/jaar] | Omgekeerd: 1/4000 jaar = 1/4000 per jaar |

In 2026 is elk **normtraject** een zelfstandige optimaliseringseenheid met eigen norm. De OptimaliseRing 2011 SQLite (`tests/validation/optimalise_ring_2011.sqlite`) dient uitsluitend als **testbed** voor de rekenkern en UI — geen productiedata.

---

## Fase 0 — Projectstructuur & tooling

### Stap 0.1 — Repository & package layout ✓ (2026-06-01)

- Mapstructuur aangemaakt, tooling geïnstalleerd (`pytest`, `ruff`, `mypy`, `pre-commit`)
- `floodopt-core` als editable package geïnstalleerd
- **Verificatie:** `floodopt_core` importeerbaar, `pytest` 0 tests 0 errors ✓

---

### Stap 0.2 — Data model ✓ (2026-06-01)

Pydantic v2 models in `floodopt_core/io/`:

| Model | Sleutelvelden |
|---|---|
| `Measure` | id, type, cost, year, **effect [m]**, location, dependencies |
| `Scenario` | id, climate, q_design, h_design, **eta [m/jaar]** |
| `Trajectory` | id, norm, length, **p0 [1/jaar]**, **alpha [1/m]**, **base_year**, geometry |

Eenheden gebaseerd op OptimaliseRing 2.3.2 (HKV, 2013).
**Verificatie:** 16/16 tests, mypy schoon, JSON round-trip ✓

---

## Fase 1 — MVP rekenkernel

### Stap 1.1 — Physics Layer ✓ (2026-06-01)

$$P(t) = P_0 \cdot e^{\alpha \eta t} \cdot e^{-\alpha \Delta h}$$

- `SimpleDikeOverflow` achter `PhysicsModel` Protocol
- Formule identiek aan OptimaliseRing 2.3.2
- `PhysicsResult` frozen dataclass

**Verificatie:** 23/23 tests, 3 handmatige cases rel_tol=1e-9 ✓

Zie: `docs/stap1.1_physics_formula.png`

---

### Stap 1.2 — Risk Layer ✓ (2026-06-01)

$$\text{NCW} = \sum_{s=0}^{T-1} P(s) \cdot V_0 \cdot e^{(\gamma - \delta)\,s}$$

- `SimpleRiskCalculator` achter `RiskCalculator` Protocol
- Discrete jaarlijkse sommatie; aparte discontovoeten voor schade (γ) en investering (δ)

**Verificatie:** 33/33 tests, handmatige NCW rel_tol=1e-9 ✓

Zie: `docs/stap1.2_risk_ncw.png`

---

### Stap 1.3 — Optimization Layer ✓ (2026-06-01)

| Objective | Formulering | Solver | Exact? |
|---|---|---|---|
| `MIN_COST` | $\min \sum c_i x_i \;\text{s.t.}\; \sum h_i x_i \geq h_{\min}$ | HiGHS MILP | Ja |
| `MAX_RISK_REDUCTION` | $\max \sum h_i x_i \;\text{s.t.}\; \sum c_i x_i \leq B$ | HiGHS MILP | Ja |
| `MIN_NCW` | $\min \sum (c_i - C\alpha h_i)\,x_i$ | HiGHS MILP | Lineaire benadering |

**Verificatie:** 13/13 tests, `BruteForce.solve() == PyomoOptimizer.solve()` voor alle 6 testcases ✓

Zie: `docs/stap1.3_optimization.png`

---

### Stap 1.4 — Integratie smoke test (CLI) ✓ (2026-06-01)

```bash
python scripts/run_smoke_test.py
```

Testcase: Rijnmond-achtig traject ($P_0 = 1/200$, norm $= 1/1000$, 5 kandidaatmaatregelen).

| Objective | Optimum | Waarde | BF = Pyomo |
|---|---|---|---|
| `MIN_COST` | {M02, M04} | € 1.089.224 | ✓ |
| `MAX_RISK_REDUCTION` | {M02, M03, M04} | Δh = 0.80 m | ✓ |
| `MIN_NCW` | {alle 5} | NCW = € 9.020.808 | ✓ |

Rekentijd: BruteForce 21 ms — Pyomo 248 ms (N=5, T=100).

**Referentiedata geïmporteerd:** `Database OptimaliseRing 2011_04_07.mdb` → `tests/validation/optimalise_ring_2011.sqlite` via `scripts/convert_mdb_to_sqlite.py`. 4 FloodOpt-views, 103 dijkringen, 176 trajecten, 3348 klimaatrecords.

**Verificatie:** 58/58 tests, exitcode 0 ✓ — **Fase 1 volledig afgerond.**

Zie: `docs/stap1.4_smoke_test.png`

---

## Fase 2 — Backend & API

### Stap 2.1 — FastAPI service ✓ (2026-06-01)

| Method | Endpoint | Status |
|---|---|---|
| POST | `/scenarios` | 201 |
| POST | `/trajectories` | 201 |
| POST | `/optimize` | 202 — `job_id` + `status: pending` |
| GET | `/results` | 200 — lijst alle jobs |
| GET | `/results/{job_id}` | 200/404 |
| DELETE | `/results/{job_id}` | 204/404 |
| GET | `/geo/trajectories` | 200 — GeoJSON FeatureCollection |
| GET | `/validation/dijkringen` | 200 — referentiedata |
| GET | `/validation/trajectories` | 200 — referentiedata |
| GET | `/docs` | 200 — Swagger UI |

Geen business logic in de API-laag.

**Verificatie:** 90/90 tests, Swagger UI bereikbaar ✓

Zie: `docs/stap2.1_api.png`

---

### Stap 2.2 — Database (SQLite + repository-pattern) ✓ (2026-06-01)

**Keuze:** SQLite als standaard — geen Docker, geen installatie.

```bash
uvicorn floodopt_api.main:app --reload   # schrijft naar floodopt.db
DATABASE_URL=postgresql://...            # optioneel PostgreSQL
```

| Klasse | Backend | Wanneer |
|---|---|---|
| `MemoryRepositories` | in-memory dict | Tests (FastAPI dependency override) |
| `OrmRepositories` | SQLAlchemy + SQLite/PostgreSQL | Productie |

**Schema (actueel):**

```
scenarios              (id, climate, q_design, h_design, eta)
trajectories           (id, norm, length, p0, alpha, base_year, geometry JSON)
optimization_results   (job_id, trajectory_id, scenario_id, objective, status,
                        solver, selected_measure_ids JSON, ncw-velden, p_series JSON)
```

Nieuwe kolommen worden automatisch toegevoegd via `init_schema()` (idempotent ALTER TABLE).

**Verificatie:** 90/90 tests (geen Docker vereist) ✓

Zie: `docs/stap2.2_database.png`

---

### Stap 2.3 — Async queue (Celery + Redis) ✓ (2026-06-01)

#### Waarom asynchroon?

Synchroon `POST /optimize` blokkeert de HTTP-verbinding. Bij 10–50 maatregelen per traject kan Pyomo/HiGHS minuten draaien — dan zijn er drie breekpunten: timeout, geblokkeerde thread, geen crash-herstel.

#### Architectuurkeuzes

| Keuze | Motivatie |
|---|---|
| Redis als broker | Standaard voor Celery; in-memory, snel |
| SQLite bij één worker | Concurrent schrijven niet nodig |
| PostgreSQL bij meerdere workers | SQLite ondersteunt geen concurrent writes |
| `task_always_eager=True` in tests | Tests draaien zonder Redis |

#### Status-flow

```
POST /optimize → save(pending) → send_task() → 202 + job_id
                                      ↓
                               Redis wachtrij
                                      ↓
                               Celery worker
                               update_status(running)
                               optimizer.solve()
                               compute_p_series()
                               save_result(status=done, p_series=[...])
```

#### Componenten

| Bestand | Package | Inhoud |
|---|---|---|
| `celery_app.py` | `floodopt-api` | Celery-instantie + REDIS_URL |
| `tasks.py` | `floodopt-worker` | `run_optimization`: pending→running→done/failed + p_series |
| `docker-compose.yml` | project root | Redis 7-alpine op poort 6379 |
| `start.bat` | project root | Redis + FastAPI + Worker + Frontend in 4 terminals |

#### Opstarten

```bat
start.bat
```

| Terminal | Proces | URL |
|---|---|---|
| 1 | Redis 7-alpine (Docker) | localhost:6379 |
| 2 | FastAPI + uvicorn | http://localhost:8000/docs |
| 3 | Celery worker (`--pool=solo`) | — |
| 4 | Vite dev server | http://localhost:5173 |

**Verificatie:** 90/90 tests (geen Redis vereist voor pytest) ✓

Zie: `docs/stap2.3_worker.png`, `docs/celery_flow.png`

---

## Fase 4 — Frontend

*Beslissing: Fase 4 vóór Fase 3. FORM/Monte Carlo en rivierverruiming zijn origineel onderzoekswerk; de frontend maakt het systeem eerst bruikbaar.*

### Stap 4.1 — Frontend scaffold ✓ (2026-06-02)

| Component | Keuze |
|---|---|
| Build tool | Vite + React + TypeScript |
| Styling | Tailwind CSS v4 (Vite-plugin) |
| API state | TanStack Query (polling, caching) |
| Kaarten | Leaflet + react-leaflet |
| Grafiek | Recharts |
| Dev proxy | `/api → localhost:8000` (geen CORS-issue) |

Pagina's: `Dashboard`, `OptimizeForm`, `Results`, `ValidationDashboard`.
Components: `MapView`, `PSeriesChart`, `JobList`, `StatusBadge`.

**Verificatie:** frontend bereikbaar op http://localhost:5173, formulier → 202 → polling → done ✓

---

### Stap 4.2 — Kaartviewer (GeoJSON + Leaflet) ✓ (2026-06-02)

- `Trajectory.geometry` — optioneel GeoJSON-veld, opgeslagen als JSON-kolom in SQLite
- `GET /geo/trajectories?year=2050` — GeoJSON FeatureCollection; `p_year` per feature voor kleurcodering
- `MapView.tsx` — Leaflet-kaart met OptimaliseRing-klasse-indeling (9 klassen, cyaan t/m donkergroen)
- Dashboard: "Laad Rijnmond-voorbeeld" knop POST-et een traject met Nieuwe Waterweg LineString

**Verificatie:** Rijnmond-traject zichtbaar op kaart ✓

Zie: `docs/geo_stack.png`

---

### Stap 4.3 — Job-overzicht op Dashboard ✓ (2026-06-02)

- `GET /results` — lijst alle jobs, nieuwste eerst
- `JobList.tsx` — tabel met job-id, traject, doelfunctie, resultaat (€), status-badge, link, verwijder-knop
- Polling: 2 s bij actieve jobs, 15 s bij rust

---

### Stap 4.4 — P(t)-grafiek conform OptimaliseRing ✓ (2026-06-02)

`compute_p_series()` in `floodopt_core/physics/p_series.py`:

$$P_{\text{midden}} = \sqrt{P_{\text{start}} \cdot P_{\text{eind}}}$$

Pmidden is de geometrische mean per maatregelinterval — identiek aan OptimaliseRing 2.3.2.

- Worker slaat `p_series` op na elke optimalisatie
- `PSeriesChart.tsx` (Recharts): P groen, Pmidden blauw gestippeld, Pwet zwarte referentielijn
- Results-pagina toont grafiek zodra job `done` is

**Verificatie:** P(t)-zaagrandgrafiek zichtbaar, kaart kleurt mee op P(2050) ✓

Zie: `docs/stap4_frontend.png`

---

### Stap 4.5 — Jobs verwijderen ✓ (2026-06-02)

- `DELETE /results/{job_id}` (204/404)
- `delete_result()` in Protocol + `MemoryRepositories` + `OrmRepositories`
- Verwijder-knop (✕) in `JobList` met bevestiging + cache-invalidatie

---

### Stap 4.6 — Validatie-dashboard + kaartlayout ✓ (2026-06-02)

- `floodopt_api/validation.py` — readonly lezer voor `optimalise_ring_2011.sqlite`
- `GET /validation/dijkringen` — 103 dijkringen met naam, norm, aantal trajecten
- `GET /validation/trajectories?dijkring=` — 176 trajecten (klimaat_id=1)
- `ValidationDashboard.tsx` — dropdown dijkringen, trajectentabel (P₀, α, η, norm)
- "Optimaliseer →" navigeert naar `OptimizeForm` met referentieparameters als pre-fill state — **P₀ is altijd bewerkbaar** vóór de berekening
- Amber-banner in `OptimizeForm` waarschuwt dat P₀ gecontroleerd moet worden (WBI2023-kansen staan ter discussie)

**Scope van de 2011-data:** uitsluitend testbed. Zie Fase 5 voor de 2026-data.

**Kaartlayout herontworpen (2026-06-02):**
- `App.tsx`: flex-col h-screen, main flex-1 min-h-0 overflow-auto
- Dashboard: drie kolommen — links (legenda, navigatie) | kaart (flex-1) | rechts (trajectory-details bij klik)
- `scripts/convert_dijkringdelen.py`: converteert `dijkringdelen.shp` (RD New) naar WGS84 GeoJSON,
  koppelt P₀ vanuit validatie-SQLite — 127 dijkringdelen, 70 met P₀-data
- `GET /geo/dijkringdelen`: serveert `tests/validation/dijkringdelen.geojson`
- MapView toont 2011-dijkringdelen als gekleurde lijnen; klik opent rechts paneel met details

---

### Stap 4.7 — Dijkring-niveau ⏳ (gepland)

Een normtraject-bundel (vroeger: dijkring) = verzameling trajecten met gedeelde optimalisatie.

- `DijkRing` model: id, name, `trajectory_ids: list[str]`
- `POST /dijkringen`, `GET /dijkringen`, `POST /optimize-dijkring`
- Worker dispatcht één taak per traject; `GET /dijkring-results/{id}` aggregeert
- Kaart toont alle trajecten van een bundel gekleurd per P(2050)
- MVP: trajectory-level optimizer per traject (onafhankelijk)

---

## Fase 5 — Data-actualisatie 2026 (gepland)

### Aanleiding

De 2011-database is het testbed. Productiedata vereist actualisatie op zes fronten:

- Dijkringen/dijkringdelen vervallen → normtrajecten
- Normen herzien (WBI2023)
- Klimaatscenario's: KNMI 2023 i.p.v. KNMI 2006
- Discontovoet gewijzigd (Rijksbegroting 2022: 2,25% reëel)
- Maatregelen: HWBP-projectenlijst 2024
- Schade: SSM2 i.p.v. SSM1/VNK1

### Datavelden per normtraject (2026)

| Veld | Bron | Eenheid |
|---|---|---|
| ID (trajectcode) | NBPW WFS | — |
| Geometrie (LineString) | NBPW WFS | WGS84 |
| Norm | NBPW / wettelijk | 1/jaar |
| P₀ | WBI2023-beoordelingsresultaten (NGR) | 1/jaar |
| α | HYDRA-NL of geschaald van 2011 | 1/m |
| η | KNMI 2023 per scenario | m/jaar |
| Lengte | NBPW WFS / berekend | km |

---

### Stap 5.1 — Normtrajecten laden (NBPW WFS) ⏳

- Python WFS-client (GeoPandas + owslib)
- Geometrie opslaan als GeoJSON in `trajectories.geometry`
- Norm, ID en lengte uit WFS-attributen

**Bron:** `https://geo.rijkswaterstaat.nl/services/ogc/wvp/ows/wfs`

**Output:** `scripts/load_nbpw_trajectories.py`

---

### Stap 5.2 — Overstromingskansen (P₀ en α) ⏳

- P₀ uit geaggregeerde WBI2023-beoordelingsresultaten (alle waterschappen + RWS)

**Bron:** Nationaal Georegister:
`https://www.nationaalgeoregister.nl/geonetwork/srv/dut/catalog.search#/metadata/bf447383-f2ae-47b0-b124-6c4db12ce689`

**Voorbehoud:** WBI2023-kansen staan ter discussie. In de lopende beoordelingsronde moeten kansen voor veel trajecten aanzienlijk omlaag (conservatisme eruit). FloodOpt vereist daarom dat P₀ **altijd handmatig overschrijfbaar** is — dit is geïmplementeerd via het bewerkbare `p0`-veld in `OptimizeForm` en de pre-fill vanuit het validatie-dashboard.

**Output:** CSV met P₀-referentiewaarden per trajectcode + importscript

---

### Stap 5.3 — Klimaatscenario's (η, KNMI 2023) ⏳

- Vier scenario's: W, W+, WH, WH+
- η = zeespiegelstijging per scenario per regio (kust vs. rivieren)
- Globaal: W ≈ 0,002 m/jr · W+ ≈ 0,003 · WH ≈ 0,005 · WH+ ≈ 0,008 m/jr

**Output:** `Scenario`-objecten per KNMI-scenario, koppelbaar aan elk normtraject

---

### Stap 5.4 — Maatregelen en effecten (HWBP) ⏳

- HWBP-projectenlijst (openbaar, Rijkswaterstaat)
- Per project: trajectcode, type, effect Δh [m], kostenraming, planningsjaar
- Expert-schattingen voor trajecten buiten HWBP

**Maatregel-types:** dijkversterking, ruimte voor rivier, overig

**Output:** kandidaatmaatregelen per normtraject als `Measure`-objecten

---

### Stap 5.5 — Economische parameters ⏳

| Parameter | 2011 | 2026 | Bron |
|---|---|---|---|
| Discontovoet δ | 5,5% nominaal | 2,25% reëel | Rijksbegroting 2022 |
| Economische groei γ | 2% | 1,5–2% | CPB 2024 |
| Basisschade V₀ | SSM1/VNK1 | SSM2 / WaterSchadeSchatter | Rijkswaterstaat |
| Tijdshorizon T | 100 jaar | 100 jaar | ongewijzigd |

**Output:** `RiskParams`-objecten per traject of regio

---

### Stap 5.6 — Validatie 2026 ⏳

- Vergelijk FloodOpt-resultaten met HWBP-prioritering
- Spot-check op 5–10 trajecten met bekende beoordelingsuitkomsten
- Criterium: FloodOpt-optimum moet de richting van HWBP-beslissingen bevestigen

**Let op:** stappen 5.1–5.6 vereisen toegang tot datasets die deels achter portals zitten. Dataverzameling loopt parallel aan de softwareontwikkeling.
