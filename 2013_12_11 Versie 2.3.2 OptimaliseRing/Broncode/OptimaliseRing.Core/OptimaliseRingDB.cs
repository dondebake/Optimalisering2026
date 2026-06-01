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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.Core/OptimaliseRingDB.cs 3     28-04-09 13:58 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Security.Permissions;

using OptimaliseRing.General;
using OptimaliseRing.DataAccess;

using CenterSpace.NMath.Core;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]
namespace OptimaliseRing.Core
{
  /// <summary>
  /// OptimaliseRing database
  /// </summary>
  public partial class OptimaliseRingDB : MyOleDB
  {
    private SortedList m_CompartimenteringsDijken;
    private ApplicationError m_ApplicationError;

    public OptimaliseRingDB(ApplicationError applicationError)
    {
      m_ApplicationError = applicationError;
    }

    /// <summary>
    /// Open de database.
    /// </summary>
    /// <param name="filename">De bestandsthis.projectFile.</param>
    /// <param name="connectionString">De connectie string.</param>
    public override void Open(string filename, string connectionString)
    {
      try
      {
        // Gebruik de basis-klasse om database te openen
        base.Open(filename, connectionString);

        // Test hier of database wel een juiste database is
        if (!this.TableExists("Dijkringen"))
        {
          m_ApplicationError.Raise(2001, new object[] { filename });
        }
      }
      catch (System.Data.OleDb.OleDbException oleDbException)
      {
        if (oleDbException.ErrorCode == -2147467259)
        {
          m_ApplicationError.Raise(2003, new object[] { filename });
        }
        else
        {
          m_ApplicationError.Raise(1004, new object[] { oleDbException.Message });
        }
      }

    }

    /// <summary>
    /// Dataset volgend uit het SQL commando
    /// </summary>
    /// <param name="sqlSelect">The SQL select.</param>
    /// <returns></returns>
    public MyDataSet GetDataset(string sqlSelect)
    {
      return this.GetDataSet(sqlSelect);
    }

    /// <summary>
    /// Dataset met Dijkringdelen van deze dijkring
    /// </summary>
    /// <param name="dijkring">Dijkring identificatie</param>
    /// <returns></returns>
    public MyDataSet Dijkring(string dijkring)
    {
      return this.GetDataset("SELECT * FROM Dijkringen WHERE Dijkring = '" + dijkring + "';");
    }

    /// <summary>
    /// Dijkrings the specified compartimenteringsdijk.
    /// </summary>
    /// <param name="compartimenteringsdijk">The compartimenteringsdijk.</param>
    /// <param name="compartimenteringsdeel">The compartimenteringsdeel.</param>
    /// <returns></returns>
    public MyDataSet Dijkring(string compartimenteringsdijk, string compartimenteringsdeel)
    {
      return this.GetDataset("SELECT * FROM Dijkringen WHERE Compartimenteringsdijk = '"
         + compartimenteringsdijk + "' AND Compartimenteringsdeel = '" + compartimenteringsdeel + "';");
    }

    /// <summary>
    /// Bepaal de dijkringnaam
    /// </summary>
    /// <param name="dijkring">The dijkring.</param>
    /// <returns></returns>
    public string DijkringNaam(string dijkring)
    {
      string naam = "";
      SortedList records = this.Dijkring(dijkring).ToSortedList();
      if (records.Count == 1)
      {
        SortedList record = (SortedList)records.GetByIndex(0);
        if (record["Naam"].ToString().Length > 0)
        {
          naam = record["Naam"].ToString();
        }
      }
      else
      {
        m_ApplicationError.Raise(2003, new object[] { dijkring });
      }

      return naam;
    }

