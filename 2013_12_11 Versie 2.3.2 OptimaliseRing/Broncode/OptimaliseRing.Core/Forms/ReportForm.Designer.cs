namespace OptimaliseRing.Core
{
  /// <summary>
  ///
  /// </summary>
  partial class ReportForm
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportForm));
      Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
      this.StrategieBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.uxButtonPanel = new System.Windows.Forms.Panel();
      this.btnOK = new System.Windows.Forms.Button();
      this.tabControl = new Crownwood.DotNetMagic.Controls.TabControl();
      this.tabPageStrategie = new Crownwood.DotNetMagic.Controls.TabPage();
      this.reportViewerStrategie = new Microsoft.Reporting.WinForms.ReportViewer();
      this.tabPageKansen = new Crownwood.DotNetMagic.Controls.TabPage();
      this.reportViewerKansen = new Microsoft.Reporting.WinForms.ReportViewer();
      this.KansenBindingSource = new System.Windows.Forms.BindingSource(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.StrategieBindingSource)).BeginInit();
      this.uxButtonPanel.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.tabPageStrategie.SuspendLayout();
      this.tabPageKansen.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.KansenBindingSource)).BeginInit();
      this.SuspendLayout();
      //
      // StrategieBindingSource
      //
      this.StrategieBindingSource.DataMember = "Strategie";
      //
      // uxButtonPanel
      //
      this.uxButtonPanel.BackColor = System.Drawing.Color.Transparent;
      this.uxButtonPanel.Controls.Add(this.btnOK);
      resources.ApplyResources(this.uxButtonPanel, "uxButtonPanel");
      this.uxButtonPanel.Name = "uxButtonPanel";
      //
      // btnOK
      //
      resources.ApplyResources(this.btnOK, "btnOK");
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnOK.Name = "btnOK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.OnBtnOKClick);
      //
      // tabControl
      //
      resources.ApplyResources(this.tabControl, "tabControl");
      this.tabControl.Name = "tabControl";
      this.tabControl.OfficeDockSides = false;
      this.tabControl.SelectedIndex = 0;
      this.tabControl.ShowDropSelect = false;
      this.tabControl.TabPages.AddRange(new Crownwood.DotNetMagic.Controls.TabPage[] {
            this.tabPageStrategie,
            this.tabPageKansen});
      this.tabControl.TextTips = true;
      this.tabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(this.TabControlSelectionChanged);
      //
      // tabPageStrategie
      //
      this.tabPageStrategie.Controls.Add(this.reportViewerStrategie);
      this.tabPageStrategie.InactiveBackColor = System.Drawing.Color.Empty;
      this.tabPageStrategie.InactiveTextBackColor = System.Drawing.Color.Empty;
      this.tabPageStrategie.InactiveTextColor = System.Drawing.Color.Empty;
      resources.ApplyResources(this.tabPageStrategie, "tabPageStrategie");
      this.tabPageStrategie.Name = "tabPageStrategie";
      this.tabPageStrategie.SelectBackColor = System.Drawing.Color.Empty;
      this.tabPageStrategie.SelectTextBackColor = System.Drawing.Color.Empty;
      this.tabPageStrategie.SelectTextColor = System.Drawing.Color.Empty;
      //
      // reportViewerStrategie
      //
      this.reportViewerStrategie.AllowDrop = true;
      this.reportViewerStrategie.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
      resources.ApplyResources(this.reportViewerStrategie, "reportViewerStrategie");
      this.reportViewerStrategie.Name = "reportViewerStrategie";
      //
      // tabPageKansen
      //
      this.tabPageKansen.Controls.Add(this.reportViewerKansen);
      this.tabPageKansen.InactiveBackColor = System.Drawing.Color.Empty;
      this.tabPageKansen.InactiveTextBackColor = System.Drawing.Color.Empty;
      this.tabPageKansen.InactiveTextColor = System.Drawing.Color.Empty;
      resources.ApplyResources(this.tabPageKansen, "tabPageKansen");
      this.tabPageKansen.Name = "tabPageKansen";
      this.tabPageKansen.SelectBackColor = System.Drawing.Color.Empty;
      this.tabPageKansen.Selected = false;
      this.tabPageKansen.SelectTextBackColor = System.Drawing.Color.Empty;
      this.tabPageKansen.SelectTextColor = System.Drawing.Color.Empty;
      //
      // reportViewerKansen
      //
      this.reportViewerKansen.AllowDrop = true;
      this.reportViewerKansen.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
      resources.ApplyResources(this.reportViewerKansen, "reportViewerKansen");
      this.reportViewerKansen.Name = "reportViewerKansen";
      //
      // KansenBindingSource
      //
      this.KansenBindingSource.DataMember = "Kansen";
      //
      // ReportForm
      //
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnOK;
      this.ControlBox = false;
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.uxButtonPanel);
      this.Name = "ReportForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Load += new System.EventHandler(this.OnReportFormLoad);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnReportFormFormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.StrategieBindingSource)).EndInit();
      this.uxButtonPanel.ResumeLayout(false);
      this.tabControl.ResumeLayout(false);
      this.tabPageStrategie.ResumeLayout(false);
      this.tabPageKansen.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.KansenBindingSource)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel uxButtonPanel;
    private System.Windows.Forms.Button btnOK;
    private Crownwood.DotNetMagic.Controls.TabControl tabControl;
    private Crownwood.DotNetMagic.Controls.TabPage tabPageStrategie;
    private Crownwood.DotNetMagic.Controls.TabPage tabPageKansen;
    private Microsoft.Reporting.WinForms.ReportViewer reportViewerStrategie;
    private Microsoft.Reporting.WinForms.ReportViewer reportViewerKansen;
    private System.Windows.Forms.BindingSource KansenBindingSource;
    private System.Windows.Forms.BindingSource StrategieBindingSource;

  }
}
