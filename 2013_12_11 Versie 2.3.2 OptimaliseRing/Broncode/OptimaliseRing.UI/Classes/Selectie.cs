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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.UI/Classes/Selectie.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using OptimaliseRing.General;
using OptimaliseRing.Core;

namespace OptimaliseRing.UI
{
   /// <summary>
   /// Selectie
   /// </summary>
   public class Selectie
   {
      #region Instance Variables -----------------------------------------------

      private string m_DijkringId;            // dijkring id
      private string m_DijkringNaam;          // dijkring naam
      private string m_DijkringDeelNaam;      // dijkringdeel naam

      #endregion Instance Variables --------------------------------------------

      #region Constructors -----------------------------------------------------

      /// <summary>
      /// Maak een nieuwe instantiatie van de  <see cref="T:Selectie"/> class.
      /// </summary>
      /// <param name="dijkringId">Dijkring identificatie</param>
      /// <param name="dijkringDeel">The dijkring deel.</param>
      public Selectie(OptimaliseRingDB optimaliseRingDB, string dijkringId, int dijkringDeel)
      {
         this.m_DijkringId = dijkringId;
         this.m_DijkringNaam = optimaliseRingDB.DijkringNaam(dijkringId);
         this.m_DijkringDeelNaam = optimaliseRingDB.DijkringDeelNaam(dijkringId, dijkringDeel);
         if (m_DijkringDeelNaam == m_DijkringNaam)
         {
            m_DijkringDeelNaam = "";
         }
      }

      #endregion Constructors --------------------------------------------------

      #region Properties -------------------------------------------------------

      /// <summary>
      /// Dijkring identificatie
      /// </summary>
      /// <value>dijkring identificatie</value>
      public string DijkringId
      {
         get { return this.m_DijkringId; }
      }

      /// <summary>
      /// Dijkringnaam.
      /// </summary>
      /// <value>Dijkringnaam</value>
      public string DijkringNaam
      {
         get { return this.m_DijkringNaam; }
      }

      /// <summary>
      /// Gets the dijkring deel naam.
      /// </summary>
      /// <value>The dijkring deel naam.</value>
      public string DijkringDeelNaam
      {
         get { return this.m_DijkringDeelNaam; }
      }

      #endregion Properties ----------------------------------------------------


   }
}
