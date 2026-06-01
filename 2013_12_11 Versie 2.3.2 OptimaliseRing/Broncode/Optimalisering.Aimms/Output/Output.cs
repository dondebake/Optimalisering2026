#region Copyright -------------------------------------------------------
// Copyright © 2009, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Aimms;
using CenterSpace.NMath.Core;

namespace OptimaliseRing.Aimms
{
  /// <summary>
  /// Output
  /// </summary>
  [Serializable]
  public class Output : IDisposable
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Solutions
    /// index: s
    /// </summary>
    private Solutions m_solutions;
    /// <summary>
    /// Segmenten/ Trajecten
    /// index: l (L)
    /// </summary>
    private Segments m_segments;
    /// <summary>
    /// Scenarios
    /// index: i
    /// </summary>
    private Scenarios m_scenarios;
    /// <summary>
    /// DikeIncrease
    /// </summary>
    private DikeIncrease m_dikeIncrease;
    /// <summary>
    /// CostObjectives
    /// </summary>
    private CostObjectives m_costObjectives;
    /// <summary>
    /// Probabilities
    /// </summary>
    private Probabilities m_probabilities;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets Solutions
    /// </summary>
    /// <value>The solutions.</value>
    public Solutions Solutions
    {
      get { return this.m_solutions; }
    }

    /// <summary>
    /// Gets the segments.
    /// </summary>
    /// <value>The segments.</value>
    public Segments Segments
    {
      get { return this.m_segments; }
    }

    /// <summary>
    /// Gets the scenarios.
    /// </summary>
    /// <value>The scenarios.</value>
    public Scenarios Scenarios
    {
      get { return this.m_scenarios; }
    }

    /// <summary>
    /// Gets the dike increase.
    /// </summary>
    /// <value>The dike increase.</value>
    public DikeIncrease DikeIncrease
    {
      get { return this.m_dikeIncrease; }
    }

    /// <summary>
    /// Gets the cost objectives.
    /// </summary>
    /// <value>The cost objectives.</value>
    public CostObjectives CostObjectives
    {
      get { return this.m_costObjectives; }
    }

