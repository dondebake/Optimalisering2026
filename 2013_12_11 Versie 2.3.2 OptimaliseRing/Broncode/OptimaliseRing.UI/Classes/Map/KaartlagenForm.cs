using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Resources;

using MapObjectsLT2;

namespace OptimaliseRing.UI
{
   /// <summary>
   ///
   /// </summary>
   public partial class KaartlagenForm :  Form
   {
      private MapControl m_MapControl;
      private bool m_Initializing;

      private string m_ColumnTonen;
      private string m_ColumnNaam;

      /// <summary>
      /// Initializes a new instance of the <see cref="T:KaartlagenForm"/> class.
      /// </summary>
      /// <param name="mapControl">The map control.</param>
      public KaartlagenForm(MapControl mapControl)
      {
         InitializeComponent();

         CreateGridColumns();

         m_MapControl = mapControl;

         TonenKaartlagen();

         EnableDisableButtons();
      }

      /// <summary>
      /// Tonen van de kaartlagen.
      /// </summary>
      private void TonenKaartlagen()
      {
         this.m_Initializing = true;

         this.dgvKaartlagen.Rows.Clear();

         Boolean visible = false;
         String name = "";
         string tag = "";

         for (int i = 0; i < this.m_MapControl.Canvas.Layers.Count; i++)
         {
            switch (this.m_MapControl.LayerType(i))
            {
               case LayerTypeConstants.moMapLayer:
                  MapLayer mapLayer = (MapLayer)this.m_MapControl.Canvas.Layers.Item(i);
                  visible = mapLayer.Visible;
                  name = mapLayer.Name;
                  tag = mapLayer.Tag;
                  break;
               case LayerTypeConstants.moImageLayer:
                  ImageLayer imageLayer = (ImageLayer)this.m_MapControl.Canvas.Layers.Item(i);
                  visible = imageLayer.Visible;
                  name = imageLayer.Name;
                  tag = imageLayer.Tag;
                  break;
            }

            if (string.Compare(tag, "ACHTERGROND", true) == 0)
            {
               int row = this.dgvKaartlagen.RowCount;
               this.dgvKaartlagen.Rows.Add(new DataGridViewRow());
               this.dgvKaartlagen[m_ColumnTonen, row].Value = visible;
               this.dgvKaartlagen[m_ColumnNaam, row].Value = name;
            }
         }
         this.m_Initializing = false;
      }

      /// <summary>
      /// De commando-buttons voor Delete, Properties, Down en Up
      /// wel of niet beschikbaar maken, afhankelijk van de selectie in het grid
      /// </summary>
      private void EnableDisableButtons()
      {
         if (this.dgvKaartlagen.SelectedRows.Count > 0)
         {
            int selectedRow = this.dgvKaartlagen.SelectedRows[0].Index;

            // De Delete en Properties button alleen beschikbaar als er wat is gesleceerd
            this.btnDelete.Enabled = this.dgvKaartlagen.SelectedRows.Count > 0;
            this.btnProperties.Enabled = this.dgvKaartlagen.SelectedRows.Count > 0;

            // Down kan alleen als er wat geslecteerd is en we al niet helemaal beneden zijn
            this.btnDown.Enabled = selectedRow > -1 & selectedRow < this.dgvKaartlagen.RowCount - 1;
            this.uxDownButton.Enabled = this.btnDown.Enabled;
            // Up kan alleen als er wat geslecteerd is en we niet al helemaal boven zijn
            this.btnUp.Enabled = selectedRow > 0;
            this.uxUpButton.Enabled = this.btnUp.Enabled;
         }
      }

      /// <summary>
      /// Adds the layer.
      /// </summary>
      private void AddLayer()
      {
         this.m_MapControl.AddLayer();
         TonenKaartlagen();
         EnableDisableButtons();
      }

      /// <summary>
      /// Deletes the layer.
      /// </summary>
      private void DeleteLayer()
      {
         if (this.dgvKaartlagen.SelectedRows.Count > 0)
         {
            int selectedRow = this.dgvKaartlagen.SelectedRows[0].Index;

            this.m_MapControl.Canvas.Layers.Remove(Convert.ToInt16(selectedRow));

            TonenKaartlagen();

            EnableDisableButtons();
         }
      }

