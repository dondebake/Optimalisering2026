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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.Core/Optimize.cs 2     7-04-09 7:47 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using OptimaliseRing.General;
using OptimaliseRing.Core;
//using OptimaliseRing.Profile;

using CenterSpace.NMath.Core;
using CenterSpace.NMath.Analysis;

namespace OptimaliseRing.Core
{
   /// <summary>
   /// Optimize
   /// </summary>
   public class Optimize
   {
      #region Instance Variables -----------------------------------------------
      private DijkringDeel m_DijkringDeel;                           // Dijkring deel
      private double m_Delta;                                        // Discontovoet investeringen;
      private double m_StartVectorT1;                                // Start waarde tijdstip eerste verhoging [jaar]
      private double m_StartVectorT2;                                // Start waarde periode tussen verhogingen [jaren]
      private double m_StartVectorU1;                                // Start waarde hoogte eerste verhoging [cm
      private double m_StartVectorU2;                                // Start waarde hoogte tweede en volgende verhoging [cm]
      private OptimizeMethod m_OptimizationMethod;                   // Optimalisatie methode
      private int m_Z;                                               // Aantal tijdstappen
      private int m_BeginJaar;                                       // Beginjaar
      private DoubleVector m_Minimum;                                // Vector met het optimalisatie minimum
      private DoubleVector m_P;                                      // Vector met de overstromingskansen
      private DoubleVector m_S;                                      // Vector met de verwachte schades
      private DoubleVector m_V;                                      // Vector met de overstromingsschades
      private double m_V0;                                           // Overstromingsschade op tijdstip 0 (zonder verhoging)
      private DoubleVector m_VerdisconteerdeKostenEersteInvestering; // Investeringskosten eerste investering
      private DoubleVector m_EersteInvestering;                      // Investeringskosten eerste investering
      private DoubleVector m_TotaleInvestering;                      // Investeringskosten totaal;
      private double m_GlobalMinimum;                                // Global minimum
      private double m_FeitelijkOverstromingInZichtJaar;             // Feitelijke overstromingskans in zichtjaar
      private double m_FeitelijkOverschrijdingInZichtJaar;           // Feitelijke overschrijding in zichtjaar
      private int m_IndexZichtJaar;                                  // Index zichtjaar
      private int m_JaartalZichtduur;                                // Jaartal zichtduur
      private int m_IndexJaartalZichtduur;                           // Index jaartal zichtduur
      private Instellingen m_Instellingen;

      #endregion Instance Variables --------------------------------------------

      #region Constructors -----------------------------------------------------
      /// <summary>
      /// Maak een nieuwe instantiatie van de  <see cref="T:Optimize"/> class.
      /// </summary>
      public Optimize(Instellingen instellingen, DijkringDeel dijkringDeel)
      {
         m_Instellingen = instellingen;
         m_DijkringDeel = dijkringDeel;

         // Discontovoet investeringen
         m_Delta = m_Instellingen.DiscontovoetInvesteringen / 100;
      }

      #endregion Constructors -----------------------------------------------------

      #region Properties -------------------------------------------------------

      public double StartVectorT1
      {
         get { return m_StartVectorT1; }
         set { m_StartVectorT1 = value; }
      }

      public double StartVectorT2
      {
         get { return m_StartVectorT2; }
         set { m_StartVectorT2 = value; }
      }

      public double StartVectorU1
      {
         get { return m_StartVectorU1; }
         set { m_StartVectorU1 = value; }
      }

      public double StartVectorU2
      {
         get { return m_StartVectorU2; }
         set { m_StartVectorU2 = value; }
      }
      /// <summary>
      /// Discontovoet investeringen
      /// </summary>
      /// <value>Discontovoet investeringen</value>
      public double Delta
      {
         set { m_Delta = value; }
         get { return m_Delta; }
      }

