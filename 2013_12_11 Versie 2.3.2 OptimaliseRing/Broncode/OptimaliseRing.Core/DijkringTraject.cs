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
using System.Collections;
using System.Text;
using System.Xml;

using OptimaliseRing.General;
using OptimaliseRing.Profile;

using CenterSpace.NMath.Core;
using System.Collections.Generic;

namespace OptimaliseRing.Core
{
  /// <summary>
  /// Dijkringtraject
  /// </summary>
  public class DijkringTraject
  {
    #region Instance Variables -----------------------------------------------

    private int m_Traject;                                   // Dijkring traject nummer
    private bool m_Iskering;                                   // is kering
    private int m_Percentage;

    private string m_Naam;                                   // Naam dijkringtraject
    private string m_DijkringId;                             // dijkring id
    private string m_DijkringNaam;                           // dijkring naam
    private string m_DijkringDeel;                           // Dijkringdeel naam
    private int m_OptimaleJaarVoorEersteInvestering;         // Optimale jaar voor eerste investering.
    private double m_VerdisconteerdeKostenEersteInvestering; // Verdisconteerde kosten eerste investeringen.
    private double m_KostenEersteInvesteringen;              // Kosten eerste investeringen
    private int m_PeriodeTussenVerhogingen;                  // Periode tussen verhogingen
    private double m_HoogteVanEersteVerhoging;               // Hoogte van eerste verhoging
    private double m_HoogteVanTweedeEnVolgendeVerhoging;     // Hoogte van tweede en volgende verhoging
    private double[] m_TotaleKostenInvesteringen;            // Totale kosten investeringen per scenario
    private double m_H0;                                     // Dijkverhoging in startjaar [cm+NAP] per traject
    private double m_Factor;                                 // Dijkringdeel/ traject factor
    private double m_P0Overstromingskans;                    // Overstromingskans in startjaar zonder verhoging [1/jaar] per traject voor Overstromingskans
    private double m_Initial_flood_probability;              // Optimaleoverstromingskans waarmee Aimms rekend. (incl. factoren.
    private double m_AlphaOverstromingskans;                 // Schaalparameter (exponentiele verdeling) [1/cm] per traject voor Overstromingskans
    private double m_P0Overschrijdingskans;                  // Overstromingskans in startjaar zonder verhoging [1/jaar] per traject voor Overschrijdingskans
    private double m_AlphaOverschrijdingskans;               // Schaalparameter (exponentiele verdeling) [1/cm] per traject voor Overschrijdingskans
    private double m_Eta;                                    // Structurele stijging relatieve waterstand	cm/jaar

    private double m_C_exp;                                 // Vaste kosten van de investeringen	M€/cm
    private double m_b_exp;                                 // Variabele kosten van de investeringen	M€/cm
    private double m_lambda_exp;                            // Schaalparameter van verhogingen	1/cm
    private double m_c_kwad;                                // Vaste kosten van de investeringen	M€/cm
    private double m_b_kwad;                                // Variabele kosten van de investeringen	M€/cm
    private double m_a_kwad;                                //
    private double m_Omega;                                 // Beheer en onderhoudskosten als percentage van de investeringskosten [%]

    private double m_W;                                     // Som van de eerdere verhogingen

    private OptimaliseRingDB m_OptimaliseRingDB;
    private int m_Schadejaar;                                // Jaartal waarvoor schade in de database gegeven zijn
    private int m_Kansjaar;                                  // Jaartal waarvoor overstromingskans in de database gegeven zijn

    private List<Investering> m_Investering;                 // Lijst met investeringen

