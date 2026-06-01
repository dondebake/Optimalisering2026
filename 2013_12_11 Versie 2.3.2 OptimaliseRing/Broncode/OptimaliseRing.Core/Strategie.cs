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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.Core/Strategie.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using OptimaliseRing.General;
using CenterSpace.NMath.Core;

namespace OptimaliseRing.Core
{
  /// <summary>
  /// Strategie
  /// </summary>
  public sealed class Strategie
  {
    #region Instance Variables -----------------------------------------------

    // Trajectnummer
    private int m_TrajectNummer;

    // Dijkring id
    private string m_DijkringId;

    // Dijkring naam
    private string m_DijkringNaam;

    // Dijkringdeel naam
    private string m_DijkringDeel;

    // Dijkringtraject naam
    private string m_Traject;

    // Jaar voor eerste verhoging
    private int m_JaarEersteVerhoging;

    // Hoogte van eerste verhoging
    private double m_HoogteEersteVerhoging;

    // Absoluut kosten eerste investering
    private double m_KostenEersteVerhoging;

    // Kosten eerste investering t.g.v. normen achterstand
    private double m_KostenEersteVerhogingNormenAchterstand;

    // Kosten eerste investering t.g.v. economie
    private double m_KostenEersteVerhogingEconomie;

    // Kosten eerste investering t.g.v. klimaat
    private double m_KostenEersteVerhogingKlimaat;

    // Jaar voor tweede verhoging
    private int m_JaarTweedeVerhoging;

    // Hoogte van tweede verhoging
    private double m_HoogteTweedeVerhoging;

    // Kosten tweede investering
    private double m_KostenTweedeVerhoging;

    // Kosten tweede investering t.g.v. economie
    private double m_KostenTweedeVerhogingEconomie;

    // Kosten tweede investering t.g.v. klimaat
    private double m_KostenTweedeVerhogingKlimaat;

    // Contante waarde investerings kosten
    private double m_ContantewaardeInvesteringskosten;

    #endregion Instance Variables --------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:Strategie"/> class.
    /// </summary>
    /// <param name="trajectNummer">Het trajectnummer.</param>
    /// <param name="dijkringId">Dijkring identificatie</param>
    /// <param name="dijkringNaam">Naam van de dijkring</param>
    /// <param name="dijkringDeel">Naam van het dijkringdeel</param>
    /// <param name="traject">Trajectnummer</param>
    public Strategie(
      int trajectNummer,
      string dijkringId,
      string dijkringNaam,
      string dijkringDeel,
      string traject)
    {
      this.m_TrajectNummer = trajectNummer;
      this.m_DijkringId = dijkringId;
      this.m_DijkringNaam = dijkringNaam;
      this.m_DijkringDeel = dijkringDeel;
      this.m_Traject = traject;

      this.m_JaarEersteVerhoging = 0;
      this.m_HoogteEersteVerhoging = 0.0;
      this.m_KostenEersteVerhoging = 0.0;
      this.m_KostenEersteVerhogingNormenAchterstand = 0.0;
      this.m_KostenEersteVerhogingEconomie = 0.0;
      this.m_KostenEersteVerhogingKlimaat = 0.0;
      this.m_JaarTweedeVerhoging = 0;
      this.m_HoogteTweedeVerhoging = 0.0;
      this.m_KostenTweedeVerhoging = 0.0;
      this.m_KostenTweedeVerhogingEconomie = 0.0;
      this.m_KostenTweedeVerhogingKlimaat = 0.0;
      this.m_ContantewaardeInvesteringskosten = 0.0;
    }

    #endregion Constructors --------------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets Trajectnummer.
    /// </summary>
    /// <value>The traject nummer.</value>
    public int TrajectNummer
    {
      get { return this.m_TrajectNummer; }
    }

    /// <summary>
    /// Gets Dijkring identificatie
    /// </summary>
    /// <value>dijkring identificatie</value>
    public string DijkringId
    {
      get { return this.m_DijkringId; }
    }

    /// <summary>
    /// Gets Dijkringnaam.
    /// </summary>
    /// <value>Dijkringnaam</value>
    public string DijkringNaam
    {
      get { return this.m_DijkringNaam; }
    }

    /// <summary>
    /// Gets Dijkringdeelnaam
    /// </summary>
    /// <value>Dijkringdeelnaam</value>
    public string Deel
    {
      get { return this.m_DijkringDeel; }
    }

