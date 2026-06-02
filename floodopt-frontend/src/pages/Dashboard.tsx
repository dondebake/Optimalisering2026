import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useQuery, useQueryClient } from '@tanstack/react-query'
import MapView, { P_CLASSES, pColor } from '../components/MapView'
import { StatusBadge } from '../components/StatusBadge'
import { getAllResults, getValTrajectories, getValReferenceData, deleteResult } from '../api/client'
import type { ValTrajectory, ValReferenceData, SchadeScenario, GammaScenario } from '../types'

// ── Helpers ───────────────────────────────────────────────────────────────────

function fmtP(p: number | null | undefined) {
  if (p == null) return '—'
  return `1/${Math.round(1 / p).toLocaleString('nl-NL')}/jr`
}

function fmtEur(n: number | null) {
  if (n == null) return '—'
  return `€ ${(n / 1e6).toLocaleString('nl-NL', { maximumFractionDigits: 0 })} M`
}

const OBJ_SHORT: Record<string, string> = {
  min_cost: 'MIN_COST', min_ncw: 'MIN_NCW', max_risk_reduction: 'MAX_RR',
}

// ── Links paneel — alleen legenda ─────────────────────────────────────────────

function MapLegendPanel() {
  return (
    <div className="space-y-1.5">
      <div className="text-xs font-semibold text-gray-500 uppercase tracking-wide">
        Overstromingskans P₀
      </div>
      {P_CLASSES.map((cls) => (
        <div key={cls.label} className="flex items-center gap-2 text-xs text-gray-700">
          <span className="inline-block w-5 h-2.5 rounded-sm flex-shrink-0"
            style={{ backgroundColor: cls.color }} />
          {cls.label}
        </div>
      ))}
      <div className="flex items-center gap-2 text-xs text-gray-400 pt-1 border-t border-gray-100">
        <span className="inline-block w-5 h-2.5 rounded-sm flex-shrink-0 bg-slate-400" />
        Geen data
      </div>
    </div>
  )
}

// ── Optimize modal ────────────────────────────────────────────────────────────

function buildCandidates(t: ValTrajectory) {
  const dhs = [0.25, 0.5, 0.75, 1.0, 1.5]
  const C = t.C_exp ?? 0, lam = t.lambda_exp_per_m ?? 0, b = t.b_exp ?? 1
  return dhs.map((dh, i) => ({
    id: `M${String(i + 1).padStart(2, '0')}`,
    type: 'dike_reinforcement' as const,
    cost: Math.round((C > 0 ? C * Math.exp(lam * dh) * Math.pow(dh, b) : dh * 5) * 1_000_000),
    year: 2028 + i * 5,
    effect: dh,
    location: `vak-${i + 1}`,
    dependencies: [],
  }))
}