      private void OnBtnAddClick(object sender, EventArgs e)
      {
         try
         {
            AddLayer();
         }
         catch (Exception appex)
         {
            ThisAppErr.Instance.Raise(1004, new object[] { appex.Message });
         }
      }

      private void ToolStripButtonsClick(object sender, EventArgs e)
      {
         if (sender == this.uxAddButton)
         {
            AddLayer();
         }
         else if (sender == this.uxDeleteButton)
         {
            DeleteLayer();
         }
         else if (sender == this.uxUpButton)
         {
            LayerUp();
         }
         else if (sender == this.uxDownButton)
         {
            LayerDown();
         }
         else if (sender == this.uxPropertiesButton)
         {
            Eigenschappen();
         }
      }

      private void OnBtnDeleteClick(object sender, EventArgs e)
      {
         try
         {
            if (this.dgvKaartlagen.SelectedRows.Count > 0)
            {
               DeleteLayer();
            }
         }
         catch (Exception appex)
         {
            ThisAppErr.Instance.Raise(1004, new object[] { appex.Message });
         }

      }

      private void OnBtnUpClick(object sender, EventArgs e)
      {
         try
         {
            LayerUp();
         }
         catch (Exception appex)
         {
            ThisAppErr.Instance.Raise(1004, new object[] { appex.Message });
         }

      }

      private void LayerUp()
      {
         if (this.dgvKaartlagen.SelectedRows.Count > 0)
         {
            int selectedRow = this.dgvKaartlagen.SelectedRows[0].Index;

            if (selectedRow > 0)
            {
               int toRow = selectedRow - 1;
               this.m_MapControl.Canvas.Layers.MoveTo(Convert.ToInt16(selectedRow), Convert.ToInt16(toRow));

               TonenKaartlagen();

               this.dgvKaartlagen.Rows[toRow].Selected = true;

               this.m_MapControl.Canvas.Refresh();
            }
            EnableDisableButtons();
         }
      }

      private void OnBtnDownClick(object sender, EventArgs e)
      {
         try
         {
            LayerDown();
         }
         catch (Exception appex)
         {
            ThisAppErr.Instance.Raise(1004, new object[] { appex.Message });
         }
      }

      private void LayerDown()
      {
         if (this.dgvKaartlagen.SelectedRows.Count > 0)
         {
            int selectedRow = this.dgvKaartlagen.SelectedRows[0].Index;

            if (selectedRow < this.dgvKaartlagen.RowCount - 1)
            {
               int toRow = selectedRow + 1;
               this.m_MapControl.Canvas.Layers.MoveTo(Convert.ToInt16(selectedRow), Convert.ToInt16(toRow));

               TonenKaartlagen();

               this.dgvKaartlagen.Rows[toRow].Selected = true;

               this.m_MapControl.Canvas.Refresh();
            }
            EnableDisableButtons();
         }
      }

      private void OnBtnPropertiesClick(object sender, EventArgs e)
      {
         try
         {
            Eigenschappen();
            this.m_MapControl.Canvas.Refresh();
         }
         catch (Exception appex)
         {
            ThisAppErr.Instance.Raise(1004, new object[] { appex.Message });
         }

      }

      private void Eigenschappen()
      {
         if (this.dgvKaartlagen.SelectedRows.Count > 0)
         {
            int index = this.dgvKaartlagen.SelectedRows[0].Index;

            UInt32 color = 0;
            Object layer = this.m_MapControl.Canvas.Layers.Item(index);

            switch (this.m_MapControl.LayerType(index))
            {
               case LayerTypeConstants.moMapLayer:
                  MapLayer mapLayer = (MapLayer)this.m_MapControl.Canvas.Layers.Item(index);
                  color = mapLayer.Symbol.Color;
                  break;
            }

            KaartlaagEigenschappenForm kaartlaagEigenschappenForm = new KaartlaagEigenschappenForm(this.m_MapControl.Profile, layer);
            kaartlaagEigenschappenForm.Icon = this.Icon;

            if (kaartlaagEigenschappenForm.ShowDialog(this) == DialogResult.OK)
            {
               this.m_MapControl.Canvas.Invalidate();
            }
            else
            {
               if (this.m_MapControl.LayerType(index) == LayerTypeConstants.moMapLayer)
               {
                  MapLayer mapLayer = (MapLayer)this.m_MapControl.Canvas.Layers.Item(index);
                  mapLayer.Symbol.Color = color;
               }
            }

            kaartlaagEigenschappenForm.Close();
            kaartlaagEigenschappenForm.Dispose();
         }
      }

