#region Copyright -------------------------------------------------------
// Copyright © 2009, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/Output/CostObjectives.cs 4     28-04-09 13:53 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Aimms;

namespace OptimaliseRing.Aimms
{
  /// <summary>
  /// CostObjectives
  /// </summary>
  public class CostObjectives
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// p_Total_discounted_investment_cost
    /// label: Total discount investment cost
    /// domain: i
    /// </summary>
    private double[] m_p_Total_discounted_investment_cost;
    /// <summary>
    /// p_Total_expected_loss
    /// label: Total expected loss (period based)
    /// domain: i
    /// </summary>
    private double[] m_p_Total_expected_loss;
    /// <summary>
    /// Total_disc_loss
    /// label: Total expected loss (year based, sum)
    /// domain: i
    /// </summary>
    private double[] m_total_disc_loss;
    /// <summary>
    /// Total_disc_loss_int
    /// label: Total expected loss (year based, int)
    /// domain: i
    /// </summary>
    private double[] m_total_disc_loss_int;
    /// <summary>
    /// p_Total_remainder_loss
    /// label: Expected loss after planning horizon
    /// domain: i
    /// </summary>
    private double[] m_p_Total_remainder_loss;
    /// <summary>
    /// p_Total_expected_costs
    /// label: Total expected costs (AIMMS model)
    /// domain: i
    /// </summary>
    private double[] m_p_Total_expected_costs;
    /// <summary>
    /// Total_expected_costs1
    /// label: Total expected costs (Definition OptimaliseRing)
    /// domain: i
    /// </summary>
    private double[] m_total_expected_costs1;
    /// <summary>
    /// Total_expected_costs2
    /// label: Total expected costs (year based)
    /// domain: i
    /// </summary>
    private double[] m_total_expected_costs2;
    /// <summary>
    /// Overall_objective
    /// Robust objective
    /// unit: MEURO
    /// </summary>
    private double m_overall_objective;
    /// <summary>
    /// Objective_AIMMS
    /// Robust objective
    /// unit: MEURO
    /// </summary>
    private double m_objective_AIMMS;
    /// <summary>
    /// Objective_year_based
    /// Robust objective
    /// unit: MEURO
    /// </summary>
    private double m_objective_year_based;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// p_Total_discounted_investment_cost
    /// label: Total discount investment cost
    /// domain: l,k
    /// </summary>
    /// <value>The p_ total_discounted_investment_cost.</value>
    public double[] P_Total_discounted_investment_cost
    {
      get { return m_p_Total_discounted_investment_cost; }
    }

    /// <summary>
    /// p_Total_expected_loss
    /// label: Total expected loss (period based)
    /// domain: i
    /// </summary>
    /// <value>The p_ total_expected_loss.</value>
    public double[] P_Total_expected_loss
    {
      get { return m_p_Total_expected_loss; }
    }

    /// <summary>
    /// Total_disc_loss
    /// label: Total expected loss (year based, sum)
    /// domain: i
    /// </summary>
    /// <value>The total_disc_loss.</value>
    public double[] Total_disc_loss
    {
      get { return m_total_disc_loss; }
    }

    /// <summary>
    /// Total_disc_loss_int
    /// label: Total expected loss (year based, int)
    /// domain: i
    /// </summary>
    /// <value>The m_total_disc_loss_int.</value>
    public double[] Total_disc_loss_int
    {
      get { return m_total_disc_loss_int; }
    }

    /// <summary>
    /// p_Total_remainder_loss
    /// label: Expected loss after planning horizon
    /// domain: i
    /// </summary>
    /// <value>The p_ total_remainder_loss.</value>
    public double[] P_Total_remainder_loss
    {
      get { return m_p_Total_remainder_loss; }
    }

    /// <summary>
    /// p_Total_expected_costs
    /// label: Total expected costs (AIMMS model)
    /// domain: i
    /// </summary>
    /// <value>The p_ total_expected_costs.</value>
    public double[] P_Total_expected_costs
    {
      get { return m_p_Total_expected_costs; }
    }

    /// <summary>
    /// Total_expected_costs1
    /// label: Total expected costs (Definition OptimaliseRing)
    /// domain: i
    /// </summary>
    /// <value>The total_expected_costs1.</value>
    public double[] Total_expected_costs1
    {
      get { return m_total_expected_costs1; }
    }

    /// <summary>
    /// Total_expected_costs2
    /// label: Total expected costs (year based)
    /// domain: i
    /// </summary>
    /// <value>Total expected costs (year based).</value>
    public double[] Total_expected_costs2
    {
      get { return m_total_expected_costs2; }
    }

    /// <summary>
    /// Overall_objective
    /// Robust objective
    /// unit: MEURO
    /// </summary>
    /// <value>Overall_objective.</value>
    public double Overall_objective
    {
      get { return m_overall_objective; }
    }

    /// <summary>
    /// Objective_AIMMS
    /// Robust objective
    /// unit: MEURO
    /// </summary>
    /// <value>Objective_AIMMS.</value>
    public double Objective_AIMMS
    {
      get { return m_objective_AIMMS; }
    }

