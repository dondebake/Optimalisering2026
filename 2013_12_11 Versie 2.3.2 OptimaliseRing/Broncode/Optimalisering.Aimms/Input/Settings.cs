#region Copyright -------------------------------------------------------
// Copyright © 2009, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/Input/Settings.cs 4     7-04-09 7:47 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Aimms;

namespace OptimaliseRing.Aimms
{
  /// <summary>
  /// Investment cost functions
  /// </summary>
  public enum InvestmentCostFunctions
  {
    /// <summary>
    /// Exponential
    /// </summary>
    Exponential = 1,
    /// <summary>
    /// Quadratic
    /// </summary>
    Quadratic = 2
  }

  /// <summary>
  /// AdditionalOptions
  /// </summary>
  [Serializable]
  public class AdditionalOptions
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Presolve
    /// </summary>
    private bool m_presolve;
    /// <summary>
    /// Add cuts
    /// </summary>
    private bool m_add_cuts;
    /// <summary>
    /// Use incumbents
    /// </summary>
    private bool m_use_incumbents;
    /// <summary>
    /// Big-M investment
    /// </summary>
    private bool m_bigM;
    /// <summary>
    /// Max_iterations_AOA
    /// eenheid: -
    /// </summary>
    private double m_max_iterations_AOA;
    /// <summary>
    /// Mip_tolerance
    /// eenheid: -
    /// </summary>
    private double m_mip_tolerance;
    /// <summary>
    /// Relative_tolerance
    /// eenheid: %
    /// </summary>
    private double m_relative_tolerance;
    /// <summary>
    /// Max_time
    /// eenheid: minute
    /// </summary>
    private double m_max_time;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="AdditionalOptions"/> is presolve.
    /// </summary>
    /// <value><c>true</c> if presolve; otherwise, <c>false</c>.</value>
    public bool Presolve
    {
      get { return this.m_presolve; }
      set { this.m_presolve = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="AdditionalOptions"/> is add_cuts.
    /// </summary>
    /// <value><c>true</c> if add_cuts; otherwise, <c>false</c>.</value>
    public bool Add_cuts
    {
      get { return this.m_add_cuts; }
      set { this.m_add_cuts = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="AdditionalOptions"/> is use_incumbents.
    /// </summary>
    /// <value><c>true</c> if use_incumbents; otherwise, <c>false</c>.</value>
    public bool Use_incumbents
    {
      get { return this.m_use_incumbents; }
      set { this.m_use_incumbents = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Big-M investment
    /// </summary>
    /// <value><c>true</c> if [Big-M investment]; otherwise, <c>false</c>.</value>
    public bool BigM
    {
      get { return this.m_bigM; }
      set { this.m_bigM = value; }
    }

    /// <summary>
    /// Gets or sets Max_iterations_AOA
    /// eenheid: -
    /// </summary>
    /// <value>The max_iterations_ AOA.</value>
    public double Max_iterations_AOA
    {
      get { return this.m_max_iterations_AOA; }
      set { this.m_max_iterations_AOA = value; }
    }

    /// <summary>
    /// Gets or sets Mip_tolerance
    /// eenheid: -
    /// </summary>
    /// <value>The mip_tolerance.</value>
    public double Mip_tolerance
    {
      get { return this.m_mip_tolerance; }
      set { this.m_mip_tolerance = value; }
    }

    /// <summary>
    /// Gets or sets Relative_tolerance
    /// eenheid: %
    /// </summary>
    /// <value>The relative_tolerance.</value>
    public double Relative_tolerance
    {
      get { return this.m_relative_tolerance; }
      set { this.m_relative_tolerance = value; }
    }

    /// <summary>
    /// Gets or sets Max_time
    /// eenheid: minute
    /// </summary>
    /// <value>The max_time.</value>
    public double Max_time
    {
      get { return this.m_max_time; }
      set { this.m_max_time = value; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="AdditionalOptions"/> class.
    /// </summary>
    public AdditionalOptions()
    {
      this.m_presolve = false;
      this.m_add_cuts = false;
      this.m_use_incumbents = false;
      this.m_bigM = false;

      this.m_max_iterations_AOA = 0.0;
      this.m_mip_tolerance = 0.0;
      this.m_relative_tolerance = 0.0;
      this.m_max_time = 0.0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdditionalOptions"/> class.
    /// </summary>
    /// <param name="presolve">presolve.</param>
    /// <param name="add_cuts">add cuts.</param>
    /// <param name="use_incumbents">use incumbents.</param>
    /// <param name="bigM">if set to <c>true</c> [big M].</param>
    /// <param name="max_iterations_AOA">Max_iterations_AOA [-].</param>
    /// <param name="mip_tolerance">Mip_tolerance [-].</param>
    /// <param name="relative_tolerance">Relative_tolerance [%].</param>
    /// <param name="max_time">Max_time [minute].</param>
    public AdditionalOptions(
      bool presolve, bool add_cuts, bool use_incumbents, bool bigM
      , double max_iterations_AOA, double mip_tolerance, double relative_tolerance, double max_time)
    {
      this.m_presolve = presolve;
      this.m_add_cuts = add_cuts;
      this.m_use_incumbents = use_incumbents;
      this.m_bigM = bigM;

      this.m_max_iterations_AOA = max_iterations_AOA;
      this.m_mip_tolerance = mip_tolerance;
      this.m_relative_tolerance = relative_tolerance;
      this.m_max_time = max_time;
    }

    #endregion Constructors --------------------------------------------------
  }

  /// <summary>
  /// TuningConstraints
  /// </summary>
  [Serializable]
  public class TuningConstraints
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Use One_increase_during_period
    /// </summary>
    private bool m_one_increase_during_period;
    /// <summary>
    /// Use Minimum_increase
    /// </summary>
    private bool m_minimum_increase;
    /// <summary>
    /// Use Two_segments
    /// </summary>
    private bool m_two_segments;
    /// <summary>
    /// Use All_at_the_same_time
    /// </summary>
    private bool m_all_at_the_same_time;
    /// <summary>
    /// Use Group_at_the_same_time
    /// </summary>
    private bool m_group_at_the_same_time;

    /// <summary>
    /// Maximum increase
    /// var: Max_increase
    /// eenheid: cm
    /// </summary>
    private double m_max_increase;

    /// <summary>
    /// Minimum increase
    /// var: Min_increase
    /// eenheid: cm
    /// </summary>
    private double m_min_increase;

    /// <summary>
    /// Init years before checking years between increases
    /// var: Years_until_steady
    /// eenheid: year
    /// </summary>
    private double m_years_until_steady;

    /// <summary>
    /// Minimum period between increases of same segments
    /// var: Years_between_increases
    /// eenheid: year
    /// </summary>
    private double m_years_between_increases;

    /// <summary>
    /// Minimum period between increases of different segments
    /// var: Years_without_increase
    /// eenheid: year
    /// </summary>
    private double m_years_without_increase;

    /// <summary>
    /// Number of years until we start forcing equal timing
    /// var: Years_until_force_equal
    /// eenheid: year
    /// </summary>
    private double m_years_until_force_equal;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="TuningConstraints"/> is one_increase_during_period.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if one_increase_during_period; otherwise, <c>false</c>.
    /// </value>
    public bool One_increase_during_period
    {
      get { return this.m_one_increase_during_period; }
      set { this.m_one_increase_during_period = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="TuningConstraints"/> is minimum_increase.
    /// </summary>
    /// <value><c>true</c> if minimum_increase; otherwise, <c>false</c>.</value>
    public bool Minimum_increase
    {
      get { return this.m_minimum_increase; }
      set { this.m_minimum_increase = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="TuningConstraints"/> is two_segments.
    /// </summary>
    /// <value><c>true</c> if two_segments; otherwise, <c>false</c>.</value>
    public bool Two_segments
    {
      get { return this.m_two_segments; }
      set { this.m_two_segments = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="TuningConstraints"/> is all_at_the_same_time.
    /// </summary>
    /// <value><c>true</c> if all_at_the_same_time; otherwise, <c>false</c>.</value>
    public bool All_at_the_same_time
    {
      get { return this.m_all_at_the_same_time; }
      set { this.m_all_at_the_same_time = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="TuningConstraints"/> is group_at_the_same_time.
    /// </summary>
    /// <value><c>true</c> if group_at_the_same_time; otherwise, <c>false</c>.</value>
    public bool Group_at_the_same_time
    {
      get { return this.m_group_at_the_same_time; }
      set { this.m_group_at_the_same_time = value; }
    }

    /// <summary>
    /// Gets or sets Maximum increase
    /// var: Max_increase
    /// eenheid: cm
    /// </summary>
    /// <value>The Max_increase.</value>
    public double Max_increase
    {
      get { return this.m_max_increase; }
      set { this.m_max_increase = value; }
    }

    /// <summary>
    /// Gets or sets Minimum increase
    /// var: Min_increase
    /// eenheid: cm
    /// </summary>
    /// <value>The Min_increase.</value>
    public double Min_increase
    {
      get { return this.m_min_increase; }
      set { this.m_min_increase = value; }
    }

    /// <summary>
    /// Gets or sets Init years before checking years between increases
    /// var: Years_until_steady
    /// eenheid: year
    /// </summary>
    /// <value>The Years_until_steady.</value>
    public double Years_until_steady
    {
      get { return this.m_years_until_steady; }
      set { this.m_years_until_steady = value; }
    }

    /// <summary>
    /// Gets or sets Minimum period between increases of same segments
    /// var: Years_between_increases
    /// eenheid: year
    /// </summary>
    /// <value>The Years_between_increases.</value>
    public double Years_between_increases
    {
      get { return this.m_years_between_increases; }
      set { this.m_years_between_increases = value; }
    }

    /// <summary>
    /// Gets or sets Minimum period between increases of different segments
    /// var: Years_without_increase
    /// eenheid: year
    /// </summary>
    /// <value>The Years_without_increase.</value>
    public double Years_without_increase
    {
      get { return this.m_years_without_increase; }
      set { this.m_years_without_increase = value; }
    }

    /// <summary>
    /// Gets or sets Number of years until we start forcing equal timing.
    /// var: Years_until_force_equal
    /// eenheid: year
    /// </summary>
    /// <value>The years_until_force_equal.</value>
    public double Years_until_force_equal
    {
      get { return this.m_years_until_force_equal; }
      set { this.m_years_until_force_equal = value; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionalConstraints"/> class.
    /// </summary>
    public TuningConstraints()
    {
      this.m_one_increase_during_period = false;
      this.m_minimum_increase = false;
      this.m_two_segments = false;
      this.m_all_at_the_same_time = false;
      this.m_group_at_the_same_time = false;

      this.m_max_increase = 0.0;
      this.m_min_increase = 0.0;
      this.m_years_until_steady = 0.0;
      this.m_years_between_increases = 0.0;
      this.m_years_without_increase = 0.0;
      this.m_years_until_force_equal = 0.0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TuningConstraints"/> class.
    /// </summary>
    /// <param name="one_increase_during_period">Minimum period between successive heightnings.</param>
    /// <param name="minimum_increase">Minimum dike increase.</param>
    /// <param name="two_segments">Minimum period between heightnings of different segments.</param>
    /// <param name="all_at_the_same_time">Force increase all dike segments at the same time.</param>
    /// <param name="group_at_the_same_time">Force group of segments at the same time.</param>
    /// <param name="max_increase">Maximum increase.</param>
    /// <param name="min_increase">Minimum increase</param>
    /// <param name="years_until_steady">Init years before checking years between increases.</param>
    /// <param name="years_between_increases">Minimum period between increases of same segments</param>
    /// <param name="years_without_increase">Minimum period between increases of different segments.</param>
    /// <param name="years_until_force_equal">Number of years until we start forcing equal timing.</param>
    public TuningConstraints(
          bool one_increase_during_period
        , bool minimum_increase
        , bool two_segments
        , bool all_at_the_same_time
        , bool group_at_the_same_time
        , double max_increase
        , double min_increase
        , double years_until_steady
        , double years_between_increases
        , double years_without_increase
        , double years_until_force_equal)
    {
      this.m_one_increase_during_period = one_increase_during_period;
      this.m_minimum_increase = minimum_increase;
      this.m_two_segments = two_segments;
      this.m_all_at_the_same_time = all_at_the_same_time;
      this.m_group_at_the_same_time = group_at_the_same_time;

      this.m_max_increase = max_increase;
      this.m_min_increase = min_increase;
      this.m_years_until_steady = years_until_steady;
      this.m_years_between_increases = years_between_increases;
      this.m_years_without_increase = years_without_increase;
      this.m_years_until_force_equal = years_until_force_equal;

    }

    #endregion Constructors --------------------------------------------------
  }

  /// <summary>
  /// OptionalConstraints
  /// </summary>
  [Serializable]
  public class OptionalConstraints
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Use maximal flood probability
    /// </summary>
    private bool m_flood_probability_constraint;
    /// <summary>
    /// Use maximal costs
    /// </summary>
    private bool m_max_Costs;
    /// <summary>
    /// Max_flood_probability
    /// var: P_max
    /// eenheid: 1/year
    /// standaard: 0.0005
    /// </summary>
    private double m_maximal_flood_probability;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="OptionalConstraints"/> is use_maximal_flood_probability.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if use_maximal_flood_probability; otherwise, <c>false</c>.
    /// </value>
    public bool Flood_probability_constraint
    {
      get { return this.m_flood_probability_constraint; }
      set { this.m_flood_probability_constraint = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="OptionalConstraints"/> is use_maximal_costs.
    /// </summary>
    /// <value><c>true</c> if use_maximal_costs; otherwise, <c>false</c>.</value>
    public bool Max_Costs
    {
      get { return this.m_max_Costs; }
      set { this.m_max_Costs = value; }
    }

    /// <summary>
    /// Gets or sets the maximal_flood_probability.
    /// </summary>
    /// <value>
    /// The maximal_flood_probability.
    /// var: P_max
    /// eenheid: 1/year
    /// standaard: 0.0005
    /// </value>
    public double Maximal_flood_probability
    {
      get { return this.m_maximal_flood_probability; }
      set { this.m_maximal_flood_probability = value; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionalConstraints"/> class.
    /// </summary>
    public OptionalConstraints()
    {
      this.m_flood_probability_constraint = false;
      this.m_max_Costs = false;
      this.m_maximal_flood_probability = 0.0005;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionalConstraints"/> class.
    /// </summary>
    /// <param name="flood_probability_constraint">if set to <c>true</c> [flood_probability_constraint].</param>
    /// <param name="max_Costs">if set to <c>true</c> [max_ costs].</param>
    /// <param name="maximal_flood_probability">The maximal_flood_probability.</param>
    public OptionalConstraints(
          bool flood_probability_constraint
        , bool max_Costs
        , double maximal_flood_probability)
    {
      this.m_flood_probability_constraint = flood_probability_constraint;
      this.m_max_Costs = max_Costs;
      this.m_maximal_flood_probability = maximal_flood_probability;
    }

    #endregion Constructors --------------------------------------------------
  }

  /// <summary>
  /// Settings
  /// </summary>
  [Serializable]
  public class Settings : IDeclarations
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// InvestmentCostFunctions (Exponential or Quadratic)
    /// </summary>
    private InvestmentCostFunctions m_investmentCostFunction;
    /// <summary>
    /// OptionalConstraints
    /// </summary>
    private OptionalConstraints m_optionalConstraints;
    /// <summary>
    /// TuningConstraints
    /// </summary>
    private TuningConstraints m_tuningConstraints;
    /// <summary>
    /// AdditionalOptions
    /// </summary>
    private AdditionalOptions m_additionalOptions;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets or sets the investment cost function.
    /// Exponential or Quadratic
    /// </summary>
    /// <value>The investment cost function.</value>
    public InvestmentCostFunctions InvestmentCostFunction
    {
      get { return this.m_investmentCostFunction; }
      set { this.m_investmentCostFunction = value; }
    }

    /// <summary>
    /// Gets the optional constraints.
    /// </summary>
    /// <value>The optional constraints.</value>
    public OptionalConstraints OptionalConstraints
    {
      get { return m_optionalConstraints; }
      set { this.m_optionalConstraints = value; }
    }

    /// <summary>
    /// Gets or sets the tuning constraints.
    /// </summary>
    /// <value>The tuning constraints.</value>
    public TuningConstraints TuningConstraints
    {
      get { return m_tuningConstraints; }
      set { this.m_tuningConstraints = value; }
    }

    /// <summary>
    /// Gets or sets the additional options.
    /// </summary>
    /// <value>The additional options.</value>
    public AdditionalOptions AdditionalOptions
    {
      get { return m_additionalOptions; }
      set { this.m_additionalOptions = value; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="Settings"/> class.
    /// </summary>
    public Settings()
    {
      this.m_investmentCostFunction = InvestmentCostFunctions.Exponential;
      this.m_optionalConstraints = new OptionalConstraints();
      this.m_tuningConstraints = new TuningConstraints();
      this.m_additionalOptions = new AdditionalOptions();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Settings"/> class.
    /// </summary>
    /// <param name="investmentCostFunction">The investment cost function.</param>
    /// <param name="optionalConstraints">The optional constraints.</param>
    /// <param name="tuningConstraints">The tuning constraints.</param>
    /// <param name="additionalOptions">The additional options.</param>
    public Settings(
        InvestmentCostFunctions investmentCostFunction
      , OptionalConstraints optionalConstraints
      , TuningConstraints tuningConstraints
      , AdditionalOptions additionalOptions)
    {
      this.m_investmentCostFunction = investmentCostFunction;
      this.m_optionalConstraints = optionalConstraints;
      this.m_tuningConstraints = tuningConstraints;
      this.m_additionalOptions = additionalOptions;
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

        // Ophalen Investment_cost_function mogelijkheden
        Set setInvestment_cost_function = project.GetSet("Investment_cost_functions");
        int[] investment_cost_function = new int[setInvestment_cost_function.Card];
        object Investment_cost_function = investment_cost_function as object;
        setInvestment_cost_function.CompoundTuplePassMode = ELEMENT_PASS_MODE.ELEMENT_BY_NUMBER;
        setInvestment_cost_function.RetrieveElementArray(ref Investment_cost_function);
        investment_cost_function = Investment_cost_function as int[];

        // Offset op 0 zetten
        setInvestment_cost_function.OrdinalsOffset = 0;

        for (int index = 0; index < setInvestment_cost_function.Card; index++)
        {
          string naam = setInvestment_cost_function.OrdinalToName(index);
          if (string.Compare(naam, this.m_investmentCostFunction.ToString(), true) == 0)
          {
            object Investment_cost_functions = index as object;
            project.set_Value("Selected_investment_cost_function", investment_cost_function[index]);
          }
        }

        // Optional_constraint
        Set setOptional_constraints = project.GetSet("Optional_constraints");
        int[] optional_constraints = new int[setOptional_constraints.Card];
        object Optional_constraints = optional_constraints as object;
        setOptional_constraints.CompoundTuplePassMode = ELEMENT_PASS_MODE.ELEMENT_BY_NUMBER;
        setOptional_constraints.RetrieveElementArray(ref Optional_constraints);
        optional_constraints = Optional_constraints as int[];

        // Optional_constraint_selection
        Identifier idOptional_constraint_selection = project.GetIdentifier("Optional_constraint_selection");
        int[] optional_constraint_selection = new int[setOptional_constraints.Card];
        object Optional_constraint_selection = optional_constraint_selection as object;
        idOptional_constraint_selection.RetrieveArray(ref Optional_constraint_selection, 0, 0);
        optional_constraint_selection = Optional_constraint_selection as int[];

        // Optional_constraint_label
        Identifier idOptional_constraint_label = project.GetIdentifier("Optional_constraint_label");
        string[] optional_constraint_label = new string[setOptional_constraints.Card];
        object Optional_constraint_label = optional_constraint_label as object;
        idOptional_constraint_label.RetrieveArray(ref Optional_constraint_label, 0, 0);
        optional_constraint_label = Optional_constraint_label as string[];

        // Lijstje afdrukken
        for (int index = 0; index < setOptional_constraints.Card; index++)
        {
          string naam = setOptional_constraints.OrdinalToName(index);

          if (string.Compare(naam, "Flood_probability_constraint", true) == 0)
          {
            optional_constraint_selection[index] =
              this.m_optionalConstraints.Flood_probability_constraint ? 1 : 0;
          }
          else if (string.Compare(naam, "Max_Costs", true) == 0)
          {
            optional_constraint_selection[index] =
              this.m_optionalConstraints.Max_Costs ? 1 : 0;
          }
        }

        // Optional Constraints
        Optional_constraint_selection = optional_constraint_selection as object;
        project.AssignArray("Optional_constraint_selection", ref Optional_constraint_selection, 0);

        // Max_flood_probability
        object Maximal_flood_probability = this.m_optionalConstraints.Maximal_flood_probability as object;
        project.set_Value("Max_flood_probability", Maximal_flood_probability);

        // Tuning_constraints
        Set setTuning_constraints = project.GetSet("Tuning_constraints");
        int[] tuning_constraints = new int[setTuning_constraints.Card];
        object Tuning_constraints = tuning_constraints as object;
        setTuning_constraints.CompoundTuplePassMode = ELEMENT_PASS_MODE.ELEMENT_BY_NUMBER;
        setTuning_constraints.RetrieveElementArray(ref Tuning_constraints);
        tuning_constraints = Tuning_constraints as int[];

        // Tuning_constraint_selection
        Identifier idTuning_constraint_selection = project.GetIdentifier("Tuning_constraint_selection");
        int[] tuning_constraint_selection = new int[setTuning_constraints.Card];
        object Tuning_constraint_selection = tuning_constraint_selection as object;
        idTuning_constraint_selection.RetrieveArray(ref Tuning_constraint_selection, 0, 0);
        tuning_constraint_selection = Tuning_constraint_selection as int[];

        // Tuning_constraint_label
        Identifier idTuning_constraint_label = project.GetIdentifier("Tuning_constraint_label");
        string[] tuning_constraint_label = new string[setTuning_constraints.Card];
        object Tuning_constraint_label = tuning_constraint_label as object;
        idTuning_constraint_label.RetrieveArray(ref Tuning_constraint_label, 0, 0);
        tuning_constraint_label = Tuning_constraint_label as string[];

        // Lijstje afdrukken
        for (int index = 0; index < setTuning_constraints.Card; index++)
        {
          string naam = setTuning_constraints.OrdinalToName(index);

          if (string.Compare(naam, "One_increase_during_period", true) == 0)
          {
            tuning_constraint_selection[index] =
              this.m_tuningConstraints.One_increase_during_period ? 1 : 0;
          }
          else if (string.Compare(naam, "Minimum_increase", true) == 0)
          {
            tuning_constraint_selection[index] =
              this.m_tuningConstraints.Minimum_increase ? 1 : 0;
          }
          else if (string.Compare(naam, "Two_segments", true) == 0)
          {
            tuning_constraint_selection[index] =
              this.m_tuningConstraints.Two_segments ? 1 : 0;
          }
          else if (string.Compare(naam, "All_at_the_same_time", true) == 0)
          {
            tuning_constraint_selection[index] =
              this.m_tuningConstraints.All_at_the_same_time ? 1 : 0;
          }
          else if (string.Compare(naam, "Group_at_the_same_time", true) == 0)
          {
            tuning_constraint_selection[index] =
              this.m_tuningConstraints.Group_at_the_same_time ? 1 : 0;
          }
        }

        // Optional Constraints
        Tuning_constraint_selection = tuning_constraint_selection as object;
        project.AssignArray("Tuning_constraint_selection", ref Tuning_constraint_selection, 0);

        // Max_increase
        object Max_increase = this.m_tuningConstraints.Max_increase;
        project.set_Value("Max_increase", Max_increase);

        // Min_increase
        object Min_increase = this.m_tuningConstraints.Min_increase;
        project.set_Value("Min_increase", Min_increase);

        // Years_until_steady
        object Years_until_steady = this.m_tuningConstraints.Years_until_steady;
        project.set_Value("Years_until_steady", Years_until_steady);

        // Years_between_increases
        object Years_between_increases = this.m_tuningConstraints.Years_between_increases;
        project.set_Value("Years_between_increases", Years_between_increases);

        // Years_without_increase
        object Years_without_increase = this.m_tuningConstraints.Years_without_increase;
        project.set_Value("Years_without_increase", Years_without_increase);

        // Years_until_force_equal
        object Years_until_force_equal = this.m_tuningConstraints.Years_until_force_equal;
        project.set_Value("Years_until_force_equal", Years_until_force_equal);

        // Additional_options
        Set setAdditional_options = project.GetSet("Additional_options");
        int[] additional_options = new int[setAdditional_options.Card];
        object Additional_options = additional_options as object;
        setAdditional_options.CompoundTuplePassMode = ELEMENT_PASS_MODE.ELEMENT_BY_NUMBER;
        setAdditional_options.RetrieveElementArray(ref Additional_options);
        additional_options = Additional_options as int[];

        // Option_active
        Identifier idOption_active = project.GetIdentifier("Option_active");
        int[] option_active = new int[setAdditional_options.Card];
        object Option_active = option_active as object;
        idOption_active.RetrieveArray(ref Option_active, 0, 0);
        option_active = Option_active as int[];

        // Lijstje afdrukken
        for (int index = 0; index < setAdditional_options.Card; index++)
        {
          string naam = setAdditional_options.OrdinalToName(index);

          if (string.Compare(naam, "Presolve", true) == 0)
          {
            option_active[index] = this.m_additionalOptions.Presolve ? 1 : 0;
          }
          else if (string.Compare(naam, "Add cuts", true) == 0)
          {
            option_active[index] = this.m_additionalOptions.Add_cuts ? 1 : 0;
          }
          else if (string.Compare(naam, "Use incumbents", true) == 0)
          {
            option_active[index] = this.m_additionalOptions.Use_incumbents ? 1 : 0;
          }
        }

        // Optional Constraints
        Option_active = option_active as object;
        project.AssignArray("Option_active", ref Option_active, 0);

        // Max_iterations_AOA
        object Max_iterations_AOA = this.m_additionalOptions.Max_iterations_AOA as object;
        project.set_Value("Max_iterations_AOA", Max_iterations_AOA);

        // Mip_tolerance
        object Mip_tolerance = this.m_additionalOptions.Mip_tolerance as object;
        project.set_Value("Mip_tolerance", Mip_tolerance);

        // Relative_tolerance
        object Relative_tolerance = this.m_additionalOptions.Relative_tolerance as object;
        project.set_Value("Relative_tolerance", Relative_tolerance);

        // Max_time
        object Max_time = this.m_additionalOptions.Max_time as object;
        project.set_Value("Max_time", Max_time);

        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return false;
      }
    }

    #endregion Member functions ----------------------------------------------
  }
}
