#region Copyright -------------------------------------------------------
// Copyright © 2009, Rijkswaterstaat/Waterdienst & ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Project    : 1377.10.00 OptimalisatieRing
//              OptimaliseRing - Economische optimalisatie van veiligheidsniveaus van dijkringen
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//              Matthijs Duits, HKV lijn in water
//
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace OptimaliseRing.Core
{
  /// <summary>
  /// Pmidden's per scenario
  /// </summary>
  public class PmiddensPerScenario
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Scenario Id
    /// </summary>
    private int m_Scenario_Id;
    /// <summary>
    /// Overstromings Kans In Zicht jaar
    /// </summary>
    private double m_OverstromingsKansInZichtjaar;
    /// <summary>
    /// Overstromings Kans In Optimale OverstromingskansJaar
    /// </summary>
    private double m_OverstromingsKansInOptimaleOverstromingskansJaar;
    /// <summary>
    /// Optimale Overstromings Kans In Zicht jaar
    /// </summary>
    private double m_OptimaleOverstromingsKansInZichtjaar;
    /// <summary>
    /// Optimale Overstromings Kans In Optimale OverstromingskansJaar
    /// </summary>
    private double m_OptimaleOverstromingsKansInOptimaleOverstromingskansJaar;
    /// <summary>
    /// Optimale Overschrijdings Kans In Zichtjaar
    /// </summary>
    private double m_OptimaleOverschrijdingsKansInZichtjaar;
    /// <summary>
    /// Optimale OverschrijdingsKans In Optimale OverstromingskansJaar
    /// </summary>
    private double m_OptimaleOverschrijdingsKansInOptimaleOverstromingskansJaar;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets Scenario_Id.
    /// </summary>
    /// <value>Scenario_Id.</value>
    public int Scenario_Id
    {
      get { return m_Scenario_Id; }
    }

    /// <summary>
    /// Gets overstromings kans in zichtjaar.
    /// </summary>
    /// <value>overstromings kans in zichtjaar.</value>
    public double OverstromingsKansInZichtjaar
    {
      get { return m_OverstromingsKansInZichtjaar; }
    }

    /// <summary>
    /// Gets overstromings kans in optimale overstromingskans jaar.
    /// </summary>
    /// <value>
    /// overstromings kans in optimale overstromingskans jaar.
    /// </value>
    public double OverstromingsKansInOptimaleOverstromingskansJaar
    {
      get { return m_OverstromingsKansInOptimaleOverstromingskansJaar; }
    }

    /// <summary>
    /// Gets optimale overstromings kans in zichtjaar.
    /// </summary>
    /// <value>optimale overstromings kans in zichtjaar.</value>
    public double OptimaleOverstromingsKansInZichtjaar
    {
      get { return m_OptimaleOverstromingsKansInZichtjaar; }
    }

    /// <summary>
    /// Gets optimale overstromings kans in optimale overstromingskans jaar.
    /// </summary>
    /// <value>
    /// optimale overstromings kans in optimale overstromingskans jaar.
    /// </value>
    public double OptimaleOverstromingsKansInOptimaleOverstromingskansJaar
    {
      get { return m_OptimaleOverstromingsKansInOptimaleOverstromingskansJaar; }
    }

    /// <summary>
    /// Gets optimale overschrijdings kans in zichtjaar.
    /// </summary>
    /// <value>optimale overschrijdings kans in zichtjaar.</value>
    public double OptimaleOverschrijdingsKansInZichtjaar
    {
      get { return m_OptimaleOverschrijdingsKansInZichtjaar; }
    }

    /// <summary>
    /// Gets optimale overschrijdings kans in optimale overstromingskans jaar.
    /// </summary>
    /// <value>
    /// Optimale overschrijdings kans in optimale overstromingskans jaar.
    /// </value>
    public double OptimaleOverschrijdingsKansInOptimaleOverstromingskansJaar
    {
      get { return m_OptimaleOverschrijdingsKansInOptimaleOverstromingskansJaar; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="PmiddensPerScenario"/> class.
    /// </summary>
    /// <param name="scenario_Id">The scenario_ id.</param>
    /// <param name="overstromingsKansInZichtjaar">The overstromings kans in zichtjaar.</param>
    /// <param name="overstromingsKansInOptimaleOverstromingskansJaar">The overstromings kans in optimale overstromingskans jaar.</param>
    /// <param name="optimaleOverstromingsKansInZichtjaar">The optimale overstromings kans in zichtjaar.</param>
    /// <param name="optimaleOverstromingsKansInOptimaleOverstromingskansJaar">The optimale overstromings kans in optimale overstromingskans jaar.</param>
    /// <param name="optimaleOverschrijdingsKansInZichtjaar">The optimale overschrijdings kans in zichtjaar.</param>
    /// <param name="optimaleOverschrijdingsKansInOptimaleOverstromingskansJaar">The optimale overschrijdings kans in optimale overstromingskans jaar.</param>
    public PmiddensPerScenario(
        int scenario_Id
      , double overstromingsKansInZichtjaar
      , double overstromingsKansInOptimaleOverstromingskansJaar
      , double optimaleOverstromingsKansInZichtjaar
      , double optimaleOverstromingsKansInOptimaleOverstromingskansJaar
      , double optimaleOverschrijdingsKansInZichtjaar
      , double optimaleOverschrijdingsKansInOptimaleOverstromingskansJaar)
    {
      this.m_Scenario_Id = scenario_Id;

      this.m_OverstromingsKansInZichtjaar = overstromingsKansInZichtjaar;
      this.m_OverstromingsKansInOptimaleOverstromingskansJaar = overstromingsKansInOptimaleOverstromingskansJaar;

      this.m_OptimaleOverstromingsKansInZichtjaar = optimaleOverstromingsKansInZichtjaar;
      this.m_OptimaleOverstromingsKansInOptimaleOverstromingskansJaar = optimaleOverstromingsKansInOptimaleOverstromingskansJaar;
      this.m_OptimaleOverschrijdingsKansInZichtjaar = optimaleOverschrijdingsKansInZichtjaar;
      this.m_OptimaleOverschrijdingsKansInOptimaleOverstromingskansJaar = optimaleOverschrijdingsKansInOptimaleOverstromingskansJaar;
    }

    #endregion Constructors --------------------------------------------------

  }
}
