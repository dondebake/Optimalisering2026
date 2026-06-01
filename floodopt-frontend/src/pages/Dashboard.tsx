import { Link } from 'react-router-dom'

export default function Dashboard() {
  return (
    <div className="max-w-2xl mx-auto p-6 space-y-8">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">FloodOpt</h1>
        <p className="text-gray-500 mt-2">
          Optimalisatie van dijkversterkingsstrategieën op traject- en dijkringniveau.
        </p>
      </div>

      <div className="grid grid-cols-1 gap-4">
        <Link to="/optimize"
          className="block border border-gray-200 rounded-xl p-5 hover:border-blue-400 hover:bg-blue-50 transition-colors group">
          <div className="flex items-start gap-4">
            <div className="bg-blue-100 rounded-lg p-2.5 text-2xl">⚡</div>
            <div>
              <div className="font-semibold text-gray-900 group-hover:text-blue-700">
                Nieuwe optimalisatie
              </div>
              <div className="text-sm text-gray-500 mt-1">
                Vul traject, scenario en kandidaatmaatregelen in. De worker rekent asynchroon.
              </div>
            </div>
          </div>
        </Link>
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
        Fase 1+2 klaar (90/90 tests) — Fase 4.1 in uitvoering
      </div>
    </div>
  )
}
