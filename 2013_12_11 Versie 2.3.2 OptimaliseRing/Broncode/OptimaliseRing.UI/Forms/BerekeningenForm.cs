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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.UI/Forms/BerekeningenForm.cs 8     28-04-09 14:30 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using OptimaliseRing.General;
using OptimaliseRing.Core;

namespace OptimaliseRing.UI.Forms
{
  public partial class BerekeningenForm : Form
  {
    private SortedList m_DijkringDelen;               // Lijst met dijkringdelen uit de database
    private SortedList m_Selecties;                   // Lijst met de voorgedefinieerde selecties van dijkringdelen
    private String m_BerekeningenMap;                 // Naam van de map met de berekeningen
    private bool m_Initializing;
    private bool m_MayClose = true;                   // Controle of form mag sluiten
    private Berekening m_Berekening;
    private DataGridViewHelper m_DataGridViewHelper;

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:BerekeningenForm"/> class.
    /// </summary>
    public BerekeningenForm(Berekening berekening)
    {
      m_Berekening = berekening;
      m_Initializing = true;

      InitializeComponent();

      this.m_DijkringDelen = m_Berekening.OptimaliseRingDB.GetDataset("SELECT * FROM DijkringDelen ORDER BY INDEX").ToSortedList();

      m_DataGridViewHelper = new DataGridViewHelper(
            ThisAppProfile.Instance
            , ThisAppLanguage.Instance
            , ThisAppCulture.Instance
            , this.dgvBerekeningen
            , m_Berekening
            , m_DijkringDelen);

      m_DataGridViewHelper.InitializeDijkringenGrid();
      m_DataGridViewHelper.InitialiseerCompartimentering();
      m_DataGridViewHelper.VulDijkringenGrid();

      InitializeBerekeningenSelecties();

      this.dgvBerekeningen.ClearSelection();

      m_Initializing = false;

      UpdateOkButton();
    }

    /// <summary>
    /// Updates the ok button.
    /// </summary>
    private void UpdateOkButton()
    {
      if (!m_Initializing)
      {
        this.btnOK.Enabled = false;

        if (txtNaam.Text.Length > 0)
        {
          for (int row = 0; row < this.dgvBerekeningen.RowCount; row++)
          {
            if (ConvertString.ToBoolean(this.dgvBerekeningen["Berekenen", row].Value.ToString()))
            {
              this.btnOK.Enabled = true;
            }
          }
        }
      }
    }

