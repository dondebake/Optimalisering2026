#region Copyright -------------------------------------------------------
// Copyright © 2009, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/Input/Parameters.cs 4     31-03-09 10:30 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Aimms;

namespace OptimaliseRing.Aimms
{
  /// <summary>
  /// Parameter declarations (Tab Parameters)
  /// </summary>
  [Serializable]
  public class Parameters : IDeclarations
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
    /// Initial_economic_value
    /// var: V_0
    /// eenheid: EUR
    /// </summary>
    private double m_initial_economic_value;
    /// <summary>
    /// Economic_growth
    /// var: gamma
    /// eenheid: 1/year
    /// </summary>
    private double m_economic_growth;
    /// <summary>
    /// Loss_increase
    /// var: zeta
    /// eenheid: 1/cm
    /// </summary>
    private double m_loss_increase;
    /// <summary>
    /// Discount_rate1
    /// var: delta_1
    /// eenheid: 1/year
    /// </summary>
    private double m_discount_rate1;
    /// <summary>
    /// Discount_rate2
    /// var: delta_2
    /// eenheid: 1/year
    /// </summary>
    private double m_discount_rate2;
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
    /// Lowest_segment_fixed
    /// range=binary
    /// default=1
    /// </summary>
    private bool m_lowest_segment_fixed;
    /// <summary>
    /// Segments
    /// var: l (lijst met segmenten)
    /// </summary>
    private Segments m_segments;

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
    /// Gets or sets the initial_economic_value.
    /// </summary>
    /// <value>The initial_economic_value.</value>
    public double Initial_economic_value
    {
      get { return m_initial_economic_value; }
      set { m_initial_economic_value = value; }
    }

    /// <summary>
    /// Gets or sets the economic_growth.
    /// </summary>
    /// <value>The economic_growth.</value>
    public double Economic_growth
    {
      get { return m_economic_growth; }
      set { m_economic_growth = value; }
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
    /// Gets or sets the discount_rate1.
    /// </summary>
    /// <value>The discount_rate1.</value>
    public double Discount_rate1
    {
      get { return m_discount_rate1; }
      set { m_discount_rate1 = value; }
    }

    /// <summary>
    /// Gets or sets the discount_rate2.
    /// </summary>
    /// <value>The discount_rate2.</value>
    public double Discount_rate2
    {
      get { return m_discount_rate2; }
      set { m_discount_rate2 = value; }
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
    /// Gets or sets a value indicating whether this <see cref="T:ParameterDeclarations"/> is lowest_segment_fixed.
    /// </summary>
    /// <value><c>true</c> if lowest_segment_fixed; otherwise, <c>false</c>.</value>
    public bool Lowest_segment_fixed
    {
      get { return m_lowest_segment_fixed; }
      set { m_lowest_segment_fixed = value; }
    }

    /// <summary>
    /// Gets the segments.
    /// </summary>
    /// <value>The segments.</value>
    public List<Segment> Segments
    {
      get { return m_segments; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ParameterDeclarations"/> class.
    /// </summary>
    public Parameters()
    {
      this.m_segments = new Segments();
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

        // General parameters deel 1 **********************************************
        // Initial_economic_value
        object Initial_economic_value = this.m_initial_economic_value;
        project.set_Value("Initial_economic_value", Initial_economic_value);

        // Economic_growth
        object Economic_growth = this.m_economic_growth;
        project.set_Value("Economic_growth", Economic_growth);

        // Loss_increase
        object Loss_increase = this.m_loss_increase;
        project.set_Value("Loss_increase", Loss_increase);

        // Discount_rate1
        object Discount_rate1 = this.m_discount_rate1;
        project.set_Value("Discount_rate1", Discount_rate1);

        // Discount_rate2
        object Discount_rate2 = this.m_discount_rate2;
        project.set_Value("Discount_rate2", Discount_rate2);

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

          // Lowest_segment_fixed
          object Lowest_segment_fixed = this.m_lowest_segment_fixed ? 1 : 0;
          project.set_Value("Lowest_segment_fixed", Lowest_segment_fixed);

          return true;
        }
        else
        {
          return false;
        }
      }
      catch (Exception)
      {
        return false;
      }
    }

    #endregion Member functions ----------------------------------------------

  }
}
