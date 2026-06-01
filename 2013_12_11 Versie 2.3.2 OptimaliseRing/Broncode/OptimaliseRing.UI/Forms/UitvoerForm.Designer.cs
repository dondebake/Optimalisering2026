namespace OptimaliseRing.UI.Forms
{
  partial class UitvoerForm
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
       System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UitvoerForm));
       this.uxButtonPanel = new System.Windows.Forms.Panel();
       this.btnTabellen = new System.Windows.Forms.Button();
       this.btnCancel = new System.Windows.Forms.Button();
       this.btnOK = new System.Windows.Forms.Button();
       this.lblSelecteer = new System.Windows.Forms.Label();
       this.dgvUitvoer = new System.Windows.Forms.DataGridView();
       this.uxButtonPanel.SuspendLayout();
       ((System.ComponentModel.ISupportInitialize)(this.dgvUitvoer)).BeginInit();
       this.SuspendLayout();
       //
       // uxButtonPanel
       //
       this.uxButtonPanel.BackColor = System.Drawing.Color.Transparent;
       this.uxButtonPanel.Controls.Add(this.btnTabellen);
       this.uxButtonPanel.Controls.Add(this.btnCancel);
       this.uxButtonPanel.Controls.Add(this.btnOK);
       resources.ApplyResources(this.uxButtonPanel, "uxButtonPanel");
       this.uxButtonPanel.Name = "uxButtonPanel";
       //
       // btnTabellen
       //
       resources.ApplyResources(this.btnTabellen, "btnTabellen");
       this.btnTabellen.Name = "btnTabellen";
       this.btnTabellen.UseVisualStyleBackColor = true;
       this.btnTabellen.Click += new System.EventHandler(this.OnBtnTabellenClick);
       //
       // btnCancel
       //
       resources.ApplyResources(this.btnCancel, "btnCancel");
       this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
       this.btnCancel.Name = "btnCancel";
       this.btnCancel.UseVisualStyleBackColor = true;
       this.btnCancel.Click += new System.EventHandler(this.OnBtnCancelClick);
       //
       // btnOK
       //
       resources.ApplyResources(this.btnOK, "btnOK");
       this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
       this.btnOK.Name = "btnOK";
       this.btnOK.UseVisualStyleBackColor = true;
       this.btnOK.Click += new System.EventHandler(this.OnBtnOKClick);
       //
       // lblSelecteer
       //
       resources.ApplyResources(this.lblSelecteer, "lblSelecteer");
       this.lblSelecteer.Name = "lblSelecteer";
       //
       // dgvUitvoer
       //
       this.dgvUitvoer.AllowUserToAddRows = false;
       this.dgvUitvoer.AllowUserToDeleteRows = false;
       this.dgvUitvoer.AllowUserToResizeRows = false;
       this.dgvUitvoer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
       resources.ApplyResources(this.dgvUitvoer, "dgvUitvoer");
       this.dgvUitvoer.Name = "dgvUitvoer";
       this.dgvUitvoer.RowHeadersVisible = false;
       this.dgvUitvoer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
       this.dgvUitvoer.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnDgvUitvoerCellContentClick);
       //
       // UitvoerForm
       //
       resources.ApplyResources(this, "$this");
       this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
       this.CancelButton = this.btnCancel;
       this.ControlBox = false;
       this.Controls.Add(this.dgvUitvoer);
       this.Controls.Add(this.lblSelecteer);
       this.Controls.Add(this.uxButtonPanel);
       this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
       this.Name = "UitvoerForm";
       this.ShowIcon = false;
       this.ShowInTaskbar = false;
       this.uxButtonPanel.ResumeLayout(false);
       ((System.ComponentModel.ISupportInitialize)(this.dgvUitvoer)).EndInit();
       this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel uxButtonPanel;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Label lblSelecteer;
    private System.Windows.Forms.Button btnTabellen;
    private System.Windows.Forms.DataGridView dgvUitvoer;
  }
}
