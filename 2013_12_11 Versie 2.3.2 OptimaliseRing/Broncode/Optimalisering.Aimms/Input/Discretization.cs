#region Copyright -------------------------------------------------------
// Copyright © 2009, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/Input/Discretization.cs 3     31-03-09 10:30 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Aimms;

namespace OptimaliseRing.Aimms
{

  /// <summary>
  /// DiscretizationMethod
  /// </summary>
  [Serializable]
  public enum DiscretizationMethod
  {
    /// <summary>
    /// Absolute
    /// </summary>
    Absolute,
    /// <summary>
    /// Percentage
    /// </summary>
    Percentage,
    /// <summary>
    /// Keep
    /// </summary>
    Keep,
    /// <summary>
    /// Full
    /// </summary>
    Full
  }

  /// <summary>
  /// Discretization
  /// </summary>
  [Serializable]
  public class Discretization : IDeclarations
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Time_horizon
    /// eenheid: year
    /// </summary>
    private int m_time_horizon;
    /// <summary>
    /// DiscretizationMethod
    /// </summary>
    private DiscretizationMethod m_discretizationMethod;
    /// <summary>
    /// Nr_variables
    /// </summary>
    private int m_nr_variables;
    /// <summary>
    /// Lijst met partitions
    /// </summary>
    private Partitions m_partitions;
    /// <summary>
    /// Option_convert_solution
    /// </summary>
    private bool m_option_convert_solution;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets or sets Time_horizon
    /// eenheid: year
    /// </summary>
    /// <value>The time_horizon.</value>
    public int Time_horizon
    {
      get { return this.m_time_horizon; }
      set { this.m_time_horizon = value; }
    }

    /// <summary>
    /// Gets or sets the discretization method.
    /// </summary>
    /// <value>The discretization method.</value>
    public DiscretizationMethod DiscretizationMethod
    {
      get { return this.m_discretizationMethod; }
      set { this.m_discretizationMethod = value; }
    }

    /// <summary>
    /// Gets or sets the nr_variables.
    /// </summary>
    /// <value>The nr_variables.</value>
    public int Nr_variables
    {
      get { return this.m_nr_variables; }
      set { this.m_nr_variables = value; }
    }

    /// <summary>
    /// Gets or sets the partitions.
    /// </summary>
    /// <value>The partitions.</value>
    public Partitions Partitions
    {
      get { return this.m_partitions; }
      set { this.m_partitions = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="Discretization"/> is option_convert_solution.
    /// </summary>
    /// <value><c>true</c> if option_convert_solution; otherwise, <c>false</c>.</value>
    public bool Option_convert_solution
    {
      get { return this.m_option_convert_solution; }
      set { this.m_option_convert_solution = value; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="Discretization"/> class.
    /// </summary>
    public Discretization()
    {
      this.m_time_horizon = 300;
      this.m_discretizationMethod = DiscretizationMethod.Absolute;
      this.m_nr_variables = 30;
      this.Partitions = new Partitions();
      this.m_option_convert_solution = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Discretization"/> class.
    /// </summary>
    /// <param name="time_horizon">The time_horizon.</param>
    /// <param name="discretizationMethod">The discretization method.</param>
    /// <param name="nr_variables">The nr_variables.</param>
    /// <param name="partitions">The partitions.</param>
    /// <param name="option_convert_solution">if set to <c>true</c> [option_convert_solution].</param>
    public Discretization(
        int time_horizon, DiscretizationMethod discretizationMethod
      , int nr_variables, Partitions partitions, bool option_convert_solution)
    {
      this.m_time_horizon = time_horizon;
      this.m_discretizationMethod = discretizationMethod;
      this.m_nr_variables = nr_variables;
      this.m_partitions = partitions;
      this.m_option_convert_solution = option_convert_solution;
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
        // Time_horizon
        object Time_horizon = this.m_time_horizon as object;
        project.set_Value("Time_horizon", Time_horizon);

        // DiscretizationMethod
        Set setDiscretization = project.GetSet("Discretization_methods");
        int[] discretization = new int[setDiscretization.Card];

        object Discretization = discretization as object;
        setDiscretization.CompoundTuplePassMode = ELEMENT_PASS_MODE.ELEMENT_BY_NUMBER;
        setDiscretization.RetrieveElementArray(ref Discretization);
        discretization = Discretization as int[];

        // Huidige keuze ophalen
        int selected_discretization_method = (int)project.get_Value("Selected_discretization_method");

        // Offset op 0 zetten
        setDiscretization.OrdinalsOffset = 0;

        // Lijstje afdrukken (* is huidige keuze)
        for (int index = 0; index < setDiscretization.Card; index++)
        {
          string naam = setDiscretization.OrdinalToName(index);
          if (string.Compare(naam, this.m_discretizationMethod.ToString(), true) == 0)
          {
            object Selected_discretization_method = discretization[index] as object;
            project.set_Value("Selected_discretization_method", Selected_discretization_method);
          }
        }

        object Nr_variables = this.m_nr_variables as object;
        project.set_Value("Nr_variables", Nr_variables);

        // Partitions
        this.m_partitions.RegisterVariabelen(project);

        // Option_convert_solution
        object Option_convert_solution = this.m_option_convert_solution ? 1 : 0 as object;
        project.set_Value("Option_convert_solution", Option_convert_solution);

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
