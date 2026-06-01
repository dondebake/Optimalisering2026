namespace OptimaliseRing.Batch
{
   partial class BatchForm
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatchForm));
        this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
        this.statusStripMain = new System.Windows.Forms.StatusStrip();
        this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
        this.groupBoxBatch = new System.Windows.Forms.GroupBox();
        this.txtSomType = new System.Windows.Forms.TextBox();
        this.lblSomType = new System.Windows.Forms.Label();
        this.txtSerieFolder = new System.Windows.Forms.TextBox();
        this.lblSerieFolder = new System.Windows.Forms.Label();
        this.splitContainerBatch = new System.Windows.Forms.SplitContainer();
        this.groupBoxInstellingen = new System.Windows.Forms.GroupBox();
        this.dgvInstellingen = new System.Windows.Forms.DataGridView();
        this.groupBoxDijkringdelen = new System.Windows.Forms.GroupBox();
        this.dgvBerekeningen = new System.Windows.Forms.DataGridView();
        this.menuStripMain = new System.Windows.Forms.MenuStrip();
        this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        this.menuBestandWerkmap = new System.Windows.Forms.ToolStripMenuItem();
        this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
        this.menuBestandExit = new System.Windows.Forms.ToolStripMenuItem();
        this.selecterenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.instellingenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
        this.nothingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.dijkringdelenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.menuRekenen = new System.Windows.Forms.ToolStripMenuItem();
        this.menuRekenenStart = new System.Windows.Forms.ToolStripMenuItem();
        this.menuRekenencompartimentering = new System.Windows.Forms.ToolStripMenuItem();
        this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
        this.menuHelpOver = new System.Windows.Forms.ToolStripMenuItem();
        this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
        this.toolStripContainer1.ContentPanel.SuspendLayout();
        this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
        this.toolStripContainer1.SuspendLayout();
        this.statusStripMain.SuspendLayout();
        this.groupBoxBatch.SuspendLayout();
        this.splitContainerBatch.Panel1.SuspendLayout();
        this.splitContainerBatch.Panel2.SuspendLayout();
        this.splitContainerBatch.SuspendLayout();
        this.groupBoxInstellingen.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.dgvInstellingen)).BeginInit();
        this.groupBoxDijkringdelen.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.dgvBerekeningen)).BeginInit();
        this.menuStripMain.SuspendLayout();
        this.SuspendLayout();
        //
        // toolStripContainer1
        //
        //
        // toolStripContainer1.BottomToolStripPanel
        //
        this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStripMain);
        //
        // toolStripContainer1.ContentPanel
        //
        this.toolStripContainer1.ContentPanel.Controls.Add(this.groupBoxBatch);
        resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
        resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
        this.toolStripContainer1.Name = "toolStripContainer1";
        //
        // toolStripContainer1.TopToolStripPanel
        //
        this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStripMain);
        //
        // statusStripMain
        //
        resources.ApplyResources(this.statusStripMain, "statusStripMain");
        this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
        this.statusStripMain.Name = "statusStripMain";
        this.statusStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
        //
        // StatusLabel
        //
        this.StatusLabel.Name = "StatusLabel";
        resources.ApplyResources(this.StatusLabel, "StatusLabel");
        this.StatusLabel.Spring = true;
        //
        // groupBoxBatch
        //
        this.groupBoxBatch.Controls.Add(this.txtSomType);
        this.groupBoxBatch.Controls.Add(this.lblSomType);
        this.groupBoxBatch.Controls.Add(this.txtSerieFolder);
        this.groupBoxBatch.Controls.Add(this.lblSerieFolder);
        this.groupBoxBatch.Controls.Add(this.splitContainerBatch);
        resources.ApplyResources(this.groupBoxBatch, "groupBoxBatch");
        this.groupBoxBatch.Name = "groupBoxBatch";
        this.groupBoxBatch.TabStop = false;
        //
        // txtSomType
        //
        resources.ApplyResources(this.txtSomType, "txtSomType");
        this.txtSomType.Name = "txtSomType";
        this.txtSomType.TextChanged += new System.EventHandler(this.OnTxtSomTypeTextChanged);
        //
        // lblSomType
        //
        resources.ApplyResources(this.lblSomType, "lblSomType");
        this.lblSomType.Name = "lblSomType";
        //
        // txtSerieFolder
        //
        resources.ApplyResources(this.txtSerieFolder, "txtSerieFolder");
        this.txtSerieFolder.Name = "txtSerieFolder";
        this.txtSerieFolder.TextChanged += new System.EventHandler(this.OnTxtSerieFolderTextChanged);
        //
        // lblSerieFolder
        //
        resources.ApplyResources(this.lblSerieFolder, "lblSerieFolder");
        this.lblSerieFolder.Name = "lblSerieFolder";
        //
        // splitContainerBatch
        //
        resources.ApplyResources(this.splitContainerBatch, "splitContainerBatch");
        this.splitContainerBatch.Name = "splitContainerBatch";
        //
        // splitContainerBatch.Panel1
        //
        this.splitContainerBatch.Panel1.Controls.Add(this.groupBoxInstellingen);
        //
        // splitContainerBatch.Panel2
        //
        this.splitContainerBatch.Panel2.Controls.Add(this.groupBoxDijkringdelen);
        //
        // groupBoxInstellingen
        //
        this.groupBoxInstellingen.Controls.Add(this.dgvInstellingen);
        resources.ApplyResources(this.groupBoxInstellingen, "groupBoxInstellingen");
        this.groupBoxInstellingen.Name = "groupBoxInstellingen";
        this.groupBoxInstellingen.TabStop = false;
        //
        // dgvInstellingen
        //
        this.dgvInstellingen.AllowUserToAddRows = false;
        this.dgvInstellingen.AllowUserToDeleteRows = false;
        this.dgvInstellingen.AllowUserToResizeColumns = false;
        this.dgvInstellingen.AllowUserToResizeRows = false;
        this.dgvInstellingen.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        resources.ApplyResources(this.dgvInstellingen, "dgvInstellingen");
        this.dgvInstellingen.Name = "dgvInstellingen";
        this.dgvInstellingen.RowHeadersVisible = false;
        this.dgvInstellingen.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
        this.dgvInstellingen.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnDgvInstellingenCellValueChanged);
        this.dgvInstellingen.CurrentCellDirtyStateChanged += new System.EventHandler(this.OnDgvInstellingenCurrentCellDirtyStateChanged);
        //
        // groupBoxDijkringdelen
        //
        this.groupBoxDijkringdelen.Controls.Add(this.dgvBerekeningen);
        resources.ApplyResources(this.groupBoxDijkringdelen, "groupBoxDijkringdelen");
        this.groupBoxDijkringdelen.Name = "groupBoxDijkringdelen";
        this.groupBoxDijkringdelen.TabStop = false;
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
        this.dgvBerekeningen.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnDgvDijkringenCellValueChanged);
        this.dgvBerekeningen.CurrentCellDirtyStateChanged += new System.EventHandler(this.OnDgvBerekeningenCurrentCellDirtyStateChanged);
        //
        // menuStripMain
        //
        resources.ApplyResources(this.menuStripMain, "menuStripMain");
        this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem1,
            this.selecterenToolStripMenuItem,
            this.menuRekenen,
            this.menuHelp});
        this.menuStripMain.Name = "menuStripMain";
        //
        // fileToolStripMenuItem1
        //
        this.fileToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuBestandWerkmap,
            this.toolStripSeparator2,
            this.menuBestandExit});
        this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
        resources.ApplyResources(this.fileToolStripMenuItem1, "fileToolStripMenuItem1");
        //
        // menuBestandWerkmap
        //
        resources.ApplyResources(this.menuBestandWerkmap, "menuBestandWerkmap");
        this.menuBestandWerkmap.Name = "menuBestandWerkmap";
        this.menuBestandWerkmap.Click += new System.EventHandler(this.OnMenuBestandWerkmapClick);
        //
        // toolStripSeparator2
        //
        this.toolStripSeparator2.Name = "toolStripSeparator2";
        resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
        //
        // menuBestandExit
        //
        this.menuBestandExit.Name = "menuBestandExit";
        resources.ApplyResources(this.menuBestandExit, "menuBestandExit");
        this.menuBestandExit.Click += new System.EventHandler(this.OnMenuBestandExitClick);
        //
        // selecterenToolStripMenuItem
        //
        this.selecterenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.instellingenToolStripMenuItem,
            this.dijkringdelenToolStripMenuItem});
        this.selecterenToolStripMenuItem.Name = "selecterenToolStripMenuItem";
        resources.ApplyResources(this.selecterenToolStripMenuItem, "selecterenToolStripMenuItem");
        //
        // instellingenToolStripMenuItem
        //
        this.instellingenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allToolStripMenuItem,
            this.toolStripMenuItem1,
            this.nothingToolStripMenuItem});
        this.instellingenToolStripMenuItem.Name = "instellingenToolStripMenuItem";
        resources.ApplyResources(this.instellingenToolStripMenuItem, "instellingenToolStripMenuItem");
        //
        // allToolStripMenuItem
        //
        this.allToolStripMenuItem.Name = "allToolStripMenuItem";
        resources.ApplyResources(this.allToolStripMenuItem, "allToolStripMenuItem");
        this.allToolStripMenuItem.Click += new System.EventHandler(this.OnAllToolStripMenuItemClick);
        //
        // toolStripMenuItem1
        //
        this.toolStripMenuItem1.Name = "toolStripMenuItem1";
        resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
        //
        // nothingToolStripMenuItem
        //
        this.nothingToolStripMenuItem.Name = "nothingToolStripMenuItem";
        resources.ApplyResources(this.nothingToolStripMenuItem, "nothingToolStripMenuItem");
        this.nothingToolStripMenuItem.Click += new System.EventHandler(this.OnNothingToolStripMenuItemClick);
        //
        // dijkringdelenToolStripMenuItem
        //
        this.dijkringdelenToolStripMenuItem.Name = "dijkringdelenToolStripMenuItem";
        resources.ApplyResources(this.dijkringdelenToolStripMenuItem, "dijkringdelenToolStripMenuItem");
        //
        // menuRekenen
        //
        this.menuRekenen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuRekenenStart,
            this.menuRekenencompartimentering});
        this.menuRekenen.Name = "menuRekenen";
        resources.ApplyResources(this.menuRekenen, "menuRekenen");
        //
        // menuRekenenStart
        //
        this.menuRekenenStart.Name = "menuRekenenStart";
        resources.ApplyResources(this.menuRekenenStart, "menuRekenenStart");
        this.menuRekenenStart.Click += new System.EventHandler(this.OnMenuRekenenStartClick);
        //
        // menuRekenencompartimentering
        //
        this.menuRekenencompartimentering.Name = "menuRekenencompartimentering";
        resources.ApplyResources(this.menuRekenencompartimentering, "menuRekenencompartimentering");
        this.menuRekenencompartimentering.Click += new System.EventHandler(this.OnMenuRekenencompartimenteringClick);
        //
        // menuHelp
        //
        this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelpOver});
        this.menuHelp.Name = "menuHelp";
        resources.ApplyResources(this.menuHelp, "menuHelp");
        //
        // menuHelpOver
        //
        this.menuHelpOver.Name = "menuHelpOver";
        resources.ApplyResources(this.menuHelpOver, "menuHelpOver");
        this.menuHelpOver.Click += new System.EventHandler(this.OnMenuHelpOverClick);
        //
        // BatchForm
        //
        resources.ApplyResources(this, "$this");
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this.toolStripContainer1);
        this.Name = "BatchForm";
        this.Load += new System.EventHandler(this.OnBatchFormLoad);
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnBatchFormClosing);
        this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
        this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
        this.toolStripContainer1.ContentPanel.ResumeLayout(false);
        this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
        this.toolStripContainer1.TopToolStripPanel.PerformLayout();
        this.toolStripContainer1.ResumeLayout(false);
        this.toolStripContainer1.PerformLayout();
        this.statusStripMain.ResumeLayout(false);
        this.statusStripMain.PerformLayout();
        this.groupBoxBatch.ResumeLayout(false);
        this.groupBoxBatch.PerformLayout();
        this.splitContainerBatch.Panel1.ResumeLayout(false);
        this.splitContainerBatch.Panel2.ResumeLayout(false);
        this.splitContainerBatch.ResumeLayout(false);
        this.groupBoxInstellingen.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.dgvInstellingen)).EndInit();
        this.groupBoxDijkringdelen.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.dgvBerekeningen)).EndInit();
        this.menuStripMain.ResumeLayout(false);
        this.menuStripMain.PerformLayout();
        this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.MenuStrip menuStripMain;
      private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem menuBestandWerkmap;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
      private System.Windows.Forms.ToolStripMenuItem menuBestandExit;
      private System.Windows.Forms.ToolStripMenuItem menuHelp;
      private System.Windows.Forms.ToolStripMenuItem menuHelpOver;
      private System.Windows.Forms.ToolStripContainer toolStripContainer1;
      private System.Windows.Forms.StatusStrip statusStripMain;
      private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
      private System.Windows.Forms.ToolStripMenuItem selecterenToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem menuRekenen;
      private System.Windows.Forms.ToolStripMenuItem menuRekenenStart;
      private System.Windows.Forms.GroupBox groupBoxBatch;
      private System.Windows.Forms.SplitContainer splitContainerBatch;
      private System.Windows.Forms.ToolStripMenuItem instellingenToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem dijkringdelenToolStripMenuItem;
      private System.Windows.Forms.GroupBox groupBoxInstellingen;
      private System.Windows.Forms.DataGridView dgvInstellingen;
      private System.Windows.Forms.GroupBox groupBoxDijkringdelen;
      private System.Windows.Forms.DataGridView dgvBerekeningen;
      private System.Windows.Forms.ToolStripMenuItem menuRekenencompartimentering;
      private System.Windows.Forms.TextBox txtSomType;
      private System.Windows.Forms.Label lblSomType;
      private System.Windows.Forms.TextBox txtSerieFolder;
      private System.Windows.Forms.Label lblSerieFolder;
      private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem nothingToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
   }
}