    /// <summary>
    /// Bepaal de dijkringdeelnaam
    /// </summary>
    /// <param name="dijkring">The dijkring.</param>
    /// <param name="deel">The deel.</param>
    /// <returns></returns>
    public string DijkringDeelNaam(string dijkring, int deel)
    {
      string naam = "";
      string sqlSelect = "SELECT * FROM DijkringDelen";
      sqlSelect += " WHERE Dijkring = '" + dijkring + " '";
      sqlSelect += " AND DijkringDeel = " + deel.ToString() + ";";

      SortedList records = this.GetDataset(sqlSelect).ToSortedList();
      if (records.Count == 1)
      {
        SortedList record = (SortedList)records.GetByIndex(0);
        if (record["Naam"].ToString().Length > 0)
        {
          naam = record["Naam"].ToString();
        }
      }
      else
      {
        m_ApplicationError.Raise(2002, new object[] { sqlSelect });
      }

      return naam;
    }

    public SortedList KeringInDijkringdeel(string naam1, string naam2, string dijkring, int deel)
    {
      string naam = "";
      string sqlSelect = "SELECT " + naam1 + " as Dijkring, " + naam2 + " as DijkringDeel, [Id] as DijkringTraject, Naam, H0, Factor FROM [B-keringen]";
      sqlSelect += " WHERE " + naam1 + " = '" + dijkring + " '";
      sqlSelect += " AND " + naam2 + " = " + deel.ToString() + " ORDER BY [B-kering];";

      SortedList record = this.GetDataset(sqlSelect).ToSortedList();
      //Id?
      //Dijkring
      //DijkringDeel
      //DijkringTraject
      //Naam
      //H0
      //Factor

      return record;
    }

    /// <summary>
    /// Dataset met Dijkringdelen van deze dijkring
    /// </summary>
    /// <param name="dijkring">Dijkring identificatie</param>
    /// <returns></returns>
    public MyDataSet DijkringDelen(string dijkring)
    {
      MyDataSet dataset = this.GetDataset("SELECT * FROM DijkringDelen WHERE Dijkring = '" + dijkring + "' ORDER BY INDEX;");

      return dataset;
    }

    /// <summary>
    /// Tel de Dijkringdelen van deze dijkring
    /// </summary>
    /// <param name="dijkring">Aantal Dijkringdelen</param>
    /// <returns></returns>
    public int DijkringAantal(string dijkring)
    {
      //SortedList aantal = this.GetDataset("SELECT COUNT(Dijkringdeel) FROM DijkringDelen WHERE Dijkring = '" + dijkring + "';").ToSortedList();
      MyDataSet aantal = this.GetDataset("SELECT COUNT(Dijkringdeel) FROM DijkringDelen WHERE Dijkring = '" + dijkring + "';");
      DataRow dr = aantal.Tables[0].Rows[0];
      return (int)dr.ItemArray[0];

    }
    /// <summary>
    /// Dataset met Dijkringtrajecten van de combinatie dijkring en deel
    /// </summary>
    /// <param name="dijkring">Dijkring identificatie</param>
    /// <param name="deel">Dijkringdeel identificatie</param>
    /// <returns></returns>
    public MyDataSet DijkringTrajecten(string dijkring, int deel)
    {
      string sqlSelect = "SELECT * FROM DijkringTrajecten";
      sqlSelect += " WHERE Dijkring = '" + dijkring + " '";
      sqlSelect += " AND DijkringDeel = " + deel.ToString() + " ORDER BY DijkringTraject;";

      return this.GetDataset(sqlSelect);
    }

