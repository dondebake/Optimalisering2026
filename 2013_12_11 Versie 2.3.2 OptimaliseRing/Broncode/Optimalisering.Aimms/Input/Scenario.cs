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

namespace OptimaliseRing.Aimms
{
  /// <summary>
  /// Enkele scenario waarmee een berekening gemaakt wordt.
  /// </summary>
  public class Scenario
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Scenarionaam
    /// </summary>
    private string m_scenarioName;
    /// <summary>
    /// Id gekregen in Aimms
    /// </summary>
    private int m_scenarioCardId;
    /// <summary>
    /// Economic_growth(i)
    /// var: gamma
    /// eenheid: 1/year
    /// var: i (lijst met scenarios)
    /// </summary>
    private double m_economic_growth;
    /// <summary>
    /// Discount_rate1(i)
    /// var: delta_1
    /// eenheid: 1/year
    /// var: i (lijst met scenarios)
    /// </summary>
    private double m_discount_rate1;
    /// <summary>
    /// Discount_rate2(i)
    /// var: delta_2
    /// eenheid: 1/year
    /// var: i (lijst met scenarios)
    /// </summary>
    private double m_discount_rate2;
    /// <summary>
    /// Initial_economic_value
    /// var: V_0
    /// eenheid: EUR
    /// </summary>
    private double m_initial_economic_value;

    // PER SEGMENT
    /// <summary>
    /// Initial_flood_probability
    /// var: P_0
    /// eenheid: 1/year
    /// domein: l (segments)
    /// </summary>
    private double[] m_initial_flood_probability;
    /// <summary>
    /// Increase_of_water_level
    /// var: eta
    /// eenheid: cm/year
    /// domein: l (segments)
    /// </summary>
    private double[] m_increase_of_water_level;
    /// <summary>
    /// Extreme_water
    /// var: alpha
    /// eenheid: 1/cm
    /// domein: l (segments)
    /// </summary>
    private double[] m_extreme_water;
    /// <summary>
    /// Exponential Cost Parameters
    /// domein: l (segments)
    /// </summary>
    private ExponentialCostParameters[] m_exponentialCostParameters;
    /// <summary>
    /// Quadratic Cost Parameters
    /// domein: l (segments)
    /// </summary>
    private QuadraticCostParameters[] m_quadraticCostParameters;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets the scenarionaam.
    /// </summary>
    /// <value>The scenarionaam.</value>
    public string ScenarioName
    {
      get { return m_scenarioName; }
    }

    /// <summary>
    /// Gets or sets the scenario card id.
    /// </summary>
    /// <value>The scenario card id.</value>
    public int ScenarioCardId
    {
      set { this.m_scenarioCardId = value; }
      get { return this.m_scenarioCardId; }
    }

    /// <summary>
    /// Gets Economic_growth(i)
    /// var: gamma
    /// eenheid: 1/year
    /// var: i (lijst met scenarios)
    /// </summary>
    /// <value>Economic_growth.</value>
    public double Economic_growth
    {
      get { return m_economic_growth; }
    }

    /// <summary>
    /// Gets Discount_rate1(i)
    /// var: delta_1
    /// eenheid: 1/year
    /// var: i (lijst met scenarios)
    /// </summary>
    /// <value>Discount_rate1.</value>
    public double Discount_rate1
    {
      get { return m_discount_rate1; }
    }

    /// <summary>
    /// Gets Discount_rate2(i)
    /// var: delta_2
    /// eenheid: 1/year
    /// var: i (lijst met scenarios)
    /// </summary>
    /// <value>Discount_rate2.</value>
    public double Discount_rate2
    {
      get { return m_discount_rate2; }
    }

    /// <summary>
    /// Gets or sets the initial_economic_value.
    /// </summary>
    /// <value>The initial_economic_value.</value>
    public double Initial_economic_value
    {
      get { return m_initial_economic_value; }
      set { m_initial_economic_value = value; }
    }

