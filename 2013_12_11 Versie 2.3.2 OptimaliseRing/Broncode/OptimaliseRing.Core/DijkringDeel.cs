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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.Core/DijkringDeel.cs 7     28-04-09 13:55 Waterman $
// $NoKeywords: $
#endregion
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

using OptimaliseRing.General;
using OptimaliseRing.Profile;

using CenterSpace.NMath.Core;
using OptimaliseRing.Aimms;
using System.Runtime.Serialization.Formatters.Binary;

namespace OptimaliseRing.Core
{
  /// <summary>
  /// Dijkringdeel
  /// </summary>
  public class DijkringDeel : ICloneable
  {
    #region Instance Variables -----------------------------------------------

    private string m_DijkringId;                         // dijkring id
    private string m_DijkringNaam;                       // dijkring naam
    private int m_DeelNummer;                            // dijkringdeel nummer
    private string m_Naam;                               // dijkringdeel naam
    private long m_AantalInwoners;                       // Aantal inwoners
    private long m_AantalSlachtoffers;                   // Aantal slachtoffers
    private long m_AantalGetroffenen;                    // Aantal getroffenen
    private double m_Schade;                             // Schade beginjaar in M€
    private double m_Nu;                                 // Schaalparameter schade afhankelijk van waterstand [1/cm]
    private double m_Gamma;                              // Tempo van de economische groei in %/jaar
    private double m_Zeta;                               // Stijgingstempo schade per cm verhoging in 1/cm
    private double m_DijkringspecifiekeFactorSchade;     // Dijkringspecifieke aanpassingsfactor overstromingsschade (werkelijke waarde)

    private double m_Psi;                                //Waterstandsverhoging als functie van klimaatverandering
    private double m_Min_overschrijdingskans;            //


    private List<DijkringTraject> m_Trajecten;           // Lijst met trajecten van dit dijkringdeel
    private bool m_IsKering;                             //is het een kering

    private Kansen m_kansen;                             // Kansen voor dit dijkringdeel

    private double m_OptimaalGrootsteOverstromingskans;  // Optimale grootste overstromingskans in eerste jaar
    private double m_OptimaalKleinsteOverstromingskans;  // Optimale kleinste overstromingskans in eerste jaar

    private OptimaliseRingDB m_OptimaliseRingDB;         // OptimaliseRing database
    private Profile.Ini m_Profile;
    private Profile.Ini m_Language;
    private CultureInfo m_CultureInfo;

    /// <summary>
    /// Pmidden van scenario's voor zichtjaar en optimale overstromingskansjaar
    /// </summary>
    private List<PmiddensPerScenario> m_PmiddensPerScenario;

    #endregion Instance Variables --------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:DijkringDeel"/> class.
    /// </summary>
    /// <param name="dijkringId">Dijkring identificatie</param>
    /// <param name="deelNummer">The deel nummer.</param>
    /// <param name="naam">The naam.</param>
    public DijkringDeel(CultureInfo cultureInfo, Profile.Ini profile, Profile.Ini languageProfile
      , Instellingen instellingen, OptimaliseRingDB optimaliseRingDB, string dijkringId, string deelNummer, string naam)
    {
      this.m_CultureInfo = cultureInfo;
      this.m_Profile = profile;
      this.m_Language = languageProfile;
      this.m_OptimaliseRingDB = optimaliseRingDB;
      this.m_DijkringId = dijkringId;
      this.m_DijkringNaam = m_OptimaliseRingDB.DijkringNaam(dijkringId);
      this.m_DeelNummer = Int32.Parse(deelNummer);
      this.m_Naam = naam;
      this.m_Trajecten = new List<DijkringTraject>();
      this.m_kansen = new Kansen(this.m_DijkringId, this.m_DijkringNaam, this.m_Naam, this.m_DeelNummer
        , instellingen.ZichtJaar, instellingen.OptimaleOverstromingskansenJaar);
      this.m_PmiddensPerScenario = new List<PmiddensPerScenario>();
      Initialize(instellingen);
    }

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:DijkringDeel"/> class.
    /// </summary>
    /// <param name="dijkringId">Dijkring identificatie</param>
    /// <param name="dijkringNaam">The dijkring naam.</param>
    /// <param name="deelNummer">The deel nummer.</param>
    /// <param name="naam">The naam.</param>
    public DijkringDeel(CultureInfo cultureInfo, Profile.Ini profile, Instellingen instellingen
      , OptimaliseRingDB optimaliseRingDB, string dijkringId, string dijkringNaam, string deelNummer, string naam)
    {
      this.m_CultureInfo = cultureInfo;
      this.m_Profile = profile;
      this.m_OptimaliseRingDB = optimaliseRingDB;
      this.m_DijkringId = dijkringId;
      this.m_DijkringNaam = dijkringNaam;
      this.m_DeelNummer = Convert.ToInt32(deelNummer);
      this.m_Naam = naam;
      this.m_Trajecten = new List<DijkringTraject>();
      this.m_kansen = new Kansen(this.m_DijkringId, this.m_DijkringNaam, this.m_Naam, this.m_DeelNummer
        , instellingen.ZichtJaar, instellingen.OptimaleOverstromingskansenJaar);
      this.m_PmiddensPerScenario = new List<PmiddensPerScenario>();
    }

    #endregion Constructors --------------------------------------------------

    #region Properties -------------------------------------------------------

    public OptimaliseRingDB OptimaliseRingDB
    {
      set { m_OptimaliseRingDB = value; }
    }

    /// <summary>
    /// Dijkring identificatie
    /// </summary>
    /// <value>dijkring identificatie</value>
    public string DijkringId
    {
      get { return m_DijkringId; }
    }

    /// <summary>
    /// Dijkringnaam.
    /// </summary>
    /// <value>Dijkringnaam</value>
    public string DijkringNaam
    {
      get { return m_DijkringNaam; }
    }

    /// <summary>
    /// Dijkringdeelnummer
    /// </summary>
    /// <value>Dijkringdeelnummer</value>
    public int DeelNummer
    {
      get { return m_DeelNummer; }
    }

    /// <summary>
    /// Naam.
    /// </summary>
    /// <value>Naam.</value>
    public string Naam
    {
      get { return m_Naam; }
    }

    /// <summary>
    /// Gets Kansen.
    /// </summary>
    /// <value>Kansen.</value>
    public Kansen Kansen
    {
      get { return m_kansen; }
    }

    /// <summary>
    /// Aantal getroffenen.
    /// </summary>
    /// <value>Aantal getroffenen.</value>
    public long AantalGetroffenen
    {
      get { return m_AantalGetroffenen; }
    }

    /// <summary>
    /// Aantal slachtoffers.
    /// </summary>
    /// <value>Aantal slachtoffers.</value>
    public long AantalSlachtoffers
    {
      get { return m_AantalSlachtoffers; }
    }

    /// <summary>
    /// Aantal inwoners.
    /// </summary>
    /// <value>Aantal inwoners.</value>
    public long AantalInwoners
    {
      get { return m_AantalInwoners; }
    }

    /// <summary>
    /// tijgingstempo schade per cm verhoging
    /// </summary>
    /// <value>tijgingstempo schade per cm verhoging</value>
    public double Zeta
    {
      get { return m_Zeta; }
    }

    /// <summary>
    /// Gets the Dijkringspecifieke aanpassingsfactor overstromingsschade
    /// </summary>
    /// <value>The dijkringspecifieke factor schade.</value>
    public double DijkringspecifiekeFactorSchade
    {
      get { return m_DijkringspecifiekeFactorSchade; }
    }

    /// <summary>
    /// Schade beginjaar
    /// </summary>
    /// <value>Schade beginjaar </value>
    public double Schade
    {
      get { return m_Schade; }
    }

    /// <summary>
    /// Tempo van de economische groei
    /// </summary>
    /// <value>Tempo van de economische groei</value>
    public double Gamma
    {
      get { return m_Gamma; }
    }

    /// <summary>
    /// Schaalparameter schade afhankelijk van waterstand
    /// </summary>
    /// <value>Schaalparameter schade afhankelijk van waterstand </value>
    public double Nu
    {
      get { return m_Nu; }
    }

    // <summary>
    /// Extra schade waterstandstoename klimaatverandering
    /// </summary>
    /// <value>Extra schade waterstandstoename klimaatverandering </value>
    public double Psi
    {
      get { return m_Psi; }
    }

    // <summary>
    /// Extra schade waterstandstoename klimaatverandering
    /// </summary>
    /// <value>Extra schade waterstandstoename klimaatverandering </value>
    public double Min_overschrijdingskans
    {
      get { return m_Min_overschrijdingskans; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is kering.
    /// </summary>
    /// <value><c>true</c> if this instance is kering; otherwise, <c>false</c>.</value>
    public bool IsKering
    {
      get { return m_IsKering; }
      set { m_IsKering = value; }
    }

    /// <summary>
    /// Lijst met dijkringtrajecten
    /// </summary>
    /// <value>Lijst met dijkringtrajecten</value>
    public List<DijkringTraject> Trajecten
    {
      get { return m_Trajecten; }
    }

    /// <summary>
    /// Optimaal grootste overstromingskans in zichtjaar.
    /// </summary>
    /// <value>Optimaal grootste overstromingskans in zichtjaar.</value>
    public double OptimaalGrootsteOverstromingskans
    {
      set { m_OptimaalGrootsteOverstromingskans = value; }
      get { return m_OptimaalGrootsteOverstromingskans; }
    }

    /// <summary>
    /// Optimaal kleinste overstromingskans in zichtjaar.
    /// </summary>
    /// <value>Optimaal kleinste overstromingskans in zichtjaar.</value>
    public double OptimaalKleinsteOverstromingskans
    {
      set { m_OptimaalKleinsteOverstromingskans = value; }
      get { return m_OptimaalKleinsteOverstromingskans; }
    }

    #endregion Properties ----------------------------------------------------

    #region Methods ----------------------------------------------------------
    /// <summary>
    /// Initialiseer
    /// </summary>
    public void Initialize(Instellingen instellingen)
    {
      double wettelijkeNorm;
      m_OptimaliseRingDB.Terugkeertijd(m_DijkringId, out wettelijkeNorm);
      this.m_kansen.WettelijkeNorm = wettelijkeNorm;

      double restrictiePmin;
      // Aantal inwoners en stijgingstempo schade per cm verhoging
      m_OptimaliseRingDB.DijkringDeelRecord(this.m_DijkringId, this.m_DeelNummer, out this.m_AantalInwoners, out restrictiePmin);
      this.m_kansen.RestrictiePmin = 1.0 / restrictiePmin;

      // Aantal slachtoffers, aantal getroffenen
      m_OptimaliseRingDB.RamingVoorSlachtoffersRecord(m_DijkringId, m_DeelNummer,
        instellingen.RamingVoorSlachtoffers, ref m_AantalSlachtoffers, ref m_AantalGetroffenen);

      // Tempo van de economische groei
      m_OptimaliseRingDB.EconomischScenarioRecord(m_DijkringId, m_DeelNummer, instellingen.EconomischScenario, ref m_Gamma);

      // Schade beginjaar [Meuro]
      m_OptimaliseRingDB.ScenarioVoorHoeveelheidSchadeData(m_DijkringId, m_DeelNummer,
        instellingen.ScenarioVoorHoeveelheidSchade,
        instellingen.SchadeFunctie,
        ref m_Schade);

      // Schaalparameter schade afhankelijk van waterstand [1/cm]
      m_OptimaliseRingDB.SchadeFunctieData(m_DijkringId, m_DeelNummer,
        instellingen.SchadeFunctie, ref m_Nu, ref m_Zeta, ref m_Psi);

      // klimaatscenario en waterstand
      m_OptimaliseRingDB.SchadeKlimaatData(m_DijkringId, m_DeelNummer,
        instellingen.KlimaatScenarioEnFysischMaxAfvoer, ref m_Min_overschrijdingskans);


      m_OptimaliseRingDB.DijkringspecifiekeFactorSchadeData(m_DijkringId, m_DeelNummer,
        instellingen.DijkringspecifiekeFactorSchade, ref m_DijkringspecifiekeFactorSchade);

      // Maak lijst met traject informatie
      m_Trajecten.Clear();
      SortedList dijkringTrajectenRecords = m_OptimaliseRingDB.DijkringTrajecten(m_DijkringId, m_DeelNummer).ToSortedList();

      for (int i = 0; i < dijkringTrajectenRecords.Count; i++)
      {
        SortedList dijkringTrajectRecord = (SortedList)dijkringTrajectenRecords.GetByIndex(i);

        DijkringTraject traject = new DijkringTraject(m_OptimaliseRingDB, m_DijkringId, m_DeelNummer, m_Trajecten.Count + 1,
          dijkringTrajectRecord["Naam"].ToString(), instellingen.KlimaatScenarioEnFysischMaxAfvoer, instellingen.ParametersKostenfunctie);

        m_Trajecten.Add(traject);
      }

      //toevoegen keringen alsof het extra trajecten zijn
      if (m_OptimaliseRingDB.KeringHasRecords())
      {
        //er kan meer dan 1 kering in een deel zitten
        //haal voor elk deel van de tabel de valide keringen op met hun percentage
        AddKeringAsTraject(instellingen, dijkringTrajectenRecords, "Dijkring1", "DijkringDeel1");
        AddKeringAsTraject(instellingen, dijkringTrajectenRecords, "Dijkring2", "DijkringDeel2");
      }
    }

    private void AddKeringAsTraject(Instellingen instellingen, SortedList dijkringTrajectenRecords, string ring, string deel)
    {
      //loop door beide reeksen in de keringentabel om de percentages te vinden
      InstellingenKeringen instellingenKeringen = instellingen.Keringenparameters.Values[0];
      int percentage = 0;

      //onderzoek helft tabel, vind alle keringen
      SortedList keringInDeel = m_OptimaliseRingDB.KeringInDijkringdeel(ring, deel, m_DijkringId, m_DeelNummer);
      for (int i = 0; i < keringInDeel.Count; i++)
      {
        SortedList keringInDeel1 = (SortedList)keringInDeel.GetByIndex(i);  //reduceer
        int test = ConvertString.ToInt32(keringInDeel1["DijkringTraject"].ToString());
        //zet percentage in Keringenparameters
        foreach (KeyValuePair<int, InstellingenKeringen> InstellingenKeringen in instellingen.Keringenparameters)
        {
          int index = ConvertString.ToInt32(InstellingenKeringen.Value.NaamKering) + 100;
          if (index == test)
          {
            if (ring == "Dijkring1")
            {
              percentage = InstellingenKeringen.Value.Percentage1;
            }
            else
            {
              percentage = InstellingenKeringen.Value.Percentage2;
            }

          }
        }
        if (percentage > 0)
        {
          int temp = (int)keringInDeel1["DijkringTraject"];
          DijkringTraject traject = new DijkringTraject(m_OptimaliseRingDB, temp, 1, m_Trajecten.Count + 1,
            keringInDeel1["Naam"].ToString(), instellingen.KlimaatScenarioEnFysischMaxAfvoer, instellingen.ParametersKostenfunctie, true);

          //voeg toe en bewaar percentage
          m_Trajecten.Add(traject);
          traject.Percentage = percentage;

          //voeg toe aan traject records
          dijkringTrajectenRecords.Add(dijkringTrajectenRecords.Count, keringInDeel1);
        }
      }
    }


    /// <summary>
    /// Lees resultaten
    /// </summary>
    /// <param name="xmlReader">The XML reader.</param>
    public void Read(XmlReader xmlReader)
    {
      int scenarioIndex = 0;

      this.m_kansen.WettelijkeNorm = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "WettelijkeNorm"));
      this.m_kansen.RestrictiePmin = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "RestrictiePmin"));

