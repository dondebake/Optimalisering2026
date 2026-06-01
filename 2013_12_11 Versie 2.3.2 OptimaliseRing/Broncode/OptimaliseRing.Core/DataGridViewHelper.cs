using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;


using OptimaliseRing.General;
using OptimaliseRing.Profile;

namespace OptimaliseRing.Core
{
  public class DataGridViewHelper
  {
    private struct Waarde
    {
      /// <summary>
      /// Minvalue
      /// </summary>
      public int Minvalue;
      /// <summary>
      /// Maxvalue
      /// </summary>
      public int Maxvalue;

      /// <summary>
      /// Initializes a new instance of the <see cref="Waarde"/> struct.
      /// </summary>
      /// <param name="minvalue">The minvalue.</param>
      /// <param name="maxvalue">The maxvalue.</param>
      public Waarde(int minvalue, int maxvalue)
      {
        this.Minvalue = minvalue;
        this.Maxvalue = maxvalue;
      }
    }
    //private OptimaliseRingDB m_OptimaliseRingDB;    // OptimaliseRing database
    private CultureInfo m_CultureInfo;
    private Profile.Ini m_Language;
    private Profile.Ini m_Profile;
    private DataGridView m_DataGridView;
    private SortedList<string, List<Compartimenteringsdijk>> m_Compartimentering;
    private SortedList m_DijkringDelen;
    private Berekening m_Berekening;
    private SortedList m_Keringen;

    public DataGridViewHelper(
       Profile.Ini profile
       , Profile.Ini language
       , CultureInfo cultureInfo
       , DataGridView dataGridView
       , Berekening berekening
       , SortedList dijkringDelen)
    {
      m_Profile = profile;
      m_Language = language;
      m_CultureInfo = cultureInfo;
      m_DataGridView = dataGridView;
      m_DijkringDelen = dijkringDelen;
      m_Berekening = berekening;
      m_Compartimentering = new SortedList<string, List<Compartimenteringsdijk>>();
    }

    public DataGridViewHelper(
       Profile.Ini profile
       , Profile.Ini language
       , CultureInfo cultureInfo
       , DataGridView dataGridView
       , Berekening berekening
       , SortedList dijkringDelen
       , SortedList keringen)
    {
      m_Profile = profile;
      m_Language = language;
      m_CultureInfo = cultureInfo;
      m_DataGridView = dataGridView;
      m_DijkringDelen = dijkringDelen;
      m_Berekening = berekening;
      m_Keringen = keringen;
      //m_Compartimentering = new SortedList<string, List<Compartimenteringsdijk>>();
    }

      public SortedList<string, List<Compartimenteringsdijk>> Compartimentering
      {
        get { return m_Compartimentering; }
      }

