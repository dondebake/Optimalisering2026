import { MapContainer, TileLayer, GeoJSON } from 'react-leaflet'
import { useQuery } from '@tanstack/react-query'
import { getTrajectoriesGeoJSON, getDijkringdelenGeoJSON } from '../api/client'
import type { Layer, PathOptions, CircleMarkerOptions } from 'leaflet'
import L from 'leaflet'

// Klasse-indeling conform OptimaliseRing (overstromingskans 1/jaar)
export const P_CLASSES: Array<{ max: number; color: string; label: string }> = [
  { max: 1 / 113_000, color: '#00bcd4', label: '< 1/113.000' },
  { max: 1 / 57_000,  color: '#2196f3', label: '1/113.000 – 1/57.000' },
  { max: 1 / 28_000,  color: '#9c27b0', label: '1/57.000 – 1/28.000' },
  { max: 1 / 14_000,  color: '#e91e63', label: '1/28.000 – 1/14.000' },
  { max: 1 / 6_300,   color: '#f44336', label: '1/14.000 – 1/6.300' },
  { max: 1 / 2_800,   color: '#ff9800', label: '1/6.300 – 1/2.800' },
  { max: 1 / 1_600,   color: '#ffeb3b', label: '1/2.800 – 1/1.600' },
  { max: 1 / 800,     color: '#8bc34a', label: '1/1.600 – 1/800' },
  { max: Infinity,    color: '#1b5e20', label: '> 1/800' },
]

export function pColor(p: number | null | undefined): string {
  if (p == null) return '#94a3b8'
  for (const cls of P_CLASSES) {
    if (p < cls.max) return cls.color
  }
  return P_CLASSES[P_CLASSES.length - 1].color
}

function lineStyle(feature?: GeoJSON.Feature): PathOptions {
  const p = feature?.properties?.p_year
  return { color: pColor(p), weight: 2.5, opacity: 0.85 }
}

function dijkringStyle(feature?: GeoJSON.Feature): PathOptions {
  const p = feature?.properties?.p0_avg
  return { color: pColor(p), weight: 3, opacity: 0.9, fillOpacity: 0 }
}

interface Props {
  onFeatureClick?: (props: Record<string, unknown>) => void
}

export default function MapView({ onFeatureClick }: Props) {
  const { data: trajData } = useQuery({
    queryKey: ['geo-trajectories'],
    queryFn: getTrajectoriesGeoJSON,
    staleTime: 30_000,
  })

  const { data: dijkData, isLoading: loadingDijk } = useQuery({
    queryKey: ['geo-dijkringdelen'],
    queryFn: getDijkringdelenGeoJSON,
    staleTime: 5 * 60_000,
  })

  function makeHandlers(data: GeoJSON.FeatureCollection | undefined) {
    return (feature: GeoJSON.Feature, layer: Layer) => {
      const p = feature.properties
      if (!p) return
      ;(layer as any).on('click', () => onFeatureClick?.(p as Record<string, unknown>))
      const p0 = p.p0_avg ?? p.p0_max ?? p.p_year
      const norm = p.norm_per_jaar ?? p.norm
      const normStr = norm ? `1/${Math.round(1 / norm).toLocaleString('nl-NL')}` : '—'
      const p0Str = p0 != null ? `1/${Math.round(1 / p0).toLocaleString('nl-NL')}` : '—'
      ;(layer as any).bindTooltip(
        `<b>${p.naam ?? p.id ?? ''}</b><br/>P₀: ${p0Str}/jr<br/>Norm: ${normStr}/jr`,
        { sticky: true }
      )
    }
  }

  return (
    <MapContainer
      center={[52.15, 5.3]}
      zoom={7}
      className="h-full w-full"
      zoomControl={true}
    >
      <TileLayer
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
      />

      {/* Dijkringdelen 2011 — gekleurde lijnen op P₀ */}
      {dijkData && dijkData.features.length > 0 && (
        <GeoJSON
          key={dijkData.features.length}
          data={dijkData}
          style={dijkringStyle}
          onEachFeature={makeHandlers(dijkData)}
        />
      )}

      {/* Opgeslagen trajecten — dikker, met p_year kleur */}
      {trajData && trajData.features.some(f => f.geometry) && (
        <GeoJSON
          key={'traj-' + trajData.features.length}
          data={trajData}
          style={lineStyle}
          onEachFeature={makeHandlers(trajData)}
        />
      )}

      {loadingDijk && (
        <div className="absolute top-2 left-1/2 -translate-x-1/2 z-[1000] bg-white/90 text-xs text-gray-500 px-3 py-1.5 rounded-full shadow">
          Dijkringdelen laden…
        </div>
      )}
    </MapContainer>
  )
}
