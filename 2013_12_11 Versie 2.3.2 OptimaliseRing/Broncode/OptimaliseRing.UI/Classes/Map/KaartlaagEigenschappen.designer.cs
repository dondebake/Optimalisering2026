namespace OptimaliseRing.UI
{
   partial class KaartlaagEigenschappenForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KaartlaagEigenschappenForm));
         this.uxGroupBoxEigenschappen = new System.Windows.Forms.GroupBox();
         this.lblLijnstijl = new System.Windows.Forms.Label();
         this.cboLijnstijl = new System.Windows.Forms.ComboBox();
         this.cboLijndikte = new System.Windows.Forms.ComboBox();
         this.lblLijndikte = new System.Windows.Forms.Label();
         this.mapControl = new OptimaliseRing.UI.MapControl();
         this.cboVulstijl = new System.Windows.Forms.ComboBox();
         this.cboKleur = new OptimaliseRing.Controls.ColorPicker.ComboBoxColorPicker();
         this.lblKleur = new System.Windows.Forms.Label();
         this.lblVulstijl = new System.Windows.Forms.Label();
         this.cboMarkerStyle = new System.Windows.Forms.ComboBox();
         this.lblMarkerStyle = new System.Windows.Forms.Label();
         this.uxButtonPanel = new System.Windows.Forms.Panel();
         this.buttonCancel = new System.Windows.Forms.Button();
         this.buttonOK = new System.Windows.Forms.Button();
         this.filenameLabel = new System.Windows.Forms.Label();
         this.uxGroupBoxLayer = new System.Windows.Forms.GroupBox();
         this.uxGroupBoxEigenschappen.SuspendLayout();
         this.uxButtonPanel.SuspendLayout();
         this.uxGroupBoxLayer.SuspendLayout();
         this.SuspendLayout();
         //
         // uxGroupBoxEigenschappen
         //
         this.uxGroupBoxEigenschappen.AccessibleDescription = null;
         this.uxGroupBoxEigenschappen.AccessibleName = null;
         resources.ApplyResources(this.uxGroupBoxEigenschappen, "uxGroupBoxEigenschappen");
         this.uxGroupBoxEigenschappen.BackgroundImage = null;
         this.uxGroupBoxEigenschappen.Controls.Add(this.lblLijnstijl);
         this.uxGroupBoxEigenschappen.Controls.Add(this.cboLijnstijl);
         this.uxGroupBoxEigenschappen.Controls.Add(this.cboLijndikte);
         this.uxGroupBoxEigenschappen.Controls.Add(this.lblLijndikte);
         this.uxGroupBoxEigenschappen.Controls.Add(this.mapControl);
         this.uxGroupBoxEigenschappen.Controls.Add(this.cboVulstijl);
         this.uxGroupBoxEigenschappen.Controls.Add(this.cboKleur);
         this.uxGroupBoxEigenschappen.Controls.Add(this.lblKleur);
         this.uxGroupBoxEigenschappen.Controls.Add(this.lblVulstijl);
         this.uxGroupBoxEigenschappen.Controls.Add(this.cboMarkerStyle);
         this.uxGroupBoxEigenschappen.Controls.Add(this.lblMarkerStyle);
         this.uxGroupBoxEigenschappen.Font = null;
         this.uxGroupBoxEigenschappen.Name = "uxGroupBoxEigenschappen";
         this.uxGroupBoxEigenschappen.TabStop = false;
         //
         // lblLijnstijl
         //
         this.lblLijnstijl.AccessibleDescription = null;
         this.lblLijnstijl.AccessibleName = null;
         resources.ApplyResources(this.lblLijnstijl, "lblLijnstijl");
         this.lblLijnstijl.Font = null;
         this.lblLijnstijl.Name = "lblLijnstijl";
         //
         // cboLijnstijl
         //
         this.cboLijnstijl.AccessibleDescription = null;
         this.cboLijnstijl.AccessibleName = null;
         resources.ApplyResources(this.cboLijnstijl, "cboLijnstijl");
         this.cboLijnstijl.BackgroundImage = null;
         this.cboLijnstijl.Font = null;
         this.cboLijnstijl.FormattingEnabled = true;
         this.cboLijnstijl.Name = "cboLijnstijl";
         this.cboLijnstijl.SelectedIndexChanged += new System.EventHandler(this.OnCboLijnstijlSelectedIndexChanged);
         //
         // cboLijndikte
         //
         this.cboLijndikte.AccessibleDescription = null;
         this.cboLijndikte.AccessibleName = null;
         resources.ApplyResources(this.cboLijndikte, "cboLijndikte");
         this.cboLijndikte.BackgroundImage = null;
         this.cboLijndikte.Font = null;
         this.cboLijndikte.FormattingEnabled = true;
         this.cboLijndikte.Name = "cboLijndikte";
         this.cboLijndikte.SelectedIndexChanged += new System.EventHandler(this.OnCboLijndikteSelectedIndexChanged);
         //
         // lblLijndikte
         //
         this.lblLijndikte.AccessibleDescription = null;
         this.lblLijndikte.AccessibleName = null;
         resources.ApplyResources(this.lblLijndikte, "lblLijndikte");
         this.lblLijndikte.Font = null;
         this.lblLijndikte.Name = "lblLijndikte";
         //
         // mapControl
         //
         this.mapControl.AccessibleDescription = null;
         this.mapControl.AccessibleName = null;
         resources.ApplyResources(this.mapControl, "mapControl");
         this.mapControl.BackgroundImage = null;
         this.mapControl.CurrentMode = OptimaliseRing.Core.PointerMode.Select;
         this.mapControl.Font = null;
         this.mapControl.KaartlagenMap = null;
         this.mapControl.Name = "mapControl";
         this.mapControl.Profile = null;
         //
         // cboVulstijl
         //
         this.cboVulstijl.AccessibleDescription = null;
         this.cboVulstijl.AccessibleName = null;
         resources.ApplyResources(this.cboVulstijl, "cboVulstijl");
         this.cboVulstijl.BackgroundImage = null;
         this.cboVulstijl.Font = null;
         this.cboVulstijl.FormattingEnabled = true;
         this.cboVulstijl.Name = "cboVulstijl";
         this.cboVulstijl.SelectedIndexChanged += new System.EventHandler(this.OnCboVulstijlSelectedIndexChanged);
         //
         // cboKleur
         //
         this.cboKleur.AccessibleDescription = null;
         this.cboKleur.AccessibleName = null;
         resources.ApplyResources(this.cboKleur, "cboKleur");
         this.cboKleur.BackgroundImage = null;
         this.cboKleur.Color = System.Drawing.Color.Black;
         this.cboKleur.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
         this.cboKleur.DropDownHeight = 1;
         this.cboKleur.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboKleur.DropDownWidth = 1;
         this.cboKleur.Font = null;
         this.cboKleur.FormattingEnabled = true;
         this.cboKleur.Items.AddRange(new object[] {
            resources.GetString("cboKleur.Items"),
            resources.GetString("cboKleur.Items1"),
            resources.GetString("cboKleur.Items2"),
            resources.GetString("cboKleur.Items3"),
            resources.GetString("cboKleur.Items4"),
            resources.GetString("cboKleur.Items5"),
            resources.GetString("cboKleur.Items6"),
            resources.GetString("cboKleur.Items7"),
            resources.GetString("cboKleur.Items8"),
            resources.GetString("cboKleur.Items9"),
            resources.GetString("cboKleur.Items10"),
            resources.GetString("cboKleur.Items11"),
            resources.GetString("cboKleur.Items12"),
            resources.GetString("cboKleur.Items13"),
            resources.GetString("cboKleur.Items14"),
            resources.GetString("cboKleur.Items15"),
            resources.GetString("cboKleur.Items16"),
            resources.GetString("cboKleur.Items17"),
            resources.GetString("cboKleur.Items18"),
            resources.GetString("cboKleur.Items19"),
            resources.GetString("cboKleur.Items20"),
            resources.GetString("cboKleur.Items21"),
            resources.GetString("cboKleur.Items22"),
            resources.GetString("cboKleur.Items23"),
            resources.GetString("cboKleur.Items24"),
            resources.GetString("cboKleur.Items25")});
         this.cboKleur.Name = "cboKleur";
         this.cboKleur.SelectedColorChanged += new System.EventHandler(this.OnCboKleurSelectedColorChanged);
         //
         // lblKleur
         //
         this.lblKleur.AccessibleDescription = null;
         this.lblKleur.AccessibleName = null;
         resources.ApplyResources(this.lblKleur, "lblKleur");
         this.lblKleur.Font = null;
         this.lblKleur.Name = "lblKleur";
         //
         // lblVulstijl
         //
         this.lblVulstijl.AccessibleDescription = null;
         this.lblVulstijl.AccessibleName = null;
         resources.ApplyResources(this.lblVulstijl, "lblVulstijl");
         this.lblVulstijl.Font = null;
         this.lblVulstijl.Name = "lblVulstijl";
         //
         // cboMarkerStyle
         //
         this.cboMarkerStyle.AccessibleDescription = null;
         this.cboMarkerStyle.AccessibleName = null;
         resources.ApplyResources(this.cboMarkerStyle, "cboMarkerStyle");
         this.cboMarkerStyle.BackgroundImage = null;
         this.cboMarkerStyle.Font = null;
         this.cboMarkerStyle.FormattingEnabled = true;
         this.cboMarkerStyle.Name = "cboMarkerStyle";
         this.cboMarkerStyle.SelectedIndexChanged += new System.EventHandler(this.OnCboMarkerStyleSelectedIndexChanged);
         //
         // lblMarkerStyle
         //
         this.lblMarkerStyle.AccessibleDescription = null;
         this.lblMarkerStyle.AccessibleName = null;
         resources.ApplyResources(this.lblMarkerStyle, "lblMarkerStyle");
         this.lblMarkerStyle.Font = null;
         this.lblMarkerStyle.Name = "lblMarkerStyle";
         //
         // uxButtonPanel
         //
         this.uxButtonPanel.AccessibleDescription = null;
         this.uxButtonPanel.AccessibleName = null;
         resources.ApplyResources(this.uxButtonPanel, "uxButtonPanel");
         this.uxButtonPanel.BackColor = System.Drawing.Color.Transparent;
         this.uxButtonPanel.BackgroundImage = null;
         this.uxButtonPanel.Controls.Add(this.buttonCancel);
         this.uxButtonPanel.Controls.Add(this.buttonOK);
         this.uxButtonPanel.Font = null;
         this.uxButtonPanel.Name = "uxButtonPanel";
         //
         // buttonCancel
         //
         this.buttonCancel.AccessibleDescription = null;
         this.buttonCancel.AccessibleName = null;
         resources.ApplyResources(this.buttonCancel, "buttonCancel");
         this.buttonCancel.BackgroundImage = null;
         this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.buttonCancel.Font = null;
         this.buttonCancel.Name = "buttonCancel";
         this.buttonCancel.UseVisualStyleBackColor = true;
         this.buttonCancel.Click += new System.EventHandler(this.OnButtonCancelClick);
         //
         // buttonOK
         //
         this.buttonOK.AccessibleDescription = null;
         this.buttonOK.AccessibleName = null;
         resources.ApplyResources(this.buttonOK, "buttonOK");
         this.buttonOK.BackgroundImage = null;
         this.buttonOK.Font = null;
         this.buttonOK.Name = "buttonOK";
         this.buttonOK.UseVisualStyleBackColor = true;
         this.buttonOK.Click += new System.EventHandler(this.OnButtonOKClick);
         //
         // filenameLabel
         //
         this.filenameLabel.AccessibleDescription = null;
         this.filenameLabel.AccessibleName = null;
         resources.ApplyResources(this.filenameLabel, "filenameLabel");
         this.filenameLabel.Name = "filenameLabel";
         //
         // uxGroupBoxLayer
         //
         this.uxGroupBoxLayer.AccessibleDescription = null;
         this.uxGroupBoxLayer.AccessibleName = null;
         resources.ApplyResources(this.uxGroupBoxLayer, "uxGroupBoxLayer");
         this.uxGroupBoxLayer.BackgroundImage = null;
         this.uxGroupBoxLayer.Controls.Add(this.filenameLabel);
         this.uxGroupBoxLayer.Font = null;
         this.uxGroupBoxLayer.Name = "uxGroupBoxLayer";
         this.uxGroupBoxLayer.TabStop = false;
         //
         // KaartlaagEigenschappenForm
         //
         this.AccessibleDescription = null;
         this.AccessibleName = null;
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackgroundImage = null;
         this.ControlBox = false;
         this.Controls.Add(this.uxGroupBoxEigenschappen);
         this.Controls.Add(this.uxButtonPanel);
         this.Controls.Add(this.uxGroupBoxLayer);
         this.Font = null;
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Icon = null;
         this.Name = "KaartlaagEigenschappenForm";
         this.ShowIcon = false;
         this.ShowInTaskbar = false;
         this.uxGroupBoxEigenschappen.ResumeLayout(false);
         this.uxGroupBoxEigenschappen.PerformLayout();
         this.uxButtonPanel.ResumeLayout(false);
         this.uxGroupBoxLayer.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.GroupBox uxGroupBoxEigenschappen;
      private System.Windows.Forms.Label lblKleur;
      private System.Windows.Forms.Label lblVulstijl;
      private System.Windows.Forms.Panel uxButtonPanel;
      private System.Windows.Forms.Button buttonCancel;
      private System.Windows.Forms.Button buttonOK;
      private System.Windows.Forms.Label filenameLabel;
      private System.Windows.Forms.GroupBox uxGroupBoxLayer;
      private MapControl mapControl;
      private System.Windows.Forms.ComboBox cboVulstijl;
      private OptimaliseRing.Controls.ColorPicker.ComboBoxColorPicker cboKleur;
      private System.Windows.Forms.ComboBox cboMarkerStyle;
      private System.Windows.Forms.Label lblMarkerStyle;
      private System.Windows.Forms.ComboBox cboLijndikte;
      private System.Windows.Forms.Label lblLijndikte;
      private System.Windows.Forms.Label lblLijnstijl;
      private System.Windows.Forms.ComboBox cboLijnstijl;

   }
}
