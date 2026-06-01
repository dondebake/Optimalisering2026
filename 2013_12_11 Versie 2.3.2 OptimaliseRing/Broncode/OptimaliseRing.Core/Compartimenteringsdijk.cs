#region Copyright -------------------------------------------------------
// Copyright © 2008, Rijkswaterstaat/Waterdienst & ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Project    : 1142.50.00 Batch OptimalisatieRing
//              OptimaliseRing - Economische optimalisatie van veiligheidsniveaus van dijkringen
//
// Author(s)  : Johan Ansink, HKV lijn in water
//              Matthijs Duits, HKV lijn in water
//              Rolf Waterman, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.Core/Compartimenteringsdijk.cs 1     16/06/08 10:23 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace OptimaliseRing.Core
{
   /// <summary>
   /// Compartimenteringsdijk
   /// </summary>
   public class Compartimenteringsdijk
   {
      #region Instance Variables -----------------------------------------------

      private string m_DijkId;
      private string m_Dijkdeel;

      #endregion

      #region Instance Variables -----------------------------------------------

      /// <summary>
      /// Dijkring compartimentnummer
      /// </summary>
      /// <value>The dijk id.</value>
      public string DijkId
      {
         get { return m_DijkId; }
         set { m_DijkId = value; }
      }

      /// <summary>
      /// Dijkringdeel identificatie
      /// </summary>
      /// <value>The dijkdeel.</value>
      public string Dijkdeel
      {
         get { return m_Dijkdeel; }
         set { m_Dijkdeel = value; }
      }

      #endregion

   }
}