      /// <summary>
      /// Index jaartal zichtduur.
      /// </summary>
      /// <value>Index jaartal zichtduur.</value>
      public int IndexJaartalZichtduur
      {
         get { return m_IndexJaartalZichtduur; }
      }

      /// <summary>
      /// Feitelijke overstromingskans in zichtjaar
      /// </summary>
      /// <value>Feitelijke overstromingskans in zichtjaar.</value>
      public double FeitelijkOverstromingInZichtJaar
      {
         get { return m_FeitelijkOverstromingInZichtJaar; }
      }

      /// <summary>
      /// Gets the feitelijk overschrijding in zicht jaar.
      /// </summary>
      /// <value>The feitelijk overschrijding in zicht jaar.</value>
      public double FeitelijkOverschrijdingInZichtJaar
      {
         get { return m_FeitelijkOverschrijdingInZichtJaar; }
      }

      /// <summary>
      /// Optimize method.
      /// </summary>
      /// <value>Optimize method.</value>
      public OptimizeMethod OptimizeMethod
      {
         get { return m_OptimizationMethod; }
         set { m_OptimizationMethod = value; }
      }

      /// <summary>
      /// Investeringskosten eerste investering
      /// </summary>
      /// <value>Investeringskosten eerste investering</value>
      public DoubleVector VerdisconteerdeKostenEersteInvestering
      {
         get { return m_VerdisconteerdeKostenEersteInvestering; }
      }

      /// <summary>
      /// Gets the eerste investering A.
      /// </summary>
      /// <value>The eerste investering A.</value>
      public DoubleVector EersteInvestering
      {
         get { return m_EersteInvestering; }
      }

      /// <summary>
      /// Investeringskosten totaal
      /// </summary>
      /// <value>Investeringskosten totaal</value>
      public DoubleVector TotaleInvestering
      {
         get { return m_TotaleInvestering; }
      }

      /// <summary>
      /// Overstromingskansen
      /// </summary>
      /// <value>Overstromingskansen</value>
      public DoubleVector P
      {
         get { return m_P; }
      }

      /// <summary>
      /// Verwachte schade
      /// </summary>
      /// <value>Verwachte schade</value>
      public DoubleVector S
      {
         get { return m_S; }
      }

      /// <summary>
      /// Overstromingsschade
      /// </summary>
      /// <value>Overstromingsschade</value>
      public DoubleVector V
      {
         get { return m_V; }
      }

      /// <summary>
      /// Globale minimum
      /// </summary>
      /// <value>Globale minimum.</value>
      public double GlobalMinimum
      {
         get { return m_GlobalMinimum; }
      }

      /// <summary>
      /// Optimum
      /// </summary>
      /// <value>Optimum</value>
      public DoubleVector Minimum
      {
         get { return m_Minimum; }
      }

      #endregion Properties

      #region Member functions -------------------------------------------------

      /// <summary>
      /// Maak de initiele vector
      /// </summary>
      /// <returns></returns>
      public DoubleVector StartVector()
      {
         DoubleVector start = null;
         int index = 0;

         // Aantal dijkringtrajecten
         if (m_DijkringDeel.Trajecten.Count == 1)
         {
            start = new DoubleVector(m_DijkringDeel.Trajecten.Count * 4);
         }
         else if (m_DijkringDeel.Trajecten.Count > 1)
         {
            start = new DoubleVector((m_DijkringDeel.Trajecten.Count + 1) * 4);
            index = 4;
         }

         for (int i = index; i < start.Length; i = i + 4)
         {
            start[i] = m_Instellingen.ZichtJaar - m_BeginJaar + m_StartVectorT1;
            start[i + 1] = m_StartVectorT2;
            start[i + 2] = m_StartVectorU1;
            start[i + 3] = m_StartVectorU2;
         }

         return start;
      }

