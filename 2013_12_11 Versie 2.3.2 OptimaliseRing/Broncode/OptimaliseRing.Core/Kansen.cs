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
using CenterSpace.NMath.Core;

namespace OptimaliseRing.Core
{
  /// <summary>
  /// Kansen
  /// </summary>
  public sealed class Kansen
  {
    #region Instance Variables -----------------------------------------------

    // Dijkring id
    private string m_DijkringId;

    // Dijkring naam
    private string m_DijkringNaam;

    // Dijkring deelnummer
    private int m_DeelNummer;

    // Dijkringdeelnaam
    private string m_DijkringDeel;

    // Wettelijke norm van dit dijkringdeel
    private double m_WettelijkeNorm;

    // Restrictie Pmin
    private double m_RestrictiePmin;

    /// <summary>
    /// Zichtjaar waarmee de berekening is gemaakt
    /// </summary>
    private int m_Zichtjaar;

    /// <summary>
    /// Optimale overstromingskans jaar waarmee berekening is gemaak
    /// of waarmee het resultaatbestand is geopend.
    /// </summary>
    private int m_OptimaleOverstromingskansenJaar;

    // Feitelijke overstromingskans in zichtjaar
    private double m_OverstromingskansInZichtjaar;

    // Feitelijke overstromingskans in optimaleoverstromingskansjaar
    private double m_OverstromingskansInOptimaleOverstromingskansJaar;

    // Overstromingskans in zichtjaar (Pmidden)
    private double m_OptimaleOverstromingskansInZichtjaar;

    // Eerste investeringjaar
    private int m_eersteInvesteringJaar;

    // Overstromingskans in het eerste inversteringjaar (Pmidden)
    private double m_OptimaleOverstromingskansInEersteInversteringjaar;

    // Overstromingskans in optimaleoverstromingskansjaar (Pmidden)
    private double m_OptimaleOverstromingskansInOptimaleOverstromingskansJaar;

    // Overschrijdingskans in zichtjaar
    private double m_OptimaleOverschrijdingskansInZichtjaar;

    // Overschrijdingskans in optimaleoverstromingskansjaar (Pmidden)
    private double m_OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar;

    // Contante waarde beginjaar investeringen en verwachte schade
    private double m_ContantewaardeInvesteringenEnSchade;

    // TotaalKostenInvesteringen over alle trajecten
    private double m_TotaalKostenInvesteringen;

    // Matrix data met resultaten t.b.v. de grafiek
    private DoubleMatrix m_MatrixData;