    /// <summary>
    /// Objective_year_based
    /// Robust objective
    /// unit: MEURO
    /// </summary>
    /// <value>Objective_year_based.</value>
    public double Objective_year_based
    {
      get { return m_objective_year_based; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="CostObjectives"/> class.
    /// </summary>
    public CostObjectives() { }

    #endregion Constructors --------------------------------------------------

    #region Event handling ---------------------------------------------------
    #endregion Event handling ------------------------------------------------

    #region Member functions -------------------------------------------------

        /// <summary>
    /// Gets the results.
    /// </summary>
    /// <param name="project">The project.</param>
    public bool GetResults(Project project, Scenarios scenarios)
    {
      try
      {

        if (project != null)
        {
          // Moment_year (k)
          Identifier idMoment_year = project.GetIdentifier("Moment_year");

          int aantalScenarios = scenarios.Count;

          this.m_p_Total_discounted_investment_cost = new double[aantalScenarios];  // domain: i
          this.m_p_Total_expected_loss = new double[aantalScenarios];               // domain: i
          this.m_total_disc_loss = new double[aantalScenarios];                     // domain: i
          this.m_total_disc_loss_int = new double[aantalScenarios];                 // domain: i
          this.m_p_Total_remainder_loss = new double[aantalScenarios];              // domain: i
          this.m_p_Total_expected_costs = new double[aantalScenarios];              // domain: i
          this.m_total_expected_costs1 = new double[aantalScenarios];               // domain: i
          this.m_total_expected_costs2 = new double[aantalScenarios];               // domain: i

          // p_Total_discounted_investment_cost (i)
          Identifier idP_Total_discounted_investment_cost = project.GetIdentifier("p_Total_discounted_investment_cost");
          object p_Total_discounted_investment_cost = this.m_p_Total_discounted_investment_cost as object;
          idP_Total_discounted_investment_cost.RetrieveArray(ref p_Total_discounted_investment_cost, 0, 0);
          this.m_p_Total_discounted_investment_cost = p_Total_discounted_investment_cost as double[];

          // p_Total_expected_loss (i)
          Identifier idP_Total_expected_loss = project.GetIdentifier("p_Total_expected_loss");
          object p_Total_expected_loss = this.m_p_Total_expected_loss as object;
          idP_Total_expected_loss.RetrieveArray(ref p_Total_expected_loss, 0, 0);
          this.m_p_Total_expected_loss = p_Total_expected_loss as double[];

          // Total_disc_loss (i)
          Identifier idTotal_disc_loss = project.GetIdentifier("Total_disc_loss");
          object Total_disc_loss = this.m_total_disc_loss as object;
          idTotal_disc_loss.RetrieveArray(ref Total_disc_loss, 0, 0);
          this.m_total_disc_loss = Total_disc_loss as double[];

          // Total_disc_loss_int (i)
          Identifier idTotal_disc_loss_int = project.GetIdentifier("Total_disc_loss_int");
          object Total_disc_loss_int = this.m_total_disc_loss_int as object;
          idTotal_disc_loss_int.RetrieveArray(ref Total_disc_loss_int, 0, 0);
          this.m_total_disc_loss_int = Total_disc_loss_int as double[];

          // p_Total_remainder_loss (i)
          Identifier idP_Total_remainder_loss = project.GetIdentifier("p_Total_remainder_loss");
          object p_Total_remainder_loss = this.m_p_Total_remainder_loss as object;
          idP_Total_remainder_loss.RetrieveArray(ref p_Total_remainder_loss, 0, 0);
          this.m_p_Total_remainder_loss = p_Total_remainder_loss as double[];

          // p_Total_expected_costs (i)
          Identifier idP_Total_expected_costs = project.GetIdentifier("p_Total_expected_costs");
          object p_Total_expected_costs = this.m_p_Total_expected_costs as object;
          idP_Total_expected_costs.RetrieveArray(ref p_Total_expected_costs, 0, 0);
          this.m_p_Total_expected_costs = p_Total_expected_costs as double[];

          // Total_expected_costs1 (i)
          Identifier idTotal_expected_costs1 = project.GetIdentifier("Total_expected_costs1");
          object Total_expected_costs1 = this.m_total_expected_costs1 as object;
          idTotal_expected_costs1.RetrieveArray(ref Total_expected_costs1, 0, 0);
          this.m_total_expected_costs1 = Total_expected_costs1 as double[];

          // Total_expected_costs2 (i)
          Identifier idTotal_expected_costs2 = project.GetIdentifier("Total_expected_costs2");
          object Total_expected_costs2 = this.m_total_expected_costs2 as object;
          idTotal_expected_costs2.RetrieveArray(ref Total_expected_costs2, 0, 0);
          this.m_total_expected_costs2 = Total_expected_costs2 as double[];

          // Totale kosten 'Robust objective'

          // Overall_objective
          this.m_overall_objective = (double)project.get_Value("Overall_objective");

          // Objective_AIMMS
          this.m_objective_AIMMS = (double)project.get_Value("Objective_AIMMS");

          //// Objective_year_based
          this.m_objective_year_based = (double)project.get_Value("Objective_year_based");

          return true;
        }
      }
      catch (Exception) { }

      return false;
    }

    #endregion Member functions ----------------------------------------------

  }
}
