namespace OptimaliseRing.UI
{
  /// <summary>
  /// Map control
  /// </summary>
  partial class MapControl
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

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapControl));
      this.tscMap = new System.Windows.Forms.ToolStripContainer();
      this.mapTipsPanel = new System.Windows.Forms.Panel();
      this.mapTipsLabel = new System.Windows.Forms.Label();
      this.Canvas = new AxMapObjectsLT2.AxMap();
      this.uxTopoToolStrip = new System.Windows.Forms.ToolStrip();
      this.uxSelectButton = new System.Windows.Forms.ToolStripButton();
      this.uxZoomInButton = new System.Windows.Forms.ToolStripButton();
      this.uxZoomOutButton = new System.Windows.Forms.ToolStripButton();
      this.uzZoomAreaButton = new System.Windows.Forms.ToolStripButton();
      this.uxDragButton = new System.Windows.Forms.ToolStripButton();
      this.uxCenterButton = new System.Windows.Forms.ToolStripButton();
      this.uxWorldButton = new System.Windows.Forms.ToolStripButton();
      this.uxSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.uxResultaatButton = new System.Windows.Forms.ToolStripButton();
      this.uxKaartlagenButton = new System.Windows.Forms.ToolStripButton();
      this.uxPrintButton = new System.Windows.Forms.ToolStripButton();
      this.uxSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.cmbWeergave = new System.Windows.Forms.ToolStripComboBox();
      this.uxCoordinatesLabel = new System.Windows.Forms.ToolStripLabel();
      this.toolTipMap = new System.Windows.Forms.ToolTip(this.components);
      this.mapTipsTimer = new System.Windows.Forms.Timer(this.components);
      this.tscMap.ContentPanel.SuspendLayout();
      this.tscMap.TopToolStripPanel.SuspendLayout();
      this.tscMap.SuspendLayout();
      this.mapTipsPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.Canvas)).BeginInit();
      this.uxTopoToolStrip.SuspendLayout();
      this.SuspendLayout();
      //
      // tscMap
      //
      //
      // tscMap.ContentPanel
      //
      this.tscMap.ContentPanel.Controls.Add(this.mapTipsPanel);
      this.tscMap.ContentPanel.Controls.Add(this.Canvas);
      resources.ApplyResources(this.tscMap.ContentPanel, "tscMap.ContentPanel");
      resources.ApplyResources(this.tscMap, "tscMap");
      this.tscMap.LeftToolStripPanelVisible = false;
      this.tscMap.Name = "tscMap";
      this.tscMap.RightToolStripPanelVisible = false;
      //
      // tscMap.TopToolStripPanel
      //
      this.tscMap.TopToolStripPanel.Controls.Add(this.uxTopoToolStrip);
      this.tscMap.Click += new System.EventHandler(this.TscMapClick);
      //
      // mapTipsPanel
      //
      resources.ApplyResources(this.mapTipsPanel, "mapTipsPanel");
      this.mapTipsPanel.BackColor = System.Drawing.SystemColors.Info;
      this.mapTipsPanel.Controls.Add(this.mapTipsLabel);
      this.mapTipsPanel.Name = "mapTipsPanel";
      //
      // mapTipsLabel
      //
      this.mapTipsLabel.BackColor = System.Drawing.Color.Transparent;
      resources.ApplyResources(this.mapTipsLabel, "mapTipsLabel");
      this.mapTipsLabel.ForeColor = System.Drawing.SystemColors.InfoText;
      this.mapTipsLabel.Name = "mapTipsLabel";
      //
      // Canvas
      //
      resources.ApplyResources(this.Canvas, "Canvas");
      this.Canvas.Name = "Canvas";
      this.Canvas.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("Canvas.OcxState")));
      this.Canvas.MouseDownEvent += new AxMapObjectsLT2._DMapEvents_MouseDownEventHandler(this.CanvasMouseDownEvent);
      this.Canvas.MouseMoveEvent += new AxMapObjectsLT2._DMapEvents_MouseMoveEventHandler(this.CanvasMouseMoveEvent);
      //
      // uxTopoToolStrip
      //
      resources.ApplyResources(this.uxTopoToolStrip, "uxTopoToolStrip");
      this.uxTopoToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxSelectButton,
            this.uxZoomInButton,
            this.uxZoomOutButton,
            this.uzZoomAreaButton,
            this.uxDragButton,
            this.uxCenterButton,
            this.uxWorldButton,
            this.uxSeparator1,
            this.uxResultaatButton,
            this.uxKaartlagenButton,
            this.uxPrintButton,
            this.uxSeparator2,
            this.cmbWeergave,
            this.uxCoordinatesLabel});
      this.uxTopoToolStrip.Name = "uxTopoToolStrip";
      this.uxTopoToolStrip.Stretch = true;
      //
      // uxSelectButton
      //
      this.uxSelectButton.Checked = true;
      this.uxSelectButton.CheckState = System.Windows.Forms.CheckState.Checked;
      this.uxSelectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.uxSelectButton, "uxSelectButton");
      this.uxSelectButton.Name = "uxSelectButton";
      this.uxSelectButton.Click += new System.EventHandler(this.TscMapClick);
      //
      // uxZoomInButton
      //
      this.uxZoomInButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.uxZoomInButton, "uxZoomInButton");
      this.uxZoomInButton.Name = "uxZoomInButton";
      this.uxZoomInButton.Click += new System.EventHandler(this.TscMapClick);
      //
      // uxZoomOutButton
      //
      this.uxZoomOutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.uxZoomOutButton, "uxZoomOutButton");
      this.uxZoomOutButton.Name = "uxZoomOutButton";
      this.uxZoomOutButton.Click += new System.EventHandler(this.TscMapClick);
      //
      // uzZoomAreaButton
      //
      this.uzZoomAreaButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.uzZoomAreaButton, "uzZoomAreaButton");
      this.uzZoomAreaButton.Name = "uzZoomAreaButton";
      this.uzZoomAreaButton.Click += new System.EventHandler(this.TscMapClick);
      //
      // uxDragButton
      //
      this.uxDragButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.uxDragButton, "uxDragButton");
      this.uxDragButton.Name = "uxDragButton";
      this.uxDragButton.Click += new System.EventHandler(this.TscMapClick);
      //
      // uxCenterButton
      //
      this.uxCenterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.uxCenterButton, "uxCenterButton");
      this.uxCenterButton.Name = "uxCenterButton";
      this.uxCenterButton.Click += new System.EventHandler(this.TscMapClick);
      //
      // uxWorldButton
      //
      this.uxWorldButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.uxWorldButton, "uxWorldButton");
      this.uxWorldButton.Name = "uxWorldButton";
      this.uxWorldButton.Click += new System.EventHandler(this.TscMapClick);
      //
      // uxSeparator1
      //
      this.uxSeparator1.Name = "uxSeparator1";
      resources.ApplyResources(this.uxSeparator1, "uxSeparator1");
      //
      // uxResultaatButton
      //
      this.uxResultaatButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.uxResultaatButton, "uxResultaatButton");
      this.uxResultaatButton.Name = "uxResultaatButton";
      this.uxResultaatButton.Click += new System.EventHandler(this.TscMapClick);
      //
      // uxKaartlagenButton
      //
      this.uxKaartlagenButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.uxKaartlagenButton, "uxKaartlagenButton");
      this.uxKaartlagenButton.Name = "uxKaartlagenButton";
      this.uxKaartlagenButton.Click += new System.EventHandler(this.TscMapClick);
      //
      // uxPrintButton
      //
      this.uxPrintButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.uxPrintButton, "uxPrintButton");
      this.uxPrintButton.Name = "uxPrintButton";
      //
      // uxSeparator2
      //
      this.uxSeparator2.Name = "uxSeparator2";
      resources.ApplyResources(this.uxSeparator2, "uxSeparator2");
      //
      // cmbWeergave
      //
      this.cmbWeergave.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbWeergave.Items.AddRange(new object[] {
            resources.GetString("cmbWeergave.Items"),
            resources.GetString("cmbWeergave.Items1")});
      this.cmbWeergave.Name = "cmbWeergave";
      resources.ApplyResources(this.cmbWeergave, "cmbWeergave");
      //
      // uxCoordinatesLabel
      //
      this.uxCoordinatesLabel.Name = "uxCoordinatesLabel";
      resources.ApplyResources(this.uxCoordinatesLabel, "uxCoordinatesLabel");
      //
      // toolTipMap
      //
      this.toolTipMap.IsBalloon = true;
      this.toolTipMap.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
      //
      // MapControl
      //
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tscMap);
      this.Name = "MapControl";
      this.tscMap.ContentPanel.ResumeLayout(false);
      this.tscMap.ContentPanel.PerformLayout();
      this.tscMap.TopToolStripPanel.ResumeLayout(false);
      this.tscMap.TopToolStripPanel.PerformLayout();
      this.tscMap.ResumeLayout(false);
      this.tscMap.PerformLayout();
      this.mapTipsPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.Canvas)).EndInit();
      this.uxTopoToolStrip.ResumeLayout(false);
      this.uxTopoToolStrip.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ToolStripContainer tscMap;
    private System.Windows.Forms.ToolStrip uxTopoToolStrip;
    private System.Windows.Forms.ToolStripButton uxSelectButton;
    private System.Windows.Forms.ToolStripButton uxZoomInButton;
    private System.Windows.Forms.ToolStripButton uxZoomOutButton;
    private System.Windows.Forms.ToolStripButton uzZoomAreaButton;
    private System.Windows.Forms.ToolStripButton uxDragButton;
    private System.Windows.Forms.ToolStripButton uxCenterButton;
    private System.Windows.Forms.ToolStripButton uxWorldButton;
    private System.Windows.Forms.ToolStripSeparator uxSeparator1;
    private System.Windows.Forms.ToolStripButton uxKaartlagenButton;
    private System.Windows.Forms.ToolStripSeparator uxSeparator2;
    private System.Windows.Forms.ToolStripLabel uxCoordinatesLabel;
    /// <summary>
    ///
    /// </summary>
    public AxMapObjectsLT2.AxMap Canvas;
    private System.Windows.Forms.ToolStripButton uxPrintButton;
    private System.Windows.Forms.ToolTip toolTipMap;
    private System.Windows.Forms.Panel mapTipsPanel;
    private System.Windows.Forms.Label mapTipsLabel;
    private System.Windows.Forms.Timer mapTipsTimer;
    private System.Windows.Forms.ToolStripComboBox cmbWeergave;
    private System.Windows.Forms.ToolStripButton uxResultaatButton;

  }
}