    /// <summary>
    /// Gets the probabilities.
    /// </summary>
    /// <value>The probabilities.</value>
    public Probabilities Probabilities
    {
      get { return this.m_probabilities; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="Output"/> class.
    /// </summary>
    public Output()
    {
      this.m_solutions = new Solutions();
      this.m_segments = new Segments();
      this.m_scenarios = new Scenarios();
      this.m_dikeIncrease = new DikeIncrease();
      this.m_costObjectives = new CostObjectives();
      this.m_probabilities = new Probabilities();
    }

    #endregion Constructors --------------------------------------------------

    #region Member functions -------------------------------------------------

    /// <summary>
    /// Gets the results.
    /// </summary>
    /// <param name="project">The project.</param>
    public bool GetResults(Project project, Segments segments)
    {
      if (project != null)
      {
        this.m_segments = segments;

        // Ophalen oplossingen
        if (this.m_solutions.GetResults(project))
        {
          // Ophalen segmenten
          //if (this.m_segments.GetResults(project))
          //{
            if (this.m_scenarios.GetResults(project))
            {
              if (this.m_dikeIncrease.GetResults(project, this.m_scenarios, this.m_segments))
              {
                if (this.m_costObjectives.GetResults(project, this.m_scenarios))
                {
                  if (this.m_probabilities.GetResults(project, this.m_scenarios, this.m_segments))
                  {
                    return true;
                  }
                }
              }
            }
          //}
        }
      }
      return false;
    }

    /// <summary>
    /// DijkVerhoging
    /// </summary>
    public struct DijkVerhoging
    {
      /// <summary>
      /// Jaar (K)
      /// </summary>
      public int Jaar; // K
      /// <summary>
      /// Increase
      /// </summary>
      public double Increase;
      /// <summary>
      /// Costs
      /// </summary>
      public double Costs;

      /// <summary>
      /// Initializes a new instance of the <see cref="DijkVerhoging"/> struct.
      /// </summary>
      /// <param name="jaar">The jaar.</param>
      /// <param name="increase">The increase.</param>
      /// <param name="costs">The costs.</param>
      public DijkVerhoging(int jaar, double increase, double costs)
      {
        this.Jaar = jaar;
        this.Increase = increase;
        this.Costs = costs;
      }
    }

    /// <summary>
    /// Gets the dijkverhogingen per traject.
    /// </summary>
    /// <param name="trajectNaam">The traject naam.</param>
    /// <returns></returns>
    public List<DijkVerhoging> GetDijkverhogingenPerTraject(int indexScenario, string trajectNaam)
    {
      List<DijkVerhoging> retval = new List<DijkVerhoging>();

      if (this.m_segments.Count > 0)
      {
        int indexSegment = -1;
        foreach (Segment segment in this.m_segments)
        {
          if (segment.SegmentName == trajectNaam)
          {
            indexSegment = this.m_segments.IndexOf(segment);
            break;
          }
        }

        if (indexSegment > -1)
        {
          if (this.m_dikeIncrease.Moment_year.Length > 0)
          {
            int indexInvestering = 0;
            for (int jaar = 0; jaar < this.m_dikeIncrease.Moment_year.Length; jaar++)
            {
              if (this.m_dikeIncrease.Dike_increase[indexSegment, jaar] > 0.0)
              {
                double kosten = this.m_dikeIncrease.Update_undiscounted_costs.GetLength(2) > indexInvestering
                  ? this.m_dikeIncrease.Update_undiscounted_costs[indexScenario, indexSegment, indexInvestering++] : double.MinValue;
                retval.Add(new DijkVerhoging(this.m_dikeIncrease.Moment_year[jaar]
                  , this.m_dikeIncrease.Dike_increase[indexSegment, jaar], kosten));
              }
            }
          }
        }
      }

      return retval;
    }

    /// <summary>
    /// Gets the index of the traject.
    /// </summary>
    /// <param name="trajectNaam">The traject naam.</param>
    /// <returns></returns>
    public int GetTrajectIndex(string trajectNaam)
    {
      int retval = -1;
      foreach (Segment segment in this.m_segments)
      {
        if (string.Compare(segment.SegmentName, trajectNaam.Trim(), true) == 0)
        {
          retval = this.m_segments.IndexOf(segment);
          break;
        }
      }
      return retval;
    }

    /// <summary>
    /// Gets the index of the scenario.
    /// </summary>
    /// <param name="scenarioNaam">The scenario naam.</param>
    /// <returns></returns>
    public int GetScenarioIndex(string scenarioNaam)
    {
      int retval = -1;
      foreach (Scenario scenario in this.m_scenarios)
      {
        if (scenario.ScenarioName == scenarioNaam)
        {
          retval = this.m_scenarios.IndexOf(scenario);
          break;
        }
      }
      return retval;
    }

    /// <summary>
    /// Gets Matrix data {P, Pmidden, per Traject) Per scenario
    /// </summary>
    /// <param name="scenarioNaam">The scenario naam.</param>
    /// <returns></returns>
    public DoubleMatrix GetMatrixDataPerScenario(string scenarioNaam)
    {

      int indexScenario = this.GetScenarioIndex(scenarioNaam);
      int aantalJaren = this.m_probabilities.All_years.Length;
      int aantalSegmenten = this.m_segments.Count;

      DoubleMatrix matrixData = new DoubleMatrix(aantalJaren, Convert.ToInt32(MatrixKolom.AANTAL) + aantalSegmenten);

      for (int indexYear = 0; indexYear < aantalJaren; indexYear++)
      {

        // @@ PMIDDEN = P_mid_final
        // @@ P       = Max alle segmenten

        matrixData[indexYear, Convert.ToInt32(MatrixKolom.JAAR)] = Convert.ToDouble(this.m_probabilities.All_years[indexYear]);
        matrixData[indexYear, Convert.ToInt32(MatrixKolom.PMIDDEN)] = this.m_probabilities.P_mid_final[indexScenario, indexYear];

        // Bepaal minimum en maximum van PMidden over de scenario's
        double minPMid = double.MaxValue;
        double maxPMid = double.MinValue;
        for (int indexScenarioTmp = 0; indexScenarioTmp < this.m_scenarios.Count; indexScenarioTmp++)
        {
          minPMid = Math.Min(minPMid, this.m_probabilities.P_mid_final[indexScenarioTmp, indexYear]);
          maxPMid = Math.Max(maxPMid, this.m_probabilities.P_mid_final[indexScenarioTmp, indexYear]);
        }
        matrixData[indexYear, Convert.ToInt32(MatrixKolom.PMIDMIN)] = minPMid;
        matrixData[indexYear, Convert.ToInt32(MatrixKolom.PMIDMAX)] = maxPMid;

        double maxvalue = double.MinValue;
        for (int indexSegment = 0; indexSegment < aantalSegmenten; indexSegment++)
        {
          maxvalue = Math.Max(maxvalue, this.m_probabilities.Segment_flood_probability[indexScenario, indexSegment, indexYear]);
          matrixData[indexYear, Convert.ToInt32(MatrixKolom.AANTAL) + indexSegment]
            = this.m_probabilities.Segment_flood_probability[indexScenario, indexSegment, indexYear];
        }

        matrixData[indexYear, Convert.ToInt32(MatrixKolom.P)] = maxvalue;
      }

      return matrixData;
    }

    /// <summary>
    /// Gets the overstromings kans per scenario.
    /// </summary>
    /// <param name="scenarioNaam">The scenario naam.</param>
    /// <param name="jaar">The jaar.</param>
    /// <returns></returns>
    public double GetOverstromingsKansPerScenario(string scenarioNaam, int jaar)
    {
      int indexScenario = this.GetScenarioIndex(scenarioNaam);
      int aantalJaren = this.m_probabilities.All_years.Length;
      int aantalSegmenten = this.m_segments.Count;
      double maxvalue = double.MinValue;

      for (int indexYear = 0; indexYear < aantalJaren; indexYear++)
      {
        int probabilityJaar = this.m_probabilities.All_years[indexYear];
        if (probabilityJaar == jaar)
        {

          for (int indexSegment = 0; indexSegment < aantalSegmenten; indexSegment++)
          {
            maxvalue = Math.Max(maxvalue, this.m_probabilities.Segment_flood_probability[indexScenario, indexSegment, indexYear]);

          }
        }
      }
      return maxvalue;
    }

    /// <summary>
    /// Gets the optimale overstromings kans per scenario.
    /// </summary>
    /// <param name="scenarioNaam">The scenario naam.</param>
    /// <param name="jaar">The jaar.</param>
    /// <returns></returns>
    public double GetOptimaleOverstromingsKansPerScenario(string scenarioNaam, int jaar)
    {
      int indexScenario = this.GetScenarioIndex(scenarioNaam);
      if (indexScenario > -1)
      {
        int aantalJaren = this.m_probabilities.All_years.Length;
        for (int indexYear = 0; indexYear < aantalJaren; indexYear++)
        {
          int probabilityJaar = this.m_probabilities.All_years[indexYear];

          if (probabilityJaar == jaar)
          {
            return this.m_probabilities.P_mid_final[indexScenario, indexYear];
          }
        }
      }
      return 0.0;
    }

    /// <summary>
    /// Gets the total_expected_costs2 per scenario.
    /// </summary>
    /// <param name="scenarioNaam">The scenario naam.</param>
    /// <returns></returns>
    public double GetTotal_expected_costs2PerScenario(string scenarioNaam)
    {
      int indexScenario = this.GetScenarioIndex(scenarioNaam);
      if (indexScenario > -1)
      {
        return this.m_costObjectives.Total_expected_costs2[indexScenario];
      }
      return 0.0;
    }

    #endregion Member functions ----------------------------------------------

    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      this.m_solutions = null;
      this.m_segments = null;
      this.m_scenarios = null;
      this.m_dikeIncrease = null;
      this.m_costObjectives = null;
      this.m_probabilities = null;
    }

    #endregion
  }
}
