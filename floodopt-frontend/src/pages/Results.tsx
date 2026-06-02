import { useParams, Link, useNavigate } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { getResult, getTrajectory } from '../api/client'
import { StatusBadge } from '../components/StatusBadge'
import PSeriesChart from '../components/PSeriesChart'
import type { InputPayload, Measure, OptimizeResponse } from '../types'

const OBJECTIVE_LABEL: Record<string, string> = {
  min_cost: 'MIN_COST — minimale investering',
  min_ncw: 'MIN_NCW — minimale totale NCW',
  max_risk_reduction: 'MAX_RR — maximale risicoreductie',
}

function fmt(n: number | null, prefix = '€ ') {
  if (n == null) return '—'
  return `${prefix}${n.toLocaleString('nl-NL', { maximumFractionDigits: 0 })}`
}

function fmtM(n: number | null) {
  if (n == null) return '—'
  return `€ ${(n / 1e6).toLocaleString('nl-NL', { maximumFractionDigits: 1 })} M`
}

function fmtP(p: number) {
  return `1/${Math.round(1 / p).toLocaleString('nl-NL')}/jr`
}

function Row({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex justify-between py-1.5 border-b border-gray-50 last:border-0">
      <span className="text-gray-500 text-sm">{label}</span>
      <span className="text-sm font-medium text-right">{value}</span>
    </div>
  )
}

function Section({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div className="border border-gray-200 rounded-xl overflow-hidden">
      <div className="px-4 py-2.5 bg-gray-50 border-b border-gray-200">
        <h2 className="text-sm font-semibold text-gray-700">{title}</h2>
      </div>
      <div className="px-4 py-3">{children}</div>
    </div>
  )
}

function MeasureTable({ measures, selectedIds }: { measures: Measure[]; selectedIds: string[] }) {
  const selSet = new Set(selectedIds)
  return (
    <table className="w-full text-xs">
      <thead>
        <tr className="text-gray-400 uppercase tracking-wide">
          <th className="text-left pb-1.5">ID</th>
          <th className="text-left pb-1.5">Type</th>
          <th className="text-right pb-1.5">Δh [m]</th>
          <th className="text-right pb-1.5">Kosten</th>
          <th className="text-right pb-1.5">Jaar</th>
          <th className="text-center pb-1.5">Geselecteerd</th>
        </tr>
      </thead>
      <tbody className="divide-y divide-gray-50">
        {measures.map(m => {
          const sel = selSet.has(m.id)
          return (
            <tr key={m.id} className={sel ? 'bg-green-50' : ''}>
              <td className="py-1.5 font-mono font-medium">{m.id}</td>
              <td className="py-1.5 text-gray-600">{m.type.replace('_', ' ')}</td>
              <td className="py-1.5 text-right tabular-nums">{m.effect.toFixed(2)}</td>
              <td className="py-1.5 text-right tabular-nums">{fmtM(m.cost)}</td>
              <td className="py-1.5 text-right tabular-nums">{m.year}</td>
              <td className="py-1.5 text-center">
                {sel ? <span className="text-green-600 font-bold">✓</span> : <span className="text-gray-300">—</span>}
              </td>
            </tr>
          )
        })}
      </tbody>
    </table>
  )
}

