using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OptimaliseRing.UI.Forms
{
  public partial class LegendaItemsForm : Form
  {

    private int m_index;

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:LegendaItemsForm"/> class.
    /// </summary>
    public LegendaItemsForm(int index)
    {
      InitializeComponent();

      this.m_index = index;

      Initialize();
    }

    private SortedList AppLegendaItems
    {
      get
      {
        return ThisAppLegendaItems2.Instance[this.m_index];
      }
    }

    /// <summary>
    /// Initialiseer this instance.
    /// </summary>
    private void Initialize()
    {
      // Create a DataSet and add the table.
      DataSet dataset = new DataSet();

      // Create a table to store customer values.
      DataTable dataTable = new DataTable("LegendaItems", "LegendaItems");

      //// Define the columns for the table.
      dataTable.Columns.Add(new DataColumn("LegendaItem", typeof(double)));

      // Populate the table.
      for (int i = 0; i < this.AppLegendaItems.Count - 1; i++)
      {
        LegendaItem legendaItem = (LegendaItem)this.AppLegendaItems.GetByIndex(i);

        DataRow tempDataRow = dataTable.NewRow();
        tempDataRow[0] = legendaItem.Max;
        dataTable.Rows.Add(tempDataRow);
      }

      dataset.Tables.Add(dataTable);

      this.dgvGrid.DataSource = dataset;
      this.dgvGrid.DataMember = "LegendaItems";

    }

    /// <summary>
    /// Handles the Click event of the btnOK control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnBtnOKClick(object sender, EventArgs e)
    {
      double minimum = 1.0;
      for (int i = 0; i < this.AppLegendaItems.Count; i++)
      {
        LegendaItem legendaItem = (LegendaItem)this.AppLegendaItems.GetByIndex(i);
        if (i == this.AppLegendaItems.Count - 1)
        {
          legendaItem.Min = minimum;
        }
        else
        {
          legendaItem.Min = minimum;
          legendaItem.Max = Double.Parse(this.dgvGrid["LegendaItem", i].Value.ToString());
          minimum = legendaItem.Max;
        }
      }
      this.DialogResult = DialogResult.OK;
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
    /// Handles the CellValidating event of the dgvGrid control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.Windows.Forms.DataGridViewCellValidatingEventArgs"/> instance containing the event data.</param>
    private void OnDgvGridCellValidating(object sender, DataGridViewCellValidatingEventArgs e)
    {
      // validate input
      double minimum = 1.0;
      double maximum = double.MaxValue;
      if (e.RowIndex > 0)
      {
        minimum = Double.Parse(this.dgvGrid["LegendaItem", e.RowIndex - 1].Value.ToString());
      }
      if (e.RowIndex < this.AppLegendaItems.Count - 2)
      {
        maximum = Double.Parse(this.dgvGrid["LegendaItem", e.RowIndex + 1].Value.ToString());
      }

      // Validate the entry by disallowing empty strings.
      if (this.dgvGrid.Columns[e.ColumnIndex].Name == "LegendaItem")
      {
        if (String.IsNullOrEmpty(e.FormattedValue.ToString()))
        {
          ThisAppErr.Instance.Raise(1002, null);
          MessageBox.Show("Legenda item mag niet leeg zijn");
          e.Cancel = true;
        }
        double v = Double.Parse(e.FormattedValue.ToString());
        if (v < minimum || v > maximum)
        {
          ThisAppErr.Instance.Raise(1003, new object[] { minimum, maximum });
          e.Cancel = true;
        }
      }
    }

    /// <summary>
    /// Handles the CellEndEdit event of the dgvGrid control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
    private void OnDgvGridCellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
      // Clear the row error in case the user presses ESC.
      this.dgvGrid.Rows[e.RowIndex].ErrorText = String.Empty;
    }

    /// <summary>
    /// Handles the DataError event of the dgvGrid control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.Windows.Forms.DataGridViewDataErrorEventArgs"/> instance containing the event data.</param>
    private void OnDgvGridDataError(object sender, DataGridViewDataErrorEventArgs e)
    {
      if ((e.Exception) is ConstraintException)
      {
        DataGridView view = (DataGridView)sender;

        ThisAppErr.Instance.Raise(1004, new object[] { view.Rows[e.RowIndex].ErrorText });
        e.ThrowException = false;
      }

    }


  }
}
