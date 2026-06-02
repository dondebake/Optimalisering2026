import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { getValDijkringen, getValTrajectories } from '../api/client'
import type { ValTrajectory } from '../types'

// IC(Dh) = C_exp * exp(lambda * Dh) * Dh^b  [M EUR]  — OptimaliseRing 2.3.2
function costEur(t: ValTrajectory, dh: number): number {
  if (t.C_exp == null) return dh * 5_000_000  // fallback als kostenfunctie ontbreekt
  const lambda = t.lambda_exp_per_m ?? 0
  const b = t.b_exp ?? 1
  return t.C_exp * Math.exp(lambda * dh) * Math.pow(dh, b) * 1_000_000
}

function buildCandidates(t: ValTrajectory) {
  const dhs = [0.25, 0.5, 0.75, 1.0, 1.5]
  const baseYear = 2023
  return dhs.map((dh, i) => ({
    id: `M${String(i + 1).padStart(2, '0')}`,
    type: 'dike_reinforcement' as const,
    cost: Math.round(costEur(t, dh)),
    year: baseYear + 5 + i * 5,
    effect: dh,
    location: `vak-${i + 1}`,
    dependencies: [],
  }))
}

function fmtNorm(n: number) {
  return `1/${Math.round(1 / n).toLocaleString('nl-NL')}`
}

function fmtSci(n: number) {
  return n.toExponential(2)
}

function TrajectRow({ t, onOptimize }: { t: ValTrajectory; onOptimize: (t: ValTrajectory) => void }) {
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
          onClick={() => onOptimize(t)}
          className="text-xs px-2.5 py-1 rounded bg-blue-600 text-white hover:bg-blue-700 transition-colors"
        >
          Optimaliseer →
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

  function handleOptimize(t: ValTrajectory) {
    navigate('/optimize', {
      state: {
        trajectory: {
          id: `ref-${t.Dijkring}-${t.DijkringDeel}-${t.DijkringTraject}`,
          norm: t.norm_per_jaar,
          length: 10.0,
          p0: t.p0_per_jaar,
          alpha: t.alpha_per_m,
          base_year: 2023,
          measures: [],
        },
        scenario: {
          id: `ref-scen-${t.Dijkring}`,
          climate: 'huidig',
          q_design: 1000.0,
          h_design: 5.0,
          eta: t.eta_m_per_jaar,
        },
        candidates: buildCandidates(t),
      },
    })
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
              <th className="text-left px-3 py-2.5">eta [m/jr]</th>
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

      <div className="bg-amber-50 border border-amber-200 rounded p-3 text-xs text-amber-800 space-y-1">
        <div className="font-semibold">Over de referentiewaarden</div>
        <div>P₀-waarden zijn afkomstig uit de OptimaliseRing 2011-database (testbed). De WBI2023-beoordelingsresultaten staan ter discussie — in de lopende beoordelingsronde moeten kansen voor veel trajecten aanzienlijk omlaag. Klik <strong>Optimaliseer →</strong> om het formulier te openen en P₀ (en alle andere parameters) aan te passen vóór de berekening.</div>
      </div>
    </div>
  )
}
