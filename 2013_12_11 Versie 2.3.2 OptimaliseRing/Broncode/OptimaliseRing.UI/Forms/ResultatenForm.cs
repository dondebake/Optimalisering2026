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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.UI/Forms/ResultatenForm.cs 2     18/06/08 14:11 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OptimaliseRing.Core;
using Hkv.General;

namespace OptimaliseRing.UI.Forms
{
  /// <summary>
  ///
  /// </summary>
  public partial class ResultatenForm : Form
  {
    /// <summary>
    /// Berekeningenmap
    /// </summary>
    private string m_BerekeningenMap;

    /// <summary>
    /// Kolomnaam
    /// </summary>
    private string m_ColumnName;

    /// <summary>
    /// Optimale overstromingskansjaar
    /// </summary>
    private int m_OptimaleOverstromingskansenJaar;

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:ResultatenForm"/> class.
    /// </summary>
    public ResultatenForm()
    {
      InitializeComponent();

      dgvResultaten.Location = new Point(5, 5);
      dgvResultaten.Size = new Size(this.gridPanel.Width - 10, this.gridPanel.Height - 10);

      Initialize();

      // nudOptimaleOverstromingskansenJaar
      int ZichtJaar = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Instellingen", "ZichtJaar", "2015"));
      int OptimaleOverstromingskansenJaar = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Instellingen", "OptimaleOverstromingskansenJaar", ZichtJaar.ToString()));

      this.nudOptimaleOverstromingskansenJaar.Value = OptimaleOverstromingskansenJaar;
      this.RefreshKeuzeMenu();
    }

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets or Berekeningenmap.
    /// </summary>
    /// <value>The berekeningen map.</value>
    public String BerekeningenMap
    {
      get { return this.m_BerekeningenMap; }
    }

    /// <summary>
    /// Gets Optimale overstromingskansjaar.
    /// </summary>
    /// <value>Optimale overstromingskansjaar.</value>
    public int OptimaleOverstromingskansenJaar
    {
      get { return this.m_OptimaleOverstromingskansenJaar; }
    }

    #endregion Properties ----------------------------------------------------

    /// <summary>
    /// Initialiseer this instance.
    /// </summary>
    private void Initialize()
    {

      // optionbuttons tekst
      string jaarKeuzeOptie1 = string.Format(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "JaarKeuzeOptie1").ToString().Replace("\\t", "\t").Replace("\\n", "\n"));
      this.JaarKeuzeOptie1Label.Text = jaarKeuzeOptie1;

      string jaarKeuzeOptie2 = string.Format(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "JaarKeuzeOptie2").ToString().Replace("\\t", "\t").Replace("\\n", "\n"));
      this.JaarKeuzeOptie2Label.Text = jaarKeuzeOptie2;

