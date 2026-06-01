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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.Core/Function.cs 1     16/06/08 10:23 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using OptimaliseRing.Core;

using CenterSpace.NMath.Core;

namespace OptimaliseRing.Core
{
   /// <summary>
   /// klasse Function bevat de schade en investeringsfuncties
   /// </summary>
   public sealed class Function
   {
      #region Member Functions -------------------------------------------------

      /// <summary>
      /// Bereken de overstromingskans van een dijkringtraject op tijdstip t
      /// </summary>
      /// <param name="t">tijdstip</param>
      /// <param name="eta">Structurele stijging relatieve waterstand	[cm/jaar]</param>
      /// <param name="verschilHoogte">Verschilhoogte in het dijkringdeel m+NAP</param>
      /// <param name="alpha">Schaalparameter (exponentiele verdeling) [1/cm]</param>
      /// <param name="pFirst">Overstromingskans op tijdstip 0 [1/jaar]</param>
      /// <param name="factorKans">Vermenigvuldigingsfactor van de kans.</param>
      /// <returns>Overstromingskans op tijdstip t [1/jaar]</returns>
      public static double Pt(int t, double eta, double verschilHoogte, double alpha, double pFirst, double factorKans)
      {
         double result = pFirst * NMathFunctions.Exp(alpha * eta * t)
           * NMathFunctions.Exp(-alpha * verschilHoogte) * factorKans; // ThisAppInstellingen.Instance.FactorKans;
         return result;
      }

      /// <summary>
      /// Bereken de overstromingsschade van een dijkringdeel op tijdstip t
      /// </summary>
      /// <param name="t">tijdstip</param>
      /// <param name="nu">Schaalparameter schade afhankelijk van waterstand [1/cm]</param>
      /// <param name="gamma">Tempo van de economische groei [%/jaar]</param>
      /// <param name="zeta">Stijgingstempo schade per cm verhoging [1/cm]</param>
      /// <param name="vFirst">De overstromingsschade V(0) op tijdstip 0 [M€]</param>
      /// <param name="verschilHoogte">Verschilhoogte in het dijkringdeel m+NAP</param>
      /// <param name="factor">Vermedigvuldigingsfacor groeischade</param>///
      /// <returns>Overstromingsschade in M€</returns>
      ////public static double Vt(int t, double nu, double gamma, double zeta, double vFirst, double verschilHoogte, double factor)
      ////{
      ////   double result = vFirst * NMathFunctions.Exp((factor * gamma + nu) * t) * NMathFunctions.Exp((zeta + nu) * verschilHoogte);
      ////   return result;
      ////}

      /// <summary>
      /// Berekenen overstromingsschade V(0) op tijdstip 0 (zonder verhoging)
      /// </summary>
      /// <param name="instellingen">The instellingen.</param>
      /// <param name="schade">Schade beginjaar in M€</param>
      /// <param name="alpha">Schaalparameter (exponentiele verdeling)</param>
      /// <param name="nu">Schaalparameter schade afhankelijk van waterstand</param>
      /// <param name="aantalInwoners">Het aantal inwoners.</param>
      /// <param name="aantalSlachtoffers">Het aantal slachtoffers.</param>
      /// <param name="aantalGetroffenen">Het aantal getroffenen.</param>
      /// <param name="gamma">Gamma.</param>
      /// <param name="psi">Extra schade per waterstandsverhoging.</param>
      /// <param name="maxEta">Maximale eta over de trajecten</param>
      /// <param name="deltaJaar">Verschil in jaar.</param>
      /// <param name="dijkringspecifiekeFactorSchade">Dijkringspecifieke aanpassingsfactor overstromingsschade.</param>
      /// <returns>Overstromingsschade in M€ op tijdstip 0</returns>
      public static double V(Instellingen instellingen, double schade, double alpha, double nu,
        long aantalInwoners, long aantalSlachtoffers, long aantalGetroffenen, double gamma, double psi, double maxEta
        , int deltaJaar, double dijkringspecifiekeFactorSchade)
      {
        double result = instellingen.AanpassingsfactorOverstromingsschade * dijkringspecifiekeFactorSchade
           * alpha / (alpha - nu) * (schade * instellingen.BeleidsfactorOverstromingsschade
           + aantalInwoners * instellingen.BedragPerInwoner / 1000.0
           + NMathFunctions.Pow(aantalSlachtoffers, instellingen.Aversiefactor)
           * instellingen.BedragPerDodelijkSlachtoffer / 1000.0
           + aantalGetroffenen * instellingen.BedragPerGetroffene / 1000.0);

         return result * NMathFunctions.Exp((instellingen.FactorGroeiSchade * gamma + psi * maxEta) * deltaJaar);

      }

