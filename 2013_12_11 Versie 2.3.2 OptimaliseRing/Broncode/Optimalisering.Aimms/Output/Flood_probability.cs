#region Copyright -------------------------------------------------------
// Copyright © 2007, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/Output/Flood_probability.cs 3     28-04-09 13:53 Waterman $
// $NoKeywords: $
#endregion


using System;
using System.Collections.Generic;
using System.Text;
using Aimms;
using CenterSpace.NMath.Core;

namespace OptimaliseRing.Aimms
{

  /// <summary>
  /// Probabilities
  /// </summary>
  [Serializable]
  public class Probabilities
  {

    #region Instance Variables -----------------------------------------------

    private int[] m_all_years;
    private double[,] m_p_mid_final; //i,y
    private double[, ,] m_segment_flood_probability;

    /// <summary>
    /// MatrixData
    /// </summary>
    private DoubleMatrix m_matrixData;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    public int[] All_years
    {
      get { return this.m_all_years; }
    }

    public double[,] P_mid_final
    {
      get { return this.m_p_mid_final; }
    }

    public double[, ,] Segment_flood_probability
    {
      get { return this.m_segment_flood_probability; }
    }

    /// <summary>
    /// Gets the matrix data.
    /// </summary>
    /// <value>The matrix data.</value>
    public DoubleMatrix MatrixData
    {
      get { return m_matrixData; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="Probabilities"/> class.
    /// </summary>
    public Probabilities() { }

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
        if (project != null)
        {
          // Alle jaren ophalen
          Set setAll_years = project.GetSet("All_years");

          int aantalScenarios = scenarios.Count;
          int aantalSegmenten = segments.Count;
          int aantalJaren = setAll_years.Card;

          int[] ordinals = new int[aantalJaren];
          object All_years = ordinals as object;
          setAll_years.CompoundTuplePassMode = ELEMENT_PASS_MODE.ELEMENT_BY_ORDINAL;
          setAll_years.RetrieveElementArray(ref All_years);
          ordinals = All_years as int[];

          // Offset op 0 zetten
          setAll_years.OrdinalsOffset = 0;

          this.m_all_years = new int[aantalJaren];
          for (int index = 0; index < aantalJaren; index++)
          {
            int jaar;
            if (int.TryParse(setAll_years.OrdinalToName(index), out jaar))
            {
              this.m_all_years[index] = jaar;
            }
          }

          this.m_p_mid_final = new double[aantalScenarios, aantalJaren];
          this.m_segment_flood_probability = new double[aantalScenarios, aantalSegmenten, aantalJaren];

          // P_mid_final(i,y)
          Identifier idP_mid_final = project.GetIdentifier("P_mid_final");
          object P_mid_final = this.m_p_mid_final as object;
          idP_mid_final.RetrieveArray(ref P_mid_final, 0, 0);
          this.m_p_mid_final = P_mid_final as double[,];

          // Segment_flood_probability(i,l,y)
          Identifier idSegment_flood_probability = project.GetIdentifier("Segment_flood_probability");
          object Segment_flood_probability = this.m_segment_flood_probability as object;
          idSegment_flood_probability.RetrieveArray(ref Segment_flood_probability, 0, 0);
          this.m_segment_flood_probability = Segment_flood_probability as double[,,];
        }
        return true;
      }
      catch (Exception) { }
      return false;
    }

    #endregion Member functions ----------------------------------------------

  }
}