    /// <summary>
    /// Lees de dataset met investeringskostenparameters
    /// </summary>
    /// <param name="dijkring">Dijkring identificatie</param>
    /// <param name="deel">Dijkringdeel identificatie</param>
    /// <param name="traject">Dijkringtraject identificatie</param>
    /// <param name="h0">Dijkverhoging in startjaar [cm+NAP]</param>
    /// <param name="factor">The factor.</param>
    public void DijkringTrajectRecord(string dijkring, int deel, int traject, ref double h0, ref double factor)
    {
      string sqlSelect = "SELECT * FROM DijkringTrajecten";
      sqlSelect += " WHERE Dijkring = '" + dijkring + " '";
      sqlSelect += " AND DijkringDeel = " + deel.ToString();
      sqlSelect += " AND DijkringTraject = " + traject.ToString();
      SortedList records = this.GetDataset(sqlSelect).ToSortedList();
      if (records.Count == 1)
      {
        SortedList record = (SortedList)records.GetByIndex(0);
        if (record["H0"].ToString().Length > 0)
        {
          h0 = ConvertString.ToDouble(record["H0"].ToString());
        }

        if (record["Factor"].ToString().Length > 0)
        {
          factor = ConvertString.ToDouble(record["Factor"].ToString());
        }
      }
      else
      {
        m_ApplicationError.Raise(2002, new object[] { sqlSelect });
      }

    }

    public void KeringRecord(int dijkring, ref double h0, ref double factor)
    {
      string sqlSelect = "SELECT * FROM [B-keringen]";
      sqlSelect += " WHERE Id = " + dijkring;

      SortedList records = this.GetDataset(sqlSelect).ToSortedList();
      if (records.Count == 1)
      {
        SortedList record = (SortedList)records.GetByIndex(0);
        if (record["H0"].ToString().Length > 0)
        {
          h0 = ConvertString.ToDouble(record["H0"].ToString());
        }

        if (record["Factor"].ToString().Length > 0)
        {
          factor = ConvertString.ToDouble(record["Factor"].ToString());
        }
      }
      else
      {
        m_ApplicationError.Raise(2002, new object[] { sqlSelect });
      }
    }

    public bool KeringHasRecords()
    {
      string sqlSelect = "SELECT * FROM [B-keringen]";
      bool hasrecords = false;

      SortedList records = this.GetDataset(sqlSelect).ToSortedList();
      if (records.Count > 0)
      {
        hasrecords = true;
      }
      else
      {
        hasrecords = false;
      }
      return hasrecords;
    }
    /// <summary>
    /// Test of de selectie aanwezig is in de database
    /// </summary>
    /// <param name="dijkring">Dijkring identificatie</param>
    /// <param name="deel">Dijkringdeel identificatie</param>
    /// <returns></returns>
    public bool SelectieAanwezig(string dijkring, int deel)
    {
      bool selectieAanwezig = false;

      string sqlSelect = "SELECT * FROM DijkringDelen";
      sqlSelect += " WHERE Dijkring = '" + dijkring + " '";
      sqlSelect += " AND DijkringDeel = " + deel.ToString();
      SortedList records = this.GetDataset(sqlSelect).ToSortedList();
      if (records.Count == 1)
      {
        selectieAanwezig = true;
      }
      return selectieAanwezig;
    }

    /// <summary>
    /// Lees het jaar van schadeopgave
    /// </summary>
    /// <returns>SchadeJaar</returns>
    public int SchadeJaar()
    {
      int beginJaar = 0;
      SortedList records = this.GetDataset("SELECT BeginJaar_s FROM BeginJaar;").ToSortedList();
      if (records.Count == 1)
      {
        SortedList record = (SortedList)records.GetByIndex(0);
        if (record["BeginJaar_s"].ToString().Length > 0)
        {
          beginJaar = ConvertString.ToInt32(record["BeginJaar_s"].ToString());
        }
      }
      else
      {
        m_ApplicationError.Raise(2002, new object[] { "SELECT BeginJaar_s FROM BeginJaar;" });
      }

      return beginJaar;
    }

    /// <summary>
    /// Lees het jaar van kansopgave
    /// </summary>
    /// <returns>KansJaar</returns>
    public int KansJaar()
    {
      int beginJaar = 0;
      SortedList records = this.GetDataset("SELECT BeginJaar_p FROM BeginJaar;").ToSortedList();
      if (records.Count == 1)
      {
        SortedList record = (SortedList)records.GetByIndex(0);
        if (record["BeginJaar_p"].ToString().Length > 0)
        {
          beginJaar = ConvertString.ToInt32(record["BeginJaar_p"].ToString());
        }
      }
      else
      {
        m_ApplicationError.Raise(2002, new object[] { "SELECT BeginJaar_p FROM BeginJaar;" });
      }

      return beginJaar;
    }

