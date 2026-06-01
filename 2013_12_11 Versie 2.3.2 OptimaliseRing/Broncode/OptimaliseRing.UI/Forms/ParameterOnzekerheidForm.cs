#region Copyright -------------------------------------------------------
// Copyright © 2009, Rijkswaterstaat/Waterdienst & ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Project    : 1377.10 Beslisk. model Optimale Veiligheid Overstromen
//              OptimaliseRing - Economische optimalisatie van veiligheidsniveaus van dijkringen
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//              Matthijs Duits, HKV lijn in water
//
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OptimaliseRing.Core;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace OptimaliseRing.UI
{

  /// <summary>
  /// ParameterOnzekerheidForm
  /// </summary>
  public partial class ParameterOnzekerheidForm : Form
  {
    private struct Waarde
    {
      /// <summary>
      /// Minvalue
      /// </summary>
      public double Minvalue;
      /// <summary>
      /// Maxvalue
      /// </summary>
      public double Maxvalue;

      /// <summary>
      /// Initializes a new instance of the <see cref="Waarde"/> struct.
      /// </summary>
      /// <param name="minvalue">The minvalue.</param>
      /// <param name="maxvalue">The maxvalue.</param>
      public Waarde(double minvalue, double maxvalue)
      {
        this.Minvalue = minvalue;
        this.Maxvalue = maxvalue;
      }
    }

    /// <summary>
    /// Berekening
    /// </summary>
    private Berekening m_Berekening;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterOnzekerheidForm"/> class.
    /// </summary>
    /// <param name="berekening">The berekening.</param>
    public ParameterOnzekerheidForm(Berekening berekening)
    {
      this.m_Berekening = berekening;

      InitializeComponent();

      this.lblAantal.Text = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "AantalScenarios").ToString();
      string jaarText = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Jaar").ToString();
      string displayMember = ThisAppCulture.Instance.Name == "en-GB" ? "Name" : "Naam";

      // max aantal rows
      this.txtAantal.Tag = 15;
      this.dgvBerekeningen.RowHeadersVisible = true;
      this.dgvBerekeningen.RowHeadersWidth = 50;
      this.dgvBerekeningen.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

      DataGridViewComboBoxColumn dataGridViewComboBoxColumnEconomischScenario = new DataGridViewComboBoxColumn();
      dataGridViewComboBoxColumnEconomischScenario.Name = "EconomischScenario";
      dataGridViewComboBoxColumnEconomischScenario.HeaderText
        = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "economischScenario").ToString();
      dataGridViewComboBoxColumnEconomischScenario.Width = 200;
      dataGridViewComboBoxColumnEconomischScenario.DataSource = m_Berekening.OptimaliseRingDB.GetDataset("SELECT * FROM EconomischScenario").Tables["General"];
      dataGridViewComboBoxColumnEconomischScenario.ValueMember = "ID";
      dataGridViewComboBoxColumnEconomischScenario.DisplayMember = displayMember;
      dataGridViewComboBoxColumnEconomischScenario.ToolTipText = dataGridViewComboBoxColumnEconomischScenario.HeaderText;
      dataGridViewComboBoxColumnEconomischScenario.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
      this.dgvBerekeningen.Columns.Add(dataGridViewComboBoxColumnEconomischScenario);

      DataGridViewComboBoxColumn dataGridViewComboBoxColumnKlimaatscenario = new DataGridViewComboBoxColumn();
      dataGridViewComboBoxColumnKlimaatscenario.Name = "Klimaatscenario";
      dataGridViewComboBoxColumnKlimaatscenario.HeaderText
        = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "KlimaatscenarioEnFysischMaximumAfvoer").ToString();
      dataGridViewComboBoxColumnKlimaatscenario.Width = 200;
      dataGridViewComboBoxColumnKlimaatscenario.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      dataGridViewComboBoxColumnKlimaatscenario.DataSource = m_Berekening.OptimaliseRingDB.GetDataset("SELECT * FROM Klimaat_AftoppenAfvoer").Tables["General"];
      dataGridViewComboBoxColumnKlimaatscenario.ValueMember = "ID";
      dataGridViewComboBoxColumnKlimaatscenario.DisplayMember = displayMember;
      dataGridViewComboBoxColumnKlimaatscenario.ToolTipText = dataGridViewComboBoxColumnKlimaatscenario.HeaderText;
      dataGridViewComboBoxColumnKlimaatscenario.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
      this.dgvBerekeningen.Columns.Add(dataGridViewComboBoxColumnKlimaatscenario);

      DataGridViewTextBoxColumn DataGridViewTextBoxColumnDiscontovoetSchade = new DataGridViewTextBoxColumn();
      DataGridViewTextBoxColumnDiscontovoetSchade.Name = "DiscontovoetSchade";
      DataGridViewTextBoxColumnDiscontovoetSchade.HeaderText =
              ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "DiscontovoetSchade").ToString() + " [%/"+jaarText+"]";
      DataGridViewTextBoxColumnDiscontovoetSchade.ValueType = typeof(double);
      DataGridViewTextBoxColumnDiscontovoetSchade.Tag = new Waarde(0.0, 100.0);
      DataGridViewTextBoxColumnDiscontovoetSchade.Width = 140;
      DataGridViewTextBoxColumnDiscontovoetSchade.ToolTipText = DataGridViewTextBoxColumnDiscontovoetSchade.HeaderText;
      DataGridViewTextBoxColumnDiscontovoetSchade.DefaultCellStyle.Format = "F1";
      DataGridViewTextBoxColumnDiscontovoetSchade.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
      this.dgvBerekeningen.Columns.Add(DataGridViewTextBoxColumnDiscontovoetSchade);

      DataGridViewTextBoxColumn DataGridViewTextBoxColumnDiscontovoetInvesteringen = new DataGridViewTextBoxColumn();
      DataGridViewTextBoxColumnDiscontovoetInvesteringen.Name = "DiscontovoetInvesteringen";
      DataGridViewTextBoxColumnDiscontovoetInvesteringen.HeaderText
        = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "DiscontovoetInvesteringen").ToString() + " [%/" + jaarText + "]";
      DataGridViewTextBoxColumnDiscontovoetInvesteringen.ValueType = typeof(double);
      DataGridViewTextBoxColumnDiscontovoetInvesteringen.Tag = new Waarde(0.0, 100.0);
      DataGridViewTextBoxColumnDiscontovoetInvesteringen.Width = 140;
      DataGridViewTextBoxColumnDiscontovoetInvesteringen.ToolTipText = DataGridViewTextBoxColumnDiscontovoetInvesteringen.HeaderText;
      DataGridViewTextBoxColumnDiscontovoetInvesteringen.DefaultCellStyle.Format = "F1";
      DataGridViewTextBoxColumnDiscontovoetInvesteringen.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
      this.dgvBerekeningen.Columns.Add(DataGridViewTextBoxColumnDiscontovoetInvesteringen);

      int aantal = berekening.Instellingen.Scenarioparameters.Count;
      this.txtAantal.Text = aantal.ToString();
      this.AddRows(this.dgvBerekeningen, aantal);
    }

    /// <summary>
    /// Handles the CellValidating event of the dgvBerekeningen control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Forms.DataGridViewCellValidatingEventArgs"/> instance containing the event data.</param>
    private void dgvBerekeningen_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
    {
      try
      {
        DataGridView dataGridView = (DataGridView)sender;
        DataGridViewColumn dataGridViewColumn = dataGridView.Columns[e.ColumnIndex];

        if (dataGridViewColumn.Tag != null)
        {
          if (dataGridViewColumn.Tag.GetType() == typeof(Waarde))
          {
            Waarde waarde = (Waarde)dataGridViewColumn.Tag;
            DataGridViewCell dataGridViewCell = dataGridView[e.ColumnIndex, e.RowIndex];

            double value = Hkv.General.ConvertString.ToDouble(e.FormattedValue.ToString());

            if (value < waarde.Minvalue || value > waarde.Maxvalue)
            {
              e.Cancel = true;
              dataGridViewCell.ErrorText = dataGridViewCell.ToolTipText;// string.Format("tussen {0:F1} en {1:F1}", waarde.Minvalue, waarde.Maxvalue);
              Hkv.General.ApplicationError.Display(this, dataGridViewCell.ToolTipText);
            }
            else
            {
              dataGridViewCell.ErrorText = string.Empty;
            }
          }
        }
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Handles the DataError event of the dgvBerekeningen control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Forms.DataGridViewDataErrorEventArgs"/> instance containing the event data.</param>
    private void dgvBerekeningen_DataError(object sender, DataGridViewDataErrorEventArgs e)
    {
      try
      {
        e.Cancel = true;
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Handles the Click event of the btnCancel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void btnCancel_Click(object sender, EventArgs e)
    {
      try
      {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Handles the Click event of the btnOK control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void btnOK_Click(object sender, EventArgs e)
    {
      try
      {
        // Opslaan
        this.SetParametersToInstellingen();

        this.DialogResult = DialogResult.OK;
        this.Close();
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Sets the parameters to instellingen.
    /// </summary>
    private void SetParametersToInstellingen()
    {
      SortedList<int, InstellingenOnzekerheid> Scenarioparameters = this.m_Berekening.Instellingen.Scenarioparameters;
      Scenarioparameters.Clear();

      foreach (DataGridViewRow row in this.dgvBerekeningen.Rows)
      {

        DataGridViewComboBoxCell cellEconomischScenario = (DataGridViewComboBoxCell)row.Cells["EconomischScenario"];
        DataGridViewComboBoxCell cellKlimaatscenario = (DataGridViewComboBoxCell)row.Cells["Klimaatscenario"];
        DataGridViewTextBoxCell cellDiscontovoetSchade = (DataGridViewTextBoxCell)row.Cells["DiscontovoetSchade"];
        DataGridViewTextBoxCell cellDiscontovoetInvesteringen = (DataGridViewTextBoxCell)row.Cells["DiscontovoetInvesteringen"];

        int economischScenario = (int)cellEconomischScenario.Value;
        int klimaatScenarioEnFysischMaxAfvoer = (int)cellKlimaatscenario.Value;
        double discontovoetSchade = (double)cellDiscontovoetSchade.Value;
        double discontovoetInvesteringen = (double)cellDiscontovoetInvesteringen.Value;

        Scenarioparameters.Add(row.Index, new InstellingenOnzekerheid(
          economischScenario, klimaatScenarioEnFysischMaxAfvoer, discontovoetSchade, discontovoetInvesteringen));
      }
    }

    #region Textbox events -----------------------------------------------

    /// <summary>
    /// Handles the TextChanged event of the txtAantal control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Forms.KeyPressEventArgs"/> instance containing the event data.</param>
    private void txtAantal_TextChanged(object sender, System.Windows.Forms.KeyPressEventArgs e)
    {
      try
      {
        if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
          e.Handled = true;
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Handles the TextChanged event of the txtAantal control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void txtAantal_TextChanged(object sender, EventArgs e)
    {
      try
      {
        TextBox textBox = (TextBox)sender;
        int getal;
        if (int.TryParse(textBox.Text, out getal))
        {
          textBox.Text = Convert.ToString(Math.Max(Math.Min(getal, (int)this.txtAantal.Tag), 0));
        }
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Handles the Leave event of the txtAantal control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void txtAantal_Leave(object sender, EventArgs e)
    {
      try
      {
        TextBox textBox = (TextBox)sender;
        int getal;
        if (int.TryParse(textBox.Text, out getal))
        {
          this.AddRows(sender, getal);
        }
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Adds the rows.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="aantal">The aantal.</param>
    private void AddRows(object sender, int aantal)
    {
      int currentAantalRows = this.dgvBerekeningen.Rows.Count;

      Hashtable waarden = new Hashtable();
      foreach (DataGridViewColumn dataGridViewColumn in this.dgvBerekeningen.Columns)
      {
        if (dataGridViewColumn.Tag != null)
        {
          if (dataGridViewColumn.Tag.GetType() == typeof(Waarde))
          {
            Waarde waarde = (Waarde)dataGridViewColumn.Tag;
            waarden.Add(dataGridViewColumn.Name, waarde);
          }
        }
      }

      if (aantal == 0)
      {
        this.dgvBerekeningen.Rows.Clear();
      }
      else if (aantal > currentAantalRows)
      {
        this.dgvBerekeningen.Rows.Add(aantal - currentAantalRows);

        for (int rowIndex = currentAantalRows; rowIndex < this.dgvBerekeningen.Rows.Count; rowIndex++)
        {
          int economischScenario = this.m_Berekening.Instellingen.EconomischScenario;
          int klimaatScenarioEnFysischMaxAfvoer = this.m_Berekening.Instellingen.KlimaatScenarioEnFysischMaxAfvoer;
          double discontovoetSchade = this.m_Berekening.Instellingen.DiscontovoetSchade;
          double discontovoetInvesteringen = this.m_Berekening.Instellingen.DiscontovoetInvesteringen;

          if (this.m_Berekening.Instellingen.Scenarioparameters.ContainsKey(rowIndex))
          {
            economischScenario = this.m_Berekening.Instellingen.Scenarioparameters[rowIndex].EconomischScenario;
            klimaatScenarioEnFysischMaxAfvoer = this.m_Berekening.Instellingen.Scenarioparameters[rowIndex].KlimaatScenarioEnFysischMaxAfvoer;
            discontovoetSchade = this.m_Berekening.Instellingen.Scenarioparameters[rowIndex].DiscontovoetSchade;
            discontovoetInvesteringen = this.m_Berekening.Instellingen.Scenarioparameters[rowIndex].DiscontovoetInvesteringen;
          }

          using (DataGridViewRow row = this.dgvBerekeningen.Rows[rowIndex])
          {
            row.HeaderCell.Value = (rowIndex + 1).ToString();

            DataGridViewComboBoxCell cellEconomischScenario = (DataGridViewComboBoxCell)row.Cells["EconomischScenario"];
            cellEconomischScenario.Value = economischScenario;
            cellEconomischScenario.ToolTipText = this.GetToolTipTextForCell(waarden, "EconomischScenario");

            DataGridViewComboBoxCell cellKlimaatscenario = (DataGridViewComboBoxCell)row.Cells["Klimaatscenario"];
            cellKlimaatscenario.Value = klimaatScenarioEnFysischMaxAfvoer;
            cellKlimaatscenario.ToolTipText = this.GetToolTipTextForCell(waarden, "Klimaatscenario");

            DataGridViewTextBoxCell cellDiscontovoetSchade = (DataGridViewTextBoxCell)row.Cells["DiscontovoetSchade"];
            cellDiscontovoetSchade.Value = discontovoetSchade;
            cellDiscontovoetSchade.ToolTipText = this.GetToolTipTextForCell(waarden, "DiscontovoetSchade");

            DataGridViewTextBoxCell cellDiscontovoetInvesteringen = (DataGridViewTextBoxCell)row.Cells["DiscontovoetInvesteringen"];
            cellDiscontovoetInvesteringen.Value = discontovoetInvesteringen;
            cellDiscontovoetInvesteringen.ToolTipText = this.GetToolTipTextForCell(waarden, "DiscontovoetInvesteringen");
          }
        }
      }
      else
      {

        while (this.dgvBerekeningen.Rows.Count > aantal)
        {
          this.dgvBerekeningen.Rows.Remove(this.dgvBerekeningen.Rows[this.dgvBerekeningen.Rows.Count - 1]);
        }

      }
    }

    /// <summary>
    /// Gets the tool tip text for cell.
    /// </summary>
    /// <param name="waarden">The waarden.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <returns></returns>
    private string GetToolTipTextForCell(Hashtable waarden, string columnName)
    {
      if (waarden.ContainsKey(columnName))
      {
        Waarde waarde = (Waarde)waarden[columnName];
        return string.Format(ThisAppLanguage.Instance.GetValue("Captions:"
          + ThisAppCulture.Instance.Name, "MinMaxWaarde").ToString(), waarde.Minvalue, waarde.Maxvalue);
      }
      return string.Empty;
    }

    #endregion Textbox events -----------------------------------------------

  }
}
