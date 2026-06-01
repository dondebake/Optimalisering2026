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

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;
using System.ComponentModel;
using Microsoft.Reporting.WinForms;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

using OptimaliseRing.General;
using OptimaliseRing.Profile;

using CenterSpace.NMath.Core;
using OptimaliseRing.Aimms;

namespace OptimaliseRing.Core
{
  /// <summary>
  /// Berekening
  /// </summary>
  public class Berekening
  {
    #region Instance Variables -----------------------------------------------

    private int m_Z;                                // Aantal tijdstappen
    private int m_ZichtJaar;                        // zichtjaar
    private int m_SchadeJaar;                       // Jaartal waarvoor schade in de database gegeven zijn
    private int m_KansJaar;                         // Jaartal waarvoor overstromingskans in de database gegeven zijn
    private int m_OptimaleOverstromingskansenJaar;  // Jaar van optimale overstroming
    private double m_FactorKans;                    // FactorKans
    private string m_BerekeningenMap;               // Map waar de berekening wordt opgeslagen
    private string m_Aimms;                         // Map waar het Aimmsproject staat
    private SortedList m_TeBerekenenDijkringdelen;  // Lijst met de te berekenen dijkringdelen
    private List<DijkringDeel> m_BerekeningenList;  // Lijst met de berekende dijkringdelen
    private List<Kansen> m_KansenList;              // Lijst t.b.v. kansen rapport
    private List<Strategie> m_StrategieList;        // Lijst t.b.v. strategie rapport
    private Profile.Ini m_Profile;                  // Applicatie ini-profile
    private Profile.Ini m_Language;                 // Language ini-profile
    private OptimaliseRingDB m_OptimaliseRingDB;    // OptimaliseRing database
    private ApplicationError m_ApplicationError;    // Applicatie error handler
    private CultureInfo m_CultureInfo;              // Huidige culture
    private Instellingen m_Instellingen;            // Instellingen van de berekening
    private int m_ResultCalculation;
    private int m_Veiligheidsnorm;                  // keuze van de veiligheidsnorm tijdens berekening

    // Scenarioparameters
    private SortedList<int, InstellingenOnzekerheid> m_Scenarioparameters;

    #endregion Instance Variables --------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Maak een nieuwe instantiatie van de <see cref="T:Berekeningen"/> class.
    /// </summary>
    public Berekening(
       CultureInfo cultureInfo
       , Profile.Ini profile
       , Profile.Ini languageProfile
       , OptimaliseRingDB optimaliseRingDB
       , ApplicationError applicationError
       )
    {
      m_CultureInfo = cultureInfo;
      m_Profile = profile;
      m_Language = languageProfile;
      m_OptimaliseRingDB = optimaliseRingDB;
      m_ApplicationError = applicationError;

      m_TeBerekenenDijkringdelen = new SortedList();
      m_BerekeningenList = new List<DijkringDeel>();
      m_KansenList = new List<Kansen>();
      m_StrategieList = new List<Strategie>();
      m_Scenarioparameters = new SortedList<int, InstellingenOnzekerheid>();

      Initialize();
    }

    #endregion Constructors --------------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets the language.
    /// </summary>
    /// <value>The language.</value>
    public Profile.Ini Language
    {
      get { return m_Language; }
    }

    /// <summary>
    /// optimaliseRing instellingen.
    /// </summary>
    /// <value>The instellingen.</value>
    public Instellingen Instellingen
    {
      get { return m_Instellingen; }
      set { m_Instellingen = value; }
    }

    /// <summary>
    /// optimaliseRing database
    /// </summary>
    /// <value>The optimalise ring DB.</value>
    public OptimaliseRingDB OptimaliseRingDB
    {
      get { return m_OptimaliseRingDB; }
    }


    /// <summary>
    /// Lijst van de te berekenen dijkringdelen.
    /// </summary>
    /// <value>The te berekenen dijkringdelen.</value>
    public SortedList TeBerekenenDijkringdelen
    {
      get { return m_TeBerekenenDijkringdelen; }
    }

    /// <summary>
    /// Berekeningsresultaat
    /// <remarks>0 = ok, 1= warning, -1 = error</remarks>
    /// </summary>
    /// <value>The result calculation.</value>
    public int ResultCalculation
    {
      set { m_ResultCalculation = value; }
      get { return m_ResultCalculation; }
    }

    /// <summary>
    /// Lijst met berekende dijkringdelen
    /// </summary>
    /// <value>The dijkring list.</value>
    public List<DijkringDeel> DijkringList
    {
      get { return m_BerekeningenList; }
    }

    /// <summary>
    /// Strategie resultaatlijst
    /// </summary>
    /// <value>Strategie resultaatlijst</value>
    public List<Strategie> StrategieList
    {
      get { return m_StrategieList; }
    }