      /// <summary>
      /// Initialiseer het optimilisatie probleem
      /// </summary>
      /// <param name="beginJaar">Begin jaar.</param>
      /// <param name="schadeJaar">Jaartal waarvoor schade in de database gegeven zijn</param>
      /// <param name="z">Aantal tijdstappen</param>
      public void Initialize(int beginJaar, int schadeJaar, int z)
      {
         m_BeginJaar = beginJaar;
         m_Z = z;

         m_IndexZichtJaar = m_Instellingen.ZichtJaar - m_BeginJaar;
         m_JaartalZichtduur = m_Instellingen.ZichtJaar + m_Z;
         m_IndexJaartalZichtduur = m_JaartalZichtduur - m_BeginJaar;

         m_P = new DoubleVector(m_IndexJaartalZichtduur);
         m_S = new DoubleVector(m_IndexJaartalZichtduur);
         m_V = new DoubleVector(m_IndexJaartalZichtduur);

         m_VerdisconteerdeKostenEersteInvestering = new DoubleVector(m_DijkringDeel.Trajecten.Count);
         m_EersteInvestering = new DoubleVector(m_DijkringDeel.Trajecten.Count);
         m_TotaleInvestering = new DoubleVector(m_DijkringDeel.Trajecten.Count);

         // Overstromingsschade op tijdstip 0 (zonder verhoging)
         m_V0 = Function.V(m_Instellingen
            , m_DijkringDeel.Schade
            , m_DijkringDeel.KleinsteAlpha()
            , m_DijkringDeel.Nu
            , m_DijkringDeel.AantalInwoners
            , m_DijkringDeel.AantalSlachtoffers
            , m_DijkringDeel.AantalGewonden
            , m_DijkringDeel.Gamma
            , beginJaar - schadeJaar // TODO : ThisAppBerekeningen.Instance.BeginJaar - ThisAppBerekeningen.Instance.Schadejaar
            );
      }

      /// <summary>
      /// Los het optimilisatie probleem op
      /// </summary>
      /// <param name="startVector">The start vector</param>
      public void Solve(DoubleVector startVector)
      {
         m_Minimum = new DoubleVector();

         // Create a multivariable function.
         MultiVariableFunction multiVariableFunction = new MultiVariableFunction(new NMathFunctions.DoubleVectorDoubleFunction(ObjectFunction));

         switch (m_OptimizationMethod)
         {
            case OptimizeMethod.DownhillSimplex:
               DownhillSimplexMinimizer downhillSimplexMinimizer = new DownhillSimplexMinimizer();

               // Minimize function
               m_Minimum = downhillSimplexMinimizer.Minimize(multiVariableFunction, startVector);

               break;

            case OptimizeMethod.PowellsMethod:
               PowellMinimizer powellMinimizer = new PowellMinimizer();

               // Minimize function
               m_Minimum = powellMinimizer.Minimize(multiVariableFunction, startVector);

               break;

            case OptimizeMethod.SimulatedAnnealing:

               // Using simulated annealing, with 10 steps of 100 iterations each, a
               // starting temperature of 0, and a starting point of (0, 0), we sometimes find
               // the global minimum.
               AnnealingScheduleBase schedule = new LinearAnnealingSchedule(6, 100, 0);
               AnnealingMinimizer annealing = new AnnealingMinimizer(schedule);

               // Set keep history flag to true in order to get enough data to improve our annealing schedule.
               annealing.KeepHistory = true;

               int global = 0;
               int reps = 100;
               for (int i = 0; i < reps; i++)
               {
                  m_Minimum = annealing.Minimize(multiVariableFunction, startVector);
                  if (multiVariableFunction.Evaluate(m_Minimum) < -874)
                  {
                     global++;
                  }
               }

               break;
         }

         m_GlobalMinimum = multiVariableFunction.Evaluate(m_Minimum);

      }

      /// <summary>
      /// Objectfunction van het optimilisatie probleem
      /// </summary>
      /// <param name="strategie">vector met optimilisatie parameters</param>
      /// <returns>Totale kosten</returns>
      private double ObjectFunction(DoubleVector strategie)
      {
         double kosten = Kosten(false, strategie);

         return kosten;
      }

