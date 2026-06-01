#region Copyright -------------------------------------------------------
// Copyright © 2009, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/Interface/IDeclarations.cs 3     31-03-09 10:30 Waterman $
// $NoKeywords: $
#endregion

using System;
using Aimms;

namespace OptimaliseRing.Aimms
{
  interface IDeclarations
  {
    bool RegisterVariabelen(Project project);
  }
}
