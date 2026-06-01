namespace OptimaliseRing.UI.Forms
{
  partial class KeringenForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeringenForm));
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.uxButtonPanel = new System.Windows.Forms.Panel();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOK = new System.Windows.Forms.Button();
      this.dgvBkeringen = new System.Windows.Forms.DataGridView();
      this.lblToedeling = new System.Windows.Forms.Label();
      this.statusStripMain = new System.Windows.Forms.StatusStrip();
      this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.groupBox1.SuspendLayout();
      this.uxButtonPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvBkeringen)).BeginInit();
      this.statusStripMain.SuspendLayout();
      this.SuspendLayout();
      //
      // groupBox1
      //
      this.groupBox1.Controls.Add(this.uxButtonPanel);
      this.groupBox1.Controls.Add(this.dgvBkeringen);
      this.groupBox1.Controls.Add(this.lblToedeling);
      resources.ApplyResources(this.groupBox1, "groupBox1");
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.TabStop = false;
      //
      // uxButtonPanel
      //
      this.uxButtonPanel.BackColor = System.Drawing.Color.Transparent;
      this.uxButtonPanel.Controls.Add(this.btnCancel);
      this.uxButtonPanel.Controls.Add(this.btnOK);
      resources.ApplyResources(this.uxButtonPanel, "uxButtonPanel");
      this.uxButtonPanel.Name = "uxButtonPanel";
      //
      // btnCancel
      //
      resources.ApplyResources(this.btnCancel, "btnCancel");
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      //
      // btnOK
      //
      resources.ApplyResources(this.btnOK, "btnOK");
      this.btnOK.Name = "btnOK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      //
      // dgvBkeringen
      //
      this.dgvBkeringen.AllowUserToAddRows = false;
      this.dgvBkeringen.AllowUserToDeleteRows = false;
      this.dgvBkeringen.AllowUserToResizeColumns = false;
      this.dgvBkeringen.AllowUserToResizeRows = false;
      resources.ApplyResources(this.dgvBkeringen, "dgvBkeringen");
      this.dgvBkeringen.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvBkeringen.Name = "dgvBkeringen";
      this.dgvBkeringen.RowHeadersVisible = false;
      this.dgvBkeringen.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
      this.dgvBkeringen.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvBkeringen_CellValidating);
      //
      // lblToedeling
      //
      resources.ApplyResources(this.lblToedeling, "lblToedeling");
      this.lblToedeling.Name = "lblToedeling";
      //
      // statusStripMain
      //
      this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
      resources.ApplyResources(this.statusStripMain, "statusStripMain");
      this.statusStripMain.Name = "statusStripMain";
      //
      // statusLabel
      //
      this.statusLabel.BackColor = System.Drawing.SystemColors.Control;
      this.statusLabel.Name = "statusLabel";
      resources.ApplyResources(this.statusLabel, "statusLabel");
      this.statusLabel.Spring = true;
      //
      // KeringenForm
      //
      this.AcceptButton = this.btnOK;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.statusStripMain);
      this.MinimizeBox = false;
      this.Name = "KeringenForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Load += new System.EventHandler(this.OnKeringenFormFormLoad);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnKeringenFormFormClosing);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.uxButtonPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvBkeringen)).EndInit();
      this.statusStripMain.ResumeLayout(false);
      this.statusStripMain.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.DataGridView dgvBkeringen;
    private System.Windows.Forms.Label lblToedeling;
    private System.Windows.Forms.Panel uxButtonPanel;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.StatusStrip statusStripMain;
    private System.Windows.Forms.ToolStripStatusLabel statusLabel;
  }
}
