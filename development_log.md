# Development Log

## Actuele architectuurkeuzes

| Component | Keuze | Reden |
|---|---|---|
| Database | **SQLite** (standaard, ingebouwd) | Nul installatie; geen Docker vereist |
| DB upgrade pad | PostgreSQL optioneel via `DATABASE_URL` | `init_schema()` werkt met beide |
| Geo-verwerking | **GeoPandas** + GeoJSON | Python-only, geen PostGIS nodig voor MVP |
| Frontend kaarten | **Leaflet** (React + Vite) | Leest GeoJSON direct van de API |
| Optimizer | Pyomo + HiGHS (MILP) | Open-source, exact voor MIN_COST/MAX_RR |

**PostgreSQL / PostGIS** wordt pas ingezet zodra meerdere Celery-workers concurrent schrijven (stap 2.3) of ruimtelijke DB-queries nodig zijn.

---

## 2026-06-01 — Stap 0.1: Repository & package layout ✓

- Mapstructuur aangemaakt, tooling geïnstalleerd (`pytest`, `ruff`, `mypy`, `pre-commit`)
- `floodopt-core` als editable package geïnstalleerd
- **Verificatie:** `floodopt_core` importeerbaar, `pytest` 0 tests 0 errors ✓

---

## 2026-06-01 — Stap 0.2: Data model ✓

Pydantic v2 models in `floodopt_core/io/`:

| Model | Sleutelvelden |
|---|---|
| `Measure` | id, type, cost, year, **effect [m]**, location, dependencies |
| `Scenario` | id, climate, q_design, h_design, **eta [m/jaar]** |
| `Trajectory` | id, norm, length, **p0 [1/jaar]**, **alpha [1/m]**, **base_year** |

Eenheden direct gebaseerd op analyse OptimaliseRing 2.3.2 (HKV, 2013).
**Verificatie:** 16/16 tests, mypy schoon, JSON round-trip ✓

---

## 2026-06-01 — Stap 1.1: Physics Layer ✓

$$P(t) = P_0 \cdot e^{\alpha \eta t} \cdot e^{-\alpha \Delta h}$$

- `SimpleDikeOverflow` achter `PhysicsModel` Protocol
- Formule identiek aan OptimaliseRing 2.3.2
- `PhysicsResult` frozen dataclass

**Verificatie:** 23/23 tests, 3 handmatige cases rel_tol=1e-9, geen optimizer-logica in physics ✓

Zie: `docs/stap1.1_physics_formula.png`

---

## 2026-06-01 — Stap 1.2: Risk Layer ✓

$$\text{NCW} = \sum_{s=0}^{T-1} P(s) \cdot V_0 \cdot e^{(\gamma - \delta)\,s}$$

- `SimpleRiskCalculator` achter `RiskCalculator` Protocol
- Discrete jaarlijkse sommatie; aparte discontovoeten voor schade (γ) en investering (δ)

**Verificatie:** 33/33 tests, handmatige NCW rel_tol=1e-9, NCW daalt bij meer maatregelen ✓

Zie: `docs/stap1.2_risk_ncw.png`

---

## 2026-06-01 — Stap 1.3: Optimization Layer ✓

| Objective | Formulering | Solver | Exact? |
|---|---|---|---|
| `MIN_COST` | $\min \sum c_i x_i \;\text{s.t.}\; \sum h_i x_i \geq h_{\min}$ | HiGHS MILP | Ja |
| `MAX_RISK_REDUCTION` | $\max \sum h_i x_i \;\text{s.t.}\; \sum c_i x_i \leq B$ | HiGHS MILP (knapsack) | Ja |
| `MIN_NCW` | $\min \sum (c_i - C\alpha h_i)\,x_i$ | HiGHS MILP | Lineaire benadering $\alpha h_i < 0.5$ |

**Verificatie (kritiek):** 13/13 tests, `BruteForce.solve() == PyomoOptimizer.solve()` voor alle 6 testcases ✓

Zie: `docs/stap1.3_optimization.png`

---

## 2026-06-01 — OptimaliseRing broncode & validatiedata geïmporteerd ✓

