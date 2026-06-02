import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { getAllResults, deleteResult } from '../api/client'
import { StatusBadge } from '../components/StatusBadge'
import type { OptimizeResponse } from '../types'

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
  const [selected, setSelected] = useState<Set<string>>(new Set())

  const { data, isLoading } = useQuery({
    queryKey: ['all-results'],
    queryFn: getAllResults,
    refetchInterval: (q) => {
      const jobs = q.state.data ?? []
      return jobs.some(j => j.status === 'pending' || j.status === 'running') ? 2000 : 15_000
    },
  })

  const deleteMutation = useMutation({
    mutationFn: deleteResult,
    onMutate: async (jobId) => {
      await qc.cancelQueries({ queryKey: ['all-results'] })
      const prev = qc.getQueryData<OptimizeResponse[]>(['all-results'])
      qc.setQueryData<OptimizeResponse[]>(['all-results'],
        old => old?.filter(j => j.job_id !== jobId) ?? [])
      setSelected(s => { const n = new Set(s); n.delete(jobId); return n })
      return { prev }
    },
    onError: (_e, _id, ctx) => {
      qc.setQueryData(['all-results'], ctx?.prev)
    },
    onSettled: () => qc.invalidateQueries({ queryKey: ['all-results'] }),
  })

  const ids = data?.map(j => j.job_id) ?? []
  const allChecked = ids.length > 0 && ids.every(id => selected.has(id))
  const someChecked = ids.some(id => selected.has(id))

  function toggleAll() {
    if (allChecked) {
      setSelected(new Set())
    } else {
      setSelected(new Set(ids))
    }
  }

  function toggleOne(id: string) {
    setSelected(s => {
      const n = new Set(s)
      n.has(id) ? n.delete(id) : n.add(id)
      return n
    })
  }

  function handleDeleteSelected() {
    if (selected.size === 0) return
    if (!window.confirm(`${selected.size} run(s) verwijderen?`)) return
    selected.forEach(id => deleteMutation.mutate(id))
  }

  function handleDeleteOne(jobId: string) {
    deleteMutation.mutate(jobId)
  }

  return (
    <div className="max-w-5xl mx-auto p-6 space-y-4">
      <div className="flex items-center justify-between flex-wrap gap-3">
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
        <>
          {/* Bulkacties */}
          <div className="flex items-center gap-4 min-h-[32px]">
            {selected.size > 0 && (
              <>
                <span className="text-sm text-gray-600">{selected.size} geselecteerd</span>
                <button
                  onClick={handleDeleteSelected}
                  className="text-sm px-3 py-1.5 rounded bg-red-600 text-white hover:bg-red-700 transition-colors"
                >
                  Verwijder selectie ({selected.size})
                </button>
                <button
                  onClick={() => setSelected(new Set())}
                  className="text-sm text-gray-400 hover:text-gray-600"
                >
                  Deselecteer alles
                </button>
              </>
            )}
          </div>

          <div className="rounded-xl border border-gray-200 overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="bg-gray-50 text-xs text-gray-500 uppercase tracking-wide">
                <tr>
                  <th className="px-4 py-3 w-10">
                    <input
                      type="checkbox"
                      checked={allChecked}
                      ref={el => { if (el) el.indeterminate = someChecked && !allChecked }}
                      onChange={toggleAll}
                      className="accent-blue-600 cursor-pointer"
                      title={allChecked ? 'Deselecteer alles' : 'Selecteer alles'}
                    />
                  </th>
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
                  <tr
                    key={job.job_id}
                    className={`transition-colors ${selected.has(job.job_id) ? 'bg-blue-50' : 'hover:bg-gray-50'}`}
                  >
                    <td className="px-4 py-3">
                      <input
                        type="checkbox"
                        checked={selected.has(job.job_id)}
                        onChange={() => toggleOne(job.job_id)}
                        className="accent-blue-600 cursor-pointer"
                      />
                    </td>
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
                          onClick={() => handleDeleteOne(job.job_id)}
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
        </>
      )}
    </div>
  )
}