    /// <summary>
    /// Lees de dataset met investeringskostenparameters
    /// </summary>
    /// <param name="dijkring">Dijkring identificatie</param>
    /// <param name="deel">Dijkringdeel identificatie</param>
    /// <param name="dijkringTraject">Dijkringtraject identificatie</param>
    /// <param name="parametersKostenfunctieId">Parameterskostenfunctie identificatie</param>
    /// <param name="C_exp">Vaste kosten van de investeringen	M€/c</param>
    /// <param name="b_exp">Variabele kosten van de investeringen	[M€/cm]</param>
    /// <param name="lambda_exp">Schaalparameter van verhogingen	[1/cm]</param>
    /// <param name="c_kwad">Vaste kosten van de investeringen	M€/c</param>
    /// <param name="b_kwad">Variabele kosten van de investeringen	[M€/cm]</param>
    /// <param name="a_kwad">a</param>
    /// <param name="omega">Beheer en onderhoudskosten als percentage van de investeringskosten [%]</param>
    public void ParametersKostenfunctieRecord(string dijkring, int deel, int dijkringTraject
      , int parametersKostenfunctieId, ref double C_exp, ref double b_exp, ref double lambda_exp
      , ref double c_kwad, ref double b_kwad, ref double a_kwad, ref double omega)
    {
      string sqlSelect = "SELECT * FROM ParametersKostenfunctieData";
      sqlSelect += " WHERE Dijkring = '" + dijkring + " '";
      sqlSelect += " AND DijkringDeel = " + deel.ToString();
      sqlSelect += " AND DijkringTraject = " + dijkringTraject.ToString();
      sqlSelect += " AND ParametersKostenfunctieId = " + parametersKostenfunctieId.ToString() + ";";
      SortedList records = this.GetDataset(sqlSelect).ToSortedList();

      if (records.Count == 1)
      {
        SortedList record = (SortedList)records.GetByIndex(0);

        if (record["C_exp"].ToString().Length > 0)
        {
          C_exp = ConvertString.ToDouble(record["C_exp"].ToString());
        }

        if (record["b_exp"].ToString().Length > 0)
        {
          b_exp = ConvertString.ToDouble(record["b_exp"].ToString());
        }

        if (record["Lambda_exp"].ToString().Length > 0)
        {
          lambda_exp = ConvertString.ToDouble(record["Lambda_exp"].ToString());
        }

        if (record["c_kwad"].ToString().Length > 0)
        {
          c_kwad = ConvertString.ToDouble(record["c_kwad"].ToString());
        }

        if (record["c_kwad"].ToString().Length > 0)
        {
          c_kwad = ConvertString.ToDouble(record["c_kwad"].ToString());
        }

        if (record["b_kwad"].ToString().Length > 0)
        {
          b_kwad = ConvertString.ToDouble(record["b_kwad"].ToString());
        }

        if (record["a_kwad"].ToString().Length > 0)
        {
          a_kwad = ConvertString.ToDouble(record["a_kwad"].ToString());
        }

        if (record["Omega"].ToString().Length > 0)
        {
          omega = ConvertString.ToDouble(record["Omega"].ToString());
        }
      }
      else
      {
        m_ApplicationError.Raise(2002, new object[] { sqlSelect });
      }
    }

