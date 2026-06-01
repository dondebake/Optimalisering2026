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
  public partial class KeringenForm : Form
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

    private SortedList m_Keringen;                    // Lijst met keringen uit de database
    private SortedList m_Leeg;                        // dummy param voor overload
    private SortedList m_Selecties;                   // Lijst met de voorgedefinieerde selecties van dijkringdelen
    private String m_BerekeningenMap;                 // Naam van de map met de berekeningen
    private bool m_Initializing;
    private bool m_MayClose = true;                   // Controle of form mag sluiten
    private Berekening m_Berekening;
    private DataGridViewHelper m_DataGridViewHelper;

    public KeringenForm(Berekening berekening)
    {
      InitializeComponent();

      m_Initializing = true;

      this.m_Berekening = berekening;

      this.m_Keringen = m_Berekening.OptimaliseRingDB.GetDataset("SELECT * FROM [B-keringen] ORDER BY [B-Kering]").ToSortedList();


      m_DataGridViewHelper = new DataGridViewHelper(
            ThisAppProfile.Instance
            , ThisAppLanguage.Instance
            , ThisAppCulture.Instance
            , this.dgvBkeringen
            , m_Berekening
            , m_Leeg
            , m_Keringen);

      m_DataGridViewHelper.InitializeKeringenGrid();
      m_DataGridViewHelper.VulKeringenGrid();

      m_Initializing = false;

    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      try
      {
        // Opslaan
        this.SetToedelingInstellingen();

        this.DialogResult = DialogResult.OK;
        this.Close();
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Opslaan percentage
    /// </summary>
    private void SetToedelingInstellingen()
    {

      SortedList<int, InstellingenKeringen> keringenParameters = this.m_Berekening.Instellingen.Keringenparameters;
      keringenParameters.Clear();

      foreach (DataGridViewRow row in this.dgvBkeringen.Rows)
      {

        DataGridViewTextBoxCell cellPercentage = (DataGridViewTextBoxCell)row.Cells["Percentage1"];
        DataGridViewTextBoxCell cellNaam = (DataGridViewTextBoxCell)row.Cells["KeringNummer"];

        int perc1 = Convert.ToInt16(cellPercentage.FormattedValue);
        string naam = cellNaam.FormattedValue.ToString();
        cellPercentage = (DataGridViewTextBoxCell)row.Cells["Percentage2"];
        int perc2 = Convert.ToInt16(cellPercentage.FormattedValue);

        keringenParameters.Add(row.Index, new InstellingenKeringen(naam, perc1, perc2));

      }
      //bewaren beide percentages voor elke rij

      //ThisAppProfile.Instance.SetValue("keringen", "6:1", 45);

      //ThisAppProfile.Instance.SetValue("keringen", "6:2", 22);
    }

    private void dgvBkeringen_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
    {
      {
        try
        {
          DataGridView dataGridView = (DataGridView)sender;
          DataGridViewColumn dataGridViewColumn = dataGridView.Columns[e.ColumnIndex];

          if (dataGridViewColumn.Tag != null)
          {
            //waarom gaat het hier fout?? maar even hard coderen...
            //if (dataGridViewColumn.Tag.GetType() == typeof(Waarde))
            //{
            //Waarde waarde = (Waarde)dataGridViewColumn.Tag.;
            DataGridViewCell dataGridViewCell = dataGridView[e.ColumnIndex, e.RowIndex];

            double value = Hkv.General.ConvertString.ToDouble(e.FormattedValue.ToString());

            //if (value < waarde.Minvalue || value > waarde.Maxvalue)
            if (value < 0 || value > 100)
            {
              e.Cancel = true;
              dataGridViewCell.ErrorText = "Waarde moet liggen tussen 0 en 100";
              Hkv.General.ApplicationError.Display(this, dataGridViewCell.ErrorText);
            }
            else
            {
              dataGridViewCell.ErrorText = string.Empty;
            }
            //}
          }
        }
        catch (Exception appex)
        {
          Hkv.General.ApplicationError.Display(this, appex);
        }
      }


    }

    /// <summary>
    /// Called when [keringen form form load].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void OnKeringenFormFormLoad(object sender, EventArgs e)
    {
      try
      {
        this.Top = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Top", "0"));
        this.Left = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Left", "0"));
        this.Width = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Width", "640"));
        this.Height = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Height", "480"));
      }
      catch (Exception appex)
      {
        ThisAppErr.Instance.Display(this, appex);
        this.Close();
      }
    }

    /// <summary>
    /// Called when [keringen form form closing].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
    private void OnKeringenFormFormClosing(object sender, FormClosingEventArgs e)
    {
      try
      {
        if (this.WindowState == FormWindowState.Normal)
        {
          ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Top", this.Top.ToString());
          ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Left", this.Left.ToString());
          ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Width", this.Width.ToString());
          ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Height", this.Height.ToString());
        }
      }
      catch (Exception appex)
      {
        ThisAppErr.Instance.Display(this, appex);
      }
    }
  }
}
