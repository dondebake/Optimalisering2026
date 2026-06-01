#region Copyright -------------------------------------------------------
// Copyright © 2008, Rijkswaterstaat/Waterdienst & ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Project    : 1142.50.00 Batch OptimalisatieRing
//              OptimaliseRing - Economische optimalisatie van veiligheidsniveaus van dijkringen
//
// Author(s)  : Johan Ansink, HKV lijn in water
//              Matthijs Duits, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/OptimaliseRingEnum.cs 1     28-04-09 13:50 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.Text;

namespace OptimaliseRing.Aimms
{

  /// <summary>
  /// Matrix kolom
  /// </summary>
  public enum MatrixKolom
  {
    /// <summary>
    /// Jaartal
    /// </summary>
    JAAR,
    /// <summary>
    /// Overstromingskans
    /// </summary>
    P,
    /// <summary>
    /// Wettelijke norm
    /// </summary>
    PWET,
    /// <summary>
    /// Pmidden
    /// </summary>
    PMIDDEN,
    ///// <summary>
    ///// P-
    ///// </summary>
    //PMIN,
    ///// <summary>
    ///// P+
    ///// </summary>
    //PPLUS,
    /// <summary>
    /// PRESTRICTIE
    /// </summary>
    PRESTRICTIE,
    /// <summary>
    /// Pmidden minimum van alle scenarios
    /// </summary>
    PMIDMIN,
    /// <summary>
    /// Pmidden maximum van alle scenarios
    /// </summary>
    PMIDMAX,
    /// <summary>
    /// Aantal kolommen
    /// </summary>
    AANTAL
  }
}
