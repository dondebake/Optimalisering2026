namespace OptimaliseRing.UI.Forms
{
  partial class GrafiekForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GrafiekForm));
      this.uxButtonPanel = new System.Windows.Forms.Panel();
      this.btnPrint = new System.Windows.Forms.Button();
      this.btnCopy = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOK = new System.Windows.Forms.Button();
      this.tchGrafiek = new Steema.TeeChart.TChart();
      this.uxButtonPanel.SuspendLayout();
      this.SuspendLayout();
      //
      // uxButtonPanel
      //
      this.uxButtonPanel.BackColor = System.Drawing.Color.Transparent;
      this.uxButtonPanel.Controls.Add(this.btnPrint);
      this.uxButtonPanel.Controls.Add(this.btnCopy);
      this.uxButtonPanel.Controls.Add(this.btnCancel);
      this.uxButtonPanel.Controls.Add(this.btnOK);
      resources.ApplyResources(this.uxButtonPanel, "uxButtonPanel");
      this.uxButtonPanel.Name = "uxButtonPanel";
      //
      // btnPrint
      //
      resources.ApplyResources(this.btnPrint, "btnPrint");
      this.btnPrint.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnPrint.Name = "btnPrint";
      this.btnPrint.UseVisualStyleBackColor = true;
      this.btnPrint.Click += new System.EventHandler(this.OnBtnPrintClick);
      //
      // btnCopy
      //
      resources.ApplyResources(this.btnCopy, "btnCopy");
      this.btnCopy.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCopy.Name = "btnCopy";
      this.btnCopy.UseVisualStyleBackColor = true;
      this.btnCopy.Click += new System.EventHandler(this.OnBtnCopyClick);
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
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnOK.Name = "btnOK";
      this.btnOK.UseVisualStyleBackColor = true;
      //
      // tchGrafiek
      //
      //
      //
      //
      this.tchGrafiek.Aspect.ElevationFloat = 345;
      this.tchGrafiek.Aspect.RotationFloat = 345;
      this.tchGrafiek.Aspect.View3D = false;
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Axes.Bottom.Automatic = true;
      //
      //
      //
      this.tchGrafiek.Axes.Bottom.Grid.Style = System.Drawing.Drawing2D.DashStyle.Dash;
      this.tchGrafiek.Axes.Bottom.Grid.ZPosition = 0;
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Axes.Bottom.Labels.Font.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.Bottom.Labels.Shadow.Visible = false;
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Axes.Bottom.Title.Font.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.Bottom.Title.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.Depth.Automatic = true;
      //
      //
      //
      this.tchGrafiek.Axes.Depth.Grid.Style = System.Drawing.Drawing2D.DashStyle.Dash;
      this.tchGrafiek.Axes.Depth.Grid.ZPosition = 0;
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Axes.Depth.Labels.Font.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.Depth.Labels.Shadow.Visible = false;
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Axes.Depth.Title.Font.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.Depth.Title.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.DepthTop.Automatic = true;
      //
      //
      //
      this.tchGrafiek.Axes.DepthTop.Grid.Style = System.Drawing.Drawing2D.DashStyle.Dash;
      this.tchGrafiek.Axes.DepthTop.Grid.ZPosition = 0;
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Axes.DepthTop.Labels.Font.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.DepthTop.Labels.Shadow.Visible = false;
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Axes.DepthTop.Title.Font.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.DepthTop.Title.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.Left.Automatic = true;
      //
      //
      //
      this.tchGrafiek.Axes.Left.Grid.Style = System.Drawing.Drawing2D.DashStyle.Dash;
      this.tchGrafiek.Axes.Left.Grid.ZPosition = 0;
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Axes.Left.Labels.Font.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.Left.Labels.Shadow.Visible = false;
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Axes.Left.Title.Font.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.Left.Title.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.Right.Automatic = true;
      //
      //
      //
      this.tchGrafiek.Axes.Right.Grid.Style = System.Drawing.Drawing2D.DashStyle.Dash;
      this.tchGrafiek.Axes.Right.Grid.ZPosition = 0;
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Axes.Right.Labels.Font.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.Right.Labels.Shadow.Visible = false;
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Axes.Right.Title.Font.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.Right.Title.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.Top.Automatic = true;
      //
      //
      //
      this.tchGrafiek.Axes.Top.Grid.Style = System.Drawing.Drawing2D.DashStyle.Dash;
      this.tchGrafiek.Axes.Top.Grid.ZPosition = 0;
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Axes.Top.Labels.Font.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.Top.Labels.Shadow.Visible = false;
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Axes.Top.Title.Font.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Axes.Top.Title.Shadow.Visible = false;
      resources.ApplyResources(this.tchGrafiek, "tchGrafiek");
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Footer.Font.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Footer.Shadow.Visible = false;
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Header.Font.Shadow.Visible = false;
      this.tchGrafiek.Header.Lines = new string[] {
        "TeeChart"};
      //
      //
      //
      this.tchGrafiek.Header.Shadow.Visible = false;
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Legend.Font.Shadow.Visible = false;
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Legend.Title.Font.Bold = true;
      //
      //
      //
      this.tchGrafiek.Legend.Title.Font.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Legend.Title.Pen.Visible = false;
      //
      //
      //
      this.tchGrafiek.Legend.Title.Shadow.Visible = false;
      this.tchGrafiek.Name = "tchGrafiek";
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Panel.Shadow.Visible = false;
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.SubFooter.Font.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.SubFooter.Shadow.Visible = false;
      //
      //
      //
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.SubHeader.Font.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.SubHeader.Shadow.Visible = false;
      //
      //
      //
      //
      //
      //
      this.tchGrafiek.Walls.Back.AutoHide = false;
      //
      //
      //
      this.tchGrafiek.Walls.Back.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Walls.Bottom.AutoHide = false;
      //
      //
      //
      this.tchGrafiek.Walls.Bottom.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Walls.Left.AutoHide = false;
      //
      //
      //
      this.tchGrafiek.Walls.Left.Shadow.Visible = false;
      //
      //
      //
      this.tchGrafiek.Walls.Right.AutoHide = false;
      //
      //
      //
      this.tchGrafiek.Walls.Right.Shadow.Visible = false;
      //
      // GrafiekForm
      //
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.Controls.Add(this.tchGrafiek);
      this.Controls.Add(this.uxButtonPanel);
      this.Name = "GrafiekForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnGrafiekFormFormClosing);
      this.uxButtonPanel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel uxButtonPanel;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnCopy;
    private System.Windows.Forms.Button btnPrint;
    private Steema.TeeChart.TChart tchGrafiek;
  }
}
