namespace OptimaliseRing.Core
{
  /// <summary>
  ///
  /// </summary>
  partial class CompartimenteringForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompartimenteringForm));
      this.uxButtonPanel = new System.Windows.Forms.Panel();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOK = new System.Windows.Forms.Button();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.dgvCompartimenteren = new System.Windows.Forms.DataGridView();
      this.uxButtonPanel.SuspendLayout();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvCompartimenteren)).BeginInit();
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
      this.btnOK.Font = null;
      this.btnOK.Name = "btnOK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.OnBtnOKClick);
      //
      // menuStrip1
      //
      this.menuStrip1.AccessibleDescription = null;
      this.menuStrip1.AccessibleName = null;
      resources.ApplyResources(this.menuStrip1, "menuStrip1");
      this.menuStrip1.BackgroundImage = null;
      this.menuStrip1.Font = null;
      this.menuStrip1.Name = "menuStrip1";
      //
      // groupBox1
      //
      this.groupBox1.AccessibleDescription = null;
      this.groupBox1.AccessibleName = null;
      resources.ApplyResources(this.groupBox1, "groupBox1");
      this.groupBox1.BackgroundImage = null;
      this.groupBox1.Controls.Add(this.dgvCompartimenteren);
      this.groupBox1.Font = null;
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.TabStop = false;
      //
      // dgvCompartimenteren
      //
      this.dgvCompartimenteren.AccessibleDescription = null;
      this.dgvCompartimenteren.AccessibleName = null;
      this.dgvCompartimenteren.AllowUserToAddRows = false;
      this.dgvCompartimenteren.AllowUserToDeleteRows = false;
      this.dgvCompartimenteren.AllowUserToResizeColumns = false;
      this.dgvCompartimenteren.AllowUserToResizeRows = false;
      resources.ApplyResources(this.dgvCompartimenteren, "dgvCompartimenteren");
      this.dgvCompartimenteren.BackgroundImage = null;
      this.dgvCompartimenteren.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvCompartimenteren.Font = null;
      this.dgvCompartimenteren.Name = "dgvCompartimenteren";
      this.dgvCompartimenteren.RowHeadersVisible = false;
      this.dgvCompartimenteren.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
      this.dgvCompartimenteren.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnDgvCompartimenterenCellEnter);
      //
      // CompartimenteringForm
      //
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.CancelButton = this.btnCancel;
      this.ControlBox = false;
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.uxButtonPanel);
      this.Controls.Add(this.menuStrip1);
      this.Font = null;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = null;
      this.Name = "CompartimenteringForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Load += new System.EventHandler(this.OnCompartimenteringFormLoad);
      this.uxButtonPanel.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvCompartimenteren)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel uxButtonPanel;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.DataGridView dgvCompartimenteren;
  }
}