      string jaarKeuzeOptieUitvoerInfoLabel = string.Format(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "JaarKeuzeOptieUitvoerInfoLabel").ToString().Replace("\\t", "\t").Replace("\\n", "\n"));
      this.JaarKeuzeOptieUitvoerInfoLabel.Text = jaarKeuzeOptieUitvoerInfoLabel;

      string multiSelectie = string.Format(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "MultiSelectie").ToString().Replace("\\t", "\t").Replace("\\n", "\n"));
      this.chkMultiSelect.Text = multiSelectie;

      // resultatengrid
      this.dgvResultaten.Columns.Clear();

      DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
      m_ColumnName = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Resultaten", "Resultaten").ToString();
      column.Name = m_ColumnName;
      column.HeaderText = column.Name;
      column.SortMode = DataGridViewColumnSortMode.NotSortable;
      column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      column.ReadOnly = true;
      this.dgvResultaten.Columns.Add(column);

      DataGridViewTextBoxColumn columnDateTime = new DataGridViewTextBoxColumn();
      string columnDateTimeName = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "DateTime", "Date time").ToString();
      columnDateTime.Name = columnDateTimeName;
      columnDateTime.HeaderText = columnDateTime.Name;
      columnDateTime.SortMode = DataGridViewColumnSortMode.NotSortable;
      columnDateTime.ReadOnly = true;
      columnDateTime.DefaultCellStyle.Format = "dd-MM-yyyy HH:mm";
      columnDateTime.ValueType = typeof(DateTime);
      this.dgvResultaten.Columns.Add(columnDateTime);

      string[] directories = Directory.GetDirectories(ThisAppWorkingDirectory.Instance.Werkmap);

      this.dgvResultaten.Rows.Clear();
      int newRow = 0;
      for (int row = 0; row < directories.Length; row++)
      {
        if (string.Compare(Path.GetFileName(directories[row]), ".svn", true) != 0)
        {

          System.IO.DirectoryInfo directoryInfo = new DirectoryInfo(directories[row]);
          DateTime startTime = DateTime.MinValue;

          bool bestandenOk = false;
          if (directoryInfo.GetFiles("Instellingen.xml").Length > 0)
          {
            System.IO.FileInfo fileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, "Instellingen.xml"));
            startTime = fileInfo.CreationTime;

            if (directoryInfo.GetFiles("OptimaliseRing.xml").Length > 0)
            {
              bestandenOk = true;
            }
          }

          this.dgvResultaten.Rows.Add(new DataGridViewRow());

          if (!bestandenOk)
          {
            this.dgvResultaten.Rows[newRow].DefaultCellStyle.ForeColor = Color.Red;
          }

          this.dgvResultaten[m_ColumnName, newRow].Value = Path.GetFileName(directories[row]);
          this.dgvResultaten[m_ColumnName, newRow].Tag = directories[row];
          if (startTime != DateTime.MinValue)
          {
            this.dgvResultaten[columnDateTimeName, newRow++].Value = startTime;// string.Format(ThisAppCulture.Instance, "{0:dd-MM-yyyy HH:mm}", startTime);
          }
          else
          {
            this.dgvResultaten[columnDateTimeName, newRow++].Value = string.Empty;
          }


        }
      }
      this.dgvResultaten.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
    }

    /// <summary>
    /// Handles the Click event of the btnCancel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnBtnCancelClick(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
    }

    /// <summary>
    /// Handles the Click event of the btnOK control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void BtnOKClick(object sender, EventArgs e)
    {

      this.m_OptimaleOverstromingskansenJaar = this.JaarKeuzeOptie1.Checked
        ?  Convert.ToInt32(this.nudOptimaleOverstromingskansenJaar.Value) : -1;

      if (this.dgvResultaten.SelectedRows.Count > 0)
      {
        int row = this.dgvResultaten.SelectedRows[0].Index;
        if (row >= 0)
        {
          this.m_BerekeningenMap = this.dgvResultaten[m_ColumnName, row].Tag.ToString();

          this.DialogResult = DialogResult.OK;
        }
      }
    }

    /// <summary>
    /// Called when [BTN delete click].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void OnBtnDeleteClick(object sender, EventArgs e)
    {

      try
      {

        if (this.dgvResultaten.SelectedRows.Count > 0)
        {
          string resultsDialogSure = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "ResultsDialogSure", "ResultsDialogSure").ToString();
          string desultsDialogTitle = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "ResultsDialogTitle", "ResultsDialogTitle").ToString();
          DialogResult dialogResult = MessageBox.Show(resultsDialogSure, desultsDialogTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

          if (dialogResult == DialogResult.Yes)
          {
            foreach (DataGridViewRow dataGridViewRow in this.dgvResultaten.SelectedRows)
            {

              int row = dataGridViewRow.Index;

              string resultaten = this.dgvResultaten[m_ColumnName, row].Tag.ToString();
              if (Directory.Exists(resultaten))
              {
                Directory.Delete(resultaten, true);
                this.dgvResultaten.Rows.RemoveAt(row);
              }
            }
            this.RefreshKeuzeMenu();
          }
        }
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Called when [DGV resultaten cell double click].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
    private void OnDgvResultatenCellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      try
      {
        if (e.RowIndex == -1)
        {
          // header
        }
        else
        {
          BtnOKClick(this.dgvResultaten, e);
        }
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Handles the CellClick event of the dgvResultaten control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
    private void dgvResultaten_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex == -1)
      {
        // header
        DataGridViewColumn dataGridViewColumn = this.dgvResultaten.Columns[e.ColumnIndex];
        ListSortDirection listSortDirection = ListSortDirection.Ascending;

        if (this.dgvResultaten.SortedColumn == dataGridViewColumn)
        {
          listSortDirection = this.dgvResultaten.SortOrder == SortOrder.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
        }

        this.dgvResultaten.Sort(dataGridViewColumn, listSortDirection);
      }
      else
      {
        this.RefreshKeuzeMenu();
      }
    }

    /// <summary>
    /// Refreshes the keuze menu.
    /// </summary>
    private void RefreshKeuzeMenu()
    {
      try
      {
        this.btnOK.Enabled = false;
        this.JaarKeuzePanel.Enabled = false;
        this.btnDelete.Enabled = this.dgvResultaten.RowCount > 0;

        if (this.chkMultiSelect.Checked == false)
        {
          if (this.dgvResultaten.SelectedRows.Count == 1)
          {
            string berekeningenMap = this.dgvResultaten.SelectedRows[0].Cells[m_ColumnName].Tag.ToString();
            if (System.IO.Directory.Exists(berekeningenMap))
            {
              string instellingenBestand = Path.Combine(berekeningenMap, "Instellingen.xml");
              if (File.Exists(instellingenBestand))
              {
                Instellingen instellingen = new Instellingen();
                instellingen.Read(instellingenBestand);

                string aimmsInstellingenBestand = Path.Combine(berekeningenMap, "Aimms_instellingen.ini");
                if (File.Exists(aimmsInstellingenBestand))
                {
                  OptimaliseRing.Profile.Ini aimmsInstellingen = new OptimaliseRing.Profile.Ini(aimmsInstellingenBestand);
                  int z = Convert.ToInt32(aimmsInstellingen.GetValue("Parameters", "Z"));

                  this.nudOptimaleOverstromingskansenJaar.Minimum = instellingen.ZichtJaar;
                  this.nudOptimaleOverstromingskansenJaar.Maximum = instellingen.ZichtJaar + z;
                  this.nudOptimaleOverstromingskansenJaar.Value = instellingen.OptimaleOverstromingskansenJaar;
                  this.JaarKeuzePanel.Enabled = true;
                  this.btnOK.Enabled = true;
                }
              }
            }
          }
        }
      }
      catch (Exception appex)
      {
        this.btnOK.Enabled = false;
        this.JaarKeuzePanel.Enabled = false;
      }
    }

    /// <summary>
    /// Handles the CheckedChanged event of the JaarKeuzeOptie1 control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void JaarKeuzeOptie1_CheckedChanged(object sender, EventArgs e)
    {
      try
      {
        this.nudOptimaleOverstromingskansenJaar.Visible = JaarKeuzeOptie1.Checked == true;
        this.JaarKeuzeOptieUitvoerInfoLabel.Visible = JaarKeuzeOptie1.Checked == true;
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Handles the Click event of the JaarKeuzeOptieLabel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void JaarKeuzeOptieLabel_Click(object sender, EventArgs e)
    {
      try
      {
        Label label = (Label)sender;
        if (label.Name == this.JaarKeuzeOptie1Label.Name)
        {
          this.JaarKeuzeOptie1.Checked = true;
        }
        else
        {
          this.JaarKeuzeOptie2.Checked = true;
        }
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Handles the CheckedChanged event of the chkMultiSelect control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void chkMultiSelect_CheckedChanged(object sender, EventArgs e)
    {
      try
      {
        this.btnOK.Enabled = this.chkMultiSelect.Checked == false;
        this.dgvResultaten.MultiSelect = this.chkMultiSelect.Checked;
        this.RefreshKeuzeMenu();
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }

    }

    /// <summary>
    /// Handles the KeyDown event of the ResultatenForm control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
    private void ResultatenForm_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Alt)
      {
        if (e.KeyCode == Keys.I)
        {
          if (!this.JaarKeuzeOptie1.Checked)
          {
            this.JaarKeuzeOptie1.Checked = true;
          }
        }
        else if (e.KeyCode == Keys.W)
        {
          if (!this.JaarKeuzeOptie2.Checked)
          {
            this.JaarKeuzeOptie2.Checked = true;
          }
        }
        else if (e.KeyCode == Keys.J)
        {
          this.nudOptimaleOverstromingskansenJaar.Focus();
        }
      }
    }
  }
}
