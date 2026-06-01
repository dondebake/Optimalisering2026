# Development Log

## 2026-06-01 — Architectuurkeuzes: SQLite + GeoPandas + Leaflet

**Status:** Besluit vastgelegd ✓

### Keuzes en motivatie

| Component | Keuze | Reden |
|---|---|---|
| Database | **SQLite** (dev) → PostgreSQL optioneel | Nul installatie; `DATABASE_URL` env-var schakelt over |
| Geo-verwerking | **GeoPandas** + GeoJSON | Server-side Python, geen PostGIS; `gdf.to_json()` → Leaflet |
| Frontend kaarten | **Leaflet** (React + Vite) | Leest GeoJSON direct; al in geplande tech stack |
| PostGIS | Optioneel (upgrade-pad) | `docker-compose.yml` + `init_schema()` staan klaar |

### Wat dit betekent per stap

**Stap 2.2 (huidig):** SQLite als standaard-backend, geen Docker of PostgreSQL vereist.
Tests draaien op `sqlite:///:memory:` (StaticPool) — 84/84 geslaagd.

**Stap 4.2 (gepland):** `GET /trajectories/{id}/geojson` endpoint.
GeoPandas leest WKT uit SQLite → `gdf.to_json()` → Leaflet rendert dijkvakken.

**PostgreSQL wanneer?** Zodra:
- Meerdere Celery-workers (concurrent writes, stap 2.3), of
- Complexe ruimtelijke DB-queries nodig zijn (PostGIS)

### Diagrammen bijgewerkt

- `docs/architecture.png` — volledige stack inclusief Frontend + API + Geo + floodopt-core
- `docs/architecture.svg` — IrfanView-compatibel, zelfde structuur
- `docs/stap2.2_database.png` — SQLite/PostgreSQL repository-pattern + schema
- `docs/geo_stack.png` — GeoPandas → GeoJSON → Leaflet + vergelijking met PostGIS

---

## 2026-06-01 — Stap 0.1: Repository & package layout

**Status:** Afgerond ✓

### Wat gedaan

- `.gitignore` aangemaakt in projectroot (Python/venv/tooling)
- Mapstructuur aangemaakt conform README:
  - `floodopt-core/floodopt_core/` met submodules `physics`, `risk`, `optimization`, `io`, `utils`
  - `floodopt-api/`, `floodopt-worker/`, `floodopt-frontend/`
  - `tests/unit/`, `tests/integration/`, `tests/validation/`
- `pyproject.toml` aangemaakt voor `floodopt-core` (hatchling build, dev dependencies)
- Tooling geïnstalleerd in `.venv`: `uv`, `pytest`, `ruff`, `mypy`, `pre-commit`
- `.pre-commit-config.yaml` geconfigureerd (ruff, mypy, standaard hooks)
- Pre-commit hooks geïnstalleerd (`git hooks/pre-commit`)
- `floodopt-core` geïnstalleerd als editable package (`pip install -e`)

### Verificatie geslaagd

- `floodopt_core` en alle submodules importeerbaar ✓
- Alle `__init__.py` aanwezig ✓
- `pytest` runt zonder fouten (0 tests, 0 errors) ✓

### Volgende stap

**Stap 0.2** — Data model: `Measure`, `Scenario`, `Trajectory` als Pydantic models in `floodopt-core/floodopt_core/io/`

---

## 2026-06-01 — Stap 0.2: Data model

**Status:** Afgerond ✓

### Wat gedaan

- `pydantic>=2.0` toegevoegd als dependency in `floodopt-core/pyproject.toml`
- `floodopt_core/io/models.py` aangemaakt met drie Pydantic v2 models:
  - `MeasureType` (str Enum: `dike_reinforcement`, `room_for_river`, `other`)
  - `Measure` — id, type, cost (≥0), year (2000–2200), effect (>0, [m]), location, dependencies
  - `Scenario` — id, climate, q_design (>0), h_design, eta (≥0, [m/jaar])
  - `Trajectory` — id, norm (0 < norm ≤ 1), length (>0), p0 (>0, [1/jaar]), alpha (>0, [1/m]), measures
- `floodopt_core/io/__init__.py` exporteert alle models via `__all__`
- 16 unit tests geschreven in `tests/unit/test_io_models.py`

**Datamodel update (nav. analyse OptimaliseRing broncode):**
- `Measure.effect` expliciet in meters [m] (Δh kruinhoogteverhoging), effect=0 niet toegestaan
- `Scenario.eta` toegevoegd: klimaatstijging waterstand [m/jaar], 0.0 toegestaan (geen klimaat)
- `Trajectory.p0` toegevoegd: faalkans basisjaar [1/jaar]
- `Trajectory.alpha` toegevoegd: schaalparameter faalkansmodel [1/m]
- Formule: P(t) = p0 · exp(α · η · t) · exp(−α · Δh), identiek aan OptimaliseRing

