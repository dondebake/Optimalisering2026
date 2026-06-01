namespace OptimaliseRing.UI.Forms
{
  partial class ResultatenForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultatenForm));
      this.btnOK = new System.Windows.Forms.Button();
      this.btnDelete = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.uxButtonPanel = new System.Windows.Forms.Panel();
      this.chkMultiSelect = new System.Windows.Forms.CheckBox();
      this.JaarKeuzePanel = new System.Windows.Forms.Panel();
      this.JaarKeuzeOptieUitvoerInfoLabel = new System.Windows.Forms.Label();
      this.nudOptimaleOverstromingskansenJaar = new System.Windows.Forms.NumericUpDown();
      this.JaarKeuzeOptie2Label = new System.Windows.Forms.Label();
      this.JaarKeuzeOptie1Label = new System.Windows.Forms.Label();
      this.JaarKeuzeOptie2 = new System.Windows.Forms.RadioButton();
      this.JaarKeuzeOptie1 = new System.Windows.Forms.RadioButton();
      this.gridPanel = new System.Windows.Forms.Panel();
      this.dgvResultaten = new System.Windows.Forms.DataGridView();
      this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.uxButtonPanel.SuspendLayout();
      this.JaarKeuzePanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nudOptimaleOverstromingskansenJaar)).BeginInit();
      this.gridPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvResultaten)).BeginInit();
      this.SuspendLayout();
      //
      // btnOK
      //
      resources.ApplyResources(this.btnOK, "btnOK");
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOK.Name = "btnOK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.BtnOKClick);
      //
      // btnDelete
      //
      resources.ApplyResources(this.btnDelete, "btnDelete");
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.UseVisualStyleBackColor = true;
      this.btnDelete.Click += new System.EventHandler(this.OnBtnDeleteClick);
      //
      // btnCancel
      //
      resources.ApplyResources(this.btnCancel, "btnCancel");
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.OnBtnCancelClick);
      //
      // uxButtonPanel
      //
      this.uxButtonPanel.BackColor = System.Drawing.Color.Transparent;
      this.uxButtonPanel.Controls.Add(this.chkMultiSelect);
      this.uxButtonPanel.Controls.Add(this.btnDelete);
      this.uxButtonPanel.Controls.Add(this.btnCancel);
      this.uxButtonPanel.Controls.Add(this.btnOK);
      resources.ApplyResources(this.uxButtonPanel, "uxButtonPanel");
      this.uxButtonPanel.Name = "uxButtonPanel";
      //
      // chkMultiSelect
      //
      resources.ApplyResources(this.chkMultiSelect, "chkMultiSelect");
      this.chkMultiSelect.Name = "chkMultiSelect";
      this.chkMultiSelect.UseVisualStyleBackColor = true;
      this.chkMultiSelect.CheckedChanged += new System.EventHandler(this.chkMultiSelect_CheckedChanged);
      //
      // JaarKeuzePanel
      //
      this.JaarKeuzePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.JaarKeuzePanel.Controls.Add(this.JaarKeuzeOptieUitvoerInfoLabel);
      this.JaarKeuzePanel.Controls.Add(this.nudOptimaleOverstromingskansenJaar);
      this.JaarKeuzePanel.Controls.Add(this.JaarKeuzeOptie2Label);
      this.JaarKeuzePanel.Controls.Add(this.JaarKeuzeOptie1Label);
      this.JaarKeuzePanel.Controls.Add(this.JaarKeuzeOptie2);
      this.JaarKeuzePanel.Controls.Add(this.JaarKeuzeOptie1);
      resources.ApplyResources(this.JaarKeuzePanel, "JaarKeuzePanel");
      this.JaarKeuzePanel.Name = "JaarKeuzePanel";
      //
      // JaarKeuzeOptieUitvoerInfoLabel
      //
      resources.ApplyResources(this.JaarKeuzeOptieUitvoerInfoLabel, "JaarKeuzeOptieUitvoerInfoLabel");
      this.JaarKeuzeOptieUitvoerInfoLabel.ForeColor = System.Drawing.Color.Blue;
      this.JaarKeuzeOptieUitvoerInfoLabel.Name = "JaarKeuzeOptieUitvoerInfoLabel";
      //
      // nudOptimaleOverstromingskansenJaar
      //
      resources.ApplyResources(this.nudOptimaleOverstromingskansenJaar, "nudOptimaleOverstromingskansenJaar");
      this.nudOptimaleOverstromingskansenJaar.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
      this.nudOptimaleOverstromingskansenJaar.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
      this.nudOptimaleOverstromingskansenJaar.Name = "nudOptimaleOverstromingskansenJaar";
      this.nudOptimaleOverstromingskansenJaar.Value = new decimal(new int[] {
            2015,
            0,
            0,
            0});
      //
      // JaarKeuzeOptie2Label
      //
      resources.ApplyResources(this.JaarKeuzeOptie2Label, "JaarKeuzeOptie2Label");
      this.JaarKeuzeOptie2Label.Name = "JaarKeuzeOptie2Label";
      this.JaarKeuzeOptie2Label.Click += new System.EventHandler(this.JaarKeuzeOptieLabel_Click);
      //
      // JaarKeuzeOptie1Label
      //
      resources.ApplyResources(this.JaarKeuzeOptie1Label, "JaarKeuzeOptie1Label");
      this.JaarKeuzeOptie1Label.Name = "JaarKeuzeOptie1Label";
      this.JaarKeuzeOptie1Label.Click += new System.EventHandler(this.JaarKeuzeOptieLabel_Click);
      //
      // JaarKeuzeOptie2
      //
      resources.ApplyResources(this.JaarKeuzeOptie2, "JaarKeuzeOptie2");
      this.JaarKeuzeOptie2.Checked = true;
      this.JaarKeuzeOptie2.Name = "JaarKeuzeOptie2";
      this.JaarKeuzeOptie2.TabStop = true;
      this.JaarKeuzeOptie2.UseVisualStyleBackColor = true;
      //
      // JaarKeuzeOptie1
      //
      resources.ApplyResources(this.JaarKeuzeOptie1, "JaarKeuzeOptie1");
      this.JaarKeuzeOptie1.Name = "JaarKeuzeOptie1";
      this.JaarKeuzeOptie1.UseVisualStyleBackColor = true;
      this.JaarKeuzeOptie1.CheckedChanged += new System.EventHandler(this.JaarKeuzeOptie1_CheckedChanged);
      //
      // gridPanel
      //
      this.gridPanel.Controls.Add(this.dgvResultaten);
      resources.ApplyResources(this.gridPanel, "gridPanel");
      this.gridPanel.Name = "gridPanel";
      //
      // dgvResultaten
      //
      this.dgvResultaten.AllowUserToAddRows = false;
      this.dgvResultaten.AllowUserToDeleteRows = false;
      this.dgvResultaten.AllowUserToResizeColumns = false;
      this.dgvResultaten.AllowUserToResizeRows = false;
      this.dgvResultaten.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
      this.dgvResultaten.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
      resources.ApplyResources(this.dgvResultaten, "dgvResultaten");
      this.dgvResultaten.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      this.dgvResultaten.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1});
      this.dgvResultaten.MultiSelect = false;
      this.dgvResultaten.Name = "dgvResultaten";
      this.dgvResultaten.ReadOnly = true;
      this.dgvResultaten.RowHeadersVisible = false;
      this.dgvResultaten.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgvResultaten.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnDgvResultatenCellDoubleClick);
      this.dgvResultaten.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvResultaten_CellClick);
      //
      // Column1
      //
      resources.ApplyResources(this.Column1, "Column1");
      this.Column1.Name = "Column1";
      this.Column1.ReadOnly = true;
      //
      // ResultatenForm
      //
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ControlBox = false;
      this.Controls.Add(this.gridPanel);
      this.Controls.Add(this.JaarKeuzePanel);
      this.Controls.Add(this.uxButtonPanel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.KeyPreview = true;
      this.Name = "ResultatenForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ResultatenForm_KeyDown);
      this.uxButtonPanel.ResumeLayout(false);
      this.uxButtonPanel.PerformLayout();
      this.JaarKeuzePanel.ResumeLayout(false);
      this.JaarKeuzePanel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nudOptimaleOverstromingskansenJaar)).EndInit();
      this.gridPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvResultaten)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnDelete;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Panel uxButtonPanel;
    private System.Windows.Forms.Panel JaarKeuzePanel;
    private System.Windows.Forms.RadioButton JaarKeuzeOptie2;
    private System.Windows.Forms.RadioButton JaarKeuzeOptie1;
    private System.Windows.Forms.Label JaarKeuzeOptie2Label;
    private System.Windows.Forms.Label JaarKeuzeOptie1Label;
    private System.Windows.Forms.NumericUpDown nudOptimaleOverstromingskansenJaar;
    private System.Windows.Forms.Label JaarKeuzeOptieUitvoerInfoLabel;
    private System.Windows.Forms.CheckBox chkMultiSelect;
    private System.Windows.Forms.Panel gridPanel;
    private System.Windows.Forms.DataGridView dgvResultaten;
    private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
  }
}
