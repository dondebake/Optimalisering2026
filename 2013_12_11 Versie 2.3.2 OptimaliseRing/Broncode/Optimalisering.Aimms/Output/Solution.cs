#region Copyright -------------------------------------------------------
// Copyright © 2009, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/Output/Solution.cs 2     30-03-09 13:16 Waterman $
// $NoKeywords: $
#endregion


using System;
using System.Collections.Generic;
using System.Text;
using Aimms;

namespace OptimaliseRing.Aimms
{
  /// <summary>
  /// Solution
  /// </summary>
  [Serializable]
  public class Solution
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Solutions
    /// index: s
    /// parameter: this_s
    /// </summary>
    private string m_solutionName;
    /// <summary>
    /// sol_description
    /// domain: s
    /// eenheid: -
    /// </summary>
    private string m_sol_description;
    /// <summary>
    /// sol_objective_aimms
    /// domain: s
    /// eenheid: EUR
    /// </summary>
    private double m_sol_objective_aimms;
    /// <summary>
    /// sol_objective_year
    /// domain: s
    /// eenheid: EUR
    /// </summary>
    private double m_sol_objective_year;
    /// <summary>
    /// sol_time
    /// domain: s
    /// eenheid: minutes
    /// </summary>
    private double m_sol_time;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets or sets Solutions
    /// index: s
    /// parameter: this_s
    /// </summary>
    /// <value>The solution.</value>
    public string SolutionName
    {
      get { return this.m_solutionName; }
    }

    /// <summary>
    /// Gets or sets sol_description
    /// domain: s
    /// eenheid: -
    /// </summary>
    /// <value>The sol_description.</value>
    public string Sol_description
    {
      get { return this.m_sol_description; }
    }

    /// <summary>
    /// Gets or sets sol_objective_aimms
    /// domain: s
    /// eenheid: EUR
    /// </summary>
    /// <value>The sol_objective_aimms.</value>
    public double Sol_objective_aimms
    {
      get { return this.m_sol_objective_aimms; }
    }

    /// <summary>
    /// Gets or sets sol_objective_year
    /// domain: s
    /// eenheid: EUR
    /// </summary>
    /// <value>The sol_objective_year.</value>
    public double Sol_objective_year
    {
      get { return this.m_sol_objective_year; }
    }

    /// <summary>
    /// Gets or sets sol_time
    /// domain: s
    /// eenheid: minutes
    /// </summary>
    /// <value>The sol_time.</value>
    public double Sol_time
    {
      get { return this.m_sol_time; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="Solution"/> class.
    /// </summary>
    public Solution()
    {
      this.m_solutionName = string.Empty;
      this.m_sol_description = string.Empty;
      this.m_sol_objective_aimms = 0.0;
      this.m_sol_objective_year = 0.0;
      this.m_sol_time = 0.0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Solution"/> class.
    /// </summary>
    /// <param name="solutionName">Name of the solution.</param>
    /// <param name="sol_description">The sol_description.</param>
    /// <param name="sol_objective_aimms">The sol_objective_aimms.</param>
    /// <param name="sol_objective_year">The sol_objective_year.</param>
    /// <param name="sol_time">The sol_time.</param>
    public Solution(
      string solutionName,
      string sol_description,
      double sol_objective_aimms,
      double sol_objective_year,
      double sol_time)
    {
      this.m_solutionName = solutionName;
      this.m_sol_description = sol_description;
      this.m_sol_objective_aimms = sol_objective_aimms;
      this.m_sol_objective_year = sol_objective_year;
      this.m_sol_time = sol_time;
    }


    #endregion Constructors --------------------------------------------------

  }

  /// <summary>
  /// Solution
  /// </summary>
  [Serializable]
  public class Solutions : List<Solution>
  {
    #region Instance Variables -----------------------------------------------
    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------
    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="Solutions"/> class.
    /// </summary>
    public Solutions()    {    }

    #endregion Constructors --------------------------------------------------

    #region Member functions -------------------------------------------------

    /// <summary>
    /// Gets the results.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <returns></returns>
    public bool GetResults(Project project)
    {
      try
      {
        if (project != null)
        {
          // Ophalen solutions
          Set setSolutions = project.GetSet("Solutions");
          int[] solutions = new int[setSolutions.Card];

          object Solutions = solutions as object;
          setSolutions.CompoundTuplePassMode = ELEMENT_PASS_MODE.ELEMENT_BY_NUMBER;
          setSolutions.RetrieveElementArray(ref Solutions);
          solutions = Solutions as int[];

          // Offset op 0 zetten
          setSolutions.OrdinalsOffset = 0;

          // sol_description
          Identifier idSol_description = project.GetIdentifier("sol_description");
          string[] sol_description = new string[setSolutions.Card];
          object Sol_description = sol_description as object;
          idSol_description.RetrieveArray(ref Sol_description, 0, 0);
          sol_description = Sol_description as string[];

          // sol_objective_aimms
          Identifier idSol_objective_aimms = project.GetIdentifier("sol_objective_aimms");
          double[] sol_objective_aimms = new double[setSolutions.Card];
          object Sol_objective_aimms = sol_objective_aimms as object;
          idSol_objective_aimms.RetrieveArray(ref Sol_objective_aimms, 0, 0);
          sol_objective_aimms = Sol_objective_aimms as double[];

          // sol_objective_year
          Identifier idSol_objective_year = project.GetIdentifier("sol_objective_year");
          double[] sol_objective_year = new double[setSolutions.Card];
          object Sol_objective_year = sol_objective_year as object;
          idSol_objective_year.RetrieveArray(ref Sol_objective_year, 0, 0);
          sol_objective_year = Sol_objective_year as double[];

          // m_sol_time
          Identifier idSol_time = project.GetIdentifier("sol_time");
          double[] sol_time = new double[setSolutions.Card];
          object Sol_time = sol_time as object;
          idSol_time.RetrieveArray(ref Sol_time, 0, 0);
          sol_time = Sol_time as double[];


          for (int index = 0; index < setSolutions.Card; index++)
          {
            string naam = setSolutions.OrdinalToName(index);
            //string sol_description = setSol_description.OrdinalToName(index);
            //Console.WriteLine(string.Format("{0}- {1}", solutions[index], naam));

            this.Add(new Solution(naam, sol_description[index]
              , sol_objective_aimms[index], sol_objective_year[index], sol_time[index]));

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