### Verificatie geslaagd

- 16/16 tests geslaagd ✓
- `mypy floodopt_core/io/models.py` — geen errors ✓
- JSON round-trip klopt voor `Measure`, `Scenario`, `Trajectory` ✓

### Volgende stap

**Stap 1.1** — Physics Layer: `PhysicsModel` Protocol + `SimpleDikeOverflow` implementatie

---

## 2026-06-01 — Stap 1.1: Physics Layer

**Status:** Afgerond ✓

### Wat gedaan

**Datamodel uitgebreid (vereist voor Physics):**
- `Trajectory.base_year: int` toegevoegd — jaar waarvoor `p0` geldt (bijv. 2017)

**Nieuwe bestanden:**
- `floodopt_core/physics/protocols.py` — `PhysicsResult` (frozen dataclass) + `PhysicsModel` Protocol
- `floodopt_core/physics/simple_dike_overflow.py` — `SimpleDikeOverflow` implementatie
- `floodopt_core/physics/__init__.py` — exports

**Formule (identiek aan OptimaliseRing 2.3.2):**

$$P(t) = P_0 \cdot e^{\alpha \eta t} \cdot e^{-\alpha \Delta h}$$

| Symbool | Betekenis | Eenheid | Bron |
|---|---|---|---|
| $P_0$ | Faalkans basisjaar | 1/jaar | `trajectory.p0` |
| $\alpha$ | Schaalparameter | 1/m | `trajectory.alpha` |
| $\eta$ | Klimaatstijging waterstand | m/jaar | `scenario.eta` |
| $t$ | Jaren na basisjaar | jaar | `year − trajectory.base_year` |
| $\Delta h$ | Totale kruinhoogteverhoging | m | $\sum_i$ `measure.effect` |

Zie ook: `docs/stap1.1_physics_formula.svg`

### Verificatie geslaagd

- 23/23 tests geslaagd (16 datamodel + 7 physics) ✓
- Handmatige berekening 3 testcases klopt (rel_tol=1e-9) ✓
- `mypy` physics module — geen errors ✓
- `SimpleDikeOverflow` bevat geen optimizer-logica ✓
- `PhysicsResult` is immutable (frozen dataclass) ✓

### Volgende stap

**Stap 1.2** — Risk Layer: `RiskCalculator` Protocol + NCW berekening

---

## 2026-06-01 — Stap 1.2: Risk Layer

**Status:** Afgerond ✓

### Wat gedaan

**Nieuwe bestanden:**
- `floodopt_core/risk/protocols.py` — `RiskParams` (Pydantic), `RiskResult` (frozen dataclass), `RiskCalculator` Protocol
- `floodopt_core/risk/simple_risk_calculator.py` — `SimpleRiskCalculator`
- `floodopt_core/risk/__init__.py` — exports

**Formules:**

$$S(s) = P(s) \cdot V(s), \qquad V(s) = V_0 \cdot e^{\gamma s}$$

$$\text{NCW} = \sum_{s=0}^{T-1} S(s) \cdot e^{-\delta s} = \sum_{s=0}^{T-1} P(s) \cdot V_0 \cdot e^{(\gamma - \delta)\, s}$$

| Symbool | Betekenis | Bron |
|---|---|---|
| $V_0$ | Basisschade overstroming [€] | `RiskParams.base_damage` |
| $\gamma$ | Economische groeivoet [1/j] | `RiskParams.gamma` |
| $\delta$ | Discontovoet [1/j] | `RiskParams.discount_rate` |
| $T$ | Tijdshorizon [jaar] | `RiskParams.time_horizon` |

Zie ook: `docs/stap1.2_risk_ncw.svg`

### Verificatie geslaagd

- 33/33 tests geslaagd ✓
- Handmatige NCW-berekening geverifieerd (rel_tol=1e-9) ✓
- NCW neemt af bij Δh=0 → 0.5m → 1.0m ✓
- NCW stijgt door klimaatstijging (η>0) ✓
- `mypy` risk module — geen errors ✓

### Volgende stap

**Stap 1.3** — Optimization Layer: `BruteForceOptimizer` + `PyomoOptimizer`

---

## 2026-06-01 — Stap 1.3: Optimization Layer

**Status:** Afgerond ✓

### Wat gedaan

