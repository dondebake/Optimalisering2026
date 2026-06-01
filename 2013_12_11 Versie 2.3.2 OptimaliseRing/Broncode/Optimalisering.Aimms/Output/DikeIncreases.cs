#region Copyright -------------------------------------------------------
// Copyright © 2009, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/Output/DikeIncreases.cs 4     28-04-09 13:53 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Aimms;

namespace OptimaliseRing.Aimms
{

  /// <summary>
  /// DikeIncrease
  /// </summary>
  [Serializable]
  public class DikeIncrease
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Moment_year
    /// domain: k
    /// eenheid: year
    /// </summary>
    private int[] m_moment_year;

    /// <summary>
    /// Dike_increase
    /// domain: l,k
    /// eenheid: cm
    /// </summary>
    private double[,] m_dike_increase;

    /// <summary>
    /// P_Total_increase
    /// domain: l,k
    /// eenheid: cm
    /// </summary>
    private double[,] m_p_total_increase;

    /// <summary>
    /// Update_undiscounted_costs
    /// domain: i,l,u (scenario, segment, investeringvolgnummer
    /// eenheid: EUR
    /// </summary>
    private double[,,] m_update_undiscounted_costs;

    /// <summary>
    /// Update_discounted_costs
    /// domain: i,l,u (scenario, segment, investeringvolgnummer
    /// eenheid: EUR
    /// </summary>
    private double[,,] m_update_discounted_costs;

    /// <summary>
    /// Total_discounted_cost
    /// domain: i,l
    /// eenheid: EUR
    /// </summary>
    private double[,] m_total_discounted_cost;

    /// <summary>
    /// max_update (aantal investeringen waar kosten worden bepaald voor
    /// m_update_discounted_costs en m_update_undiscounted_costs
    /// domain: u
    /// </summary>
    private int m_max_updates;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Moment_year
    /// domain: k
    /// eenheid: year
    /// </summary>
    /// <value>The moment_year.</value>
    public int[] Moment_year
    {
      get { return this.m_moment_year; }
    }

    /// <summary>
    /// Gets or sets the dike_increase.
    /// </summary>
    /// <value>The dike_increase.</value>
    public double[,] Dike_increase
    {
      get { return this.m_dike_increase; }
    }

    /// <summary>
    /// Gets or sets the p_total_increase.
    /// </summary>
    /// <value>The p_total_increase.</value>
    public double[,] P_total_increase
    {
      get { return this.m_p_total_increase; }
    }


    /// <summary>
    /// Update_undiscounted_costs
    /// domain: i,l,u
    /// eenheid: EUR
    /// </summary>
    /// <value>Update_undiscounted_costs</value>
    public double[,,] Update_undiscounted_costs
    {
      get { return this.m_update_undiscounted_costs; }
    }

    /// <summary>
    /// Discounted_cost_first_update
    /// domain: i,l,u
    /// eenheid: EUR
    /// </summary>
    /// <value>Discounted_cost_first_update.</value>
    public double[,,] Update_discounted_costs
    {
      get { return this.m_update_discounted_costs; }
    }

    /// <summary>
    /// Total_discounted_cost
    /// domain: i,l
    /// eenheid: EUR
    /// </summary>
    /// <value>Total_discounted_cost.</value>
    public double[,] Total_discounted_cost
    {
      get { return this.m_total_discounted_cost; }
    }

    /// <summary>
    /// Gets max_updates (aantal investeringen waar kosten worden bepaald voor
    /// m_update_discounted_costs en m_update_undiscounted_costs
    /// </summary>
    /// <value>Max_updates.</value>
    public int Max_updates
    {
      get { return this.m_max_updates; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="DikeIncrease"/> class.
    /// </summary>
    public DikeIncrease() { }

    #endregion Constructors --------------------------------------------------

    #region Member functions -------------------------------------------------

    /// <summary>
    /// Gets the results.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <param name="scenarios">The scenarios.</param>
    /// <param name="segments">The segments.</param>
    /// <returns></returns>
    public bool GetResults(Project project, Scenarios scenarios, Segments segments)
    {
      try
      {
        // Moment_year (k)
        Identifier idMoment_year = project.GetIdentifier("Moment_year");
        idMoment_year.ElementValuePassMode = ELEMENT_PASS_MODE.ELEMENT_BY_NAME;
        this.m_moment_year = new int[idMoment_year.Card];
        object Moment_year = this.m_moment_year as object;
        idMoment_year.RetrieveArray(ref Moment_year, 0, 0);
        this.m_moment_year = Moment_year as int[];

        int aantalScenarios = scenarios.Count;
        int aantalSegmenten = segments.Count;
        int aantalMoments = idMoment_year.Card;

        this.m_max_updates = (int)project.get_Value("Max_updates");

        // Variabelen initialiseren
        this.m_dike_increase = new double[aantalSegmenten, aantalMoments]; // domain: l,k    *
        this.m_p_total_increase = new double[aantalSegmenten, aantalMoments]; // domain: l,k
        this.m_update_undiscounted_costs = new double[aantalScenarios, aantalSegmenten, this.m_max_updates]; // domain: i,l,u    *
        this.m_update_discounted_costs = new double[aantalScenarios, aantalSegmenten, this.m_max_updates]; // domain: i,l,u    *
        this.m_total_discounted_cost = new double[aantalScenarios, aantalSegmenten]; // domain: i,l  *

        // Gegeven ophalen
        // Dike_increase (k)
        Identifier idDike_increase = project.GetIdentifier("Dike_increase");
        object Dike_increase = this.m_dike_increase as object;
        idDike_increase.RetrieveArray(ref Dike_increase, 0, 0);
        this.m_dike_increase = Dike_increase as double[,];

        // kosten
        // Update_undiscounted_costs (l,u)
        Identifier idUpdate_undiscounted_costs = project.GetIdentifier("Update_undiscounted_costs");
        object Update_undiscounted_costs = this.m_update_undiscounted_costs as object;
        idUpdate_undiscounted_costs.RetrieveArray(ref Update_undiscounted_costs, 0, 0);
        this.m_update_undiscounted_costs = Update_undiscounted_costs as double[,,];

        // Update_discounted_costs (i,l,u)
        Identifier idUpdate_discounted_costs = project.GetIdentifier("Update_discounted_costs");
        object Update_discounted_costs = this.m_update_discounted_costs as object;
        idUpdate_discounted_costs.RetrieveArray(ref Update_discounted_costs, 0, 0);
        this.m_update_discounted_costs = Update_discounted_costs as double[,,];

        // Total_discounted_cost (i,l)
        Identifier idTotal_discounted_cost = project.GetIdentifier("Total_discounted_cost");
        object Total_discounted_cost = this.m_total_discounted_cost as object;
        idTotal_discounted_cost.RetrieveArray(ref Total_discounted_cost, 0, 0);
        this.m_total_discounted_cost = Total_discounted_cost as double[,];

        for (int indexSegment = 0; indexSegment < aantalSegmenten; indexSegment++)
        {
          for (int indexMoment = 0; indexMoment < aantalMoments; indexMoment++)
          {
            if (this.m_dike_increase[indexSegment, indexMoment] > 0.0) //l,k
            {
              this.m_p_total_increase[indexSegment, indexMoment] += this.m_dike_increase[indexSegment, indexMoment];
            }
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