    /// <summary>
    /// Lees de overstromingsschade parameters van het dijkringdeel
    /// </summary>
    /// <param name="dijkring">Dijkring identificatie</param>
    /// <param name="deel">Dijkringdeel identificatie</param>
    /// <param name="aantalInwoners">Het aantal inwoners.</param>
    /// <param name="restrictie_pmin">Restrictie pmin.</param>
    public void DijkringDeelRecord(string dijkring, int deel, out long aantalInwoners, out double restrictie_pmin)
    {

      aantalInwoners = 0;
      restrictie_pmin = 0.0;

      string sqlSelect = "SELECT * FROM DijkringDelen";
      sqlSelect += " WHERE Dijkring = '" + dijkring + " '";
      sqlSelect += " AND DijkringDeel = " + deel.ToString() + ";";

      SortedList records = this.GetDataset(sqlSelect).ToSortedList();
      if (records.Count == 1)
      {
        SortedList record = (SortedList)records.GetByIndex(0);
        if (record["Inwoners"].ToString().Length > 0)
        {
          aantalInwoners = ConvertString.ToInt64(record["Inwoners"].ToString());
        }

        if (record["Restrictie_pmin"].ToString().Length > 0)
        {
          restrictie_pmin = ConvertString.ToDouble(record["Restrictie_pmin"].ToString());
        }
      }
      else
      {
        m_ApplicationError.Raise(2002, new object[] { sqlSelect });
      }

    }

    /// <summary>
    // Lees de schaalparameter schade afhankelijk van waterstand [1/cm]
    /// </summary>
    /// <param name="dijkring">Dijkring identificatie</param>
    /// <param name="deel">Dijkringdeel identificatie</param>
    /// <param name="schadeFunctieId">Schade functie identificatie</param>
    /// <param name="nu">schaalparameter schade afhankelijk van waterstand [1/cm]</param>
    /// <param name="zeta">Stijgingstempo schade per cm verhoging in 1/cm</param>
    /// <param name="psi">Waterstandsverhoging als functie van klimaatverandering [1/cm] </param>
    public void SchadeFunctieData(string dijkring, int deel, int schadeFunctieId, ref double nu
      , ref double zeta, ref double psi)
    {
      string sqlSelect = "SELECT * FROM SchadeFunctieData";
      sqlSelect += " WHERE Dijkring = '" + dijkring + " '";
      sqlSelect += " AND DijkringDeel = " + deel.ToString();
      sqlSelect += " AND SchadeFunctieId = " + schadeFunctieId.ToString() + ";";

      SortedList records = this.GetDataset(sqlSelect).ToSortedList();
      if (records.Count == 1)
      {
        SortedList record = (SortedList)records.GetByIndex(0);
        if (record["Nu"].ToString().Length > 0)
        {
          nu = ConvertString.ToDouble(record["Nu"].ToString());
        }
        if (record["Zeta"].ToString().Length > 0)
        {
          zeta = ConvertString.ToDouble(record["Zeta"].ToString());
        }
        if (record["Psi"].ToString().Length > 0)
        {
          psi = ConvertString.ToDouble(record["Psi"].ToString());
        }
      }
      else
      {
        m_ApplicationError.Raise(2002, new object[] { sqlSelect });
      }
    }

    /// <summary>
    /// Lees Waterstandsverhoging als functie van klimaatverandering [1/jaar]
    /// </summary>
    /// <param name="dijkring">Dijkring identificatie</param>
    /// <param name="deel">Dijkringdeel identificatie</param>
    /// <param name="klimaatScenarioId">The klimaat scenario id.</param>
    public void SchadeKlimaatData(string dijkring, int deel, int klimaatscenarioId, ref double min_overschrijdingskans)
    {
      string sqlSelect = "SELECT * FROM Klimaat_AftoppenAfvoerDataDijkringdeel";
      sqlSelect += " WHERE Dijkring = '" + dijkring + " '";
      sqlSelect += " AND DijkringDeel = " + deel.ToString();
      sqlSelect += " AND Klimaat_AftoppenafvoerId = " + klimaatscenarioId.ToString() + ";";

      SortedList records = this.GetDataset(sqlSelect).ToSortedList();
      if (records.Count == 1)
      {
        SortedList record = (SortedList)records.GetByIndex(0);
        if (record["Min_overschrijdingskans"].ToString().Length > 0)
        {
          min_overschrijdingskans = ConvertString.ToDouble(record["Min_overschrijdingskans"].ToString());
        }
      }
      else
      {
        m_ApplicationError.Raise(2002, new object[] { sqlSelect });
      }
    }