      this.m_kansen.OverstromingskansInZichtjaar = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "OverstromingskansInZichtjaar"));
      this.m_kansen.OverstromingskansInOptimaleOverstromingskansJaar = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "OverstromingskansInOptimaleOverstromingskansJaar"));
      this.m_kansen.OptimaleOverstromingskansInZichtjaar = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "OptimaleOverstromingskansInZichtjaar"));
      this.m_kansen.OptimaleOverstromingskansInOptimaleOverstromingskansJaar = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "OptimaleOverstromingskansInOptimaleOverstromingskansJaar"));
      this.m_kansen.OptimaleOverschrijdingskansInZichtjaar = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "OptimaleOverschrijdingskansInZichtjaar"));
      this.m_kansen.OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar"));
      this.m_kansen.ContantewaardeInvesteringenEnSchade = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "ContantewaardeInvesteringenEnSchade"));

      this.m_kansen.Zichtjaar = ConvertString.ToInt32(OptimaliseRing.General.Xml.ReadElement(xmlReader, "Zichtjaar"));
      this.m_kansen.OptimaleOverstromingskansenJaar = ConvertString.ToInt32(OptimaliseRing.General.Xml.ReadElement(xmlReader, "OptimaleOverstromingskansenJaar"));

      this.m_kansen.TotaalKostenInvesteringen = 0.0;
      for (int indexTraject = 0; indexTraject < this.Trajecten.Count; indexTraject++)
      {
        DijkringTraject dijkringTraject = this.Trajecten[indexTraject];
        if (dijkringTraject != null)
        {
          this.m_kansen.TotaalKostenInvesteringen += dijkringTraject.TotaleKostenInvesteringen[scenarioIndex];
        }
      }

      this.m_kansen.EersteInvesteringJaar = ConvertString.ToInt32(OptimaliseRing.General.Xml.ReadElement(xmlReader, "EersteInvesteringJaar"));
      this.m_kansen.OptimaleOverstromingskansInEersteInversteringjaar = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "OptimaleOverstromingskansInEersteInversteringjaar"));

      this.m_Gamma = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "Gamma"));
      this.m_Psi = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "Psi"));
      this.m_Min_overschrijdingskans = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "MinimaleOverschrijdingskans"));

      ReadTrajecten(xmlReader);

      xmlReader.Read();
      if ((xmlReader.Name == "Matrix") && xmlReader.IsStartElement())
      {
        // Read matrix data
        this.m_kansen.MatrixData = ReadMatrix(xmlReader);
      }
    }

    private double GetOverschrijdingskans(int jaar, double optimaleOverstromingskans, double factorKans)
    {
      double retval = 0.0;

      if (Trajecten.Count == 1)
      {
        // Ophalen eerste traject
        DijkringTraject traject = (DijkringTraject)m_Trajecten[0];

        // Bereken overschrijdingskans voor 1 traject
        double kans = (traject.GetOverschrijdingskansInOpgegevenJaar(jaar, factorKans)
          / (factorKans * traject.Factor))
          * Math.Pow((1.0 / optimaleOverstromingskans) / traject.GetOverstromingskansInOpgegevenJaar(jaar, factorKans)
          , traject.AlphaOverschrijdingskans / traject.AlphaOverstromingskans);

        retval = 1.0 / Math.Max(kans, Min_overschrijdingskans);

      }
      else if (Trajecten.Count > 1)
      {
        double trKans = 0;

        // Bereken overschrijdingskans voor meerdere trajecten
        foreach (DijkringTraject traject in m_Trajecten)
        {

          double kans = (traject.GetOverschrijdingskansInOpgegevenJaar(jaar, factorKans)
            / (factorKans * traject.Factor))
            * Math.Pow((1.0 / optimaleOverstromingskans) / traject.GetOverstromingskansInOpgegevenJaar(jaar, factorKans)
            , traject.AlphaOverschrijdingskans / traject.AlphaOverstromingskans);

          trKans += kans;

        }
        double temp = trKans / Trajecten.Count;

        retval = 1.0/(Math.Max(temp, Min_overschrijdingskans));
      }
      return retval;
    }

    /// <summary>
    /// Lees de  traject resultaten
    /// </summary>
    /// <param name="xmlReader">The XML reader.</param>
    private void ReadTrajecten(XmlReader xmlReader)
    {
      xmlReader.Read();
      if ((xmlReader.Name == "Trajecten") && xmlReader.IsStartElement())
      {
        m_Trajecten.Clear();

        while (xmlReader.Read())
        {
          // <Traject>
          if ((xmlReader.Name == "Traject") && xmlReader.IsStartElement())
          {
            string trajectNaam = "";
            if (xmlReader.HasAttributes)
            {
              while (xmlReader.MoveToNextAttribute())
              {
                if (xmlReader.Name == "Naam")
                {
                  trajectNaam = xmlReader.Value;
                }
              }

              DijkringTraject dijkringTraject = new DijkringTraject(
                   m_OptimaliseRingDB
                 , DijkringId
                 , DijkringNaam
                 , Naam
                 , m_Trajecten.Count + 1
                 , trajectNaam
                 , m_DeelNummer
                 );

              dijkringTraject.Read(xmlReader);

              m_Trajecten.Add(dijkringTraject);
            }
          }
          if ((xmlReader.Name == "Trajecten") && !xmlReader.IsStartElement())
          {
            return;
          }
        }
      }
    }

    /// <summary>
    /// Schrijf de resultaten
    /// </summary>
    /// <param name="xmlWriter">The XML writer.</param>
    public void Write(XmlWriter xmlWriter)
    {
      xmlWriter.WriteStartElement("DijkringDeel", null);
      xmlWriter.WriteAttributeString("DijkringId", this.m_kansen.DijkringId);
      xmlWriter.WriteAttributeString("DijkringNaam", this.m_kansen.DijkringNaam);
      xmlWriter.WriteAttributeString("Deel", this.m_kansen.Deel);
      xmlWriter.WriteAttributeString("DeelNummer", this.m_kansen.DeelNummer.ToString());

      xmlWriter.WriteElementString("WettelijkeNorm", this.m_kansen.WettelijkeNorm.ToString());
      xmlWriter.WriteElementString("RestrictiePmin", this.m_kansen.RestrictiePmin.ToString());
      xmlWriter.WriteElementString("OverstromingskansInZichtjaar", this.m_kansen.OverstromingskansInZichtjaar.ToString());
      xmlWriter.WriteElementString("OverstromingskansInOptimaleOverstromingskansJaar"
        , this.m_kansen.OverstromingskansInOptimaleOverstromingskansJaar.ToString());
      xmlWriter.WriteElementString("OptimaleOverstromingskansInZichtjaar"
        , this.m_kansen.OptimaleOverstromingskansInZichtjaar.ToString());
      xmlWriter.WriteElementString("OptimaleOverstromingskansInOptimaleOverstromingskansJaar"
        , this.m_kansen.OptimaleOverstromingskansInOptimaleOverstromingskansJaar.ToString());
      xmlWriter.WriteElementString("OptimaleOverschrijdingskansInZichtjaar"
        , this.m_kansen.OptimaleOverschrijdingskansInZichtjaar.ToString());
      xmlWriter.WriteElementString("OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar"
        , this.m_kansen.OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar.ToString());
      xmlWriter.WriteElementString("ContantewaardeInvesteringenEnSchade"
        , this.m_kansen.ContantewaardeInvesteringenEnSchade.ToString("F3"));

      xmlWriter.WriteElementString("Zichtjaar", this.m_kansen.Zichtjaar.ToString());
      xmlWriter.WriteElementString("OptimaleOverstromingskansenJaar", this.m_kansen.OptimaleOverstromingskansenJaar.ToString());

      xmlWriter.WriteElementString("EersteInvesteringJaar", this.m_kansen.EersteInvesteringJaar.ToString());
      xmlWriter.WriteElementString("OptimaleOverstromingskansInEersteInversteringjaar"
        , this.m_kansen.OptimaleOverstromingskansInEersteInversteringjaar.ToString());
      xmlWriter.WriteElementString("Gamma", this.m_Gamma.ToString());
      xmlWriter.WriteElementString("Psi", this.m_Psi.ToString());
      xmlWriter.WriteElementString("MinimaleOverschrijdingskans", this.m_Min_overschrijdingskans.ToString());

      xmlWriter.WriteStartElement("Trajecten", null);
      for (int i = 0; i < Trajecten.Count; i++)
      {
        DijkringTraject dijkringTraject = (DijkringTraject)Trajecten[i];
        dijkringTraject.Write(xmlWriter, i + 1);
      }
      xmlWriter.WriteEndElement();
      xmlWriter.Flush();

      WriteMatrix(xmlWriter);

      xmlWriter.WriteEndElement();
      xmlWriter.Flush();

    }

    /// <summary>
    /// Lees de resultaten-matrix.
    /// </summary>
    /// <param name="xmlReader">The XML reader.</param>
    /// <returns></returns>
    public DoubleMatrix ReadMatrix(XmlReader xmlReader)
    {
      DoubleMatrix matrix = null;

      if (xmlReader.HasAttributes)
      {
        int rows = 0;
        int columns = 0;
        while (xmlReader.MoveToNextAttribute())
        {
          if (xmlReader.Name == "Rows")
          {
            rows = ConvertString.ToInt32(xmlReader.Value);
          }
          else if (xmlReader.Name == "Columns")
          {
            columns = ConvertString.ToInt32(xmlReader.Value);
          }
        }
        matrix = new DoubleMatrix(rows, columns);
      }
      else
      {
        throw new ApplicationException(string.Format("Matrix attributen rows en columns  verwacht"));
      }

      int row = -1;
      while (xmlReader.Read())
      {
        if ((xmlReader.Name == "Row") && xmlReader.IsStartElement())
        {
          while (xmlReader.MoveToNextAttribute())
          {
            if (xmlReader.Name == "Number")
            {
              row = Int32.Parse(xmlReader.Value) - 1;
            }
          }
        }
        else if ((xmlReader.Name == "Column") && xmlReader.IsStartElement())
        {
          int col = -1;
          string value = "";
          while (xmlReader.MoveToNextAttribute())
          {
            if (xmlReader.Name == "Number")
            {
              col = Int32.Parse(xmlReader.Value) - 1;
            }
            else if (xmlReader.Name == "Value")
            {
              value = xmlReader.Value;
            }
          }
          matrix[row, col] = ConvertString.ToDouble(value);
        }
        if ((xmlReader.Name == "Matrix") && !xmlReader.IsStartElement())
        {
          break;
        }
      }
      return matrix;
    }

    /// <summary>
    /// Schrijf de resultaten-matrix
    /// </summary>
    /// <param name="xmlWriter">The XML writer.</param>
    public void WriteMatrix(XmlWriter xmlWriter)
    {
      xmlWriter.WriteStartElement("Matrix", null);
      xmlWriter.WriteAttributeString("Rows", this.m_kansen.MatrixData.Rows.ToString());
      xmlWriter.WriteAttributeString("Columns", this.m_kansen.MatrixData.Cols.ToString());
      for (int row = 0; row < this.m_kansen.MatrixData.Rows; row++)
      {
        xmlWriter.WriteStartElement("Row", null);
        xmlWriter.WriteAttributeString("Number", (row + 1).ToString());
        for (int col = 0; col < this.m_kansen.MatrixData.Cols; col++)
        {
          xmlWriter.WriteStartElement("Column", null);
          xmlWriter.WriteAttributeString("Number", (col + 1).ToString());
          xmlWriter.WriteAttributeString("Value", this.m_kansen.MatrixData[row, col].ToString());
          xmlWriter.WriteEndElement();
        }
        xmlWriter.WriteEndElement();
      }
      xmlWriter.WriteEndElement();
    }

    /// <summary>
    /// Maak het overzichtsbestand
    /// </summary>
    /// <param name="instellingen">The instellingen.</param>
    /// <param name="berekeningenMap">The berekeningen map.</param>
    /// <param name="zichtJaar">Zichtjaar.</param>
    /// <param name="schadeJaar">The schade jaar.</param>
    /// <param name="kansjaar">The kansjaar.</param>
    /// <param name="optimaleOverstromingskansenJaar">The optimale overstromingskansen jaar.</param>
    public void Overzichtsbestand(Instellingen instellingen, string berekeningenMap, int zichtJaar
      , int schadeJaar, int kansjaar, int optimaleOverstromingskansenJaar, double factorKans)
    {
      //string bestandsNaam = DijkringNaam + "-" + DeelNummer.ToString() + ".txt";
      string bestandsNaam = this.DijkringId + "-" + this.DeelNummer + " " + DijkringNaam.ToString() + ".txt";

      bestandsNaam = bestandsNaam.Replace("/", "-");

      string overzichtsbestand = Path.Combine(berekeningenMap, bestandsNaam);
      if (File.Exists(overzichtsbestand))
      {
        File.Delete(overzichtsbestand);
      }
      WriteOverzichtsbestand(instellingen, overzichtsbestand, zichtJaar
        , schadeJaar, kansjaar, optimaleOverstromingskansenJaar, factorKans);
    }

    /// <summary>
    /// Overzichtsbestand
    /// </summary>
    /// <param name="instellingen">The instellingen.</param>
    /// <param name="overzichtsbestand">Het overzichtsbestand.</param>
    /// <param name="zichtJaar">The zicht jaar.</param>
    /// <param name="schadeJaar">The schade jaar.</param>
    /// <param name="kansjaar">The kansjaar.</param>
    /// <param name="optimaleOverstromingskansenJaar">The optimale overstromingskansen jaar.</param>
    /// <param name="factorKans">The factor kans.</param>
    private void WriteOverzichtsbestand(Instellingen instellingen, string overzichtsbestand
      , int zichtJaar, int schadeJaar, int kansjaar
      , int optimaleOverstromingskansenJaar, double factorKans)
    {
      using (StreamWriter writer = new StreamWriter(overzichtsbestand, true))
      {

        // rekenen met parameteronzekerheid?
        bool rekenenMetParameteronzekerheid = instellingen.Parametersonzekerheid && instellingen.Scenarioparameters.Count > 0;

        string puntjes = rekenenMetParameteronzekerheid ? " ..." : string.Empty;

        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UDeelVanDijk").ToString().Replace("\\t", "\t"), DeelNummer, DijkringId, DijkringNaam));
        writer.WriteLine();

        // Instellingen
        instellingen.Overzichtsbestand(writer, m_OptimaliseRingDB.Filename);
        writer.WriteLine();

        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UGekozenInstellingenDijkringdeel").ToString().Replace("\\t", "\t")));
        writer.WriteLine();

        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UAantalInwoners").ToString().Replace("\\t", "\t"), m_AantalInwoners));
        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UAantalGetroffenen").ToString().Replace("\\t", "\t"), m_AantalGetroffenen));
        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UAantalSlachtoffers").ToString().Replace("\\t", "\t"), m_AantalSlachtoffers));

        // Toename schadepotentieel (gamma en psi)

        double gammaPercentage = this.m_Gamma * 100.0;
        string volgendeGammaPercentage = string.Empty;
        double psiWaarde = this.m_Psi;
        double min_overschrijdingskansWaarde=this.m_Min_overschrijdingskans;
        string volgendeMin_overschrijdingskans = string.Empty;

        if (rekenenMetParameteronzekerheid)
        {
          //gamma's
          string seperator = string.Empty;
          foreach (KeyValuePair<int, InstellingenOnzekerheid> instellingenOnzekerheid in instellingen.Scenarioparameters)
          {
            double gammaScenario = 0.0;
            m_OptimaliseRingDB.EconomischScenarioRecord(m_DijkringId, m_DeelNummer
              , instellingenOnzekerheid.Value.EconomischScenario, ref gammaScenario);

            if (instellingenOnzekerheid.Key > 0)
            {
              volgendeGammaPercentage += string.Format("{1}{0:F2}", gammaScenario * 100, seperator);
              seperator = ";";
            }
          }

          //psi's
          seperator = "";
          foreach (KeyValuePair<int, InstellingenOnzekerheid> instellingenOnzekerheid in instellingen.Scenarioparameters)
          {
            double min_overschrijdingskansScenario = 0.0;
            m_OptimaliseRingDB.SchadeKlimaatData(m_DijkringId, m_DeelNummer
              , instellingenOnzekerheid.Value.KlimaatScenarioEnFysischMaxAfvoer, ref min_overschrijdingskansScenario);

            if (instellingenOnzekerheid.Key > 0)
            {
              volgendeMin_overschrijdingskans += string.Format("{1}{0:F5}", min_overschrijdingskansScenario, seperator);
              seperator = ";";
            }
          }
        }

        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UToenameSchadepotentieel").ToString().Replace("\\t", "\t")
          , gammaPercentage, "[%/" + m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Jaar").ToString() + "]" + puntjes));

        if (volgendeGammaPercentage != string.Empty)
        { writer.WriteLine(string.Format("   Scenario[{0}]", volgendeGammaPercentage)); }


        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UWaterstandstoenameKlimaat").ToString().Replace("\\t", "\t")
          , psiWaarde, "[1/cm]"));

        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UWaterstandstoenameKlimaatOvkans").ToString().Replace("\\t", "\t")
          , min_overschrijdingskansWaarde, "[1/" + m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Jaar").ToString() + "]" + puntjes));

        if (volgendeMin_overschrijdingskans != string.Empty)
        { writer.WriteLine(string.Format("   Scenario[{0}]", volgendeMin_overschrijdingskans)); }


        if (m_Nu != 0)
        {
          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "USchaalparameterAbsoluut").ToString().Replace("\\t", "\t"), m_Nu, "[1/cm]"));
        }
        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "USchadeOpgegevenjaar").ToString().Replace("\\t", "\t"), schadeJaar, m_Schade, "[M€]"));
        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UStijgTempoVerhoging").ToString().Replace("\\t", "\t"), m_Zeta, "[1/cm]"));
        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UDijkringspecifiekeFactorSchade").ToString().Replace("\\t", "\t"), this.m_DijkringspecifiekeFactorSchade, "[-]"));


        // Veiligheidsnorm op (met restrictie)
        if (instellingen.Veiligheidsnorm == 2)
        {
          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UVeiligheidsnorm").ToString().Replace("\\t", "\t"), 1.0 / this.m_kansen.RestrictiePmin, "[1/jaar]"));
        }

        writer.WriteLine();

        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UDijkringTrajectBestaatUit").ToString().Replace("\\t", "\t"), Naam, Trajecten.Count));
        writer.WriteLine();
        writer.WriteLine(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UGekozenInstellingenTrajecten").ToString().Replace("\\t", "\t"));
        writer.WriteLine();
        for (int i = 0; i < Trajecten.Count; i++)
        {
          DijkringTraject dijkringTraject = Trajecten[i];

          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UDijkringtraject").ToString().Replace("\\t", "\t"), i + 1, dijkringTraject.Naam));
          writer.WriteLine();

          if (m_Language.GetValue("Captions:" + m_CultureInfo.Name, "USchaalparameterRelatief2").ToString().Trim().Length > 0)
          {
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "USchaalparameterRelatief2").ToString().Replace("\\t", "\t")));
          }

          double alphaOverstromingskans = dijkringTraject.AlphaOverstromingskans;
          string volgendeAlphaOverstromingskans = string.Empty;

          double eta = dijkringTraject.Eta;
          string volgendeEta = string.Empty;

          double pOverstromingskans = dijkringTraject.P0Overstromingskans;
          string volgendePOverstromingskans = string.Empty;

          double alphaOverschrijdingskans = dijkringTraject.AlphaOverschrijdingskans;
          string volgendeAlphaOverschrijdingskans = string.Empty;

          double pOverschrijdingskans = dijkringTraject.P0Overschrijdingskans;
          string volgendePOverschrijdingskans = string.Empty;

          if (rekenenMetParameteronzekerheid)
          {
            string seperator = string.Empty;
            foreach (KeyValuePair<int, InstellingenOnzekerheid> instellingenOnzekerheid in instellingen.Scenarioparameters)
            {
              if (!dijkringTraject.Iskering)
              {
                TrajectOnzekerheid trajectOnzekerheid = this.GetTrajectOnzekerheid(dijkringTraject
                  , instellingenOnzekerheid.Value.KlimaatScenarioEnFysischMaxAfvoer, zichtJaar, kansjaar, factorKans);

                if (instellingenOnzekerheid.Key == 0)
                {
                  alphaOverstromingskans = trajectOnzekerheid.AlphaOverstromingskans;
                  eta = trajectOnzekerheid.Eta;
                  pOverstromingskans = trajectOnzekerheid.P0Overstromingskans;
                  alphaOverschrijdingskans = trajectOnzekerheid.AlphaOverschrijdingskans;
                  pOverschrijdingskans = trajectOnzekerheid.P0Overschrijdingskans;
                }
                else
                {
                  volgendeAlphaOverstromingskans += string.Format("{1}{0:F4}", trajectOnzekerheid.AlphaOverstromingskans, seperator);
                  volgendeEta += string.Format("{1}{0:F2}", trajectOnzekerheid.Eta, seperator);
                  volgendePOverstromingskans += string.Format("{1}{0:F6}", trajectOnzekerheid.P0Overstromingskans, seperator);
                  volgendeAlphaOverschrijdingskans += string.Format("{1}{0:F4}", trajectOnzekerheid.AlphaOverschrijdingskans, seperator);
                  volgendePOverschrijdingskans += string.Format("{1}{0:F6}", trajectOnzekerheid.P0Overschrijdingskans, seperator);

                  seperator = ";";
                }
              }
              else
              {
                KeringOnzekerheid keringOnzekerheid = this.GetKeringOnzekerheid(dijkringTraject
                  , instellingenOnzekerheid.Value.KlimaatScenarioEnFysischMaxAfvoer, zichtJaar, kansjaar, factorKans);

                if (instellingenOnzekerheid.Key == 0)
                {
                  alphaOverstromingskans = keringOnzekerheid.AlphaOverstromingskans;
                  eta = keringOnzekerheid.Eta;
                  pOverstromingskans = keringOnzekerheid.P0Overstromingskans;
                  alphaOverschrijdingskans = keringOnzekerheid.AlphaOverschrijdingskans;
                  pOverschrijdingskans = keringOnzekerheid.P0Overschrijdingskans;
                }
                else
                {
                  volgendeAlphaOverstromingskans += string.Format("{1}{0:F4}", keringOnzekerheid.AlphaOverstromingskans, seperator);
                  volgendeEta += string.Format("{1}{0:F2}", keringOnzekerheid.Eta, seperator);
                  volgendePOverstromingskans += string.Format("{1}{0:F6}", keringOnzekerheid.P0Overstromingskans, seperator);
                  volgendeAlphaOverschrijdingskans += string.Format("{1}{0:F4}", keringOnzekerheid.AlphaOverschrijdingskans, seperator);
                  volgendePOverschrijdingskans += string.Format("{1}{0:F6}", keringOnzekerheid.P0Overschrijdingskans, seperator);

                  seperator = ";";
                }
              }
            }
          }

          // Schaalparameter relatieve waterstand (alpha)
          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "USchaalparameterRelatief").ToString().Replace("\\t", "\t"), alphaOverstromingskans, "[1/cm]" + puntjes));
          if (volgendeAlphaOverstromingskans != string.Empty)
          { writer.WriteLine(string.Format("   Scenario[{0}]", volgendeAlphaOverstromingskans)); }

          // Structurele stijging relatieve waterstand (eta)
          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UStructureleStijging").ToString().Replace("\\t", "\t"), eta, "[cm/" + m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Jaar").ToString() + "]" + puntjes));
          if (volgendeAlphaOverstromingskans != string.Empty)
          { writer.WriteLine(string.Format("   Scenario[{0}]", volgendeEta)); }

          // Dijkhoogte in {0} volgens database
          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UDijkhoogte").ToString().Replace("\\t", "\t"), zichtJaar, dijkringTraject.H0, "[cm+NAP]"));

          // Overstromingskans in {0} volgens database
          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UOverstromingskansDatabase").ToString().Replace("\\t", "\t")
            , kansjaar, pOverstromingskans, "[1/" + m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Jaar").ToString() + "]" + puntjes));
          if (volgendePOverstromingskans != string.Empty)
          { writer.WriteLine(string.Format("   Scenario[{0}]", volgendePOverstromingskans)); }

          if (instellingen.TypeKostenfunctie == (int)OptimaliseRing.Aimms.InvestmentCostFunctions.Exponential)
          {
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "USchaalparameterVerhoging").ToString().Replace("\\t", "\t"), dijkringTraject.lambda_exp, "[1/cm]"));
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UVariabeleKostenInvestering").ToString().Replace("\\t", "\t"), dijkringTraject.b_exp, "[M€/cm]"));
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UVasteKostenInvestering").ToString().Replace("\\t", "\t"), dijkringTraject.C_exp, "[M€]"));
          }
          else
          {
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UparameterKostenAKwad").ToString().Replace("\\t", "\t"), dijkringTraject.a_kwad, "[M€/cm^2]"));
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UparameterKostenBKwad").ToString().Replace("\\t", "\t"), dijkringTraject.b_kwad, "[M€/cm]"));
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UparameterKostenCKwad").ToString().Replace("\\t", "\t"), dijkringTraject.c_kwad, "[M€]"));
          }

          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UPercentageOnderhoudskosten").ToString().Replace("\\t", "\t"), dijkringTraject.Omega * 100, "[%/" + m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Jaar").ToString() + "]"));
          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UDijkringdeelFactor").ToString().Replace("\\t", "\t"), dijkringTraject.Factor, "[-]"));

          if (m_Language.GetValue("Captions:" + m_CultureInfo.Name, "USchaalparameterRelatiefOverschrijding2").ToString().Trim().Length > 0)
          {
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "USchaalparameterRelatiefOverschrijding2").ToString().Replace("\\t", "\t")));
          }

          // Schaalparameter relatieve waterstand (alpha overschrijdingskans)
          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "USchaalparameterRelatiefOverschrijding").ToString().Replace("\\t", "\t"), alphaOverschrijdingskans, "[1/cm]" + puntjes));
          if (volgendeAlphaOverschrijdingskans != string.Empty)
          { writer.WriteLine(string.Format("   Scenario[{0}]", volgendeAlphaOverschrijdingskans)); }

          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UOverschrijdingskansDatabase").ToString().Replace("\\t", "\t"), kansjaar, pOverschrijdingskans, "[1/" + m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Jaar").ToString() + "]" + puntjes));
          if (volgendePOverschrijdingskans != string.Empty)
          { writer.WriteLine(string.Format("   Scenario[{0}]", volgendePOverschrijdingskans)); }

          if (dijkringTraject.Iskering)
          {
              writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UKostentoedelingPercentage").ToString().Replace("\\t", "\t"), dijkringTraject.Percentage, "[%]"));
          }

          writer.WriteLine();
        }

        // Uitvoer resulaten
        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UResultaten").ToString().Replace("\\t", "\t")));
        writer.WriteLine();

        if (rekenenMetParameteronzekerheid)
        {
          // Voor alle kosten zijn gegevens van het eerste parameterscenario gebruikt!
          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UMeldingUitvoer").ToString().Replace("\\t", "\t")));
          writer.WriteLine();
        }

        double totaalInvesteringen = 0;

        //double toedelingkostenInhaal = 0;
        //double toedelingkostenKlimaat = 0;
        //double toedelingkostenEconomie = 0;

        int scenarioIndex = 0;
        for (int indexTraject = 0; indexTraject < Trajecten.Count; indexTraject++)
        {

          // Ophalen traject
          DijkringTraject dijkringTraject = Trajecten[indexTraject];

          // Dijkringtrajectnaam
          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UDijkringtraject").ToString().Replace("\\t", "\t"), indexTraject + 1, dijkringTraject.Naam));
          writer.WriteLine();

          if (dijkringTraject.Investeringen.Count > 0)
          {

            // Bepaal overstromingskans in jaar voor eerste investering
            double overstromingskansInJaarVoorInvestering = 0.0;

            if (this.m_kansen.EersteInvesteringJaar == zichtJaar)
            {
              double overstromingskans = dijkringTraject.P0Overstromingskans
                * NMathFunctions.Exp(dijkringTraject.AlphaOverstromingskans * dijkringTraject.Eta
                * (zichtJaar - kansjaar));

              overstromingskansInJaarVoorInvestering = overstromingskans
                * factorKans * dijkringTraject.Factor;
            }
            else
            {
              int indexJaarVoorInvestering
                = Math.Max(0, (this.m_kansen.EersteInvesteringJaar - 1) - zichtJaar);

              overstromingskansInJaarVoorInvestering = this.m_kansen.MatrixData[
                indexJaarVoorInvestering, Convert.ToInt32(MatrixKolom.AANTAL) + indexTraject];
            }

            int indexJaarVanInvestering = this.m_kansen.EersteInvesteringJaar - zichtJaar;

            double overstromingskansJaarVanInvestering = this.m_kansen.MatrixData[
              indexJaarVanInvestering, Convert.ToInt32(MatrixKolom.AANTAL) + indexTraject];

            // Bepaal fractie van totaal over klimaat en economie
            OptimaliseRing.Core.DijkringTraject.Kostensplitsing kostensplitsing = new DijkringTraject.Kostensplitsing();
            if (dijkringTraject.Investeringen.Count > 0)
            {
              kostensplitsing
                = dijkringTraject.GetKostensplitsing(scenarioIndex, this.m_kansen.WettelijkeNorm, this.Gamma
                , this.m_Psi, this.MaxEta()
                , this.m_kansen.OptimaleOverstromingskansInZichtjaar
                , overstromingskansInJaarVoorInvestering, overstromingskansJaarVanInvestering
                , this.m_kansen.EersteInvesteringJaar, zichtJaar, instellingen.FactorGroeiSchade);
            }

            int teller = 1;
            foreach (Investering investering in dijkringTraject.Investeringen)
            {
              // Jaar en hoogte van investeringen
              writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UEersteInvestering").ToString().Replace("\\t", "\t"), teller++, investering.Jaar, investering.Hoogte, "[cm]"));
            }

            // EERSTE INVESTERING

            // Kosten van de eerste verhoging
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name
              , "UKostenEersteVerhoging").ToString().Replace("\\t", "\t")
              , kostensplitsing.KostenInvestering[0].ToString("F1"), "[M€]", dijkringTraject.Investeringen[0].Jaar));

            // Kosten eerste investering inhaal van de normen
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name
              , "UEersteInvesteringInhaal").ToString().Replace("\\t", "\t"), zichtJaar
              , kostensplitsing.Inhaal[0].ToString("F1"), "[M€]"));

            // Kosten eerste investering t.g.v. klimaatverandering
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:"
              + m_CultureInfo.Name, "UEersteInvesteringKlimaat").ToString().Replace("\\t", "\t"), zichtJaar
              , kostensplitsing.KlimaatInvestering[0].ToString("F1"), "[M€]"));

            // Kosten eerste investering t.g.v. economische groei
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:"
              + m_CultureInfo.Name, "UEersteInvesteringEconomie").ToString().Replace("\\t", "\t")
              , zichtJaar, kostensplitsing.EconomieInvestering[0].ToString("F1"), "[M€]"));

            // Kosten van de eerste verhoging (contante waarde in zichtjaar)
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name
              , "UVerdisconteerdeKostenEersteVerhoging").ToString().Replace("\\t", "\t"), zichtJaar
              , dijkringTraject.Investeringen[0].VerdisconteerdeKosten[0].ToString("F1"), "[M€]"));


            // TWEEDE INVESTERING
            if (dijkringTraject.Investeringen.Count > 1)
            {
              // Kosten van de tweede verhoging
              writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name
                , "UKostenTweedeVerhoging").ToString().Replace("\\t", "\t")
                , kostensplitsing.KostenInvestering[1].ToString("F1"), "[M€]", dijkringTraject.Investeringen[1].Jaar));

              // Kosten eerste investering t.g.v. klimaatverandering
              writer.WriteLine(string.Format(m_Language.GetValue("Captions:"
                + m_CultureInfo.Name, "UTweedeInvesteringKlimaat").ToString().Replace("\\t", "\t"), zichtJaar
                , kostensplitsing.KlimaatInvestering[1].ToString("F1"), "[M€]"));

              // Kosten eerste investering t.g.v. economische groei
              writer.WriteLine(string.Format(m_Language.GetValue("Captions:"
                + m_CultureInfo.Name, "UTweedeInvesteringEconomie").ToString().Replace("\\t", "\t")
                , zichtJaar, kostensplitsing.EconomieInvestering[1].ToString("F1"), "[M€]"));

              // Kosten van de eerste verhoging (contante waarde in zichtjaar)
              writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name
                , "UVerdisconteerdeKostenTweedeVerhoging").ToString().Replace("\\t", "\t"), zichtJaar
                , dijkringTraject.Investeringen[1].VerdisconteerdeKosten[0].ToString("F1"), "[M€]"));
            }

            // Totale kosten investeringen (contante waarde in [jaar])
            double totaleEersteInversteringContant = kostensplitsing.TotaleKostenInvesteringen; //dijkringTraject.TotaleKostenInvesteringen;
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:"
              + m_CultureInfo.Name, "UTotaleKostenInvesteringen").ToString().Replace("\\t", "\t"), zichtJaar, totaleEersteInversteringContant, "[M€]"));

            //if (dijkringTraject.OptimaleJaarVoorEersteInvestering == zichtJaar)
            //{
            //  // Totale kosten investeringen (contante waarde in [jaar])
            //  writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name
            //    , "UTotaleKostenInvesteringenInhaal").ToString().Replace("\\t", "\t"), zichtJaar, kostensplitsing.Inhaal, "[M€]"));
            //  toedelingkostenInhaal += kostensplitsing.Inhaal[0];
            //}

            //// Totale kosten investeringen (Klimaat)
            //writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name
            //  , "UTotaleKostenInvesteringenKlimaat").ToString().Replace("\\t", "\t"), zichtJaar, kostensplitsing.KlimaatTotaleKosten, "[M€]"));

            //// Totale kosten investeringen (Economie)
            //writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name
            //  , "UTotaleKostenInvesteringenEconomie").ToString().Replace("\\t", "\t"), zichtJaar, kostensplitsing.EconomieTotaleKosten, "[M€]"));


            //toedelingkostenKlimaat += kostensplitsing.KlimaatTotaleKosten;
            //toedelingkostenEconomie += kostensplitsing.EconomieTotaleKosten;
          }
          else
          {
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name
              , "UGeenInvesteringen").ToString().Replace("\\t", "\t")));
          }

          writer.WriteLine();

          // onthouden totalen
          totaalInvesteringen += dijkringTraject.TotaleKostenInvesteringen[scenarioIndex];
        }

        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UDijkringdeelGeheel").ToString().Replace("\\t", "\t"), Naam, DijkringId));
        writer.WriteLine();
        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UWettelijkeNorm").ToString().Replace("\\t", "\t"), this.m_kansen.WettelijkeNorm, "[1/" + m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Jaar").ToString() + "]"));

        if (rekenenMetParameteronzekerheid)
        {
          // overstromingskans *************************************************************************************

          // regel overslaan
          writer.WriteLine();

          // Header
          string overstromingskans = this.m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Overstromingskans").ToString();

          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "IPOverstromingskansPerScenarioRegel").ToString().Replace("\\t", "\t")
            , string.Empty, overstromingskans, overstromingskans));

          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "IPOverstromingskansPerScenarioRegel").ToString().Replace("\\t", "\t")
            , string.Empty
            , "in " + zichtJaar.ToString()
            , "in " + optimaleOverstromingskansenJaar.ToString()));

          string eenheid = "[1/" + m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Jaar").ToString() + "]";
          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "IPOverstromingskansPerScenarioRegel").ToString().Replace("\\t", "\t")
            , string.Empty, eenheid, eenheid));

          // DetailRow
          foreach (PmiddensPerScenario pmiddensPerScenario in this.m_PmiddensPerScenario)
          {
            writer.WriteLine(string.Format(this.m_Language.GetValue("Captions:" + m_CultureInfo.Name, "IPOverstromingskansPerScenarioDetailRegel").ToString().Replace("\\t", "\t")
              , pmiddensPerScenario.Scenario_Id + 1
              , "1/ " + pmiddensPerScenario.OverstromingsKansInZichtjaar.ToString("F0")
              , "1/ " + pmiddensPerScenario.OverstromingsKansInOptimaleOverstromingskansJaar.ToString("F0")));
          }

          // regel overslaan
          writer.WriteLine();

          // optimale overstromingskans ****************************************************************************

          // regel overslaan
          writer.WriteLine();

          // Header
          string optimaleOverstromingskans = this.m_Language.GetValue("Captions:" + m_CultureInfo.Name, "OptimaleOverstromingskans").ToString();
          string optimaleOverschrijdingskans = this.m_Language.GetValue("Captions:" + m_CultureInfo.Name, "OptimaleOverschrijdingskans").ToString();

          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "IPMiddenPerScenarioRegel").ToString().Replace("\\t", "\t")
            , string.Empty, optimaleOverstromingskans, optimaleOverstromingskans, optimaleOverschrijdingskans, optimaleOverschrijdingskans));

          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "IPMiddenPerScenarioRegel").ToString().Replace("\\t", "\t")
            , string.Empty
            , "in " + zichtJaar.ToString()
            , "in " + optimaleOverstromingskansenJaar.ToString()
            , "in " + zichtJaar.ToString()
            , "in " + optimaleOverstromingskansenJaar.ToString()));

          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "IPMiddenPerScenarioRegel").ToString().Replace("\\t", "\t")
            , string.Empty, eenheid, eenheid, eenheid, eenheid));

          // DetailRow
          foreach (PmiddensPerScenario pmiddensPerScenario in this.m_PmiddensPerScenario)
          {
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "IPMiddenPerScenarioDetailRegel").ToString().Replace("\\t", "\t")
              , pmiddensPerScenario.Scenario_Id + 1
              , "1/ " + pmiddensPerScenario.OptimaleOverstromingsKansInZichtjaar.ToString("F0")
              , "1/ " + pmiddensPerScenario.OptimaleOverstromingsKansInOptimaleOverstromingskansJaar.ToString("F0")
              , "1/ " + pmiddensPerScenario.OptimaleOverschrijdingsKansInZichtjaar.ToString("F0")
              , "1/ " + pmiddensPerScenario.OptimaleOverschrijdingsKansInOptimaleOverstromingskansJaar.ToString("F0")));
          }

          // regel overslaan
          writer.WriteLine();
        }
        else
        {
          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UOverstromingskans").ToString().Replace("\\t", "\t")
            , zichtJaar, this.m_kansen.OverstromingskansInZichtjaar, "[1/" + m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Jaar").ToString() + "]"));

          writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UOverstromingskans").ToString().Replace("\\t", "\t")
            , instellingen.OptimaleOverstromingskansenJaar, this.m_kansen.OverstromingskansInOptimaleOverstromingskansJaar, "[1/" + m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Jaar").ToString() + "]"));

          if (!double.IsInfinity(this.m_kansen.OptimaleOverstromingskansInZichtjaar))
          {
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UMiddenOverstromingskans").ToString().Replace("\\t", "\t")
              , zichtJaar, this.m_kansen.OptimaleOverstromingskansInZichtjaar, "[1/" + m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Jaar").ToString() + "]"));
          }

          if (!double.IsInfinity(this.m_kansen.OptimaleOverstromingskansInOptimaleOverstromingskansJaar))
          {
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UMiddenOverstromingskans").ToString().Replace("\\t", "\t")
              , instellingen.OptimaleOverstromingskansenJaar, this.m_kansen.OptimaleOverstromingskansInOptimaleOverstromingskansJaar
              , "[1/" + m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Jaar").ToString() + "]"));
          }

          if (!double.IsInfinity(this.m_kansen.OptimaleOverstromingskansInZichtjaar))
          {
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UOptimaleOverschrijdingskans").ToString().Replace("\\t", "\t")
              , zichtJaar, this.m_kansen.OptimaleOverschrijdingskansInZichtjaar, "[1/" + m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Jaar").ToString() + "]"));
          }

          if (!double.IsInfinity(this.m_kansen.OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar))
          {
            writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UOptimaleOverschrijdingskans").ToString().Replace("\\t", "\t")
              , instellingen.OptimaleOverstromingskansenJaar, this.m_kansen.OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar, "[1/" + m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Jaar").ToString() + "]"));
          }
        }

        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UTotaleInvesteringskosten").ToString().Replace("\\t", "\t"), zichtJaar, totaalInvesteringen, "[M€]"));
        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UTotaleKostenVerwachteSchade").ToString().Replace("\\t", "\t")));
        writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UContanteWaarde").ToString().Replace("\\t", "\t"), zichtJaar, this.m_kansen.ContantewaardeInvesteringenEnSchade.ToString("F1"), "[M€]"));

        //writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UContanteWaardeInhaal").ToString().Replace("\\t", "\t"), zichtJaar, toedelingkostenInhaal.ToString("F1"), "[M€]"));
        //writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UContanteWaardeKlimaat").ToString().Replace("\\t", "\t"), zichtJaar, toedelingkostenKlimaat.ToString("F1"), "[M€]"));
        //writer.WriteLine(string.Format(m_Language.GetValue("Captions:" + m_CultureInfo.Name, "UContanteWaardeEconomie").ToString().Replace("\\t", "\t"), zichtJaar, toedelingkostenEconomie.ToString("F1"), "[M€]"));

        writer.WriteLine();

        writer.Flush();
        writer.Close();
      }
    }

    /// <summary>
    /// TrajectOnzekerheid
    /// </summary>
    public struct TrajectOnzekerheid
    {
      public double AlphaOverstromingskans;
      public double P0Overstromingskans;
      public double AlphaOverschrijdingskans;
      public double P0Overschrijdingskans;
      public double Eta;
      public double OverstromingskansZichtjaar;
      public double Initial_flood_probability;
    }

    /// <summary>
    /// KeringOnzekerheid
    /// </summary>
    public struct KeringOnzekerheid
    {
      public double AlphaOverstromingskans;
      public double P0Overstromingskans;
      public double AlphaOverschrijdingskans;
      public double P0Overschrijdingskans;
      public double Eta;
      public double OverstromingskansZichtjaar;
      public double Initial_flood_probability;
    }

    /// <summary>
    /// Gets the traject onzekerheid.
    /// </summary>
    /// <param name="dijkringTraject">The dijkring traject.</param>
    /// <param name="klimaatScenarioEnFysischMaxAfvoer">The klimaat scenario en fysisch max afvoer.</param>
    /// <param name="zichtjaar">The zichtjaar.</param>
    /// <param name="kansjaar">The kansjaar.</param>
    /// <param name="factorKans">The factor kans.</param>
    /// <returns></returns>
    public TrajectOnzekerheid GetTrajectOnzekerheid(DijkringTraject dijkringTraject
      , int klimaatScenarioEnFysischMaxAfvoer, int zichtjaar, int kansjaar, double factorKans)
    {
      TrajectOnzekerheid trajectOnzekerheid = new TrajectOnzekerheid();

      m_OptimaliseRingDB.KlimaatScenarioEnFysischMaxAfvoerDataRecord(m_DijkringId, m_DeelNummer
        , dijkringTraject.Traject
        , klimaatScenarioEnFysischMaxAfvoer, ref trajectOnzekerheid.AlphaOverstromingskans
        , ref trajectOnzekerheid.P0Overstromingskans, ref trajectOnzekerheid.AlphaOverschrijdingskans
        , ref trajectOnzekerheid.P0Overschrijdingskans, ref trajectOnzekerheid.Eta);

      // Overstromingskans
      trajectOnzekerheid.OverstromingskansZichtjaar = trajectOnzekerheid.P0Overstromingskans
        * NMathFunctions.Exp(trajectOnzekerheid.AlphaOverstromingskans * trajectOnzekerheid.Eta
        * (zichtjaar - kansjaar));

      // P0
      trajectOnzekerheid.Initial_flood_probability = trajectOnzekerheid.OverstromingskansZichtjaar
        * factorKans * dijkringTraject.Factor;

      return trajectOnzekerheid;
    }

    public KeringOnzekerheid GetKeringOnzekerheid(DijkringTraject dijkringTraject
      , int klimaatScenarioEnFysischMaxAfvoer, int zichtjaar, int kansjaar, double factorKans)
    {
      KeringOnzekerheid keringOnzekerheid = new KeringOnzekerheid();

      //hier de kering faken in de query
      //int dijkringId, deel, traject
      int keringid = Convert.ToInt32(dijkringTraject.DijkringId);
      //maar ook het deelnummer faken, niet m_DeelNummer meegeven maar per definitie 1

      m_OptimaliseRingDB.KlimaatScenarioEnFysischMaxAfvoerDataRecord(dijkringTraject.DijkringId, 1
        , keringid
        , klimaatScenarioEnFysischMaxAfvoer, ref keringOnzekerheid.AlphaOverstromingskans
        , ref keringOnzekerheid.P0Overstromingskans, ref keringOnzekerheid.AlphaOverschrijdingskans
        , ref keringOnzekerheid.P0Overschrijdingskans, ref keringOnzekerheid.Eta);

      // Overstromingskans
      keringOnzekerheid.OverstromingskansZichtjaar = keringOnzekerheid.P0Overstromingskans
        * NMathFunctions.Exp(keringOnzekerheid.AlphaOverstromingskans * keringOnzekerheid.Eta
        * (zichtjaar - kansjaar));

      // P0
      keringOnzekerheid.Initial_flood_probability = keringOnzekerheid.OverstromingskansZichtjaar
        * factorKans * dijkringTraject.Factor;

      return keringOnzekerheid;
    }

    /// <summary>
    /// Checks for the discontovoet schade en investeringen
    /// </summary>
    /// <param name="discontovoetSchade">The discontovoet schade.</param>
    /// <param name="discontovoetInvesteringen">The discontovoet investeringen.</param>
    /// <returns></returns>
    public bool CheckDiscontovoet(double discontovoetSchade, double discontovoetInvesteringen, double factorGroeischade)
    {
      List<bool> trajectOk = new List<bool>();

      for (int i = 0; i < Trajecten.Count; i++)
      {
        trajectOk.Add(false);

        DijkringTraject traject = (DijkringTraject)Trajecten[i];

        double beta = traject.AlphaOverstromingskans * traject.Eta + factorGroeischade * Gamma + this.m_Psi * this.MaxEta();
        double theta = traject.AlphaOverstromingskans - Zeta;

        double delta = (traject.lambda_exp / (theta + traject.lambda_exp)) * beta;

        if (((discontovoetSchade / 100) > delta) && ((discontovoetInvesteringen / 100) > delta))
        {
          trajectOk[i] = true;
        }
      }

      return trajectOk.Contains(false);

    }

    /// <summary>
    /// Calculates the aimms.
    /// </summary>
    /// <param name="instellingen">The instellingen.</param>
    /// <param name="kansjaar">The kansjaar.</param>
    /// <param name="schadeJaar">The schade jaar.</param>
    /// <param name="zichtJaar">The zicht jaar.</param>
    /// <param name="optimaleOverstromingskansenJaar">The optimale overstromingskansen jaar.</param>
    /// <param name="z">The z.</param>
    /// <param name="aimmsProject">The aimms project.</param>
    /// <param name="berekeningMap">The berekening map.</param>
    /// <param name="factorKans">The factor kans.</param>
    /// <param name="scenarioparameters">The scenarioparameters.</param>
    /// <returns></returns>
    public bool CalculateAimms(Instellingen instellingen, int kansjaar, int schadeJaar, int zichtJaar
      , int optimaleOverstromingskansenJaar, int z, string aimmsProject, string berekeningMap, double factorKans
      , SortedList<int, InstellingenOnzekerheid> scenarioparameters)
    {

      // reset kansenvalues
      this.m_kansen.ResetValues();

      // Maak verbinding met Aimmsproject
      using (OptimaliseRing.Aimms.Connection aimmsCalulation = new OptimaliseRing.Aimms.Connection(aimmsProject, m_CultureInfo.Name))
      {
        // rekenen met parameteronzekerheid?
        bool rekenenMetParameteronzekerheid = scenarioparameters.Count > 0;

        OptimaliseRing.Aimms.Input inputVariabelen;

        OptimaliseRing.Aimms.Projectvariabelen projectvariabelen = new OptimaliseRing.Aimms.Projectvariabelen();
        projectvariabelen.Dijkringnummer = this.m_DijkringId;
        projectvariabelen.Dijkringnaam = this.m_DijkringNaam;
        projectvariabelen.Dijkringdeelnummer = this.m_DeelNummer;
        projectvariabelen.Dijkringdeelnaam = this.m_Naam;
        projectvariabelen.Startyear = zichtJaar;
        projectvariabelen.Max_updates = Hkv.General.ConvertString.ToInt32(this.m_Profile.GetValue("Parameters", "max_updates").ToString());

        // zeta (1/cm)
        projectvariabelen.Loss_increase = this.m_Zeta + this.m_Nu;

        projectvariabelen.Segments = this.GetSegmenten(instellingen);

        if (projectvariabelen.Segments.Count > 0)
        {
          int Lowest_segment1 = Hkv.General.ConvertString.ToInt32(this.m_Profile.GetValue("Parameters", "Lowest_segment1").ToString());
          int Lowest_segment2 = Hkv.General.ConvertString.ToInt32(this.m_Profile.GetValue("Parameters", "Lowest_segment2").ToString());

          // l_0
          projectvariabelen.Lowest_segment1 = projectvariabelen.Segments[Lowest_segment1];
          // l*
          projectvariabelen.Lowest_segment2 = projectvariabelen.Segments[Lowest_segment2];
        }

        // bepaal delta tijd voor de schade
        int deltaTijdVoorSchade = zichtJaar - schadeJaar;

        string scenarionaam = "Scenario0";
        if (rekenenMetParameteronzekerheid)
        {
          projectvariabelen.Objective = Objective.Sum;

          double kleinsteAlphaSchade = double.MaxValue;

          foreach (KeyValuePair<int, InstellingenOnzekerheid> instellingenOnzekerheid in scenarioparameters)
          {
            double gamma = 0.0;
            m_OptimaliseRingDB.EconomischScenarioRecord(m_DijkringId, m_DeelNummer
              , instellingenOnzekerheid.Value.EconomischScenario, ref gamma);

            double min_overschrijdingskans = 0.0;
            m_OptimaliseRingDB.SchadeKlimaatData(m_DijkringId, m_DeelNummer
              , instellingenOnzekerheid.Value.KlimaatScenarioEnFysischMaxAfvoer, ref min_overschrijdingskans);

            scenarionaam = "Scenario" + instellingenOnzekerheid.Key;
            double economic_growth = gamma * instellingen.FactorGroeiSchade + this.m_Psi * this.MaxEta();

            // delta_1 (1/year)
            double discount_rate1 = Math.Log(1 + instellingenOnzekerheid.Value.DiscontovoetSchade / 100.0);
            // delta_2 (1/year)
            double discount_rate2 = Math.Log(1 + instellingenOnzekerheid.Value.DiscontovoetInvesteringen / 100.0);

            // Scenario's toevoegen
            Scenario scenario = new Scenario(scenarionaam
              , economic_growth, discount_rate1, discount_rate2, projectvariabelen.Segments.Count);

            int trajectTeller = 0;
            foreach (DijkringTraject dijkringTraject in this.Trajecten)
            {
              if (!dijkringTraject.Iskering)
              {
                TrajectOnzekerheid trajectOnzekerheid = this.GetTrajectOnzekerheid(dijkringTraject
                  , instellingenOnzekerheid.Value.KlimaatScenarioEnFysischMaxAfvoer, zichtJaar, kansjaar, factorKans);

                double discontovoetInvesteringen = instellingenOnzekerheid.Value.DiscontovoetInvesteringen;
                double factorOpslag = 1 + dijkringTraject.Omega / (discontovoetInvesteringen / 100.0);

                double toedelingsfactor = 1.0;
                if (dijkringTraject.Iskering)
                {
                  toedelingsfactor = dijkringTraject.Percentage / 100.0;
                }

                double exp_fixed = dijkringTraject.C_exp * instellingen.FactorKosten * factorOpslag * toedelingsfactor;
                double exp_linear = dijkringTraject.b_exp * instellingen.FactorKosten * factorOpslag * toedelingsfactor;
                //double exp_power = dijkringTraject.lambda_exp;
                double q_fixed = dijkringTraject.c_kwad * instellingen.FactorKosten * factorOpslag * toedelingsfactor;
                double q_linear = dijkringTraject.b_kwad * instellingen.FactorKosten * factorOpslag * toedelingsfactor;
                double q_quad = dijkringTraject.a_kwad * instellingen.FactorKosten * factorOpslag * toedelingsfactor;

                // ExponentialCostParameters
                scenario.ExponentialCostParameters[trajectTeller] = new ExponentialCostParameters(exp_fixed, exp_linear);

                // QuadraticCostParameters
                scenario.QuadraticCostParameters[trajectTeller] = new QuadraticCostParameters(q_fixed, q_linear, q_quad);

                scenario.Initial_flood_probability[trajectTeller] = trajectOnzekerheid.Initial_flood_probability;
                scenario.Extreme_water[trajectTeller] = trajectOnzekerheid.AlphaOverstromingskans;
                scenario.Increase_of_water_level[trajectTeller++] = trajectOnzekerheid.Eta;

                // Bepaal kleinste Alpha
                kleinsteAlphaSchade = Math.Min(kleinsteAlphaSchade, trajectOnzekerheid.AlphaOverstromingskans);

              }
              else
              {
                KeringOnzekerheid keringOnzekerheid = this.GetKeringOnzekerheid(dijkringTraject
                  , instellingenOnzekerheid.Value.KlimaatScenarioEnFysischMaxAfvoer, zichtJaar, kansjaar, factorKans);

                double discontovoetInvesteringen = instellingenOnzekerheid.Value.DiscontovoetInvesteringen;
                double factorOpslag = 1 + dijkringTraject.Omega / (discontovoetInvesteringen / 100.0);

                double toedelingsfactor = 1.0;
                if (dijkringTraject.Iskering)
                {
                  toedelingsfactor = dijkringTraject.Percentage / 100.0;
                }

                double exp_fixed = dijkringTraject.C_exp * instellingen.FactorKosten * factorOpslag * toedelingsfactor;
                double exp_linear = dijkringTraject.b_exp * instellingen.FactorKosten * factorOpslag * toedelingsfactor;
                //double exp_power = dijkringTraject.lambda_exp;
                double q_fixed = dijkringTraject.c_kwad * instellingen.FactorKosten * factorOpslag * toedelingsfactor;
                double q_linear = dijkringTraject.b_kwad * instellingen.FactorKosten * factorOpslag * toedelingsfactor;
                double q_quad = dijkringTraject.a_kwad * instellingen.FactorKosten * factorOpslag * toedelingsfactor;

                // ExponentialCostParameters
                scenario.ExponentialCostParameters[trajectTeller] = new ExponentialCostParameters(exp_fixed, exp_linear);

                // QuadraticCostParameters
                scenario.QuadraticCostParameters[trajectTeller] = new QuadraticCostParameters(q_fixed, q_linear, q_quad);

                scenario.Initial_flood_probability[trajectTeller] = keringOnzekerheid.Initial_flood_probability;
                scenario.Extreme_water[trajectTeller] = keringOnzekerheid.AlphaOverstromingskans;
                scenario.Increase_of_water_level[trajectTeller++] = keringOnzekerheid.Eta;

                // Bepaal kleinste Alpha
                kleinsteAlphaSchade = Math.Min(kleinsteAlphaSchade, keringOnzekerheid.AlphaOverstromingskans);

              }
            }

            // Voor bepalen van de schade wordt de gamma van het eerste scenario gebruikt.
            double schade = Function.V(instellingen, this.m_Schade
              , kleinsteAlphaSchade, this.m_Nu, this.m_AantalInwoners, this.m_AantalSlachtoffers
              , this.m_AantalGetroffenen, gamma, this.m_Psi, this.MaxEta(), deltaTijdVoorSchade, this.m_DijkringspecifiekeFactorSchade);

            scenario.Initial_economic_value = schade;

            projectvariabelen.Add(scenario);
          }
        }
        else
        {
          projectvariabelen.Objective = Objective.One_scenario;

          double economic_growth = this.m_Gamma * instellingen.FactorGroeiSchade + this.m_Psi * this.MaxEta();

          // delta_1 (1/year)
          double discount_rate1 = Math.Log(1 + instellingen.DiscontovoetSchade / 100.0);//*
          // delta_2 (1/year)
          double discount_rate2 = Math.Log(1 + instellingen.DiscontovoetInvesteringen / 100.0);//*

          // Scenario's toevoegen
          Scenario scenario = new Scenario(scenarionaam
            , economic_growth, discount_rate1, discount_rate2, projectvariabelen.Segments.Count);//*

          int teller = 0;
          foreach (DijkringTraject dijkringTraject in this.Trajecten)
          {
            double overstromingskans = dijkringTraject.P0Overstromingskans
              * NMathFunctions.Exp(dijkringTraject.AlphaOverstromingskans * dijkringTraject.Eta
              * (zichtJaar - kansjaar));

            // P0
            dijkringTraject.Initial_flood_probability = overstromingskans * factorKans
              * dijkringTraject.Factor;

            double discontovoetInvesteringen = instellingen.DiscontovoetInvesteringen;
            double factorOpslag = 1 + dijkringTraject.Omega / (discontovoetInvesteringen / 100.0);

            double toedelingsfactor = 1.0;
            if (dijkringTraject.Iskering)
            {
              toedelingsfactor = dijkringTraject.Percentage / 100.0;
            }

            double exp_fixed = dijkringTraject.C_exp * instellingen.FactorKosten * factorOpslag * toedelingsfactor;
            double exp_linear = dijkringTraject.b_exp * instellingen.FactorKosten * factorOpslag * toedelingsfactor;
            //double exp_power = dijkringTraject.lambda_exp;
            double q_fixed = dijkringTraject.c_kwad * instellingen.FactorKosten * factorOpslag * toedelingsfactor;
            double q_linear = dijkringTraject.b_kwad * instellingen.FactorKosten * factorOpslag * toedelingsfactor;
            double q_quad = dijkringTraject.a_kwad * instellingen.FactorKosten * factorOpslag * toedelingsfactor;

            // ExponentialCostParameters
            scenario.ExponentialCostParameters[teller] = new ExponentialCostParameters(exp_fixed, exp_linear);

            // QuadraticCostParameters
            scenario.QuadraticCostParameters[teller] = new QuadraticCostParameters(q_fixed, q_linear, q_quad);

            scenario.Initial_flood_probability[teller] = dijkringTraject.Initial_flood_probability;
            scenario.Extreme_water[teller] = dijkringTraject.AlphaOverstromingskans;
            scenario.Increase_of_water_level[teller++] = dijkringTraject.Eta;
          }

          // bepaal schade
          double schade = Function.V(instellingen, this.m_Schade
           , this.KleinsteAlpha(), this.m_Nu, this.m_AantalInwoners, this.m_AantalSlachtoffers
           , this.m_AantalGetroffenen, this.m_Gamma, this.m_Psi, this.MaxEta(), deltaTijdVoorSchade, this.m_DijkringspecifiekeFactorSchade);

          scenario.Initial_economic_value = schade;
          projectvariabelen.Add(scenario);
        }

        inputVariabelen = new Input(projectvariabelen);


        // TABBLAD SETTINGS ************************************************************

        InvestmentCostFunctions InvestmentCostFunction = (InvestmentCostFunctions)instellingen.TypeKostenfunctie;
        bool flood_probability_constraint = instellingen.Veiligheidsnorm == 2;
        //bool max_Costs = Convert.ToBoolean(this.m_Profile.GetValue("Parameters", "max_Costs"));
        bool max_Costs = Convert.ToBoolean(this.m_Profile.GetValue("Parameters", "max_Costs"));
        double maximal_flood_probability = 1.0 / this.m_kansen.RestrictiePmin;

        inputVariabelen.Settings.InvestmentCostFunction = InvestmentCostFunction;
        inputVariabelen.Settings.OptionalConstraints = new OptionalConstraints(flood_probability_constraint, max_Costs, maximal_flood_probability);

        bool one_increase_during_period = Convert.ToBoolean(this.m_Profile.GetValue("Parameters", "one_increase_during_period"));
        bool minimum_increase = Convert.ToBoolean(this.m_Profile.GetValue("Parameters", "minimum_increase"));
        bool two_segments = Convert.ToBoolean(this.m_Profile.GetValue("Parameters", "two_segments"));
        bool all_at_the_same_time = Convert.ToBoolean(this.m_Profile.GetValue("Parameters", "all_at_the_same_time"));
        bool group_at_the_same_time = Convert.ToBoolean(this.m_Profile.GetValue("Parameters", "group_at_the_same_time"));
        double max_increase = Hkv.General.ConvertString.ToDouble(this.m_Profile.GetValue("Parameters", "max_increase").ToString());
        double min_increase = Hkv.General.ConvertString.ToDouble(this.m_Profile.GetValue("Parameters", "min_increase").ToString());
        double years_until_steady = Hkv.General.ConvertString.ToDouble(this.m_Profile.GetValue("Parameters", "years_until_steady").ToString());
        double years_between_increases = Hkv.General.ConvertString.ToDouble(this.m_Profile.GetValue("Parameters", "years_between_increases").ToString());
        double years_without_increase = Hkv.General.ConvertString.ToDouble(this.m_Profile.GetValue("Parameters", "years_without_increase").ToString());
        double years_until_force_equal = Hkv.General.ConvertString.ToDouble(this.m_Profile.GetValue("Parameters", "years_until_force_equal").ToString());

        inputVariabelen.Settings.TuningConstraints
          = new TuningConstraints(one_increase_during_period, minimum_increase, two_segments, all_at_the_same_time, group_at_the_same_time
          , max_increase, min_increase, years_until_steady, years_between_increases, years_without_increase, years_until_force_equal);

        bool presolve = Convert.ToBoolean(this.m_Profile.GetValue("Parameters", "presolve"));
        bool add_cuts = Convert.ToBoolean(this.m_Profile.GetValue("Parameters", "add_cuts"));
        bool use_incumbents = Convert.ToBoolean(this.m_Profile.GetValue("Parameters", "use_incumbents"));
        bool bigM = Convert.ToBoolean(this.m_Profile.GetValue("Parameters", "bigM"));

        double max_iterations_AOA = Hkv.General.ConvertString.ToDouble(this.m_Profile.GetValue("Parameters", "max_iterations_AOA").ToString());
        double mip_tolerance = Hkv.General.ConvertString.ToDouble(this.m_Profile.GetValue("Parameters", "mip_tolerance").ToString());
        double relative_tolerance = Hkv.General.ConvertString.ToDouble(this.m_Profile.GetValue("Parameters", "relative_tolerance").ToString());
        double max_time = Hkv.General.ConvertString.ToDouble(this.m_Profile.GetValue("Parameters", "max_time").ToString());

        inputVariabelen.Settings.AdditionalOptions
          = new AdditionalOptions(presolve, add_cuts, use_incumbents, bigM, max_iterations_AOA, mip_tolerance, relative_tolerance, max_time);

        // TABBLAD DISCRETIZATION ******************************************************
        inputVariabelen.Discretization.Time_horizon = z;
        inputVariabelen.Discretization.DiscretizationMethod = (DiscretizationMethod)Enum.Parse(typeof(DiscretizationMethod)
          , this.m_Profile.GetValue("Parameters", "DiscretizationMethod").ToString());
        if (Trajecten.Count == 1)
        {
          inputVariabelen.Discretization.Nr_variables = Hkv.General.ConvertString.ToInt32(this.m_Profile.GetValue("Parameters", "Nr_variables_1traject").ToString());
        }
        else
        {
          inputVariabelen.Discretization.Nr_variables = Hkv.General.ConvertString.ToInt32(this.m_Profile.GetValue("Parameters", "Nr_variables_ntraject").ToString());
        }

        int partitionTeller = 1;
        // naam, H_0, P_0, eta, alpha
        while (this.m_Profile.GetValue("Parameters", string.Format("partition_p{0}", partitionTeller), string.Empty).ToString() != string.Empty)
        {
          string partition = this.m_Profile.GetValue("Parameters", string.Format("partition_p{0}", partitionTeller)).ToString();
          int partition_start_moment = Hkv.General.ConvertString.ToInt32(this.m_Profile.GetValue("Parameters", string.Format("partition_start_moment_p{0}", partitionTeller)).ToString());
          int partition_end_moment = Hkv.General.ConvertString.ToInt32(this.m_Profile.GetValue("Parameters", string.Format("partition_end_moment_p{0}", partitionTeller)).ToString());
          int partition_no_periods = Hkv.General.ConvertString.ToInt32(this.m_Profile.GetValue("Parameters", string.Format("partition_no_periods_p{0}", partitionTeller)).ToString());
          double partition_pct = Hkv.General.ConvertString.ToDouble(this.m_Profile.GetValue("Parameters", string.Format("partition_pct_p{0}", partitionTeller)).ToString());

          inputVariabelen.Discretization.Partitions.Add(
            new Partition(partition, partition_start_moment, partition_end_moment, partition_no_periods, partition_pct));

          partitionTeller += 1;
        }

        inputVariabelen.Discretization.Option_convert_solution = Convert.ToBoolean(this.m_Profile.GetValue("Parameters", "Option_convert_solution"));

        // TABBLAD SEGMENTOPTIONS ******************************************************

        // TABBLAD SOLVE ***************************************************************
        inputVariabelen.Solve.SolutionAlgorithm = (SolutionAlgorithm)Enum.Parse(typeof(SolutionAlgorithm), this.m_Profile.GetValue("Parameters", "SolutionAlgorithm").ToString());
        inputVariabelen.Solve.Tune_nr = Hkv.General.ConvertString.ToInt32(this.m_Profile.GetValue("Parameters", "Tune_nr").ToString());
        inputVariabelen.Solve.Tune_perc_width = Hkv.General.ConvertString.ToDouble(this.m_Profile.GetValue("Parameters", "Tune_perc_width").ToString());
        inputVariabelen.Solve.Tune_tolerance_factor = Hkv.General.ConvertString.ToDouble(this.m_Profile.GetValue("Parameters", "Tune_tolerance_factor").ToString());

        inputVariabelen.Solve.Case_prefix = this.m_Profile.GetValue("Parameters", "Case_prefix").ToString();

        this.m_kansen.EersteInvesteringJaar = int.MaxValue;

        // Per scenario matrixdata ophalen (voor scenario 0)
        scenarionaam = "Scenario0";

        Aimms.Output outputVariabelen = aimmsCalulation.Run(inputVariabelen);
        if (outputVariabelen != null)
        {
          int teller = 1;

          // Dijkringdeel waarde
          //this.m_kansen.ContantewaardeInvesteringenEnSchade = outputVariabelen.CostObjectives.Objective_year_based;

          this.m_kansen.ContantewaardeInvesteringenEnSchade = outputVariabelen.GetTotal_expected_costs2PerScenario(scenarionaam);

          // Berekening gelukt, resultaten verwerken
          foreach (DijkringTraject dijkringTraject in this.Trajecten)
          {

            //string segment = this.Trajecten.Count > 1
            //  ? string.Format("{0}-{1}", teller++, dijkringTraject.Naam)
            //  : string.Format("{0}", dijkringTraject.Naam);

            string segment = string.Format("{0}", dijkringTraject.Naam);
            if (this.Trajecten.Count > 1 && this.Trajecten.Count < 10)
            {
              segment = string.Format("{0}-{1}", teller++, dijkringTraject.Naam);
            }
            else if (this.Trajecten.Count >= 10)
            {
              segment = string.Format("{0:00}-{1}", teller++, dijkringTraject.Naam);
            }

            List<OptimaliseRing.Aimms.Output.DijkVerhoging> dijkVerhogingen
              = outputVariabelen.GetDijkverhogingenPerTraject(0, segment);

            if (dijkVerhogingen != null)
            {
              if (dijkVerhogingen.Count == 1)
              {
                dijkringTraject.OptimaleJaarVoorEersteInvestering = dijkVerhogingen[0].Jaar;
                dijkringTraject.PeriodeTussenVerhogingen = int.MinValue;
                dijkringTraject.HoogteVanEersteVerhoging = dijkVerhogingen[0].Increase;
                dijkringTraject.HoogteVanTweedeEnVolgendeVerhoging = 0.0;
                this.m_kansen.EersteInvesteringJaar = Math.Min(dijkVerhogingen[0].Jaar, this.m_kansen.EersteInvesteringJaar);
              }
              else if (dijkVerhogingen.Count >= 2)
              {
                dijkringTraject.OptimaleJaarVoorEersteInvestering = dijkVerhogingen[0].Jaar;
                dijkringTraject.PeriodeTussenVerhogingen = dijkVerhogingen[1].Jaar;
                dijkringTraject.HoogteVanEersteVerhoging = dijkVerhogingen[0].Increase;
                dijkringTraject.HoogteVanTweedeEnVolgendeVerhoging = dijkVerhogingen[1].Increase;
                this.m_kansen.EersteInvesteringJaar = Math.Min(dijkVerhogingen[0].Jaar, this.m_kansen.EersteInvesteringJaar);
              }

              // Bepaal trajectIndex in outputvariabelen van Aimms
              int indexTraject = outputVariabelen.GetTrajectIndex(segment);

              double[,] verdisconteerdeKosten = new double[outputVariabelen.Scenarios.Count, dijkVerhogingen.Count];
              double[] totaleKostenInvesteringen = new double[outputVariabelen.Scenarios.Count];

              for (int scenarioIndex = 0; scenarioIndex < outputVariabelen.Scenarios.Count; scenarioIndex++)
              {
                for (int dijkverhogingIndex = 0; dijkverhogingIndex < dijkVerhogingen.Count; dijkverhogingIndex++)
                {
                  verdisconteerdeKosten[scenarioIndex, dijkverhogingIndex]
                    = outputVariabelen.DikeIncrease.Update_discounted_costs.GetLength(2) > dijkverhogingIndex
                    ? outputVariabelen.DikeIncrease.Update_discounted_costs[scenarioIndex, indexTraject, dijkverhogingIndex] : 0.0;
                }
                totaleKostenInvesteringen[scenarioIndex]
                  += outputVariabelen.DikeIncrease.Total_discounted_cost[scenarioIndex, indexTraject];

              }

              dijkringTraject.TotaleKostenInvesteringen = totaleKostenInvesteringen;

              for (int dijkverhogingIndex = 0; dijkverhogingIndex < dijkVerhogingen.Count; dijkverhogingIndex++)
              {
                OptimaliseRing.Aimms.Output.DijkVerhoging dijkVerhoging = dijkVerhogingen[dijkverhogingIndex];
                double[] verdisconteerdeKostenDijkverhoging= new double[outputVariabelen.Scenarios.Count];
                for (int index = 0; index < outputVariabelen.Scenarios.Count; index++)
                {
                  verdisconteerdeKostenDijkverhoging[index] = verdisconteerdeKosten[index, dijkverhogingIndex];
                }

                dijkringTraject.Investeringen.Add(new Investering(dijkVerhoging.Jaar
                  , dijkVerhoging.Increase, dijkVerhoging.Costs, verdisconteerdeKostenDijkverhoging));
              }
            }
          }

          this.m_kansen.MatrixData = outputVariabelen.GetMatrixDataPerScenario(scenarionaam);
          this.m_PmiddensPerScenario.Clear();

          if (rekenenMetParameteronzekerheid)
          {
            foreach (KeyValuePair<int, InstellingenOnzekerheid> instellingenOnzekerheid in scenarioparameters)
            {
              scenarionaam = "Scenario" + instellingenOnzekerheid.Key;

              double overstromingskansInZichtjaar
                = Math.Round(1.0 / outputVariabelen.GetOverstromingsKansPerScenario(scenarionaam, zichtJaar) / 5, 0) * 5;

              double overstromingskansInOverstromingskansenJaar
                = Math.Round(1.0 / outputVariabelen.GetOverstromingsKansPerScenario(scenarionaam
                , optimaleOverstromingskansenJaar) / 5, 0) * 5;

              double optimaleOverstromingskansInZichtjaar
                = Math.Round(1.0 / outputVariabelen.GetOptimaleOverstromingsKansPerScenario(scenarionaam, zichtJaar) / 5, 0) * 5;

              double optimaleOverstromingskansInOverstromingskansenJaar
                = Math.Round(1.0 / outputVariabelen.GetOptimaleOverstromingsKansPerScenario(scenarionaam
                , optimaleOverstromingskansenJaar) / 5, 0) * 5;

              double overschrijdingsKansInZichtJaar
                = GetOverschrijdingskans(zichtJaar, optimaleOverstromingskansInZichtjaar, factorKans);

              double overschrijdingsKansInOptimaleOverstromingskansenJaar
                = GetOverschrijdingskans(optimaleOverstromingskansenJaar, optimaleOverstromingskansInOverstromingskansenJaar, factorKans);

              this.m_PmiddensPerScenario.Add(new PmiddensPerScenario(instellingenOnzekerheid.Key
                , overstromingskansInZichtjaar, overstromingskansInOverstromingskansenJaar
                , optimaleOverstromingskansInZichtjaar, optimaleOverstromingskansInOverstromingskansenJaar
                , overschrijdingsKansInZichtJaar, overschrijdingsKansInOptimaleOverstromingskansenJaar));
            }
          }
          else
          {
            scenarionaam = "Scenario" + 0;

            double overstromingskansInZichtjaar
            = Math.Round(1.0 / outputVariabelen.GetOverstromingsKansPerScenario(scenarionaam, zichtJaar) / 5, 0) * 5;

            double overstromingskansInOverstromingskansenJaar
              = Math.Round(1.0 / outputVariabelen.GetOverstromingsKansPerScenario(scenarionaam
              , optimaleOverstromingskansenJaar) / 5, 0) * 5;

            double optimaleOverstromingskansInZichtjaar
              = Math.Round(1.0 / outputVariabelen.GetOptimaleOverstromingsKansPerScenario(scenarionaam, zichtJaar) / 5, 0) * 5;

            double optimaleOverstromingskansInOverstromingskansenJaar
              = Math.Round(1.0 / outputVariabelen.GetOptimaleOverstromingsKansPerScenario(scenarionaam
              , optimaleOverstromingskansenJaar) / 5, 0) * 5;

            double overschrijdingsKansInZichtJaar
              = GetOverschrijdingskans(zichtJaar, optimaleOverstromingskansInZichtjaar, factorKans);

            double overschrijdingsKansInOptimaleOverstromingskansenJaar
              = GetOverschrijdingskans(optimaleOverstromingskansenJaar
              , optimaleOverstromingskansInOverstromingskansenJaar, factorKans);

            this.m_PmiddensPerScenario.Add(new PmiddensPerScenario(0
              , overstromingskansInZichtjaar, overstromingskansInOverstromingskansenJaar
              , optimaleOverstromingskansInZichtjaar, optimaleOverstromingskansInOverstromingskansenJaar
              , overschrijdingsKansInZichtJaar, overschrijdingsKansInOptimaleOverstromingskansenJaar));
          }

          if (this.m_PmiddensPerScenario.Count > 0)
          {
            this.m_kansen.OptimaleOverstromingskansInZichtjaar = this.m_PmiddensPerScenario[0].OptimaleOverstromingsKansInZichtjaar;
            this.m_kansen.OptimaleOverschrijdingskansInZichtjaar = this.m_PmiddensPerScenario[0].OptimaleOverschrijdingsKansInZichtjaar;
            this.m_kansen.OptimaleOverstromingskansInOptimaleOverstromingskansJaar = this.m_PmiddensPerScenario[0].OptimaleOverstromingsKansInOptimaleOverstromingskansJaar;
            this.m_kansen.OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar = this.m_PmiddensPerScenario[0].OptimaleOverschrijdingsKansInOptimaleOverstromingskansJaar;

            if (this.m_kansen.EersteInvesteringJaar < int.MaxValue)
            {
              this.m_kansen.OptimaleOverstromingskansInEersteInversteringjaar = outputVariabelen.GetOptimaleOverstromingsKansPerScenario(
                scenarionaam, this.m_kansen.EersteInvesteringJaar);
            }
          }

          int aantalJaren = outputVariabelen.Probabilities.All_years.Length;
          for (int jaar = 0; jaar < aantalJaren; jaar++)
          {
            this.m_kansen.MatrixData[jaar, Convert.ToInt32(MatrixKolom.PWET)] = 1.0 / this.m_kansen.WettelijkeNorm;
            this.m_kansen.MatrixData[jaar, Convert.ToInt32(MatrixKolom.PRESTRICTIE)] = 1.0 / this.m_kansen.RestrictiePmin;
          }

          this.m_kansen.OverstromingskansInZichtjaar = Math.Round(1.0
            / this.m_kansen.MatrixData[0, Convert.ToInt32(MatrixKolom.P)] / 5, 0) * 5;

          int indexOptimaleOverstromingskansenJaar = optimaleOverstromingskansenJaar - zichtJaar;

          this.m_kansen.OverstromingskansInOptimaleOverstromingskansJaar = Math.Round(1.0
             / this.m_kansen.MatrixData[indexOptimaleOverstromingskansenJaar, Convert.ToInt32(MatrixKolom.P)] / 5, 0) * 5;

          return true;

        }
      }

      return false;
    }

    /// <summary>
    /// Bepaals the kansen in optimale overstromings jaar.
    /// </summary>
    /// <param name="zichtJaar">The zicht jaar.</param>
    /// <param name="optimaleOverstromingskansenJaar">The optimale overstromingskansen jaar.</param>
    /// <param name="factorKans">The factor kans.</param>
    public void BepaalKansenInOptimaleOverstromingsJaar(int zichtJaar, int optimaleOverstromingskansenJaar, double factorKans)
    {
      int indexOptimaleOverstromingskansenJaar = optimaleOverstromingskansenJaar - zichtJaar;

      indexOptimaleOverstromingskansenJaar = Math.Min(this.m_kansen.MatrixData.Rows -1, indexOptimaleOverstromingskansenJaar);

      this.m_kansen.OverstromingskansInOptimaleOverstromingskansJaar = Math.Round(1.0
        / this.m_kansen.MatrixData[indexOptimaleOverstromingskansenJaar, Convert.ToInt32(MatrixKolom.P)] / 5, 0) * 5;

      this.m_kansen.OptimaleOverstromingskansInOptimaleOverstromingskansJaar = Math.Round(1.0
        / this.m_kansen.MatrixData[indexOptimaleOverstromingskansenJaar, Convert.ToInt32(MatrixKolom.PMIDDEN)] / 5, 0) * 5;

      this.m_kansen.OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar
        = this.GetOverschrijdingskans(optimaleOverstromingskansenJaar
        , this.m_kansen.OptimaleOverstromingskansInOptimaleOverstromingskansJaar, factorKans);

    }

    /// <summary>
    /// Gets the segmenten.
    /// </summary>
    /// <returns></returns>
    private Segments GetSegmenten(Instellingen instellingen)
    {
      Segments segments = new Segments();
      int teller = 1;

      // rekenen met parameteronzekerheid?
      bool rekenenMetParameteronzekerheid = instellingen.Parametersonzekerheid && instellingen.Scenarioparameters.Count > 0;

      foreach (DijkringTraject dijkringTraject in this.Trajecten)
      {
        double discontovoetInvesteringen = instellingen.DiscontovoetInvesteringen;
        if (rekenenMetParameteronzekerheid)
        {
          // Eerste scenario pakken voor discontovoetInvesteringen om factorOpslag te bepalen
          discontovoetInvesteringen = instellingen.Scenarioparameters[0].DiscontovoetInvesteringen;
        }

        string segment = string.Format("{0}", dijkringTraject.Naam);
        if (this.Trajecten.Count > 1 && this.Trajecten.Count < 10)
        {
          segment = string.Format("{0}-{1}", teller++, dijkringTraject.Naam);
        }
        else if (this.Trajecten.Count >= 10)
        {
          segment = string.Format("{0:00}-{1}", teller++, dijkringTraject.Naam);
        }

        // H0
        double initial_height = dijkringTraject.H0;
        double exp_power = dijkringTraject.lambda_exp;

        segments.Add(new Segment(segment, initial_height, exp_power));
      }
      return segments;
    }

    /// <summary>
    /// Bepaal de kleinste alpha.
    /// </summary>
    /// <returns></returns>
    public double KleinsteAlpha()
    {
      double kleinsteAlpha = Double.MaxValue;

      for (int i = 0; i < Trajecten.Count; i++)
      {
        DijkringTraject traject = (DijkringTraject)Trajecten[i];

        kleinsteAlpha = Math.Min(kleinsteAlpha, traject.AlphaOverstromingskans);
      }

      return kleinsteAlpha;
    }

    /// <summary>
    /// Maximale eta van alle trajecten
    /// </summary>
    /// <returns></returns>
    public double MaxEta()
    {
      double maxEta = Double.MinValue;

      for (int i = 0; i < Trajecten.Count; i++)
      {
        DijkringTraject traject = (DijkringTraject)Trajecten[i];

        maxEta = Math.Max(maxEta, traject.Eta);
      }

      return maxEta;
    }

    #endregion Methods ----------------------------------------------------

    #region ICloneable Members

    public object Clone()
    {
      System.IO.MemoryStream ms = new System.IO.MemoryStream();
      BinaryFormatter bf = new BinaryFormatter();
      bf.Serialize(ms, this);
      ms.Seek(0, System.IO.SeekOrigin.Begin);
      DijkringDeel obj = (DijkringDeel)bf.Deserialize(ms);
      ms.Close();
      return obj;
    }

    #endregion
  }
}
