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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Diagnostics;

using OptimaliseRing.Core;

namespace OptimaliseRing.UI.Forms
{
   /// <summary>
   /// UitvoerForm
   /// </summary>
  public partial class UitvoerForm : Form
  {

    #region Instance Variables -----------------------------------------------

    private MainForm m_MainForm;                     // Pointer naar het hoofdformulier
    private Berekening m_Berekening;                   // Pointer naar de berekening

    #endregion Instance Variables --------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Maak een nieuwe instantiatie van de <see cref="T:UitvoerForm"/> class.
    /// </summary>
    /// <param name="mainForm">The main form.</param>
    /// <param name="berekening">Pointer naar het hoofdformulier.</param>
    /// <param name="calculated">if set to <c>true</c> [calculated].</param>
    /// <param name="optimaleOverstromingskansenJaar">The optimale overstromingskansen jaar.</param>
    /// <param name="createTabel">if set to <c>true</c> [create tabel].</param>
    public UitvoerForm(MainForm mainForm, Berekening berekening, bool calculated
      , int optimaleOverstromingskansenJaar, bool createTabel)
    {
      InitializeComponent();

      CultureInfo cultureInfo = ThisAppCulture.Instance;

      m_MainForm = mainForm;
      m_Berekening = berekening;

      Text = "OptimaliseRing - " + Path.GetFileName(m_Berekening.BerekeningenMap);

      InitializeGrid();

      if (!calculated)
      {
        m_Berekening.Read(m_Berekening.BerekeningenMap, optimaleOverstromingskansenJaar, createTabel);
      }

      dgvUitvoer.Rows.Clear();
      for (int i = 0; i < m_Berekening.KansenList.Count; i++)
      {
        dgvUitvoer.Rows.Add(new DataGridViewRow());
        Kansen kansen = m_Berekening.KansenList[i];
        dgvUitvoer["Dijkring", i].Value = kansen.DijkringId;
        dgvUitvoer["Naam", i].Value = kansen.DijkringNaam;
        dgvUitvoer["Dijkringdeel", i].Value = kansen.Deel;
        dgvUitvoer["Grafiek", i].Value = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "GrafiekButton", "GrafiekButton");
        dgvUitvoer["Grafiek", i].Tag = kansen;
        dgvUitvoer["Uitvoerbestand", i].Value = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "UitvoerbestandButton", "UitvoerbestandButton");
      }
    }

    #endregion Constructors --------------------------------------------------

    #region Member functions -------------------------------------------------

    /// <summary>
    /// Initialiseer het grid
    /// </summary>
    private void InitializeGrid()
    {
      dgvUitvoer.Columns.Clear();

      DataGridViewTextBoxColumn column1 = new DataGridViewTextBoxColumn();
      column1.Name = "Dijkring";
      column1.HeaderText = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Dijkring", "Dijkring");
      column1.SortMode = DataGridViewColumnSortMode.NotSortable;
      column1.ReadOnly = true;
      column1.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      column1.Width = 60;
      dgvUitvoer.Columns.Add(column1);

      DataGridViewTextBoxColumn column2 = new DataGridViewTextBoxColumn();
      column2.Name = "Naam";
      column2.HeaderText = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Naam", "Naam");
      column2.SortMode = DataGridViewColumnSortMode.NotSortable;
      column2.ReadOnly = true;
      column2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      dgvUitvoer.Columns.Add(column2);

      DataGridViewTextBoxColumn column3 = new DataGridViewTextBoxColumn();
      column3.Name = "Dijkringdeel";
      column3.HeaderText = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Deel", "Deel");
      column3.SortMode = DataGridViewColumnSortMode.NotSortable;
      column3.ReadOnly = true;
      column3.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
      dgvUitvoer.Columns.Add(column3);

      DataGridViewButtonColumn column4 = new DataGridViewButtonColumn();
      column4.Name = "Grafiek";
      column4.HeaderText = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Grafiek", "Grafiek");
      column4.SortMode = DataGridViewColumnSortMode.NotSortable;
      column4.ReadOnly = true;
      column4.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
      dgvUitvoer.Columns.Add(column4);

      DataGridViewButtonColumn column5 = new DataGridViewButtonColumn();
      column5.Name = "Uitvoerbestand";
      column5.HeaderText = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Uitvoerbestand", "Uitvoerbestand");
      column5.SortMode = DataGridViewColumnSortMode.NotSortable;
      column5.ReadOnly = true;
      column5.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
      dgvUitvoer.Columns.Add(column5);

    }

    /// <summary>
    /// Handles the Click event of the btnCancel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnBtnCancelClick(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
    }

    /// <summary>
    /// Handles the Click event of the btnOK control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnBtnOKClick(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
    }

    /// <summary>
    /// Handles the CellContentClick event of the dgvUitvoer control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
    private void OnDgvUitvoerCellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      int row = e.RowIndex;
      if (row >= 0)
      {
        string columnUitvoerbestand = "Uitvoerbestand";
        string columnGrafiek = "Grafiek";

        Kansen kansen = (Kansen)dgvUitvoer[columnGrafiek, row].Tag;

        if (dgvUitvoer.Columns[e.ColumnIndex].Name == columnUitvoerbestand)
        {
          string notepad = ThisAppProfile.Instance.GetValue("OptimaliseRing", "Notepad", "Notepad.exe");

          ProcessStartInfo startInfo = new ProcessStartInfo(notepad);
          startInfo.WindowStyle = ProcessWindowStyle.Normal;

          //string bestandsNaam = kansen.DijkringNaam + "-" + kansen.DeelNummer.ToString() + ".txt";
          string bestandsNaam = kansen.DijkringId + "-" + kansen.DeelNummer + " " + kansen.DijkringNaam.ToString() + ".txt";
          bestandsNaam = bestandsNaam.Replace("/", "-");

          startInfo.Arguments = Path.Combine(m_Berekening.BerekeningenMap, bestandsNaam);

          Process.Start(startInfo);
        }
        if (dgvUitvoer.Columns[e.ColumnIndex].Name == columnGrafiek)
        {
          OptimaliseRing.UI.Forms.GrafiekForm grafiekForm = new GrafiekForm(m_Berekening, kansen);
          grafiekForm.Icon = Icon;

          if (grafiekForm.ShowDialog(this) == DialogResult.OK)
          {
            //Console.WriteLine(grafiekForm.Left);
          }
          grafiekForm.Dispose();
        }
      }
    }

    /// <summary>
    /// Handles the Click event of the btnTabellen control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnBtnTabellenClick(object sender, EventArgs e)
    {
      m_MainForm.SetStatusLabel(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Genereren").ToString());

      ReportForm reportForm = new ReportForm(ThisAppProfile.Instance, ThisAppLanguage.Instance, m_Berekening);
      reportForm.Text = Application.ProductName + " - " + ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Resultaten", "Resultaten");

      // Strategiereport vullen
      reportForm.StrategieReport();

      if (reportForm.ShowDialog(this) == DialogResult.OK)
      {
        //reportForm.ReportViewerStrategie.RefreshReport();
        //reportForm.ReportViewerKansen.RefreshReport();

        //// Save reports
        //Warning[] warnings;
        //string[] streamids;
        //string mimeType;
        //string encoding;
        //string filenameExtension;

        //byte[] strategieBytes = reportForm.ReportViewerStrategie.LocalReport.Render(
        //  "Excel", null, out mimeType, out encoding, out filenameExtension,
        //  out streamids, out warnings);

        //using (FileStream fs = new FileStream(Path.Combine(m_Berekening.BerekeningenMap, "Strategie.xls"), FileMode.Create))
        //{
        //  fs.Write(strategieBytes, 0, strategieBytes.Length);
        //}

        //byte[] kansenBytes = reportForm.ReportViewerKansen.LocalReport.Render(
        //  "Excel", null, out mimeType, out encoding, out filenameExtension,
        //  out streamids, out warnings);

        //using (FileStream fs = new FileStream(Path.Combine(m_Berekening.BerekeningenMap, "Kansen.xls"), FileMode.Create))
        //{
        //  fs.Write(kansenBytes, 0, kansenBytes.Length);
        //}
      }

      m_MainForm.SetStatusLabel("");
      reportForm.Dispose();
    }

    #endregion Member functions ----------------------------------------------

  }
}
