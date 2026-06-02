import { HashRouter, Routes, Route, Link, useLocation } from 'react-router-dom'
import Dashboard from './pages/Dashboard'
import OptimizeForm from './pages/OptimizeForm'
import Results from './pages/Results'
import ValidationDashboard from './pages/ValidationDashboard'

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
    <nav className="border-b bg-white sticky top-0 z-10">
      <div className="max-w-3xl mx-auto px-6 py-3 flex items-center gap-2">
        <Link to="/" className="font-bold text-gray-900 mr-4">FloodOpt</Link>
        {link('/', 'Dashboard')}
        {link('/optimize', 'Optimaliseren')}
        {link('/validatie', 'Validatie')}
      </div>
    </nav>
  )
}

export default function App() {
  return (
    <HashRouter>
      <div className="min-h-screen bg-gray-50">
        <Nav />
        <main className="py-8">
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/optimize" element={<OptimizeForm />} />
            <Route path="/results/:jobId" element={<Results />} />
            <Route path="/validatie" element={<ValidationDashboard />} />
          </Routes>
        </main>
      </div>
    </HashRouter>
  )
}
