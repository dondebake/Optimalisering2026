namespace OptimaliseRing.UI
{
  partial class ParameterOnzekerheidForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParameterOnzekerheidForm));
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.txtAantal = new System.Windows.Forms.TextBox();
      this.dgvBerekeningen = new System.Windows.Forms.DataGridView();
      this.lblAantal = new System.Windows.Forms.Label();
      this.btnCancel = new System.Windows.Forms.Button();
      this.uxButtonPanel = new System.Windows.Forms.Panel();
      this.btnOK = new System.Windows.Forms.Button();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvBerekeningen)).BeginInit();
      this.uxButtonPanel.SuspendLayout();
      this.SuspendLayout();
      //
      // groupBox1
      //
      this.groupBox1.Controls.Add(this.txtAantal);
      this.groupBox1.Controls.Add(this.dgvBerekeningen);
      this.groupBox1.Controls.Add(this.lblAantal);
      resources.ApplyResources(this.groupBox1, "groupBox1");
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.TabStop = false;
      //
      // txtAantal
      //
      resources.ApplyResources(this.txtAantal, "txtAantal");
      this.txtAantal.Name = "txtAantal";
      this.txtAantal.TextChanged += new System.EventHandler(this.txtAantal_TextChanged);
      this.txtAantal.Leave += new System.EventHandler(this.txtAantal_Leave);
      this.txtAantal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAantal_TextChanged);
      //
      // dgvBerekeningen
      //
      this.dgvBerekeningen.AllowUserToAddRows = false;
      this.dgvBerekeningen.AllowUserToDeleteRows = false;
      this.dgvBerekeningen.AllowUserToResizeColumns = false;
      this.dgvBerekeningen.AllowUserToResizeRows = false;
      this.dgvBerekeningen.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      resources.ApplyResources(this.dgvBerekeningen, "dgvBerekeningen");
      this.dgvBerekeningen.Name = "dgvBerekeningen";
      this.dgvBerekeningen.RowHeadersVisible = false;
      this.dgvBerekeningen.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
      this.dgvBerekeningen.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvBerekeningen_CellValidating);
      this.dgvBerekeningen.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvBerekeningen_DataError);
      //
      // lblAantal
      //
      resources.ApplyResources(this.lblAantal, "lblAantal");
      this.lblAantal.Name = "lblAantal";
      //
      // btnCancel
      //
      resources.ApplyResources(this.btnCancel, "btnCancel");
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      //
      // uxButtonPanel
      //
      this.uxButtonPanel.BackColor = System.Drawing.Color.Transparent;
      this.uxButtonPanel.Controls.Add(this.btnCancel);
      this.uxButtonPanel.Controls.Add(this.btnOK);
      resources.ApplyResources(this.uxButtonPanel, "uxButtonPanel");
      this.uxButtonPanel.Name = "uxButtonPanel";
      //
      // btnOK
      //
      resources.ApplyResources(this.btnOK, "btnOK");
      this.btnOK.Name = "btnOK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      //
      // ParameterOnzekerheidForm
      //
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ControlBox = false;
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.uxButtonPanel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ParameterOnzekerheidForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvBerekeningen)).EndInit();
      this.uxButtonPanel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.DataGridView dgvBerekeningen;
    private System.Windows.Forms.Label lblAantal;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Panel uxButtonPanel;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.TextBox txtAantal;
  }
}