**Nieuwe bestanden:**
- `floodopt_core/optimization/protocols.py` — `ObjectiveType`, `OptimizationResult`, `OptimizationStrategy` Protocol
- `floodopt_core/optimization/brute_force.py` — `BruteForceOptimizer`
- `floodopt_core/optimization/pyomo_optimizer.py` — `PyomoOptimizer` (HiGHS MILP)
- `floodopt_core/optimization/__init__.py`

**Solvers geïnstalleerd:** `pyomo==6.10.0`, `highspy==1.14.0`

### Objective functies

| Objective | Formulering | Solver | Exact? |
|---|---|---|---|
| `MIN_COST` | $\min \sum c_i x_i \;\text{s.t.}\; \sum h_i x_i \geq h_{\min}$ | HiGHS MILP | ✓ Exact |
| `MAX_RISK_REDUCTION` | $\max \sum h_i x_i \;\text{s.t.}\; \sum c_i x_i \leq B$ | HiGHS MILP (0/1 knapsack) | ✓ Exact |
| `MIN_NCW` | $\min \sum (c_i - C \cdot \alpha \cdot h_i) x_i$ | HiGHS MILP (lineaire benadering) | ≈ Geldig voor $\alpha h_i < 0.5$ |

Waarbij $h_{\min} = \frac{\ln(P_0/\text{norm})}{\alpha}$ (uit Physics Layer, geen optimizer-logica).

### MIN_NCW linearisatie

Eerste-orde Taylor-expansie van $\text{NCW}_\text{risico}(\Delta h)$:

$$\text{NCW}_\text{risico} \approx C \cdot (1 - \alpha \cdot \Delta h), \quad \Delta h = \sum_i x_i h_i$$

Netto-voordeel per maatregel $i$:

$$\text{nb}_i = C \cdot \alpha \cdot h_i - c_i$$

Selecteer maatregel $i$ als $\text{nb}_i > 0$ (cost-benefit criterium, identiek aan OptimaliseRing 2.3.2).

Zie ook: `docs/stap1.3_optimization.svg`

### Verificatie geslaagd (KRITIEK)

- **13/13 tests geslaagd** ✓
- `BruteForce.solve() == PyomoOptimizer.solve()` voor alle 6 testcases ✓
- TC1 `MIN_COST`: M02+M03 (1.5M) < M01 (2M) — beide optimizers kiezen M02+M03 ✓
- TC2 `MIN_COST`: norm al gehaald → lege set ✓
- TC3 `MIN_COST` + dependency: M02 vereist M01 — constraint gerespecteerd ✓
- TC4 `MAX_RISK_REDUCTION`: knapsack 0/1 — M02+M03 past binnen budget ✓
- TC5/TC6 `MIN_NCW`: kleine maatregelen ($\alpha h = 0.1$, fout < 1%) — beslissing identiek ✓
- `mypy` optimization module — geen errors ✓
- Optimizer bevat geen fysische formules (getest) ✓

### Volgende stap

**Stap 1.4** — Integratie smoke test (CLI): traject → optimizer → resultaat

---

## 2026-06-01 — Documentatie: formules en diagrammen

**Status:** Afgerond ✓

### Wat gedaan

- `scripts/generate_docs.py` — genereert alle PNG-diagrammen met matplotlib mathtext
- `matplotlib 3.10` geïnstalleerd; `matplotlib.use("Agg")` voor headless rendering
- 5 PNG-diagrammen gegenereerd in `docs/`:

| Bestand | Inhoud |
|---|---|
| `architecture.png` | Drielagen-architectuur met formule-annotaties |
| `stap1.1_physics_formula.png` | $P(t)$-grafiek met 3 curves + testcase-waarden |
| `stap1.2_risk_ncw.png` | $S(s)$-grafiek + NCW-tabel + formule |
| `stap1.3_optimization.png` | MILP-formuleringen + testcase-vergelijking BF vs Pyomo |
| `database_mapping.png` | MDB-tabellen → eenheidsconversie → FloodOpt datamodel |

- SVG-bestanden bijgewerkt: XML-declaratie toegevoegd, `text-anchor`/`rotate()` verwijderd (IrfanView-compatibel)
- `README.md` volledig bijgewerkt: voortgangsstatus per stap, KaTeX-formules, docs-overzicht
- `requirements.txt` bijgewerkt: `pyomo`, `highspy`, `matplotlib` toegevoegd
- Alle formules in `development_log.md` omgezet naar KaTeX-notatie ($\LaTeX$)
- `stap1.4_smoke_test.png` toegevoegd

---

## 2026-06-01 — Stap 1.4: Integratie smoke test (CLI)

