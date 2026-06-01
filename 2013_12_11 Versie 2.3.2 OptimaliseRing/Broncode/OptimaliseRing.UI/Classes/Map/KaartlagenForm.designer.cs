namespace OptimaliseRing.UI
{
  partial class KaartlagenForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KaartlagenForm));
      this.uxButtonPanel = new System.Windows.Forms.Panel();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOK = new System.Windows.Forms.Button();
      this.btnAdd = new System.Windows.Forms.Button();
      this.btnDelete = new System.Windows.Forms.Button();
      this.btnUp = new System.Windows.Forms.Button();
      this.btnDown = new System.Windows.Forms.Button();
      this.btnProperties = new System.Windows.Forms.Button();
      this.dgvKaartlagen = new System.Windows.Forms.DataGridView();
      this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.toolStripButtons = new System.Windows.Forms.ToolStrip();
      this.uxAddButton = new System.Windows.Forms.ToolStripButton();
      this.uxDeleteButton = new System.Windows.Forms.ToolStripButton();
      this.uxUpButton = new System.Windows.Forms.ToolStripButton();
      this.uxDownButton = new System.Windows.Forms.ToolStripButton();
      this.uxPropertiesButton = new System.Windows.Forms.ToolStripButton();
      this.uxButtonPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvKaartlagen)).BeginInit();
      this.toolStripButtons.SuspendLayout();
      this.SuspendLayout();
      //
      // uxButtonPanel
      //
      this.uxButtonPanel.AccessibleDescription = null;
      this.uxButtonPanel.AccessibleName = null;
      resources.ApplyResources(this.uxButtonPanel, "uxButtonPanel");
      this.uxButtonPanel.BackColor = System.Drawing.Color.Transparent;
      this.uxButtonPanel.BackgroundImage = null;
      this.uxButtonPanel.Controls.Add(this.btnCancel);
      this.uxButtonPanel.Controls.Add(this.btnOK);
      this.uxButtonPanel.Font = null;
      this.uxButtonPanel.Name = "uxButtonPanel";
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
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOK.Font = null;
      this.btnOK.Name = "btnOK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.OnBtnOKClick);
      //
      // btnAdd
      //
      this.btnAdd.AccessibleDescription = null;
      this.btnAdd.AccessibleName = null;
      resources.ApplyResources(this.btnAdd, "btnAdd");
      this.btnAdd.BackgroundImage = null;
      this.btnAdd.Font = null;
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new System.EventHandler(this.OnBtnAddClick);
      //
      // btnDelete
      //
      this.btnDelete.AccessibleDescription = null;
      this.btnDelete.AccessibleName = null;
      resources.ApplyResources(this.btnDelete, "btnDelete");
      this.btnDelete.BackgroundImage = null;
      this.btnDelete.Font = null;
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.UseVisualStyleBackColor = true;
      this.btnDelete.Click += new System.EventHandler(this.OnBtnDeleteClick);
      //
      // btnUp
      //
      this.btnUp.AccessibleDescription = null;
      this.btnUp.AccessibleName = null;
      resources.ApplyResources(this.btnUp, "btnUp");
      this.btnUp.BackgroundImage = null;
      this.btnUp.Font = null;
      this.btnUp.Name = "btnUp";
      this.btnUp.UseVisualStyleBackColor = true;
      this.btnUp.Click += new System.EventHandler(this.OnBtnUpClick);
      //
      // btnDown
      //
      this.btnDown.AccessibleDescription = null;
      this.btnDown.AccessibleName = null;
      resources.ApplyResources(this.btnDown, "btnDown");
      this.btnDown.BackgroundImage = null;
      this.btnDown.Font = null;
      this.btnDown.Name = "btnDown";
      this.btnDown.UseVisualStyleBackColor = true;
      this.btnDown.Click += new System.EventHandler(this.OnBtnDownClick);
      //
      // btnProperties
      //
      this.btnProperties.AccessibleDescription = null;
      this.btnProperties.AccessibleName = null;
      resources.ApplyResources(this.btnProperties, "btnProperties");
      this.btnProperties.BackgroundImage = null;
      this.btnProperties.Font = null;
      this.btnProperties.Name = "btnProperties";
      this.btnProperties.UseVisualStyleBackColor = true;
      this.btnProperties.Click += new System.EventHandler(this.OnBtnPropertiesClick);
      //
      // dgvKaartlagen
      //
      this.dgvKaartlagen.AccessibleDescription = null;
      this.dgvKaartlagen.AccessibleName = null;
      this.dgvKaartlagen.AllowUserToAddRows = false;
      this.dgvKaartlagen.AllowUserToDeleteRows = false;
      this.dgvKaartlagen.AllowUserToResizeColumns = false;
      this.dgvKaartlagen.AllowUserToResizeRows = false;
      resources.ApplyResources(this.dgvKaartlagen, "dgvKaartlagen");
      this.dgvKaartlagen.BackgroundImage = null;
      this.dgvKaartlagen.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvKaartlagen.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
      this.dgvKaartlagen.Font = null;
      this.dgvKaartlagen.GridColor = System.Drawing.SystemColors.Control;
      this.dgvKaartlagen.MultiSelect = false;
      this.dgvKaartlagen.Name = "dgvKaartlagen";
      this.dgvKaartlagen.RowHeadersVisible = false;
      this.dgvKaartlagen.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      //
      // Column1
      //
      resources.ApplyResources(this.Column1, "Column1");
      this.Column1.Name = "Column1";
      //
      // Column2
      //
      resources.ApplyResources(this.Column2, "Column2");
      this.Column2.Name = "Column2";
      //
      // toolStripButtons
      //
      this.toolStripButtons.AccessibleDescription = null;
      this.toolStripButtons.AccessibleName = null;
      resources.ApplyResources(this.toolStripButtons, "toolStripButtons");
      this.toolStripButtons.BackgroundImage = null;
      this.toolStripButtons.Font = null;
      this.toolStripButtons.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.toolStripButtons.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxAddButton,
            this.uxDeleteButton,
            this.uxUpButton,
            this.uxDownButton,
            this.uxPropertiesButton});
      this.toolStripButtons.Name = "toolStripButtons";
      this.toolStripButtons.Click += new System.EventHandler(this.ToolStripButtonsClick);
      //
      // uxAddButton
      //
      this.uxAddButton.AccessibleDescription = null;
      this.uxAddButton.AccessibleName = null;
      resources.ApplyResources(this.uxAddButton, "uxAddButton");
      this.uxAddButton.BackgroundImage = null;
      this.uxAddButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.uxAddButton.Name = "uxAddButton";
      this.uxAddButton.Click += new System.EventHandler(this.ToolStripButtonsClick);
      //
      // uxDeleteButton
      //
      this.uxDeleteButton.AccessibleDescription = null;
      this.uxDeleteButton.AccessibleName = null;
      resources.ApplyResources(this.uxDeleteButton, "uxDeleteButton");
      this.uxDeleteButton.BackgroundImage = null;
      this.uxDeleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.uxDeleteButton.Name = "uxDeleteButton";
      this.uxDeleteButton.Click += new System.EventHandler(this.ToolStripButtonsClick);
      //
      // uxUpButton
      //
      this.uxUpButton.AccessibleDescription = null;
      this.uxUpButton.AccessibleName = null;
      resources.ApplyResources(this.uxUpButton, "uxUpButton");
      this.uxUpButton.BackgroundImage = null;
      this.uxUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.uxUpButton.Name = "uxUpButton";
      this.uxUpButton.Click += new System.EventHandler(this.ToolStripButtonsClick);
      //
      // uxDownButton
      //
      this.uxDownButton.AccessibleDescription = null;
      this.uxDownButton.AccessibleName = null;
      resources.ApplyResources(this.uxDownButton, "uxDownButton");
      this.uxDownButton.BackgroundImage = null;
      this.uxDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.uxDownButton.Name = "uxDownButton";
      this.uxDownButton.Click += new System.EventHandler(this.ToolStripButtonsClick);
      //
      // uxPropertiesButton
      //
      this.uxPropertiesButton.AccessibleDescription = null;
      this.uxPropertiesButton.AccessibleName = null;
      resources.ApplyResources(this.uxPropertiesButton, "uxPropertiesButton");
      this.uxPropertiesButton.BackgroundImage = null;
      this.uxPropertiesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.uxPropertiesButton.Name = "uxPropertiesButton";
      this.uxPropertiesButton.Click += new System.EventHandler(this.ToolStripButtonsClick);
      //
      // KaartlagenForm
      //
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.CancelButton = this.btnCancel;
      this.Controls.Add(this.dgvKaartlagen);
      this.Controls.Add(this.btnProperties);
      this.Controls.Add(this.btnDown);
      this.Controls.Add(this.btnUp);
      this.Controls.Add(this.btnDelete);
      this.Controls.Add(this.btnAdd);
      this.Controls.Add(this.uxButtonPanel);
      this.Controls.Add(this.toolStripButtons);
      this.Font = null;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = null;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "KaartlagenForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.uxButtonPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvKaartlagen)).EndInit();
      this.toolStripButtons.ResumeLayout(false);
      this.toolStripButtons.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel uxButtonPanel;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Button btnDelete;
    private System.Windows.Forms.Button btnUp;
    private System.Windows.Forms.Button btnDown;
    private System.Windows.Forms.Button btnProperties;
    private System.Windows.Forms.DataGridView dgvKaartlagen;
    private System.Windows.Forms.ToolStrip toolStripButtons;
    private System.Windows.Forms.ToolStripButton uxAddButton;
    private System.Windows.Forms.ToolStripButton uxDeleteButton;
    private System.Windows.Forms.ToolStripButton uxUpButton;
    private System.Windows.Forms.ToolStripButton uxDownButton;
    private System.Windows.Forms.ToolStripButton uxPropertiesButton;
    private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
    private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
  }
}
