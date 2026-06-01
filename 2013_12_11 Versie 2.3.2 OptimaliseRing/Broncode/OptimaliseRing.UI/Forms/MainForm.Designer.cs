namespace OptimaliseRing.UI
{
  partial class MainForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.menuStripMain = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.menuBestandWerkmap = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.menuBestandExit = new System.Windows.Forms.ToolStripMenuItem();
      this.bewerkenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.menuBewerkenSelectie = new System.Windows.Forms.ToolStripMenuItem();
      this.menuBewerkenZoomIn = new System.Windows.Forms.ToolStripMenuItem();
      this.menuBewerkenZoomUit = new System.Windows.Forms.ToolStripMenuItem();
      this.menuBewerkenZoomGebied = new System.Windows.Forms.ToolStripMenuItem();
      this.menuBewerkenSchuif = new System.Windows.Forms.ToolStripMenuItem();
      this.menuBewerkenCentreer = new System.Windows.Forms.ToolStripMenuItem();
      this.menuBewerkenWereld = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
      this.menuBewerkenKaartlagen = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuBewerkenLegenda = new System.Windows.Forms.ToolStripMenuItem();
      this.menuBewerkenMapTips = new System.Windows.Forms.ToolStripMenuItem();
      this.menuRekenen = new System.Windows.Forms.ToolStripMenuItem();
      this.menuRekenenBerekeningen = new System.Windows.Forms.ToolStripMenuItem();
      this.menuRekenenResultaten = new System.Windows.Forms.ToolStripMenuItem();
      this.menuRekenencompartimentering = new System.Windows.Forms.ToolStripMenuItem();
      this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
      this.menuHelpHandleiding = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      this.menuHelpOver = new System.Windows.Forms.ToolStripMenuItem();
      this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
      this.statusStripMain = new System.Windows.Forms.StatusStrip();
      this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
      this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
      this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
      this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
      this.mapControlMain = new OptimaliseRing.UI.MapControl();
      this.menuStripMain.SuspendLayout();
      this.statusStripMain.SuspendLayout();
      this.SuspendLayout();
      //
      // menuStripMain
      //
      this.menuStripMain.AccessibleDescription = null;
      this.menuStripMain.AccessibleName = null;
      resources.ApplyResources(this.menuStripMain, "menuStripMain");
      this.menuStripMain.BackgroundImage = null;
      this.menuStripMain.Font = null;
      this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem1,
            this.bewerkenToolStripMenuItem,
            this.menuRekenen,
            this.menuHelp});
      this.menuStripMain.Name = "menuStripMain";
      //
      // fileToolStripMenuItem1
      //
      this.fileToolStripMenuItem1.AccessibleDescription = null;
      this.fileToolStripMenuItem1.AccessibleName = null;
      resources.ApplyResources(this.fileToolStripMenuItem1, "fileToolStripMenuItem1");
      this.fileToolStripMenuItem1.BackgroundImage = null;
      this.fileToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuBestandWerkmap,
            this.toolStripSeparator2,
            this.menuBestandExit});
      this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
      this.fileToolStripMenuItem1.ShortcutKeyDisplayString = null;
      //
      // menuBestandWerkmap
      //
      this.menuBestandWerkmap.AccessibleDescription = null;
      this.menuBestandWerkmap.AccessibleName = null;
      resources.ApplyResources(this.menuBestandWerkmap, "menuBestandWerkmap");
      this.menuBestandWerkmap.BackgroundImage = null;
      this.menuBestandWerkmap.Name = "menuBestandWerkmap";
      this.menuBestandWerkmap.ShortcutKeyDisplayString = null;
      this.menuBestandWerkmap.Click += new System.EventHandler(this.OnMenuBestandWerkmapClick);
      //
      // toolStripSeparator2
      //
      this.toolStripSeparator2.AccessibleDescription = null;
      this.toolStripSeparator2.AccessibleName = null;
      resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      //
      // menuBestandExit
      //
      this.menuBestandExit.AccessibleDescription = null;
      this.menuBestandExit.AccessibleName = null;
      resources.ApplyResources(this.menuBestandExit, "menuBestandExit");
      this.menuBestandExit.BackgroundImage = null;
      this.menuBestandExit.Name = "menuBestandExit";
      this.menuBestandExit.ShortcutKeyDisplayString = null;
      this.menuBestandExit.Click += new System.EventHandler(this.OnMenuBestandExitClick);
      //
      // bewerkenToolStripMenuItem
      //
      this.bewerkenToolStripMenuItem.AccessibleDescription = null;
      this.bewerkenToolStripMenuItem.AccessibleName = null;
      resources.ApplyResources(this.bewerkenToolStripMenuItem, "bewerkenToolStripMenuItem");
      this.bewerkenToolStripMenuItem.BackgroundImage = null;
      this.bewerkenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuBewerkenSelectie,
            this.menuBewerkenZoomIn,
            this.menuBewerkenZoomUit,
            this.menuBewerkenZoomGebied,
            this.menuBewerkenSchuif,
            this.menuBewerkenCentreer,
            this.menuBewerkenWereld,
            this.toolStripMenuItem2,
            this.menuBewerkenKaartlagen,
            this.mnuBewerkenLegenda,
            this.menuBewerkenMapTips});
      this.bewerkenToolStripMenuItem.Name = "bewerkenToolStripMenuItem";
      this.bewerkenToolStripMenuItem.ShortcutKeyDisplayString = null;
      //
      // menuBewerkenSelectie
      //
      this.menuBewerkenSelectie.AccessibleDescription = null;
      this.menuBewerkenSelectie.AccessibleName = null;
      resources.ApplyResources(this.menuBewerkenSelectie, "menuBewerkenSelectie");
      this.menuBewerkenSelectie.BackgroundImage = null;
      this.menuBewerkenSelectie.Checked = true;
      this.menuBewerkenSelectie.CheckState = System.Windows.Forms.CheckState.Checked;
      this.menuBewerkenSelectie.Name = "menuBewerkenSelectie";
      this.menuBewerkenSelectie.ShortcutKeyDisplayString = null;
      this.menuBewerkenSelectie.Click += new System.EventHandler(this.MenuBewerkenClick);
      //
      // menuBewerkenZoomIn
      //
      this.menuBewerkenZoomIn.AccessibleDescription = null;
      this.menuBewerkenZoomIn.AccessibleName = null;
      resources.ApplyResources(this.menuBewerkenZoomIn, "menuBewerkenZoomIn");
      this.menuBewerkenZoomIn.BackgroundImage = null;
      this.menuBewerkenZoomIn.Name = "menuBewerkenZoomIn";
      this.menuBewerkenZoomIn.ShortcutKeyDisplayString = null;
      this.menuBewerkenZoomIn.Click += new System.EventHandler(this.MenuBewerkenClick);
      //
      // menuBewerkenZoomUit
      //
      this.menuBewerkenZoomUit.AccessibleDescription = null;
      this.menuBewerkenZoomUit.AccessibleName = null;
      resources.ApplyResources(this.menuBewerkenZoomUit, "menuBewerkenZoomUit");
      this.menuBewerkenZoomUit.BackgroundImage = null;
      this.menuBewerkenZoomUit.Name = "menuBewerkenZoomUit";
      this.menuBewerkenZoomUit.ShortcutKeyDisplayString = null;
      this.menuBewerkenZoomUit.Click += new System.EventHandler(this.MenuBewerkenClick);
      //
      // menuBewerkenZoomGebied
      //
      this.menuBewerkenZoomGebied.AccessibleDescription = null;
      this.menuBewerkenZoomGebied.AccessibleName = null;
      resources.ApplyResources(this.menuBewerkenZoomGebied, "menuBewerkenZoomGebied");
      this.menuBewerkenZoomGebied.BackgroundImage = null;
      this.menuBewerkenZoomGebied.Name = "menuBewerkenZoomGebied";
      this.menuBewerkenZoomGebied.ShortcutKeyDisplayString = null;
      this.menuBewerkenZoomGebied.Click += new System.EventHandler(this.MenuBewerkenClick);
      //
      // menuBewerkenSchuif
      //
      this.menuBewerkenSchuif.AccessibleDescription = null;
      this.menuBewerkenSchuif.AccessibleName = null;
      resources.ApplyResources(this.menuBewerkenSchuif, "menuBewerkenSchuif");
      this.menuBewerkenSchuif.BackgroundImage = null;
      this.menuBewerkenSchuif.Name = "menuBewerkenSchuif";
      this.menuBewerkenSchuif.ShortcutKeyDisplayString = null;
      this.menuBewerkenSchuif.Click += new System.EventHandler(this.MenuBewerkenClick);
      //
      // menuBewerkenCentreer
      //
      this.menuBewerkenCentreer.AccessibleDescription = null;
      this.menuBewerkenCentreer.AccessibleName = null;
      resources.ApplyResources(this.menuBewerkenCentreer, "menuBewerkenCentreer");
      this.menuBewerkenCentreer.BackgroundImage = null;
      this.menuBewerkenCentreer.Name = "menuBewerkenCentreer";
      this.menuBewerkenCentreer.ShortcutKeyDisplayString = null;
      this.menuBewerkenCentreer.Click += new System.EventHandler(this.MenuBewerkenClick);
      //
      // menuBewerkenWereld
      //
      this.menuBewerkenWereld.AccessibleDescription = null;
      this.menuBewerkenWereld.AccessibleName = null;
      resources.ApplyResources(this.menuBewerkenWereld, "menuBewerkenWereld");
      this.menuBewerkenWereld.BackgroundImage = null;
      this.menuBewerkenWereld.Name = "menuBewerkenWereld";
      this.menuBewerkenWereld.ShortcutKeyDisplayString = null;
      this.menuBewerkenWereld.Click += new System.EventHandler(this.MenuBewerkenClick);
      //
      // toolStripMenuItem2
      //
      this.toolStripMenuItem2.AccessibleDescription = null;
      this.toolStripMenuItem2.AccessibleName = null;
      resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      //
      // menuBewerkenKaartlagen
      //
      this.menuBewerkenKaartlagen.AccessibleDescription = null;
      this.menuBewerkenKaartlagen.AccessibleName = null;
      resources.ApplyResources(this.menuBewerkenKaartlagen, "menuBewerkenKaartlagen");
      this.menuBewerkenKaartlagen.BackgroundImage = null;
      this.menuBewerkenKaartlagen.Name = "menuBewerkenKaartlagen";
      this.menuBewerkenKaartlagen.ShortcutKeyDisplayString = null;
      this.menuBewerkenKaartlagen.Click += new System.EventHandler(this.MenuBewerkenClick);
      //
      // mnuBewerkenLegenda
      //
      this.mnuBewerkenLegenda.AccessibleDescription = null;
      this.mnuBewerkenLegenda.AccessibleName = null;
      resources.ApplyResources(this.mnuBewerkenLegenda, "mnuBewerkenLegenda");
      this.mnuBewerkenLegenda.BackgroundImage = null;
      this.mnuBewerkenLegenda.Checked = true;
      this.mnuBewerkenLegenda.CheckOnClick = true;
      this.mnuBewerkenLegenda.CheckState = System.Windows.Forms.CheckState.Checked;
      this.mnuBewerkenLegenda.Name = "mnuBewerkenLegenda";
      this.mnuBewerkenLegenda.ShortcutKeyDisplayString = null;
      this.mnuBewerkenLegenda.Click += new System.EventHandler(this.MenuBewerkenClick);
      //
      // menuBewerkenMapTips
      //
      this.menuBewerkenMapTips.AccessibleDescription = null;
      this.menuBewerkenMapTips.AccessibleName = null;
      resources.ApplyResources(this.menuBewerkenMapTips, "menuBewerkenMapTips");
      this.menuBewerkenMapTips.BackgroundImage = null;
      this.menuBewerkenMapTips.Checked = true;
      this.menuBewerkenMapTips.CheckOnClick = true;
      this.menuBewerkenMapTips.CheckState = System.Windows.Forms.CheckState.Checked;
      this.menuBewerkenMapTips.Name = "menuBewerkenMapTips";
      this.menuBewerkenMapTips.ShortcutKeyDisplayString = null;
      this.menuBewerkenMapTips.Click += new System.EventHandler(this.MenuBewerkenClick);
      //
      // menuRekenen
      //
      this.menuRekenen.AccessibleDescription = null;
      this.menuRekenen.AccessibleName = null;
      resources.ApplyResources(this.menuRekenen, "menuRekenen");
      this.menuRekenen.BackgroundImage = null;
      this.menuRekenen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuRekenenBerekeningen,
            this.menuRekenenResultaten,
            this.menuRekenencompartimentering});
      this.menuRekenen.Name = "menuRekenen";
      this.menuRekenen.ShortcutKeyDisplayString = null;
      //
      // menuRekenenBerekeningen
      //
      this.menuRekenenBerekeningen.AccessibleDescription = null;
      this.menuRekenenBerekeningen.AccessibleName = null;
      resources.ApplyResources(this.menuRekenenBerekeningen, "menuRekenenBerekeningen");
      this.menuRekenenBerekeningen.BackgroundImage = null;
      this.menuRekenenBerekeningen.Name = "menuRekenenBerekeningen";
      this.menuRekenenBerekeningen.ShortcutKeyDisplayString = null;
      this.menuRekenenBerekeningen.Click += new System.EventHandler(this.OnMenuRekenenBerekeningenClick);
      //
      // menuRekenenResultaten
      //
      this.menuRekenenResultaten.AccessibleDescription = null;
      this.menuRekenenResultaten.AccessibleName = null;
      resources.ApplyResources(this.menuRekenenResultaten, "menuRekenenResultaten");
      this.menuRekenenResultaten.BackgroundImage = null;
      this.menuRekenenResultaten.Name = "menuRekenenResultaten";
      this.menuRekenenResultaten.ShortcutKeyDisplayString = null;
      this.menuRekenenResultaten.Click += new System.EventHandler(this.OnMenuRekenenResultatenClick);
      //
      // menuRekenencompartimentering
      //
      this.menuRekenencompartimentering.AccessibleDescription = null;
      this.menuRekenencompartimentering.AccessibleName = null;
      resources.ApplyResources(this.menuRekenencompartimentering, "menuRekenencompartimentering");
      this.menuRekenencompartimentering.BackgroundImage = null;
      this.menuRekenencompartimentering.Name = "menuRekenencompartimentering";
      this.menuRekenencompartimentering.ShortcutKeyDisplayString = null;
      this.menuRekenencompartimentering.Click += new System.EventHandler(this.OnMenuRekenencompartimenteringClick);
      //
      // menuHelp
      //
      this.menuHelp.AccessibleDescription = null;
      this.menuHelp.AccessibleName = null;
      resources.ApplyResources(this.menuHelp, "menuHelp");
      this.menuHelp.BackgroundImage = null;
      this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelpHandleiding,
            this.toolStripMenuItem1,
            this.menuHelpOver});
      this.menuHelp.Name = "menuHelp";
      this.menuHelp.ShortcutKeyDisplayString = null;
      //
      // menuHelpHandleiding
      //
      this.menuHelpHandleiding.AccessibleDescription = null;
      this.menuHelpHandleiding.AccessibleName = null;
      resources.ApplyResources(this.menuHelpHandleiding, "menuHelpHandleiding");
      this.menuHelpHandleiding.BackgroundImage = null;
      this.menuHelpHandleiding.Name = "menuHelpHandleiding";
      this.menuHelpHandleiding.ShortcutKeyDisplayString = null;
      this.menuHelpHandleiding.Click += new System.EventHandler(this.OnMnuHelpHandleidingClick);
      //
      // toolStripMenuItem1
      //
      this.toolStripMenuItem1.AccessibleDescription = null;
      this.toolStripMenuItem1.AccessibleName = null;
      resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      //
      // menuHelpOver
      //
      this.menuHelpOver.AccessibleDescription = null;
      this.menuHelpOver.AccessibleName = null;
      resources.ApplyResources(this.menuHelpOver, "menuHelpOver");
      this.menuHelpOver.BackgroundImage = null;
      this.menuHelpOver.Name = "menuHelpOver";
      this.menuHelpOver.ShortcutKeyDisplayString = null;
      this.menuHelpOver.Click += new System.EventHandler(this.OnMnuHelpOverClick);
      //
      // BottomToolStripPanel
      //
      this.BottomToolStripPanel.AccessibleDescription = null;
      this.BottomToolStripPanel.AccessibleName = null;
      resources.ApplyResources(this.BottomToolStripPanel, "BottomToolStripPanel");
      this.BottomToolStripPanel.BackgroundImage = null;
      this.BottomToolStripPanel.Font = null;
      this.BottomToolStripPanel.Name = "BottomToolStripPanel";
      this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
      //
      // statusStripMain
      //
      this.statusStripMain.AccessibleDescription = null;
      this.statusStripMain.AccessibleName = null;
      resources.ApplyResources(this.statusStripMain, "statusStripMain");
      this.statusStripMain.BackgroundImage = null;
      this.statusStripMain.Font = null;
      this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
      this.statusStripMain.Name = "statusStripMain";
      this.statusStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
      //
      // statusLabel
      //
      this.statusLabel.AccessibleDescription = null;
      this.statusLabel.AccessibleName = null;
      resources.ApplyResources(this.statusLabel, "statusLabel");
      this.statusLabel.BackgroundImage = null;
      this.statusLabel.Name = "statusLabel";
      this.statusLabel.Spring = true;
      //
      // TopToolStripPanel
      //
      this.TopToolStripPanel.AccessibleDescription = null;
      this.TopToolStripPanel.AccessibleName = null;
      resources.ApplyResources(this.TopToolStripPanel, "TopToolStripPanel");
      this.TopToolStripPanel.BackgroundImage = null;
      this.TopToolStripPanel.Font = null;
      this.TopToolStripPanel.Name = "TopToolStripPanel";
      this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
      //
      // RightToolStripPanel
      //
      this.RightToolStripPanel.AccessibleDescription = null;
      this.RightToolStripPanel.AccessibleName = null;
      resources.ApplyResources(this.RightToolStripPanel, "RightToolStripPanel");
      this.RightToolStripPanel.BackgroundImage = null;
      this.RightToolStripPanel.Font = null;
      this.RightToolStripPanel.Name = "RightToolStripPanel";
      this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
      //
      // LeftToolStripPanel
      //
      this.LeftToolStripPanel.AccessibleDescription = null;
      this.LeftToolStripPanel.AccessibleName = null;
      resources.ApplyResources(this.LeftToolStripPanel, "LeftToolStripPanel");
      this.LeftToolStripPanel.BackgroundImage = null;
      this.LeftToolStripPanel.Font = null;
      this.LeftToolStripPanel.Name = "LeftToolStripPanel";
      this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
      //
      // ContentPanel
      //
      this.ContentPanel.AccessibleDescription = null;
      this.ContentPanel.AccessibleName = null;
      resources.ApplyResources(this.ContentPanel, "ContentPanel");
      this.ContentPanel.BackgroundImage = null;
      this.ContentPanel.Font = null;
      //
      // mapControlMain
      //
      this.mapControlMain.AccessibleDescription = null;
      this.mapControlMain.AccessibleName = null;
      resources.ApplyResources(this.mapControlMain, "mapControlMain");
      this.mapControlMain.BackgroundImage = null;
      this.mapControlMain.Berekening = null;
      this.mapControlMain.CurrentMode = OptimaliseRing.Core.PointerMode.Select;
      this.mapControlMain.Font = null;
      this.mapControlMain.KaartlagenMap = null;
      this.mapControlMain.Name = "mapControlMain";
      this.mapControlMain.Profile = null;
      //
      // MainForm
      //
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.Controls.Add(this.mapControlMain);
      this.Controls.Add(this.statusStripMain);
      this.Controls.Add(this.menuStripMain);
      this.Font = null;
      this.Name = "MainForm";
      this.Load += new System.EventHandler(this.OnMainFormLoad);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnMainFormFormClosing);
      this.menuStripMain.ResumeLayout(false);
      this.menuStripMain.PerformLayout();
      this.statusStripMain.ResumeLayout(false);
      this.statusStripMain.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStripMain;
    private System.Windows.Forms.ToolStripMenuItem bewerkenToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem menuBewerkenKaartlagen;
    private System.Windows.Forms.ToolStripMenuItem menuRekenen;
    private System.Windows.Forms.ToolStripMenuItem menuRekenenBerekeningen;
    private System.Windows.Forms.ToolStripMenuItem menuRekenenResultaten;
    private System.Windows.Forms.ToolStripMenuItem menuHelp;
    private System.Windows.Forms.ToolStripMenuItem menuHelpHandleiding;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem menuHelpOver;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem menuBestandWerkmap;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripMenuItem menuBestandExit;
    private System.Windows.Forms.StatusStrip statusStripMain;
    private System.Windows.Forms.ToolStripMenuItem mnuBewerkenLegenda;
    private System.Windows.Forms.ToolStripStatusLabel statusLabel;
    private System.Windows.Forms.ToolStripMenuItem menuBewerkenSelectie;
    private System.Windows.Forms.ToolStripMenuItem menuBewerkenZoomIn;
    private System.Windows.Forms.ToolStripMenuItem menuBewerkenZoomUit;
    private System.Windows.Forms.ToolStripMenuItem menuBewerkenZoomGebied;
    private System.Windows.Forms.ToolStripMenuItem menuBewerkenSchuif;
    private System.Windows.Forms.ToolStripMenuItem menuBewerkenCentreer;
    private System.Windows.Forms.ToolStripMenuItem menuBewerkenWereld;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
    private System.Windows.Forms.ToolStripMenuItem menuBewerkenMapTips;
    private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
    private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
    private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
    private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
    private System.Windows.Forms.ToolStripContentPanel ContentPanel;
    /// <summary>
    /// map
    /// </summary>
    public MapControl mapControlMain;
    private System.Windows.Forms.ToolStripMenuItem menuRekenencompartimentering;
  }
}