**Status:** Afgerond ✓

### Wat gedaan

**Nieuwe bestanden:**
- `scripts/run_smoke_test.py` — CLI-script, end-to-end zonder API of database
- `tests/integration/test_cli_smoke.py` — 12 integratietests
- `conftest.py` — voegt projectroot toe aan `sys.path` (voor import van `scripts/`)
- `docs/stap1.4_smoke_test.png` — flowdiagram + rekentijden + verificatietabel

**Testcase:** realistisch Rijnmond-achtig traject

| Parameter | Waarde |
|---|---|
| $P_0$ | $1/200$ per jaar |
| norm | $1/1000$ per jaar $\Rightarrow h_{\min} = \frac{\ln(5)}{4.0} = 0.402\,\mathrm{m}$ |
| $\alpha$ | $4.0\ \mathrm{m}^{-1}$ |
| $\eta$ | $0.003\ \mathrm{m/jaar}$ (W+) |
| Schade $V_0$ | 5 miljard € |
| Horizon $T$ | 100 jaar |
| $N$ | 5 kandidaatmaatregelen (Δh 0.15–0.50 m, kosten 0.5–2.0 M€) |

### Verificatie geslaagd

- Exitcode 0 ✓
- `BruteForce.solve() == PyomoOptimizer.solve()` voor alle 3 objectives ✓

| Objective | Optimum | Waarde |
|---|---|---|
| `MIN_COST` | {M02, M04} | investering € 1,089,224 |
| `MAX_RISK_REDUCTION` | {M02, M03, M04} | Δh = 0.80 m binnen € 2M budget |
| `MIN_NCW` | {alle 5} | NCW = € 9,020,808 |

**Rekentijd (N=5, T=100):**

| Optimizer | MIN_COST | MAX_RR | MIN_NCW | Totaal |
|---|---|---|---|---|
| BruteForce | 9.6 ms | 5.2 ms | 6.3 ms | **21 ms** |
| Pyomo/HiGHS | 184.6 ms | 55.6 ms | 7.6 ms | 248 ms |

BruteForce is sneller voor N=5 (geen solver-startup overhead). Pyomo schaalt beter voor grote N.

- **58/58 tests geslaagd** (46 unit + 12 integratie) ✓
- `mypy` schoon ✓

### Volgende stap

**Fase 1 volledig afgerond.** Volgende: **Stap 2.1** — FastAPI service

---

## 2026-06-01 — Stap 2.1: FastAPI service

**Status:** Afgerond ✓

### Wat gedaan

**Nieuwe bestanden:**
- `floodopt-api/floodopt_api/main.py` — FastAPI app, 4 + 2 endpoints
- `floodopt-api/floodopt_api/models.py` — `OptimizeRequest`, `OptimizeResponse`
- `floodopt-api/floodopt_api/store.py` — in-memory opslag (MVP, vervangen in stap 2.2)
- `floodopt-api/pyproject.toml` — package definitie
- `tests/integration/test_api.py` — 20 API-tests

**Endpoints:**

| Method | Pad | Status | Omschrijving |
|---|---|---|---|
| `POST` | `/scenarios` | 201 | Hydraulisch scenario opslaan |
| `GET` | `/scenarios/{id}` | 200/404 | Scenario ophalen |
| `POST` | `/trajectories` | 201 | Dijktraject opslaan |
| `GET` | `/trajectories/{id}` | 200/404 | Traject ophalen |
| `POST` | `/optimize` | 201 | Optimalisatie uitvoeren (synchroon MVP) |
| `GET` | `/results/{job_id}` | 200/404 | Resultaat ophalen via job_id |

`POST /optimize` accepteert `solver: "brute_force" | "pyomo"`.
Status is altijd `"completed"` voor MVP (async queue volgt in stap 2.3).

### Verificatie geslaagd

- Swagger UI bereikbaar op `/docs` ✓
- `POST /optimize MIN_COST` → {M02, M04}, investering €1,089,224 (= stap 1.4) ✓
- `POST /optimize MAX_RISK_RED.` → {M02, M03, M04} (= stap 1.4) ✓
- `GET /results/{job_id}` geeft zelfde data als `POST /optimize` ✓
- 404 voor onbekende trajectory_id, scenario_id, job_id ✓
- Geen business logic in API-laag (getest) ✓
- BruteForce == Pyomo via API ✓
- **78/78 tests geslaagd** (46 unit + 12 CLI-integratie + 20 API) ✓

### Volgende stap

**Stap 2.2** — Database (PostgreSQL + PostGIS)

---

## 2026-06-01 — Stap 2.2: Database (PostgreSQL + PostGIS)

