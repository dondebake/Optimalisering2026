import type { Measure, OptimizeRequest, OptimizeResponse, Scenario, Trajectory } from '../types'

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
