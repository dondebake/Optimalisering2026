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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.Core/Instellingen.cs 5     28-04-09 13:55 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

using OptimaliseRing.General;
using OptimaliseRing.Profile;

namespace OptimaliseRing.Core
{

  /// <summary>
  /// Instellingen voor onzekerheid
  /// </summary>
  public class InstellingenOnzekerheid
  {
    #region Instance Variables -----------------------------------------------

    private int m_EconomischScenario;                        // Economische scenario
    private int m_KlimaatScenarioEnFysischMaxAfvoer;         // Klimaatscenario En Fysisch Max Afvoer:
    private double m_DiscontovoetSchade;                     // Discontovoet schade [%/jaar]
    private double m_DiscontovoetInvesteringen;              // Discontovoet investeringen [%/jaar]

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Discontovoet schade
    /// </summary>
    /// <value>Discontovoet schade</value>
    public double DiscontovoetSchade
    {
      get { return m_DiscontovoetSchade; }
      set { m_DiscontovoetSchade = value; }
    }

    /// <summary>
    /// Discontovoet investeringen
    /// </summary>
    /// <value>Discontovoet investeringen.</value>
    public double DiscontovoetInvesteringen
    {
      get { return m_DiscontovoetInvesteringen; }
      set { m_DiscontovoetInvesteringen = value; }
    }

    /// <summary>
    /// Economisch scenario.
    /// </summary>
    /// <value>Economisch scenario.</value>
    public int EconomischScenario
    {
      get { return m_EconomischScenario; }
      set { m_EconomischScenario = value; }
    }

