import { Link } from 'react-router-dom'
import { useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import MapView from '../components/MapView'
import JobList from '../components/JobList'
import { postTrajectory } from '../api/client'

const RIJNMOND_SAMPLE = {
  id: 'rijnmond-demo',
  norm: 1 / 10000,
  length: 62,
  p0: 1 / 200,
  alpha: 4.0,
  base_year: 2023,
  measures: [],
  geometry: {
    type: 'LineString',
    coordinates: [
      [4.05, 51.98],
      [4.15, 51.96],
      [4.28, 51.93],
      [4.42, 51.90],
      [4.55, 51.88],
      [4.65, 51.87],
    ],
  },
}

export default function Dashboard() {
  const qc = useQueryClient()
  const [loading, setLoading] = useState(false)
  const [loaded, setLoaded] = useState(false)

  async function loadSample() {
    setLoading(true)
    try {
      await postTrajectory(RIJNMOND_SAMPLE)
      await qc.invalidateQueries({ queryKey: ['geo-trajectories'] })
      setLoaded(true)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="max-w-3xl mx-auto p-6 space-y-8">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">FloodOpt</h1>
        <p className="text-gray-500 mt-2">
          Optimalisatie van dijkversterkingsstrategieën op traject- en dijkringniveau.
        </p>
      </div>

      <div className="space-y-2">
        <div className="flex items-center justify-between">
          <h2 className="text-sm font-semibold text-gray-700 uppercase tracking-wide">Kaart</h2>
          {!loaded && (
            <button
              onClick={loadSample}
              disabled={loading}
              className="text-xs px-3 py-1.5 rounded border border-gray-300 hover:border-blue-400 hover:text-blue-700 transition-colors disabled:opacity-50"
            >
              {loading ? 'Laden…' : 'Laad Rijnmond-voorbeeld'}
            </button>
          )}
        </div>
        <MapView />
      </div>

      <div className="space-y-2">
        <div className="flex items-center justify-between">
          <h2 className="text-sm font-semibold text-gray-700 uppercase tracking-wide">Recente jobs</h2>
          <Link to="/optimize"
            className="text-xs px-3 py-1.5 rounded bg-blue-600 text-white hover:bg-blue-700 transition-colors">
            + Nieuwe optimalisatie
          </Link>
        </div>
        <JobList />
      </div>

      <div className="bg-gray-50 rounded-xl p-5 space-y-2 text-sm text-gray-600">
        <div className="font-semibold text-gray-800 mb-3">Stack status</div>
        <div className="flex justify-between">
          <span>FastAPI</span>
          <a href="http://localhost:8000/docs" target="_blank" rel="noopener noreferrer"
            className="text-blue-600 hover:underline">localhost:8000/docs ↗</a>
        </div>
        <div className="flex justify-between">
          <span>Redis broker</span>
          <span className="text-gray-400">localhost:6379</span>
        </div>
        <div className="flex justify-between">
          <span>Celery worker</span>
          <span className="text-gray-400">pending → running → done</span>
        </div>
      </div>

      <div className="text-xs text-gray-400 border-t pt-4">
        Fase 1+2 klaar (90/90 tests) — Fase 4.1+4.2 klaar
      </div>
    </div>
  )
}
