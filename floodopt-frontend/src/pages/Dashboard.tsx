import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import MapView, { P_CLASSES, pColor } from '../components/MapView'

function fmtP(p: number | null | undefined) {
  if (p == null) return '—'
  return `1/${Math.round(1 / p).toLocaleString('nl-NL')}/jr`
}

function Legend() {
  return (
    <div className="space-y-1.5">
      <div className="text-xs font-semibold text-gray-500 uppercase tracking-wide">
        Overstromingskans P₀
      </div>
      {P_CLASSES.map((cls) => (
        <div key={cls.label} className="flex items-center gap-2 text-xs text-gray-700">
          <span className="inline-block w-5 h-2.5 rounded-sm flex-shrink-0" style={{ backgroundColor: cls.color }} />
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

  function handleOptimize() {
    if (dijkringNr) {
      navigate('/validatie', { state: { highlightDijkring: dijkringNr } })
    } else {
      navigate('/optimize')
    }
  }

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
        <button onClick={onClose} className="text-gray-400 hover:text-gray-600 text-lg leading-none">×</button>
      </div>

      <div className="space-y-2 text-sm">
        <div className="flex justify-between">
          <span className="text-gray-500">P₀ (gemiddeld)</span>
          <div className="flex items-center gap-1.5">
            {p0 != null && (
              <span className="inline-block w-3 h-3 rounded-full" style={{ backgroundColor: pColor(p0) }} />
            )}
            <span className="font-mono">{fmtP(p0)}</span>
          </div>
        </div>
        <div className="flex justify-between">
          <span className="text-gray-500">Wettelijke norm</span>
          <span className="font-mono">{fmtP(norm)}</span>
        </div>
        {props.n_trajecten != null && (
          <div className="flex justify-between">
            <span className="text-gray-500">Trajecten</span>
            <span>{String(props.n_trajecten)}</span>
          </div>
        )}
        {props.naam_water && (
          <div className="flex justify-between">
            <span className="text-gray-500">Waterlichaam</span>
            <span className="text-right text-xs max-w-[150px]">{String(props.naam_water)}</span>
          </div>
        )}
      </div>

      {dijkringNr && (
        <button
          onClick={handleOptimize}
          className="w-full text-sm px-3 py-2 rounded bg-blue-600 text-white hover:bg-blue-700 transition-colors"
        >
          Optimaliseer trajecten →
        </button>
      )}
    </div>
  )
}

export default function Dashboard() {
  const [selectedFeature, setSelectedFeature] = useState<Record<string, unknown> | null>(null)

  return (
    <div className="flex h-full">

      {/* ── Links paneel ── */}
      <aside className="w-64 flex-shrink-0 border-r bg-white flex flex-col overflow-hidden">
        <div className="p-4 border-b flex-shrink-0">
          <div className="font-bold text-gray-900">FloodOpt</div>
          <div className="text-xs text-gray-400 mt-0.5">Dijkversterkingsoptimalisatie</div>
        </div>

        <div className="flex-1 overflow-y-auto p-4 space-y-6">
          <Legend />

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

        <div className="p-3 border-t flex-shrink-0 text-xs text-gray-400">
          Kleur = P₀ (2011 ref.) · Klik op traject voor details
        </div>
      </aside>

      {/* ── Kaart ── */}
      <div className="flex-1 min-w-0 relative">
        <MapView onFeatureClick={setSelectedFeature} />
      </div>

      {/* ── Rechts paneel (conditoneel) ── */}
      {selectedFeature && (
        <aside className="w-72 flex-shrink-0 border-l bg-white overflow-y-auto p-4">
          <FeaturePanel props={selectedFeature} onClose={() => setSelectedFeature(null)} />
        </aside>
      )}

    </div>
  )
}
