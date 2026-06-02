import { HashRouter, Routes, Route, Link, useLocation } from 'react-router-dom'
import Dashboard from './pages/Dashboard'
import OptimizeForm from './pages/OptimizeForm'
import Results from './pages/Results'
import ValidationDashboard from './pages/ValidationDashboard'
import RunsPage from './pages/RunsPage'

function Nav() {
  const { pathname } = useLocation()
  const link = (to: string, label: string) => (
    <Link to={to}
      className={`text-sm px-3 py-1.5 rounded transition-colors ${
        pathname === to ? 'bg-blue-100 text-blue-700 font-medium' : 'text-gray-600 hover:text-gray-900'
      }`}>
      {label}
    </Link>
  )
  return (
    <nav className="border-b bg-white sticky top-0 z-10 flex-shrink-0">
      <div className="px-6 py-3 flex items-center gap-2">
        <Link to="/" className="font-bold text-gray-900 mr-4">FloodOpt</Link>
        {link('/', 'Dashboard')}
        {link('/runs', 'Runs')}
        {link('/validatie', 'Validatie')}
      </div>
    </nav>
  )
}

export default function App() {
  return (
    <HashRouter>
      <div className="flex flex-col h-screen bg-gray-50">
        <Nav />
        <main className="flex-1 min-h-0 overflow-auto">
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/runs" element={<RunsPage />} />
            <Route path="/results/:jobId" element={<Results />} />
            <Route path="/validatie" element={<ValidationDashboard />} />
            {/* /optimize blijft beschikbaar als standalone formulier */}
            <Route path="/optimize" element={<OptimizeForm />} />
          </Routes>
        </main>
      </div>
    </HashRouter>
  )
}