      /// <summary>
      /// Kosten functie voor het optimilisatie probleem
      /// </summary>
      /// <param name="debug">if set to <c>true</c> [debug].</param>
      /// <param name="strategie">vector met optimilisatie parameters</param>
      /// <returns>Totale kosten</returns>
      public double Kosten(bool debug, DoubleVector strategie)
      {
         // Strategie matrix:
         //
         // Het aantal rijen is bepaald door het aantal dijkringtrajecten
         // De kolommen zijn hieronder beschreven
         // Kolom 0 bevat het jaartal van de eerste verhoging</param>
         // Kolom 1 bevat de periode tussen verhogingen [jaren]</param>
         // Kolom 2 bevat de hoogte van de eerste verhoging [cm]</param>
         // Kolom 3 bevat de hoogte van de tweede en volgende verhoging [cm]</param>
         //

         DoubleMatrix strategieMatrix = new DoubleMatrix(m_DijkringDeel.Trajecten.Count, 4);

         if (m_DijkringDeel.Trajecten.Count == 1)
         {
            int index = 0;
            for (int row = 0; row < m_DijkringDeel.Trajecten.Count; row++)
            {
               for (int col = 0; col < 4; col++)
               {
                  strategieMatrix[row, col] = strategie[index++];
               }
            }
         }
         else if (m_DijkringDeel.Trajecten.Count > 1)
         {
            int index = 4;
            for (int row = 0; row < m_DijkringDeel.Trajecten.Count; row++)
            {
               for (int col = 0; col < 4; col++)
               {
                  strategieMatrix[row, col] = strategie[col] + strategie[index++];
               }
            }
         }

         // Initialiseer de kosten
         double kosten = 0.0;
         bool doorgaan = true;

         // Bepaal de kosten per dijkringtraject
         for (int row = 0; row < strategieMatrix.Rows; row++)
         {
            int tFirst = ConvertString.ToInt32(strategieMatrix[row, 0].ToString());
            int tSecond = ConvertString.ToInt32(strategieMatrix[row, 1].ToString());
            double uFirst = strategieMatrix[row, 2];
            double uSecond = strategieMatrix[row, 3];

            if ((tFirst < m_IndexZichtJaar) ||
                (tFirst >= m_IndexJaartalZichtduur) ||
                (tSecond < 1) ||
                (uFirst <= 0) ||
                (uSecond <= 0))
            {
               kosten = Double.MaxValue;
               doorgaan = false;
            }

            if (debug)
            {
               using (StreamWriter writer = new StreamWriter("optimize.txt", true))
               {
                  writer.WriteLine("Strategie:{0}={1},{2},{3},{4}"
                     , row
                     , strategieMatrix[row, 0] // tFirst
                     , strategieMatrix[row, 1] // tSecond
                     , strategieMatrix[row, 2] // uFirst
                     , strategieMatrix[row, 3] // uSecond
                     );
               }
            }

         }

         if (doorgaan)
         {
            // Schade + Investeringskosten
            kosten = Schade(debug, strategieMatrix) + Investeringskosten(debug, strategieMatrix);
         }

         if (debug)
         {
            using (StreamWriter writer = new StreamWriter("optimize.txt", true))
            {
               writer.WriteLine("Kosten:{0}", kosten);
            }
         }

         return kosten;
      }