    /// <summary>
    /// Haal Dijkringspecifieke aanpassingsfactor overstromingsschade uit database
    /// </summary>
    /// <param name="dijkring">The dijkring.</param>
    /// <param name="deel">The deel.</param>
    /// <param name="dijkringspecifiekeFactorSchadeId">The dijkringspecifieke factor schade id.</param>
    /// <param name="dijkringspecifiekeFactorSchade">The dijkringspecifieke factor schade.</param>
    public void DijkringspecifiekeFactorSchadeData(string dijkring, int deel
      , int dijkringspecifiekeFactorSchadeId, ref double dijkringspecifiekeFactorSchade)
    {
      string sqlSelect = "SELECT * FROM DijkringspecifiekeFactorSchadeData";
      sqlSelect += " WHERE Dijkring = '" + dijkring + " '";
      sqlSelect += " AND DijkringDeel = " + deel.ToString();
      sqlSelect += " AND DijkringspecifiekeFactorSchadeId = " + dijkringspecifiekeFactorSchadeId.ToString() + ";";

      SortedList records = this.GetDataset(sqlSelect).ToSortedList();
      if (records.Count == 1)
      {
        SortedList record = (SortedList)records.GetByIndex(0);
        if (record["AanpassingsFactor"].ToString().Length > 0)
        {
          dijkringspecifiekeFactorSchade = ConvertString.ToDouble(record["AanpassingsFactor"].ToString());
        }
      }
      else
      {
        m_ApplicationError.Raise(2002, new object[] { sqlSelect });
      }

    }

    /// <summary>
    /// Lees de Schade beginjaar [Meuro]
    /// </summary>
    /// <param name="dijkring">Dijkring identificatie</param>
    /// <param name="deel">Dijkringdeel identificatie</param>
    /// <param name="scenarioVoorHoeveelheidSchade">Scenario voor hoeveelheid schade identificatie</param>
    /// <param name="schadeFunctieId">Schade functie identificatie</param>
    /// <param name="schade">Schade beginjaar [Meuro]</param>
    public void ScenarioVoorHoeveelheidSchadeData(string dijkring, int deel, int scenarioVoorHoeveelheidSchade,
      int schadeFunctieId, ref double schade)
    {
      string sqlSelect = "SELECT * FROM ScenarioVoorHoeveelheidSchadeData";
      sqlSelect += " WHERE Dijkring = '" + dijkring + " '";
      sqlSelect += " AND DijkringDeel = " + deel.ToString();
      sqlSelect += " AND ScenarioVoorHoeveelheidSchadeId = " + scenarioVoorHoeveelheidSchade.ToString();
      sqlSelect += " AND SchadeFunctieId = " + schadeFunctieId.ToString() + ";";

      SortedList records = this.GetDataset(sqlSelect).ToSortedList();
      if (records.Count == 1)
      {
        SortedList record = (SortedList)records.GetByIndex(0);
        if (record["Schade"].ToString().Length > 0)
        {
          schade = ConvertString.ToDouble(record["Schade"].ToString());
        }
      }
      else
      {
        m_ApplicationError.Raise(2002, new object[] { sqlSelect });
      }
    }

    /// <summary>
    /// Lees data van een economische scenario
    /// </summary>
    /// <param name="dijkring">Dijkring identificatie</param>
    /// <param name="deel">Dijkringdeel identificatie</param>
    /// <param name="economischScenarioId">Het economisch scenario</param>
    /// <param name="gamma">De waarde van de economische groei.</param>
    public void EconomischScenarioRecord(string dijkring, int deel, int economischScenarioId, ref double gamma)
    {
      string sqlSelect = "SELECT * FROM EconomischScenarioData";
      sqlSelect += " WHERE Dijkring = '" + dijkring + " '";
      sqlSelect += " AND DijkringDeel = " + deel.ToString();
      sqlSelect += " AND EconomischScenarioId = " + economischScenarioId.ToString() + ";";

      SortedList records = this.GetDataset(sqlSelect).ToSortedList();
      if (records.Count == 1)
      {
        SortedList record = (SortedList)records.GetByIndex(0);
        if (record["Gamma"].ToString().Length > 0)
        {
          gamma = ConvertString.ToDouble(record["Gamma"].ToString());
        }
      }
      else
      {
        m_ApplicationError.Raise(2002, new object[] { sqlSelect });
      }

    }