- Broncode OptimaliseRing v2.3.2 (C#, HKV 2013) als referentie toegevoegd
- `Database OptimaliseRing 2011_04_07.mdb` → `tests/validation/optimalise_ring_2011.sqlite`
- Script: `scripts/convert_mdb_to_sqlite.py`
- 4 FloodOpt-views met eenheidsconversie (α×100 [1/cm→1/m], η÷100 [cm/j→m/j])
- 103 dijkringen, 176 trajecten, 3348 klimaatrecords beschikbaar voor validatie

---

## 2026-06-01 — Stap 1.4: Integratie smoke test (CLI) ✓

```bash
python scripts/run_smoke_test.py
```

Testcase: Rijnmond-achtig traject ($P_0 = 1/200$, norm $= 1/1000$, 5 kandidaatmaatregelen).

| Objective | Optimum | Waarde | BF = Pyomo |
|---|---|---|---|
| `MIN_COST` | {M02, M04} | € 1,089,224 | ✓ |
| `MAX_RISK_REDUCTION` | {M02, M03, M04} | Δh = 0.80 m | ✓ |
| `MIN_NCW` | {alle 5} | NCW = € 9,020,808 | ✓ |

Rekentijd: BruteForce 21 ms — Pyomo 248 ms (N=5, T=100).

**Verificatie:** 58/58 tests, exitcode 0 ✓ — **Fase 1 volledig afgerond.**

Zie: `docs/stap1.4_smoke_test.png`

---

## 2026-06-01 — Stap 2.1: FastAPI service ✓

| Method | Endpoint | Status |
|---|---|---|
| POST | `/scenarios` | 201 |
| POST | `/trajectories` | 201 |
| POST | `/optimize` | 201 — `solver: "brute_force" \| "pyomo"` |
| GET | `/results/{job_id}` | 200/404 |
| GET | `/docs` | 200 — Swagger UI |

Geen business logic in API-laag. Status altijd `"completed"` (async queue volgt stap 2.3).

**Verificatie:** 78/78 tests, Swagger UI bereikbaar, resultaten identiek aan stap 1.4 ✓

Zie: `docs/stap2.1_api.png`

---

## 2026-06-01 — Stap 2.2: Database (SQLite + repository-pattern) ✓

**Keuze:** SQLite als standaard — geen Docker, geen installatie.

### Opstarten

```bash
# Standaard (SQLite, geen configuratie nodig):
uvicorn floodopt_api.main:app --reload
# schrijft naar floodopt.db naast de projectroot

# Optioneel PostgreSQL:
DATABASE_URL=postgresql://user:pw@host/db uvicorn floodopt_api.main:app --reload
python scripts/init_db.py   # maakt tabellen aan (idempotent)
```

### Repository-pattern

| Klasse | Backend | Wanneer |
|---|---|---|
| `MemoryRepositories` | in-memory dict | Altijd in tests (FastAPI dependency override) |
| `OrmRepositories` | SQLAlchemy + SQLite of PostgreSQL | `get_repositories()` in productie |

`DATABASE_URL` env-var bepaalt welke SQLAlchemy-URL gebruikt wordt; standaard `sqlite:///floodopt.db`.

### Schema

```
scenarios              (id, climate, q_design, h_design, eta)
trajectories           (id, norm, length, p0, alpha, base_year)
optimization_results   (job_id, trajectory_id, scenario_id, objective,
                        solver, selected_measure_ids JSON, ncw-velden)
```

### Verificatie

- **84/84 tests geslaagd** (geen Docker vereist) ✓
- 6 DB round-trip tests op `sqlite:///:memory:` (StaticPool) ✓
- Scenario/traject opslaan → ophalen → identiek ✓
- Resultaat persistent na `session.expire_all()` ✓

Zie: `docs/stap2.2_database.png`

---

## 2026-06-01 — Documentatie: diagrammen herontworpen ✓

Aanleiding: overlappende tekst en frames in alle PNG-diagrammen.

**Oplossing:** `scripts/generate_docs.py` volledig herschreven (1983 → ~470 regels).

Nieuwe ontwerpregels:
- `ax.table()` voor alle tabellen — geen handmatige tekstlussen meer
- Elk formulepaneel = eigen subplot (geen gedeelde assen)
- `ax.annotate()` met expliciete `xytext`-offset voor data-annotaties
- `fig.tight_layout(rect=[0,0,1,0.95])` op elke figuur

9 PNGs gegenereerd in `docs/`:

| Bestand | Inhoud |
|---|---|
| `architecture.png` | Volledige stack: Frontend → API → SQLite/Geo → Core |
| `stap1.1_physics_formula.png` | $P(t)$-grafiek + formule |
| `stap1.2_risk_ncw.png` | $S(s)$-grafiek + NCW-tabel + formule |
| `stap1.3_optimization.png` | 3 objectieve formules + NCW-grafiek + verificatietabel |
| `stap1.4_smoke_test.png` | Rekentijden + testresultaten |
| `stap2.1_api.png` | Request-flow + endpoints |
| `stap2.2_database.png` | Repository-pattern + schema |
| `geo_stack.png` | GeoPandas → GeoJSON → Leaflet |
| `database_mapping.png` | OptimaliseRing MDB → FloodOpt datamodel |

---

---

## 2026-06-01 — Stap 2.3: Ontwerpbeslissing — Async queue (Celery + Redis)

### Aanleiding

Het synchrone `POST /optimize` blokkeert de HTTP-verbinding totdat de optimizer klaar is. Voor N=5 maatregelen duurt dat 21–248 ms — geen probleem. Maar bij dijkring- of systeemniveau (10–50 maatregelen per traject, meerdere klimaatscenario's) kan Pyomo/HiGHS minuten draaien. Dan zijn er drie breekpunten:

| Probleem | Gevolg |
|---|---|
| HTTP-verbinding time-out (30–60 sec) | Client krijgt fout; resultaat gaat verloren |
| FastAPI-thread geblokkeerd | Andere requests wachten achter de berekening |
| Geen herstelbaarheid bij crash | Hele job verloren |

### Oplossing: Celery + Redis

```
POST /optimize  →  job_id opslaan (status: pending)  →  task.delay()  →  202 Accepted
                                                              ↓
                                                   Redis wachtrij
                                                              ↓
                                                   Celery worker
                                                   status: running
                                                   optimizer.solve()
                                                   status: done + resultaten
```

### Architectuurkeuzes

| Keuze | Motivatie |
|---|---|
| Redis als broker | Standaard voor Celery; in-memory, snel |
| SQLite bij één worker | Concurrent schrijven niet nodig; nul installatie |
| PostgreSQL bij meerdere workers | SQLite ondersteunt geen concurrent schrijven vanuit meerdere processen |
| `task_always_eager=True` in tests | Tests draaien zonder Redis; Celery voert taken synchroon uit |
| Bestaande `status`-kolom in ORM | `OptimizationResultORM.status` was er al — geen schemamigrate nodig |

### Wat verandert in de API

| | Voor 2.3 | Na 2.3 |
|---|---|---|
| `POST /optimize` status | 201 + `status: completed` | 202 + `status: pending` |
| Berekening | In de API-request handler | In Celery worker |
| `GET /results/{job_id}` | Altijd `completed` | `pending` / `running` / `done` |

### Implementatievolgorde

1. `floodopt-worker/` — Celery app + `run_optimization` task
2. `floodopt-api/` — `POST /optimize` geeft 202 terug, dispatch naar worker
3. ORM + repository — `update_result_status()` voor worker
4. Tests — API-tests mocken task dispatch; worker-tests gebruiken `always_eager`
5. `docker-compose.yml` — Redis service

---

## 2026-06-01 — Stap 2.3: Async queue (Redis + Celery) ✓

### Wat is gebouwd

| Component | Bestand | Inhoud |
|---|---|---|
| Celery-app | `floodopt_api/celery_app.py` | Broker + backend via `REDIS_URL` |
| Worker-taak | `floodopt_worker/tasks.py` | `run_optimization`: pending→running→done/failed |
| Worker-package | `floodopt-worker/` | `pyproject.toml`, eigen editable install |
| Broker | `docker-compose.yml` | Redis 7-alpine op poort 6379 |

### API-wijzigingen

| | Voor 2.3 | Na 2.3 |
|---|---|---|
| `POST /optimize` status code | 201 | 202 |
| Directe response status | `"completed"` + resultaten | `"pending"` + lege resultaatvelden |
| Berekening | In API-request handler | In Celery worker |
| `GET /results/{job_id}` | Altijd `completed` | `pending` / `running` / `done` / `failed` |

### Status-flow

```
POST /optimize  →  save(status=pending)  →  send_task()  →  202 + job_id
                                                 ↓
                                          Redis wachtrij
                                                 ↓
                                          Celery worker
                                          update_status(running)
                                          optimizer.solve()
                                          save_result(status=done)
```

### Opstarten (lokaal)

```bash
# 1. Redis starten
docker compose up -d redis

# 2. API starten
uvicorn floodopt_api.main:app --reload

# 3. Worker starten (apart terminal)
celery -A floodopt_worker.tasks worker --loglevel=info

# 4. Optimalisatie uitvoeren
curl -X POST http://localhost:8000/optimize -H "Content-Type: application/json" -d '{...}'
# → {"job_id": "...", "status": "pending", ...}

curl http://localhost:8000/results/{job_id}
# → {"status": "done", "selected_measure_ids": [...], ...}
```

### Verificatie

- **90/90 tests geslaagd** (geen Redis vereist voor pytest) ✓
- 19 API-tests: 202, pending, Celery send_task mock ✓
- 7 worker-tests: pending→done, MIN_COST/MAX_RR/MIN_NCW vs stap 1.4, BF==Pyomo, failed-status ✓
- 6 DB round-trip tests: pending opslaan + ophalen ✓

Zie: `docs/stap2.3_worker.png` *(toe te voegen)*

---

---

## 2026-06-01 — Stap 2.3: Lokaal opstarten via start.bat ✓

Docker geïnstalleerd. `start.bat` opent drie terminals automatisch:

| Terminal | Proces | Poort |
|---|---|---|
| 1 | Redis 7-alpine (Docker) | 6379 |
| 2 | FastAPI + uvicorn | 8000 |
| 3 | Celery worker (`--pool=solo`) | — |

90/90 tests groen. Volledige async pipeline operationeel.

---

## 2026-06-02 — Stap 4.1: Frontend setup ✓

Beslissing: **Fase 4 (frontend) vóór Fase 3** (rekenkernel uitbreidingen).

Reden: FORM/Monte Carlo, lengte-effecten en rivierverruiming zitten **niet** in het oorspronkelijke OptimaliseRing 2.3.2 model — het is origineel onderzoekswerk. Frontend maakt het systeem eerst bruikbaar.

### Tech stack

| Component | Keuze | Reden |
|---|---|---|
| Build tool | Vite + React + TypeScript | Standaard, snel, goede DX |
| Styling | Tailwind CSS (v4, Vite-plugin) | Utility-first, geen losse CSS-bestanden |
| API state | TanStack Query | Polling, caching, loading-states ingebouwd |
| Kaarten | Leaflet + react-leaflet | Leest GeoJSON direct van API (stap 4.2) |
| Dev proxy | Vite proxy `/api → localhost:8000` | Geen CORS-issue in development |

### Wat is gebouwd

| Bestand | Inhoud |
|---|---|
| `src/types/index.ts` | TypeScript interfaces — `Measure`, `Scenario`, `Trajectory`, `OptimizeRequest/Response`, `JobStatus` |
| `src/api/client.ts` | Typed fetch-wrapper — `postScenario`, `postTrajectory`, `postOptimize`, `getResult`, `submitOptimization` |
| `src/pages/OptimizeForm.tsx` | Formulier: traject + scenario + kandidaatmaatregelen + doelfunctie |
| `src/pages/Results.tsx` | Polling via TanStack Query (2 s interval) — pending/running/done/failed weergave + financiële samenvatting |
| `src/pages/Dashboard.tsx` | Startpagina met link naar OptimizeForm en stack-status |
| `src/components/StatusBadge.tsx` | Kleurgecodeerde badge per `JobStatus` |
| `src/components/MeasureList.tsx` | Herbruikbare maatregel-editor |

### API-kant

- `CORSMiddleware` in `floodopt_api/main.py` — `allow_origins=["http://localhost:5173"]`
- `start.bat` uitgebreid met terminal 4: `cd floodopt-frontend && npm run dev`

### Verificatie

- Frontend bereikbaar op `http://localhost:5173` ✓
- Optimalisatieformulier → 202 → polling → done ✓
- Alle vier terminals starten via `start.bat` ✓
