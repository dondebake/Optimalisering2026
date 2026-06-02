import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { getValDijkringen, getValTrajectories, getValReferenceData } from '../api/client'
import type { ValTrajectory, ValReferenceData } from '../types'

function fmtNorm(n: number) {
  return `1/${Math.round(1 / n).toLocaleString('nl-NL')}`
}

function fmtSci(n: number) {
  return n.toExponential(2)
}

// IC(Dh) = C_exp * exp(lambda * Dh) * Dh^b  [M EUR]  — OptimaliseRing 2.3.2
function costEur(t: ValTrajectory, dh: number): number {
  if (t.C_exp == null) return dh * 5_000_000
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

// ── Scenario-selectie panel ──────────────────────────────────────────────────

function ScenarioPanel({
  traj,
  ref,
  onConfirm,
  onCancel,
}: {
  traj: ValTrajectory
  ref: ValReferenceData
  onConfirm: (v0_eur: number, gamma: number) => void
  onCancel: () => void
}) {
  const [schadeId, setSchadeId] = useState<number>(
    ref.schade_scenarios.find(s => s.scenario_naam === 'Verwacht')?.scenario_id ?? ref.schade_scenarios[0]?.scenario_id ?? 2
  )
  const [gammaId, setGammaId] = useState<number>(
    ref.gamma_scenarios.find(g => g.scenario_naam.startsWith('Transatlantic'))?.scenario_id ?? ref.gamma_scenarios[0]?.scenario_id ?? 3
  )

  const selectedSchade = ref.schade_scenarios.find(s => s.scenario_id === schadeId)
  const selectedGamma = ref.gamma_scenarios.find(g => g.scenario_id === gammaId)

  return (
    <div className="fixed inset-0 bg-black/40 z-[2000] flex items-center justify-center p-4">
      <div className="bg-white rounded-xl shadow-xl w-full max-w-lg p-6 space-y-5">
        <div>
          <h2 className="font-bold text-gray-900">Scenarioselectie</h2>
          <p className="text-sm text-gray-500 mt-0.5">
            {traj.Dijkring}-{traj.DijkringDeel}-{traj.DijkringTraject} · {traj.Naam}
          </p>
        </div>

        {/* Schade scenario */}
        <div className="space-y-2">
          <div className="text-sm font-semibold text-gray-700">
            Schadescenario — V₀ (basisschade bij overstroming)
          </div>
          <div className="space-y-1">
            {ref.schade_scenarios.map(s => (
              <label key={s.scenario_id} className="flex items-center gap-3 cursor-pointer p-2 rounded hover:bg-gray-50">
                <input
                  type="radio"
                  name="schade"
                  value={s.scenario_id}
                  checked={schadeId === s.scenario_id}
                  onChange={() => setSchadeId(s.scenario_id)}
                  className="accent-blue-600"
                />
                <span className="text-sm text-gray-800 font-medium w-20">{s.scenario_naam}</span>
                <span className="text-sm font-mono text-blue-700">
                  € {(s.schade_meur * 1e6).toLocaleString('nl-NL', { maximumFractionDigits: 0 })}
                </span>
                <span className="text-xs text-gray-400">({s.schade_meur.toLocaleString('nl-NL')} M€)</span>
              </label>
            ))}
          </div>
          {ref.schade_scenarios.length === 0 && (
            <p className="text-xs text-amber-600">Geen schadedata beschikbaar voor dit dijkringdeel.</p>
          )}
        </div>

        {/* Gamma scenario */}
        <div className="space-y-2">
          <div className="text-sm font-semibold text-gray-700">
            Economisch scenario — γ (economische groeivoet)
          </div>
          <div className="space-y-1 max-h-44 overflow-y-auto">
            {ref.gamma_scenarios.map(g => (
              <label key={g.scenario_id} className="flex items-center gap-3 cursor-pointer p-2 rounded hover:bg-gray-50">
                <input
                  type="radio"
                  name="gamma"
                  value={g.scenario_id}
                  checked={gammaId === g.scenario_id}
                  onChange={() => setGammaId(g.scenario_id)}
                  className="accent-blue-600"
                />
                <span className="text-sm text-gray-800 font-medium w-48 truncate" title={g.scenario_naam}>{g.scenario_naam}</span>
                <span className="text-sm font-mono text-blue-700">γ = {(g.gamma * 100).toFixed(1)} %</span>
              </label>
            ))}
          </div>
        </div>

        {/* Samenvatting */}
        <div className="bg-blue-50 rounded p-3 text-sm space-y-1">
          <div className="flex justify-between">
            <span className="text-gray-600">V₀ (basisschade)</span>
            <span className="font-semibold">
              {selectedSchade
                ? `€ ${(selectedSchade.schade_meur * 1e6).toLocaleString('nl-NL', { maximumFractionDigits: 0 })}`
                : '—'}
            </span>
          </div>
          <div className="flex justify-between">
            <span className="text-gray-600">γ (economische groei)</span>
            <span className="font-semibold">
              {selectedGamma ? `${(selectedGamma.gamma * 100).toFixed(1)} %` : '—'}
            </span>
          </div>
        </div>

        <div className="flex gap-3 pt-1">
          <button
            onClick={() => {
              if (selectedSchade && selectedGamma) {
                onConfirm(selectedSchade.schade_meur * 1_000_000, selectedGamma.gamma)
              }
            }}
            disabled={!selectedSchade || !selectedGamma}
            className="flex-1 bg-blue-600 text-white text-sm py-2 rounded-lg hover:bg-blue-700 disabled:opacity-40 transition-colors"
          >
            Naar OptimizeForm →
          </button>
          <button
            onClick={onCancel}
            className="px-4 text-sm text-gray-600 hover:text-gray-900"
          >
            Annuleer
          </button>
        </div>
      </div>
    </div>
  )
}

// ── Tabelrij ─────────────────────────────────────────────────────────────────

function TrajectRow({
  t,
  onOptimize,
}: {
  t: ValTrajectory
  onOptimize: (t: ValTrajectory) => void
}) {
  return (
    <tr className="hover:bg-gray-50 transition-colors">
      <td className="px-3 py-2 font-mono text-xs text-gray-500">
        {t.Dijkring}-{t.DijkringDeel}-{t.DijkringTraject}
      </td>
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

// ── Hoofdpagina ───────────────────────────────────────────────────────────────

export default function ValidationDashboard() {
  const navigate = useNavigate()
  const [selectedDijkring, setSelectedDijkring] = useState<string>('')

  // Traject waarvoor de scenario-selectie open staat
  const [pendingTraj, setPendingTraj] = useState<ValTrajectory | null>(null)

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

  // Referentiedata ophalen voor het geselecteerde traject
  const { data: refData, isLoading: loadingRef } = useQuery({
    queryKey: ['val-reference', pendingTraj?.Dijkring, pendingTraj?.DijkringDeel],
    queryFn: () => getValReferenceData(pendingTraj!.Dijkring, pendingTraj!.DijkringDeel),
    enabled: !!pendingTraj,
    staleTime: Infinity,
  })

  function handleOptimize(t: ValTrajectory) {
    setPendingTraj(t)
  }

  function handleConfirm(v0_eur: number, gamma: number) {
    if (!pendingTraj) return
    navigate('/optimize', {
      state: {
        trajectory: {
          id: `ref-${pendingTraj.Dijkring}-${pendingTraj.DijkringDeel}-${pendingTraj.DijkringTraject}`,
          norm: pendingTraj.norm_per_jaar,
          length: 10.0,
          p0: pendingTraj.p0_per_jaar,
          alpha: pendingTraj.alpha_per_m,
          base_year: 2015,
          measures: [],
        },
        scenario: {
          id: `ref-scen-${pendingTraj.Dijkring}`,
          climate: 'huidig',
          q_design: 1000.0,
          h_design: 5.0,
          eta: pendingTraj.eta_m_per_jaar,
        },
        candidates: buildCandidates(pendingTraj),
        risk_params: {
          base_damage: v0_eur,
          discount_rate: 0.04,
          gamma,
          time_horizon: 100,
        },
      },
    })
    setPendingTraj(null)
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
          {trajectories?.length ?? 0} trajecten
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
              <tr>
                <td colSpan={7} className="px-3 py-4 text-center text-gray-400 animate-pulse text-sm">Laden…</td>
              </tr>
            ) : trajectories?.length === 0 ? (
              <tr>
                <td colSpan={7} className="px-3 py-4 text-center text-gray-400 text-sm">Geen trajecten</td>
              </tr>
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
        <div>
          P₀-waarden zijn uit OptimaliseRing 2011 (testbed). WBI2023-kansen staan ter discussie.
          Klik <strong>Optimaliseer →</strong> om schade- en economisch scenario te kiezen
          vóór de berekening — alle waarden zijn bewerkbaar in het formulier.
        </div>
      </div>

      {/* Scenario-selectie modal */}
      {pendingTraj && refData && !loadingRef && (
        <ScenarioPanel
          traj={pendingTraj}
          ref={refData}
          onConfirm={handleConfirm}
          onCancel={() => setPendingTraj(null)}
        />
      )}
      {pendingTraj && loadingRef && (
        <div className="fixed inset-0 bg-black/40 z-[2000] flex items-center justify-center">
          <div className="bg-white rounded-xl p-6 text-sm text-gray-500 animate-pulse">
            Referentiedata ophalen…
          </div>
        </div>
      )}
    </div>
  )
}