function NcwTable({ data, inp }: { data: OptimizeResponse; inp: InputPayload }) {
  const { risk_params, trajectory, scenario } = inp
  const { discount_rate: delta, gamma, base_damage: v0, time_horizon: T } = risk_params
  const baseYear = trajectory.base_year

  // NCW₀ = verwachte schade ZONDER maatregelen
  // NCW₀ = V₀ · P₀ · Σ exp(β·s)  voor s=0..T-1,  β = α·η + γ − δ
  const beta = trajectory.alpha * scenario.eta + gamma - delta
  const geomSum = Math.abs(beta) < 1e-10
    ? T
    : (Math.exp(beta * T) - 1) / (Math.exp(beta) - 1)
  const ncw0 = v0 * trajectory.p0 * geomSum

  const baten = ncw0 - (data.risk_ncw ?? 0)
  const bcr = (data.investment_npv ?? 0) > 0
    ? baten / (data.investment_npv ?? 1)
    : null

  const selectedMeasures = inp.candidates.filter(m =>
    data.selected_measure_ids.includes(m.id)
  )

  // NPV per maatregel: IC / (1 + delta)^(jaar - basisjaar)
  const measureRows = selectedMeasures.map(m => {
    const years = m.year - baseYear
    const npv = m.cost / Math.pow(1 + delta, years)
    return { ...m, years, npv }
  })

  const investTotal = measureRows.reduce((s, m) => s + m.npv, 0)

  return (
    <div className="space-y-5">

      {/* Formules */}
      <div className="bg-gray-50 rounded-lg p-4 text-xs text-gray-600 space-y-2">
        <div className="font-semibold text-gray-700 mb-1">Formules</div>
        <div>
          <span className="font-mono">NCW<sub>risico</sub></span>
          {' = '}
          <span className="font-mono">∑ P(s) · V₀ · e<sup>(γ−δ)·s</sup></span>
          {' voor s = 0 … T−1'}
        </div>
        <div>
          <span className="font-mono">NPV<sub>i</sub></span>
          {' = '}
          <span className="font-mono">IC<sub>i</sub> / (1 + δ)<sup>t<sub>i</sub>−t₀</sup></span>
        </div>
        <div className="grid grid-cols-2 sm:grid-cols-4 gap-2 pt-1">
          {[
            ['V₀', fmt(v0)],
            ['δ', `${(delta * 100).toFixed(2)} %`],
            ['γ', `${(gamma * 100).toFixed(1)} %`],
            ['T', `${T} jr · t₀ = ${baseYear}`],
          ].map(([k, v]) => (
            <div key={k} className="bg-white rounded px-2 py-1 border border-gray-200">
              <span className="font-mono font-semibold text-gray-800">{k}</span>
              <span className="ml-2 text-gray-600">{v}</span>
            </div>
          ))}
        </div>
      </div>

      {/* Investering per maatregel */}
      {measureRows.length > 0 && (
        <div>
          <div className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2">
            Investering per geselecteerde maatregel
          </div>
          <table className="w-full text-xs">
            <thead>
              <tr className="text-gray-400 uppercase tracking-wide border-b border-gray-100">
                <th className="text-left pb-1.5">Maatregel</th>
                <th className="text-right pb-1.5">Δh [m]</th>
                <th className="text-right pb-1.5">Bruto kosten</th>
                <th className="text-right pb-1.5">Jaar</th>
                <th className="text-right pb-1.5">Jaren na t₀</th>
                <th className="text-right pb-1.5">Discontofactor</th>
                <th className="text-right pb-1.5 font-semibold text-gray-600">NPV</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-50">
              {measureRows.map(m => (
                <tr key={m.id}>
                  <td className="py-1.5 font-mono font-medium text-green-700">{m.id}</td>
                  <td className="py-1.5 text-right tabular-nums">{m.effect.toFixed(2)}</td>
                  <td className="py-1.5 text-right tabular-nums">{fmtM(m.cost)}</td>
                  <td className="py-1.5 text-right tabular-nums">{m.year}</td>
                  <td className="py-1.5 text-right tabular-nums text-gray-500">{m.years}</td>
                  <td className="py-1.5 text-right tabular-nums text-gray-500">
                    {(1 / Math.pow(1 + delta, m.years)).toFixed(3)}
                  </td>
                  <td className="py-1.5 text-right tabular-nums font-medium">{fmtM(m.npv)}</td>
                </tr>
              ))}
              <tr className="border-t border-gray-200 font-semibold">
                <td colSpan={6} className="pt-2 text-gray-600">Totaal investering (NPV)</td>
                <td className="pt-2 text-right tabular-nums text-blue-700">{fmtM(investTotal)}</td>
              </tr>
            </tbody>
          </table>
          {Math.abs(investTotal - (data.investment_npv ?? 0)) > 1000 && (
            <div className="text-xs text-amber-600 mt-1">
              Let op: berekend totaal wijkt af van opgeslagen waarde ({fmtM(data.investment_npv)}) door afronding.
            </div>
          )}
        </div>
      )}

      {/* NCW samenvatting */}
      <div>
        <div className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2">
          NCW-opbouw
        </div>
        <table className="w-full text-sm">
          <tbody>
            <tr className="border-b border-gray-100">
              <td className="py-2 text-gray-600">
                NCW<sub>risico</sub> — verwachte toekomstige schade
              </td>
              <td className="py-2 text-right tabular-nums font-medium">{fmt(data.risk_ncw)}</td>
            </tr>
            <tr className="border-b border-gray-100">
              <td className="py-2 text-gray-600">
                NCW<sub>investering</sub> — verdisconteerde investeringen
              </td>
              <td className="py-2 text-right tabular-nums font-medium">{fmt(data.investment_npv)}</td>
            </tr>
            <tr className="bg-blue-50">
              <td className="py-2.5 px-2 font-semibold text-gray-800">NCW totaal</td>
              <td className="py-2.5 px-2 text-right tabular-nums font-bold text-blue-700 text-base">
                {fmt(data.total_ncw)}
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      {/* Batenanalyse */}
      <div className="border-t-2 border-green-100 pt-4">
        <div className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2">
          Batenanalyse — vermeden schade
        </div>
        <div className="bg-gray-50 rounded p-3 mb-3 text-xs text-gray-600 space-y-1">
          <div>
            <span className="font-mono">NCW₀</span>
            {' = '}
            <span className="font-mono">V₀ · P₀ · (e<sup>βT</sup> − 1) / (e<sup>β</sup> − 1)</span>
            {'  met '}
            <span className="font-mono">β = α·η + γ − δ = {beta.toFixed(4)}</span>
          </div>
        </div>
        <table className="w-full text-sm">
          <tbody>
            <tr className="border-b border-gray-100">
              <td className="py-2 text-gray-500">
                NCW₀ — verwachte schade <em>zonder</em> maatregelen
              </td>
              <td className="py-2 text-right tabular-nums font-medium text-gray-700">{fmt(ncw0)}</td>
            </tr>
            <tr className="border-b border-gray-100">
              <td className="py-2 text-gray-500">
                NCW<sub>risico</sub> — verwachte schade <em>met</em> maatregelen
              </td>
              <td className="py-2 text-right tabular-nums font-medium">{fmt(data.risk_ncw)}</td>
            </tr>
            <tr className="border-b border-green-200 bg-green-50">
              <td className="py-2.5 px-2 font-semibold text-green-800">
                Baten (vermeden schade)
              </td>
              <td className="py-2.5 px-2 text-right tabular-nums font-bold text-green-700 text-base">
                {fmt(baten)}
              </td>
            </tr>
            <tr className="border-b border-gray-100">
              <td className="py-2 text-gray-500">
                NCW<sub>investering</sub>
              </td>
              <td className="py-2 text-right tabular-nums font-medium">{fmt(data.investment_npv)}</td>
            </tr>
            {bcr !== null && (
              <tr className={`font-semibold ${bcr >= 1 ? 'bg-green-50' : 'bg-amber-50'}`}>
                <td className="py-2.5 px-2 text-gray-800">
                  BCR — baten-kostenratio
                  <span className="ml-2 text-xs font-normal text-gray-500">
                    {bcr >= 1 ? '(investering loont)' : '(investering loont niet)'}
                  </span>
                </td>
                <td className={`py-2.5 px-2 text-right tabular-nums font-bold text-xl ${bcr >= 1 ? 'text-green-700' : 'text-amber-700'}`}>
                  {bcr.toFixed(2)}
                </td>
              </tr>
            )}
          </tbody>
        </table>
        <div className="mt-2 text-xs text-gray-400">
          V₀ bevat directe schade + gemonetariseerde slachtoffers + getroffenen
          (conform OptimaliseRing 2011, {
            inp.risk_params.base_damage >= 1e9
              ? `scenario ${fmtM(inp.risk_params.base_damage)}`
              : fmtM(inp.risk_params.base_damage)
          }).
          Actualisatie via LDO ROR 2025 is voorzien in stap 5.5.
        </div>
      </div>
    </div>
  )
}

