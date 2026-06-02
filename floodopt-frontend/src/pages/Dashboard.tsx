import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useQuery, useQueryClient } from '@tanstack/react-query'
import MapView, { P_CLASSES, pColor } from '../components/MapView'
import { StatusBadge } from '../components/StatusBadge'
import { getAllResults, deleteResult } from '../api/client'

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
  min_cost: 'MIN_COST',
  min_ncw: 'MIN_NCW',
  max_risk_reduction: 'MAX_RR',
}

// ── Kaart-tab inhoud ──────────────────────────────────────────────────────────

function MapPanel() {
  return (
    <div className="space-y-6">
      {/* Legenda */}
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

      {/* Navigatie */}
      <div className="space-y-1.5">
        <div className="text-xs font-semibold text-gray-500 uppercase tracking-wide">Navigatie</div>
        <Link to="/optimize"
          className="flex items-center gap-2 text-sm text-blue-600 hover:text-blue-800 py-1">
          ⚡ Nieuwe optimalisatie
        </Link>
        <Link to="/validatie"
          className="flex items-center gap-2 text-sm text-gray-600 hover:text-gray-900 py-1">
          📋 Validatie-dashboard
        </Link>
      </div>

      {/* Stack */}
      <div className="space-y-1.5">
        <div className="text-xs font-semibold text-gray-500 uppercase tracking-wide">Stack</div>
        <div className="text-xs space-y-1 text-gray-500">
          <div className="flex justify-between">
            <span>FastAPI</span>
            <a href="http://localhost:8000/docs" target="_blank" rel="noopener noreferrer"
              className="text-blue-500 hover:underline">docs ↗</a>
          </div>
          <div className="flex justify-between">
            <span>Redis</span>
            <span className="text-gray-400">:6379</span>
          </div>
          <div className="flex justify-between">
            <span>Celery</span>
            <span className="text-gray-400">--pool=solo</span>
          </div>
        </div>
      </div>
    </div>
  )
}

// ── Runs-tab inhoud ───────────────────────────────────────────────────────────

