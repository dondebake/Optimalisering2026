namespace OptimaliseRing.Core
{
  /// <summary>
  ///
  /// </summary>
  partial class OverForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OverForm));
      this.uxButtonPanel = new System.Windows.Forms.Panel();
      this.btnOK = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.lblInformatie = new System.Windows.Forms.Label();
      this.lblOntwikkeld = new System.Windows.Forms.Label();
      this.lblDescription = new System.Windows.Forms.Label();
      this.lblCopyrightLabel = new System.Windows.Forms.Label();
      this.lblVersie = new System.Windows.Forms.Label();
      this.uxWhatLabel = new System.Windows.Forms.Label();
      this.uxButtonPanel.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
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
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      resources.ApplyResources(this.btnOK, "btnOK");
      this.btnOK.Name = "btnOK";
      this.btnOK.UseVisualStyleBackColor = true;
      //
      // groupBox1
      //
      this.groupBox1.Controls.Add(this.lblInformatie);
      this.groupBox1.Controls.Add(this.lblOntwikkeld);
      this.groupBox1.Controls.Add(this.lblDescription);
      this.groupBox1.Controls.Add(this.lblCopyrightLabel);
      this.groupBox1.Controls.Add(this.lblVersie);
      this.groupBox1.Controls.Add(this.uxWhatLabel);
      resources.ApplyResources(this.groupBox1, "groupBox1");
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.TabStop = false;
      //
      // lblInformatie
      //
      this.lblInformatie.BackColor = System.Drawing.Color.Transparent;
      resources.ApplyResources(this.lblInformatie, "lblInformatie");
      this.lblInformatie.Name = "lblInformatie";
      //
      // lblOntwikkeld
      //
      this.lblOntwikkeld.BackColor = System.Drawing.Color.Transparent;
      resources.ApplyResources(this.lblOntwikkeld, "lblOntwikkeld");
      this.lblOntwikkeld.Name = "lblOntwikkeld";
      //
      // lblDescription
      //
      this.lblDescription.BackColor = System.Drawing.Color.Transparent;
      resources.ApplyResources(this.lblDescription, "lblDescription");
      this.lblDescription.Name = "lblDescription";
      //
      // lblCopyrightLabel
      //
      this.lblCopyrightLabel.BackColor = System.Drawing.Color.Transparent;
      resources.ApplyResources(this.lblCopyrightLabel, "lblCopyrightLabel");
      this.lblCopyrightLabel.Name = "lblCopyrightLabel";
      this.lblCopyrightLabel.UseMnemonic = false;
      //
      // lblVersie
      //
      this.lblVersie.BackColor = System.Drawing.Color.Transparent;
      resources.ApplyResources(this.lblVersie, "lblVersie");
      this.lblVersie.Name = "lblVersie";
      //
      // uxWhatLabel
      //
      this.uxWhatLabel.BackColor = System.Drawing.Color.Transparent;
      resources.ApplyResources(this.uxWhatLabel, "uxWhatLabel");
      this.uxWhatLabel.Name = "uxWhatLabel";
      //
      // OverForm
      //
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnOK;
      this.ControlBox = false;
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.uxButtonPanel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "OverForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.uxButtonPanel.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel uxButtonPanel;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label lblCopyrightLabel;
    private System.Windows.Forms.Label lblOntwikkeld;
    private System.Windows.Forms.Label lblDescription;
    private System.Windows.Forms.Label lblVersie;
    private System.Windows.Forms.Label uxWhatLabel;
    private System.Windows.Forms.Label lblInformatie;
  }
}
