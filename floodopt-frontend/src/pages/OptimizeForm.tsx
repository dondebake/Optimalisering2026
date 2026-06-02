import { useState } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import { submitOptimization } from '../api/client'
import type { Measure, MeasureType, ObjectiveType, RiskParams, Scenario, Trajectory } from '../types'

const DEFAULT_SCENARIO: Scenario = {
  id: 'sc-demo',
  climate: 'huidig',
  q_design: 16000,
  h_design: 7.5,
  eta: 0.003,
}

const DEFAULT_TRAJECTORY: Trajectory = {
  id: 'traj-demo',
  norm: 1e-3,
  length: 12.5,
  p0: 1 / 200,
  alpha: 4.0,
  base_year: 2025,
  measures: [],
}

const DEFAULT_RISK: RiskParams = {
  base_damage: 2e9,
  discount_rate: 0.04,
  gamma: 0.02,
  time_horizon: 100,
}

const EMPTY_MEASURE: Measure = {
  id: '',
  type: 'dike_reinforcement',
  cost: 0,
  year: 2030,
  effect: 0.5,
  location: '',
  dependencies: [],
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <label className="flex flex-col gap-1 text-sm">
      <span className="font-medium text-gray-700">{label}</span>
      {children}
    </label>
  )
}

const INPUT = 'border border-gray-300 rounded px-2 py-1.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400'
const SELECT = INPUT