    /// <summary>
    /// Gets Dijkringtrajectnaam
    /// </summary>
    /// <value>Dijkringtrajectnaam</value>
    public string Traject
    {
      get { return this.m_Traject; }
    }

    /// <summary>
    /// Gets or sets Jaar eerste verhoging.
    /// </summary>
    /// <value>Jaar eerste verhoging.</value>
    public int JaarEersteVerhoging
    {
      get { return this.m_JaarEersteVerhoging; }
      set { this.m_JaarEersteVerhoging = value; }
    }

    /// <summary>
    /// Gets or sets Hoogte van eerste verhoging.
    /// </summary>
    /// <value>Hoogte van eerste verhoging.</value>
    public double HoogteEersteVerhoging
    {
      get { return this.m_HoogteEersteVerhoging; }
      set { this.m_HoogteEersteVerhoging = value; }
    }

    /// <summary>
    /// Gets or sets Absoluut kosten eerste investering.
    /// </summary>
    /// <value>Absoluut kosten eerste investering.</value>
    public double KostenEersteVerhoging
    {
      get { return this.m_KostenEersteVerhoging; }
      set { this.m_KostenEersteVerhoging = value; }
    }

    /// <summary>
    /// Gets or sets Kosten eerste investering t.g.v. economie.
    /// </summary>
    /// <value>Kosten eerste investering t.g.v. economie.</value>
    public double KostenEersteVerhogingEconomie
    {
      get { return this.m_KostenEersteVerhogingEconomie; }
      set { this.m_KostenEersteVerhogingEconomie = value; }
    }

    /// <summary>
    /// Gets or sets Kosten eerste investering t.g.v. klimaat.
    /// </summary>
    /// <value>Kosten eerste investering t.g.v. klimaat.</value>
    public double KostenEersteVerhogingKlimaat
    {
      get { return this.m_KostenEersteVerhogingKlimaat; }
      set { this.m_KostenEersteVerhogingKlimaat = value; }
    }

    /// <summary>
    /// Gets or sets Kosten eerste investering t.g.v. normen achterstand.
    /// </summary>
    /// <value>The kosten eerste verhoging normen achterstand.</value>
    public double KostenEersteVerhogingNormenAchterstand
    {
      get { return this.m_KostenEersteVerhogingNormenAchterstand; }
      set { this.m_KostenEersteVerhogingNormenAchterstand = value; }
    }

    /// <summary>
    /// Gets or sets Jaar voor tweede verhoging.
    /// </summary>
    /// <value>Jaar voor tweede verhoging.</value>
    public int JaarTweedeVerhoging
    {
      get { return this.m_JaarTweedeVerhoging; }
      set { this.m_JaarTweedeVerhoging = value; }
    }

    /// <summary>
    /// Gets or sets Hoogte van tweede verhoging.
    /// </summary>
    /// <value>Hoogte van tweede verhoging.</value>
    public double HoogteTweedeVerhoging
    {
      get { return this.m_HoogteTweedeVerhoging; }
      set { this.m_HoogteTweedeVerhoging = value; }
    }

    /// <summary>
    /// Gets or sets Kosten tweede investering.
    /// </summary>
    /// <value>Kosten tweede investering.</value>
    public double KostenTweedeVerhoging
    {
      get { return this.m_KostenTweedeVerhoging; }
      set { this.m_KostenTweedeVerhoging = value; }
    }

    /// <summary>
    /// Gets or sets Kosten tweede investering t.g.v. economie.
    /// </summary>
    /// <value>Kosten tweede investering t.g.v. economie.</value>
    public double KostenTweedeVerhogingEconomie
    {
      get { return this.m_KostenTweedeVerhogingEconomie; }
      set { this.m_KostenTweedeVerhogingEconomie = value; }
    }

    /// <summary>
    /// Gets or sets Kosten tweede investering t.g.v. klimaat.
    /// </summary>
    /// <value>Kosten tweede investering t.g.v. klimaat.</value>
    public double KostenTweedeVerhogingKlimaat
    {
      get { return this.m_KostenTweedeVerhogingKlimaat; }
      set { this.m_KostenTweedeVerhogingKlimaat = value; }
    }

    /// <summary>
    /// Gets or sets Contante waarde investerings kosten.
    /// </summary>
    /// <value>Contante waarde investerings kosten.</value>
    public double ContantewaardeInvesteringskosten
    {
      get { return this.m_ContantewaardeInvesteringskosten; }
      set { this.m_ContantewaardeInvesteringskosten = value; }
    }

    #endregion Properties ----------------------------------------------------
  }
}
