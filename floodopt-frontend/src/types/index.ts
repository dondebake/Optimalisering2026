export type MeasureType = 'dike_reinforcement' | 'room_for_river' | 'other'

export interface Measure {
  id: string
  type: MeasureType
  cost: number
  year: number
  effect: number
  location: string
  dependencies: string[]
}

export interface Scenario {
  id: string
  climate: string
  q_design: number
  h_design: number
  eta: number
}

export interface Trajectory {
  id: string
  norm: number
  length: number
  p0: number
  alpha: number
  base_year: number
  measures: Measure[]
  geometry?: object | null
}

export interface RiskParams {
  base_damage: number
  discount_rate: number
  gamma: number
  time_horizon: number
}

export type ObjectiveType = 'min_cost' | 'min_ncw' | 'max_risk_reduction'

export interface OptimizeRequest {
  trajectory_id: string
  scenario_id: string
  candidates: Measure[]
  risk_params: RiskParams
  objective: ObjectiveType
  budget: number | null
  solver: 'brute_force' | 'pyomo'
}

export type JobStatus = 'pending' | 'running' | 'done' | 'failed'

export interface PSeriesPoint {
  year: number
  p: number
  p_mid: number
}

export interface OptimizeResponse {
  job_id: string
  status: JobStatus
  trajectory_id: string
  scenario_id: string
  objective: ObjectiveType
  solver: string
  selected_measure_ids: string[]
  total_ncw: number | null
  risk_ncw: number | null
  investment_npv: number | null
  objective_value: number | null
  p_series: PSeriesPoint[] | null
}
