namespace OptimaliseRing.UI
{
  partial class LegendaForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LegendaForm));
      this.picLegend = new System.Windows.Forms.PictureBox();
      this.uxButtonPanel = new System.Windows.Forms.Panel();
      this.btnBewerken = new System.Windows.Forms.Button();
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.lblLegendaKeuze = new System.Windows.Forms.ToolStripLabel();
      this.cmbLegendaKeuze = new System.Windows.Forms.ToolStripComboBox();
      ((System.ComponentModel.ISupportInitialize)(this.picLegend)).BeginInit();
      this.uxButtonPanel.SuspendLayout();
      this.toolStrip1.SuspendLayout();
      this.SuspendLayout();
      //
      // picLegend
      //
      this.picLegend.BackColor = System.Drawing.Color.White;
      resources.ApplyResources(this.picLegend, "picLegend");
      this.picLegend.Name = "picLegend";
      this.picLegend.TabStop = false;
      //
      // uxButtonPanel
      //
      this.uxButtonPanel.BackColor = System.Drawing.SystemColors.Control;
      this.uxButtonPanel.Controls.Add(this.btnBewerken);
      resources.ApplyResources(this.uxButtonPanel, "uxButtonPanel");
      this.uxButtonPanel.Name = "uxButtonPanel";
      //
      // btnBewerken
      //
      resources.ApplyResources(this.btnBewerken, "btnBewerken");
      this.btnBewerken.Name = "btnBewerken";
      this.btnBewerken.UseVisualStyleBackColor = true;
      this.btnBewerken.Click += new System.EventHandler(this.OnBtnBewerkenClick);
      //
      // toolStrip1
      //
      this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblLegendaKeuze,
            this.cmbLegendaKeuze});
      resources.ApplyResources(this.toolStrip1, "toolStrip1");
      this.toolStrip1.Name = "toolStrip1";
      //
      // lblLegendaKeuze
      //
      this.lblLegendaKeuze.Name = "lblLegendaKeuze";
      resources.ApplyResources(this.lblLegendaKeuze, "lblLegendaKeuze");
      //
      // cmbLegendaKeuze
      //
      resources.ApplyResources(this.cmbLegendaKeuze, "cmbLegendaKeuze");
      this.cmbLegendaKeuze.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbLegendaKeuze.Name = "cmbLegendaKeuze";
      this.cmbLegendaKeuze.SelectedIndexChanged += new System.EventHandler(this.OnCmbLegendaKeuzeSelectedIndexChanged);
      //
      // LegendaForm
      //
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.picLegend);
      this.Controls.Add(this.toolStrip1);
      this.Controls.Add(this.uxButtonPanel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "LegendaForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnLegendaFormFormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.picLegend)).EndInit();
      this.uxButtonPanel.ResumeLayout(false);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox picLegend;
    private System.Windows.Forms.Panel uxButtonPanel;
    private System.Windows.Forms.Button btnBewerken;
    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.ToolStripLabel lblLegendaKeuze;
    private System.Windows.Forms.ToolStripComboBox cmbLegendaKeuze;




  }
}
