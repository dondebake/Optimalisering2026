import { Link } from 'react-router-dom'
import { useQuery, useQueryClient } from '@tanstack/react-query'
import { getAllResults, deleteResult } from '../api/client'
import { StatusBadge } from '../components/StatusBadge'

const OBJ: Record<string, string> = {
  min_cost: 'MIN_COST',
  min_ncw: 'MIN_NCW',
  max_risk_reduction: 'MAX_RR',
}

function fmtEur(n: number | null) {
  if (n == null) return '—'
  return `€ ${(n / 1e6).toLocaleString('nl-NL', { maximumFractionDigits: 0 })} M`
}

export default function RunsPage() {
  const qc = useQueryClient()

  const { data, isLoading } = useQuery({
    queryKey: ['all-results'],
    queryFn: getAllResults,
    refetchInterval: (q) => {
      const jobs = q.state.data ?? []
      return jobs.some(j => j.status === 'pending' || j.status === 'running') ? 2000 : 15_000
    },
  })

  async function handleDelete(jobId: string) {
    if (!window.confirm('Run verwijderen?')) return
    await deleteResult(jobId)
    qc.invalidateQueries({ queryKey: ['all-results'] })
  }

  return (
    <div className="max-w-5xl mx-auto p-6 space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Berekeningen</h1>
          <p className="text-sm text-gray-500 mt-0.5">
            Alle optimalisatieruns — nieuwste eerst
          </p>
        </div>
        <Link to="/validatie"
          className="text-sm px-4 py-2 rounded-lg bg-blue-600 text-white hover:bg-blue-700 transition-colors">
          + Nieuwe berekening
        </Link>
      </div>

      {isLoading && (
        <p className="text-sm text-gray-400 animate-pulse">Laden…</p>
      )}

      {!isLoading && (!data || data.length === 0) && (
        <div className="text-center py-16 text-gray-400 text-sm space-y-2">
          <div>Nog geen berekeningen.</div>
          <Link to="/validatie" className="text-blue-600 hover:underline">
            Start vanuit het validatie-dashboard →
          </Link>
        </div>
      )}

      {data && data.length > 0 && (
        <div className="rounded-xl border border-gray-200 overflow-x-auto">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 text-xs text-gray-500 uppercase tracking-wide">
              <tr>
                <th className="text-left px-4 py-3">Traject</th>
                <th className="text-left px-4 py-3">Doelfunctie</th>
                <th className="text-left px-4 py-3">Solver</th>
                <th className="text-left px-4 py-3">Resultaat</th>
                <th className="text-left px-4 py-3">Status</th>
                <th className="text-left px-4 py-3">Job ID</th>
                <th className="px-4 py-3" />
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {data.map((job) => (
                <tr key={job.job_id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-4 py-3 font-mono text-xs text-gray-700">
                    {job.trajectory_id}
                  </td>
                  <td className="px-4 py-3">
                    {OBJ[job.objective] ?? job.objective}
                  </td>
                  <td className="px-4 py-3 text-gray-500">
                    {job.solver}
                  </td>
                  <td className="px-4 py-3 tabular-nums text-blue-700 font-medium">
                    {job.status === 'done' ? fmtEur(job.objective_value) : '—'}
                  </td>
                  <td className="px-4 py-3">
                    <StatusBadge status={job.status} />
                  </td>
                  <td className="px-4 py-3 font-mono text-xs text-gray-400">
                    {job.job_id.slice(0, 8)}…
                  </td>
                  <td className="px-4 py-3 text-right">
                    <div className="flex items-center justify-end gap-3">
                      <Link to={`/results/${job.job_id}`}
                        className="text-xs text-blue-600 hover:underline">
                        Bekijk →
                      </Link>
                      <button
                        onClick={() => handleDelete(job.job_id)}
                        className="text-xs text-gray-300 hover:text-red-500 transition-colors"
                        title="Verwijder">
                        ✕
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
