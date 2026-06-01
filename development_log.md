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

## Volgende stap

**Stap 2.3** — Async queue (Redis + Celery)

`POST /optimize` geeft direct `job_id` terug. Status: `pending → running → done`.
Let op: SQLite → PostgreSQL switch verplicht bij meerdere Celery-workers (concurrent writes).
