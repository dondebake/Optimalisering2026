import { Link } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { getAllResults, deleteResult } from '../api/client'
import { StatusBadge } from './StatusBadge'
import type { OptimizeResponse } from '../types'

const OBJECTIVE_LABEL: Record<string, string> = {
  min_cost: 'MIN_COST',
  min_ncw: 'MIN_NCW',
  max_risk_reduction: 'MAX_RR',
}

function fmt(n: number | null) {
  if (n == null) return '—'
  return `€ ${n.toLocaleString('nl-NL', { maximumFractionDigits: 0 })}`
}

export default function JobList() {
  const qc = useQueryClient()

  const deleteMutation = useMutation({
    mutationFn: deleteResult,
    onMutate: async (jobId) => {
      await qc.cancelQueries({ queryKey: ['all-results'] })
      const prev = qc.getQueryData<OptimizeResponse[]>(['all-results'])
      qc.setQueryData<OptimizeResponse[]>(['all-results'],
        old => old?.filter(j => j.job_id !== jobId) ?? [])
      return { prev }
    },
    onError: (_e, _id, ctx) => {
      qc.setQueryData(['all-results'], ctx?.prev)
    },
    onSettled: () => qc.invalidateQueries({ queryKey: ['all-results'] }),
  })

  function handleDelete(jobId: string) {
    if (!window.confirm('Job verwijderen?')) return
    deleteMutation.mutate(jobId)
  }

  const { data, isLoading } = useQuery({
    queryKey: ['all-results'],
    queryFn: getAllResults,
    refetchInterval: (query) => {
      const jobs = query.state.data ?? []
      const active = jobs.some(j => j.status === 'pending' || j.status === 'running')
      return active ? 2000 : 15_000
    },
  })

  if (isLoading) {
    return <p className="text-sm text-gray-400 animate-pulse">Jobs ophalen…</p>
  }

  if (!data || data.length === 0) {
    return (
      <p className="text-sm text-gray-400">
        Nog geen optimalisaties — start er een via{' '}
        <Link to="/optimize" className="text-blue-600 hover:underline">Optimaliseren</Link>.
      </p>
    )
  }

  return (
    <div className="overflow-x-auto rounded-xl border border-gray-200">
      <table className="w-full text-sm">
        <thead className="bg-gray-50 text-xs text-gray-500 uppercase tracking-wide">
          <tr>
            <th className="text-left px-4 py-2.5">Job ID</th>
            <th className="text-left px-4 py-2.5">Traject</th>
            <th className="text-left px-4 py-2.5">Doelfunctie</th>
            <th className="text-left px-4 py-2.5">Resultaat</th>
            <th className="text-left px-4 py-2.5">Status</th>
            <th className="px-4 py-2.5" />
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-100">
          {data.map((job) => (
            <tr key={job.job_id} className="hover:bg-gray-50 transition-colors">
              <td className="px-4 py-3 font-mono text-xs text-gray-500">
                {job.job_id.slice(0, 8)}…
              </td>
              <td className="px-4 py-3 font-mono text-xs">{job.trajectory_id}</td>
              <td className="px-4 py-3">{OBJECTIVE_LABEL[job.objective] ?? job.objective}</td>
              <td className="px-4 py-3 tabular-nums text-gray-700">
                {job.status === 'done' ? fmt(job.objective_value) : '—'}
              </td>
              <td className="px-4 py-3">
                <StatusBadge status={job.status} />
              </td>
              <td className="px-4 py-3 text-right flex items-center justify-end gap-3">
                <Link
                  to={`/results/${job.job_id}`}
                  className="text-blue-600 hover:underline text-xs"
                >
                  Bekijk →
                </Link>
                <button
                  onClick={() => handleDelete(job.job_id)}
                  className="text-red-400 hover:text-red-600 text-xs"
                  title="Verwijder job"
                >
                  ✕
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