    /// <summary>
    /// Initializes the selecties.
    /// </summary>
    private void InitializeBerekeningenSelecties()
    {
      bool doorgaan = true;

      this.selecterenToolStripMenuItem.DropDownItems.Clear();

      this.m_Selecties = new SortedList();
      int i = 1;
      while (doorgaan)
      {
        string entry = "BerekeningenSelectie" + i.ToString();
        string item = ThisAppProfile.Instance.GetValue("BerekeningenSelecties:" + ThisAppCulture.Instance.Name, entry, "NO_MORE");
        if (item.Length > 0)
        {
          if (string.Compare(item.ToUpper(), "NO_MORE") == 0)
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
                selectieList.Add(new Selectie(m_Berekening.OptimaliseRingDB
                   , dijkringDeel["Dijkring"].ToString()
                   , Convert.ToInt32(dijkringDeel["DijkringDeel"].ToString())));
              }
            }
            this.m_Selecties.Add(item, selectieList);
            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(item);

            toolStripMenuItem.Click += new EventHandler(ToolStripMenuItemClick);
            this.selecterenToolStripMenuItem.DropDownItems.Add(toolStripMenuItem);
          }
          else if (string.Compare(item.ToUpper(), "NIETS") == 0 || string.Compare(item.ToUpper(), "NOTHING") == 0)
          {
            ToolStripMenuItem toolStripMenuItemNiets = new ToolStripMenuItem(item);

            toolStripMenuItemNiets.Click += new EventHandler(OnToolStripMenuItemNiets);
            this.selecterenToolStripMenuItem.DropDownItems.Add(toolStripMenuItemNiets);
          }
          else if (string.Compare(item.ToUpper(), "-") == 0)
          {
            ToolStripSeparator toolStripSeparator = new ToolStripSeparator();

            this.selecterenToolStripMenuItem.DropDownItems.Add(toolStripSeparator);
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

              this.m_Selecties.Add(items[0], selectieList);

              ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(items[0]);

              toolStripMenuItem.Click += new EventHandler(ToolStripMenuItemClick);
              this.selecterenToolStripMenuItem.DropDownItems.Add(toolStripMenuItem);
            }
          }
          i++;
        }
      }

      this.selecterenToolStripMenuItem.Enabled = this.selecterenToolStripMenuItem.DropDownItems.Count > 0;
    }

    /// <summary>
    /// Get percentages for keringen.
    /// </summary>
    //private void InitializeKeringenToedeling()
    //{

    //}
    /// <summary>
    /// Handles the Click event of the toolStripMenuItem control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void ToolStripMenuItemClick(object sender, EventArgs e)
    {
      MaakBerekeningen(sender.ToString());
      UpdateOkButton();
    }

    /// <summary>
    /// Called when [tool strip menu item niets].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void OnToolStripMenuItemNiets(object sender, EventArgs e)
    {
      ClearSelectie();
      UpdateOkButton();
    }

    private void ClearSelectie()
    {
      for (int row = 0; row < this.dgvBerekeningen.RowCount; row++)
      {
        this.dgvBerekeningen["Berekenen", row].Value = false;
      }
      this.txtNaam.Text = string.Empty;
    }

    /// <summary>
    /// Maaks the berekeningen.
    /// </summary>
    /// <param name="naam">The naam.</param>
    private void MaakBerekeningen(string naam)
    {
      this.txtNaam.Text = naam;

      try
      {
        // Check op aantal geselecteerden
        int aantal = 0;

        this.dgvBerekeningen.ClearSelection();

        List<Selectie> selectieList = (List<Selectie>)m_Selecties[naam];

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
      }
      catch (Exception appex)
      {
        //ThisAppErr.Instance.Raise(1004, new object[] { appex.Message });
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }



    /// <summary>
    /// Handles the Click event of the btnOK control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnBtnOKClick(object sender, EventArgs e)
    {
      try
      {
        bool start = true;
        this.m_MayClose = true;

        StringBuilder foutedijkringen = m_Berekening.Controleer(this.dgvBerekeningen, m_DijkringDelen, m_DataGridViewHelper.Compartimentering);

        // Eerst controleren of het zichtjaar groter of gelijk dan het maxSchadeKansjaar is
        int maxSchadeKansjaar = Math.Max(m_Berekening.Schadejaar, m_Berekening.Kansjaar);

        if (m_Berekening.Instellingen.ZichtJaar >= maxSchadeKansjaar)
        {
          if (foutedijkringen.Length > 0)
          {
            if (ThisAppErr.Instance.DisplayQuestion(this, 1, new string[] { foutedijkringen.ToString() }) != DialogResult.Yes)
            {
              start = false;
            }
          }
        }
        else
        {
          ThisAppErr.Instance.DisplayMessage(this, 2, new object[] { maxSchadeKansjaar, m_Berekening.Instellingen.ZichtJaar });
          this.DialogResult = DialogResult.Cancel;
          start = false;
          this.m_MayClose = false;
        }

        if (start)
        {
          this.m_BerekeningenMap = Path.Combine(ThisAppWorkingDirectory.Instance.Werkmap, this.txtNaam.Text);
          if (BerekeningOverschrijven())
          {
            if (m_Berekening.TeBerekenenDijkringdelen.Count > 0)
            {
              Directory.CreateDirectory(this.m_BerekeningenMap);
              this.DialogResult = DialogResult.OK;
            }
          }
          else
          {
            this.DialogResult = DialogResult.None;
          }
        }
        else
        {
          this.DialogResult = DialogResult.Cancel;
        }
      }
      catch (Exception appex)
      {
        //ThisAppErr.Instance.Raise(1004, new object[] { appex.Message });
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Handles the Click event of the btnCancel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnBtnCancelClick(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.m_MayClose = true;
    }

    /// <summary>
    /// Gets the naam.
    /// </summary>
    /// <value>The naam.</value>
    public String Naam
    {
      get
      {
        return this.txtNaam.Text;
      }
    }
    /// <summary>
    /// Gets the berekeningen map.
    /// </summary>
    /// <value>The berekeningen map.</value>
    public String BerekeningenMap
    {
      get
      {
        return this.m_BerekeningenMap;
      }
    }

    /// <summary>
    /// Handles the TextChanged event of the txtNaam control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnTxtNaamTextChanged(object sender, EventArgs e)
    {
      UpdateOkButton();
    }

    /// <summary>
    /// Handles the Click event of the btnInstellingen control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnBtnInstellingenClick(object sender, EventArgs e)
    {
      try
      {
        OptimaliseRing.UI.Forms.InstellingenForm instellingenForm = new OptimaliseRing.UI.Forms.InstellingenForm(m_Berekening);
        if (instellingenForm.ShowDialog(this) == DialogResult.OK)
        {
          instellingenForm.SaveInstellingen();
        }
        instellingenForm.Dispose();
      }
      catch (Exception appex)
      {
        //ThisAppErr.Instance.Raise(1004, new object[] { appex.Message });
        Hkv.General.ApplicationError.Display(this, appex);
      }

    }

    /// <summary>
    /// Overschrijven van een bestaande berekening
    /// </summary>
    /// <returns></returns>
    private bool BerekeningOverschrijven()
    {
      bool result = false;

      if (Directory.Exists(this.m_BerekeningenMap))
      {

        // bestaande berekening overschrijven?
        DialogResult dialogResult = ThisAppErr.Instance.DisplayQuestion(this, 2,
          new string[] { Path.GetFileNameWithoutExtension(m_BerekeningenMap) });

        switch (dialogResult)
        {
          case DialogResult.Yes:
            Directory.Delete(m_BerekeningenMap, true);
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

    /// <summary>
    /// Handles the CellValueChanged event of the dgvBerekeningen control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
    private void OnDgvBerekeningenCellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      UpdateOkButton();
    }

    /// <summary>
    /// Called when [berekeningen form form closing].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
    private void OnBerekeningenFormFormClosing(object sender, FormClosingEventArgs e)
    {
      e.Cancel = !this.m_MayClose;
    }

    /// <summary>
    /// Called when [DGV berekeningen current cell dirty state changed].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void OnDgvBerekeningenCurrentCellDirtyStateChanged(object sender, EventArgs e)
    {

      this.dgvBerekeningen.CommitEdit(DataGridViewDataErrorContexts.Commit);

      UpdateOkButton();
    }

    /// <summary>
    /// Handles the CellDoubleClick event of the dgvBerekeningen control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
    private void dgvBerekeningen_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      try
      {
        if (e.ColumnIndex == 2 && e.RowIndex > -1)
        {
          string tekst = (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string;
          if (!string.IsNullOrEmpty(tekst))
          {
            this.txtNaam.Text = tekst;
          }
        }
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }

    }

  }
}