    #endregion Instance Variables --------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Maak een nieuwe instantiatie van de <see cref="T:DijkringTraject"/> class.
    /// </summary>
    /// <param name="dijkringId">Dijkring identificatie</param>
    /// <param name="deel">Deelnummer</param>
    /// <param name="traject">Trajectnummer</param>
    /// <param name="naam">Naam van het traject</param>
    public DijkringTraject(OptimaliseRingDB optimaliseRingDB,
       string dijkringId, int deel, int traject, string naam
      , int klimaatScenarioEnFysischMaxAfvoer, int parametersKostenfunctie)
    {
      m_OptimaliseRingDB = optimaliseRingDB;
      m_DijkringId = dijkringId.Trim();
      m_DijkringNaam = m_OptimaliseRingDB.DijkringNaam(dijkringId).Trim();
      m_DijkringDeel = m_OptimaliseRingDB.DijkringDeelNaam(dijkringId, deel).Trim();
      m_Traject = traject;
      m_Naam = naam.Trim();

      Initialize(dijkringId, deel, traject, klimaatScenarioEnFysischMaxAfvoer, parametersKostenfunctie);

      m_Kansjaar = m_OptimaliseRingDB.KansJaar();
      m_Schadejaar = m_OptimaliseRingDB.SchadeJaar();
      m_Investering = new List<Investering>();

    }


    public DijkringTraject(OptimaliseRingDB optimaliseRingDB,
    int dijkringId, int deel, int traject, string naam
   , int klimaatScenarioEnFysischMaxAfvoer, int parametersKostenfunctie, bool iskering)
    {
      m_OptimaliseRingDB = optimaliseRingDB;
      //lees de afwijkende trajectrecords van de keringen
      m_DijkringId = dijkringId.ToString();
      m_DijkringNaam = naam.Trim();
      m_DijkringDeel = deel.ToString();
      m_Traject = traject;
      m_Naam = naam.Trim();
      m_Iskering = iskering;

      InitializeKering(dijkringId, deel, traject, klimaatScenarioEnFysischMaxAfvoer, parametersKostenfunctie, true);

      m_Kansjaar = m_OptimaliseRingDB.KansJaar();
      m_Schadejaar = m_OptimaliseRingDB.SchadeJaar();
      m_Investering = new List<Investering>();

    }


    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:DijkringTraject"/> class.
    /// </summary>
    /// <param name="optimaliseRingDB">The optimalise ring DB.</param>
    /// <param name="dijkringId">Dijkring identificatie</param>
    /// <param name="dijkringNaam">Naam van de dijkring</param>
    /// <param name="dijkringDeelNaam">Naam van het dijkringdeel</param>
    /// <param name="traject">Trajectnummer</param>
    /// <param name="naam">Naam van het traject</param>
    /// <param name="deelNummer">The deel nummer.</param>
    public DijkringTraject(OptimaliseRingDB optimaliseRingDB
      , string dijkringId, string dijkringNaam, string dijkringDeelNaam, int traject, string naam
      , int deelNummer)
    {
      m_OptimaliseRingDB = optimaliseRingDB;
      m_DijkringId = dijkringId.Trim();
      m_DijkringNaam = dijkringNaam.Trim();
      m_DijkringDeel = dijkringDeelNaam.Trim();
      m_Traject = traject;
      m_Naam = naam.Trim();
      m_Investering = new List<Investering>();
    }

    /// <summary>
    /// Initializes the specified dijkring id.
    /// </summary>
    /// <param name="dijkringId">Dijkring identificatie</param>
    /// <param name="deel">Dijkringdeel identificatie</param>
    /// <param name="traject">Dijkringtraject identificatie</param>
    /// <param name="klimaatScenarioEnFysischMaxAfvoer">The klimaat scenario en fysisch max afvoer.</param>
    /// <param name="parametersKostenfunctie">The parameters kostenfunctie.</param>
    public void Initialize(string dijkringId, int deel, int traject
      , int klimaatScenarioEnFysischMaxAfvoer, int parametersKostenfunctie)
    {
      m_OptimaliseRingDB.DijkringTrajectRecord(dijkringId, deel, traject, ref m_H0, ref m_Factor);

      // Structurele stijging relatieve waterstand	cm/jaar
      m_OptimaliseRingDB.KlimaatScenarioEnFysischMaxAfvoerDataRecord(dijkringId, deel, traject,
        klimaatScenarioEnFysischMaxAfvoer, ref m_AlphaOverstromingskans, ref m_P0Overstromingskans,
        ref m_AlphaOverschrijdingskans, ref m_P0Overschrijdingskans, ref m_Eta);

      m_OptimaliseRingDB.ParametersKostenfunctieRecord(dijkringId, deel, traject
        , parametersKostenfunctie, ref this.m_C_exp, ref this.m_b_exp, ref this.m_lambda_exp
        , ref this.m_c_kwad, ref this.m_b_kwad, ref this.m_a_kwad, ref this.m_Omega);
    }