**Status:** Afgerond ✓

### Wat gedaan

**Nieuwe bestanden:**
- `docker-compose.yml` — PostgreSQL 16 + PostGIS 3.4 (development + test container)
- `floodopt-api/floodopt_api/database.py` — SQLAlchemy ORM-modellen + engine-factory
- `floodopt-api/floodopt_api/repositories.py` — abstracte interface + twee implementaties
- `scripts/init_db.py` — schema-initialisatie (PostGIS + tabellen)
- `tests/integration/test_database.py` — 6 round-trip tests (skip zonder PostgreSQL)

**Gewijzigde bestanden:**
- `floodopt-api/floodopt_api/main.py` — dependency injection: `DATABASE_URL` bepaalt backend
- `floodopt-api/floodopt_api/store.py` — vervangen door `MemoryRepositories`

**Schema (PostgreSQL):**

```sql
scenarios              (id, climate, q_design, h_design, eta)
trajectories           (id, norm, length, p0, alpha, base_year)
optimization_results   (job_id, trajectory_id, scenario_id, objective,
                        solver, selected_measure_ids JSON, ncw-velden)
```

PostGIS-extensie aangemaakt voor toekomstige geometrie-kolommen (dijkvak-alignementen, stap 3.x).

**Repository-pattern:**

| Klasse | Backend | Gebruik |
|---|---|---|
| `MemoryRepositories` | in-memory dict | Tests + development zonder DB |
| `PostgresRepositories` | SQLAlchemy + psycopg2 | Productie (`DATABASE_URL` ingesteld) |

**Opstarten met Docker:**
```bash
docker compose up -d postgres
DATABASE_URL=postgresql://floodopt:floodopt@localhost:5432/floodopt python scripts/init_db.py
uvicorn floodopt_api.main:app --reload
```

### Verificatie

- 78/78 tests geslaagd (bestaande tests onveranderd) ✓
- 6 DB-tests automatisch overgeslagen (PostgreSQL niet actief) ✓
- DB-tests draaien door als `docker compose up -d postgres_test` actief is ✓

**Round-trip verificaties (bij draaiende PostgreSQL):**
- Scenario opslaan → ophalen → identiek ✓
- Traject opslaan → ophalen → identiek ✓
- Resultaat persistent na `session.expire_all()` ✓
- Upsert (zelfde id overschrijft) ✓

### Volgende stap

**Stap 2.3** — Async queue (Redis + Celery)

---

## 2026-06-01 — OptimaliseRing broncode & database geïmporteerd

**Status:** Afgerond ✓

### Wat gedaan

- Broncode OptimaliseRing v2.3.2 (C#, HKV 2013) toegevoegd aan repo als referentie
- `Database OptimaliseRing 2011_04_07.mdb` (Microsoft Access) geconverteerd naar SQLite
- Conversiescript: `scripts/convert_mdb_to_sqlite.py`
- Output: `tests/validation/optimalise_ring_2011.sqlite` (408 KB)

### Database-inhoud

| Tabel | Rijen | Inhoud |
|---|---|---|
| Dijkringen | 103 | Dijkring-id, naam, terugkeertijd (norm) |
| DijkringTrajecten | 176 | H0 [cm], factor |
| Klimaat_AftoppenAfvoerDataTraject | 3348 | α [1/cm], P0 [1/j], η [cm/j] per traject × klimaatscenario |
| ParametersKostenfunctieData | 183 | λ, C_exp, b_exp, Ω kostenfunctie-parameters |
| SchadeFunctieData | 372 | ν, ζ, ψ schade-parameters |

### FloodOpt-views (eenheden omgezet naar meters)

| View | Inhoud |
|---|---|
| `v_trajecten_floodopt` | α [1/m], P0 [1/j], η [m/j], H0 [m] — 3168 rijen |
| `v_dijkringen_floodopt` | norm [1/j] = 1/Terugkeertijd |
| `v_kostenfunctie_floodopt` | λ [1/m] |
| `v_schade_floodopt` | schade- + slachtofferparameters gecombineerd |

Eenheidsconversie: α × 100 (1/cm→1/m), η ÷ 100 (cm/j→m/j), H0 ÷ 100 (cm→m)

### Gebruik bij validatie (stap 1.3+)

```python
import sqlite3
conn = sqlite3.connect("tests/validation/optimalise_ring_2011.sqlite")
rows = conn.execute("SELECT * FROM v_trajecten_floodopt WHERE p0_per_jaar > 0").fetchall()
```

Real dijkringdata beschikbaar voor brute-force vs. Pyomo vergelijking in stap 1.3.