    /// <summary>
    /// Kansen resultaatlijst
    /// </summary>
    /// <value>Kansen resultaatlijst</value>
    public List<Kansen> KansenList
    {
      get { return m_KansenList; }
    }

    /// <summary>
    /// Jaartal waarvoor overstromingskans in de database gegeven zijn
    /// </summary>
    /// <value>BeginjaarOverstromingskans.</value>
    public int Kansjaar
    {
      get { return m_KansJaar; }
    }

    /// <summary>
    /// Jaartal waarvoor schade in de database gegeven zijn
    /// </summary>
    /// <value>BeginjaarSchade</value>
    public int Schadejaar
    {
      get { return m_SchadeJaar; }
    }

    /// <summary>
    /// Gets or Sets Zichtjaar.
    /// </summary>
    /// <value>Zichtjaar</value>
    public int ZichtJaar
    {
      get { return this.m_ZichtJaar; }
      set { this.m_ZichtJaar = value; }
    }

    /// <summary>
    /// Gets or sets the factor kans.
    /// </summary>
    /// <value>The factor kans.</value>
    public double FactorKans
    {
      get { return this.m_FactorKans; }
      set { this.m_FactorKans = value; }
    }

    /// <summary>
    /// Gets or Sets De optimale overstromingskansen jaar.
    /// </summary>
    /// <value>The optimale overstromingskansen jaar.</value>
    public int OptimaleOverstromingskansenJaar
    {
      get { return this.m_OptimaleOverstromingskansenJaar; }
      set { this.m_OptimaleOverstromingskansenJaar = value; }
    }

    /// <summary>
    /// Aantal tijdstappen
    /// </summary>
    /// <value>Aantal tijdstappen</value>
    public int Z
    {
      get { return m_Z; }
    }

    /// <summary>
    /// Map met de berekeningen
    /// </summary>
    /// <value>Map met de berekeningen</value>
    public string BerekeningenMap
    {
      set { m_BerekeningenMap = value; }
      get { return m_BerekeningenMap; }
    }

    /// <summary>
    /// Map waar het Aimmsproject staat
    /// </summary>
    /// <value>The aimms map.</value>
    public string Aimms
    {
      set { m_Aimms = value; }
      get { return Aimms; }
    }

    /// <summary>
    /// Gets the scenarioparameters.
    /// </summary>
    /// <value>The scenarioparameters.</value>
    public SortedList<int, InstellingenOnzekerheid> Scenarioparameters
    {
      get { return m_Scenarioparameters; }
    }

    /// <summary>
    /// Gets the veiligheidsnorm.
    /// </summary>
    /// <value>The veiligheidsnorm.</value>
    public int Veiligheidsnorm
    {
      get { return this.m_Veiligheidsnorm; }
    }

    /// <summary>
    /// Gets the veiligheidsnorm tekst.
    /// </summary>
    /// <value>The veiligheidsnorm tekst.</value>
    public string VeiligheidsnormTekst
    {
      get
      {
        string veiligheidsnormTekst = m_Language.GetValue("Captions:" + m_CultureInfo.Name
        , string.Format("UVeiligheidsnormKeuze{0}", this.m_Veiligheidsnorm)
        , string.Format("UVeiligheidsnormKeuze{0}", this.m_Veiligheidsnorm));
        return veiligheidsnormTekst;
      }
    }

    #endregion Properties ----------------------------------------------------

    #region Member functions -------------------------------------------------

    /// <summary>
    /// Initialiseer de berekening
    /// </summary>
    private void Initialize()
    {
      if (m_OptimaliseRingDB != null)
      {
        m_KansJaar = m_OptimaliseRingDB.KansJaar();
        m_SchadeJaar = m_OptimaliseRingDB.SchadeJaar();
      }

      m_Z = ConvertString.ToInt32(m_Profile.GetValue("Parameters", "Z", "300"));

      this.m_Aimms = Path.Combine(Application.StartupPath, "Aimms\\Dijk1.prj");
    }


