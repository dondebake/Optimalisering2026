import type { Measure, OptimizeRequest, OptimizeResponse, Scenario, Trajectory, ValDijkring, ValTrajectory, ValReferenceData } from '../types'

const BASE = '/api'

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...init,
  })
  if (!res.ok) {
    const text = await res.text()
    throw new Error(`${res.status} ${res.statusText}: ${text}`)
  }
  return res.json() as Promise<T>
}

export function postScenario(scenario: Scenario): Promise<Scenario> {
  return request('/scenarios', { method: 'POST', body: JSON.stringify(scenario) })
}

export function getScenario(id: string): Promise<Scenario> {
  return request(`/scenarios/${id}`)
}

export function postTrajectory(trajectory: Trajectory): Promise<Trajectory> {
  return request('/trajectories', { method: 'POST', body: JSON.stringify(trajectory) })
}

export function getTrajectory(id: string): Promise<Trajectory> {
  return request(`/trajectories/${id}`)
}

export function postOptimize(req: OptimizeRequest): Promise<OptimizeResponse> {
  return request('/optimize', { method: 'POST', body: JSON.stringify(req) })
}

export function getResult(jobId: string): Promise<OptimizeResponse> {
  return request(`/results/${jobId}`)
}

export function getTrajectoriesGeoJSON(): Promise<GeoJSON.FeatureCollection> {
  return request('/geo/trajectories')
}

export function getDijkringdelenGeoJSON(): Promise<GeoJSON.FeatureCollection> {
  return request('/geo/dijkringdelen')
}

export function getAllResults(): Promise<OptimizeResponse[]> {
  return request('/results')
}

export function deleteResult(jobId: string): Promise<void> {
  return request(`/results/${jobId}`, { method: 'DELETE' })
}

export function getValDijkringen(): Promise<ValDijkring[]> {
  return request('/validation/dijkringen')
}

export function getValReferenceData(dijkring: string, deel: number): Promise<ValReferenceData> {
  return request(`/validation/reference/${dijkring}/${deel}`)
}

export function getValTrajectories(dijkringId?: string): Promise<ValTrajectory[]> {
  const qs = dijkringId ? `?dijkring=${encodeURIComponent(dijkringId)}` : ''
  return request(`/validation/trajectories${qs}`)
}

export function valOptimize(dijkring: string, deel: number, traject: number): Promise<OptimizeResponse> {
  return request(`/validation/optimize/${dijkring}/${deel}/${traject}`, { method: 'POST' })
}

export async function submitOptimization(
  scenario: Scenario,
  trajectory: Trajectory,
  candidates: Measure[],
  req: Omit<OptimizeRequest, 'trajectory_id' | 'scenario_id' | 'candidates'>,
): Promise<OptimizeResponse> {
  await postScenario(scenario)
  await postTrajectory(trajectory)
  return postOptimize({
    ...req,
    trajectory_id: trajectory.id,
    scenario_id: scenario.id,
    candidates,
  })
}
