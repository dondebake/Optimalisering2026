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

---

## Databasedocumentatie — OptimaliseRing 2011 SQLite

Bestand: `tests/validation/optimalise_ring_2011.sqlite`
Afgeleid van: `Database OptimaliseRing 2011_04_07.mdb` (HKV, 2013)
Conversie: `scripts/convert_mdb_to_sqlite.py` (eenheidsconversie α en η)

---

### 1. Dijkringen — `v_dijkringen_floodopt`

**103 dijkringen** met naam en wettelijke norm.

| Kolom | Type | Eenheid | Beschrijving |
|---|---|---|---|
| `Dijkring` | tekst | — | Dijkring-ID (numeriek als string) |
| `Naam` | tekst | — | Naam dijkring |
| `norm_per_jaar` | getal | 1/jaar | Wettelijke overstromingsnorm |
| `Terugkeertijd` | getal | jaar | $= 1/\text{norm}$ |

**Unieke normen in de dataset:**

| norm [1/jaar] | Terugkeertijd [jaar] |
|---|---|
| 1/10.000 | 10.000 |
| 1/4.000 | 4.000 |
| 1/2.000 | 2.000 |
| 1/1.250 | 1.250 |
| 1/500 | 500 |
| 1/250 | 250 |

---

### 2. Faalkansmodel — `v_trajecten_floodopt`

**176 trajecten** (klimaat_id=1), elke combinatie van Dijkring / DijkringDeel / DijkringTraject.
Dezelfde 176 trajecten zijn herhaald voor elk van de **18 klimaatscenario's** (totaal 3.168 rijen).

#### Formule

$$P(t) = P_0 \cdot e^{\alpha \eta t} \cdot e^{-\alpha \Delta h}$$

waarbij $t$ het aantal jaren na het basisjaar is en $\Delta h$ de cumulatieve kruinhoogteverhoging [m].

#### Kolommen

| Kolom | Eenheid | Waardebereik | Beschrijving |
|---|---|---|---|
| `Dijkring` | — | 1 – 60+ | Dijkring-ID |
| `DijkringDeel` | — | 1 – 4 | Deel van de dijkring |
| `DijkringTraject` | — | 1 – 7 | Traject binnen een deel |
| `Naam` | — | — | Naam van het traject |
| `p0_per_jaar` ($P_0$) | 1/jaar | $10^{-4}$ – $8 \times 10^{-4}$ | Faalkans in basisjaar |
| `alpha_per_m` ($\alpha$) | 1/m | 0,76 – 11,41 | Schaalparameter faalkansmodel |
| `eta_m_per_jaar` ($\eta$) | m/jaar | 0 – 0,00201 | Klimaatstijging waterstand |
| `klimaat_id` | — | 1 – 18 | Klimaatscenario (zie §3) |
| `h0_m` | m | 0 (alle rijen) | Initiële kruinhoogteoffset — niet ingevuld |
| `Factor` | — | 1 (alle rijen) | Schalingsfactor — niet ingevuld |

**BeginJaar:** $t_0 = 2015$ (zowel planningsjaar als rekenjaar)

---

### 3. Klimaatscenario's — `Klimaat_AftoppenAfvoer`

