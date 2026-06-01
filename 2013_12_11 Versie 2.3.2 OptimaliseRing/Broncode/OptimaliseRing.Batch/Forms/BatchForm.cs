using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;

using OptimaliseRing.Core;
using OptimaliseRing.General;

namespace OptimaliseRing.Batch
{
   public partial class BatchForm : Form, IStatus
   {
      private SortedList                                       m_SelectieDijkringDelen;   // Lijst met de voorgedefinieerde selecties van dijkringdelen
      private SortedList                                       m_DijkringDelen;           // Lijst met dijkringdelen uit de database
      private bool                                             m_Initializing;
      private Berekening                                       m_Berekening;
      private string                                           m_SomType;
      private OptimaliseRingDB                                 m_OptimaliseRingDB;
      private DataGridViewHelper                               m_DataGridViewHelper;

      public BatchForm()
      {
         InitializeComponent();
      }


      #region Menu -------------------------------------------------------------

      #region Bestand Menu

      /// <summary>
      /// Handles the Click event of the menuBestandWerkmap control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
      private void OnMenuBestandWerkmapClick(object sender, EventArgs e)
      {
         string werkMap = OptimaliseRingDialog.Folder(
            Application.ProductName + " - " + ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "BatchFolderBrowserDialogTitle", "")
            , ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "BatchFolderBrowserDialogDescription", "")
            , ThisAppWorkingDirectory.Instance.BatchMap
            , false
            );