export default function Results() {
  const { jobId } = useParams<{ jobId: string }>()
  const navigate = useNavigate()

  const { data, error } = useQuery({
    queryKey: ['result', jobId],
    queryFn: () => getResult(jobId!),
    refetchInterval: (query) => {
      const status = query.state.data?.status
      return status === 'done' || status === 'failed' ? false : 2000
    },
    enabled: !!jobId,
  })

  const { data: trajectory } = useQuery({
    queryKey: ['trajectory', data?.trajectory_id],
    queryFn: () => getTrajectory(data!.trajectory_id),
    enabled: !!data?.trajectory_id,
  })

  function handleOpnieuw(withChanges = false) {
    const p = data?.input_payload as InputPayload | null
    navigate('/optimize', {
      state: {
        trajectory: p?.trajectory ?? (trajectory ? {
          id: trajectory.id, norm: trajectory.norm, length: trajectory.length,
          p0: trajectory.p0, alpha: trajectory.alpha, base_year: trajectory.base_year,
          measures: [],
        } : undefined),
        scenario: p?.scenario,
        candidates: p?.candidates,
        risk_params: p?.risk_params,
        _withChanges: withChanges,
      },
    })
  }

  if (error) return (
    <div className="max-w-xl mx-auto p-6">
      <div className="bg-red-50 border border-red-200 rounded p-4 text-red-700">
        Fout: {error instanceof Error ? error.message : 'Onbekend'}
      </div>
    </div>
  )

  if (!data) return (
    <div className="max-w-xl mx-auto p-6 text-gray-500 text-sm animate-pulse">
      Resultaat ophalen…
    </div>
  )

  const inp = data.input_payload as InputPayload | null

  return (
    <div className="max-w-4xl mx-auto p-6 space-y-5">

      {/* Header */}
      <div className="flex items-start justify-between flex-wrap gap-3">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Resultaat</h1>
          <div className="text-xs text-gray-400 font-mono mt-0.5">{data.job_id}</div>
        </div>
        <div className="flex items-center gap-3">
          <StatusBadge status={data.status} />
          <div className="flex gap-2">
            <button
              onClick={() => handleOpnieuw(false)}
              className="text-sm px-3 py-1.5 rounded border border-gray-300 hover:border-blue-400 hover:text-blue-700 transition-colors"
            >
              Opnieuw ↺
            </button>
            <button
              onClick={() => handleOpnieuw(true)}
              className="text-sm px-3 py-1.5 rounded bg-blue-600 text-white hover:bg-blue-700 transition-colors"
            >
              Opnieuw met aanpassingen →
            </button>
          </div>
        </div>
      </div>

      {/* Status melding */}
      {(data.status === 'pending' || data.status === 'running') && (
        <div className="bg-blue-50 border border-blue-200 rounded p-4 text-sm text-blue-700">
          Berekening is{data.status === 'pending' ? ' in de wachtrij' : ' bezig'}… Pagina ververst automatisch.
        </div>
      )}
      {data.status === 'failed' && (
        <div className="bg-red-50 border border-red-200 rounded p-4 text-sm text-red-700 space-y-2">
          <div className="font-semibold">De optimalisatie is mislukt.</div>
          {data.error_message && (
            <pre className="text-xs bg-red-100 rounded p-3 overflow-x-auto whitespace-pre-wrap max-h-48">
              {data.error_message}
            </pre>
          )}
          {!data.error_message && (
            <div className="text-xs text-red-500">Herstart de Celery worker (terminal 3) en probeer opnieuw.</div>
          )}
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-5">

        {/* Linker kolom — invoerparameters */}
        <div className="space-y-4">

          {/* Doelfunctie + solver */}
          <Section title="Optimalisatie-instellingen">
            <Row label="Doelfunctie" value={OBJECTIVE_LABEL[data.objective] ?? data.objective} />
            <Row label="Solver" value={data.solver} />
            {inp?.budget != null && (
              <Row label="Budget" value={fmt(inp.budget)} />
            )}
          </Section>

          {/* Traject */}
          {inp && (
            <Section title="Dijktraject">
              <Row label="ID" value={inp.trajectory.id} />
              <Row label="P₀ (basisjaar)" value={fmtP(inp.trajectory.p0)} />
              <Row label="Norm" value={fmtP(inp.trajectory.norm)} />
              <Row label="α" value={`${inp.trajectory.alpha} /m`} />
              <Row label="Lengte" value={`${inp.trajectory.length} km`} />
              <Row label="Basisjaar" value={String(inp.trajectory.base_year)} />
            </Section>
          )}

          {/* Scenario */}
          {inp && (
            <Section title="Klimaatscenario">
              <Row label="ID" value={inp.scenario.id} />
              <Row label="Klimaat" value={inp.scenario.climate} />
              <Row label="η (zeespiegelstijging)" value={`${inp.scenario.eta} m/jr`} />
              <Row label="q ontwerp" value={`${inp.scenario.q_design} m³/s`} />
              <Row label="h ontwerp" value={`${inp.scenario.h_design} m+NAP`} />
            </Section>
          )}

          {/* Risicoparameters */}
          {inp && (
            <Section title="Risicoparameters">
              <Row label="V₀ (basisschade)" value={fmt(inp.risk_params.base_damage)} />
              <Row label="δ (discontovoet)" value={`${(inp.risk_params.discount_rate * 100).toFixed(2)} %`} />
              <Row label="γ (economische groei)" value={`${(inp.risk_params.gamma * 100).toFixed(1)} %`} />
              <Row label="Tijdshorizon T" value={`${inp.risk_params.time_horizon} jaar`} />
            </Section>
          )}
        </div>

        {/* Rechter kolom — resultaten */}
        <div className="space-y-4">

          {/* Financiële samenvatting */}
          {data.status === 'done' && (
            <Section title="Financieel resultaat">
              <Row label="Doelfunctiewaarde" value={fmt(data.objective_value)} />
              <Row label="Totale NCW" value={fmt(data.total_ncw)} />
              <Row label="Risico-NCW (verwachte schade)" value={fmt(data.risk_ncw)} />
              <Row label="Investeringskosten (NPV)" value={fmt(data.investment_npv)} />
            </Section>
          )}

          {/* Continue optimizer — investeringsschema */}
          {data.status === 'done' && data.investments && data.investments.length > 0 && (
            <Section title={`Investeringsschema — ${data.investments.length} investering(en)`}>
              <table className="w-full text-xs">
                <thead>
                  <tr className="text-gray-400 uppercase tracking-wide border-b border-gray-100">
                    <th className="text-left pb-1.5">#</th>
                    <th className="text-right pb-1.5">Jaar</th>
                    <th className="text-right pb-1.5">Δh [m]</th>
                    <th className="text-right pb-1.5">W [m]</th>
                    <th className="text-right pb-1.5">Kosten (A)</th>
                    <th className="text-right pb-1.5">Kosten (B+C)</th>
                    <th className="text-right pb-1.5 text-gray-600 font-semibold">Totaal</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-50">
                  {data.investments.map((inv, i) => (
                    <tr key={i} className="hover:bg-gray-50">
                      <td className="py-1.5 font-mono text-green-700 font-semibold">{i + 1}</td>
                      <td className="py-1.5 text-right tabular-nums">{inv.year}</td>
                      <td className="py-1.5 text-right tabular-nums">{inv.delta_h.toFixed(2)}</td>
                      <td className="py-1.5 text-right tabular-nums text-gray-400">{inv.W.toFixed(2)}</td>
                      <td className="py-1.5 text-right tabular-nums">{inv.cost_A_meur.toFixed(1)} M€</td>
                      <td className="py-1.5 text-right tabular-nums">{inv.cost_BC_meur.toFixed(1)} M€</td>
                      <td className="py-1.5 text-right tabular-nums font-medium">{inv.cost_meur.toFixed(1)} M€</td>
                    </tr>
                  ))}
                </tbody>
              </table>
              <div className="mt-2 text-xs text-gray-400">
                A = kosten door normenachterstand · B+C = kosten door klimaat + economische groei · W = cumulatieve eerdere verhoging
              </div>
            </Section>
          )}

          {/* Kandidaatmaatregelen (discrete solvers) */}
          {inp && inp.candidates.length > 0 && !data.investments && (
            <Section title={`Kandidaatmaatregelen (${inp.candidates.length})`}>
              <MeasureTable
                measures={inp.candidates}
                selectedIds={data.selected_measure_ids}
              />
            </Section>
          )}
        </div>
      </div>

      {/* NCW-berekening — vol breedte */}
      {data.status === 'done' && inp && (
        <Section title="NCW-berekening">
          <NcwTable data={data} inp={inp} />
        </Section>
      )}

      {/* P(t) grafiek — vol breedte */}
      {data.status === 'done' && data.p_series && data.p_series.length > 0 && trajectory && (
        <Section title="P(t) — overstromingskans als functie van de tijd">
          <PSeriesChart
            series={data.p_series}
            norm={trajectory.norm}
            trajectoryId={data.trajectory_id}
          />
        </Section>
      )}

      <div className="pt-1 flex gap-4">
        <Link to="/runs" className="text-sm text-blue-600 hover:underline">← Alle runs</Link>
        <Link to="/" className="text-sm text-gray-500 hover:underline">← Dashboard</Link>
      </div>
    </div>
  )
}