    /// <summary>
    /// Start de optimalisatie berekening
    /// </summary>
    /// <param name="status">Status van de berekening.</param>
    /// <param name="berekeningNummer">Berekeningnummer.</param>
    /// <param name="totaalAantalBerekeningen">Totaal aantal berekeningen.</param>
    public bool Start(IStatus status, ref int berekeningNummer, int totaalAantalBerekeningen)
    {
      using (new WaitCursor())
      {
        Initialize();

        // Reset instellingen
        this.m_BerekeningenList.Clear();

        this.m_ZichtJaar = this.m_Instellingen.ZichtJaar;
        this.m_OptimaleOverstromingskansenJaar = this.m_Instellingen.OptimaleOverstromingskansenJaar;
        this.m_FactorKans = this.m_Instellingen.FactorKans;
        this.m_Veiligheidsnorm = this.m_Instellingen.Veiligheidsnorm;
        this.m_Scenarioparameters.Clear();

        if (this.Instellingen.Parametersonzekerheid)
        {
          foreach (KeyValuePair<int, InstellingenOnzekerheid> keyValuePair in this.m_Instellingen.Scenarioparameters)
          {
            this.m_Scenarioparameters.Add(keyValuePair.Key, new InstellingenOnzekerheid(
                keyValuePair.Value.EconomischScenario
              , keyValuePair.Value.KlimaatScenarioEnFysischMaxAfvoer
              , keyValuePair.Value.DiscontovoetSchade
              , keyValuePair.Value.DiscontovoetInvesteringen));
          }
        }

        if (m_TeBerekenenDijkringdelen != null)
        {
          // Bewaar de gebruikte instellingen
          m_Instellingen.Write(Path.Combine(m_BerekeningenMap, "Instellingen.xml"), Application.ProductName, Application.ProductVersion);

          m_KansenList.Clear();
          m_StrategieList.Clear();

          // Maak instellingenbestand
          Instellingenbestand(this.m_BerekeningenMap);

          // Lus over de te berekenen dijkringen
          for (int index = 0; index < m_TeBerekenenDijkringdelen.Count; index++)
          {
            berekeningNummer++;

            SortedList record = (SortedList)m_TeBerekenenDijkringdelen.GetByIndex(index);

            DijkringDeel dijkringDeel = new DijkringDeel(
               m_CultureInfo
               , m_Profile
               , m_Language
               , m_Instellingen
               , m_OptimaliseRingDB
               , record["Dijkring"].ToString()
               , record["DijkringDeel"].ToString()
               , record["Naam"].ToString()
               );

            // Schrijf statusbericht
            if (status != null)
            {
              string statusBerekening = string.Format(" ({0} van {1})", berekeningNummer, totaalAantalBerekeningen);
              if (dijkringDeel.Naam == dijkringDeel.DijkringNaam)
              {
                status.Status = "Start : " + dijkringDeel.DijkringId + "-" + dijkringDeel.DijkringNaam + statusBerekening;

              }
              else
              {
                status.Status = "Start : " + dijkringDeel.DijkringId + "-" + dijkringDeel.DijkringNaam + "-" + dijkringDeel.Naam + statusBerekening;

              }
            }

            // Maak de rapporten
            CreateReportTabellen(dijkringDeel);

            // Start berekening
            if (dijkringDeel.CalculateAimms(this.m_Instellingen, this.m_KansJaar, this.m_SchadeJaar, this.m_ZichtJaar
              , this.m_OptimaleOverstromingskansenJaar, this.m_Z, this.m_Aimms, this.m_BerekeningenMap, this.FactorKans
              , this.m_Scenarioparameters))
            {

              // Vul de rapporten
              FillReportTabellen(dijkringDeel, this.m_ZichtJaar);

              // Bewaar resultaten
              m_BerekeningenList.Add(dijkringDeel);

              // Maak het overzichtsbestand
              dijkringDeel.Overzichtsbestand(this.m_Instellingen, BerekeningenMap, this.m_ZichtJaar, this.m_SchadeJaar
                , this.m_KansJaar, this.m_OptimaleOverstromingskansenJaar, this.FactorKans);

              // Schrijf statusbericht
              if (status != null)
              {
                status.Status = string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Genereren").ToString());
              }
              Write(Application.ProductName, Application.ProductVersion);
            }
            else
            {
              if (status != null)
              {
                status.Status = string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Mislukt").ToString());
              }
              return false;
            }
          }

          //Write(Application.ProductName, Application.ProductVersion);

        }
      }
      return true;
    }

    /// <summary>
    /// Scrijf de instellingen uit ini-bestand onder parameters naar een nieuwe ini-bestand
    /// en zet deze in de berekeningmap.
    /// </summary>
    /// <param name="berekeningenMap">The berekeningen map.</param>
    public void Instellingenbestand(string berekeningenMap)
    {
      string bestandsNaam = "Aimms_instellingen.ini";
      string sectionNaam = "Parameters";

      string instellingenbestand = Path.Combine(berekeningenMap, bestandsNaam);
      if (File.Exists(instellingenbestand))
      {
        File.Delete(instellingenbestand);
      }

      Ini instellingenIni = new Ini(instellingenbestand);

      foreach (string entryName in this.m_Profile.GetEntryNames(sectionNaam))
      {
        instellingenIni.SetValue(sectionNaam, entryName, this.m_Profile.GetValue(sectionNaam, entryName));
      }

    }

    /// <summary>
    /// maak de Kansen en Strategie excel bestanden
    /// </summary>
    public void StrategieBestand(string bestandsnaam)
    {
      ReportForm reportForm = new ReportForm(m_Profile, m_Language, this);
      reportForm.Text = Application.ProductName + " - " + m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Resultaten", "Resultaten");

      // Strategiereport vullen
      reportForm.StrategieReport();

      // Save reports
      Warning[] warnings;
      string[] streamids;
      string mimeType;
      string encoding;
      string filenameExtension;

      byte[] strategieBytes = reportForm.ReportViewerStrategie.LocalReport.Render(
        "Excel", null, out mimeType, out encoding, out filenameExtension,
        out streamids, out warnings);

      using (FileStream fs = new FileStream(Path.Combine(this.BerekeningenMap, bestandsnaam), FileMode.Create))
      {
        fs.Write(strategieBytes, 0, strategieBytes.Length);
      }

      reportForm.Dispose();

    }