    /// <summary>
    /// Klimaatscenario en fysisch max afvoer gegevens
    /// </summary>
    /// <param name="dijkring">Dijkring identificatie</param>
    /// <param name="deel">Dijkringdeel identificatie</param>
    /// <param name="dijkringTraject">The dijkring traject.</param>
    /// <param name="klimaatScenarioId">The klimaat scenario id.</param>
    /// <param name="alpha_Overstromingskans">The alpha_ overstromingskans.</param>
    /// <param name="p0_Overstromingskans">The P0_ overstromingskans.</param>
    /// <param name="alpha_Overschrijdingskans">The alpha_ overschrijdingskans.</param>
    /// <param name="p0_Overschrijdingskans">The P0_ overschrijdingskans.</param>
    /// <param name="eta">The eta.</param>
    public void KlimaatScenarioEnFysischMaxAfvoerDataRecord(string dijkring, int deel, int dijkringTraject,
       int klimaatScenarioId, ref double alpha_Overstromingskans, ref double p0_Overstromingskans,
       ref double alpha_Overschrijdingskans, ref double p0_Overschrijdingskans, ref double eta)
    {
      // Dijkring
      // DijkringDeel
      // DijkringTraject
      // Klimaat_AftoppenafvoerId

      // Alpha_overstromingskans
      // P0_overstromingskans
      // Alpha_overschrijdingskans
      // P0_overschrijdingskans
      // Eta

      string sqlSelect = "SELECT * FROM Klimaat_AftoppenAfvoerDataTraject";
      sqlSelect += " WHERE Dijkring = '" + dijkring + " '";
      sqlSelect += " AND DijkringDeel = " + deel.ToString();
      sqlSelect += " AND DijkringTraject = " + dijkringTraject.ToString();
      sqlSelect += " AND Klimaat_AftoppenafvoerId = " + klimaatScenarioId.ToString() + ";";

      SortedList records = this.GetDataset(sqlSelect).ToSortedList();
      if (records.Count == 1)
      {
        SortedList record = (SortedList)records.GetByIndex(0);
        if (record["Alpha_overstromingskans"].ToString().Length > 0)
        {
          alpha_Overstromingskans = ConvertString.ToDouble(record["Alpha_overstromingskans"].ToString());
        }

        if (record["P0_overstromingskans"].ToString().Length > 0)
        {
          p0_Overstromingskans = ConvertString.ToDouble(record["P0_overstromingskans"].ToString());
        }

        if (record["Alpha_overschrijdingskans"].ToString().Length > 0)
        {
          alpha_Overschrijdingskans = ConvertString.ToDouble(record["Alpha_overschrijdingskans"].ToString());
        }

        if (record["P0_overschrijdingskans"].ToString().Length > 0)
        {
          p0_Overschrijdingskans = ConvertString.ToDouble(record["P0_overschrijdingskans"].ToString());
        }
        if (record["Eta"].ToString().Length > 0)
        {
          eta = ConvertString.ToDouble(record["Eta"].ToString());
        }
      }
      else
      {
        m_ApplicationError.Raise(2002, new object[] { sqlSelect });
      }

    }