      /// <summary>
      /// Initialiseer het dijkringen grid.
      /// </summary>
      public void InitializeDijkringenGrid()
      {
        m_DataGridView.Columns.Clear();

        DataGridViewCheckBoxColumn kolomBerekenen = new DataGridViewCheckBoxColumn();
        kolomBerekenen.Name = "Berekenen";
        kolomBerekenen.HeaderText = m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Berekenen", "");
        kolomBerekenen.SortMode = DataGridViewColumnSortMode.NotSortable;
        kolomBerekenen.ReadOnly = false;
        kolomBerekenen.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        m_DataGridView.Columns.Add(kolomBerekenen);

        DataGridViewTextBoxColumn kolomDijkring = new DataGridViewTextBoxColumn();
        kolomDijkring.Name = "Dijkring";
        kolomDijkring.HeaderText = m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Dijkring", "");
        kolomDijkring.SortMode = DataGridViewColumnSortMode.NotSortable;
        kolomDijkring.ReadOnly = true;
        kolomDijkring.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        m_DataGridView.Columns.Add(kolomDijkring);

        DataGridViewTextBoxColumn kolomDijkringNaam = new DataGridViewTextBoxColumn();
        kolomDijkringNaam.Name = "DijkringNaam";
        kolomDijkringNaam.HeaderText = m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Naam", "");
        kolomDijkringNaam.SortMode = DataGridViewColumnSortMode.NotSortable;
        kolomDijkringNaam.ReadOnly = true;
        kolomDijkringNaam.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        m_DataGridView.Columns.Add(kolomDijkringNaam);

        DataGridViewTextBoxColumn kolomDijkringDeelNaam = new DataGridViewTextBoxColumn();
        kolomDijkringDeelNaam.Name = "DijkringDeelNaam";
        kolomDijkringDeelNaam.HeaderText = m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Deel", "");
        kolomDijkringDeelNaam.SortMode = DataGridViewColumnSortMode.NotSortable;
        kolomDijkringDeelNaam.ReadOnly = true;
        kolomDijkringDeelNaam.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        m_DataGridView.Columns.Add(kolomDijkringDeelNaam);

        DataGridViewTextBoxColumn kolomAantalTrajecten = new DataGridViewTextBoxColumn();
        kolomAantalTrajecten.Name = "AantalTrajecten";
        kolomAantalTrajecten.HeaderText = m_Language.GetValue("Captions:" + m_CultureInfo.Name, "AantalTrajecten", "");
        kolomAantalTrajecten.SortMode = DataGridViewColumnSortMode.NotSortable;
        kolomAantalTrajecten.ReadOnly = true;
        kolomAantalTrajecten.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        this.m_DataGridView.Columns.Add(kolomAantalTrajecten);

        DataGridViewTextBoxColumn kolomCompartimenteringsDijkring = new DataGridViewTextBoxColumn();
        kolomCompartimenteringsDijkring.Name = "CompartimenteringsDijkring";
        kolomCompartimenteringsDijkring.HeaderText = "Compartimenteringsdijkring";
        kolomCompartimenteringsDijkring.SortMode = DataGridViewColumnSortMode.NotSortable;
        kolomCompartimenteringsDijkring.ReadOnly = true;
        kolomCompartimenteringsDijkring.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        m_DataGridView.Columns.Add(kolomCompartimenteringsDijkring);

        m_DataGridView.Columns["CompartimenteringsDijkring"].Visible = false;
      }

      /// <summary>
      /// Initialiseer de compartimenteringsdijken
      /// </summary>
      public void InitialiseerCompartimentering()
      {
        m_Compartimentering.Clear();

        for (int i = 0; i < m_DijkringDelen.Count; i++)
        {
          SortedList dijkringDeel = (SortedList)m_DijkringDelen.GetByIndex(i);

          SortedList dijkring = m_Berekening.OptimaliseRingDB.Dijkring(dijkringDeel["Dijkring"].ToString()).ToSortedList();

          string compartimenteringsdijk = "";
          if (dijkring.Count == 1)
          {
            // Controleer of het een compartiment is van een dijkring
            dijkring = (SortedList)dijkring[0];
            bool compartimenteren = false;
            compartimenteringsdijk = dijkring["Compartimenteringsdijk"].ToString();

            if (compartimenteringsdijk.Length > 0)
            {
              compartimenteren = m_Profile.GetValue("Compartimenteren", compartimenteringsdijk, false);

              if (compartimenteren)
              {
                if (!m_Compartimentering.ContainsKey(compartimenteringsdijk))
                {
                  List<Compartimenteringsdijk> compartimenteringdelen = new List<Compartimenteringsdijk>();

                  Compartimenteringsdijk compartimenteringsdijkdeel = new Compartimenteringsdijk();
                  compartimenteringsdijkdeel.DijkId = dijkring["Dijkring"].ToString();
                  compartimenteringsdijkdeel.Dijkdeel = dijkring["Compartimenteringsdeel"].ToString();
                  compartimenteringdelen.Add(compartimenteringsdijkdeel);

                  m_Compartimentering.Add(compartimenteringsdijk, compartimenteringdelen);
                }
                else
                {
                  List<Compartimenteringsdijk> compartimenteringdelen = m_Compartimentering[compartimenteringsdijk];

                  Compartimenteringsdijk compartimenteringsdijkdeel = new Compartimenteringsdijk();
                  compartimenteringsdijkdeel.DijkId = dijkring["Dijkring"].ToString();
                  compartimenteringsdijkdeel.Dijkdeel = dijkring["Compartimenteringsdeel"].ToString();
                  compartimenteringdelen.Add(compartimenteringsdijkdeel);
                }
              }
            }
          }
        }
      }


