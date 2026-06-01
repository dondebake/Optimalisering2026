import type { Measure } from '../types'

const TYPE_LABEL: Record<string, string> = {
  dike_reinforcement: 'Dijkversterking',
  room_for_river: 'Ruimte voor Rivier',
  other: 'Overig',
}

export function MeasureList({ measures }: { measures: Measure[] }) {
  if (measures.length === 0) return <p className="text-gray-400 text-sm">Geen maatregelen.</p>

  return (
    <table className="w-full text-sm border-collapse">
      <thead>
        <tr className="bg-gray-100 text-left">
          <th className="px-3 py-2">ID</th>
          <th className="px-3 py-2">Type</th>
          <th className="px-3 py-2">Effect (m)</th>
          <th className="px-3 py-2">Kosten (€)</th>
          <th className="px-3 py-2">Jaar</th>
          <th className="px-3 py-2">Locatie</th>
        </tr>
      </thead>
      <tbody>
        {measures.map((m, i) => (
          <tr key={m.id} className={i % 2 === 0 ? 'bg-white' : 'bg-gray-50'}>
            <td className="px-3 py-1.5 font-mono">{m.id}</td>
            <td className="px-3 py-1.5">{TYPE_LABEL[m.type] ?? m.type}</td>
            <td className="px-3 py-1.5">{m.effect.toFixed(2)}</td>
            <td className="px-3 py-1.5">{m.cost.toLocaleString('nl-NL')}</td>
            <td className="px-3 py-1.5">{m.year}</td>
            <td className="px-3 py-1.5">{m.location}</td>
          </tr>
        ))}
      </tbody>
    </table>
  )
}