function OptimizeModal({
  traj,
  refData,
  onClose,
}: {
  traj: ValTrajectory
  refData: ValReferenceData
  onClose: () => void
}) {
  const navigate = useNavigate()
  const defaultSchade = refData.schade_scenarios.find(s => s.scenario_naam === 'Verwacht')
    ?? refData.schade_scenarios[0]
  const defaultGamma = refData.gamma_scenarios.find(g => g.scenario_naam.startsWith('Transatlantic'))
    ?? refData.gamma_scenarios[0]

  const [schadeId, setSchadeId] = useState<number>(defaultSchade?.scenario_id ?? 2)
  const [gammaId, setGammaId] = useState<number>(defaultGamma?.scenario_id ?? 3)

  const sel = refData.schade_scenarios.find(s => s.scenario_id === schadeId)
  const selG = refData.gamma_scenarios.find(g => g.scenario_id === gammaId)

  function confirm() {
    if (!sel || !selG) return
    onClose()
    navigate('/optimize', {
      state: {
        trajectory: {
          id: `ref-${traj.Dijkring}-${traj.DijkringDeel}-${traj.DijkringTraject}`,
          norm: traj.norm_per_jaar, length: 10.0,
          p0: traj.p0_per_jaar, alpha: traj.alpha_per_m,
          base_year: 2015, measures: [],
        },
        scenario: {
          id: `ref-scen-${traj.Dijkring}`,
          climate: 'huidig', q_design: 1000.0, h_design: 5.0,
          eta: traj.eta_m_per_jaar,
        },
        candidates: buildCandidates(traj),
        risk_params: {
          base_damage: sel.schade_meur * 1_000_000,
          discount_rate: 0.04,
          gamma: selG.gamma,
          time_horizon: 100,
        },
      },
    })
  }

  return (
    <div className="fixed inset-0 bg-black/40 z-[2000] flex items-center justify-center p-4">
      <div className="bg-white rounded-xl shadow-xl w-full max-w-lg p-6 space-y-5">
        <div className="flex items-start justify-between">
          <div>
            <h2 className="font-bold text-gray-900">Nieuwe berekening</h2>
            <p className="text-xs text-gray-500 mt-0.5">
              {traj.Naam} · {traj.Dijkring}-{traj.DijkringDeel}-{traj.DijkringTraject}
            </p>
          </div>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600 text-xl">×</button>
        </div>

        {/* Schade */}
        <div className="space-y-1.5">
          <div className="text-sm font-semibold text-gray-700">V₀ — basisschade bij overstroming</div>
          {refData.schade_scenarios.length === 0
            ? <p className="text-xs text-amber-600">Geen schadedata beschikbaar.</p>
            : refData.schade_scenarios.map(s => (
              <label key={s.scenario_id} className="flex items-center gap-3 p-2 rounded cursor-pointer hover:bg-gray-50">
                <input type="radio" name="schade" checked={schadeId === s.scenario_id}
                  onChange={() => setSchadeId(s.scenario_id)} className="accent-blue-600" />
                <span className="text-sm font-medium w-20">{s.scenario_naam}</span>
                <span className="text-sm font-mono text-blue-700">
                  € {(s.schade_meur * 1e6).toLocaleString('nl-NL', { maximumFractionDigits: 0 })}
                </span>
              </label>
            ))}
        </div>

        {/* Gamma */}
        <div className="space-y-1.5">
          <div className="text-sm font-semibold text-gray-700">γ — economische groeivoet</div>
          <div className="max-h-36 overflow-y-auto space-y-0.5">
            {refData.gamma_scenarios.map(g => (
              <label key={g.scenario_id} className="flex items-center gap-3 p-2 rounded cursor-pointer hover:bg-gray-50">
                <input type="radio" name="gamma" checked={gammaId === g.scenario_id}
                  onChange={() => setGammaId(g.scenario_id)} className="accent-blue-600" />
                <span className="text-xs w-44 truncate" title={g.scenario_naam}>{g.scenario_naam}</span>
                <span className="text-xs font-mono text-blue-700">γ = {(g.gamma * 100).toFixed(1)} %</span>
              </label>
            ))}
          </div>
        </div>

        {/* Samenvatting */}
        {sel && selG && (
          <div className="bg-blue-50 rounded p-3 text-sm flex justify-between">
            <span>V₀ = € {(sel.schade_meur * 1e6).toLocaleString('nl-NL', { maximumFractionDigits: 0 })}</span>
            <span>γ = {(selG.gamma * 100).toFixed(1)} %</span>
          </div>
        )}

        <div className="flex gap-3">
          <button onClick={confirm} disabled={!sel || !selG}
            className="flex-1 bg-blue-600 text-white text-sm py-2.5 rounded-lg hover:bg-blue-700 disabled:opacity-40 transition-colors">
            Naar OptimizeForm →
          </button>
          <button onClick={onClose} className="px-4 text-sm text-gray-500 hover:text-gray-700">
            Annuleer
          </button>
        </div>
      </div>
    </div>
  )
}

// ── Rechts paneel — trajectdetails + runs + modal-trigger ─────────────────────