    /// <summary>
    /// Gets or sets the klimaat scenario en fysisch max afvoer.
    /// </summary>
    /// <value>The klimaat scenario en fysisch max afvoer.</value>
    public int KlimaatScenarioEnFysischMaxAfvoer
    {
      get { return m_KlimaatScenarioEnFysischMaxAfvoer; }
      set { m_KlimaatScenarioEnFysischMaxAfvoer = value; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="InstellingenOnzekerheid"/> class.
    /// </summary>
    /// <param name="economischScenario">The economisch scenario.</param>
    /// <param name="klimaatScenarioEnFysischMaxAfvoer">The klimaat scenario en fysisch max afvoer.</param>
    /// <param name="discontovoetSchade">The discontovoet schade.</param>
    /// <param name="discontovoetInvesteringen">The discontovoet investeringen.</param>
    public InstellingenOnzekerheid(
      int economischScenario,
      int klimaatScenarioEnFysischMaxAfvoer,
      double discontovoetSchade,
      double discontovoetInvesteringen)
    {
      this.m_EconomischScenario = economischScenario;
      this.m_KlimaatScenarioEnFysischMaxAfvoer = klimaatScenarioEnFysischMaxAfvoer;
      this.m_DiscontovoetSchade = discontovoetSchade;
      this.m_DiscontovoetInvesteringen = discontovoetInvesteringen;
    }


    #endregion Constructors --------------------------------------------------
  }


  //=========
  public class InstellingenKeringen
  {
    #region Instance Variables -----------------------------------------------

    private int m_Percentage1;
    private int m_Percentage2;
    private string m_NaamKering;


    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    public int Percentage1
    {
      get { return m_Percentage1; }
      set { m_Percentage1 = value; }
    }

    public string NaamKering
    {
      get { return m_NaamKering; }
      set { m_NaamKering = value; }
    }

    public int Percentage2
    {
      get { return m_Percentage2; }
      set { m_Percentage2 = value; }
    }
    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="InstellingenOnzekerheid"/> class.
    /// </summary>
    /// <param name="economischScenario">The economisch scenario.</param>
    /// <param name="klimaatScenarioEnFysischMaxAfvoer">The klimaat scenario en fysisch max afvoer.</param>
    /// <param name="discontovoetSchade">The discontovoet schade.</param>
    /// <param name="discontovoetInvesteringen">The discontovoet investeringen.</param>
    public InstellingenKeringen(string naam, int percentage1, int percentage2)

    {
      this.m_Percentage1 = percentage1;
      this.m_Percentage2 = percentage2;
      this.m_NaamKering = naam;

    }

    #endregion Constructors --------------------------------------------------
  }


  //===========


  /// <summary>
  /// Instellingen van het programma
  /// </summary>
  public class Instellingen
  {
    #region Instance Variables -----------------------------------------------

    private int m_ZichtJaar;                                 // Zichtjaar
    private int m_OptimaleOverstromingskansenJaar;           // OptimaleOverstromingskansenJaar
    private double m_DiscontovoetSchade;                     // Discontovoet schade [%/jaar]
    private double m_DiscontovoetInvesteringen;              // Discontovoet investeringen [%/jaar]
    private int m_EconomischScenario;                        // Economische scenario
    //    1 = Regional Communities (RC)
    //    2 = Strong Europe (SE)
    //    3 = Transatlantic Market (TM)
    //    4 = Global Economy (GE)
    private int m_RamingVoorSlachtoffers;                    // Raming voor aantal slachtoffers
    //    1 = Laag
    //    2 = Gemiddeld
    //    3 = Hoog
    private int m_BedragPerInwoner;                          // Bedrag per inwoner [K€]
    private int m_BedragPerDodelijkSlachtoffer;              // Bedrag per dodelijk slachtoffer [K€]
    private int m_BedragPerGetroffene;                       // Bedrag per getroffene [K€]
    private double m_Aversiefactor;                          // Aversiefactor groepsrisico [-]
    private int m_ScenarioVoorHoeveelheidSchade;             // Scenario voor hoeveelheid schade
    //    1 = Laag
    //    2 = Gemiddeld
    //    3 = Hoog
    private int m_SchadeFunctie;                             // Schadefunctie
    //    1 = Onafhankelijk van de kans
    //    2 = Afhankelijk van de kans
    private double m_BeleidsfactorOverstromingsschade;       // Beleidsfactor overstromingsschade
    private double m_AanpassingsfactorOverstromingsschade;   // Aanpassingsfactor overstromingsschade
    private int m_DijkringspecifiekeFactorSchade;            // Dijkringspecifieke aanpassingsfactor schade
    //    1	Geen (factor = 1)
    //    2	KBA icm LIR
    //    3	KBA icm GR
    //    4	KBA icm LIR en GR
    //    5	KBA icm factor 10
    private int m_ParametersKostenfunctie;                   // Parameters kostenfunctie
    //    1 = Zonder overhoogte
    //    2 = Met overhoogte
    private double m_FactorKosten;                           // Vermenigvuldigingsfactor van de kosten
    private double m_FactorKans;                             // Vermenigvuldigingsfactor van de kans
    private double m_FactorGroeiSchade;                      // Vermenigvuldigingsfactor voor de groei van de schade
    private int m_KlimaatScenarioEnFysischMaxAfvoer;         // Klimaatscenario En Fysisch Max Afvoer:
    //    1	= Zonder aftoppen/	Without topping
    //    2	= Met aftoppen/	With topping
    //    3	= Gemiddeld zonder aftoppen/	Average without topping
    //    4	= Gemiddeld met hoog aftoppen/	Average with high topping
    //    5	= Gemiddeld met gemiddeld aftoppen/	Average with average topping
    //    6	= Gemiddeld met laag aftoppen/	Average with low topping
    //    7	= Gemiddeld+ zonder aftoppen/	Average+ without topping
    //    8	= Gemiddeld+ met hoog aftoppen/	Average+ with high topping
    //    9	= Gemiddeld+ met gemiddeld aftoppen/	Average+ with average topping
    //    10 = Gemiddeld+ met laag aftoppen/	Average+ with low topping
    //    11 = Warm zonder aftoppen/	Warm without topping
    //    12 = Warm met hoog aftoppen/	Warm with high topping
    //    13 = Warm met gemiddeld aftoppen/	Warm with average topping
    //    14 = Warm met laag aftoppen/	Warm with low topping
    //    15 = Warm+ zonder aftoppen/	Warm+ without topping
    //    16 = Warm+ met hoog aftoppen/	Warm+ with high topping
    //    17 = Warm+ met gemiddeld aftoppen/	Warm+ with average topping
    //    18 = Warm+ met laag aftoppen/	Warm+ with low topping
    private string m_Naam;                                   // Naam xml bestand met instellingen

    private int m_TypeKostenfunctie;                         // Type kostenfunctie
    //    1 = Exponentieel
    //    2 = Quadratisch

    private int m_Veiligheidsnorm;                          // Veiligheidsnormen
    //    1 = Zonder beperking
    //    2 = Met beperking

    private SortedList<string, List<Compartimenteringsdijk>> m_Compartimentering; // Compartimenteringsdelen

    private Profile.Profile m_Profile;
    private Profile.Profile m_Language;
    private OptimaliseRingDB m_OptimaliseRingDB;

    // Rekenen met parametersonzekerheid
    private bool m_Parametersonzekerheid;

    // Scenarioparameters
    private SortedList<int, InstellingenOnzekerheid> m_Scenarioparameters;

    // Keringenparameters
    private SortedList<int, InstellingenKeringen> m_Keringenparameters;


    #endregion Instance Variables --------------------------------------------

    #region Constructors -----------------------------------------------------
    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:Instellingen"/> class.
    /// </summary>
    public Instellingen(Profile.Ini profile, Profile.Ini language, OptimaliseRingDB optimaliseRingDB)
    {
      this.m_Profile = profile;
      this.m_Language = language;
      this.m_OptimaliseRingDB = optimaliseRingDB;
      this.m_Compartimentering = new SortedList<string, List<Compartimenteringsdijk>>();
      this.m_Naam = string.Empty;

      this.m_Scenarioparameters = new SortedList<int, InstellingenOnzekerheid>();
      this.m_Keringenparameters = new SortedList<int, InstellingenKeringen>();
    }


    public Instellingen()
    {
      this.m_Naam = string.Empty;
      this.m_Compartimentering = new SortedList<string, List<Compartimenteringsdijk>>();
      this.m_Scenarioparameters = new SortedList<int, InstellingenOnzekerheid>();
      this.m_Keringenparameters = new SortedList<int, InstellingenKeringen>();
    }

    #endregion Constructors --------------------------------------------------

    #region Properties -------------------------------------------------------


    /// <summary>
    /// Zichtjaar
    /// </summary>
    /// <value>Zichtjaar</value>
    public int ZichtJaar
    {
      get { return m_ZichtJaar; }
      set { m_ZichtJaar = value; }
    }

    /// <summary>
    /// Gets or sets the optimale overstromingskansen jaar.
    /// </summary>
    /// <value>The optimale overstromingskansen jaar.</value>
    public int OptimaleOverstromingskansenJaar
    {
      get { return m_OptimaleOverstromingskansenJaar; }
      set { m_OptimaleOverstromingskansenJaar = value; }
    }

    /// <summary>
    /// Naam.
    /// </summary>
    /// <value>Naam.</value>
    public string Naam
    {
      get { return m_Naam; }
      set { m_Naam = value; }
    }

    /// <summary>
    /// Gets or sets the compartimentering.
    /// </summary>
    /// <value>The compartimentering.</value>
    public SortedList<string, List<Compartimenteringsdijk>> Compartimentering
    {
      get { return m_Compartimentering; }
      //set { m_Compartimentering = value; }
    }

    /// <summary>
    /// Vermenigvuldigingsfactor van de kosten
    /// </summary>
    /// <value>Vermenigvuldigingsfactor van de kosten</value>
    public double FactorKosten
    {
      get { return m_FactorKosten; }
      set { m_FactorKosten = value; }
    }

    /// <summary>
    /// Vermenigvuldigingsfactor van de kans
    /// </summary>
    /// <value>Vermenigvuldigingsfactor van de kans</value>
    public double FactorKans
    {
      get { return m_FactorKans; }
      set { m_FactorKans = value; }
    }

    /// <summary>
    /// Gets or sets the factor groei schade.
    /// </summary>
    /// <value>The factor groei schade.</value>
    public double FactorGroeiSchade
    {
      get { return m_FactorGroeiSchade; }
      set { m_FactorGroeiSchade = value; }
    }

    /// <summary>
    /// Schadefunctie
    /// </summary>
    /// <value>Schadefunctie</value>
    public int SchadeFunctie
    {
      get { return m_SchadeFunctie; }
      set { m_SchadeFunctie = value; }
    }

    /// <summary>
    /// Raming voor aantal slachtoffers
    /// </summary>
    /// <value>Raming voor aantal slachtoffers</value>
    public int RamingVoorSlachtoffers
    {
      get { return m_RamingVoorSlachtoffers; }
      set { m_RamingVoorSlachtoffers = value; }
    }

    /// <summary>
    /// Discontovoet schade
    /// </summary>
    /// <value>Discontovoet schade</value>
    public double DiscontovoetSchade
    {
      get { return m_DiscontovoetSchade; }
      set { m_DiscontovoetSchade = value; }
    }

    /// <summary>
    /// Discontovoet investeringen
    /// </summary>
    /// <value>Discontovoet investeringen.</value>
    public double DiscontovoetInvesteringen
    {
      get { return m_DiscontovoetInvesteringen; }
      set { m_DiscontovoetInvesteringen = value; }
    }

    /// <summary>
    /// Economisch scenario.
    /// </summary>
    /// <value>Economisch scenario.</value>
    public int EconomischScenario
    {
      get { return m_EconomischScenario; }
      set { m_EconomischScenario = value; }
    }

    /// <summary>
    /// Bedrag per inwoner.
    /// </summary>
    /// <value>Bedrag per inwoner.</value>
    public int BedragPerInwoner
    {
      get { return m_BedragPerInwoner; }
      set { m_BedragPerInwoner = value; }
    }

    /// <summary>
    /// Bedrag per dodelijk slachtoffer.
    /// </summary>
    /// <value>Bedrag per dodelijk slachtoffer.</value>
    public int BedragPerDodelijkSlachtoffer
    {
      get { return m_BedragPerDodelijkSlachtoffer; }
      set { m_BedragPerDodelijkSlachtoffer = value; }
    }

    /// <summary>
    /// Bedrag per getroffene.
    /// </summary>
    /// <value>Bedrag per getroffene.</value>
    public int BedragPerGetroffene
    {
      get { return m_BedragPerGetroffene; }
      set { m_BedragPerGetroffene = value; }
    }

    /// <summary>
    /// Aversiefactor groepsrisico.
    /// </summary>
    /// <value>Aversiefactor.</value>
    public double Aversiefactor
    {
      get { return m_Aversiefactor; }
      set { m_Aversiefactor = value; }
    }

    /// <summary>
    /// Beleidsfactor overstromingsschade.
    /// </summary>
    /// <value>Beleidsfactor overstromingsschade.</value>
    public double BeleidsfactorOverstromingsschade
    {
      get { return m_BeleidsfactorOverstromingsschade; }
      set { m_BeleidsfactorOverstromingsschade = value; }
    }

    /// <summary>
    /// Aanpassingsfactor overstromingsschade.
    /// </summary>
    /// <value>Aanpassingsfactor overstromingsschade.</value>
    public double AanpassingsfactorOverstromingsschade
    {
      get { return m_AanpassingsfactorOverstromingsschade; }
      set { m_AanpassingsfactorOverstromingsschade = value; }
    }

    /// <summary>
    /// Gets or sets Dijkringspecifieke aanpassingsfactor schade.
    /// </summary>
    /// <value>Dijkringspecifieke aanpassingsfactor schade.</value>
    public int DijkringspecifiekeFactorSchade
    {
      get { return m_DijkringspecifiekeFactorSchade; }
      set { m_DijkringspecifiekeFactorSchade = value; }
    }

    /// <summary>
    /// Parameters kostenfunctie.
    /// </summary>
    /// <value>Parameters kostenfunctie.</value>
    public int ParametersKostenfunctie
    {
      get { return m_ParametersKostenfunctie; }
      set { m_ParametersKostenfunctie = value; }
    }

    /// <summary>
    /// Scenario voor hoeveelheid schade.
    /// </summary>
    /// <value>Scenario voor hoeveelheid schade.</value>
    public int ScenarioVoorHoeveelheidSchade
    {
      get { return m_ScenarioVoorHoeveelheidSchade; }
      set { m_ScenarioVoorHoeveelheidSchade = value; }
    }

    /// <summary>
    /// Gets or sets the klimaat scenario en fysisch max afvoer.
    /// </summary>
    /// <value>The klimaat scenario en fysisch max afvoer.</value>
    public int KlimaatScenarioEnFysischMaxAfvoer
    {
      get { return m_KlimaatScenarioEnFysischMaxAfvoer; }
      set { m_KlimaatScenarioEnFysischMaxAfvoer = value; }
    }

    /// <summary>
    /// Gets or sets Type kostenfunctie
    /// </summary>
    /// <value>The type kostenfunctie.</value>
    public int TypeKostenfunctie
    {
      get { return m_TypeKostenfunctie; }
      set { m_TypeKostenfunctie = value; }
    }

    /// <summary>
    /// Gets or sets Veiligheidsnorm
    /// </summary>
    /// <value>The veiligheidsnorm.</value>
    public int Veiligheidsnorm
    {
      get { return m_Veiligheidsnorm; }
      set { m_Veiligheidsnorm = value; }
    }

    /// <summary>
    /// Gets or sets Rekenen met parametersonzekerheid
    /// </summary>
    /// <value>Rekenen met parametersonzekerheid</value>
    public bool Parametersonzekerheid
    {
      get { return m_Parametersonzekerheid; }
      set { m_Parametersonzekerheid = value; }
    }

    /// <summary>
    /// Gets the scenarioparameters.
    /// </summary>
    /// <value>The scenarioparameters.</value>
    public SortedList<int, InstellingenOnzekerheid> Scenarioparameters
    {
      get { return m_Scenarioparameters; }
    }

    public SortedList<int, InstellingenKeringen> Keringenparameters
    {
      get { return m_Keringenparameters; }
    }

    #endregion Properties ----------------------------------------------------

    #region Member functions -------------------------------------------------

    /// <summary>
    /// Laad de instellingen van het initialisatie bestand
    /// </summary>
    public void Load()
    {
      ZichtJaar = ConvertString.ToInt32(m_Profile.GetValue("Instellingen", "ZichtJaar", "2015"));
      OptimaleOverstromingskansenJaar = ConvertString.ToInt32(m_Profile.GetValue("Instellingen", "OptimaleOverstromingskansenJaar", ZichtJaar.ToString()));
      DiscontovoetSchade = ConvertString.ToDouble(m_Profile.GetValue("Instellingen", "DiscontovoetSchade", "4"));
      DiscontovoetInvesteringen = ConvertString.ToDouble(m_Profile.GetValue("Instellingen", "DiscontovoetInvesteringen", "4"));
      EconomischScenario = ConvertString.ToInt32(m_Profile.GetValue("Instellingen", "EconomischScenario", "1"));
      BedragPerInwoner = ConvertString.ToInt32(m_Profile.GetValue("Instellingen", "BedragPerInwoner", "0"));
      BedragPerDodelijkSlachtoffer = ConvertString.ToInt32(m_Profile.GetValue("Instellingen", "BedragPerDodelijkSlachtoffer", "0"));
      BedragPerGetroffene = ConvertString.ToInt32(m_Profile.GetValue("Instellingen", "BedragPerGetroffene", "0"));
      Aversiefactor = ConvertString.ToDouble(m_Profile.GetValue("Instellingen", "Aversiefactor", "1"));
      BeleidsfactorOverstromingsschade = ConvertString.ToDouble(m_Profile.GetValue("Instellingen", "BeleidsfactorOverstromingsschade", "1"));
      AanpassingsfactorOverstromingsschade = ConvertString.ToDouble(m_Profile.GetValue("Instellingen", "AanpassingsfactorOverstromingsschade", "1"));
      DijkringspecifiekeFactorSchade = ConvertString.ToInt32(m_Profile.GetValue("Instellingen", "DijkringspecifiekeFactorSchade", "1"));
      ParametersKostenfunctie = ConvertString.ToInt32(m_Profile.GetValue("Instellingen", "ParametersKostenfunctie", "1"));
      ScenarioVoorHoeveelheidSchade = ConvertString.ToInt32(m_Profile.GetValue("Instellingen", "ScenarioVoorHoeveelheidSchade", "1"));
      KlimaatScenarioEnFysischMaxAfvoer = ConvertString.ToInt32(m_Profile.GetValue("Instellingen", "KlimaatScenarioEnFysischMaxAfvoer", "1"));
      FactorKans = ConvertString.ToDouble(m_Profile.GetValue("Instellingen", "FactorKans", "1"));
      FactorKosten = ConvertString.ToDouble(m_Profile.GetValue("Instellingen", "FactorKosten", "1"));
      FactorGroeiSchade = ConvertString.ToDouble(m_Profile.GetValue("Instellingen", "FactorGroeiSchade", "1"));
      RamingVoorSlachtoffers = ConvertString.ToInt32(m_Profile.GetValue("Instellingen", "RamingVoorSlachtoffers", "1"));
      SchadeFunctie = ConvertString.ToInt32(m_Profile.GetValue("Instellingen", "SchadeFunctie", "1"));

      TypeKostenfunctie = ConvertString.ToInt32(m_Profile.GetValue("Instellingen", "TypeKostenfunctie", "1"));
      Veiligheidsnorm = ConvertString.ToInt32(m_Profile.GetValue("Instellingen", "Veiligheidsnorm", "1"));

      this.m_Parametersonzekerheid = m_Profile.GetValue("Instellingen", "Parametersonzekerheid", "0") == "1" ? true : false;

      // Scenarioparameters
      int aantalParameterscenarios = ConvertString.ToInt32(m_Profile.GetValue("Instellingen", "AantalParameterscenarios", "0"));
      this.m_Scenarioparameters.Clear();

      for (int teller = 0; teller < aantalParameterscenarios; teller++)
      {
        string scenario = m_Profile.GetValue("Instellingen", "Parameterscenario" + teller.ToString(), "-");
        if (scenario != "-")
        {
          string[] token = scenario.Split(';');
          if (token.Length == 4)
          {
            int economischScenario = ConvertString.ToInt32(token[0]);
            int klimaatScenarioEnFysischMaxAfvoer = ConvertString.ToInt32(token[1]);
            double discontovoetSchade = ConvertString.ToDouble(token[2]);
            double discontovoetInvesteringen = ConvertString.ToDouble(token[3]);

            this.m_Scenarioparameters.Add(teller, new InstellingenOnzekerheid(
              economischScenario, klimaatScenarioEnFysischMaxAfvoer, discontovoetSchade, discontovoetInvesteringen));
          }
          else
          {
            this.m_Scenarioparameters.Add(teller, new InstellingenOnzekerheid(
              this.m_EconomischScenario, m_KlimaatScenarioEnFysischMaxAfvoer, m_DiscontovoetSchade, m_DiscontovoetInvesteringen));
          }
        }
      }
    }

    public void LoadKeringPercentages()
    {
      this.m_Keringenparameters.Clear();
      //haal keringen uit de database
      SortedList keringen = m_OptimaliseRingDB.GetDataset("SELECT [B-Kering] FROM [B-keringen] ORDER BY [B-Kering]").ToSortedList();

      int teller = 1;
      foreach (SortedList kering in keringen.Values)
      {
        string inivalue = kering.GetByIndex(0) + ":" + "1";
        int perc1 = ConvertString.ToInt32(m_Profile.GetValue("B-keringen", inivalue, "0"));
        inivalue = kering.GetByIndex(0) + ":" + "2";
        int perc2 = ConvertString.ToInt32(m_Profile.GetValue("B-keringen", inivalue, "0"));

        this.m_Keringenparameters.Add(teller, new InstellingenKeringen(kering.GetByIndex(0).ToString(),perc1, perc2));
        teller+=1;
      }

     // keringpercentage = ConvertString.ToInt32(m_Profile.GetValue("B-keringen", "6:1", "0"));
    }


    public void SaveKeringPercentages()
    {
      //haal keringen uit de database
      SortedList keringen = m_OptimaliseRingDB.GetDataset("SELECT [B-Kering] FROM [B-keringen] ORDER BY [B-Kering]").ToSortedList();

      //foreach (InstellingenKeringen instellingenKeringen in this.m_Keringenparameters.Values[int])

        int teller = 0;
        foreach (SortedList kering in keringen.Values)
        {
          InstellingenKeringen instellingenKeringen = this.m_Keringenparameters.Values[teller];

          string inivalue = kering.GetByIndex(0) + ":" + "1";

          m_Profile.SetValue("B-keringen", inivalue, instellingenKeringen.Percentage1);

          inivalue = kering.GetByIndex(0) + ":" + "2";
          m_Profile.SetValue("B-keringen", inivalue, instellingenKeringen.Percentage2);

          teller += 1;
        }

    }


    /// <summary>
    /// Bewaar de instellingen in het initialisatie bestand
    /// </summary>
    public void Save()
    {
      m_Profile.SetValue("Instellingen", "ZichtJaar", ZichtJaar.ToString());
      m_Profile.SetValue("Instellingen", "OptimaleOverstromingskansenJaar", OptimaleOverstromingskansenJaar.ToString());
      m_Profile.SetValue("Instellingen", "DiscontovoetSchade", DiscontovoetSchade.ToString());
      m_Profile.SetValue("Instellingen", "DiscontovoetInvesteringen", DiscontovoetInvesteringen.ToString());
      m_Profile.SetValue("Instellingen", "EconomischScenario", EconomischScenario.ToString());
      m_Profile.SetValue("Instellingen", "BedragPerInwoner", BedragPerInwoner.ToString());
      m_Profile.SetValue("Instellingen", "BedragPerDodelijkSlachtoffer", BedragPerDodelijkSlachtoffer.ToString());
      m_Profile.SetValue("Instellingen", "BedragPerGetroffene", BedragPerGetroffene.ToString());
      m_Profile.SetValue("Instellingen", "Aversiefactor", Aversiefactor.ToString());
      m_Profile.SetValue("Instellingen", "BeleidsfactorOverstromingsschade", BeleidsfactorOverstromingsschade.ToString());
      m_Profile.SetValue("Instellingen", "AanpassingsfactorOverstromingsschade", AanpassingsfactorOverstromingsschade.ToString());
      m_Profile.SetValue("Instellingen", "DijkringspecifiekeFactorSchade", DijkringspecifiekeFactorSchade.ToString());
      m_Profile.SetValue("Instellingen", "ParametersKostenfunctie", ParametersKostenfunctie.ToString());
      m_Profile.SetValue("Instellingen", "ScenarioVoorHoeveelheidSchade", ScenarioVoorHoeveelheidSchade.ToString());
      m_Profile.SetValue("Instellingen", "KlimaatScenarioEnFysischMaxAfvoer", KlimaatScenarioEnFysischMaxAfvoer.ToString());
      m_Profile.SetValue("Instellingen", "FactorKans", FactorKans.ToString());
      m_Profile.SetValue("Instellingen", "FactorKosten", FactorKosten.ToString());
      m_Profile.SetValue("Instellingen", "FactorGroeiSchade", FactorGroeiSchade.ToString());
      m_Profile.SetValue("Instellingen", "RamingVoorSlachtoffers", RamingVoorSlachtoffers.ToString());
      m_Profile.SetValue("Instellingen", "SchadeFunctie", SchadeFunctie.ToString());

      m_Profile.SetValue("Instellingen", "TypeKostenfunctie", TypeKostenfunctie.ToString());
      m_Profile.SetValue("Instellingen", "Veiligheidsnorm", Veiligheidsnorm.ToString());

      m_Profile.SetValue("Instellingen", "Parametersonzekerheid", this.m_Parametersonzekerheid ? "1" : "0");
      m_Profile.SetValue("Instellingen", "AantalParameterscenarios", (this.m_Scenarioparameters.Count).ToString());

      int teller = 0;
      foreach (InstellingenOnzekerheid instellingenOnzekerheid in this.m_Scenarioparameters.Values)
      {
        m_Profile.SetValue("Instellingen", "Parameterscenario" + teller.ToString()
          , string.Format("{0};{1};{2};{3}", instellingenOnzekerheid.EconomischScenario
          , instellingenOnzekerheid.KlimaatScenarioEnFysischMaxAfvoer
          , instellingenOnzekerheid.DiscontovoetSchade
          , instellingenOnzekerheid.DiscontovoetInvesteringen));
        teller++;
      }
    }

    /// <summary>
    /// Schrijf de huidige instellingen in het overzichtsbestand
    /// </summary>
    /// <param name="writer">Uitvoer bestand</param>
    public void Overzichtsbestand(StreamWriter writer, string dbNaam)
    {
      CultureInfo cultureInfo = new CultureInfo(m_Profile.GetValue("OptimaliseRing", "Taal", "en-GB"));

      writer.WriteLine(string.Format("{0}:", m_Language.GetValue("Captions:" + cultureInfo.Name, "Instellingen")));
      writer.WriteLine("");
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IZichtjaar").ToString().Replace("\\t", "\t"), ZichtJaar, "[-]"));
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IOptimaleOverstromingskansenJaar").ToString().Replace("\\t", "\t"), OptimaleOverstromingskansenJaar, "[-]"));

      // Alleen afdrukken als er niet met parameteronzekerheid wordt gerekend.
      if (!this.m_Parametersonzekerheid)
      {
        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IDiscontovoetSchade").ToString().Replace("\\t", "\t"), DiscontovoetSchade, "[%/" + m_Language.GetValue("Captions:" + cultureInfo.Name, "Jaar").ToString() + "]"));
        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IDiscontovoetInvesteringen").ToString().Replace("\\t", "\t"), DiscontovoetInvesteringen, "[%/" + m_Language.GetValue("Captions:" + cultureInfo.Name, "Jaar").ToString() + "]"));
        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IEconomischScenario").ToString().Replace("\\t", "\t"), LeesNaamDatabase("EconomischScenario", "ID", EconomischScenario)));
      }

      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IRamingSlachtoffers").ToString().Replace("\\t", "\t"), LeesNaamDatabase("RamingVoorSlachtoffers", "ID", RamingVoorSlachtoffers)));
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IBedragPerinwoner").ToString().Replace("\\t", "\t"), BedragPerInwoner, "[K€]"));
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IBedragPerSlachtoffer").ToString().Replace("\\t", "\t"), BedragPerDodelijkSlachtoffer, "[K€]"));
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IBedragPerGetroffene").ToString().Replace("\\t", "\t"), BedragPerGetroffene, "[K€]"));
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IAversiefactor").ToString().Replace("\\t", "\t"), Aversiefactor, "[-]"));
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IScenarioSchade").ToString().Replace("\\t", "\t"), LeesNaamDatabase("ScenarioVoorHoeveelheidSchade", "ID", ScenarioVoorHoeveelheidSchade)));
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "ISchadeFunctie").ToString().Replace("\\t", "\t"), LeesNaamDatabase("SchadeFunctie", "ID", SchadeFunctie)));
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IBeleidsfactor").ToString().Replace("\\t", "\t"), BeleidsfactorOverstromingsschade, "[-]"));
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IAanpassingsfactor").ToString().Replace("\\t", "\t"), AanpassingsfactorOverstromingsschade, "[-]"));
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IDijkringspecifiekeFactorSchade").ToString().Replace("\\t", "\t"), LeesNaamDatabase("DijkringspecifiekeFactorSchade", "ID", DijkringspecifiekeFactorSchade)));
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "ITypeKostenfunctie").ToString().Replace("\\t", "\t"), LeesNaamDatabase("TypeKostenfunctie", "ID", TypeKostenfunctie)));
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IKostenfunctie").ToString().Replace("\\t", "\t"), LeesNaamDatabase("ParametersKostenfunctie", "ID", ParametersKostenfunctie)));
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IFactorKosten").ToString().Replace("\\t", "\t"), FactorKosten, "[-]"));
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IFactorKans").ToString().Replace("\\t", "\t"), FactorKans, "[-]"));
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IFactorGroei").ToString().Replace("\\t", "\t"), FactorGroeiSchade, "[-]"));

      // Alleen afdrukken als er niet met parameteronzekerheid wordt gerekend.
      if (!this.m_Parametersonzekerheid)
      {
        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IKlimaatScenarioEnFysischMaxAfvoer").ToString().Replace("\\t", "\t"), LeesNaamDatabase("Klimaat_AftoppenAfvoer", "Id", KlimaatScenarioEnFysischMaxAfvoer)));
      }

      string veiligheidsnormTekst = m_Language.GetValue("Captions:" + cultureInfo.Name
        , string.Format("UVeiligheidsnormKeuze{0}", Veiligheidsnorm)
        , string.Format("UVeiligheidsnormKeuze{0}", Veiligheidsnorm));

      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IVeiligheidsnorm").ToString().Replace("\\t", "\t"), veiligheidsnormTekst));

      writer.WriteLine("");

      // Blok parameteronzekerheid

      string key = this.m_Parametersonzekerheid ? "IParametersonzekerheidJa" : "IParametersonzekerheidNee";

      //IParametersonzekerheid1=Er is{0} gerekend met parameteronzekerheid.
      writer.WriteLine(string.Format(this.m_Language.GetValue("Captions:" + cultureInfo.Name
        , key).ToString().Replace("\\t", "\t")));

      if (this.m_Parametersonzekerheid)
      {
        //IParametersonzekerheid2=Voor de parameteronzekerheid zijn {0} scenario's gedefinieerd:
        writer.WriteLine(string.Format(this.m_Language.GetValue("Captions:" + cultureInfo.Name
          , "IParametersonzekerheid2").ToString().Replace("\\t", "\t"), this.m_Scenarioparameters.Count.ToString()));

        writer.WriteLine("");

        // Header
        string scenario = string.Empty;
        string economischScenario = this.m_Language.GetValue("Captions:" + cultureInfo.Name, "EconomischScenario").ToString();
        string klimaatscenarioEnFysischMaximumAfvoer = this.m_Language.GetValue("Captions:" + cultureInfo.Name, "KlimaatscenarioEnFysischMaximumAfvoer").ToString();
        string discontovoetSchade = this.m_Language.GetValue("Captions:" + cultureInfo.Name, "DiscontovoetSchade").ToString();
        string discontovoetInvesteringen = this.m_Language.GetValue("Captions:" + cultureInfo.Name, "DiscontovoetInvesteringen").ToString();

        //IParametersonzekerheidRegel={0,-10}{1,-30}{2,-30}{3,-12:F1}{4,-12:F1}
        writer.WriteLine(string.Format(this.m_Language.GetValue("Captions:" + cultureInfo.Name
          , "IParametersonzekerheidRegel").ToString().Replace("\\t", "\t")
          , scenario, economischScenario, klimaatscenarioEnFysischMaximumAfvoer, discontovoetSchade, discontovoetInvesteringen));

        // Eenheid
        scenario = string.Empty;
        economischScenario = string.Empty;
        klimaatscenarioEnFysischMaximumAfvoer = string.Empty;

        string jaarText = this.m_Language.GetValue("Captions:" + cultureInfo.Name, "Jaar").ToString();
        discontovoetSchade = "[%/" + jaarText + "]";
        discontovoetInvesteringen = "[%/" + jaarText + "]";

        //IParametersonzekerheidRegel={0,-10}{1,-30}{2,-30}{3,-12:F1}{4,-12:F1}
        writer.WriteLine(string.Format(this.m_Language.GetValue("Captions:" + cultureInfo.Name
          , "IParametersonzekerheidRegel").ToString().Replace("\\t", "\t")
          , scenario, economischScenario, klimaatscenarioEnFysischMaximumAfvoer, discontovoetSchade, discontovoetInvesteringen));


        foreach (KeyValuePair<int, InstellingenOnzekerheid> instellingenOnzekerheid in this.m_Scenarioparameters)
        {
          scenario = (instellingenOnzekerheid.Key + 1).ToString();
          economischScenario = LeesNaamDatabase("EconomischScenario", "ID", instellingenOnzekerheid.Value.EconomischScenario);
          klimaatscenarioEnFysischMaximumAfvoer = LeesNaamDatabase("Klimaat_AftoppenAfvoer", "Id", instellingenOnzekerheid.Value.KlimaatScenarioEnFysischMaxAfvoer);
          discontovoetSchade = instellingenOnzekerheid.Value.DiscontovoetSchade.ToString("F2");
          discontovoetInvesteringen = instellingenOnzekerheid.Value.DiscontovoetInvesteringen.ToString("F2");

          //IParametersonzekerheidRegel={0,-10}{1,-30}{2,-30}{3,-12:F1}{4,-12:F1}
          writer.WriteLine(string.Format(this.m_Language.GetValue("Captions:" + cultureInfo.Name
            , "IParametersonzekerheidRegel").ToString().Replace("\\t", "\t")
            , scenario, economischScenario, klimaatscenarioEnFysischMaximumAfvoer, discontovoetSchade, discontovoetInvesteringen));
        }
      }

      writer.WriteLine("");

      FileInfo dbFile = new FileInfo(dbNaam);
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IDatabase").ToString().Replace("\\t", "\t"), dbFile.FullName, "[-]"));
      writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + cultureInfo.Name, "IDatumTijd").ToString().Replace("\\t", "\t"), dbFile.LastWriteTime.ToString("dd-MM-yyyy HH:mm:ss"), "[-]"));
      writer.WriteLine("");

    }

    /// <summary>
    /// Lees de naam van de huidige instelling uit de database
    /// </summary>
    /// <param name="tabel">Database tabelnaam</param>
    /// <param name="id">Database identificatie</param>
    /// <param name="value">instelling</param>
    /// <returns>Naam instelling</returns>
    private string LeesNaamDatabase(string tabel, string id, int value)
    {
      string naam = "";
      SortedList records = m_OptimaliseRingDB.GetDataset("SELECT * FROM " + tabel + " WHERE " + id + " = " + value.ToString() + ";").ToSortedList();
      if (records.Count == 1)
      {
        string veldNaam = "Naam";

        CultureInfo cultureInfo = new CultureInfo(m_Profile.GetValue("OptimaliseRing", "Taal", "en-GB"));
        if (cultureInfo.Name == "en-GB") veldNaam = "Name";

        SortedList record = (SortedList)records.GetByIndex(0);
        if (record[veldNaam].ToString().Length > 0)
        {
          naam = record[veldNaam].ToString();
        }
      }
      return naam;
    }

    /// <summary>
    /// Lees de instellingen
    /// </summary>
    /// <param name="naam">Naam xml bestand met instellingen </param>
    public void Read(string naam)
    {
      string currentElement = "";
      this.m_Compartimentering.Clear();
      this.m_Scenarioparameters.Clear();
      this.m_Keringenparameters.Clear();

      if (File.Exists(naam))
      {
        m_Naam = naam;

        XmlTextReader xmlreader = new XmlTextReader(m_Naam);
        while (xmlreader.Read())
        {
          // parse based on NodeType
          if (xmlreader.NodeType == XmlNodeType.Element)
          {
            currentElement = xmlreader.Name;
            if (currentElement == "Compartimentering")
            {
              XmlReader xmlreaderA = xmlreader.ReadSubtree();

              while (xmlreaderA.Read())
              {
                if (xmlreaderA.NodeType == XmlNodeType.Element)
                {
                  string name = xmlreader.Name;
                  if (name.Substring(0, "Dijkring".Length) == "Dijkring")
                  {
                    if (xmlreaderA.GetAttribute("Dijkring") != null)
                    {

                      string dijkring = xmlreaderA.GetAttribute("Dijkring");
                      List<Compartimenteringsdijk> compartimenten = new List<Compartimenteringsdijk>();

                      XmlReader xmlreaderB = xmlreaderA.ReadSubtree();
                      while (xmlreaderB.Read())
                      {
                        if (xmlreaderB.NodeType == XmlNodeType.Element)
                        {

                          if (xmlreaderB.Name.Substring(0, "Dijkdeel".Length) == "Dijkdeel")
                          {
                            Compartimenteringsdijk compartimenteringsdijk = new Compartimenteringsdijk();
                            compartimenteringsdijk.DijkId = xmlreaderB.GetAttribute("DijkId");
                            compartimenteringsdijk.Dijkdeel = xmlreaderB.GetAttribute("DeelNr");
                            compartimenten.Add(compartimenteringsdijk);
                          }
                        }
                      }
                      Compartimentering.Add(dijkring, compartimenten);
                    }
                  }
                }
              }
            }
            if (currentElement == "ParametersOnzekerheid")
            {
              XmlReader xmlreaderParametersOnzekerheid = xmlreader.ReadSubtree();
              if (xmlreaderParametersOnzekerheid.Read())
              {
                if (xmlreaderParametersOnzekerheid.Name == "ParametersOnzekerheid")
                {
                  int teller = 0;
                  while (xmlreaderParametersOnzekerheid.Read())
                  {
                    if (xmlreaderParametersOnzekerheid.Name.Length > "Scenario".Length && xmlreaderParametersOnzekerheid.Name.Substring(0, "Scenario".Length) == "Scenario")
                    {
                      string economischScenarioString = xmlreaderParametersOnzekerheid.GetAttribute("EconomischScenario");
                      string klimaatScenarioEnFysischMaxAfvoerString = xmlreaderParametersOnzekerheid.GetAttribute("KlimaatScenarioEnFysischMaxAfvoer");
                      string discontovoetSchadeString = xmlreaderParametersOnzekerheid.GetAttribute("DiscontovoetSchade");
                      string discontovoetInvesteringenString = xmlreaderParametersOnzekerheid.GetAttribute("DiscontovoetInvesteringen");

                      int economischScenario = ConvertString.ToInt32(economischScenarioString);
                      int klimaatScenarioEnFysischMaxAfvoer = ConvertString.ToInt32(klimaatScenarioEnFysischMaxAfvoerString);
                      double discontovoetSchade = ConvertString.ToDouble(discontovoetSchadeString);
                      double discontovoetInvesteringen = ConvertString.ToDouble(discontovoetInvesteringenString);

                      this.m_Scenarioparameters.Add(teller++, new InstellingenOnzekerheid(
                        economischScenario, klimaatScenarioEnFysischMaxAfvoer, discontovoetSchade, discontovoetInvesteringen));
                    }
                  }
                  //<ParametersOnzekerheid>
                  //  <Scenario0 EconomischScenario="1" KlimaatScenarioEnFysischMaxAfvoer="5" DiscontovoetSchade="6,0" DiscontovoetInvesteringen="88,0" />
                  //  <Scenario1 EconomischScenario="4" KlimaatScenarioEnFysischMaxAfvoer="1" DiscontovoetSchade="3,0" DiscontovoetInvesteringen="12,0" />
                  //</ParametersOnzekerheid>
                }
              }
            }

            if (currentElement == "B-Keringen")
            {
              XmlReader xmlreaderKeringen = xmlreader.ReadSubtree();
              if (xmlreaderKeringen.Read())
              {
                  int teller = 0;
                  while (xmlreaderKeringen.Read())
                  {
                    if (xmlreaderKeringen.Name.Length > "Kering".Length && xmlreaderKeringen.Name.Substring(0, "Kering".Length) == "Kering")
                    {
                      string naamkering = xmlreaderKeringen.GetAttribute("Kering");
                      int perc1 = ConvertString.ToInt32(xmlreaderKeringen.GetAttribute("Percentage1"));
                      int perc2 = ConvertString.ToInt32(xmlreaderKeringen.GetAttribute("Percentage2"));

                      this.m_Keringenparameters.Add(teller++, new InstellingenKeringen(naamkering, perc1, perc2));

                    }
                  }

              }
            }

          }
          else if (xmlreader.NodeType == XmlNodeType.Text)
          {
            switch (currentElement)
            {
              case "ZichtJaar":
                ZichtJaar = ConvertString.ToInt32(xmlreader.Value);
                break;
              case "OptimaleOverstromingskansenJaar":
                OptimaleOverstromingskansenJaar = ConvertString.ToInt32(xmlreader.Value);
                break;
              case "DiscontovoetSchade":
                DiscontovoetSchade = ConvertString.ToDouble(xmlreader.Value);
                break;
              case "DiscontovoetInvesteringen":
                DiscontovoetInvesteringen = ConvertString.ToDouble(xmlreader.Value);
                break;
              case "EconomischScenario":
                EconomischScenario = ConvertString.ToInt32(xmlreader.Value);
                break;
              case "BedragPerInwoner":
                BedragPerInwoner = ConvertString.ToInt32(xmlreader.Value);
                break;
              case "BedragPerDodelijkSlachtoffer":
                BedragPerDodelijkSlachtoffer = ConvertString.ToInt32(xmlreader.Value);
                break;
              case "BedragPerGetroffene":
                BedragPerGetroffene = ConvertString.ToInt32(xmlreader.Value);
                break;
              case "Aversiefactor":
                Aversiefactor = ConvertString.ToDouble(xmlreader.Value);
                break;
              case "BeleidsfactorOverstromingsschade":
                BeleidsfactorOverstromingsschade = ConvertString.ToDouble(xmlreader.Value);
                break;
              case "AanpassingsfactorOverstromingsschade":
                AanpassingsfactorOverstromingsschade = ConvertString.ToDouble(xmlreader.Value);
                break;
              case "DijkringspecifiekeFactorSchade":
                DijkringspecifiekeFactorSchade = ConvertString.ToInt32(xmlreader.Value);
                break;
              case "ParametersKostenfunctie":
                ParametersKostenfunctie = ConvertString.ToInt32(xmlreader.Value);
                break;
              case "ScenarioVoorHoeveelheidSchade":
                ScenarioVoorHoeveelheidSchade = ConvertString.ToInt32(xmlreader.Value);
                break;
              case "KlimaatScenarioEnFysischMaxAfvoer":
                KlimaatScenarioEnFysischMaxAfvoer = ConvertString.ToInt32(xmlreader.Value);
                break;
              case "RamingVoorSlachtoffers":
                RamingVoorSlachtoffers = ConvertString.ToInt32(xmlreader.Value);
                break;
              case "SchadeFunctie":
                SchadeFunctie = ConvertString.ToInt32(xmlreader.Value);
                break;
              case "FactorKosten":
                FactorKosten = ConvertString.ToDouble(xmlreader.Value);
                break;
              case "FactorKans":
                FactorKans = ConvertString.ToDouble(xmlreader.Value);
                break;
              case "FactorGroeiSchade":
                FactorGroeiSchade = ConvertString.ToDouble(xmlreader.Value);
                break;
              case "TypeKostenfunctie":
                TypeKostenfunctie = ConvertString.ToInt32(xmlreader.Value);
                break;
              case "Veiligheidsnorm":
                Veiligheidsnorm = ConvertString.ToInt32(xmlreader.Value);
                break;
              case "Parametersonzekerheid":
                Parametersonzekerheid = xmlreader.Value == "1";
                break;

            }
          }
        }
        xmlreader.Close();
      }
      else
      {
        throw new ApplicationException(string.Format("Bestand {0} bestaat niet!", naam));
      }
    }

    /// <summary>
    /// Bewaar de instellingen in een xml bestand
    /// </summary>
    /// <param name="naam">Naam instellingen bestand</param>
    public void Write(string naam, string applicationProductName, string applicationProductVersion)
    {
      XmlTextWriter xmlWriter = new XmlTextWriter(naam, Encoding.UTF8);

      //Use automatic indentation for readability.
      xmlWriter.Formatting = Formatting.Indented;

      xmlWriter.WriteStartDocument();
      xmlWriter.WriteComment(applicationProductName + " versie : " + applicationProductVersion);

      //Write the root element
      xmlWriter.WriteStartElement("OptimaliseRing");

      //Start an element
      xmlWriter.WriteStartElement("Instellingen");

      //add sub-elements
      xmlWriter.WriteElementString("ZichtJaar", ZichtJaar.ToString());
      xmlWriter.WriteElementString("OptimaleOverstromingskansenJaar", OptimaleOverstromingskansenJaar.ToString());
      xmlWriter.WriteElementString("DiscontovoetSchade", DiscontovoetSchade.ToString("F1"));
      xmlWriter.WriteElementString("DiscontovoetInvesteringen", DiscontovoetInvesteringen.ToString("F1"));
      xmlWriter.WriteElementString("EconomischScenario", EconomischScenario.ToString());
      xmlWriter.WriteElementString("BedragPerInwoner", BedragPerInwoner.ToString());
      xmlWriter.WriteElementString("BedragPerDodelijkSlachtoffer", BedragPerDodelijkSlachtoffer.ToString());
      xmlWriter.WriteElementString("BedragPerGetroffene", BedragPerGetroffene.ToString());
      xmlWriter.WriteElementString("Aversiefactor", Aversiefactor.ToString("F2"));
      xmlWriter.WriteElementString("BeleidsfactorOverstromingsschade", BeleidsfactorOverstromingsschade.ToString("F2"));
      xmlWriter.WriteElementString("AanpassingsfactorOverstromingsschade", AanpassingsfactorOverstromingsschade.ToString("F2"));
      xmlWriter.WriteElementString("DijkringspecifiekeFactorSchade", DijkringspecifiekeFactorSchade.ToString());
      xmlWriter.WriteElementString("ParametersKostenfunctie", ParametersKostenfunctie.ToString());
      xmlWriter.WriteElementString("ScenarioVoorHoeveelheidSchade", ScenarioVoorHoeveelheidSchade.ToString());
      xmlWriter.WriteElementString("KlimaatScenarioEnFysischMaxAfvoer", KlimaatScenarioEnFysischMaxAfvoer.ToString());
      xmlWriter.WriteElementString("TypeKostenfunctie", TypeKostenfunctie.ToString());
      xmlWriter.WriteElementString("Veiligheidsnorm", Veiligheidsnorm.ToString());
      xmlWriter.WriteElementString("RamingVoorSlachtoffers", RamingVoorSlachtoffers.ToString());
      xmlWriter.WriteElementString("SchadeFunctie", SchadeFunctie.ToString());
      xmlWriter.WriteElementString("FactorKosten", FactorKosten.ToString("F2"));
      xmlWriter.WriteElementString("FactorKans", FactorKans.ToString("F2"));
      xmlWriter.WriteElementString("FactorGroeiSchade", FactorGroeiSchade.ToString("F2"));
      xmlWriter.WriteElementString("Parametersonzekerheid", Parametersonzekerheid ? "1" : "0");

      xmlWriter.WriteStartElement("Compartimentering");

      int dijkringteller = 1;
      foreach (string dijkring in Compartimentering.Keys)
      {
        List<Compartimenteringsdijk> delen = Compartimentering[dijkring];

        xmlWriter.WriteStartElement("Dijkring" + dijkringteller++.ToString());
        xmlWriter.WriteAttributeString("Dijkring", dijkring);

        int deelteller = 1;
        foreach (Compartimenteringsdijk deel in Compartimentering[dijkring])
        {
          xmlWriter.WriteStartElement("Dijkdeel" + deelteller++.ToString());
          xmlWriter.WriteAttributeString("DijkId", deel.DijkId);
          xmlWriter.WriteAttributeString("DeelNr", deel.Dijkdeel);
          xmlWriter.WriteEndElement();
        }

        xmlWriter.WriteEndElement();
      }

      xmlWriter.WriteEndElement();

      // Scenario parameters
      xmlWriter.WriteStartElement("ParametersOnzekerheid");
      foreach (KeyValuePair<int, InstellingenOnzekerheid> instellingenOnzekerheid in this.m_Scenarioparameters)
      {
        xmlWriter.WriteStartElement("Scenario" + instellingenOnzekerheid.Key.ToString());
        xmlWriter.WriteAttributeString("EconomischScenario", instellingenOnzekerheid.Value.EconomischScenario.ToString());
        xmlWriter.WriteAttributeString("KlimaatScenarioEnFysischMaxAfvoer", instellingenOnzekerheid.Value.KlimaatScenarioEnFysischMaxAfvoer.ToString());
        xmlWriter.WriteAttributeString("DiscontovoetSchade", instellingenOnzekerheid.Value.DiscontovoetSchade.ToString("F1"));
        xmlWriter.WriteAttributeString("DiscontovoetInvesteringen", instellingenOnzekerheid.Value.DiscontovoetInvesteringen.ToString("F1"));
        xmlWriter.WriteEndElement();
      }
      xmlWriter.WriteEndElement();

      // Keringen
      xmlWriter.WriteStartElement("B-Keringen");
      foreach (KeyValuePair<int, InstellingenKeringen> instellingenKeringen in this.m_Keringenparameters)
      {
        xmlWriter.WriteStartElement("Kering" + instellingenKeringen.Key.ToString());
        xmlWriter.WriteAttributeString("Kering", instellingenKeringen.Value.NaamKering.ToString());
        xmlWriter.WriteAttributeString("Percentage1", instellingenKeringen.Value.Percentage1.ToString());
        xmlWriter.WriteAttributeString("Percentage2", instellingenKeringen.Value.Percentage2.ToString());

        xmlWriter.WriteEndElement();
      }
      xmlWriter.WriteEndElement();

      //End the item element
      xmlWriter.WriteEndElement();  // end item

      // end the root element
      xmlWriter.WriteFullEndElement();

      //Write the XML to file and close the xmlWriter
      xmlWriter.Close();

    }

    #endregion Member functions ----------------------------------------------
  }
}
