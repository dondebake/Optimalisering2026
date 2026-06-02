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

export interface CostFunctionParams {
  C: number        // vaste kosten [M EUR]
  b: number        // variabele kosten [M EUR/m]
  lam: number      // schaalparameter [1/m]
  omega: number    // onderhoudsfractie
}

export interface InvestmentRow {
  year: number
  delta_h: number
  W: number
  cost_meur: number
  cost_A_meur: number
  cost_BC_meur: number
}

export interface OptimizeRequest {
  trajectory_id: string
  scenario_id: string
  candidates: Measure[]
  risk_params: RiskParams
  objective: ObjectiveType
  budget: number | null
  solver: 'brute_force' | 'pyomo' | 'continuous'
  cost_function?: CostFunctionParams | null
}

export type JobStatus = 'pending' | 'running' | 'done' | 'failed'

export interface ValDijkring {
  Dijkring: string
  Naam: string
  norm_per_jaar: number
  n_trajecten: number
}

export interface SchadeScenario {
  scenario_id: number
  scenario_naam: string   // 'Laag' | 'Verwacht' | 'Hoog'
  schade_meur: number     // V0 in M EUR
}

export interface GammaScenario {
  scenario_id: number
  scenario_naam: string   // bijv. 'Transatlantic Market (TM)'
  gamma: number           // economische groeivoet [1/jaar]
}

export interface ValReferenceData {
  dijkring: string
  dijkringdeel: number
  schade_scenarios: SchadeScenario[]
  gamma_scenarios: GammaScenario[]
}

export interface ValTrajectory {
  Dijkring: string
  DijkringDeel: number
  DijkringTraject: number
  Naam: string
  alpha_per_m: number
  p0_per_jaar: number
  eta_m_per_jaar: number
  norm_per_jaar: number
  // kostenfunctie: IC(Dh) = C_exp * exp(lambda_exp_per_m * Dh) * Dh^b_exp  [M EUR]
  lambda_exp_per_m: number | null
  C_exp: number | null
  b_exp: number | null
  Omega: number | null
}

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
  investments: InvestmentRow[] | null
  input_payload: InputPayload | null
}

export interface InputPayload {
  trajectory: Trajectory
  scenario: Scenario
  candidates: Measure[]
  risk_params: RiskParams
  objective: ObjectiveType
  budget: number | null
  solver: 'brute_force' | 'pyomo'
}
