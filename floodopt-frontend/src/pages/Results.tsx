import { useParams, Link, useNavigate } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { getResult, getTrajectory } from '../api/client'
import { StatusBadge } from '../components/StatusBadge'
import PSeriesChart from '../components/PSeriesChart'
import type { InputPayload, Measure } from '../types'

const OBJECTIVE_LABEL: Record<string, string> = {
  min_cost: 'MIN_COST — minimale investering',
  min_ncw: 'MIN_NCW — minimale totale NCW',
  max_risk_reduction: 'MAX_RR — maximale risicoreductie',
}

function fmt(n: number | null, prefix = '€ ') {
  if (n == null) return '—'
  return `${prefix}${n.toLocaleString('nl-NL', { maximumFractionDigits: 0 })}`
}

function fmtM(n: number | null) {
  if (n == null) return '—'
  return `€ ${(n / 1e6).toLocaleString('nl-NL', { maximumFractionDigits: 1 })} M`
}

function fmtP(p: number) {
  return `1/${Math.round(1 / p).toLocaleString('nl-NL')}/jr`
}

function Row({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex justify-between py-1.5 border-b border-gray-50 last:border-0">
      <span className="text-gray-500 text-sm">{label}</span>
      <span className="text-sm font-medium text-right">{value}</span>
    </div>
  )
}

function Section({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div className="border border-gray-200 rounded-xl overflow-hidden">
      <div className="px-4 py-2.5 bg-gray-50 border-b border-gray-200">
        <h2 className="text-sm font-semibold text-gray-700">{title}</h2>
      </div>
      <div className="px-4 py-3">{children}</div>
    </div>
  )
}

function MeasureTable({ measures, selectedIds }: { measures: Measure[]; selectedIds: string[] }) {
  const selSet = new Set(selectedIds)
  return (
    <table className="w-full text-xs">
      <thead>
        <tr className="text-gray-400 uppercase tracking-wide">
          <th className="text-left pb-1.5">ID</th>
          <th className="text-left pb-1.5">Type</th>
          <th className="text-right pb-1.5">Δh [m]</th>
          <th className="text-right pb-1.5">Kosten</th>
          <th className="text-right pb-1.5">Jaar</th>
          <th className="text-center pb-1.5">Geselecteerd</th>
        </tr>
      </thead>
      <tbody className="divide-y divide-gray-50">
        {measures.map(m => {
          const sel = selSet.has(m.id)
          return (
            <tr key={m.id} className={sel ? 'bg-green-50' : ''}>
              <td className="py-1.5 font-mono font-medium">{m.id}</td>
              <td className="py-1.5 text-gray-600">{m.type.replace('_', ' ')}</td>
              <td className="py-1.5 text-right tabular-nums">{m.effect.toFixed(2)}</td>
              <td className="py-1.5 text-right tabular-nums">{fmtM(m.cost)}</td>
              <td className="py-1.5 text-right tabular-nums">{m.year}</td>
              <td className="py-1.5 text-center">
                {sel ? <span className="text-green-600 font-bold">✓</span> : <span className="text-gray-300">—</span>}
              </td>
            </tr>
          )
        })}
      </tbody>
    </table>
  )
}