    /// <summary>
    /// Gets the initial_flood_probability.
    /// </summary>
    /// <value>The initial_flood_probability.</value>
    public double[] Initial_flood_probability
    {
      get { return this.m_initial_flood_probability; }
    }

    /// <summary>
    /// Gets the increase_of_water_level.
    /// </summary>
    /// <value>The increase_of_water_level.</value>
    public double[] Increase_of_water_level
    {
      get { return this.m_increase_of_water_level; }
    }

    /// <summary>
    /// Gets the extreme_water.
    /// </summary>
    /// <value>The extreme_water.</value>
    public double[] Extreme_water
    {
      get { return this.m_extreme_water; }
    }

    /// <summary>
    /// Gets the exponential cost parameters.
    /// </summary>
    /// <value>The exponential cost parameters.</value>
    public ExponentialCostParameters[] ExponentialCostParameters
    {
      get { return this.m_exponentialCostParameters; }
    }

    /// <summary>
    /// Gets the quadratic cost parameters.
    /// </summary>
    /// <value>The quadratic cost parameters.</value>
    public QuadraticCostParameters[] QuadraticCostParameters
    {
      get { return this.m_quadraticCostParameters; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="Scenario"/> class.
    /// </summary>
    public Scenario() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Scenario"/> class.
    /// </summary>
    /// <param name="scenario">The scenario.</param>
    /// <param name="cardId">The card id.</param>
    public Scenario(string scenario, int cardId)
    {
      this.m_scenarioName = scenario;
      this.ScenarioCardId = cardId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Scenario"/> class.
    /// </summary>
    /// <param name="scenarionaam">Naam van de scenario</param>
    /// <param name="economic_growth">Economic_growth</param>
    /// <param name="discount_rate1">Discount_rate1 (delta1)</param>
    /// <param name="discount_rate2">Discount_rate2 (delta2)</param>
    /// <param name="aantalSegmenten">The aantal segmenten.</param>
    public Scenario(string scenarionaam, double economic_growth
      , double discount_rate1, double discount_rate2, int aantalSegmenten)
    {
      this.m_scenarioName = scenarionaam;
      this.m_economic_growth = economic_growth;
      this.m_discount_rate1 = discount_rate1;
      this.m_discount_rate2 = discount_rate2;
      this.m_initial_economic_value = 0.0; // Wordt later via property gevuld.

      this.m_extreme_water = new double[aantalSegmenten];
      this.m_increase_of_water_level = new double[aantalSegmenten];
      this.m_initial_flood_probability = new double[aantalSegmenten];
      this.m_exponentialCostParameters = new ExponentialCostParameters[aantalSegmenten];
      this.m_quadraticCostParameters = new QuadraticCostParameters[aantalSegmenten];
    }

    #endregion Constructors --------------------------------------------------
  }

    /// <summary>
  /// var: i (lijst met Scenarios)
  /// </summary>
  [Serializable]
  public class Scenarios : List<Scenario>
  {
    #region Member functions -------------------------------------------------

    /// <summary>
    /// Ophalen alleen Scenarionaam en CardId
    /// </summary>
    /// <param name="project">The project.</param>
    public bool GetResults(Project project)
    {
      try
      {
        // Huidige scenarios verwijderen
        this.Clear();

        if (project != null)
        {
          // Ophalen Scenario index
          Set setScenarios = project.GetSet("Scenarios");
          int[] scenarios = new int[setScenarios.Card];

          object ScenariosUit = scenarios as object;
          setScenarios.CompoundTuplePassMode = ELEMENT_PASS_MODE.ELEMENT_BY_NUMBER;
          setScenarios.RetrieveElementArray(ref ScenariosUit);
          scenarios = ScenariosUit as int[];

          // Offset op 0 zetten
          setScenarios.OrdinalsOffset = 0;

          // Lijstje toevoegen
          for (int index = 0; index < setScenarios.Card; index++)
          {
            string naam = setScenarios.OrdinalToName(index);
            this.Add(new Scenario(naam, scenarios[index]));
          }
        }
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    #endregion Member functions ----------------------------------------------
  }
}