function TrajectoryPanel({
  props,
  onClose,
}: {
  props: Record<string, unknown>
  onClose: () => void
}) {
  const qc = useQueryClient()
  const dijkringNr = props.dijkringnummer as string | null
  const dijkringDeel = props.dijkringdeel as number | null
  const naam = (props.naam ?? '') as string
  const p0 = (props.p0_avg ?? props.p0_max) as number | null
  const norm = props.norm_per_jaar as number | null

  // State voor modal
  const [pendingTraj, setPendingTraj] = useState<ValTrajectory | null>(null)
  const [pendingRef, setPendingRef] = useState<ValReferenceData | null>(null)
  const [loadingModal, setLoadingModal] = useState(false)

  // Trajecten voor dit dijkring
  const { data: trajecten } = useQuery({
    queryKey: ['val-traj-panel', dijkringNr],
    queryFn: () => getValTrajectories(dijkringNr!),
    enabled: !!dijkringNr,
    staleTime: Infinity,
  })

  // Alle runs, gefilterd op dit dijkring
  const { data: allRuns } = useQuery({
    queryKey: ['all-results'],
    queryFn: getAllResults,
    refetchInterval: (q) => {
      const jobs = q.state.data ?? []
      return jobs.some(j => j.status === 'pending' || j.status === 'running') ? 2000 : 15_000
    },
  })

  const dijkringRuns = (allRuns ?? []).filter(j =>
    dijkringNr && j.trajectory_id.includes(`-${dijkringNr}-`)
  )

  async function handleDeleteRun(jobId: string) {
    if (!window.confirm('Run verwijderen?')) return
    await deleteResult(jobId)
    qc.invalidateQueries({ queryKey: ['all-results'] })
  }

  async function handleNewRun(t: ValTrajectory) {
    setLoadingModal(true)
    try {
      const ref = await getValReferenceData(t.Dijkring, t.DijkringDeel)
      setPendingTraj(t)
      setPendingRef(ref)
    } finally {
      setLoadingModal(false)
    }
  }

  return (
    <>
      <div className="flex flex-col h-full">
        {/* Header */}
        <div className="p-4 border-b flex-shrink-0">
          <div className="flex items-start justify-between">
            <div className="min-w-0">
              <div className="font-semibold text-gray-900 text-sm truncate">{naam}</div>
              <div className="text-xs text-gray-400 mt-0.5">
                Dijkring {dijkringNr ?? '—'}{dijkringDeel ? `, deel ${dijkringDeel}` : ''}
              </div>
            </div>
            <button onClick={onClose} className="text-gray-400 hover:text-gray-600 ml-2 text-lg leading-none flex-shrink-0">×</button>
          </div>
          <div className="mt-3 grid grid-cols-2 gap-2 text-xs">
            <div className="bg-gray-50 rounded p-2">
              <div className="text-gray-400">P₀ gem.</div>
              <div className="font-mono font-medium">{fmtP(p0)}</div>
            </div>
            <div className="bg-gray-50 rounded p-2">
              <div className="text-gray-400">Norm</div>
              <div className="font-mono font-medium">{fmtP(norm)}</div>
            </div>
          </div>
        </div>

        {/* Trajecten + runs */}
        <div className="flex-1 overflow-y-auto p-4 space-y-4">

          {/* Runs voor dit dijkring */}
          <div className="space-y-2">
            <div className="flex items-center justify-between">
              <div className="text-xs font-semibold text-gray-500 uppercase tracking-wide">
                Berekeningen ({dijkringRuns.length})
              </div>
              <Link to="/runs" className="text-xs text-blue-500 hover:underline">
                Alle runs →
              </Link>
            </div>

            {dijkringRuns.length === 0 ? (
              <p className="text-xs text-gray-400">Geen berekeningen voor dit trajectdeel.</p>
            ) : (
              <div className="space-y-1.5">
                {dijkringRuns.map(job => (
                  <div key={job.job_id} className="border border-gray-100 rounded-lg p-2.5">
                    <div className="flex items-center justify-between mb-1">
                      <span className="text-xs text-gray-600 font-mono truncate max-w-[120px]">
                        {job.trajectory_id}
                      </span>
                      <StatusBadge status={job.status} />
                    </div>
                    <div className="text-xs text-gray-400 mb-1.5">
                      {OBJ_SHORT[job.objective] ?? job.objective} · {job.solver}
                    </div>
                    {job.status === 'done' && (
                      <div className="text-xs font-medium text-blue-700 mb-1.5">
                        {fmtEur(job.objective_value)}
                      </div>
                    )}
                    <div className="flex justify-between">
                      <Link to={`/results/${job.job_id}`}
                        className="text-xs text-blue-600 hover:underline">Bekijk →</Link>
                      <button onClick={() => handleDeleteRun(job.job_id)}
                        className="text-xs text-gray-300 hover:text-red-500">✕</button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Trajecten om te optimaliseren */}
          {dijkringNr && trajecten && trajecten.length > 0 && (
            <div className="space-y-2">
              <div className="text-xs font-semibold text-gray-500 uppercase tracking-wide">
                Nieuwe berekening
              </div>
              {trajecten.map(t => (
                <button
                  key={`${t.DijkringDeel}-${t.DijkringTraject}`}
                  onClick={() => handleNewRun(t)}
                  disabled={loadingModal}
                  className="w-full text-left border border-gray-200 rounded-lg p-2.5 hover:border-blue-400 hover:bg-blue-50 transition-colors disabled:opacity-50"
                >
                  <div className="text-xs font-medium text-gray-800">
                    {t.Dijkring}-{t.DijkringDeel}-{t.DijkringTraject}
                  </div>
                  <div className="text-xs text-gray-500 truncate">{t.Naam}</div>
                  <div className="text-xs text-gray-400 mt-0.5">
                    P₀={t.p0_per_jaar.toExponential(1)} · α={t.alpha_per_m.toFixed(1)} · norm {fmtP(t.norm_per_jaar)}
                  </div>
                </button>
              ))}
              {loadingModal && (
                <p className="text-xs text-gray-400 animate-pulse">Referentiedata ophalen…</p>
              )}
            </div>
          )}
        </div>
      </div>

      {/* Optimize modal */}
      {pendingTraj && pendingRef && (
        <OptimizeModal
          traj={pendingTraj}
          refData={pendingRef}
          onClose={() => { setPendingTraj(null); setPendingRef(null) }}
        />
      )}
    </>
  )
}

// ── Dashboard ─────────────────────────────────────────────────────────────────

export default function Dashboard() {
  const [selectedFeature, setSelectedFeature] = useState<Record<string, unknown> | null>(null)

  return (
    <div className="flex h-full">

      {/* ── Links paneel — legenda ── */}
      <aside className="w-56 flex-shrink-0 border-r bg-white flex flex-col overflow-hidden">
        <div className="p-4 border-b flex-shrink-0">
          <div className="font-bold text-gray-900 text-sm">FloodOpt</div>
          <div className="text-xs text-gray-400 mt-0.5">Dijkversterkingsoptimalisatie</div>
        </div>
        <div className="flex-1 overflow-y-auto p-4">
          <MapLegendPanel />
        </div>
        <div className="p-3 border-t flex-shrink-0 text-xs text-gray-400">
          Klik op een traject voor details
        </div>
      </aside>

      {/* ── Kaart ── */}
      <div className="flex-1 min-w-0 relative">
        <MapView onFeatureClick={setSelectedFeature} />
      </div>

      {/* ── Rechts paneel — trajectdetails + runs ── */}
      {selectedFeature && (
        <aside className="w-80 flex-shrink-0 border-l bg-white overflow-hidden flex flex-col">
          <TrajectoryPanel
            props={selectedFeature}
            onClose={() => setSelectedFeature(null)}
          />
        </aside>
      )}

    </div>
  )
}
