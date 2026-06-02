import { MapContainer, TileLayer, GeoJSON } from 'react-leaflet'
import { useQuery } from '@tanstack/react-query'
import { getTrajectoriesGeoJSON } from '../api/client'
import type { Layer, PathOptions } from 'leaflet'

// Klasse-indeling conform OptimaliseRing (overstromingskans 1/jaar)
const P_CLASSES: Array<{ max: number; color: string; label: string }> = [
  { max: 1 / 113_000, color: '#00bcd4', label: '< 1/113.000' },
  { max: 1 / 57_000,  color: '#2196f3', label: '1/113.000–1/57.000' },
  { max: 1 / 28_000,  color: '#9c27b0', label: '1/57.000–1/28.000' },
  { max: 1 / 14_000,  color: '#e91e63', label: '1/28.000–1/14.000' },
  { max: 1 / 6_300,   color: '#f44336', label: '1/14.000–1/6.300' },
  { max: 1 / 2_800,   color: '#ff9800', label: '1/6.300–1/2.800' },
  { max: 1 / 1_600,   color: '#ffeb3b', label: '1/2.800–1/1.600' },
  { max: 1 / 800,     color: '#8bc34a', label: '1/1.600–1/800' },
  { max: Infinity,    color: '#1b5e20', label: '> 1/800' },
]

function pColor(p: number | null | undefined): string {
  if (p == null) return '#2563eb'
  for (const cls of P_CLASSES) {
    if (p < cls.max) return cls.color
  }
  return P_CLASSES[P_CLASSES.length - 1].color
}

function featureStyle(feature?: GeoJSON.Feature): PathOptions {
  const p = feature?.properties?.p_year
  return { color: pColor(p), weight: 3, opacity: 0.85 }
}

function onEachFeature(feature: GeoJSON.Feature, layer: Layer) {
  const p = feature.properties
  if (!p) return
  const norm = p.norm ? `1/${Math.round(1 / p.norm).toLocaleString('nl-NL')}` : '—'
  const pYear = p.p_year != null
    ? `1/${Math.round(1 / p.p_year).toLocaleString('nl-NL')}`
    : '—'
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  ;(layer as any).bindPopup(
    `<b>${p.id}</b><br/>` +
    `Norm: ${norm}/jaar<br/>` +
    `Lengte: ${p.length} km<br/>` +
    `P(${p.year ?? 2050}): ${pYear}/jaar`
  )
}

function Legend() {
  return (
    <div className="absolute bottom-4 left-4 z-[1000] bg-white/95 rounded-lg shadow p-3 text-xs space-y-1">
      <div className="font-semibold text-gray-700 mb-1.5">Overstromingskans 2050</div>
      {P_CLASSES.map((cls) => (
        <div key={cls.label} className="flex items-center gap-2">
          <span className="inline-block w-5 h-2 rounded-sm" style={{ backgroundColor: cls.color }} />
          <span className="text-gray-600">{cls.label}</span>
        </div>
      ))}
      <div className="flex items-center gap-2 pt-1 border-t border-gray-100">
        <span className="inline-block w-5 h-2 rounded-sm" style={{ backgroundColor: '#2563eb' }} />
        <span className="text-gray-400">Geen resultaat</span>
      </div>
    </div>
  )
}

export default function MapView() {
  const { data, isLoading } = useQuery({
    queryKey: ['geo-trajectories'],
    queryFn: getTrajectoriesGeoJSON,
    staleTime: 30_000,
  })

  const hasGeometry = data?.features.some(f => f.geometry !== null)

  return (
    <div className="relative rounded-xl overflow-hidden border border-gray-200">
      <MapContainer
        center={[52.15, 5.3]}
        zoom={7}
        className="h-80 w-full"
      >
        <TileLayer
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
        />
        {hasGeometry && (
          <GeoJSON
            key={JSON.stringify(data?.features.map(f => f.properties?.p_year))}
            data={data!}
            style={featureStyle}
            onEachFeature={onEachFeature}
          />
        )}
      </MapContainer>

      {hasGeometry && <Legend />}

      {isLoading && (
        <div className="absolute inset-0 flex items-center justify-center bg-white/60 text-sm text-gray-500">
          Kaart laden…
        </div>
      )}

      {!isLoading && !hasGeometry && (
        <div className="absolute bottom-3 left-1/2 -translate-x-1/2 bg-white/90 text-xs text-gray-500 px-3 py-1.5 rounded-full shadow">
          Geen trajecten met geometrie — gebruik POST /trajectories met een geometry-veld
        </div>
      )}
    </div>
  )
}