      /// <summary>
      /// Bereken de verwachte schade per jaar
      /// </summary>
      /// <param name="debug">if set to <c>true</c> [debug].</param>
      /// <param name="strategieMatrix">The strategie matrix.</param>
      /// <returns></returns>
      private double Schade(bool debug, DoubleMatrix strategieMatrix)
      {
         double schade = 0.0;

         DoubleVector overstromingsKans = new DoubleVector(strategieMatrix.Rows);
         DoubleVector hoogtes = new DoubleVector(strategieMatrix.Rows);
         DoubleVector hFirst = new DoubleVector(strategieMatrix.Rows);
         DoubleVector feitelijkePOverstroming = new DoubleVector(strategieMatrix.Rows);
         DoubleVector feitelijkePOverschrijding = new DoubleVector(strategieMatrix.Rows);

         for (int t = 0; t < IndexJaartalZichtduur; t++)
         {
            for (int row = 0; row < strategieMatrix.Rows; row++)
            {
               int tFirst = ConvertString.ToInt32(strategieMatrix[row, 0].ToString());
               int tSecond = ConvertString.ToInt32(strategieMatrix[row, 1].ToString());
               double uFirst = strategieMatrix[row, 2];
               double uSecond = strategieMatrix[row, 3];

               DijkringTraject traject = (DijkringTraject)m_DijkringDeel.Trajecten[row];

               // Hoogtes
               hoogtes[row] = Function.Hoogte(t, traject.H0, tFirst, tSecond, uFirst, uSecond);
               hFirst[row] = traject.H0;

               // Overstromingskans
               overstromingsKans[row] = (Function.Pt(t, traject.Eta, hoogtes[row] - traject.H0, traject.AlphaOverstromingskans,
                 traject.POverstromingskansBeginJaar, m_Instellingen.FactorKans) * traject.Factor);

               if (t == m_IndexZichtJaar)
               {
                  feitelijkePOverstroming[row] = traject.FeitelijkeOverstromingskansInZichtjaar * traject.Factor;
                  feitelijkePOverschrijding[row] = traject.FeitelijkeOverschrijdingskansInZichtjaar * traject.Factor;
               }
            }

            // Feitelijke overstromingskans  in het zichtjaar
            m_FeitelijkOverstromingInZichtJaar = NMathFunctions.MaxValue(feitelijkePOverstroming);

            // Feitelijke overscrijdingskans  in het zichtjaar
            m_FeitelijkOverschrijdingInZichtJaar = NMathFunctions.MaxValue(feitelijkePOverschrijding);

            // Overstromingskans
            m_P[t] = NMathFunctions.MaxValue(overstromingsKans);

            // Overstromingsschade
            m_V[t] = Function.Vt(t, m_DijkringDeel.Nu, m_DijkringDeel.Gamma,
                                    m_DijkringDeel.Zeta, m_V0,
                                    NMathFunctions.MinValue(hoogtes) - NMathFunctions.MinValue(hFirst),
                                    m_Instellingen.FactorGroeiSchade);

            // Verwachte schade
            m_S[t] = m_P[t] * m_V[t];

            // Verwachte schade per jaar
            if (t >= m_IndexZichtJaar)
            {
               schade += m_S[t] * NMathFunctions.Exp(-Math.Log(1 + (m_Instellingen.DiscontovoetSchade / 100))
                 * (t + m_BeginJaar - m_Instellingen.ZichtJaar));

            }
         }

         schade += m_S[IndexJaartalZichtduur - 1] * NMathFunctions.Exp(-Math.Log(
           1 + (m_Instellingen.DiscontovoetSchade / 100))
           * (IndexJaartalZichtduur - 1 + m_BeginJaar - m_Instellingen.ZichtJaar))
           / Math.Log(1 + (m_Instellingen.DiscontovoetSchade / 100));

         return schade;
      }