    /// <summary>
    /// Lees de terugkeertijd.
    /// </summary>
    /// <param name="dijkring">Dijkring identificatie</param>
    /// <param name="terugkeertijd">The terugkeertijd.</param>
    public void Terugkeertijd(string dijkring, out double terugkeertijd)
    {
      terugkeertijd = Double.NaN;

      SortedList records = this.Dijkring(dijkring).ToSortedList();
      if (records.Count == 1)
      {
        SortedList record = (SortedList)records.GetByIndex(0);
        if (record["Terugkeertijd"].ToString().Length > 0)
        {
          terugkeertijd = ConvertString.ToDouble(record["Terugkeertijd"].ToString());
        }
      }
      else
      {
        m_ApplicationError.Raise(2003, new object[] { dijkring });
      }

    }

    /// <summary>
    /// Lees record uit de table van een raming voor het aantal slachtoffers
    /// </summary>
    /// <param name="dijkring">Dijkring identificatie</param>
    /// <param name="deel">Dijkringdeel identificatie</param>
    /// <param name="ramingVoorSlachtoffersId">De raming voor slachtoffers identificatie</param>
    /// <param name="aantalDodelijkeSlachtoffers">Het aantal dodelijke slachtoffers.</param>
    /// <param name="aantalGetroffenen">Het aantal getroffenen.</param>
    public void RamingVoorSlachtoffersRecord(string dijkring, int deel, int ramingVoorSlachtoffersId,
      ref long aantalDodelijkeSlachtoffers, ref long aantalGetroffenen)
    {
      aantalDodelijkeSlachtoffers = 0;
      aantalGetroffenen = 0;

      string sqlSelect = "SELECT * FROM RamingVoorSlachtoffersData";
      sqlSelect += " WHERE Dijkring = '" + dijkring + " '";
      sqlSelect += " AND DijkringDeel = " + deel.ToString();
      sqlSelect += " AND RamingVoorSlachtoffersId = " + ramingVoorSlachtoffersId.ToString() + ";";
      SortedList records = this.GetDataset(sqlSelect).ToSortedList();
      if (records.Count == 1)
      {
        SortedList record = (SortedList)records.GetByIndex(0);
        if (record["Slachtoffers"].ToString().Length > 0)
        {
          aantalDodelijkeSlachtoffers = ConvertString.ToInt64(record["Slachtoffers"].ToString());
        }
        if (record["Getroffenen"].ToString().Length > 0)
        {
          aantalGetroffenen = ConvertString.ToInt64(record["Getroffenen"].ToString());
        }
      }
      else
      {
        m_ApplicationError.Raise(2002, new object[] { sqlSelect });
      }

    }


    /// <summary>
    /// CompartimenteringsDijken
    /// </summary>
    public SortedList CompartimenteringsDijken()
    {
      if (m_CompartimenteringsDijken == null)
      {
        StringBuilder sqlSelect = new StringBuilder();
        sqlSelect.Append("SELECT DISTINCT ");
        sqlSelect.Append("Dijkringen_1.Dijkring, ");
        sqlSelect.Append("Dijkringen_1.Naam ");
        sqlSelect.Append("FROM Dijkringen INNER JOIN Dijkringen AS Dijkringen_1 ON ");
        sqlSelect.Append("Dijkringen.Compartimenteringsdijk = Dijkringen_1.Dijkring;");

        this.m_CompartimenteringsDijken = GetDataset(sqlSelect.ToString()).ToSortedList();
      }

      return this.m_CompartimenteringsDijken;
    }

    /// <summary>
    /// Test of opgegeven dijk een compartiment van een compartimenteringsdijk is
    /// </summary>
    /// <param name="dijknummer">The dijknummer.</param>
    /// <returns>
    /// 	<c>true</c> if [is compartimenterings dijk] [the specified dijknummer]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsCompartimenteringsDijk(string dijknummer)
    {
      foreach (SortedList dijk in this.CompartimenteringsDijken().Values)
      {
        if (dijknummer == dijk["Dijkring"].ToString())
        {
          return true;
        }
      }
      return false;
    }


    internal object DijkringDeelNaam(string p)
    {
      throw new Exception("The method or operation is not implemented.");
    }
  }
}
