#region Copyright -------------------------------------------------------
// Copyright © 2009, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/Input/Solve.cs 4     28-04-09 13:52 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Aimms;

namespace OptimaliseRing.Aimms
{

  /// <summary>
  /// Solution algorithm
  /// </summary>
  public enum SolutionAlgorithm
  {
    /// <summary>
    /// Solve single
    /// </summary>
    SolveSingle,
    /// <summary>
    /// Solve and tune
    /// </summary>
    SolveAndTune,
    /// <summary>
    /// Iterateprob
    /// </summary>
    Iterateprob,
    /// <summary>
    ///
    /// </summary>
    IterateprobAndCosts,
  }

  /// <summary>
  /// Solve
  /// </summary>
  [Serializable]
  public class Solve : IDeclarations
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Solution algorithm
    /// </summary>
    private SolutionAlgorithm m_solutionAlgorithm;
    /// <summary>
    /// Number of additional moments on either side
    /// </summary>
    private int m_tune_nr;
    /// <summary>
    /// Fraction of interval on either side of moment
    /// </summary>
    private double m_tune_perc_width;
    /// <summary>
    /// Relative tolerance multiplication factor
    /// </summary>
    private double m_tune_tolerance_factor;
    /// <summary>
    /// Case_prefix
    /// </summary>
    private string m_case_prefix;
    /// <summary>
    /// Case_name
    /// </summary>
    private string m_case_name;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets or sets the solution algorithm.
    /// </summary>
    /// <value>The solution algorithm.</value>
    public SolutionAlgorithm SolutionAlgorithm
    {
      get { return m_solutionAlgorithm; }
      set { m_solutionAlgorithm = value; }
    }

    /// <summary>
    /// Gets or sets Number of additional moments on either side
    /// </summary>
    /// <value>Number of additional moments on either side.</value>
    public int Tune_nr
    {
      get { return this.m_tune_nr; }
      set { this.m_tune_nr = value; }
    }

    /// <summary>
    /// Gets or sets Fraction of interval on either side of moment
    /// </summary>
    /// <value>Fraction of interval on either side of moment.</value>
    public double Tune_perc_width
    {
      get { return this.m_tune_perc_width; }
      set { this.m_tune_perc_width = value; }
    }

    /// <summary>
    /// Gets or sets Relative tolerance multiplication factor
    /// </summary>
    /// <value>Relative tolerance multiplication factor.</value>
    public double Tune_tolerance_factor
    {
      get { return this.m_tune_tolerance_factor; }
      set { this.m_tune_tolerance_factor = value; }
    }

    /// <summary>
    /// Gets or sets Case_prefix.
    /// </summary>
    /// <value>Case_prefix.</value>
    public string Case_prefix
    {
      get { return this.m_case_prefix; }
      set { this.m_case_prefix = value; }
    }

    /// <summary>
    /// Gets or sets the case_name.
    /// </summary>
    /// <value>The case_name.</value>
    public string Case_name
    {
      get { return this.m_case_name; }
      set { this.m_case_name = value; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="Solve"/> class.
    /// </summary>
    public Solve()
    {
      this.m_solutionAlgorithm = SolutionAlgorithm.Iterateprob;
      this.m_tune_nr=0;
      this.m_tune_perc_width = 0.0;
      this.m_tune_tolerance_factor = 0.0;
      this.m_case_prefix = string.Format("OptimaliseRing_{0:ddMMyyyyHHmm}", DateTime.Now);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Solve"/> class.
    /// </summary>
    /// <param name="solutionAlgorithm">Solution algorithm.</param>
    /// <param name="tune_nr">Number of additional moments on either side.</param>
    /// <param name="tune_perc_width">Fraction of interval on either side of moment.</param>
    /// <param name="tune_tolerance_factor">Relative tolerance multiplication factor.</param>
    /// <param name="case_prefix">The case_prefix.</param>
    public Solve(
        SolutionAlgorithm solutionAlgorithm, int tune_nr, double tune_perc_width
      , double tune_tolerance_factor, string case_prefix)
    {
      this.m_solutionAlgorithm = solutionAlgorithm;
      this.m_tune_nr = tune_nr;
      this.m_tune_perc_width = tune_perc_width;
      this.m_tune_tolerance_factor = tune_tolerance_factor;
      this.m_case_prefix = case_prefix;
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
        // Solution algorithm
        Set setAlgoritms = project.GetSet("Algoritms");
        int[] algoritmes = new int[setAlgoritms.Card];

        object Algoritms = algoritmes as object;
        setAlgoritms.CompoundTuplePassMode = ELEMENT_PASS_MODE.ELEMENT_BY_NUMBER;
        setAlgoritms.RetrieveElementArray(ref Algoritms);
        algoritmes = Algoritms as int[];

        // Offset op 0 zetten
        setAlgoritms.OrdinalsOffset = 0;

        for (int index = 0; index < setAlgoritms.Card; index++)
        {
          string naam = setAlgoritms.OrdinalToName(index);
          if ((string.Compare(naam, "solve single", true) == 0 && this.m_solutionAlgorithm == SolutionAlgorithm.SolveSingle) ||
              (string.Compare(naam, "solve and tune", true) == 0 && this.m_solutionAlgorithm == SolutionAlgorithm.SolveAndTune) ||
              (string.Compare(naam, "iterate prob", true) == 0 && this.m_solutionAlgorithm == SolutionAlgorithm.Iterateprob) ||
              (string.Compare(naam, "iterate prob & costs", true) == 0 && this.m_solutionAlgorithm == SolutionAlgorithm.IterateprobAndCosts))
          {
            object Selected_algorithm = algoritmes[index] as object;
            project.set_Value("Selected_algorithm", Selected_algorithm);
          }
        }

        // Tune Solution parameters

        // Tune_nr
        object Tune_nr = this.m_tune_nr as object;
        project.set_Value("Tune_nr", Tune_nr);

        // Tune_perc_width
        object Tune_perc_width = this.m_tune_perc_width as object;
        project.set_Value("Tune_perc_width", Tune_perc_width);

        // Tune_tolerance_factor
        object Tune_tolerance_factor = this.m_tune_tolerance_factor as object;
        project.set_Value("Tune_tolerance_factor", Tune_tolerance_factor);

        // Case_prefix
        object Case_prefix = this.m_case_prefix as object;
        project.set_Value("Case_prefix", Case_prefix);

        // Case_name
        object Case_name = this.m_case_name as object;
        project.set_Value("Case_name", Case_name);

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
