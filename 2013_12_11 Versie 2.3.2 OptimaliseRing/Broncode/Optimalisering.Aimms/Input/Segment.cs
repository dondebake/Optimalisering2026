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
  /// Segments
  /// var: l
  /// </summary>
  [Serializable]
  public class Segment
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Segment
    /// </summary>
    private string m_segment;
    /// <summary>
    /// Id gekregen in Aimms
    /// </summary>
    private int m_segmentCardId;
    /// <summary>
    /// Initial_height
    /// var: H_0
    /// eenheid: m
    /// domein: l (segments)
    /// </summary>
    private double m_initial_height;
    /// <summary>
    /// Exp_power
    /// var: lambda
    /// eenheid: 1/cm
    /// domein: l (segments)
    /// </summary>
    private double m_exp_power;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets the segmentname.
    /// </summary>
    /// <value>The segmentname.</value>
    public string SegmentName
    {
      get { return this.m_segment; }
    }

    /// <summary>
    /// Gets the segment card id.
    /// </summary>
    /// <value>The segment card id.</value>
    public int SegmentCardId
    {
      set { this.m_segmentCardId = value; }
      get { return this.m_segmentCardId; }
    }

    /// <summary>
    /// Gets the initial_height.
    /// </summary>
    /// <value>The initial_height.</value>
    public double Initial_height
    {
      get { return this.m_initial_height; }
    }
    /// <summary>
    /// Gets or sets the Exp_power.
    /// </summary>
    /// <value>
    /// Exp_power
    /// var: lambda
    /// eenheid: 1/cm
    /// domein: l (segments)
    /// </value>
    public double Exp_power
    {
      get { return this.m_exp_power; }
      set { this.m_exp_power = value; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Segment"/> class.
    /// </summary>
    public Segment() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Segment"/> class.
    /// </summary>
    /// <param name="segment">The segment.</param>
    /// <param name="cardId">The card id.</param>
    public Segment(string segment, int cardId)
    {
      this.m_segment = segment;
      this.m_segmentCardId = cardId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Segment"/> class.
    /// </summary>
    /// <param name="segment">Segment</param>
    /// <param name="initial_height">Initial_height (H_0)</param>
    /// <param name="exp_power">The exp_power.</param>
    public Segment(string segment, double initial_height, double exp_power)
    {
      this.m_segment = segment;
      this.m_initial_height = initial_height;
      this.m_exp_power = exp_power;
    }

    #endregion Constructors --------------------------------------------------
  }

  /// <summary>
  /// var: l (lijst met segmenten)
  /// </summary>
  [Serializable]
  public class Segments : List<Segment>, IDeclarations
  {
    /// <summary>
    /// Registreer segment variabelen.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <returns></returns>
    public bool RegisterVariabelen(Project project)
    {
      try
      {
        string[] segment = new string[this.Count];
        double[] initial_height = new double[this.Count];
        double[] initial_flood_probability = new double[this.Count];
        double[] increase_of_water_level = new double[this.Count];
        double[] extreme_water = new double[this.Count];
        double[] exp_power = new double[this.Count];

        int teller = 0;

        foreach (Segment segmentItem in this)
        {
          segment[teller] = segmentItem.SegmentName;
          initial_height[teller] = segmentItem.Initial_height;
          exp_power[teller++] = segmentItem.Exp_power;
        }

        object Segments = segment as object;
        project.AssignElementArray("Segments", ref Segments, REPLACE_MODE.REPLACE);

        // Ophalen Segments index
        Set setSegments = project.GetSet("Segments");
        int[] segments = new int[setSegments.Card];

        object SegmentsUit = segments as object;
        setSegments.CompoundTuplePassMode = ELEMENT_PASS_MODE.ELEMENT_BY_NUMBER;
        setSegments.RetrieveElementArray(ref SegmentsUit);
        segments = SegmentsUit as int[];

        // Offset op 0 zetten
        setSegments.OrdinalsOffset = 0;

        // Lijstje afdrukken
        for (int index = 0; index < setSegments.Card; index++)
        {
          string naam = setSegments.OrdinalToName(index);
          Segment segmentItem;
          if ((segmentItem = this.getSegmentByName(naam)) != null)
          {
            segmentItem.SegmentCardId = segments[index];
          }
        }
        // Andere parameters doorgeven
        object Initial_height = initial_height as object;
        project.AssignArray("Initial_height", ref Initial_height, 0);

        object Exp_power = exp_power as object;
        project.AssignArray("Exp_power", ref Exp_power, 0);

        return true;
      }
      catch (Exception)
      {
        return false;
      }

    }

    /// <summary>
    /// Ophalen alleen segmentnaam en CardId
    /// </summary>
    /// <param name="project">The project.</param>
    public bool GetResults(Project project)
    {
      try
      {
        // Huidige segmenten verwijderen
        this.Clear();

        if (project != null)
        {
          // Ophalen Segments index
          Set setSegments = project.GetSet("Segments");
          int[] segments = new int[setSegments.Card];

          object SegmentsUit = segments as object;
          setSegments.CompoundTuplePassMode = ELEMENT_PASS_MODE.ELEMENT_BY_NUMBER;
          setSegments.RetrieveElementArray(ref SegmentsUit);
          segments = SegmentsUit as int[];

          // Offset op 0 zetten
          setSegments.OrdinalsOffset = 0;

          // Lijstje toevoegen
          for (int index = 0; index < setSegments.Card; index++)
          {
            string naam = setSegments.OrdinalToName(index);
            this.Add(new Segment(naam, segments[index]));
          }
        }
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// Gets the segment by name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    private Segment getSegmentByName(string name)
    {
      foreach (Segment segment in this)
      {
        if (string.Compare(segment.SegmentName, name, true) == 0)
        {
          return segment;
        }
      }
      // Niets gevonden
      return null;
    }
  }
}
