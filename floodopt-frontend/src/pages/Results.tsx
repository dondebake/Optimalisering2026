import { useParams, Link } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { getResult } from '../api/client'
import { StatusBadge } from '../components/StatusBadge'

const OBJECTIVE_LABEL: Record<string, string> = {
  min_cost: 'MIN_COST',
  min_ncw: 'MIN_NCW',
  max_risk_reduction: 'MAX_RISK_REDUCTION',
}

function fmt(n: number | null, prefix = '€ ') {
  if (n == null) return '—'
  return `${prefix}${n.toLocaleString('nl-NL', { maximumFractionDigits: 0 })}`
}

export default function Results() {
  const { jobId } = useParams<{ jobId: string }>()

  const { data, error } = useQuery({
    queryKey: ['result', jobId],
    queryFn: () => getResult(jobId!),
    refetchInterval: (query) => {
      const status = query.state.data?.status
      return status === 'done' || status === 'failed' ? false : 2000
    },
    enabled: !!jobId,
  })

  if (error) {
    return (
      <div className="max-w-xl mx-auto p-6">
        <div className="bg-red-50 border border-red-200 rounded p-4 text-red-700">
          Fout: {error instanceof Error ? error.message : 'Onbekend'}
        </div>
      </div>
    )
  }

  if (!data) {
    return (
      <div className="max-w-xl mx-auto p-6 text-gray-500 text-sm animate-pulse">
        Resultaat ophalen…
      </div>
    )
  }

  return (
    <div className="max-w-2xl mx-auto p-6 space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">Resultaat</h1>
        <StatusBadge status={data.status} />
      </div>

      <div className="grid grid-cols-2 gap-3 text-sm">
        <div className="bg-gray-50 rounded p-3">
          <div className="text-gray-500 text-xs mb-1">Job ID</div>
          <div className="font-mono text-xs break-all">{data.job_id}</div>
        </div>
        <div className="bg-gray-50 rounded p-3">
          <div className="text-gray-500 text-xs mb-1">Doelfunctie / Solver</div>
          <div className="font-semibold">{OBJECTIVE_LABEL[data.objective] ?? data.objective}</div>
          <div className="text-gray-500 text-xs">{data.solver}</div>
        </div>
        <div className="bg-gray-50 rounded p-3">
          <div className="text-gray-500 text-xs mb-1">Traject</div>
          <div className="font-mono text-xs">{data.trajectory_id}</div>
        </div>
        <div className="bg-gray-50 rounded p-3">
          <div className="text-gray-500 text-xs mb-1">Scenario</div>
          <div className="font-mono text-xs">{data.scenario_id}</div>
        </div>
      </div>

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

      {data.status === 'done' && (
        <div className="space-y-4">
          <h2 className="text-lg font-semibold text-gray-800 border-b pb-1">Geselecteerde maatregelen</h2>
          {data.selected_measure_ids.length === 0 ? (
            <p className="text-gray-400 text-sm">Geen maatregelen geselecteerd.</p>
          ) : (
            <div className="flex flex-wrap gap-2">
              {data.selected_measure_ids.map((id) => (
                <span key={id} className="bg-green-100 text-green-800 px-2 py-1 rounded text-sm font-mono">
                  {id}
                </span>
              ))}
            </div>
          )}

          <h2 className="text-lg font-semibold text-gray-800 border-b pb-1">Financiële samenvatting</h2>
          <div className="grid grid-cols-2 gap-3 text-sm">
            <div className="bg-white border rounded p-3">
              <div className="text-gray-500 text-xs mb-1">Doelfunctiewaarde</div>
              <div className="text-xl font-bold text-blue-700">{fmt(data.objective_value)}</div>
            </div>
            <div className="bg-white border rounded p-3">
              <div className="text-gray-500 text-xs mb-1">Totale NCW</div>
              <div className="text-xl font-bold text-gray-900">{fmt(data.total_ncw)}</div>
            </div>
            <div className="bg-white border rounded p-3">
              <div className="text-gray-500 text-xs mb-1">Risico-NCW (verwachte schade)</div>
              <div className="font-semibold">{fmt(data.risk_ncw)}</div>
            </div>
            <div className="bg-white border rounded p-3">
              <div className="text-gray-500 text-xs mb-1">Investeringskosten (NPV)</div>
              <div className="font-semibold">{fmt(data.investment_npv)}</div>
            </div>
          </div>
        </div>
      )}

      <div className="pt-2">
        <Link to="/optimize" className="text-sm text-blue-600 hover:underline">
          ← Nieuwe optimalisatie
        </Link>
      </div>
    </div>
  )
}