18 scenario's: combinaties van KNMI 2006-klimaatklasse en aftopping van rivierafvoer.
Klimaat_id 1–2 (basisscenario's) geven η=0; id 3–18 geven η ≈ 0,002 m/jr.

| ID | Naam | η |
|---|---|---|
| 1 | Zonder aftoppen | 0 |
| 2 | Met aftoppen | 0 |
| 3 | Gemiddeld zonder aftoppen | 0,00201 m/jr |
| 4 | Gemiddeld met hoog aftoppen | 0,00201 m/jr |
| 5 | Gemiddeld met gemiddeld aftoppen | 0,00201 m/jr |
| 6 | Gemiddeld met laag aftoppen | 0,00201 m/jr |
| 7 | Gemiddeld+ zonder aftoppen | 0,00201 m/jr |
| 8–10 | Gemiddeld+ met hoog/gemiddeld/laag aftoppen | 0,00201 m/jr |
| 11 | Warm zonder aftoppen | 0,00201 m/jr |
| 12–14 | Warm met hoog/gemiddeld/laag aftoppen | 0,00201 m/jr |
| 15 | Warm+ zonder aftoppen | 0,00201 m/jr |
| 16–18 | Warm+ met hoog/gemiddeld/laag aftoppen | 0,00201 m/jr |

FloodOpt gebruikt nu uitsluitend klimaat_id=1 (basisscenario, η=0). Uitbreiding naar meerdere scenario's is gepland (stap 5.3).

---

### 4. Kostenfunctie — `v_kostenfunctie_floodopt`

**183 rijen** (één per Dijkring/DijkringDeel/DijkringTraject combinatie).
Type: Exponentieel (ID 1) of Kwadratisch (ID 2); parametertype: Zonder overhoogte (ID 1) of Met overhoogte (ID 2).

#### Formule (exponentieel)

$$IC(\Delta h) = C \cdot e^{\lambda \Delta h} \cdot \Delta h^{\,b} \quad [\text{M EUR}]$$

Jaarlijkse onderhoudskosten: $\,IC_{\text{onderhoud}} = \Omega \cdot IC(\Delta h)$ per jaar.

#### Kolommen

| Kolom | Symbool | Eenheid | Bereik | Beschrijving |
|---|---|---|---|---|
| `lambda_exp_per_m` | $\lambda$ | 1/m | 0 – 0,63 | Exponentiële kostenstijging per meter verhoging |
| `C_exp` | $C$ | M EUR | 0 – 218 | Kostenconstante |
| `b_exp` | $b$ | — | 0 – 3,12 | Machtsverheffing (curve-vorm) |
| `Omega` | $\Omega$ | — | 0,002 (constant) | Jaarlijkse onderhoudfractie van investering |

Wanneer $\lambda = 0$: vereenvoudigt naar machtsformule $IC = C \cdot \Delta h^{\,b}$.

---

### 5. Schadeparameters — `v_schade_floodopt`

**372 rijen** (103 dijkringen × ≤3 SchadeFunctieIds × DijkringDelen).

| SchadeFunctieId | Naam |
|---|---|
| 1 | Onafhankelijk van waterstand |
| 2 | Afhankelijk van waterstand |
| 3 | Zeta en Nu gelijk aan nul (referentie) |

| Kolom | Symbool | Eenheid | Bereik | Beschrijving |
|---|---|---|---|---|
| `Zeta` | $\zeta$ | — | 0 – 0,00432 | Directe schadedichtheid |
| `Nu` | $\nu$ | — | 0 (alle rijen) | Schade-aandeel gebouwen — niet ingevuld |
| `Psi` | $\psi$ | — | 0,01 (constant) | Indirecte schadeverhouding |
| `Inwoners` | — | personen | 0 – 1.200.842 | Bevolking dijkring |
| `Slachtoffers` | — | personen/overstroming | 0 – 250 | Verwacht aantal slachtoffers |
| `Getroffenen` | — | — | 0 (alle rijen) | Getroffen personen — niet ingevuld |

**Opmerking:** Zeta=0 voor grote dijkringen (waaronder Rijnmond). V₀ is daarom niet te berekenen uit Zeta+Inwoners. Gebruik in plaats daarvan `ScenarioVoorHoeveelheidSchadeData` (§6).

---

### 6. Voorberekende schadewaarden — `ScenarioVoorHoeveelheidSchadeData`

**Pre-computed V₀** per dijkring/deel, schade-scenario en schadefunctie-type.
Dit is de primaire bron voor $V_0$ in de NCW-berekening.

| ScenarioId | Naam | Schade min [M EUR] | Schade max [M EUR] | Schade gem. [M EUR] |
|---|---|---|---|---|
| 1 | Laag | 7 | 14.056 | 1.833 |
| 2 | Verwacht | 9 | 27.984 | 4.607 |
| 3 | Hoog | 17 | 30.400 | 7.408 |

Voorbeeld dijkring 14 (Rijnmond), deel 1:

| Scenario | $V_0$ [M EUR] | $V_0$ [EUR] |
|---|---|---|
| Laag | 128 | € 128.000.000 |
| Verwacht | 8.564 | € 8.564.000.000 |
| Hoog | 17.034 | € 17.034.000.000 |

FloodOpt gebruikt `SchadeFunctieId=1` (waterstandsonafhankelijk) als standaard.

---

### 7. Economische groei — `EconomischScenarioData`

**868 rijen** (103 dijkringen × ≤ dijkringdelen × 7 scenario's).
Gebaseerd op CPB WLO-scenario's (Welvaart en Leefomgeving, 2006).

#### NCW-formule

$$\text{NCW} = \sum_{s=0}^{T-1} P(s) \cdot V_0 \cdot e^{(\gamma - \delta)\,s} + \sum_{i \in S} \frac{IC(\Delta h_i)}{(1+\delta)^{t_i - t_0}}$$

waarbij:
- $\gamma$ = economische groeivoet [1/jaar] — uit `EconomischScenarioData`
- $\delta$ = discontovoet [1/jaar] — niet in de database; in 2011: 5,5 % nominaal
- $T$ = planningshorizon [jaar] — typisch 100 jaar

#### Scenario's

| ID | Naam (NL) | $\gamma$ bereik |
|---|---|---|
| 1 | Regional Communities (RC) | 0,7 – 1,0 % |
| 2 | Strong Europe (SE) | 1,0 – 1,6 % |
| 3 | Transatlantic Market (TM) | 1,0 – 1,9 % |
| 4 | Global Economy (GE) | 1,0 – 2,6 % |
| 5 | Geen | 0 – 1,0 % |
| 6 | TM minus 1,5 % | 0,4 % (constant) |
| 7 | GE minus 1,5 % | 1,1 % (constant) |

γ varieert per dijkring/deel omdat economisch actieve gebieden hogere groeiverwachtingen kennen.

---

### 8. Geometrie — `dijkringdelen.shp`

Bestand in de OptimaliseRing-broncode (`DijkringDelen/dijkringdelen.shp`).
Geconverteerd naar WGS84 GeoJSON via `scripts/convert_dijkringdelen.py`.

| Kolom | Beschrijving |
|---|---|
| `DIJKRING` | Dijkringnaam (tekst) |
| `DIJKRINGDE` | Dijkringdeelnummer |
| `DIJKRINGNU` | Dijkringnummer (numeriek als tekst, soms met letter: "13-a") |
| `NAAM_WATER` | Naam waterlichaam |
| Geometrie | LineString / MultiLineString (dijkcruinlijn) |

**127 features**, 70 met P₀-koppeling. 57 zonder koppeling (DIJKRINGNU bevat letters zoals "13-a", "34-a" voor buitendijkse compartimenten).

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

### Stap 4.7 — Dashboard herontworpen + Runs pagina + OptimizeModal ✓ (2026-06-02)

#### Dashboard

- **Volledige kaartlayout**: kaart vult het volledige centrale gebied (`flex-1`, `h-full`)
- **Links paneel** (256 px): kaartlegenda met 9 P₀-klassen conform OptimaliseRing
- **Rechts paneel** (320 px, conditioneel bij klik): trajectdetails, gefilterde runs voor het geselecteerde dijkringdeel, knoppen om nieuwe berekeningen te starten
- **Dijkringdelen op kaart**: `dijkringdelen.shp` (RD New → WGS84) gekleurd op P₀ via `GET /geo/dijkringdelen`; klik opent rechter paneel met alle beschikbare trajecten en historische runs

#### Runs pagina (`/runs`)

- Volledige tabel alle optimalisatieruns — nieuwste eerst
- Kolommen: traject, doelfunctie, solver, resultaat [M EUR], status, job-id, verwijder
- Vervangt de tabbladen-aanpak; "Optimaliseren" verdwenen als globale tab

#### OptimizeModal

Wanneer de gebruiker een traject kiest in het rechter paneel:
1. Haalt referentiedata op: `GET /validation/reference/{dijkring}/{deel}`
2. Toont scenario-keuzescherm: V₀ (Laag/Verwacht/Hoog) + γ (7 CPB-scenario's)
3. Navigeert naar `OptimizeForm` met volledige prefill (traject, scenario, kandidaatmaatregelen, risicoparameters)

#### V₀ en γ uit database ✓

- `GET /validation/reference/{dijkring}/{deel}`: retourneert alle schade- en economische scenario's
- `ScenarioVoorHoeveelheidSchadeData.Schade` [M EUR]: V₀ voor Laag/Verwacht/Hoog
- `EconomischScenarioData.Gamma`: γ voor 7 CPB WLO-scenario's (RC, SE, TM, GE, ...)
- Geen schattingen meer — alle waarden rechtstreeks uit de 2011-database

---

### Stap 4.8 — Results-pagina compleet + invoerparameters opgeslagen ✓ (2026-06-02)

#### input_payload in database

- `OptimizationResultORM.input_payload` (JSON-kolom, migratie idempotent)
- `POST /optimize` slaat payload direct op bij pending-status
- Worker slaat payload ook op bij done-status
- Bevat: volledige trajectory, scenario, candidates, risk_params, objective, solver

#### Results-pagina

Twee kolommen + volle breedte:

| Sectie | Inhoud |
|---|---|
| Links | Doelfunctie/solver, traject (P₀ α norm η basisjaar), klimaatscenario, risicoparameters (V₀ δ γ T) |
| Rechts | Financieel resultaat (NCW, risico, investering), kandidaatmaatregelen-tabel (✓ = geselecteerd) |
| Vol breedte | P(t)-zaagrandgrafiek (P, Pmidden, Pwet) |

Twee actieknoppen:
- **"Opnieuw ↺"** — zelfde instellingen, bijv. andere solver
- **"Opnieuw met aanpassingen →"** — OptimizeForm met alle waarden pre-ingevuld

#### Bugfixes

- **Optimistic delete**: `useMutation` met `onMutate` verwijdert run direct uit TanStack Query-cache; geen zichtbare vertraging meer. Rollback bij API-fout.
- **204 No Content**: `request()` in de fetch-client probeerde `res.json()` op een lege body (DELETE-response). Opgelost door te returnen op status 204.

---

### Stap 4.9 — Normtraject-bundel (vroeger: dijkring-niveau) ⏳ (gepland)

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