      /// <summary>
      /// Vul het dijkringen grid.
      /// </summary>
      public void VulDijkringenGrid()
      {
        using (new WaitCursor())
        {
          m_DataGridView.Rows.Clear();

          for (int i = 0; i < m_DijkringDelen.Count; i++)
          {
            SortedList dijkringDeel = (SortedList)m_DijkringDelen.GetByIndex(i);

            SortedList dijkring = m_Berekening.OptimaliseRingDB.Dijkring(dijkringDeel["Dijkring"].ToString()).ToSortedList();

            if (dijkring.Count == 1)
            {
              dijkring = (SortedList)dijkring[0];

              bool compartimenteren = false;
              string compartimenteringsdijk = dijkring["Compartimenteringsdijk"].ToString();
              if (compartimenteringsdijk.Length > 0)
              {
                compartimenteren = m_Profile.GetValue("Compartimenteren", compartimenteringsdijk, false);
              }
              else
              {
                // Test of er compartimenteringsdijkdelen aanwezig zijn
                if (m_Berekening.OptimaliseRingDB.IsCompartimenteringsDijk(dijkringDeel["Dijkring"].ToString()))
                {
                  compartimenteren = !m_Profile.GetValue("Compartimenteren", dijkringDeel["Dijkring"].ToString(), false);
                }
                else
                {
                  compartimenteren = true;
                }
              }

              // Voeg dijkring toe aan grid
              AddDijkring(
                  dijkringDeel["Dijkring"].ToString()
                 , m_Berekening.OptimaliseRingDB.DijkringNaam(dijkringDeel["Dijkring"].ToString())
                 , dijkringDeel["Naam"].ToString()
                 , dijkringDeel["DijkringDeel"].ToString()
                 , compartimenteringsdijk
                 , compartimenteren
                 );
            }
          }
        }
      }

      /// <summary>
      /// Voeg dijkring toe aan de berekening
      /// </summary>
      /// <param name="dijkring">The dijkring.</param>
      /// <param name="dijkringnaam">The dijkringnaam.</param>
      /// <param name="naam">The naam.</param>
      /// <param name="dijkringdeel">The dijkringdeel.</param>
      /// <param name="compdijk">The compdijk.</param>
      private void AddDijkring(string dijkring, string dijkringnaam, string naam
         , string dijkringdeel, string compdijk, bool visible)
      {
        m_DataGridView.Rows.Add(new DataGridViewRow());

        m_DataGridView["Berekenen", m_DataGridView.RowCount - 1].Value = false;
        m_DataGridView["Dijkring", m_DataGridView.RowCount - 1].Value = dijkring;
        m_DataGridView["DijkringNaam", m_DataGridView.RowCount - 1].Value = dijkringnaam;
        m_DataGridView["DijkringDeelNaam", m_DataGridView.RowCount - 1].Value = "";
        if (naam != m_DataGridView["DijkringNaam", m_DataGridView.RowCount - 1].Value.ToString())
        {
          m_DataGridView["DijkringDeelNaam", m_DataGridView.RowCount - 1].Value = naam;
        }
        SortedList dijkringTrajecten = m_Berekening.OptimaliseRingDB.DijkringTrajecten(dijkring, Convert.ToInt32(dijkringdeel)).ToSortedList();

        m_DataGridView["AantalTrajecten", m_DataGridView.RowCount - 1].Value = dijkringTrajecten.Count.ToString();

        int telkeringen1 = 0;
        int telkeringen2 = 0;
        //voeg de keringen toe in de trajecten optelling
        SortedList keringInDeel = m_Berekening.OptimaliseRingDB.KeringInDijkringdeel("Dijkring1", "DijkringDeel1", dijkring, Convert.ToInt32(dijkringdeel));
        if (keringInDeel.Count != 0) //er is een kering bijgekomen
        {
          //m_DataGridView["AantalTrajecten", m_DataGridView.RowCount - 1].Value = dijkringTrajecten.Count + keringInDeel.Count;
          telkeringen1 = keringInDeel.Count;
        }
        keringInDeel = m_Berekening.OptimaliseRingDB.KeringInDijkringdeel("Dijkring2", "DijkringDeel2", dijkring, Convert.ToInt32(dijkringdeel));
        if (keringInDeel.Count != 0)
        {
          //m_DataGridView["AantalTrajecten", m_DataGridView.RowCount - 1].Value = dijkringTrajecten.Count + keringInDeel.Count;
          telkeringen2 = keringInDeel.Count;
        }
        m_DataGridView["AantalTrajecten", m_DataGridView.RowCount - 1].Value = dijkringTrajecten.Count + telkeringen1 + telkeringen2;

        m_DataGridView["CompartimenteringsDijkring", m_DataGridView.RowCount - 1].Value = compdijk;

        m_DataGridView.Rows[m_DataGridView.RowCount - 1].Visible = visible;
      }

