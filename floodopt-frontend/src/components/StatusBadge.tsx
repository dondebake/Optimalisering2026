import type { JobStatus } from '../types'

const STYLES: Record<JobStatus, string> = {
  pending: 'bg-yellow-100 text-yellow-800',
  running: 'bg-blue-100 text-blue-800 animate-pulse',
  done: 'bg-green-100 text-green-800',
  failed: 'bg-red-100 text-red-800',
}

const LABELS: Record<JobStatus, string> = {
  pending: 'In wachtrij',
  running: 'Berekening…',
  done: 'Klaar',
  failed: 'Mislukt',
}

export function StatusBadge({ status }: { status: JobStatus }) {
  return (
    <span className={`inline-block px-2 py-0.5 rounded text-sm font-medium ${STYLES[status]}`}>
      {LABELS[status]}
    </span>
  )
}