export default function Results() {
  const { jobId } = useParams<{ jobId: string }>()
  const navigate = useNavigate()

  const { data, error } = useQuery({
    queryKey: ['result', jobId],
    queryFn: () => getResult(jobId!),
    refetchInterval: (query) => {
      const status = query.state.data?.status
      return status === 'done' || status === 'failed' ? false : 2000
    },
    enabled: !!jobId,
  })

  const { data: trajectory } = useQuery({
    queryKey: ['trajectory', data?.trajectory_id],
    queryFn: () => getTrajectory(data!.trajectory_id),
    enabled: !!data?.trajectory_id,
  })

  function handleOpnieuw(withChanges = false) {
    const p = data?.input_payload as InputPayload | null
    navigate('/optimize', {
      state: {
        trajectory: p?.trajectory ?? (trajectory ? {
          id: trajectory.id, norm: trajectory.norm, length: trajectory.length,
          p0: trajectory.p0, alpha: trajectory.alpha, base_year: trajectory.base_year,
          measures: [],
        } : undefined),
        scenario: p?.scenario,
        candidates: p?.candidates,
        risk_params: p?.risk_params,
        _withChanges: withChanges,
      },
    })
  }

  if (error) return (
    <div className="max-w-xl mx-auto p-6">
      <div className="bg-red-50 border border-red-200 rounded p-4 text-red-700">
        Fout: {error instanceof Error ? error.message : 'Onbekend'}
      </div>
    </div>
  )

  if (!data) return (
    <div className="max-w-xl mx-auto p-6 text-gray-500 text-sm animate-pulse">
      Resultaat ophalen…
    </div>
  )

  const inp = data.input_payload as InputPayload | null

  return (
    <div className="max-w-4xl mx-auto p-6 space-y-5">

      {/* Header */}
      <div className="flex items-start justify-between flex-wrap gap-3">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Resultaat</h1>
          <div className="text-xs text-gray-400 font-mono mt-0.5">{data.job_id}</div>
        </div>
        <div className="flex items-center gap-3">
          <StatusBadge status={data.status} />
          <div className="flex gap-2">
            <button
              onClick={() => handleOpnieuw(false)}
              className="text-sm px-3 py-1.5 rounded border border-gray-300 hover:border-blue-400 hover:text-blue-700 transition-colors"
            >
              Opnieuw ↺
            </button>
            <button
              onClick={() => handleOpnieuw(true)}
              className="text-sm px-3 py-1.5 rounded bg-blue-600 text-white hover:bg-blue-700 transition-colors"
            >
              Opnieuw met aanpassingen →
            </button>
          </div>
        </div>
      </div>

      {/* Status melding */}
      {(data.status === 'pending' || data.status === 'running') && (
        <div className="bg-blue-50 border border-blue-200 rounded p-4 text-sm text-blue-700">
          Berekening is{data.status === 'pending' ? ' in de wachtrij' : ' bezig'}… Pagina ververst automatisch.
        </div>
      )}
      {data.status === 'failed' && (
        <div className="bg-red-50 border border-red-200 rounded p-4 text-sm text-red-700">
          De optimalisatie is mislukt. Controleer de worker-logs.
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-5">

        {/* Linker kolom — invoerparameters */}
        <div className="space-y-4">

          {/* Doelfunctie + solver */}
          <Section title="Optimalisatie-instellingen">
            <Row label="Doelfunctie" value={OBJECTIVE_LABEL[data.objective] ?? data.objective} />
            <Row label="Solver" value={data.solver} />
            {inp?.budget != null && (
              <Row label="Budget" value={fmt(inp.budget)} />
            )}
          </Section>

          {/* Traject */}
          {inp && (
            <Section title="Dijktraject">
              <Row label="ID" value={inp.trajectory.id} />
              <Row label="P₀ (basisjaar)" value={fmtP(inp.trajectory.p0)} />
              <Row label="Norm" value={fmtP(inp.trajectory.norm)} />
              <Row label="α" value={`${inp.trajectory.alpha} /m`} />
              <Row label="Lengte" value={`${inp.trajectory.length} km`} />
              <Row label="Basisjaar" value={String(inp.trajectory.base_year)} />
            </Section>
          )}

          {/* Scenario */}
          {inp && (
            <Section title="Klimaatscenario">
              <Row label="ID" value={inp.scenario.id} />
              <Row label="Klimaat" value={inp.scenario.climate} />
              <Row label="η (zeespiegelstijging)" value={`${inp.scenario.eta} m/jr`} />
              <Row label="q ontwerp" value={`${inp.scenario.q_design} m³/s`} />
              <Row label="h ontwerp" value={`${inp.scenario.h_design} m+NAP`} />
            </Section>
          )}

          {/* Risicoparameters */}
          {inp && (
            <Section title="Risicoparameters">
              <Row label="V₀ (basisschade)" value={fmt(inp.risk_params.base_damage)} />
              <Row label="δ (discontovoet)" value={`${(inp.risk_params.discount_rate * 100).toFixed(2)} %`} />
              <Row label="γ (economische groei)" value={`${(inp.risk_params.gamma * 100).toFixed(1)} %`} />
              <Row label="Tijdshorizon T" value={`${inp.risk_params.time_horizon} jaar`} />
            </Section>
          )}
        </div>

        {/* Rechter kolom — resultaten */}
        <div className="space-y-4">

          {/* Financiële samenvatting */}
          {data.status === 'done' && (
            <Section title="Financieel resultaat">
              <Row label="Doelfunctiewaarde" value={fmt(data.objective_value)} />
              <Row label="Totale NCW" value={fmt(data.total_ncw)} />
              <Row label="Risico-NCW (verwachte schade)" value={fmt(data.risk_ncw)} />
              <Row label="Investeringskosten (NPV)" value={fmt(data.investment_npv)} />
            </Section>
          )}

          {/* Kandidaatmaatregelen */}
          {inp && inp.candidates.length > 0 && (
            <Section title={`Kandidaatmaatregelen (${inp.candidates.length})`}>
              <MeasureTable
                measures={inp.candidates}
                selectedIds={data.selected_measure_ids}
              />
            </Section>
          )}
        </div>
      </div>

      {/* P(t) grafiek — vol breedte */}
      {data.status === 'done' && data.p_series && data.p_series.length > 0 && trajectory && (
        <Section title="P(t) — overstromingskans als functie van de tijd">
          <PSeriesChart
            series={data.p_series}
            norm={trajectory.norm}
            trajectoryId={data.trajectory_id}
          />
        </Section>
      )}

      <div className="pt-1 flex gap-4">
        <Link to="/runs" className="text-sm text-blue-600 hover:underline">← Alle runs</Link>
        <Link to="/" className="text-sm text-gray-500 hover:underline">← Dashboard</Link>
      </div>
    </div>
  )
}