    #endregion Instance Variables --------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:Kansen"/> class.
    /// </summary>
    /// <param name="dijkringId">Dijkring identificatie</param>
    /// <param name="dijkringNaam">The dijkring naam.</param>
    /// <param name="dijkringDeel">The dijkring deel.</param>
    /// <param name="deelNummer">The deel nummer.</param>
    /// <param name="zichtjaar">The zichtjaar.</param>
    /// <param name="overstromingskansJaar">The overstromingskans jaar.</param>
    public Kansen(string dijkringId
      , string dijkringNaam, string dijkringDeel, int deelNummer, int zichtjaar, int optimaleOverstromingskansenJaar)
    {
      this.m_DijkringId = dijkringId;
      this.m_DijkringNaam = dijkringNaam;
      this.m_DijkringDeel = dijkringDeel;
      this.m_DeelNummer = deelNummer;

      this.m_WettelijkeNorm = 0.0;
      this.m_RestrictiePmin = 0.0;

      this.m_OverstromingskansInZichtjaar = 0.0;
      this.m_OverstromingskansInOptimaleOverstromingskansJaar = 0.0;

      this.m_OptimaleOverstromingskansInZichtjaar = 0.0;
      this.m_OptimaleOverstromingskansInOptimaleOverstromingskansJaar = 0.0;

      this.m_OptimaleOverschrijdingskansInZichtjaar = 0.0;
      this.m_OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar = 0.0;

      this.m_ContantewaardeInvesteringenEnSchade = 0.0;
      this.m_TotaalKostenInvesteringen = 0.0;

      this.m_eersteInvesteringJaar = 0;

      // tijdinstellingen
      this.m_Zichtjaar = zichtjaar;
      this.m_OptimaleOverstromingskansenJaar = optimaleOverstromingskansenJaar;

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Kansen"/> class.
    /// </summary>
    public Kansen()
    { }

    #endregion Constructors --------------------------------------------------

    #region Properties -------------------------------------------------------

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
    /// Gets Dijkringdeelnummer
    /// </summary>
    /// <value>Dijkringdeelnummer</value>
    public int DeelNummer
    {
      get { return this.m_DeelNummer; }
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
    /// Gets or sets Wettelijke norm.
    /// </summary>
    /// <value>Wettelijke norm.</value>
    public double WettelijkeNorm
    {
      set { this.m_WettelijkeNorm = value; }
      get { return this.m_WettelijkeNorm; }
    }

    /// <summary>
    /// Gets or sets Restrictie Pmin in beginjaar
    /// </summary>
    /// <value>Restrictie Pmin in beginjaar .</value>
    public double RestrictiePmin
    {
      set { this.m_RestrictiePmin = value; }
      get { return this.m_RestrictiePmin; }
    }

    /// <summary>
    /// Gets or sets Zichtjaar waarmee de berekening is gemaakt
    /// </summary>
    /// <value>The zichtjaar.</value>
    public int Zichtjaar
    {
      set { this.m_Zichtjaar = value; }
      get { return this.m_Zichtjaar; }
    }

    /// <summary>
    /// Gets or sets Optimale overstromingskans jaar waarmee berekening is gemaak
    /// of waarmee het resultaatbestand is geopend.
    /// </summary>
    /// <value>Optimale overstromingskans jaar.</value>
    public int OptimaleOverstromingskansenJaar
    {
      set { this.m_OptimaleOverstromingskansenJaar = value; }
      get { return this.m_OptimaleOverstromingskansenJaar; }
    }

    /// <summary>
    /// Gets or sets Feitelijke overstromingskans in Zichtjaar
    /// </summary>
    /// <value>Feitelijke overstromingskans in Zichtjaar .</value>
    public double OverstromingskansInZichtjaar
    {
      set { this.m_OverstromingskansInZichtjaar = value; }
      get { return this.m_OverstromingskansInZichtjaar; }
    }

    /// <summary>
    /// Gets or sets Feitelijke overstromingskans in OptimaleOverstromingskansJaar
    /// </summary>
    /// <value>Feitelijke overstromingskans in OptimaleOverstromingskansJaar .</value>
    public double OverstromingskansInOptimaleOverstromingskansJaar
    {
      set { this.m_OverstromingskansInOptimaleOverstromingskansJaar = value; }
      get { return this.m_OverstromingskansInOptimaleOverstromingskansJaar; }
    }

    /// <summary>
    /// Gets or sets Overstromingskans in zichtjaar (Pmidden)
    /// </summary>
    /// <value>Overstromingskans in zichtjaar (Pmidden).</value>
    public double OptimaleOverstromingskansInZichtjaar
    {
      set { this.m_OptimaleOverstromingskansInZichtjaar = value; }
      get { return this.m_OptimaleOverstromingskansInZichtjaar; }
    }

    /// <summary>
    /// Gets or sets eerste investeringjaar van het dijkringdeel.
    /// </summary>
    /// <value>The eerste investering jaar.</value>
    public int EersteInvesteringJaar
    {
      set { this.m_eersteInvesteringJaar = value; }
      get { return this.m_eersteInvesteringJaar; }
    }

    /// <summary>
    /// Gets or sets the optimale overstromingskans in eerste inversteringsjaar.
    /// </summary>
    /// <value>The optimale overstromingskans in eerste inversteringsjaar.</value>
    public double OptimaleOverstromingskansInEersteInversteringjaar
    {
      set { this.m_OptimaleOverstromingskansInEersteInversteringjaar = value; }
      get { return this.m_OptimaleOverstromingskansInEersteInversteringjaar; }
    }

    /// <summary>
    /// Gets or sets Overstromingskans in OverstromingskansJaar (Pmidden)
    /// </summary>
    /// <value>Overstromingskans in OverstromingskansJaar (Pmidden).</value>
    public double OptimaleOverstromingskansInOptimaleOverstromingskansJaar
    {
      set { this.m_OptimaleOverstromingskansInOptimaleOverstromingskansJaar = value; }
      get { return this.m_OptimaleOverstromingskansInOptimaleOverstromingskansJaar; }
    }

    /// <summary>
    /// Gets or sets Overschrijdingskans in zichtjaar
    /// </summary>
    /// <value>Overschrijdingskans in zichtjaar.</value>
    public double OptimaleOverschrijdingskansInZichtjaar
    {
      set { this.m_OptimaleOverschrijdingskansInZichtjaar = value; }
      get { return this.m_OptimaleOverschrijdingskansInZichtjaar; }
    }

    /// <summary>
    /// Gets or sets Overschrijdingskans in OptimaleOverstromingskansJaar (Pmidden)
    /// </summary>
    /// <value>Overschrijdingskans in OptimaleOverstromingskansJaar (Pmidden).</value>
    public double OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar
    {
      set { this.m_OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar = value; }
      get { return this.m_OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar; }
    }

    /// <summary>
    /// Gets or sets Contante waarde beginjaar investeringen en verwachte schade
    /// </summary>
    /// <value>Contante waarde beginjaar investeringen en verwachte schade.</value>
    public double ContantewaardeInvesteringenEnSchade
    {
      set { this.m_ContantewaardeInvesteringenEnSchade = value; }
      get { return this.m_ContantewaardeInvesteringenEnSchade; }
    }

    /// <summary>
    /// Gets or sets the totaal kosten investeringen.
    /// </summary>
    /// <value>The totaal kosten investeringen.</value>
    public double TotaalKostenInvesteringen
    {
      set { this.m_TotaalKostenInvesteringen = value; }
      get { return this.m_TotaalKostenInvesteringen; }
    }

    /// <summary>
    /// Matrixdata.
    /// </summary>
    /// <value>Matrixdata</value>
    public DoubleMatrix MatrixData
    {
      set { this.m_MatrixData = value; }
      get { return this.m_MatrixData; }
    }

    /// <summary>
    /// Bepaals the kans voor kleuring op kaart.
    /// </summary>
    /// <param name="viewMode">The view mode.</param>
    /// <returns></returns>
    public string BepaalKansVoorKleuringOpKaart(ViewMode viewMode)
    {
      switch (viewMode)
      {
        case ViewMode.Overstromingskans:
          return string.Format("1/{0:F0}", this.OptimaleOverstromingskansInOptimaleOverstromingskansJaar);
        case ViewMode.Overschrijdingskans:
          return string.Format("1/{0:F0}", this.OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar);
        default:
          return string.Empty;
      }
    }

    #endregion Properties ----------------------------------------------------

    #region Member functions -------------------------------------------------

    /// <summary>
    /// Resets the values.
    /// </summary>
    public void ResetValues()
    {

      this.m_OverstromingskansInZichtjaar = 0.0;
      this.m_OverstromingskansInOptimaleOverstromingskansJaar = 0.0;

      this.m_OptimaleOverstromingskansInZichtjaar = 0.0;
      this.m_OptimaleOverstromingskansInEersteInversteringjaar = 0.0;
      this.m_OptimaleOverstromingskansInOptimaleOverstromingskansJaar = 0.0;

      this.m_OptimaleOverschrijdingskansInZichtjaar = 0.0;
      this.m_OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar = 0.0;

      this.m_ContantewaardeInvesteringenEnSchade = 0.0;
      this.m_TotaalKostenInvesteringen = 0.0;
    }

    #endregion Member functions ----------------------------------------------
  }
}