    /// <summary>
    /// maak de Kansen en Strategie excel bestanden
    /// </summary>
    public void KansenBestand(string bestandsnaam)
    {
      ReportForm reportForm = new ReportForm(m_Profile, m_Language, this);
      reportForm.Text = Application.ProductName + " - " + m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Resultaten", "Resultaten");

      // Kansenreport vullen
      reportForm.KansenReport();

      // Save reports
      Warning[] warnings;
      string[] streamids;
      string mimeType;
      string encoding;
      string filenameExtension;

      byte[] kansenBytes = reportForm.ReportViewerKansen.LocalReport.Render(
        "Excel", null, out mimeType, out encoding, out filenameExtension,
        out streamids, out warnings);

      using (FileStream fs = new FileStream(Path.Combine(this.BerekeningenMap, bestandsnaam), FileMode.Create))
      {
        fs.Write(kansenBytes, 0, kansenBytes.Length);
      }


      reportForm.Dispose();

    }

    /// <summary>
    /// Maak de resultaattabellen.
    /// </summary>
    /// <param name="dijkringDeel">Berekende dijkringdeel</param>
    private void CreateReportTabellen(DijkringDeel dijkringDeel)
    {
      Kansen kansen = new Kansen(dijkringDeel.DijkringId, dijkringDeel.DijkringNaam, dijkringDeel.Naam, dijkringDeel.DeelNummer
        , this.Instellingen.ZichtJaar, this.Instellingen.OptimaleOverstromingskansenJaar);

      KansenList.Add(kansen);

      for (int j = 0; j < dijkringDeel.Trajecten.Count; j++)
      {
        DijkringTraject dijkringTraject = (DijkringTraject)dijkringDeel.Trajecten[j];
        if (dijkringTraject != null)
        {
          Strategie strategie = new Strategie(j + 1, dijkringDeel.DijkringId, dijkringDeel.DijkringNaam, dijkringDeel.Naam, dijkringTraject.Naam);
          StrategieList.Add(strategie);
        }
      }
    }