      /// <summary>
      ///  Initialiseer het Keringen grid.
      /// </summary>
      public void InitializeKeringenGrid()
      {
        m_DataGridView.Columns.Clear();

        DataGridViewTextBoxColumn kolomKeringNummer = new DataGridViewTextBoxColumn();
        kolomKeringNummer.Name = "KeringNummer";
        kolomKeringNummer.HeaderText = m_Language.GetValue("Captions:" + m_CultureInfo.Name, "KeringNummer", "");
        kolomKeringNummer.SortMode = DataGridViewColumnSortMode.NotSortable;
        kolomKeringNummer.ReadOnly = true;
        kolomKeringNummer.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        m_DataGridView.Columns.Add(kolomKeringNummer);

        DataGridViewTextBoxColumn kolomKering = new DataGridViewTextBoxColumn();
        kolomKering.Name = "Bkering";
        kolomKering.HeaderText = m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Bkering", "");
        kolomKering.SortMode = DataGridViewColumnSortMode.NotSortable;
        kolomKering.ReadOnly = true;
        kolomKering.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        m_DataGridView.Columns.Add(kolomKering);

        DataGridViewTextBoxColumn kolomDijkring1 = new DataGridViewTextBoxColumn();
        kolomDijkring1.Name = "Dijkring1";
        kolomDijkring1.HeaderText = m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Dijkring1", "");
        kolomDijkring1.SortMode = DataGridViewColumnSortMode.NotSortable;
        kolomDijkring1.ReadOnly = true;
        kolomDijkring1.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        m_DataGridView.Columns.Add(kolomDijkring1);

        DataGridViewTextBoxColumn kolomDijkringDeel1 = new DataGridViewTextBoxColumn();
        kolomDijkringDeel1.Name = "Deel1";
        kolomDijkringDeel1.HeaderText = m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Deel1", "");
        kolomDijkringDeel1.SortMode = DataGridViewColumnSortMode.NotSortable;
        kolomDijkringDeel1.ReadOnly = true;
        kolomDijkringDeel1.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        m_DataGridView.Columns.Add(kolomDijkringDeel1);

        DataGridViewTextBoxColumn kolomPercentage1 = new DataGridViewTextBoxColumn();
        kolomPercentage1.Name = "Percentage1";
        kolomPercentage1.HeaderText = m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Percentage", "");
        kolomPercentage1.SortMode = DataGridViewColumnSortMode.NotSortable;
        kolomPercentage1.ReadOnly = false;
        kolomPercentage1.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        kolomPercentage1.ValueType = typeof(int);
        kolomPercentage1.Tag = new Waarde(0, 100);
        //kolomPercentage1.ToolTipText = this.GetToolTipTextForCell(waarden, "Percentage1");

        m_DataGridView.Columns.Add(kolomPercentage1);

        DataGridViewTextBoxColumn kolomDijkring2 = new DataGridViewTextBoxColumn();
        kolomDijkring2.Name = "Dijkring2";
        kolomDijkring2.HeaderText = m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Dijkring2", "");
        kolomDijkring2.SortMode = DataGridViewColumnSortMode.NotSortable;
        kolomDijkring2.ReadOnly = true;
        kolomDijkring2.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        m_DataGridView.Columns.Add(kolomDijkring2);

        DataGridViewTextBoxColumn kolomDijkringDeel2 = new DataGridViewTextBoxColumn();
        kolomDijkringDeel2.Name = "Deel2";
        kolomDijkringDeel2.HeaderText = m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Deel2", "");
        kolomDijkringDeel2.SortMode = DataGridViewColumnSortMode.NotSortable;
        kolomDijkringDeel2.ReadOnly = true;
        kolomDijkringDeel2.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        m_DataGridView.Columns.Add(kolomDijkringDeel2);

        DataGridViewTextBoxColumn kolomPercentage2 = new DataGridViewTextBoxColumn();
        kolomPercentage2.Name = "Percentage2";
        kolomPercentage2.HeaderText = m_Language.GetValue("Captions:" + m_CultureInfo.Name, "Percentage", "");
        kolomPercentage2.SortMode = DataGridViewColumnSortMode.NotSortable;
        kolomPercentage2.ReadOnly = false;
        kolomPercentage2.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        kolomPercentage2.ValueType = typeof(int);
        kolomPercentage2.Tag = new Waarde(0, 100);
        m_DataGridView.Columns.Add(kolomPercentage2);
      }