    public void InitializeKering(int dijkringId, int deel, int traject
   , int klimaatScenarioEnFysischMaxAfvoer, int parametersKostenfunctie, bool iskering)
    {
      m_OptimaliseRingDB.KeringRecord(dijkringId, ref m_H0, ref m_Factor);

      // Structurele stijging relatieve waterstand	cm/jaar
      m_OptimaliseRingDB.KlimaatScenarioEnFysischMaxAfvoerDataRecord(dijkringId.ToString(), deel, dijkringId,
        klimaatScenarioEnFysischMaxAfvoer, ref m_AlphaOverstromingskans, ref m_P0Overstromingskans,
        ref m_AlphaOverschrijdingskans, ref m_P0Overschrijdingskans, ref m_Eta);

      m_OptimaliseRingDB.ParametersKostenfunctieRecord(dijkringId.ToString(), deel, dijkringId
        , parametersKostenfunctie, ref this.m_C_exp, ref this.m_b_exp, ref this.m_lambda_exp
        , ref this.m_c_kwad, ref this.m_b_kwad, ref this.m_a_kwad, ref this.m_Omega);
    }

    #endregion Constructors --------------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Dijkring traject nummer
    /// </summary>
    /// <value>Dijkring traject nummer</value>
    public int Traject
    {
      get { return m_Traject; }
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
    /// Dijkringdeelnaam
    /// </summary>
    /// <value>Dijkringdeelnaam</value>
    public string DijkringDeel
    {
      get { return m_DijkringDeel; }
    }

    /// <summary>
    /// Dijkringtrajectnaam.
    /// </summary>
    /// <value>Dijkringtrajectnaam</value>
    public string Naam
    {
      get { return m_Naam; }
    }

    /// <summary>
    /// Gets the kansjaar.
    /// </summary>
    /// <value>The kansjaar.</value>
    public int Kansjaar
    {
      get { return this.m_Kansjaar; }
    }

    /// <summary>
    /// Dijkverhoging in startjaar [cm+NAP]
    /// </summary>
    /// <value>Dijkverhoging in startjaar [cm+NAP]</value>
    public double H0
    {
      get { return m_H0; }
    }

    /// <summary>
    /// Dijkringdeel /traject factor
    /// </summary>
    /// <value>The factor.</value>
    public double Factor
    {
      get { return m_Factor; }
      set { m_Factor = value; }
    }

    /// <summary>
    /// Overstromingskans in startjaar zonder verhoging [1/jaar]
    /// </summary>
    /// <value>Overstromingskans in startjaar zonder verhoging [1/jaar]</value>
    public double P0Overstromingskans
    {
      get { return m_P0Overstromingskans; }
    }

    /// <summary>
    /// Gets the overstromingskans in opgegeven jaar.
    /// </summary>
    /// <param name="jaar">The jaar.</param>
    /// <param name="factorKans">The factor kans.</param>
    /// <returns></returns>
    public double GetOverstromingskansInOpgegevenJaar(int jaar, double factorKans)
    {
      Int32 indexJaar = jaar - this.m_Kansjaar;
      return Function.Pt(indexJaar, m_Eta, 0.0, m_AlphaOverstromingskans, P0Overstromingskans, factorKans);
    }

    /// <summary>
    /// Schaalparameter (exponentiele verdeling) [1/cm]
    /// </summary>
    /// <value>Schaalparameter (exponentiele verdeling) [1/cm] per traject voor overstromingskans</value>
    public double AlphaOverstromingskans
    {
      get { return m_AlphaOverstromingskans; }
    }

    /// <summary>
    /// Overschrijdingskans in jaar van opgave [1/jaar]
    /// </summary>
    /// <value>Overschrijdingskans in startjaar zonder verhoging [1/jaar]</value>
    public double P0Overschrijdingskans
    {
      get { return m_P0Overschrijdingskans; }
    }

    /// <summary>
    /// Gets or sets the initial_flood_probability.
    /// </summary>
    /// <value>The initial_flood_probability.</value>
    public double Initial_flood_probability
    {
      get { return m_Initial_flood_probability; }
      set { m_Initial_flood_probability = value; }
    }

    /// <summary>
    /// Bereken de feitelijke overstromingskans.
    /// </summary>
    /// <param name="jaar">Jaar.</param>
    /// <returns></returns>
    /// <value>The feitelijke overstromingskans.</value>
    public double GetOverschrijdingskansInOpgegevenJaar(int jaar, double factorKans)
    {
      Int32 indexJaar = jaar - m_Kansjaar;
      return Function.Pt(indexJaar, m_Eta, 0.0, m_AlphaOverschrijdingskans, P0Overschrijdingskans, factorKans);
    }

    /// <summary>
    /// Schaalparameter (exponentiele verdeling) [1/cm]
    /// </summary>
    /// <value>Schaalparameter (exponentiele verdeling) [1/cm] per traject voor overschrijdingskans</value>
    public double AlphaOverschrijdingskans
    {
      get { return m_AlphaOverschrijdingskans; }
    }

    /// <summary>
    /// Structurele stijging relatieve waterstand	cm/jaar
    /// </summary>
    /// <value>Structurele stijging relatieve waterstand	cm/jaar</value>
    public double Eta
    {
      get { return m_Eta; }
    }

    /// <summary>
    /// Vaste kosten van de investeringen	M€/cm
    /// </summary>
    /// <value>Vaste kosten van de investeringen	M€/cm</value>
    public double C_exp
    {
      get { return m_C_exp; }
    }

    /// <summary>
    /// Variabele kosten van de investeringen	M€/cm
    /// </summary>
    /// <value>Variabele kosten van de investeringen	M€/cm</value>
    public double b_exp
    {
      get { return m_b_exp; }
    }

    /// <summary>
    /// Schaalparameter van verhogingen	1/cm
    /// </summary>
    /// <value>Schaalparameter van verhogingen	1/cm</value>
    public double lambda_exp
    {
      get { return m_lambda_exp; }
    }

    /// <summary>
    /// Vaste kosten van de investeringen	M€/cm
    /// </summary>
    /// <value>Vaste kosten van de investeringen	M€/cm</value>
    public double c_kwad
    {
      get { return m_c_kwad; }
    }

    /// <summary>
    /// Variabele kosten van de investeringen	M€/cm
    /// </summary>
    /// <value>Variabele kosten van de investeringen	M€/cm</value>
    public double b_kwad
    {
      get { return m_b_kwad; }
    }

    /// <summary>
    /// A
    /// </summary>
    /// <value>A</value>
    public double a_kwad
    {
      get { return m_a_kwad; }
    }

    /// <summary>
    /// Beheer en onderhoudskosten als percentage van de investeringskosten [%]
    /// </summary>
    /// <value>Beheer en onderhoudskosten als percentage van de investeringskosten [%]</value>
    public double Omega
    {
      get { return m_Omega; }
    }

    /// <summary>
    ///Optimale jaar voor eerste investering.
    /// </summary>
    /// <value>Optimale jaar voor eerste investering.</value>
    public int OptimaleJaarVoorEersteInvestering
    {
      set { m_OptimaleJaarVoorEersteInvestering = value; }
      get { return m_OptimaleJaarVoorEersteInvestering; }
    }

    /// <summary>
    /// Verdisconteerde kosten eerste investeringen.
    /// </summary>
    /// <value>Kosten eerste investeringen.</value>
    public double VerdisconteerdeKostenEersteInvesteringen
    {
      set { m_VerdisconteerdeKostenEersteInvestering = value; }
      get { return m_VerdisconteerdeKostenEersteInvestering; }
    }

    /// <summary>
    /// Kosten eerste investeringen.
    /// </summary>
    /// <value>Kosten eerste investeringen.</value>
    public double KostenEersteInvesteringenQ
    {
      set { m_KostenEersteInvesteringen = value; }
      get { return m_KostenEersteInvesteringen; }
    }

    /// <summary>
    ///Periode tussen verhogingen.
    /// </summary>
    /// <value>Periode tussen verhogingen.</value>
    public int PeriodeTussenVerhogingen
    {
      set { m_PeriodeTussenVerhogingen = value; }
      get { return m_PeriodeTussenVerhogingen; }
    }

    /// <summary>
    /// Hoogte van eerste verhoging.
    /// </summary>
    /// <value>Hoogte van eerste verhoging.</value>
    public double HoogteVanEersteVerhoging
    {
      set { m_HoogteVanEersteVerhoging = value; }
      get { return m_HoogteVanEersteVerhoging; }
    }

    /// <summary>
    /// Hoogte van tweede en volgende verhoging.
    /// </summary>
    /// <value>Hoogte van tweede en volgende verhoging.</value>
    public double HoogteVanTweedeEnVolgendeVerhoging
    {
      set { m_HoogteVanTweedeEnVolgendeVerhoging = value; }
      get { return m_HoogteVanTweedeEnVolgendeVerhoging; }
    }

    /// <summary>
    /// Totale kosten investeringen per scenario.
    /// </summary>
    /// <value>Totale kosten investeringen.</value>
    public double[] TotaleKostenInvesteringen
    {
      set { m_TotaleKostenInvesteringen = value; }
      get { return m_TotaleKostenInvesteringen; }
    }

    /// <summary>
    /// Som van de eerdere verhogingen
    /// </summary>
    /// <value>The W.</value>
    public double W
    {
      set { m_W = value; }
      get { return m_W; }
    }

    /// <summary>
    /// Gets the investering.
    /// </summary>
    /// <value>The investering.</value>
    public List<Investering> Investeringen
    {
      get { return m_Investering; }
    }

    /// <summary>
    /// Is dit traject een B-kering
    /// </summary>
    public bool Iskering
    {
      get { return m_Iskering; }
      set { m_Iskering = value; }
    }

    /// <summary>
    /// Gets percentage toedeling
    /// </summary>
    public int Percentage
    {
      get { return m_Percentage; }
      set { m_Percentage = value; }
    }


    #endregion Properties ----------------------------------------------------

    #region Methods ----------------------------------------------------------

    /// <summary>
    /// Lees resultaten
    /// </summary>
    /// <param name="xmlReader">The XML reader.</param>
    public void Read(XmlReader xmlReader)
    {
      m_P0Overstromingskans = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "P0_Overstromingkans"));
      m_P0Overschrijdingskans = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "P0_Overschrijdingskans"));
      m_AlphaOverstromingskans = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "Alpha_Overstromingkans"));
      m_AlphaOverschrijdingskans = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "Alpha_Overschrijdingskans"));
      m_Eta = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "Eta"));
      m_H0 = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "H0"));
      m_Factor = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "Factor"));

      m_C_exp = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "C_exp"));
      m_b_exp = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "b_exp"));
      m_lambda_exp = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "lambda_exp"));
      m_c_kwad = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "c_kwad"));
      m_b_kwad = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "b_kwad"));
      m_a_kwad = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "a_kwad"));
      m_Omega = ConvertString.ToDouble(OptimaliseRing.General.Xml.ReadElement(xmlReader, "Omega"));

      xmlReader.Read();

      this.m_TotaleKostenInvesteringen = new double[0];
      if (xmlReader.Name.Contains("TotaleKostenInvesteringen"))
      {
        while (xmlReader.Read())
        {
          if (xmlReader.Name.Contains("Scenario"))
          {
            double totaleKostenInvestering = ConvertString.ToDouble(xmlReader.GetAttribute("Kosten"));
            Array.Resize(ref this.m_TotaleKostenInvesteringen, this.m_TotaleKostenInvesteringen.Length + 1);
            this.m_TotaleKostenInvesteringen[this.m_TotaleKostenInvesteringen.Length - 1] = totaleKostenInvestering;
          }
          else
          {
            break;
          }
        }
      }

      //Investeringen0
      while (xmlReader.Read())
      {
        if (xmlReader.Name.Contains("Investering"))
        {
          if (xmlReader.AttributeCount == 3)
          {
            string jaar = (string)xmlReader.GetAttribute("Jaar");
            string hoogte = (string)xmlReader.GetAttribute("Hoogte");
            string kosten = (string)xmlReader.GetAttribute("Kosten");
            double[] verdisconteerdeKostenDijkverhoging = new double[0];

            xmlReader.Read();
            if (xmlReader.Name.Contains("VerdisconteerdeKosten"))
            {
              while (xmlReader.Read())
              {
                if (xmlReader.Name.Contains("Scenario"))
                {
                  double verdisconteerdeKosten = ConvertString.ToDouble(xmlReader.GetAttribute("Kosten"));
                  Array.Resize(ref verdisconteerdeKostenDijkverhoging, verdisconteerdeKostenDijkverhoging.Length + 1);
                  verdisconteerdeKostenDijkverhoging[verdisconteerdeKostenDijkverhoging.Length - 1] = verdisconteerdeKosten;
                }
                else
                {
                  break;
                }
              }
            }
            this.Investeringen.Add(new Investering(ConvertString.ToInt32(jaar)
              , ConvertString.ToDouble(hoogte), ConvertString.ToDouble(kosten), verdisconteerdeKostenDijkverhoging));
          }
        }
        else
        {
          break;
        }
      }
    }

    /// <summary>
    /// Schrijf resultaten
    /// </summary>
    /// <param name="xmlWriter">The XML writer.</param>
    /// <param name="nummer">The nummer.</param>
    public void Write(XmlWriter xmlWriter, int nummer)
    {
      xmlWriter.WriteStartElement("Traject", null);
      xmlWriter.WriteAttributeString("Naam", Naam);

      // Instellingen
      xmlWriter.WriteElementString("P0_Overstromingkans", m_P0Overstromingskans.ToString());
      xmlWriter.WriteElementString("P0_Overschrijdingskans", m_P0Overschrijdingskans.ToString());
      xmlWriter.WriteElementString("Alpha_Overstromingkans", m_AlphaOverstromingskans.ToString());
      xmlWriter.WriteElementString("Alpha_Overschrijdingskans", m_AlphaOverschrijdingskans.ToString());
      xmlWriter.WriteElementString("Eta", m_Eta.ToString());
      xmlWriter.WriteElementString("H0", m_H0.ToString());
      xmlWriter.WriteElementString("Factor", m_Factor.ToString());

      xmlWriter.WriteElementString("C_exp", m_C_exp.ToString());
      xmlWriter.WriteElementString("b_exp", m_b_exp.ToString());
      xmlWriter.WriteElementString("lambda_exp", m_lambda_exp.ToString());
      xmlWriter.WriteElementString("c_kwad", m_c_kwad.ToString());
      xmlWriter.WriteElementString("b_kwad", m_b_kwad.ToString());
      xmlWriter.WriteElementString("a_kwad", m_a_kwad.ToString());
      xmlWriter.WriteElementString("Omega", m_Omega.ToString());

      // resultaten
      xmlWriter.WriteStartElement(string.Format("TotaleKostenInvesteringen"), null);
      for (int scenarioIndex = 0; scenarioIndex < this.m_TotaleKostenInvesteringen.Length; scenarioIndex++)
      {
        xmlWriter.WriteStartElement("Scenario" + scenarioIndex.ToString());
        xmlWriter.WriteAttributeString("Kosten", this.m_TotaleKostenInvesteringen[scenarioIndex].ToString("F3"));
        xmlWriter.WriteEndElement();
      }
      xmlWriter.WriteEndElement();

      // investeringen
      foreach (Investering investering in this.Investeringen)
      {
        // SortedList<int, double>
        xmlWriter.WriteStartElement(string.Format("Investering"), null);
        xmlWriter.WriteAttributeString("Jaar", investering.Jaar.ToString());
        xmlWriter.WriteAttributeString("Hoogte", investering.Hoogte.ToString());
        xmlWriter.WriteAttributeString("Kosten", investering.Kosten.ToString());

        xmlWriter.WriteStartElement(string.Format("VerdisconteerdeKosten"), null);
        for (int scenarioIndex = 0; scenarioIndex < investering.VerdisconteerdeKosten.Length; scenarioIndex++)
        {
          xmlWriter.WriteStartElement("Scenario" + scenarioIndex.ToString());
          xmlWriter.WriteAttributeString("Kosten", investering.VerdisconteerdeKosten[scenarioIndex].ToString("F3"));
          xmlWriter.WriteEndElement();
        }
        xmlWriter.WriteEndElement();

        xmlWriter.WriteEndElement();
      }

      xmlWriter.WriteEndElement();
      xmlWriter.Flush();
    }

    /// <summary>
    /// Kostensplitsing
    /// </summary>
    public struct Kostensplitsing
    {
      // per verhoging
      public double[] KostenInvestering;
      public double[] Inhaal;
      public double[] KlimaatInvestering;
      public double[] EconomieInvestering;

      public double TotaleKostenInvesteringen;
      public double KlimaatTotaleKosten;
      public double EconomieTotaleKosten;
      public double FractieToedelingkostenKlimaat;
      public double FractieToedelingkostenEconomie;

      public Kostensplitsing(int aantalInvesteringen)
      {
        this.KostenInvestering = new double[aantalInvesteringen];
        this.Inhaal = new double[aantalInvesteringen];
        this.KlimaatInvestering = new double[aantalInvesteringen];
        this.EconomieInvestering = new double[aantalInvesteringen];

        this.TotaleKostenInvesteringen = 0.0;
        this.KlimaatTotaleKosten = 0.0;
        this.EconomieTotaleKosten = 0.0;
        this.FractieToedelingkostenKlimaat = 0.0;
        this.FractieToedelingkostenEconomie = 0.0;
      }
    }

    /// <summary>
    /// Splits totale kosten over inhaal, economie en klimaat kosten.
    /// </summary>
    /// <param name="scenarioIndex">Index of the scenario.</param>
    /// <param name="wettelijkenorm">The wettelijkenorm.</param>
    /// <param name="gamma">Gamma.</param>
    /// <param name="psi">Extra schade per cm waterstandsverhoging.</param>
    /// <param name="maxEta">Maximale eta van alle trajecten.</param>
    /// <param name="pmiddenZichtjaar">The pmidden in zichtjaar.</param>
    /// <param name="overstromingskansInJaarVoorInvestering">The overstromingskans in jaar voor investering.</param>
    /// <param name="overstromingskansJaarVanInvestering">The overstromingskans jaar van investering.</param>
    /// <param name="eersteInvesteringjaar">The eerste investeringjaar.</param>
    /// <param name="zichtjaar">The zichtjaar.</param>
    /// <param name="factorGroeischade">The factor groeischade.</param>
    /// <returns></returns>
    public Kostensplitsing GetKostensplitsing(
      int scenarioIndex, double wettelijkenorm, double gamma, double psi, double maxEta, double pmiddenZichtjaar
      , double overstromingskansInJaarVoorInvestering, double overstromingskansJaarVanInvestering
      , int eersteInvesteringjaar, int zichtjaar, double factorGroeischade)
    {
      Kostensplitsing kostensplitsing = new Kostensplitsing(this.Investeringen.Count);

      if (Math.Abs(this.AlphaOverstromingskans * this.Eta + psi * maxEta + gamma * factorGroeischade) > 1E-6)
      {
        // Bepaal fractie van totaal over klimaat en economie
        kostensplitsing.FractieToedelingkostenKlimaat
          = (this.AlphaOverstromingskans * this.Eta + psi * maxEta)
          / (this.AlphaOverstromingskans * this.Eta + psi * maxEta + gamma * factorGroeischade);

        kostensplitsing.FractieToedelingkostenEconomie
          = gamma * factorGroeischade / (this.AlphaOverstromingskans * this.Eta + psi * maxEta + gamma * factorGroeischade);
      }
      else
      {
        kostensplitsing.FractieToedelingkostenKlimaat = 0.5;
        kostensplitsing.FractieToedelingkostenEconomie = 0.5;
      }

      // Bepaal totale kosten voor opgegeven scenario
      kostensplitsing.TotaleKostenInvesteringen = this.TotaleKostenInvesteringen[scenarioIndex];

      // achterstandNorm (A)
      double achterstandNorm = Math.Max(0, Math.Log((1.0 / wettelijkenorm)
        / (1.0 / Math.Min(1.0 / overstromingskansJaarVanInvestering, pmiddenZichtjaar))))
        / this.AlphaOverstromingskans;

      // besparingOverhoogte (wordt niet gebruikt misschien later) (D)
      double besparingOverhoogte = Math.Max(0, Math.Log((1.0 / wettelijkenorm) / overstromingskansInJaarVoorInvestering))
        / this.AlphaOverstromingskans;

      // BesparingOverhoogte moet maximaal achterstandNorm zijn.
      besparingOverhoogte = Math.Min(besparingOverhoogte, achterstandNorm);

      // Bepaal kosteninverstering
      for (int investeringIndex = 0; investeringIndex < this.Investeringen.Count; investeringIndex++)
      {
        kostensplitsing.KostenInvestering[investeringIndex] = this.Investeringen[investeringIndex].Kosten;

        // Kosten eerste investering inhaal
        kostensplitsing.Inhaal[investeringIndex]
          = (achterstandNorm / this.Investeringen[investeringIndex].Hoogte) * kostensplitsing.KostenInvestering[investeringIndex];

        // Kosten besparing t.g.v. de overhoogte eerste investering
        double kostenBesparingOverhoogte
          = (besparingOverhoogte / this.Investeringen[investeringIndex].Hoogte) * kostensplitsing.KostenInvestering[investeringIndex];

        // Kosten eerste investering niet inhaal
        double kostenInvesteringNietInhaal
          = kostensplitsing.KostenInvestering[investeringIndex] - kostensplitsing.Inhaal[investeringIndex] + kostenBesparingOverhoogte;

        // De inhaal wordt op 0 gezet als de eerste investering van het traject niet valt
        // op het moment van de allereerste investering van het dijkringdeel.
        if (investeringIndex > 0 || this.Investeringen[0].Jaar > eersteInvesteringjaar)
        {
          kostensplitsing.Inhaal[investeringIndex] = 0.0;
          kostenInvesteringNietInhaal = kostensplitsing.KostenInvestering[investeringIndex];
        }

        // Kosten investering
        kostensplitsing.KlimaatInvestering[investeringIndex]
          = kostensplitsing.FractieToedelingkostenKlimaat * kostenInvesteringNietInhaal;

        kostensplitsing.EconomieInvestering[investeringIndex]
          = kostensplitsing.FractieToedelingkostenEconomie * kostenInvesteringNietInhaal;

        if (investeringIndex == 0)
        {
          // bereken correctiefactor
          double correctiefactor = this.Investeringen[investeringIndex].Kosten /
             (kostensplitsing.Inhaal[investeringIndex]
            + kostensplitsing.KlimaatInvestering[investeringIndex]
            + kostensplitsing.EconomieInvestering[investeringIndex]);

           kostensplitsing.Inhaal[investeringIndex] = kostensplitsing.Inhaal[investeringIndex] * correctiefactor;
           kostensplitsing.KlimaatInvestering[investeringIndex] = kostensplitsing.KlimaatInvestering[investeringIndex] * correctiefactor;
           kostensplitsing.EconomieInvestering[investeringIndex] = kostensplitsing.EconomieInvestering[investeringIndex] * correctiefactor;
        }
      }

      // TODO kostensplitsing.KlimaatInvestering en EconomieInvestering in onderstaande code
      // is nog niet verdisconteerd en dat moet natuurlijk wel.


      // Totale kosten, minimaal 1 investering nodig
      ////if (this.Investeringen.Count > 0)
      ////{
      ////  kostensplitsing.KlimaatTotaleKosten = kostensplitsing.KlimaatInvestering[0]
      ////    + kostensplitsing.FractieToedelingkostenKlimaat
      ////    * (kostensplitsing.TotaleKostenInvesteringen - this.Investeringen[0].VerdisconteerdeKosten[scenarioIndex]);

      ////  kostensplitsing.EconomieTotaleKosten = kostensplitsing.EconomieInvestering[0]
      ////    + kostensplitsing.FractieToedelingkostenEconomie
      ////    * (kostensplitsing.TotaleKostenInvesteringen - this.Investeringen[0].VerdisconteerdeKosten[scenarioIndex]);
      ////}
      return kostensplitsing;
    }

    #endregion Methods ----------------------------------------------------
  }
}
