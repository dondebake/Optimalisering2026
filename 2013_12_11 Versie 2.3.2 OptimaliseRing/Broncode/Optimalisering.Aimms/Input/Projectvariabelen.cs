using System;
using System.Collections.Generic;
using System.Text;
using Aimms;

namespace OptimaliseRing.Aimms
{
  /// <summary>
  /// Objective
  /// </summary>
  public enum Objective
  {
    /// <summary>
    /// Optellen
    /// </summary>
    Sum,
    /// <summary>
    /// Maximum
    /// </summary>
    Max,
    /// <summary>
    /// één scenario per berekening
    /// </summary>
    One_scenario,
    /// <summary>
    /// Regret
    /// </summary>
    Regret
  }

  /// <summary>
  /// Aimmsvariabelen en lijst met scenarios (i)
  /// </summary>
  public class Projectvariabelen : List<Scenario>, IDeclarations
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Dijkring
    /// </summary>
    private string m_dijkringnummer;
    /// <summary>
    /// Dijkring_naam
    /// </summary>
    private string m_dijkringnaam;
    /// <summary>
    /// Dijkringdeel
    /// </summary>
    private int m_dijkringdeelnummer;
    /// <summary>
    /// Dijkringdeel_naam
    /// </summary>
    private string m_dijkringdeelnaam;
    /// <summary>
    /// Start_year
    /// eenheid: jaar
    /// </summary>
    private int m_startyear;
    /// <summary>
    /// Zeta
    /// </summary>
    private double m_loss_increase;
    /// <summary>
    /// Objective
    /// sum, max, one-scenario of regret
    /// </summary>
    private Objective m_objective;
    /// <summary>
    /// Segments(i,l)
    /// var: i (lijst met scenarios)
    /// var: l (lijst met segmenten)
    /// </summary>
    private Segments m_segments;
    /// <summary>
    /// Lowest_segment1
    /// var: l_0
    /// range=Segments
    /// </summary>
    private Segment m_lowest_segment1;
    /// <summary>
    /// Lowest_segment2
    /// var: l*
    /// range=Segments
    /// </summary>
    private Segment m_lowest_segment2;
    /// <summary>
    /// Max_updates (aantal investeringen waar kosten worden bepaald voor
    /// (m_update_discounted_costs en m_update_undiscounted_costs)
    /// domain: u
    /// </summary>
    private int m_max_updates;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets or sets the dijkringnummer.
    /// </summary>
    /// <value>The dijkringnummer.</value>
    public string Dijkringnummer
    {
      get { return m_dijkringnummer; }
      set { m_dijkringnummer = value; }
    }

    /// <summary>
    /// Gets or sets the dijkringnaam.
    /// </summary>
    /// <value>The dijkringnaam.</value>
    public string Dijkringnaam
    {
      get { return m_dijkringnaam; }
      set { m_dijkringnaam = value; }
    }

    /// <summary>
    /// Gets or sets the dijkringdeelnummer.
    /// </summary>
    /// <value>The dijkringdeelnummer.</value>
    public int Dijkringdeelnummer
    {
      get { return m_dijkringdeelnummer; }
      set { m_dijkringdeelnummer = value; }
    }

    /// <summary>
    /// Gets or sets the dijkringdeelnaam.
    /// </summary>
    /// <value>The dijkringdeelnaam.</value>
    public string Dijkringdeelnaam
    {
      get { return m_dijkringdeelnaam; }
      set { m_dijkringdeelnaam = value; }
    }

    /// <summary>
    /// Gets or sets the startyear.
    /// </summary>
    /// <value>The startyear.</value>
    public int Startyear
    {
      get { return m_startyear; }
      set { m_startyear = value; }
    }

    /// <summary>
    /// Gets or sets the loss_increase.
    /// </summary>
    /// <value>The loss_increase.</value>
    public double Loss_increase
    {
      get { return m_loss_increase; }
      set { m_loss_increase = value; }
    }

    /// <summary>
    /// Gets or sets the objective.
    /// </summary>
    /// <value>The objective.</value>
    public Objective Objective
    {
      get { return m_objective; }
      set { m_objective = value; }
    }

    /// <summary>
    /// Gets Segments(i,l)
    /// var: i (lijst met scenarios)
    /// var: l (lijst met segmenten)
    /// </summary>
    /// <value>Segments.</value>
    public Segments Segments
    {
      get { return m_segments; }
      set { m_segments = value; }
    }