function RunsPanel() {
  const qc = useQueryClient()

  const { data, isLoading } = useQuery({
    queryKey: ['all-results'],
    queryFn: getAllResults,
    refetchInterval: (query) => {
      const jobs = query.state.data ?? []
      const active = jobs.some(j => j.status === 'pending' || j.status === 'running')
      return active ? 2000 : 15_000
    },
  })

  async function handleDelete(jobId: string) {
    if (!window.confirm('Job verwijderen?')) return
    await deleteResult(jobId)
    qc.invalidateQueries({ queryKey: ['all-results'] })
  }

  if (isLoading) {
    return <p className="text-xs text-gray-400 animate-pulse">Laden…</p>
  }

  if (!data || data.length === 0) {
    return (
      <div className="space-y-3">
        <p className="text-xs text-gray-400">Nog geen berekeningen.</p>
        <Link to="/optimize"
          className="block text-sm text-blue-600 hover:text-blue-800">
          ⚡ Nieuwe optimalisatie →
        </Link>
      </div>
    )
  }

  return (
    <div className="space-y-2">
      <div className="flex items-center justify-between">
        <span className="text-xs font-semibold text-gray-500 uppercase tracking-wide">
          {data.length} run{data.length !== 1 ? 's' : ''}
        </span>
        <Link to="/optimize" className="text-xs text-blue-600 hover:underline">+ Nieuw</Link>
      </div>

      <div className="space-y-1.5">
        {data.map((job) => (
          <div key={job.job_id}
            className="border border-gray-100 rounded-lg p-2.5 hover:border-gray-300 transition-colors">
            <div className="flex items-start justify-between gap-1 mb-1.5">
              <div className="min-w-0">
                <div className="text-xs font-medium text-gray-800 truncate">
                  {job.trajectory_id}
                </div>
                <div className="text-xs text-gray-400">
                  {OBJ_SHORT[job.objective] ?? job.objective} · {job.solver}
                </div>
              </div>
              <StatusBadge status={job.status} />
            </div>

            {job.status === 'done' && job.objective_value != null && (
              <div className="text-xs text-blue-700 font-mono mb-1.5">
                {fmtEur(job.objective_value)}
              </div>
            )}

            <div className="flex items-center justify-between">
              <Link to={`/results/${job.job_id}`}
                className="text-xs text-blue-600 hover:underline">
                Bekijk →
              </Link>
              <button
                onClick={() => handleDelete(job.job_id)}
                className="text-xs text-gray-300 hover:text-red-500 transition-colors"
                title="Verwijder"
              >
                ✕
              </button>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

// ── Feature detail paneel (rechts) ────────────────────────────────────────────

function FeaturePanel({
  props,
  onClose,
}: {
  props: Record<string, unknown>
  onClose: () => void
}) {
  const navigate = useNavigate()
  const naam = (props.naam ?? props.id ?? 'Onbekend') as string
  const p0 = (props.p0_avg ?? props.p0_max ?? props.p_year) as number | null
  const norm = (props.norm_per_jaar ?? props.norm) as number | null
  const dijkringNr = props.dijkringnummer as string | null
  const dijkringDeel = props.dijkringdeel as number | null

  return (
    <div className="space-y-4">
      <div className="flex items-start justify-between">
        <div>
          <div className="font-semibold text-gray-900 text-sm">{naam}</div>
          {dijkringNr && (
            <div className="text-xs text-gray-400">
              Dijkring {dijkringNr}{dijkringDeel ? `, deel ${dijkringDeel}` : ''}
            </div>
          )}
        </div>
        <button onClick={onClose}
          className="text-gray-400 hover:text-gray-600 text-lg leading-none">×</button>
      </div>

      <div className="space-y-2 text-sm">
        <div className="flex justify-between">
          <span className="text-gray-500">P₀ (gemiddeld)</span>
          <div className="flex items-center gap-1.5">
            {p0 != null && (
              <span className="inline-block w-3 h-3 rounded-full"
                style={{ backgroundColor: pColor(p0) }} />
            )}
            <span className="font-mono text-xs">{fmtP(p0)}</span>
          </div>
        </div>
        <div className="flex justify-between">
          <span className="text-gray-500">Norm</span>
          <span className="font-mono text-xs">{fmtP(norm)}</span>
        </div>
        {props.n_trajecten != null && (
          <div className="flex justify-between">
            <span className="text-gray-500">Trajecten</span>
            <span className="text-xs">{String(props.n_trajecten)}</span>
          </div>
        )}
        {props.naam_water && (
          <div className="flex justify-between gap-2">
            <span className="text-gray-500 flex-shrink-0">Waterlichaam</span>
            <span className="text-xs text-right">{String(props.naam_water)}</span>
          </div>
        )}
      </div>

      {dijkringNr && (
        <button
          onClick={() => navigate('/validatie', { state: { highlightDijkring: dijkringNr } })}
          className="w-full text-sm px-3 py-2 rounded bg-blue-600 text-white hover:bg-blue-700 transition-colors"
        >
          Optimaliseer trajecten →
        </button>
      )}
    </div>
  )
}

// ── Dashboard ─────────────────────────────────────────────────────────────────

type Tab = 'kaart' | 'runs'

export default function Dashboard() {
  const [activeTab, setActiveTab] = useState<Tab>('kaart')
  const [selectedFeature, setSelectedFeature] = useState<Record<string, unknown> | null>(null)

  return (
    <div className="flex h-full">

      {/* ── Links paneel ── */}
      <aside className="w-64 flex-shrink-0 border-r bg-white flex flex-col overflow-hidden">

        {/* Tabs */}
        <div className="flex border-b flex-shrink-0">
          {(['kaart', 'runs'] as Tab[]).map((tab) => (
            <button
              key={tab}
              onClick={() => setActiveTab(tab)}
              className={`flex-1 py-2.5 text-xs font-semibold transition-colors ${
                activeTab === tab
                  ? 'border-b-2 border-blue-600 text-blue-700 bg-white'
                  : 'text-gray-500 hover:text-gray-700'
              }`}
            >
              {tab === 'kaart' ? 'Kaart' : 'Runs'}
            </button>
          ))}
        </div>

        {/* Tab inhoud */}
        <div className="flex-1 overflow-y-auto p-4">
          {activeTab === 'kaart' ? <MapPanel /> : <RunsPanel />}
        </div>

        <div className="p-3 border-t flex-shrink-0 text-xs text-gray-400">
          {activeTab === 'kaart'
            ? 'Kleur = P₀ (2011 ref.) · Klik op traject voor details'
            : 'Klik op een run om de resultaten te bekijken'}
        </div>
      </aside>

      {/* ── Kaart ── */}
      <div className="flex-1 min-w-0 relative">
        <MapView onFeatureClick={setSelectedFeature} />
      </div>

      {/* ── Rechts paneel ── */}
      {selectedFeature && (
        <aside className="w-72 flex-shrink-0 border-l bg-white overflow-y-auto p-4">
          <FeaturePanel props={selectedFeature} onClose={() => setSelectedFeature(null)} />
        </aside>
      )}

    </div>
  )
}
