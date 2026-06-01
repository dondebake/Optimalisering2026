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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.Core/OptimaliseRingEnum.cs 2     28-04-09 14:00 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.Text;

namespace OptimaliseRing.Core
{
  /// <summary>
  /// pointer mode
  /// </summary>
  public enum PointerMode
  {
    /// <summary>
    /// pointer mode select
    /// </summary>
    Select,
    /// <summary>
    /// pointer mode select
    /// </summary>
    ZoomIn,
    /// <summary>
    /// pointer mode zoom in
    /// </summary>
    ZoomOut,
    /// <summary>
    /// pointer mode zoom out
    /// </summary>
    ZoomArea,
    /// <summary>
    /// pointer mode zoom area
    /// </summary>
    Drag,
    /// <summary>
    /// pointer mode drag
    /// </summary>
    Center,
    /// <summary>
    /// pointer mode whole world
    /// </summary>
    World
  };

  /// <summary>
  /// view mode
  /// </summary>
  public enum ViewMode
  {
    /// <summary>
    /// Overstromingskans
    /// </summary>
    Overstromingskans,
    /// <summary>
    /// Overschrijdingskans
    /// </summary>
    Overschrijdingskans
  }

  /// <summary>
  /// Optimization methods
  /// </summary>
  public enum OptimizeMethod
  {
    /// <summary>
    /// Downhill simplex
    /// </summary>
    DownhillSimplex = 1,
    /// <summary>
    /// Powell's Method
    /// </summary>
    PowellsMethod,
    /// <summary>
    /// Simulated annealing
    /// </summary>
    SimulatedAnnealing
  }
}