    /// <summary>
    /// Gets or sets the lowest_segment1.
    /// </summary>
    /// <value>The lowest_segment1.</value>
    public Segment Lowest_segment1
    {
      get { return m_lowest_segment1; }
      set { m_lowest_segment1 = value; }
    }

    /// <summary>
    /// Gets or sets the lowest_segment2.
    /// </summary>
    /// <value>The lowest_segment2.</value>
    public Segment Lowest_segment2
    {
      get { return m_lowest_segment2; }
      set { m_lowest_segment2 = value; }
    }

    /// <summary>
    /// Gets or sets Max_updates.
    /// </summary>
    /// <value>The max_updates.</value>
    public int Max_updates
    {
      get { return m_max_updates; }
      set { m_max_updates = value; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="Projectvariabelen"/> class.
    /// </summary>
    public Projectvariabelen() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Projectvariabelen"/> class.
    /// </summary>
    /// <param name="dijkringnummer">The dijkringnummer.</param>
    /// <param name="dijkringnaam">The dijkringnaam.</param>
    /// <param name="dijkringdeelnummer">The dijkringdeelnummer.</param>
    /// <param name="dijkringdeelnaam">The dijkringdeelnaam.</param>
    /// <param name="startyear">The startyear.</param>
    /// <param name="loss_increase">The loss_increase.</param>
    /// <param name="objective">The objective.</param>
    /// <param name="segments">The segments.</param>
    /// <param name="lowest_segment1">The lowest_segment1.</param>
    /// <param name="lowest_segment2">The lowest_segment2.</param>
    /// <param name="max_updates">The max_updates.</param>
    public Projectvariabelen(int dijkringnummer, string dijkringnaam, int dijkringdeelnummer
      , string dijkringdeelnaam, int startyear, double loss_increase,Objective objective
      , Segments segments, Segment lowest_segment1, Segment lowest_segment2, int max_updates)
    {
      this.m_dijkringdeelnummer = dijkringnummer;
      this.m_dijkringnaam = dijkringnaam;
      this.m_dijkringdeelnummer = dijkringdeelnummer;
      this.m_dijkringdeelnaam = dijkringdeelnaam;
      this.m_startyear = startyear;

      this.m_objective = objective;
      this.m_segments = segments;
      this.m_lowest_segment1 = lowest_segment1;
      this.m_lowest_segment2 = lowest_segment2;
      this.m_loss_increase = loss_increase;
      this.m_max_updates = max_updates;
    }

    #endregion Constructors --------------------------------------------------

    #region Member functions -------------------------------------------------

    /// <summary>
    /// Registers the variabelen.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <returns></returns>
    public bool RegisterVariabelen(Project project)
    {
      try
      {
        // Dike information *******************************************************
        // Dijkringnaam
        object Dijkring_naam = this.m_dijkringnaam as object;
        project.set_Value("Dijkring_naam", Dijkring_naam);

        // Dijkringnummer
        object Dijkring = this.m_dijkringnummer as object;
        project.set_Value("Dijkring", Dijkring);

        // Dijkringdeelnummer
        object Dijkringdeel = this.m_dijkringdeelnummer as object;
        project.set_Value("Dijkringdeel", Dijkringdeel);

        // Dijkringdeelnaam
        object Dijkringdeel_naam = this.m_dijkringdeelnaam as object;
        project.set_Value("Dijkringdeel_naam", Dijkringdeel_naam);

        // Start_year
        object Start_year = this.m_startyear;
        project.set_Value("Start_year", Start_year);

        // Scenarios
        ScenarioVars scenarioVars = this.Scenarios;

        object Scenarios = scenarioVars.Scenario_names as object;
        project.AssignElementArray("Scenarios", ref Scenarios, REPLACE_MODE.REPLACE);

        // Ophalen Segments index
        Set setScenarios = project.GetSet("Scenarios");
        int[] scenarios = new int[setScenarios.Card];

        object ScenariosUit = scenarios as object;
        setScenarios.CompoundTuplePassMode = ELEMENT_PASS_MODE.ELEMENT_BY_NUMBER;
        setScenarios.RetrieveElementArray(ref ScenariosUit);
        scenarios = ScenariosUit as int[];

        // Offset op 0 zetten
        setScenarios.OrdinalsOffset = 0;

        // Lijstje afdrukken
        for (int index = 0; index < setScenarios.Card; index++)
        {
          string naam = setScenarios.OrdinalToName(index);
          Scenario scenarioItem;
          if ((scenarioItem = this.getScenarioByName(naam)) != null)
          {
            scenarioItem.ScenarioCardId = scenarios[index];
          }
        }

        // Max_updates (aantal investeringen waar kosten worden bepaald voor
        // (m_update_discounted_costs en m_update_undiscounted_costs)
        object Max_updates = this.m_max_updates as object;
        project.set_Value("Max_updates", Max_updates);

        object Economic_growth = scenarioVars.Economic_growth as object;
        project.AssignArray("Economic_growth", ref Economic_growth, 0);

        object Loss_increase = this.Loss_increase;
        project.set_Value("Loss_increase", Loss_increase);

        object Discount_rate1 = scenarioVars.Discount_rate1 as object;
        project.AssignArray("Discount_rate1", ref Discount_rate1, 0);

        object Discount_rate2 = scenarioVars.Discount_rate2 as object;
        project.AssignArray("Discount_rate2", ref Discount_rate2, 0);

        // Initial_economic_value
        object Initial_economic_value = scenarioVars.Initial_economic_value as object;
        project.AssignArray("Initial_economic_value", ref Initial_economic_value, 0);

        // Selecteer scenario

        // Selecteerd het eerste scenario
        object Selected_scenario = this[0].ScenarioCardId as object;
        project.set_Value("Selected_scenario", Selected_scenario);

        Set setObjectives = project.GetSet("Objectives");
        int[] objectives = new int[setObjectives.Card];
        object Objectives = objectives as object;
        setObjectives.CompoundTuplePassMode = ELEMENT_PASS_MODE.ELEMENT_BY_NUMBER;
        setObjectives.RetrieveElementArray(ref Objectives);
        objectives = Objectives as int[];

        // Offset op 0 zetten
        setObjectives.OrdinalsOffset = 0;

        // Lijstje afdrukken
        for (int index = 0; index < setObjectives.Card; index++)
        {
          string naam = setObjectives.OrdinalToName(index);
          if ((string.Compare(naam, "sum", true) == 0 && this.m_objective == Objective.Sum) ||
              (string.Compare(naam, "max", true) == 0 && this.m_objective == Objective.Max) ||
              (string.Compare(naam, "one-scenario", true) == 0 && this.m_objective == Objective.One_scenario) ||
              (string.Compare(naam, "regret", true) == 0 && this.m_objective == Objective.Regret))
          {
            object Selected_objective = objectives[index] as object;
            project.set_Value("Selected_objective", Selected_objective);
          }
        }

        // Segments + Segment parameters ******************************************
        // Eerst de segmenten toevoegen, wordt ook de Aimms CardId gevuld.

        if (this.m_segments.RegisterVariabelen(project))
        {
          // General parameters deel 2 ********************************************
          // Lowest_segment1
          object Lowest_segment1 = this.m_lowest_segment1.SegmentCardId;
          project.set_Value("Lowest_segment1", Lowest_segment1);

          // Lowest_segment2
          object Lowest_segment2 = this.m_lowest_segment2.SegmentCardId;
          project.set_Value("Lowest_segment2", Lowest_segment2);

          object Initial_flood_probability = scenarioVars.Initial_flood_probability as object;
          project.AssignArray("Initial_flood_probability", ref Initial_flood_probability, 0);

          object Increase_of_water_level = scenarioVars.Increase_of_water_level as object;
          project.AssignArray("Increase_of_water_level", ref Increase_of_water_level, 0);

          object Extreme_water = scenarioVars.Extreme_water as object;
          project.AssignArray("Extreme_water", ref Extreme_water, 0);

          // Exponential cost parameters
          object Exp_fixed = scenarioVars.Exp_fixed as object;
          project.AssignArray("Exp_fixed", ref Exp_fixed, 0);

          object Exp_linear = scenarioVars.Exp_linear as object;
          project.AssignArray("Exp_linear", ref Exp_linear, 0);

          //object Exp_power = scenarioVars.Exp_power as object;
          //project.AssignArray("Exp_power", ref Exp_power, 0);

          // Quadratic cost parameters
          object Q_fixed = scenarioVars.Q_fixed as object;
          project.AssignArray("Q_fixed", ref Q_fixed, 0);

          object Q_linear = scenarioVars.Q_linear as object;
          project.AssignArray("Q_linear", ref Q_linear, 0);

          object Q_quad = scenarioVars.Q_quad as object;
          project.AssignArray("Q_quad", ref Q_quad, 0);

        }
        else
        {
          return false;
        }

        // Segments + Segment parameters ******************************************

        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return false;
      }
    }

    struct ScenarioVars
    {
      public string[] Scenario_names;
      public double[] Economic_growth;
      public double[] Discount_rate1;
      public double[] Discount_rate2;
      public double[] Initial_economic_value;
      public double[,] Extreme_water;
      public double[,] Increase_of_water_level;
      public double[,] Initial_flood_probability;

      // ExponentialCostParameters
      public double[,] Exp_fixed;
      public double[,] Exp_linear;
      //public double[,] Exp_power;

      // Quadratic cost parameters
      public double[,] Q_fixed;
      public double[,] Q_linear;
      public double[,] Q_quad;

      public ScenarioVars(int count, int segmentCount)
      {
        this.Scenario_names = new string[count];
        this.Economic_growth = new double[count];
        this.Discount_rate1 = new double[count];
        this.Discount_rate2 = new double[count];
        this.Initial_economic_value = new double[count];

        this.Extreme_water = new double[count, segmentCount];
        this.Increase_of_water_level = new double[count, segmentCount];
        this.Initial_flood_probability = new double[count, segmentCount];

        // ExponentialCostParameters
        this.Exp_fixed = new double[count, segmentCount];
        this.Exp_linear = new double[count, segmentCount];
        //this.Exp_power = new double[count, segmentCount];

        // Quadratic cost parameters
        this.Q_fixed = new double[count, segmentCount];
        this.Q_linear = new double[count, segmentCount];
        this.Q_quad = new double[count, segmentCount];
      }
    }

    /// <summary>
    /// Gets the scenarios.
    /// </summary>
    /// <value>The scenarios.</value>
    private ScenarioVars Scenarios
    {
      get
      {
        ScenarioVars scenarios = new ScenarioVars(this.Count, this.Segments.Count);

        for (int scenario = 0; scenario < this.Count; scenario++)
        {
          scenarios.Scenario_names[scenario] = this[scenario].ScenarioName;
          scenarios.Economic_growth[scenario] = this[scenario].Economic_growth;
          scenarios.Discount_rate1[scenario] = this[scenario].Discount_rate1;
          scenarios.Discount_rate2[scenario] = this[scenario].Discount_rate2;
          scenarios.Initial_economic_value[scenario] = this[scenario].Initial_economic_value;

          for (int segments = 0; segments < this.Segments.Count; segments++)
          {
            scenarios.Extreme_water[scenario, segments] = this[scenario].Extreme_water[segments];
            scenarios.Increase_of_water_level[scenario, segments] = this[scenario].Increase_of_water_level[segments];
            scenarios.Initial_flood_probability[scenario, segments] = this[scenario].Initial_flood_probability[segments];

            // ExponentialCostParameters
            scenarios.Exp_fixed[scenario, segments] = this[scenario].ExponentialCostParameters[segments].Exp_fixed;
            scenarios.Exp_linear[scenario, segments] = this[scenario].ExponentialCostParameters[segments].Exp_linear;
            //scenarios.Exp_power[scenario, segments] = this[scenario].ExponentialCostParameters[segments].Exp_power;

            // Quadratic cost parameters
            scenarios.Q_fixed[scenario, segments] = this[scenario].QuadraticCostParameters[segments].Q_fixed;
            scenarios.Q_linear[scenario, segments] = this[scenario].QuadraticCostParameters[segments].Q_linear;
            scenarios.Q_quad[scenario, segments] = this[scenario].QuadraticCostParameters[segments].Q_quad;
          }
        }
        return scenarios;
      }
    }

    private Scenario getScenarioByName(string name)
    {
      foreach (Scenario scenario in this)
      {
        if (string.Compare(scenario.ScenarioName, name, true) == 0)
        {
          return scenario;
        }
      }
      // Niets gevonden
      return null;
    }

    #endregion Member functions ----------------------------------------------
  }
}