      private void ApplyChanges(int index)
      {
         // Het doorvoeren van de door de gebruiker aangegeven veranderingen
         // (eigenlijk alleen maar of een laag zichtbaar is of niet).

         bool visible = (bool)this.dgvKaartlagen[m_ColumnTonen, index].Value;

         Object layer = this.m_MapControl.Canvas.Layers.Item(index);

         switch (this.m_MapControl.LayerType(index))
         {
            case LayerTypeConstants.moMapLayer:
               MapLayer mapLayer = (MapLayer)this.m_MapControl.Canvas.Layers.Item(index);
               mapLayer.Visible = visible;
               break;
            case LayerTypeConstants.moImageLayer:
               ImageLayer imageLayer = (ImageLayer)this.m_MapControl.Canvas.Layers.Item(index);
               imageLayer.Visible = visible;
               break;
         }

         this.m_MapControl.Canvas.Refresh();

      }

      private void CreateGridColumns()
      {
         this.dgvKaartlagen.Columns.Clear();

         m_ColumnNaam = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Naam", "Naam").ToString();
         m_ColumnTonen = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Tonen", "Tonen").ToString();

         DataGridViewCheckBoxColumn column1 = new DataGridViewCheckBoxColumn();
         column1.Name = m_ColumnTonen;
         column1.HeaderText = column1.Name;
         column1.SortMode = DataGridViewColumnSortMode.NotSortable;
         column1.Width = 40;
         column1.ReadOnly = false;
         this.dgvKaartlagen.Columns.Add(column1);

         DataGridViewTextBoxColumn column2 = new DataGridViewTextBoxColumn();
         column2.Name = m_ColumnNaam;
         column2.HeaderText = column2.Name;
         column2.SortMode = DataGridViewColumnSortMode.NotSortable;
         column2.Width = 200;
         column2.ReadOnly = true;
         this.dgvKaartlagen.Columns.Add(column2);

         this.dgvKaartlagen.CurrentCellDirtyStateChanged += new EventHandler(OnDgvKaartlagenCurrentCellDirtyStateChanged);
         this.dgvKaartlagen.CellValueChanged += new DataGridViewCellEventHandler(OnDgvKaartlagenCellValueChanged);

         this.dgvKaartlagen.SelectionChanged += new EventHandler(OnDgvKaartlagenSelectionChanged);
      }

      void OnDgvKaartlagenSelectionChanged(object sender, EventArgs e)
      {
         EnableDisableButtons();
      }

      // This event handler manually raises the CellValueChanged event
      // by calling the CommitEdit method.
      void OnDgvKaartlagenCurrentCellDirtyStateChanged(object sender, EventArgs e)
      {
         if (this.dgvKaartlagen.IsCurrentCellDirty)
         {
            this.dgvKaartlagen.CommitEdit(DataGridViewDataErrorContexts.Commit);
         }
      }

      void OnDgvKaartlagenCellValueChanged(object sender, DataGridViewCellEventArgs e)
      {
         if (!this.m_Initializing)
         {
            if (this.dgvKaartlagen.Columns[e.ColumnIndex].Name == m_ColumnTonen)
            {
               ApplyChanges(e.RowIndex);
            }
         }
      }

      /// <summary>
      /// Called when [BTN OK click].
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
      private void OnBtnOKClick(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.OK;
      }

      /// <summary>
      /// Handles the Click event of the btnCancel control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
      private void OnBtnCancelClick(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
      }

   }
}