         if (!string.IsNullOrEmpty(werkMap))
         {
            ThisAppProfile.Instance.SetValue("OptimaliseRing", "BatchMap", MyPath.RelativeName(werkMap));
            ThisAppWorkingDirectory.Instance.BatchMap = werkMap;
         }
      }

      /// <summary>
      /// Handles the Click event of the exitToolStripMenuItem control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
      private void OnMenuBestandExitClick(object sender, EventArgs e)
      {
         BestandExit();
      }

      #endregion

      #region Selectie Menu

      private void OnMenuSelectieClick(object sender, EventArgs e)
      {
         SetSelection(sender.ToString());
         UpdateStartButton();
      }

      /// <summary>
      /// Called when [tool strip menu item niets].
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
      private void OnMenuNietsClick(object sender, EventArgs e)
      {
         ClearSelectie();
         UpdateStartButton();
      }


      #endregion

      #region Rekenen Menu

      /// <summary>
      /// Called when [menu rekenen start click].
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
      private void OnMenuRekenenStartClick(object sender, EventArgs e)
      {
         try
         {
            if (BerekeningOverschrijven( Path.Combine(ThisAppWorkingDirectory.Instance.BatchMap,this.txtSerieFolder.Text)))
            {
               RekenenStart(this.txtSerieFolder.Text, m_SomType, this.dgvInstellingen, this.dgvBerekeningen);
            }
         }
         catch (Exception appex)
         {
            ThisAppErr.Instance.Display(this, appex);
         }
      }

      private void OnMenuRekenencompartimenteringClick(object sender, EventArgs e)
      {
         try
         {
            CompartimenteringForm compartimenteringForm = new CompartimenteringForm(
               ThisAppProfile.Instance
               , ThisAppLanguage.Instance
               , m_Berekening
               , ThisAppErr.Instance);
            compartimenteringForm.Text = Application.ProductName + " - " + ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Compartimentering", "Compartimentering");

            if (compartimenteringForm.ShowDialog() == DialogResult.OK)
            {
               Refresh();

               m_DataGridViewHelper.VulDijkringenGrid();
            }
         }
         catch (Exception appex)
         {
            ThisAppErr.Instance.Display(this, appex);
         }
      }

      #endregion

      #region Help Menu

      /// <summary>
      /// Called when [menu help over click].
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
      private void OnMenuHelpOverClick(object sender, EventArgs e)
      {
         OverForm overForm = new OverForm(ThisAppProfile.Instance);
         overForm.Icon = this.Icon;
         overForm.Text = Application.ProductName + " - " + ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Over", "Over");
         overForm.ShowDialog(this);
         overForm.Dispose();
      }

      #endregion

      #endregion Menu ----------------------------------------------------------

      #region Event handling ---------------------------------------------------

     private void OnBatchFormLoad(object sender, EventArgs e)
     {
       try
       {
         this.Top = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Top", "0"));
         this.Left = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Left", "0"));
         this.Width = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Width", "640"));
         this.Height = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Height", "480"));

         this.Text = Application.ProductName;

         m_Initializing = true;

         m_SomType = string.Empty;

         Initialize();

         this.dgvBerekeningen.ClearSelection();

         this.menuRekenenStart.Enabled = false;

         UpdateStartButton();

         m_Initializing = false;
       }
       catch (Exception appex)
       {
         ThisAppErr.Instance.Display(this, appex);
         this.Close();
       }
     }

      /// <summary>
      /// Handles the FormClosing event of the MainForm control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
      private void OnBatchFormClosing(object sender, FormClosingEventArgs e)
      {
         BestandExit();
      }

      /// <summary>
      /// Called when [DGV dijkringen cell value changed].
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
      private void OnDgvDijkringenCellValueChanged(object sender, DataGridViewCellEventArgs e)
      {
         UpdateStartButton();
      }

      /// <summary>
      /// Called when [DGV instellingen cell value changed].
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
      private void OnDgvInstellingenCellValueChanged(object sender, DataGridViewCellEventArgs e)
      {
         UpdateStartButton();
      }


      /// <summary>
      /// Called when [DGV instellingen current cell dirty state changed].
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
      private void OnDgvInstellingenCurrentCellDirtyStateChanged(object sender, EventArgs e)
      {
         this.dgvInstellingen.CommitEdit(DataGridViewDataErrorContexts.Commit);

         UpdateStartButton();
      }

      /// <summary>
      /// Called when [DGV berekeningen current cell dirty state changed].
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
      private void OnDgvBerekeningenCurrentCellDirtyStateChanged(object sender, EventArgs e)
      {
         this.dgvBerekeningen.CommitEdit(DataGridViewDataErrorContexts.Commit);

         UpdateStartButton();
      }


      /// <summary>
      /// Called when [TXT serie folder text changed].
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
      private void OnTxtSerieFolderTextChanged(object sender, EventArgs e)
      {
         UpdateStartButton();
      }

      /// <summary>
      /// Called when [TXT som type text changed].
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
      private void OnTxtSomTypeTextChanged(object sender, EventArgs e)
      {
         m_SomType = this.txtSomType.Text;
         UpdateStartButton();
      }

      #endregion Event handling ------------------------------------------------

      /// <summary>
      /// Afsluiten programma
      /// </summary>
      private void BestandExit()
      {
         try
         {
            ThisAppProfile.Instance.SetValue("OptimaliseRing", "BatchMap", MyPath.RelativeName(ThisAppWorkingDirectory.Instance.BatchMap));

            if (this.WindowState == FormWindowState.Normal)
            {
               ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Top", this.Top.ToString());
               ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Left", this.Left.ToString());
               ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Width", this.Width.ToString());
               ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Height", this.Height.ToString());
            }
            this.Dispose();
         }
         catch (Exception appex)
         {
            ThisAppErr.Instance.Display(this, appex);
         }
      }


      /// <summary>
      /// Initialiseer het instellingen grid.
      /// </summary>
      private void InitializeInstellingenGrid()
      {
         this.dgvInstellingen.Columns.Clear();

         DataGridViewCheckBoxColumn kolomBerekenen = new DataGridViewCheckBoxColumn();
         kolomBerekenen.Name = "Berekenen";
         kolomBerekenen.HeaderText = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Berekenen", "");
         kolomBerekenen.SortMode = DataGridViewColumnSortMode.NotSortable;
         kolomBerekenen.ReadOnly = false;
         kolomBerekenen.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
         this.dgvInstellingen.Columns.Add(kolomBerekenen);

         DataGridViewTextBoxColumn kolomNaam = new DataGridViewTextBoxColumn();
         kolomNaam.Name = "kolomNaam";
         kolomNaam.HeaderText = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Naam", "");
         kolomNaam.SortMode = DataGridViewColumnSortMode.Automatic;
         kolomNaam.ReadOnly = true;
         kolomNaam.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
         this.dgvInstellingen.Columns.Add(kolomNaam);

         string instellingenMap = MyPath.AbsoluteName(ThisAppProfile.Instance.GetValue("OptimaliseRing", "InstellingenMap", "./Instellingen"));

         string[] instellingen = Directory.GetFiles(instellingenMap, "*.xml");
         if (instellingen.Length > 0)
         {
            for (int i = 0; i < instellingen.Length; i++)
            {
               // Test of bestand een instellingen bestand is
               if (InstellingenBestand(instellingen[i]))
               {
                  this.dgvInstellingen.Rows.Add(new DataGridViewRow());
                  this.dgvInstellingen["Berekenen", this.dgvInstellingen.RowCount - 1].Value = false;
                  this.dgvInstellingen["kolomNaam", this.dgvInstellingen.RowCount - 1].Value = Path.GetFileName(instellingen[i]);
                  this.dgvInstellingen["kolomNaam", this.dgvInstellingen.RowCount - 1].Tag = instellingen[i];
               }
            }
         }
         else
         {
            ThisAppErr.Instance.Raise(6, new object[] { instellingenMap });
         }
      }

      /// <summary>
      /// Test of dit xml bestand een instellingen xml bestand is
      /// </summary>
      /// <param name="bestand">Naam van het bestand.</param>
      /// <returns>true als dit een instellingen bestand is</returns>
      private bool InstellingenBestand(string bestand)
      {
         bool instellingenBestand = false;

         XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
         xmlReaderSettings.ConformanceLevel = ConformanceLevel.Document;
         xmlReaderSettings.IgnoreWhitespace = true;
         xmlReaderSettings.IgnoreComments = true;

         using (XmlReader xmlReader = XmlReader.Create(bestand, xmlReaderSettings))
         {
            // Lees <Instellingen>
            while (xmlReader.Read())
            {
               if ((xmlReader.Name == "Instellingen") && xmlReader.IsStartElement())
               {
                  instellingenBestand = true;
                  break;
               }
            }
         }
         return instellingenBestand;
      }


      /// <summary>
      /// Initialiseer
      /// </summary>
      private void Initialize()
      {
         this.txtSerieFolder.Text = string.Empty;
         this.txtSomType.Text = string.Empty;

         string databaseName = MyPath.AbsoluteName(ThisAppProfile.Instance.GetValue("OptimaliseRing", "Database", ""));
         if (File.Exists(databaseName))
         {
            m_OptimaliseRingDB = new OptimaliseRingDB(ThisAppErr.Instance);
            m_OptimaliseRingDB.Open(databaseName, "Provider=Microsoft.Jet.OLEDB.4.0;User Id=admin;Password=;Mode=12");

            m_Berekening = new Berekening(
               ThisAppCulture.Instance
               , ThisAppProfile.Instance
               , ThisAppLanguage.Instance
               , m_OptimaliseRingDB
               , ThisAppErr.Instance);

            m_DijkringDelen = m_Berekening.OptimaliseRingDB.GetDataset("SELECT * FROM DijkringDelen ORDER BY INDEX").ToSortedList();

            InitializeInstellingenGrid();

            m_DataGridViewHelper = new DataGridViewHelper(
               ThisAppProfile.Instance
               , ThisAppLanguage.Instance
               , ThisAppCulture.Instance
               , this.dgvBerekeningen
               , m_Berekening
               , m_DijkringDelen );

            m_DataGridViewHelper.InitializeDijkringenGrid();
            m_DataGridViewHelper.InitialiseerCompartimentering();
            m_DataGridViewHelper.VulDijkringenGrid();

            InitializeBerekeningenSelecties();
         }
         else
         {
            ThisAppErr.Instance.Raise(1000, new object[] { databaseName });
         }
      }

      /// <summary>
      /// Initialiseer berekening selectie
      /// </summary>
      private void InitializeBerekeningenSelecties()
      {
         bool doorgaan = true;

         this.dijkringdelenToolStripMenuItem.DropDownItems.Clear();

         this.m_SelectieDijkringDelen = new SortedList();
         int i = 1;
         while (doorgaan)
         {
            string entry = "BerekeningenSelectie" + i.ToString();
            string item = ThisAppProfile.Instance.GetValue("BerekeningenSelecties:" + ThisAppCulture.Instance.Name, entry, "NO_MORE");
            if (item.Length > 0)
            {
               if (string.Compare(item, "NO_MORE") == 0)
               {
                  doorgaan = false;
               }
               else if (string.Compare(item.ToUpper(), "ALLES") == 0 || string.Compare(item.ToUpper(), "ALL") == 0)
               {
                  List<Selectie> selectieList = new List<Selectie>();
                  for (int j = 0; j < this.m_DijkringDelen.Count; j++)
                  {
                     SortedList dijkringDeel = (SortedList)this.m_DijkringDelen.GetByIndex(j);

                     if (m_Berekening.OptimaliseRingDB.SelectieAanwezig(dijkringDeel["Dijkring"].ToString(),
                       Convert.ToInt32(dijkringDeel["DijkringDeel"].ToString())))
                     {
                        selectieList.Add(new Selectie(m_Berekening.OptimaliseRingDB, dijkringDeel["Dijkring"].ToString(), Convert.ToInt32(dijkringDeel["DijkringDeel"].ToString())));
                     }
                  }
                  this.m_SelectieDijkringDelen.Add(item, selectieList);
                  ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(item);

                  toolStripMenuItem.Click += new EventHandler(OnMenuSelectieClick);
                  this.dijkringdelenToolStripMenuItem.DropDownItems.Add(toolStripMenuItem);
               }
               else if (string.Compare(item.ToUpper(), "NIETS") == 0 || string.Compare(item.ToUpper(), "NOTHING") == 0)
               {
                  ToolStripMenuItem toolStripMenuItemNiets = new ToolStripMenuItem(item);

                  toolStripMenuItemNiets.Click += new EventHandler(OnMenuNietsClick);
                  this.dijkringdelenToolStripMenuItem.DropDownItems.Add(toolStripMenuItemNiets);
               }
               else if (string.Compare(item.ToUpper(), "-") == 0)
               {
                  ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
                  this.dijkringdelenToolStripMenuItem.DropDownItems.Add(toolStripSeparator);
               }
               else
               {
                  string[] items = item.Split("|".ToCharArray());
                  if (items.Length > 0)
                  {
                     List<Selectie> selectieList = new List<Selectie>();

                     for (int j = 1; j < items.Length; j++)
                     {
                        string[] dijkringDeelItems = items[j].Split(":".ToCharArray());
                        if (dijkringDeelItems.Length > 0)
                        {
                           if (m_Berekening.OptimaliseRingDB.SelectieAanwezig(dijkringDeelItems[0],
                             Convert.ToInt32(dijkringDeelItems[1])))
                           {
                              selectieList.Add(new Selectie(m_Berekening.OptimaliseRingDB
                                 , dijkringDeelItems[0]
                                 , Convert.ToInt32(dijkringDeelItems[1])));
                           }
                        }
                     }

                     this.m_SelectieDijkringDelen.Add(items[0], selectieList);

                     ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(items[0]);

                     toolStripMenuItem.Click += new EventHandler(OnMenuSelectieClick);
                     this.dijkringdelenToolStripMenuItem.DropDownItems.Add(toolStripMenuItem);
                  }
               }
               i++;
            }
         }

         this.dijkringdelenToolStripMenuItem.Enabled = this.dijkringdelenToolStripMenuItem.DropDownItems.Count > 0;
      }

      private void ClearSelectie()
      {
         for (int row = 0; row < this.dgvBerekeningen.RowCount; row++)
         {
            this.dgvBerekeningen["Berekenen", row].Value = false;
         }
         this.txtSomType.Text = string.Empty;
      }

      /// <summary>
      /// Updates the start berekeneingen button.
      /// </summary>
      private void UpdateStartButton()
      {
         if (!m_Initializing)
         {
            this.menuRekenenStart.Enabled = false;

            if (txtSerieFolder.Text.Length > 0)
            {
               for (int rowInstellingen = 0; rowInstellingen < this.dgvInstellingen.RowCount; rowInstellingen++)
               {
                  if (ConvertString.ToBoolean(this.dgvInstellingen["Berekenen", rowInstellingen].Value.ToString()))
                  {
                     for (int rowBerekeningen = 0; rowBerekeningen < this.dgvBerekeningen.RowCount; rowBerekeningen++)
                     {
                        if (ConvertString.ToBoolean(this.dgvBerekeningen["Berekenen", rowBerekeningen].Value.ToString()))
                        {
                           this.menuRekenenStart.Enabled = true;
                           break;
                        }
                     }
                  }
               }
            }
         }
      }

      /// <summary>
      /// Zet de gekozen selectie
      /// </summary>
      /// <param name="naam">The naam.</param>
      private void SetSelection(string naam)
      {
         try
         {
            // Check op aantal geselecteerden
            int aantal = 0;

            this.dgvBerekeningen.ClearSelection();

            List<Selectie> selectieList = (List<Selectie>)m_SelectieDijkringDelen[naam];

            for (int row = 0; row < this.dgvBerekeningen.RowCount; row++)
            {
               this.dgvBerekeningen["Berekenen", row].Value = false;
            }

            foreach (Selectie selectie in selectieList)
            {
               for (int row = 0; row < this.dgvBerekeningen.RowCount; row++)
               {
                  if ((string.Compare(this.dgvBerekeningen["Dijkring", row].Value.ToString(), selectie.DijkringId) == 0) &&
                      (string.Compare(this.dgvBerekeningen["DijkringNaam", row].Value.ToString(), selectie.DijkringNaam) == 0) &&
                      (string.Compare(this.dgvBerekeningen["DijkringDeelNaam", row].Value.ToString(), selectie.DijkringDeelNaam) == 0))
                  {
                     if (this.dgvBerekeningen.Rows[row].Visible)
                     {
                        this.dgvBerekeningen["Berekenen", row].Value = true;

                        aantal++;
                        this.dgvBerekeningen.CurrentCell = this.dgvBerekeningen["Berekenen", row];
                     }
                     break;
                  }
               }
            }

            if (aantal == 0)
            {
               ThisAppErr.Instance.DisplayMessage(this, 1, new object[] { naam });
            }
            else
            {
               m_SomType = naam;
               this.txtSomType.Text = m_SomType;
            }
         }
         catch (Exception appex)
         {
            ThisAppErr.Instance.Raise(1004, new object[] { appex.Message });
         }
      }


      /// <summary>
      /// Start de berekeningen
      /// </summary>
      /// <param name="serieFolder">The serie folder.</param>
      /// <param name="somType">Type of the som.</param>
      /// <param name="dgvInstellingen">The DGV instellingen.</param>
      /// <param name="dgvBerekeningen">The DGV berekeningen.</param>
     private void RekenenStart(
        string serieFolder
        , string somType
        , DataGridView dgvInstellingen
        , DataGridView dgvBerekeningen)
     {
       using (new WaitCursor())
       {
         int aantalSeries = 0;
         for (int rowInstellingen = 0; rowInstellingen < dgvInstellingen.RowCount; rowInstellingen++)
         {
           if (ConvertString.ToBoolean(dgvInstellingen["Berekenen", rowInstellingen].Value.ToString()))
           {
             aantalSeries++;
           }
         }

         int aantalBerekeningen = 0;
         for (int rowBerekeningen = 0; rowBerekeningen < dgvBerekeningen.RowCount; rowBerekeningen++)
         {
           if (ConvertString.ToBoolean(dgvBerekeningen["Berekenen", rowBerekeningen].Value.ToString()))
           {
             aantalBerekeningen++;
           }
         }

         int berekeningNummer = 0;

         SetStatusLabel(string.Format(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "StartBerekening").ToString()));

         // lus over de instellingen
         for (int rowInstellingen = 0; rowInstellingen < dgvInstellingen.RowCount; rowInstellingen++)
         {
           if (ConvertString.ToBoolean(dgvInstellingen["Berekenen", rowInstellingen].Value.ToString()))
           {
             //Naam instellingen bestand
             string instellingenNaam = dgvInstellingen["kolomNaam", rowInstellingen].Tag.ToString();
             if (File.Exists(instellingenNaam))
             {
               //Lees instellingen
               m_Berekening.Instellingen = new Instellingen(ThisAppProfile.Instance, ThisAppLanguage.Instance, m_Berekening.OptimaliseRingDB);
               m_Berekening.Instellingen.Read(instellingenNaam);

               bool start = true;

               StringBuilder foutedijkringen = m_Berekening.Controleer(dgvBerekeningen, m_DijkringDelen, m_DataGridViewHelper.Compartimentering);

               // Controleren of het zichtjaar groter of gelijk dan het maxSchadeKansjaar is
               int maxSchadeKansjaar = Math.Max(m_Berekening.Schadejaar, m_Berekening.Kansjaar);

               if (m_Berekening.Instellingen.ZichtJaar >= maxSchadeKansjaar)
               {
                 if (foutedijkringen.Length > 0)
                 {
                   if (ThisAppErr.Instance.DisplayQuestion(this, 1, new string[] { foutedijkringen.ToString() }) != DialogResult.Yes)
                   {
                     start = false;
                   }
                   else
                   {
                     if (m_Berekening.TeBerekenenDijkringdelen.Count == 0)
                     {
                       start = false;
                     }
                   }
                 }
               }
               else
               {
                 ThisAppErr.Instance.DisplayMessage(this, 2, new object[] { maxSchadeKansjaar, m_Berekening.Instellingen.ZichtJaar });
                 start = false;
               }

               if (start)
               {
                 if (somType.Length > 0)
                 {
                   m_Berekening.BerekeningenMap = Path.Combine(ThisAppWorkingDirectory.Instance.BatchMap, Path.Combine(serieFolder, string.Format("{0} - {1}", somType, Path.GetFileNameWithoutExtension(instellingenNaam))));
                 }
                 else
                 {
                   m_Berekening.BerekeningenMap = Path.Combine(ThisAppWorkingDirectory.Instance.BatchMap, Path.Combine(serieFolder, string.Format("{0}", Path.GetFileNameWithoutExtension(instellingenNaam))));
                 }

                 // Maak de serie werkmap
                 Directory.CreateDirectory(m_Berekening.BerekeningenMap);

                 m_Berekening.Start(this, ref berekeningNummer, aantalSeries * aantalBerekeningen);

                 m_Berekening.StrategieBestand("Strategie.xls");
                 m_Berekening.KansenBestand("Kansen.xls");

               }
             }
             else
             {
               ThisAppErr.Instance.Raise(5, new object[] { instellingenNaam });
             }
           }
         }
         SetStatusLabel("");
       }
     }

      /// <summary>
      /// Sets the status label.
      /// </summary>
      /// <param name="tekst">The tekst.</param>
      public void SetStatusLabel(string tekst)
      {
         this.StatusLabel.Text = tekst;
         this.statusStripMain.Refresh();
      }

      /// <summary>
      /// Overschrijven van een bestaande berekening
      /// </summary>
      /// <returns></returns>
      private bool BerekeningOverschrijven(string batchMap)
      {
         bool result = false;

         if (Directory.Exists(batchMap))
         {
            // bestaande berekening overschrijven?
            DialogResult dialogResult = ThisAppErr.Instance.DisplayQuestion(this, 4,
              new string[] { Path.GetFileNameWithoutExtension(batchMap) });

            switch (dialogResult)
            {
               case DialogResult.Yes:
                  Directory.Delete(batchMap, true);
                  result = true;
                  break;

               case DialogResult.No:
                  result = false;
                  break;
            }
         }
         else
         {
            result = true;
         }

         this.Refresh();

         return result;
      }

      #region IStatus Members

      public string Status
      {
         set { SetStatusLabel(value); }
      }

      #endregion

      private void OnAllToolStripMenuItemClick(object sender, EventArgs e)
      {
         for (int row = 0; row < this.dgvInstellingen.RowCount; row++)
         {
            this.dgvInstellingen["Berekenen", row].Value = true;
         }
         UpdateStartButton();
      }

      private void OnNothingToolStripMenuItemClick(object sender, EventArgs e)
      {
         for (int row = 0; row < this.dgvInstellingen.RowCount; row++)
         {
            this.dgvInstellingen["Berekenen", row].Value = false;
         }
         UpdateStartButton();
      }



   }
}
