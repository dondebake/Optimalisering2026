import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { getValDijkringen, getValTrajectories, valOptimize } from '../api/client'
import type { ValTrajectory } from '../types'

function fmtNorm(n: number) {
  return `1/${Math.round(1 / n).toLocaleString('nl-NL')}`
}

function fmtSci(n: number) {
  return n.toExponential(2)
}

function TrajectRow({ t, onOptimize }: { t: ValTrajectory; onOptimize: (t: ValTrajectory) => void }) {
  const [loading, setLoading] = useState(false)

  async function handleClick() {
    setLoading(true)
    try { await onOptimize(t) } finally { setLoading(false) }
  }

  return (
    <tr className="hover:bg-gray-50 transition-colors">
      <td className="px-3 py-2 font-mono text-xs text-gray-500">{t.Dijkring}-{t.DijkringDeel}-{t.DijkringTraject}</td>
      <td className="px-3 py-2 text-sm">{t.Naam}</td>
      <td className="px-3 py-2 text-sm tabular-nums">{fmtNorm(t.norm_per_jaar)}/jr</td>
      <td className="px-3 py-2 text-sm tabular-nums">{fmtSci(t.p0_per_jaar)}</td>
      <td className="px-3 py-2 text-sm tabular-nums">{t.alpha_per_m.toFixed(2)}</td>
      <td className="px-3 py-2 text-sm tabular-nums">{t.eta_m_per_jaar.toFixed(4)}</td>
      <td className="px-3 py-2 text-right">
        <button
          onClick={handleClick}
          disabled={loading}
          className="text-xs px-2.5 py-1 rounded bg-blue-600 text-white hover:bg-blue-700 disabled:opacity-50 transition-colors"
        >
          {loading ? '…' : 'Optimaliseer'}
        </button>
      </td>
    </tr>
  )
}

export default function ValidationDashboard() {
  const navigate = useNavigate()
  const [selectedDijkring, setSelectedDijkring] = useState<string>('')

  const { data: dijkringen, isLoading: loadingDR } = useQuery({
    queryKey: ['val-dijkringen'],
    queryFn: getValDijkringen,
    staleTime: Infinity,
  })

  const { data: trajectories, isLoading: loadingTR } = useQuery({
    queryKey: ['val-trajectories', selectedDijkring],
    queryFn: () => getValTrajectories(selectedDijkring || undefined),
    staleTime: Infinity,
  })

  async function handleOptimize(t: ValTrajectory) {
    const job = await valOptimize(t.Dijkring, t.DijkringDeel, t.DijkringTraject)
    navigate(`/results/${job.job_id}`)
  }

  return (
    <div className="max-w-5xl mx-auto p-6 space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Validatie-dashboard</h1>
        <p className="text-sm text-gray-500 mt-1">
          OptimaliseRing 2011 referentiedata — 103 dijkringen, 176 trajecten (klimaat_id=1).
        </p>
      </div>

      {/* Dijkring filter */}
      <div className="flex items-center gap-3">
        <label className="text-sm font-medium text-gray-700">Dijkring</label>
        {loadingDR ? (
          <span className="text-sm text-gray-400 animate-pulse">laden…</span>
        ) : (
          <select
            className="border border-gray-300 rounded px-2 py-1.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
            value={selectedDijkring}
            onChange={(e) => setSelectedDijkring(e.target.value)}
          >
            <option value="">Alle dijkringen ({dijkringen?.length ?? 0})</option>
            {dijkringen?.map((d) => (
              <option key={d.Dijkring} value={d.Dijkring}>
                {d.Dijkring} — {d.Naam} (norm {fmtNorm(d.norm_per_jaar)}/jr, {d.n_trajecten} trajecten)
              </option>
            ))}
          </select>
        )}
        <span className="text-xs text-gray-400">
          {trajectories?.length ?? 0} trajecten getoond
        </span>
      </div>

      {/* Trajecten tabel */}
      <div className="overflow-x-auto rounded-xl border border-gray-200">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 text-xs text-gray-500 uppercase tracking-wide">
            <tr>
              <th className="text-left px-3 py-2.5">ID</th>
              <th className="text-left px-3 py-2.5">Naam</th>
              <th className="text-left px-3 py-2.5">Norm</th>
              <th className="text-left px-3 py-2.5">P₀ [1/jr]</th>
              <th className="text-left px-3 py-2.5">α [1/m]</th>
              <th className="text-left px-3 py-2.5">η [m/jr]</th>
              <th className="px-3 py-2.5" />
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {loadingTR ? (
              <tr><td colSpan={7} className="px-3 py-4 text-center text-gray-400 animate-pulse text-sm">Laden…</td></tr>
            ) : trajectories?.length === 0 ? (
              <tr><td colSpan={7} className="px-3 py-4 text-center text-gray-400 text-sm">Geen trajecten</td></tr>
            ) : (
              trajectories?.map((t) => (
                <TrajectRow
                  key={`${t.Dijkring}-${t.DijkringDeel}-${t.DijkringTraject}`}
                  t={t}
                  onOptimize={handleOptimize}
                />
              ))
            )}
          </tbody>
        </table>
      </div>

      <p className="text-xs text-gray-400">
        "Optimaliseer" gebruikt 3 standaard maatregelen (Δh 0.5/1.0/1.5 m) en MIN_COST.
        Resultaten worden opgeslagen en zijn zichtbaar in de joblijst op het Dashboard.
      </p>
    </div>
  )
}