    /// <summary>
    /// Vul de resultaattabellen
    /// </summary>
    /// <param name="dijkringDeel">Berekende dijkringdeel</param>
    private void FillReportTabellen(DijkringDeel dijkringDeel, int zichtJaar)
    {
      int scenarioIndex = 0;

      if (dijkringDeel != null)
      {
        Kansen kansen = GetKansen(dijkringDeel);
        if (kansen != null)
        {
          kansen.WettelijkeNorm = dijkringDeel.Kansen.WettelijkeNorm;
          kansen.RestrictiePmin = dijkringDeel.Kansen.RestrictiePmin;

          kansen.OverstromingskansInZichtjaar = dijkringDeel.Kansen.OverstromingskansInZichtjaar;
          kansen.OverstromingskansInOptimaleOverstromingskansJaar = dijkringDeel.Kansen.OverstromingskansInOptimaleOverstromingskansJaar;

          kansen.OptimaleOverstromingskansInZichtjaar = dijkringDeel.Kansen.OptimaleOverstromingskansInZichtjaar;
          kansen.OptimaleOverstromingskansInOptimaleOverstromingskansJaar = dijkringDeel.Kansen.OptimaleOverstromingskansInOptimaleOverstromingskansJaar;

          kansen.OptimaleOverschrijdingskansInZichtjaar = dijkringDeel.Kansen.OptimaleOverschrijdingskansInZichtjaar;
          kansen.OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar = dijkringDeel.Kansen.OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar;

          kansen.ContantewaardeInvesteringenEnSchade = dijkringDeel.Kansen.ContantewaardeInvesteringenEnSchade;

          kansen.MatrixData = dijkringDeel.Kansen.MatrixData;

          kansen.Zichtjaar = dijkringDeel.Kansen.Zichtjaar;
          kansen.OptimaleOverstromingskansenJaar = dijkringDeel.Kansen.OptimaleOverstromingskansenJaar;

          kansen.TotaalKostenInvesteringen = 0.0;
          for (int indexTraject = 0; indexTraject < dijkringDeel.Trajecten.Count; indexTraject++)
          {
            DijkringTraject dijkringTraject = dijkringDeel.Trajecten[indexTraject];
            if (dijkringTraject != null)
            {
              kansen.TotaalKostenInvesteringen += dijkringTraject.TotaleKostenInvesteringen[scenarioIndex];
            }
          }

          for (int indexTraject = 0; indexTraject < dijkringDeel.Trajecten.Count; indexTraject++)
          {
            DijkringTraject dijkringTraject = dijkringDeel.Trajecten[indexTraject];
            if (dijkringTraject != null)
            {
              Strategie strategie = GetStrategie(dijkringTraject, indexTraject + 1);
              if (strategie != null)
              {
                if (dijkringDeel.Kansen.EersteInvesteringJaar != int.MaxValue)
                {
                  // Bepaal overstromingskans in jaar voor eerste investering
                  double overstromingskansInJaarVoorInvestering = 0.0;

                  if (dijkringDeel.Kansen.EersteInvesteringJaar == zichtJaar)
                  {
                    double overstromingskans = dijkringTraject.P0Overstromingskans
                      * NMathFunctions.Exp(dijkringTraject.AlphaOverstromingskans * dijkringTraject.Eta
                      * (zichtJaar - this.Kansjaar));

                    overstromingskansInJaarVoorInvestering = overstromingskans
                      * this.FactorKans * dijkringTraject.Factor;
                  }
                  else
                  {
                    int indexJaarVoorInvestering
                      = Math.Max(0, (dijkringDeel.Kansen.EersteInvesteringJaar - 1) - zichtJaar);

                    overstromingskansInJaarVoorInvestering = dijkringDeel.Kansen.MatrixData[
                      indexJaarVoorInvestering, Convert.ToInt32(MatrixKolom.AANTAL) + indexTraject];
                  }

                  int indexJaarVanInvestering = dijkringDeel.Kansen.EersteInvesteringJaar - zichtJaar;

                  double overstromingskansJaarVanInvestering = dijkringDeel.Kansen.MatrixData[
                    indexJaarVanInvestering, Convert.ToInt32(MatrixKolom.AANTAL) + indexTraject];

                  // Splits kosten
                  OptimaliseRing.Core.DijkringTraject.Kostensplitsing kostensplitsing = new DijkringTraject.Kostensplitsing();
                  if (dijkringTraject.Investeringen.Count > 0)
                  {
                    kostensplitsing
                      = dijkringTraject.GetKostensplitsing(scenarioIndex, dijkringDeel.Kansen.WettelijkeNorm, dijkringDeel.Gamma
                      , dijkringDeel.Psi, dijkringDeel.MaxEta()
                      , dijkringDeel.Kansen.OptimaleOverstromingskansInZichtjaar
                      , overstromingskansInJaarVoorInvestering, overstromingskansJaarVanInvestering
                      , dijkringDeel.Kansen.EersteInvesteringJaar, this.ZichtJaar, Instellingen.FactorGroeiSchade);
                  }

                  strategie.JaarEersteVerhoging = dijkringTraject.Investeringen.Count > 0 ? dijkringTraject.Investeringen[0].Jaar : 0;
                  strategie.HoogteEersteVerhoging = dijkringTraject.Investeringen.Count > 0 ? dijkringTraject.Investeringen[0].Hoogte : 0.0;
                  strategie.KostenEersteVerhoging = kostensplitsing.KostenInvestering != null && kostensplitsing.KostenInvestering.Length > 0 ? kostensplitsing.KostenInvestering[0] : 0.0;
                  strategie.KostenEersteVerhogingNormenAchterstand = kostensplitsing.Inhaal != null && kostensplitsing.Inhaal.Length > 0 ? kostensplitsing.Inhaal[0] : 0.0;
                  strategie.KostenEersteVerhogingEconomie = kostensplitsing.EconomieInvestering != null && kostensplitsing.EconomieInvestering.Length > 0 ? kostensplitsing.EconomieInvestering[0] : 0.0;
                  strategie.KostenEersteVerhogingKlimaat = kostensplitsing.KlimaatInvestering != null && kostensplitsing.KlimaatInvestering.Length > 0 ? kostensplitsing.KlimaatInvestering[0] : 0.0;

                  strategie.JaarTweedeVerhoging = dijkringTraject.Investeringen.Count > 1 ? dijkringTraject.Investeringen[1].Jaar : 0;
                  strategie.HoogteTweedeVerhoging = dijkringTraject.Investeringen.Count > 1 ? dijkringTraject.Investeringen[1].Hoogte : 0.0;
                  strategie.KostenTweedeVerhoging = kostensplitsing.KostenInvestering != null && kostensplitsing.KostenInvestering.Length > 1 ? kostensplitsing.KostenInvestering[1] : 0.0;
                  strategie.KostenTweedeVerhogingEconomie = kostensplitsing.EconomieInvestering != null && kostensplitsing.EconomieInvestering.Length > 1 ? kostensplitsing.EconomieInvestering[1] : 0.0;
                  strategie.KostenTweedeVerhogingKlimaat = kostensplitsing.KlimaatInvestering != null && kostensplitsing.KlimaatInvestering.Length > 1 ? kostensplitsing.KlimaatInvestering[1] : 0.0;

                  strategie.ContantewaardeInvesteringskosten = kostensplitsing.TotaleKostenInvesteringen;
                }
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Gets the dijkring deel.
    /// </summary>
    /// <param name="kansen">The kansen.</param>
    /// <returns></returns>
    public DijkringDeel GetDijkringDeel(Kansen kansen)
    {
      DijkringDeel dijkringDeel = null;
      for (int i = 0; i < DijkringList.Count; i++)
      {

        if ((kansen.DijkringId == DijkringList[i].DijkringId)
         && (kansen.DijkringNaam == DijkringList[i].DijkringNaam)
         && ((kansen.Deel == DijkringList[i].Naam) || (kansen.Deel == "")))
        {
          dijkringDeel = DijkringList[i];
          break;
        }
      }
      return dijkringDeel;
    }

    /// <summary>
    /// Bepaal de kansen behorende bij dit dijkringdeel
    /// </summary>
    /// <param name="dijkringDeel">Berekende dijkringdeel</param>
    /// <returns></returns>
    private Kansen GetKansen(DijkringDeel dijkringDeel)
    {
      Kansen kansen = null;
      for (int i = 0; i < KansenList.Count; i++)
      {
        kansen = KansenList[i];
        if ((kansen.DijkringId == dijkringDeel.DijkringId)
          && (kansen.DijkringNaam == dijkringDeel.DijkringNaam)
          && (kansen.Deel == dijkringDeel.Naam))
        {
          break;
        }
      }
      return kansen;
    }

    /// <summary>
    /// Bepaal de strategie behorende bij dit traject
    /// </summary>
    /// <param name="traject">Dijkringtraject</param>
    /// <returns></returns>
    private Strategie GetStrategie(DijkringTraject traject, int trajectNummer)
    {
      Strategie strategie = null;
      for (int i = 0; i < StrategieList.Count; i++)
      {
        strategie = StrategieList[i];

        if (!traject.Iskering)
        {
          if ((strategie.DijkringId == traject.DijkringId)
            && (strategie.DijkringNaam == traject.DijkringNaam)
            && (strategie.Deel == traject.DijkringDeel)
            && (strategie.Traject == traject.Naam)
            && (strategie.TrajectNummer == trajectNummer))
          {
            return strategie;
          }
        }
        else
        {
          if ((strategie.Traject == traject.Naam)
            && (strategie.TrajectNummer == trajectNummer))
          {
            return strategie;
          }
        }

      }
      return null;
    }

    /// <summary>
    /// Lees de berekeningen
    /// </summary>
    /// <param name="berekeningenMap">De map met de berekeningen</param>
    /// <param name="optimaleOverstromingskansenJaar">The optimale overstromingskansen jaar.</param>
    /// <param name="createTabel">if set to <c>true</c> [create tabel].</param>
    public void Read(string berekeningenMap, int optimaleOverstromingskansenJaar, bool createTabel)
    {
      bool optimaliseRing = false;
      string bestand = Path.Combine(berekeningenMap, "OptimaliseRing.xml");
      if (File.Exists(bestand))
      {
        using (new WaitCursor())
        {
          XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
          xmlReaderSettings.ConformanceLevel = ConformanceLevel.Document;
          xmlReaderSettings.IgnoreWhitespace = true;
          xmlReaderSettings.IgnoreComments = true;

          m_KansenList.Clear();
          m_StrategieList.Clear();
          m_BerekeningenList.Clear();

          using (XmlReader xmlReader = XmlReader.Create(bestand, xmlReaderSettings))
          {
            // Lees <OptimaliseRing>
            while (xmlReader.Read())
            {
              if ((xmlReader.Name == "OptimaliseRing") && xmlReader.IsStartElement())
              {
                optimaliseRing = true;
                ReadBerekeningen(xmlReader, optimaleOverstromingskansenJaar, createTabel);
              }
            }
          }
          if (!optimaliseRing)
          {
            {
              throw new ApplicationException(string.Format("'{0}' is geen OptimaliseRing bestand!", bestand));
            }
          }
        }
      }
      else
      {
        throw new ApplicationException(string.Format("Bestand '{0}' bestaat niet !", bestand));
      }
    }

    /// <summary>
    /// Lees de berekeningen
    /// </summary>
    /// <param name="xmlReader">XML reader.</param>
    /// <param name="optimaleOverstromingskansenJaar">The optimale overstromingskansen jaar.</param>
    /// <param name="createTabel">if set to <c>true</c> [create tabel].</param>
    private void ReadBerekeningen(XmlReader xmlReader, int optimaleOverstromingskansenJaar, bool createTabel)
    {
      while (xmlReader.Read())
      {
        if ((xmlReader.Name == "Berekeningen") && xmlReader.IsStartElement())
        {
          if (xmlReader.HasAttributes)
          {
            while (xmlReader.MoveToNextAttribute())
            {
              if (xmlReader.Name == "Zichtjaar")
              {
                m_ZichtJaar = Int32.Parse(xmlReader.Value);
              }
              if (xmlReader.Name == "OptimaleOverstromingskansenJaar")
              {
                m_OptimaleOverstromingskansenJaar = optimaleOverstromingskansenJaar == -1
                  ? Int32.Parse(xmlReader.Value) : optimaleOverstromingskansenJaar;
              }
              if (xmlReader.Name == "Schadejaar")
              {
                m_SchadeJaar = Int32.Parse(xmlReader.Value);
              }
              if (xmlReader.Name == "Kansjaar")
              {
                m_KansJaar = Int32.Parse(xmlReader.Value);
              }
              if (xmlReader.Name == "Z")
              {
                m_Z = Int32.Parse(xmlReader.Value);
              }
              if (xmlReader.Name == "FactorKans")
              {
                m_FactorKans = Double.Parse(xmlReader.Value);
              }
              if (xmlReader.Name == "Veiligheidsnorm")
              {
                m_Veiligheidsnorm = Int32.Parse(xmlReader.Value);
              }
            }
          }
          ReadScenarioparameters(xmlReader);
          ReadDijkringDeel(xmlReader, optimaleOverstromingskansenJaar, createTabel);
        }
      }
    }

    private void ReadScenarioparameters(XmlReader xmlReader)
    {
      this.m_Scenarioparameters.Clear();

      xmlReader.Read();

      if (xmlReader.Name == "ParametersOnzekerheid")
      {
        XmlReader xmlreaderParametersOnzekerheid = xmlReader.ReadSubtree();
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
          }
        }
      }
    }

    /// <summary>
    /// Lees dijkringdeel
    /// </summary>
    /// <param name="xmlReader">XML reader.</param>
    /// <param name="optimaleOverstromingskansenJaar">The optimale overstromingskansen jaar.</param>
    /// <param name="createTabel">if set to <c>true</c> [create tabel].</param>
    private void ReadDijkringDeel(XmlReader xmlReader, int optimaleOverstromingskansenJaar, bool createTabel)
    {

      while (xmlReader.Read())
      {
        if ((xmlReader.Name == "DijkringDeel") && xmlReader.IsStartElement())
        {
          string dijkringId = "";
          string dijkringNaam = "";
          string deel = "";
          string deelNummer = "";

          if (xmlReader.HasAttributes)
          {
            while (xmlReader.MoveToNextAttribute())
            {
              if (xmlReader.Name == "DijkringId")
              {
                dijkringId = xmlReader.Value;
              }
              else if (xmlReader.Name == "DijkringNaam")
              {
                dijkringNaam = xmlReader.Value;
              }
              else if (xmlReader.Name == "Deel")
              {
                deel = xmlReader.Value;
              }
              else if (xmlReader.Name == "DeelNummer")
              {
                deelNummer = xmlReader.Value;
              }
            }
          }

          DijkringDeel dijkringDeel = new DijkringDeel(
             m_CultureInfo
             , m_Profile
             , m_Instellingen
             , m_OptimaliseRingDB
             , dijkringId
             , dijkringNaam
             , deelNummer
             , deel);

          dijkringDeel.Read(xmlReader);

          // zet OptimaleOverstromingskansenJaar
          if (optimaleOverstromingskansenJaar > -1)
          {
            dijkringDeel.Kansen.OptimaleOverstromingskansenJaar = Math.Max(dijkringDeel.Kansen.Zichtjaar
              , optimaleOverstromingskansenJaar);

            dijkringDeel.BepaalKansenInOptimaleOverstromingsJaar(dijkringDeel.Kansen.Zichtjaar
              , dijkringDeel.Kansen.OptimaleOverstromingskansenJaar, this.m_FactorKans);
          }

          CreateReportTabellen(dijkringDeel);

          FillReportTabellen(dijkringDeel, m_Instellingen.ZichtJaar);

          m_BerekeningenList.Add(dijkringDeel);

          //if (optimaleOverstromingskansenJaar > -1 & createTabel)
          //{
          //  this.KansenBestand(string.Format("Kansen_{0}.xls", optimaleOverstromingskansenJaar));
          //}

        }
      }
      if (optimaleOverstromingskansenJaar > -1 & createTabel)
      {
        this.KansenBestand(string.Format("Kansen_{0}.xls", optimaleOverstromingskansenJaar));
      }
    }

    /// <summary>
    /// Schrijf xml bestand
    /// </summary>
    public void Write(string productName, string productVersion)
    {
      string bestand = Path.Combine(BerekeningenMap, "OptimaliseRing.xml");
      if (File.Exists(bestand))
      {
        File.Delete(bestand);
      }

      XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
      xmlWriterSettings.Indent = true;
      xmlWriterSettings.CloseOutput = false;
      xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
      xmlWriterSettings.OmitXmlDeclaration = false;

      // Write XML data.
      using (XmlWriter xmlWriter = XmlWriter.Create(bestand, xmlWriterSettings))
      {
        xmlWriter.WriteComment("Generated by : " + productName + " - Version : " + productVersion);

        xmlWriter.WriteStartElement("OptimaliseRing");
        xmlWriter.WriteStartElement("Berekeningen");

        xmlWriter.WriteAttributeString("Zichtjaar", m_ZichtJaar.ToString());
        xmlWriter.WriteAttributeString("OptimaleOverstromingskansenJaar", m_OptimaleOverstromingskansenJaar.ToString());
        xmlWriter.WriteAttributeString("Schadejaar", m_SchadeJaar.ToString());
        xmlWriter.WriteAttributeString("Kansjaar", m_KansJaar.ToString());
        xmlWriter.WriteAttributeString("Z", m_Z.ToString());
        xmlWriter.WriteAttributeString("FactorKans", m_FactorKans.ToString());
        xmlWriter.WriteAttributeString("Veiligheidsnorm", m_Veiligheidsnorm.ToString());

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

        for (int i = 0; i < m_BerekeningenList.Count; i++)
        {
          DijkringDeel dijkringDeel = m_BerekeningenList[i];
          dijkringDeel.Write(xmlWriter);
        }
        // </Berekeningen>
        xmlWriter.WriteEndElement();

        xmlWriter.WriteEndElement();
        xmlWriter.Flush();
        xmlWriter.Close();
      }

    }

    /// <summary>
    /// Gets the dijkringdeel by id.
    /// </summary>
    /// <param name="dijkringdeelId">The dijkringdeel id.</param>
    /// <param name="deelnummer">The deelnummer.</param>
    /// <returns></returns>
    public DijkringDeel GetDijkringdeelById(string dijkringdeelId, Int32 deelnummer)
    {
      foreach (DijkringDeel dijkringDeel in DijkringList)
      {
        if (dijkringDeel.DijkringId.ToLower() == dijkringdeelId.ToLower() &&
            deelnummer == dijkringDeel.DeelNummer)
        {
          return dijkringDeel;
        }
      }
      return null;
    }

    /// <summary>
    /// Gets the kans by id.
    /// </summary>
    /// <param name="dijkringId">The dijkring id.</param>
    /// <param name="deelnummer">The deelnummer.</param>
    /// <returns></returns>
    public Kansen GetKansById(string dijkringId, Int32 deelnummer)
    {
      if (KansenList != null)
      {
        foreach (Kansen kans in KansenList)
        {
          if (string.Compare(dijkringId, kans.DijkringId) == 0)
          {
            if (deelnummer == kans.DeelNummer)
            {
              return kans;
            }
          }
        }
      }

      return null;
    }


    /// <summary>
    /// Controleer de dijkring berekeningen
    /// </summary>
    public StringBuilder Controleer(
       DataGridView dgvBerekeningen
       , SortedList dijkringDelen
       , SortedList<string, List<Compartimenteringsdijk>> compartimentering)
    {
      List<bool> containsError = new List<bool>();

      m_Instellingen.Compartimentering.Clear();

      StringBuilder fouteDijkringen = new StringBuilder();

      m_TeBerekenenDijkringdelen.Clear();

      // Lus over alle dijkringdelen
      for (int i = 0; i < dgvBerekeningen.RowCount; i++)
      {
        containsError.Add(false);

        if (ConvertString.ToBoolean(dgvBerekeningen["Berekenen", i].Value.ToString()))
        {
          // Dijkringdeel doet mee
          SortedList record = (SortedList)dijkringDelen.GetByIndex(i);

          DijkringDeel dijkringDeel = new DijkringDeel(
             m_CultureInfo
             , m_Profile
             , m_Language
             , m_Instellingen
             , m_OptimaliseRingDB
             , record["Dijkring"].ToString()
             , record["DijkringDeel"].ToString()
             , record["Naam"].ToString());

          // Check
          if (dijkringDeel.CheckDiscontovoet(m_Instellingen.DiscontovoetSchade
            , m_Instellingen.DiscontovoetInvesteringen, m_Instellingen.FactorGroeiSchade))
          {
            containsError[i] = true;
            fouteDijkringen.Append(record["Naam"].ToString() + "\n");
          }
          else
          {
            string compDijk = dgvBerekeningen["CompartimenteringsDijkring", i].Value.ToString();
            if (compartimentering.ContainsKey(compDijk))
            {
              if (!m_Instellingen.Compartimentering.ContainsKey(compDijk))
              {
                m_Instellingen.Compartimentering.Add(compDijk, compartimentering[compDijk]);
              }
            }
            m_TeBerekenenDijkringdelen.Add(m_TeBerekenenDijkringdelen.Count, record);
          }
        }
      }

      return fouteDijkringen;
    }


    #endregion Member functions ----------------------------------------------
  }
}
