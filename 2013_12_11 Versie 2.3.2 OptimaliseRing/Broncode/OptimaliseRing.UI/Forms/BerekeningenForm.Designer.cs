namespace OptimaliseRing.UI.Forms
{
  /// <summary>
  ///
  /// </summary>
  partial class BerekeningenForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BerekeningenForm));
      this.uxButtonPanel = new System.Windows.Forms.Panel();
      this.btnInstellingen = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOK = new System.Windows.Forms.Button();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.selecterenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.txtNaam = new System.Windows.Forms.TextBox();
      this.dgvBerekeningen = new System.Windows.Forms.DataGridView();
      this.lblOverzichtsbestand = new System.Windows.Forms.Label();
      this.uxButtonPanel.SuspendLayout();
      this.menuStrip1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvBerekeningen)).BeginInit();
      this.SuspendLayout();
      //
      // uxButtonPanel
      //
      this.uxButtonPanel.AccessibleDescription = null;
      this.uxButtonPanel.AccessibleName = null;
      resources.ApplyResources(this.uxButtonPanel, "uxButtonPanel");
      this.uxButtonPanel.BackColor = System.Drawing.Color.Transparent;
      this.uxButtonPanel.BackgroundImage = null;
      this.uxButtonPanel.Controls.Add(this.btnInstellingen);
      this.uxButtonPanel.Controls.Add(this.btnCancel);
      this.uxButtonPanel.Controls.Add(this.btnOK);
      this.uxButtonPanel.Font = null;
      this.uxButtonPanel.Name = "uxButtonPanel";
      //
      // btnInstellingen
      //
      this.btnInstellingen.AccessibleDescription = null;
      this.btnInstellingen.AccessibleName = null;
      resources.ApplyResources(this.btnInstellingen, "btnInstellingen");
      this.btnInstellingen.BackgroundImage = null;
      this.btnInstellingen.Font = null;
      this.btnInstellingen.Name = "btnInstellingen";
      this.btnInstellingen.UseVisualStyleBackColor = true;
      this.btnInstellingen.Click += new System.EventHandler(this.OnBtnInstellingenClick);
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
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selecterenToolStripMenuItem});
      this.menuStrip1.Name = "menuStrip1";
      //
      // selecterenToolStripMenuItem
      //
      this.selecterenToolStripMenuItem.AccessibleDescription = null;
      this.selecterenToolStripMenuItem.AccessibleName = null;
      resources.ApplyResources(this.selecterenToolStripMenuItem, "selecterenToolStripMenuItem");
      this.selecterenToolStripMenuItem.BackgroundImage = null;
      this.selecterenToolStripMenuItem.Name = "selecterenToolStripMenuItem";
      this.selecterenToolStripMenuItem.ShortcutKeyDisplayString = null;
      //
      // groupBox1
      //
      this.groupBox1.AccessibleDescription = null;
      this.groupBox1.AccessibleName = null;
      resources.ApplyResources(this.groupBox1, "groupBox1");
      this.groupBox1.BackgroundImage = null;
      this.groupBox1.Controls.Add(this.txtNaam);
      this.groupBox1.Controls.Add(this.dgvBerekeningen);
      this.groupBox1.Controls.Add(this.lblOverzichtsbestand);
      this.groupBox1.Font = null;
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.TabStop = false;
      //
      // txtNaam
      //
      this.txtNaam.AccessibleDescription = null;
      this.txtNaam.AccessibleName = null;
      resources.ApplyResources(this.txtNaam, "txtNaam");
      this.txtNaam.BackgroundImage = null;
      this.txtNaam.Font = null;
      this.txtNaam.Name = "txtNaam";
      this.txtNaam.TextChanged += new System.EventHandler(this.OnTxtNaamTextChanged);
      //
      // dgvBerekeningen
      //
      this.dgvBerekeningen.AccessibleDescription = null;
      this.dgvBerekeningen.AccessibleName = null;
      this.dgvBerekeningen.AllowUserToAddRows = false;
      this.dgvBerekeningen.AllowUserToDeleteRows = false;
      this.dgvBerekeningen.AllowUserToResizeColumns = false;
      this.dgvBerekeningen.AllowUserToResizeRows = false;
      resources.ApplyResources(this.dgvBerekeningen, "dgvBerekeningen");
      this.dgvBerekeningen.BackgroundImage = null;
      this.dgvBerekeningen.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvBerekeningen.Font = null;
      this.dgvBerekeningen.Name = "dgvBerekeningen";
      this.dgvBerekeningen.RowHeadersVisible = false;
      this.dgvBerekeningen.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
      this.dgvBerekeningen.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnDgvBerekeningenCellValueChanged);
      this.dgvBerekeningen.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBerekeningen_CellDoubleClick);
      this.dgvBerekeningen.CurrentCellDirtyStateChanged += new System.EventHandler(this.OnDgvBerekeningenCurrentCellDirtyStateChanged);
      //
      // lblOverzichtsbestand
      //
      this.lblOverzichtsbestand.AccessibleDescription = null;
      this.lblOverzichtsbestand.AccessibleName = null;
      resources.ApplyResources(this.lblOverzichtsbestand, "lblOverzichtsbestand");
      this.lblOverzichtsbestand.Font = null;
      this.lblOverzichtsbestand.Name = "lblOverzichtsbestand";
      //
      // BerekeningenForm
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
      this.Name = "BerekeningenForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnBerekeningenFormFormClosing);
      this.uxButtonPanel.ResumeLayout(false);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvBerekeningen)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel uxButtonPanel;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.DataGridView dgvBerekeningen;
    private System.Windows.Forms.Label lblOverzichtsbestand;
    private System.Windows.Forms.ToolStripMenuItem selecterenToolStripMenuItem;
    private System.Windows.Forms.TextBox txtNaam;
    private System.Windows.Forms.Button btnInstellingen;
  }
}
