import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ReferenceLine,
  ResponsiveContainer,
} from 'recharts'
import type { PSeriesPoint } from '../types'

interface Props {
  series: PSeriesPoint[]
  norm: number        // trajectory norm (Pwet)
  trajectoryId: string
}

function fmtP(p: number): string {
  if (p === 0) return '0'
  const inv = Math.round(1 / p)
  return `1/${inv.toLocaleString('nl-NL')}`
}

function fmtPAxis(p: number): string {
  if (p === 0) return '0'
  return p.toExponential(1)
}

const TOOLTIP_STYLE = {
  backgroundColor: '#fff',
  border: '1px solid #e5e7eb',
  borderRadius: 6,
  fontSize: 12,
}

function CustomTooltip({ active, payload, label }: any) {
  if (!active || !payload?.length) return null
  return (
    <div style={TOOLTIP_STYLE} className="p-2 space-y-1">
      <div className="font-semibold text-gray-700">{Math.round(label)}</div>
      {payload.map((entry: any) => (
        <div key={entry.dataKey} style={{ color: entry.color }}>
          {entry.name}: {fmtP(entry.value)}
        </div>
      ))}
    </div>
  )
}

export default function PSeriesChart({ series, norm, trajectoryId }: Props) {
  return (
    <div className="space-y-2">
      <div className="text-sm font-semibold text-gray-700">{trajectoryId} — overstromingskans als functie van de tijd</div>
      <ResponsiveContainer width="100%" height={340}>
        <LineChart data={series} margin={{ top: 8, right: 24, left: 16, bottom: 8 }}>
          <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
          <XAxis
            dataKey="year"
            type="number"
            domain={['dataMin', 'dataMax']}
            tickFormatter={(v) => String(Math.round(v))}
            tick={{ fontSize: 11 }}
            label={{ value: 'Jaar', position: 'insideBottomRight', offset: -4, fontSize: 11 }}
          />
          <YAxis
            tickFormatter={fmtPAxis}
            tick={{ fontSize: 11 }}
            label={{
              value: 'Overstromingskans [1/jaar]',
              angle: -90,
              position: 'insideLeft',
              offset: 8,
              fontSize: 11,
            }}
            width={72}
          />
          <Tooltip content={<CustomTooltip />} />
          <Legend wrapperStyle={{ fontSize: 12 }} />

          {/* Pwet — wettelijke norm */}
          <ReferenceLine
            y={norm}
            stroke="#000"
            strokeWidth={1.5}
            label={{ value: 'Pwet', position: 'right', fontSize: 11 }}
          />

          {/* P — optimale strategie */}
          <Line
            type="linear"
            dataKey="p"
            name="P (optimaal)"
            stroke="#16a34a"
            strokeWidth={2}
            dot={false}
            isAnimationActive={false}
          />

          {/* Pmidden */}
          <Line
            type="stepAfter"
            dataKey="p_mid"
            name="Pmidden"
            stroke="#2563eb"
            strokeWidth={1.5}
            strokeDasharray="5 3"
            dot={false}
            isAnimationActive={false}
          />
        </LineChart>
      </ResponsiveContainer>
      <div className="flex gap-4 text-xs text-gray-500">
        <span><span className="inline-block w-6 border-t-2 border-black align-middle mr-1" />Pwet = 1/{Math.round(1 / norm).toLocaleString('nl-NL')}/jaar</span>
      </div>
    </div>
  )
}