      /// <summary>
      /// Bereken de investeringskosten bij een verhoging
      /// </summary>
      /// <param name="lambda">Schaalparameter van verhogingen [1/cm]</param>
      /// <param name="b">Variabele kosten van de investeringen	[M€/cm]</param>
      /// <param name="C">Vaste kosten van de investeringen	[M€/cm]</param>
      /// <param name="u">De verhoging [cm+NAP]</param>
      /// <returns>
      /// De investeringskosten bij een verhoging [M€]
      /// </returns>
      public static double InvesteringsKosten(double lambda, double b, double c, double u)
      {
         double result = (c + b * u) * NMathFunctions.Exp(lambda * u);
         return result;
      }

      /// <summary>
      /// Beheer en onderhoud toeslag
      /// </summary>
      /// <param name="omega">omega.</param>
      /// <param name="delta">delta.</param>
      /// <returns></returns>
      public static double ToeslagBeheerEnOnderhoud(double omega, double delta)
      {
         double result = 1 + (omega / delta);
         return result;
      }

      /// <summary>
      /// Bereken de som van de eerdere verhogingen.
      /// </summary>
      /// <param name="t">Jaartal</param>
      /// <param name="tFirst">Tijdstip eerste verhoging [jaar]</param>
      /// <param name="tSecond">Periode tussen verhogingen [jaren] </param>
      /// <param name="uFirst">Hoogte eerste verhoging [cm]</param>
      /// <param name="uSecond">Hoogte tweede en volgende verhoging [cm]</param>
      /// <returns>Totale kosten</returns>
      /// <returns>som van de eerdere verhogingen.</returns>
      public static double SomEerdereDijkverhogingen(int t, int tFirst, int tSecond, double uFirst, double uSecond)
      {
         double w = 0.0;

         if (t < tFirst)
         {
            w = 0.0;
         }
         else
         {
            int aantal = (int)((t - tFirst) / tSecond);

            w = uFirst + aantal * uSecond;
         }
         return w;
      }

      /// <summary>
      /// Bereken de hoogte op een tijdstip
      /// </summary>
      /// <param name="t">Tijdstip</param>
      /// <param name="hFirst">Hoogte op tijdstip 0 [cm+NAP]</param>
      /// <param name="tFirst">Tijdstip eerste verhoging [jaar]</param>
      /// <param name="tSecond">Periode tussen verhogingen [jaren]</param>
      /// <param name="uFirst">Hoogte eerste verhoging [cm]</param>
      /// <param name="uSecond">Hoogte tweede en volgende verhoging [cm]</param>
      /// <returns>Hoogte</returns>
      public static double Hoogte(int t, double hFirst, int tFirst, int tSecond, double uFirst, double uSecond)
      {
         double h = 0.0;

         if (t < tFirst)
         {
            h = hFirst;
         }
         else
         {
            int aantal = (int)((t - tFirst) / tSecond);
            //// Slechts 2 investeringen
            //aantal = Math.Min(aantal, 1);
            h = hFirst + uFirst + aantal * uSecond;
         }
         return h;
      }

      /// <summary>
      /// Bereken de gemiddelde verwachte schade.
      /// </summary>
      /// <param name="t">Jaartal</param>
      /// <param name="tFirst">Tijdstip eerste verhoging [jaar]</param>
      /// <param name="tSecond">Periode tussen verhogingen [jaren]</param>
      /// <param name="s">Verwachte schades</param>
      /// <param name="v">Overstromingsschades</param>
      /// <returns>Gemiddelde verwachte schade</returns>
      public static double GemiddeldeVerwachteSchade(int t, int tFirst, int tSecond, DoubleVector s, DoubleVector v)
      {
         double result = Double.NaN;
         if (t >= tFirst)
         {
            int n = (int)((t - tFirst) / tSecond);

            int start = tFirst + n * tSecond;
            int einde = Math.Min(start + tSecond, s.Length - 1);

            double sGem = 0.0;
            double vGem = 0.0;

            for (int i = start; i < einde; i++)
            {
               sGem += s[i];
               vGem += v[i];
            }

            sGem /= tSecond;
            vGem /= tSecond;

            result = sGem / vGem;
         }
         return result;
      }

      #endregion Member Functions -------------------------------------------------
   }
}