      /// <summary>
      /// Vul het keringen grid.
      /// </summary>
    public void VulKeringenGrid()
    {
      using (new WaitCursor())
      {
        m_DataGridView.Rows.Clear();

        SortedList<int, InstellingenKeringen> percentages = (SortedList<int, InstellingenKeringen>)m_Berekening.Instellingen.Keringenparameters;

        for (int i = 0; i < m_Keringen.Count; i++)
        {
          SortedList keringen = (SortedList)m_Keringen.GetByIndex(i);
          int percentage1 = percentages.Values[i].Percentage1;
          int percentage2 = percentages.Values[i].Percentage2;

          int cnt = m_Berekening.OptimaliseRingDB.DijkringAantal(keringen["Dijkring1"].ToString());

          //nu de naampjes van de dijkringen/delen ophalen
          SortedList dijkring = m_Berekening.OptimaliseRingDB.Dijkring(keringen["Dijkring1"].ToString()).ToSortedList();
          dijkring = (SortedList)dijkring[0];
          string dijkring1 = dijkring["Naam"].ToString();

          dijkring = m_Berekening.OptimaliseRingDB.Dijkring(keringen["Dijkring2"].ToString()).ToSortedList();
          dijkring = (SortedList)dijkring[0];
          string dijkring2 = dijkring["Naam"].ToString();

          //alleen dijkrindeel namen tonen als count =0
          //string deel1 = string.Empty;
          //if (cnt > 1)

          int index = (int)keringen["DijkringDeel1"];
          string deel1 = m_Berekening.OptimaliseRingDB.DijkringDeelNaam(keringen["Dijkring1"].ToString(), index);

          if (dijkring1 == deel1)
          {
            //int index = (int)keringen["DijkringDeel1"];
            deel1 = string.Empty;
          }

          //string deel2 = string.Empty;
          index = (int)keringen["DijkringDeel2"];
          string deel2 = m_Berekening.OptimaliseRingDB.DijkringDeelNaam(keringen["Dijkring2"].ToString(), index);

          if (dijkring2 == deel2)
          {
            //int index = (int)keringen["DijkringDeel2"];
            deel2 = string.Empty;
          }

          string nummer = keringen["B-kering"].ToString();
          string kering  = keringen["Naam"].ToString();

          // Voeg dijkring toe aan grid
          AddDijkringToKeringengrid(
               nummer
             , kering
             , dijkring1
             , deel1
             , percentage1
             , dijkring2
             , deel2
             , percentage2
             );
        }
      }
    }

      /// <summary>
      /// Voeg dijkring toe aan keringengrid
      /// </summary>
      /// <param name="dijkring">The dijkring.</param>
      /// <param name="dijkringnaam">The dijkringnaam.</param>
      /// <param name="naam">The naam.</param>
      /// <param name="dijkringdeel">The dijkringdeel.</param>
      /// <param name="compdijk">The compdijk.</param>
      private void AddDijkringToKeringengrid(string nummer, string kering, string dijkring1, string deel1, int percentage1, string dijkring2
         , string deel2, int percentage2)
      {
        m_DataGridView.Rows.Add(new DataGridViewRow());

        ////KeringNummer
        ////Bkering
        ////Dijkring1
        ////Deel1
        ////Percentage1
        ////Dijkring2
        ////Deel2
        ////Percentage2

        m_DataGridView["KeringNummer", m_DataGridView.RowCount - 1].Value = nummer;
        m_DataGridView["Bkering", m_DataGridView.RowCount - 1].Value = kering;
        m_DataGridView["Dijkring1", m_DataGridView.RowCount - 1].Value = dijkring1;
        m_DataGridView["Deel1", m_DataGridView.RowCount - 1].Value = deel1;

        m_DataGridView["Percentage1", m_DataGridView.RowCount - 1].Value = percentage1;

        m_DataGridView["Dijkring2", m_DataGridView.RowCount - 1].Value = dijkring2;
        m_DataGridView["Deel2", m_DataGridView.RowCount - 1].Value = deel2;
        m_DataGridView["Percentage2", m_DataGridView.RowCount - 1].Value = percentage2;

        m_DataGridView.Rows[m_DataGridView.RowCount - 1].Visible = true;
      }
    }
  }