export default function OptimizeForm() {
  const navigate = useNavigate()
  const location = useLocation()
  const prefill = location.state as { trajectory?: Partial<Trajectory>; scenario?: Partial<Scenario>; candidates?: Measure[] } | null

  const [scenario, setScenario] = useState<Scenario>({ ...DEFAULT_SCENARIO, ...prefill?.scenario })
  const [trajectory, setTrajectory] = useState<Trajectory>({ ...DEFAULT_TRAJECTORY, ...prefill?.trajectory })
  const [risk, setRisk] = useState<RiskParams>(DEFAULT_RISK)
  const [candidates, setCandidates] = useState<Measure[]>(
    prefill?.candidates ?? [
      { ...EMPTY_MEASURE, id: 'M01', effect: 0.5, cost: 500_000, location: 'vak-1' },
      { ...EMPTY_MEASURE, id: 'M02', effect: 0.5, cost: 400_000, location: 'vak-2' },
    ]
  )
  const [objective, setObjective] = useState<ObjectiveType>('min_cost')
  const [solver, setSolver] = useState<'brute_force' | 'pyomo'>('brute_force')
  const [budget, setBudget] = useState<string>('')
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  function updateMeasure(i: number, patch: Partial<Measure>) {
    setCandidates((prev) => prev.map((m, idx) => (idx === i ? { ...m, ...patch } : m)))
  }

  function addMeasure() {
    const n = candidates.length + 1
    setCandidates((prev) => [
      ...prev,
      { ...EMPTY_MEASURE, id: `M${String(n).padStart(2, '0')}`, location: `vak-${n}` },
    ])
  }

  function removeMeasure(i: number) {
    setCandidates((prev) => prev.filter((_, idx) => idx !== i))
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    if (candidates.length === 0) {
      setError('Voeg minimaal één kandidaatmaatregel toe.')
      return
    }
    if (objective === 'max_risk_reduction' && !budget) {
      setError('Budget is verplicht voor MAX_RISK_REDUCTION.')
      return
    }
    setLoading(true)
    try {
      const job = await submitOptimization(scenario, trajectory, candidates, {
        risk_params: risk,
        objective,
        solver,
        budget: budget ? parseFloat(budget) : null,
      })
      navigate(`/results/${job.job_id}`)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Onbekende fout')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="max-w-3xl mx-auto p-6 space-y-8">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Nieuwe optimalisatie</h1>
        <p className="text-sm text-gray-500 mt-1">
          Vul traject, scenario en kandidaatmaatregelen in. De Celery worker voert de berekening asynchroon uit.
        </p>
      </div>

      {prefill && (
        <div className="bg-amber-50 border border-amber-200 rounded p-3 text-sm text-amber-800">
          Parameters ingeladen vanuit de referentiedatabase. Controleer en pas aan waar nodig — met name P₀ kan afwijken van de actuele beoordelingsuitkomst.
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-8">
        {/* Traject */}
        <section className="space-y-4">
          <h2 className="text-lg font-semibold text-gray-800 border-b pb-1">Dijktraject</h2>
          <div className="grid grid-cols-2 gap-4">
            <Field label="ID">
              <input className={INPUT} value={trajectory.id}
                onChange={(e) => setTrajectory((t) => ({ ...t, id: e.target.value }))} required />
            </Field>
            <Field label="Norm (1/jaar, bijv. 0.001)">
              <input className={INPUT} type="number" step="any" value={trajectory.norm}
                onChange={(e) => setTrajectory((t) => ({ ...t, norm: parseFloat(e.target.value) }))} required />
            </Field>
            <Field label="Lengte (km)">
              <input className={INPUT} type="number" step="any" value={trajectory.length}
                onChange={(e) => setTrajectory((t) => ({ ...t, length: parseFloat(e.target.value) }))} required />
            </Field>
            <Field label="P₀ — faalkans basisjaar (1/jaar)">
              <input className={INPUT} type="number" step="any" value={trajectory.p0}
                onChange={(e) => setTrajectory((t) => ({ ...t, p0: parseFloat(e.target.value) }))} required />
            </Field>
            <Field label="α — schaalparameter (1/m)">
              <input className={INPUT} type="number" step="any" value={trajectory.alpha}
                onChange={(e) => setTrajectory((t) => ({ ...t, alpha: parseFloat(e.target.value) }))} required />
            </Field>
            <Field label="Basisjaar">
              <input className={INPUT} type="number" value={trajectory.base_year}
                onChange={(e) => setTrajectory((t) => ({ ...t, base_year: parseInt(e.target.value) }))} required />
            </Field>
          </div>
        </section>

        {/* Scenario */}
        <section className="space-y-4">
          <h2 className="text-lg font-semibold text-gray-800 border-b pb-1">Klimaatscenario</h2>
          <div className="grid grid-cols-2 gap-4">
            <Field label="ID">
              <input className={INPUT} value={scenario.id}
                onChange={(e) => setScenario((s) => ({ ...s, id: e.target.value }))} required />
            </Field>
            <Field label="Klimaatscenario (bijv. huidig, W+, WH)">
              <input className={INPUT} value={scenario.climate}
                onChange={(e) => setScenario((s) => ({ ...s, climate: e.target.value }))} required />
            </Field>
            <Field label="Ontwerpafvoer q (m³/s)">
              <input className={INPUT} type="number" step="any" value={scenario.q_design}
                onChange={(e) => setScenario((s) => ({ ...s, q_design: parseFloat(e.target.value) }))} required />
            </Field>
            <Field label="Ontwerpwaterstand h (m+NAP)">
              <input className={INPUT} type="number" step="any" value={scenario.h_design}
                onChange={(e) => setScenario((s) => ({ ...s, h_design: parseFloat(e.target.value) }))} required />
            </Field>
            <Field label="Klimaatstijging η (m/jaar)">
              <input className={INPUT} type="number" step="any" value={scenario.eta}
                onChange={(e) => setScenario((s) => ({ ...s, eta: parseFloat(e.target.value) }))} required />
            </Field>
          </div>
        </section>

        {/* Risicoparameters */}
        <section className="space-y-4">
          <h2 className="text-lg font-semibold text-gray-800 border-b pb-1">Risicoparameters</h2>
          <div className="grid grid-cols-2 gap-4">
            <Field label="Basisschade V₀ (€)">
              <input className={INPUT} type="number" step="any" value={risk.base_damage}
                onChange={(e) => setRisk((r) => ({ ...r, base_damage: parseFloat(e.target.value) }))} required />
            </Field>
            <Field label="Discontovoet δ (bijv. 0.04)">
              <input className={INPUT} type="number" step="any" value={risk.discount_rate}
                onChange={(e) => setRisk((r) => ({ ...r, discount_rate: parseFloat(e.target.value) }))} required />
            </Field>
            <Field label="Economische groei γ (bijv. 0.02)">
              <input className={INPUT} type="number" step="any" value={risk.gamma}
                onChange={(e) => setRisk((r) => ({ ...r, gamma: parseFloat(e.target.value) }))} required />
            </Field>
            <Field label="Tijdshorizon T (jaar)">
              <input className={INPUT} type="number" value={risk.time_horizon}
                onChange={(e) => setRisk((r) => ({ ...r, time_horizon: parseInt(e.target.value) }))} required />
            </Field>
          </div>
        </section>

        {/* Kandidaatmaatregelen */}
        <section className="space-y-3">
          <h2 className="text-lg font-semibold text-gray-800 border-b pb-1">
            Kandidaatmaatregelen ({candidates.length})
          </h2>
          {candidates.map((m, i) => (
            <div key={i} className="border rounded-lg p-4 bg-gray-50 space-y-3">
              <div className="flex justify-between items-center">
                <span className="font-mono text-sm font-semibold text-gray-700">{m.id || `#${i + 1}`}</span>
                <button type="button" onClick={() => removeMeasure(i)}
                  className="text-red-500 hover:text-red-700 text-sm">Verwijder</button>
              </div>
              <div className="grid grid-cols-3 gap-3">
                <Field label="ID">
                  <input className={INPUT} value={m.id}
                    onChange={(e) => updateMeasure(i, { id: e.target.value })} required />
                </Field>
                <Field label="Type">
                  <select className={SELECT} value={m.type}
                    onChange={(e) => updateMeasure(i, { type: e.target.value as MeasureType })}>
                    <option value="dike_reinforcement">Dijkversterking</option>
                    <option value="room_for_river">Ruimte voor Rivier</option>
                    <option value="other">Overig</option>
                  </select>
                </Field>
                <Field label="Locatie">
                  <input className={INPUT} value={m.location}
                    onChange={(e) => updateMeasure(i, { location: e.target.value })} required />
                </Field>
                <Field label="Effect Δh (m)">
                  <input className={INPUT} type="number" step="any" value={m.effect}
                    onChange={(e) => updateMeasure(i, { effect: parseFloat(e.target.value) })} required />
                </Field>
                <Field label="Kosten (€)">
                  <input className={INPUT} type="number" step="any" value={m.cost}
                    onChange={(e) => updateMeasure(i, { cost: parseFloat(e.target.value) })} required />
                </Field>
                <Field label="Uitvoeringsjaar">
                  <input className={INPUT} type="number" value={m.year}
                    onChange={(e) => updateMeasure(i, { year: parseInt(e.target.value) })} required />
                </Field>
              </div>
            </div>
          ))}
          <button type="button" onClick={addMeasure}
            className="text-sm text-blue-600 hover:underline">+ Maatregel toevoegen</button>
        </section>

        {/* Optimalisatie-instellingen */}
        <section className="space-y-4">
          <h2 className="text-lg font-semibold text-gray-800 border-b pb-1">Optimalisatie</h2>
          <div className="grid grid-cols-2 gap-4">
            <Field label="Doelfunctie">
              <select className={SELECT} value={objective}
                onChange={(e) => setObjective(e.target.value as ObjectiveType)}>
                <option value="min_cost">MIN_COST — minimale investering</option>
                <option value="min_ncw">MIN_NCW — minimale totale NCW</option>
                <option value="max_risk_reduction">MAX_RISK_REDUCTION — maximale risicoreductie</option>
              </select>
            </Field>
            <Field label="Solver">
              <select className={SELECT} value={solver}
                onChange={(e) => setSolver(e.target.value as 'brute_force' | 'pyomo')}>
                <option value="brute_force">BruteForce</option>
                <option value="pyomo">Pyomo / HiGHS</option>
              </select>
            </Field>
            {objective === 'max_risk_reduction' && (
              <Field label="Budget (€)">
                <input className={INPUT} type="number" step="any" value={budget}
                  onChange={(e) => setBudget(e.target.value)} required />
              </Field>
            )}
          </div>
        </section>

        {error && (
          <div className="bg-red-50 border border-red-200 rounded p-3 text-sm text-red-700">{error}</div>
        )}

        <button type="submit" disabled={loading}
          className="w-full bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white font-semibold py-2.5 rounded-lg transition-colors">
          {loading ? 'Bezig…' : 'Optimalisatie starten'}
        </button>
      </form>
    </div>
  )
}
