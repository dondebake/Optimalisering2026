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
  /// Investering (verhoging van een dijkring + kosten)
  /// </summary>
  public class Investering
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Jaar waarin de dijk wordt verbeterd
    /// </summary>
    private int m_Jaar;
    /// <summary>
    /// Hoogte voor het verbeteren van de dijk
    /// </summary>
    private double m_Hoogte;
    /// <summary>
    /// Kosten voor het verbeteren van de dijk
    /// </summary>
    private double m_Kosten;
    /// <summary>
    /// Verdisconteerde kosten (discounted_costs)per scenario
    /// </summary>
    private double[] m_VerdisconteerdeKosten;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets Jaar waarin de dijk wordt verbeterd
    /// </summary>
    /// <value>Jaar.</value>
    public int Jaar
    {
      get { return m_Jaar; }
    }

    /// <summary>
    /// Gets Hoogte voor het verbeteren van de dijk
    /// </summary>
    /// <value>Hoogte.</value>
    public double Hoogte
    {
      get { return m_Hoogte; }
    }

    /// <summary>
    /// Gets Kosten voor het verbeteren van de dijk
    /// </summary>
    /// <value>Kosten.</value>
    public double Kosten
    {
      get { return m_Kosten; }
    }

    /// <summary>
    /// Gets Verdisconteerde kosten (discounted_costs)per scenario
    /// </summary>
    /// <value>The verdisconteerde kosten.</value>
    public double[] VerdisconteerdeKosten
    {
      get { return m_VerdisconteerdeKosten; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="Investering"/> class.
    /// </summary>
    /// <param name="jaar">The jaar.</param>
    /// <param name="hoogte">The hoogte.</param>
    /// <param name="kosten">The kosten.</param>
    /// <param name="verdisconteerdeKosten">The verdisconteerde kosten.</param>
    public Investering(int jaar, double hoogte, double kosten, double[] verdisconteerdeKosten)
    {
      this.m_Jaar = jaar;
      this.m_Hoogte = hoogte;
      this.m_Kosten = kosten;
      this.m_VerdisconteerdeKosten = verdisconteerdeKosten;
    }

    #endregion Constructors --------------------------------------------------

  }
}
