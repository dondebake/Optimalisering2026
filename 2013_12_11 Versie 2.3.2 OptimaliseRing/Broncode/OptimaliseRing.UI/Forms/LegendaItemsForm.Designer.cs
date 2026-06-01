namespace OptimaliseRing.UI.Forms
{
  /// <summary>
  ///
  /// </summary>
  partial class LegendaItemsForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LegendaItemsForm));
      this.uxButtonPanel = new System.Windows.Forms.Panel();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOK = new System.Windows.Forms.Button();
      this.groupBox = new System.Windows.Forms.GroupBox();
      this.dgvGrid = new System.Windows.Forms.DataGridView();
      this.uxButtonPanel.SuspendLayout();
      this.groupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvGrid)).BeginInit();
      this.SuspendLayout();
      //
      // uxButtonPanel
      //
      this.uxButtonPanel.AccessibleDescription = null;
      this.uxButtonPanel.AccessibleName = null;
      resources.ApplyResources(this.uxButtonPanel, "uxButtonPanel");
      this.uxButtonPanel.BackColor = System.Drawing.Color.Transparent;
      this.uxButtonPanel.BackgroundImage = null;
      this.uxButtonPanel.Controls.Add(this.btnCancel);
      this.uxButtonPanel.Controls.Add(this.btnOK);
      this.uxButtonPanel.Font = null;
      this.uxButtonPanel.Name = "uxButtonPanel";
      //
      // btnCancel
      //
      this.btnCancel.AccessibleDescription = null;
      this.btnCancel.AccessibleName = null;
      resources.ApplyResources(this.btnCancel, "btnCancel");
      this.btnCancel.BackgroundImage = null;
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Font = null;
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.OnBtnCancelClick);
      //
      // btnOK
      //
      this.btnOK.AccessibleDescription = null;
      this.btnOK.AccessibleName = null;
      resources.ApplyResources(this.btnOK, "btnOK");
      this.btnOK.BackgroundImage = null;
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnOK.Font = null;
      this.btnOK.Name = "btnOK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.OnBtnOKClick);
      //
      // groupBox
      //
      this.groupBox.AccessibleDescription = null;
      this.groupBox.AccessibleName = null;
      resources.ApplyResources(this.groupBox, "groupBox");
      this.groupBox.BackgroundImage = null;
      this.groupBox.Controls.Add(this.dgvGrid);
      this.groupBox.Font = null;
      this.groupBox.Name = "groupBox";
      this.groupBox.TabStop = false;
      //
      // dgvGrid
      //
      this.dgvGrid.AccessibleDescription = null;
      this.dgvGrid.AccessibleName = null;
      this.dgvGrid.AllowUserToAddRows = false;
      this.dgvGrid.AllowUserToDeleteRows = false;
      this.dgvGrid.AllowUserToResizeColumns = false;
      this.dgvGrid.AllowUserToResizeRows = false;
      resources.ApplyResources(this.dgvGrid, "dgvGrid");
      this.dgvGrid.BackgroundImage = null;
      this.dgvGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvGrid.ColumnHeadersVisible = false;
      this.dgvGrid.Font = null;
      this.dgvGrid.MultiSelect = false;
      this.dgvGrid.Name = "dgvGrid";
      this.dgvGrid.RowHeadersVisible = false;
      this.dgvGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
      this.dgvGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.OnDgvGridCellValidating);
      this.dgvGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnDgvGridCellEndEdit);
      this.dgvGrid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.OnDgvGridDataError);
      //
      // LegendaItemsForm
      //
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.CancelButton = this.btnCancel;
      this.ControlBox = false;
      this.Controls.Add(this.groupBox);
      this.Controls.Add(this.uxButtonPanel);
      this.Font = null;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Icon = null;
      this.Name = "LegendaItemsForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.uxButtonPanel.ResumeLayout(false);
      this.groupBox.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvGrid)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel uxButtonPanel;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.GroupBox groupBox;
    private System.Windows.Forms.DataGridView dgvGrid;
  }
}