      /// <summary>
      /// Bereken de investeringskosten
      /// </summary>
      /// <param name="debug">if set to <c>true</c> [debug].</param>
      /// <param name="strategieMatrix">The strategieMatrix.</param>
      /// <returns></returns>
      private double Investeringskosten(bool debug, DoubleMatrix strategieMatrix)
      {
         double investeringsKosten = 0.0;
         double deltaTilde = Math.Log(1 + m_Delta);

         for (int row = 0; row < strategieMatrix.Rows; row++)
         {
            int tFirst = ConvertString.ToInt32(strategieMatrix[row, 0].ToString());
            int tSecond = ConvertString.ToInt32(strategieMatrix[row, 1].ToString());
            double uFirst = strategieMatrix[row, 2];
            double uSecond = strategieMatrix[row, 3];

            DijkringTraject traject = (DijkringTraject)m_DijkringDeel.Trajecten[row];

            // Eerste investeringskosten
            m_VerdisconteerdeKostenEersteInvestering[row] = TrajectInvesteringskosten(traject, uFirst)
              * NMathFunctions.Exp(-deltaTilde * (tFirst + m_BeginJaar - m_Instellingen.ZichtJaar))
              * Function.ToeslagBeheerEnOnderhoud(traject.Omega, Delta);

            // Eerste investeringskostenA
            m_EersteInvestering[row] = TrajectInvesteringskosten(traject, uFirst)
              * Function.ToeslagBeheerEnOnderhoud(traject.Omega, Delta);

            // Initialiseer de totale kosten
            m_TotaleInvestering[row] = VerdisconteerdeKostenEersteInvestering[row];

            // Tweede en volgende investeringskosten
            int n = 1;
            while (tFirst + n * tSecond <= IndexJaartalZichtduur)
            {
               int tijdstip = tFirst + n * tSecond;
               m_TotaleInvestering[row] += TrajectInvesteringskosten(traject, uSecond)
                 * NMathFunctions.Exp(-deltaTilde * (tijdstip + m_BeginJaar - m_Instellingen.ZichtJaar))
                 * NMathFunctions.Exp(traject.lambda_exp * (uFirst + (n - 1) * uSecond))
                 * Function.ToeslagBeheerEnOnderhoud(traject.Omega, Delta);

               //if (debug)
               //{
               //  using (StreamWriter writer = new StreamWriter("optimize.txt", true))
               //  {
               //    writer.WriteLine("Investeringskosten {0} {1}={2}", traject.Naam , tFirst + n * tSecond, totaleInvestering[row]);
               //  }
               //}

               n++;
            }
            //// Slechts 2 investeringen
            //int tijdstip = tFirst + n * tSecond;
            //totaleInvestering[row] += TrajectInvesteringskosten(traject, uSecond)
            //  * NMathFunctions.Exp(-delta * (tijdstip + beginJaar - m_Instellingen.ZichtJaar))
            //  * NMathFunctions.Exp(traject.Lambda * uFirst);

            investeringsKosten += m_TotaleInvestering[row];
         }
         return investeringsKosten;
      }

      /// <summary>
      /// Investeringskosten behorende bij de optimale verhoging
      /// </summary>
      /// <param name="traject">The traject.</param>
      /// <param name="u">De hoogte van de optimale verhoging [cm]</param>
      /// <returns></returns>
      public double TrajectInvesteringskosten(DijkringTraject traject, double u)
      {
         return m_Instellingen.FactorKosten
           * Function.InvesteringsKosten(traject.lambda_exp, traject.b_exp, traject.C_exp, u);
      }

      #endregion Member functions ----------------------------------------------

      #region Test functions ------------------------------------------------

      /// <summary>
      /// Testfunctie
      /// </summary>
      /// <param name="tFirst">jaartal van de eerste verhoging</param>
      /// <param name="tSecond">de periode tussen verhogingen </param>
      /// <param name="uFirst">de hoogte van de eerste verhoging</param>
      /// <param name="uSecond">de hoogte van de tweede en volgende verhoging</param>
      /// <returns></returns>
      public double KostenStrategie(int tFirst, int tSecond, double uFirst, double uSecond)
      {
         DoubleMatrix strategieMatrix = new DoubleMatrix(1, 4);
         strategieMatrix[0, 0] = tFirst;
         strategieMatrix[0, 1] = tSecond;
         strategieMatrix[0, 2] = uFirst;
         strategieMatrix[0, 3] = uSecond;

         return Schade(false, strategieMatrix) + Investeringskosten(false, strategieMatrix);
      }

      #endregion Test functions ---------------------------------------------

   }
}
