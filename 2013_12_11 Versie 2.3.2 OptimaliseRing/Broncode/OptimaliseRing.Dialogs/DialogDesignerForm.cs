//---------------------------------------------------------------------------
// Dialog Workshop for .NET Framework
// Copyright (c) 2002 - 2004, COMPONENTAGE Software,
// all rights reserved
//
// http://www.componentage.com
// support@componentage.com
//
// All files included into Dialog Workshop.NET source code package,
// remain COMPONENTAGE's exclusive property. Regardless of
// any modifications that you make, you may not distribute
// or publish any source code files.
//---------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Reflection;

namespace ComponentAge.Dialogs
{
    internal class DialogDesignerForm : System.Windows.Forms.Form
	{
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBottom;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.NumericUpDown numericY;
        private System.Windows.Forms.NumericUpDown numericX;
        private System.Windows.Forms.ComboBox comboPosition;
        private System.Windows.Forms.ComboBox comboLeft;
        private System.Windows.Forms.ComboBox comboRight;
        private System.Windows.Forms.ComboBox comboTop;
        private System.Windows.Forms.Button button1;
        internal System.Windows.Forms.TextBox textCaption;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private System.Windows.Forms.CheckBox checkFit;

        CommonDialog _instance;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.NumericUpDown numericUpDown2;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.RadioButton rbOwnerCenter;
		private System.Windows.Forms.RadioButton rbAbsolute;
		private System.Windows.Forms.RadioButton rbDefault;
		private System.Windows.Forms.RadioButton rbMainFormCenter;
		private System.Windows.Forms.RadioButton rbCenter;
        CaCustDlgParams dlgparams;

		public DialogDesignerForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DialogDesignerForm));
			this.comboTop = new System.Windows.Forms.ComboBox();
			this.comboRight = new System.Windows.Forms.ComboBox();
			this.comboLeft = new System.Windows.Forms.ComboBox();
			this.comboBottom = new System.Windows.Forms.ComboBox();
			this.textCaption = new System.Windows.Forms.TextBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.checkFit = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.numericY = new System.Windows.Forms.NumericUpDown();
			this.numericX = new System.Windows.Forms.NumericUpDown();
			this.comboPosition = new System.Windows.Forms.ComboBox();
			this.button1 = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.label10 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.rbOwnerCenter = new System.Windows.Forms.RadioButton();
			this.rbAbsolute = new System.Windows.Forms.RadioButton();
			this.rbDefault = new System.Windows.Forms.RadioButton();
			this.rbMainFormCenter = new System.Windows.Forms.RadioButton();
			this.rbCenter = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.numericY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericX)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.SuspendLayout();
			//
			// comboTop
			//
			this.comboTop.DropDownWidth = 250;
			this.comboTop.Location = new System.Drawing.Point(184, 60);
			this.comboTop.Name = "comboTop";
			this.comboTop.Size = new System.Drawing.Size(280, 21);
			this.comboTop.TabIndex = 4;
			//
			// comboRight
			//
			this.comboRight.DropDownWidth = 200;
			this.comboRight.Location = new System.Drawing.Point(480, 144);
			this.comboRight.Name = "comboRight";
			this.comboRight.Size = new System.Drawing.Size(152, 21);
			this.comboRight.TabIndex = 6;
			//
			// comboLeft
			//
			this.comboLeft.DropDownWidth = 200;
			this.comboLeft.Location = new System.Drawing.Point(16, 144);
			this.comboLeft.Name = "comboLeft";
			this.comboLeft.Size = new System.Drawing.Size(152, 21);
			this.comboLeft.TabIndex = 2;
			//
			// comboBottom
			//
			this.comboBottom.DropDownWidth = 250;
			this.comboBottom.Location = new System.Drawing.Point(184, 328);
			this.comboBottom.Name = "comboBottom";
			this.comboBottom.Size = new System.Drawing.Size(280, 21);
			this.comboBottom.TabIndex = 8;
			//
			// textCaption
			//
			this.textCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textCaption.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(51)), ((System.Byte)(104)), ((System.Byte)(227)));
			this.textCaption.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.textCaption.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.textCaption.Location = new System.Drawing.Point(16, 12);
			this.textCaption.Multiline = true;
			this.textCaption.Name = "textCaption";
			this.textCaption.Size = new System.Drawing.Size(568, 20);
			this.textCaption.TabIndex = 0;
			this.textCaption.Text = "";
			//
			// buttonCancel
			//
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(576, 432);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			//
			// buttonOK
			//
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(496, 432);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			//
			// checkFit
			//
			this.checkFit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.checkFit.Location = new System.Drawing.Point(296, 16);
			this.checkFit.Name = "checkFit";
			this.checkFit.Size = new System.Drawing.Size(112, 24);
			this.checkFit.TabIndex = 2;
			this.checkFit.Text = "Fit to screen";
			//
			// label7
			//
			this.label7.Location = new System.Drawing.Point(392, 264);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(48, 23);
			this.label7.TabIndex = 5;
			this.label7.Text = "Y shift:";
			this.label7.Visible = false;
			//
			// label6
			//
			this.label6.Location = new System.Drawing.Point(176, 328);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(48, 23);
			this.label6.TabIndex = 3;
			this.label6.Text = "X shift:";
			this.label6.Visible = false;
			//
			// label5
			//
			this.label5.Location = new System.Drawing.Point(392, 336);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(64, 23);
			this.label5.TabIndex = 0;
			this.label5.Text = "Relative to:";
			this.label5.Visible = false;
			//
			// numericY
			//
			this.numericY.Location = new System.Drawing.Point(400, 224);
			this.numericY.Maximum = new System.Decimal(new int[] {
																	 10000,
																	 0,
																	 0,
																	 0});
			this.numericY.Minimum = new System.Decimal(new int[] {
																	 10000,
																	 0,
																	 0,
																	 -2147483648});
			this.numericY.Name = "numericY";
			this.numericY.TabIndex = 6;
			this.numericY.Visible = false;
			//
			// numericX
			//
			this.numericX.Location = new System.Drawing.Point(184, 352);
			this.numericX.Maximum = new System.Decimal(new int[] {
																	 1000,
																	 0,
																	 0,
																	 0});
			this.numericX.Minimum = new System.Decimal(new int[] {
																	 1000,
																	 0,
																	 0,
																	 -2147483648});
			this.numericX.Name = "numericX";
			this.numericX.TabIndex = 4;
			this.numericX.Visible = false;
			//
			// comboPosition
			//
			this.comboPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPosition.Items.AddRange(new object[] {
															   "Default position",
															   "Screen center",
															   "Main form center",
															   "Owner center"});
			this.comboPosition.Location = new System.Drawing.Point(464, 344);
			this.comboPosition.Name = "comboPosition";
			this.comboPosition.Size = new System.Drawing.Size(136, 21);
			this.comboPosition.TabIndex = 1;
			this.comboPosition.Visible = false;
			//
			// button1
			//
			this.button1.Location = new System.Drawing.Point(16, 432);
			this.button1.Name = "button1";
			this.button1.TabIndex = 1;
			this.button1.Text = "About...";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			//
			// pictureBox1
			//
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(8, 8);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(632, 368);
			this.pictureBox1.TabIndex = 6;
			this.pictureBox1.TabStop = false;
			//
			// tabControl1
			//
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(8, 16);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(656, 408);
			this.tabControl1.TabIndex = 0;
			//
			// tabPage1
			//
			this.tabPage1.Controls.Add(this.comboBottom);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this.comboLeft);
			this.tabPage1.Controls.Add(this.comboRight);
			this.tabPage1.Controls.Add(this.comboTop);
			this.tabPage1.Controls.Add(this.textCaption);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Controls.Add(this.pictureBox1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(648, 382);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Docked forms";
			//
			// label4
			//
			this.label4.Location = new System.Drawing.Point(16, 120);
			this.label4.Name = "label4";
			this.label4.TabIndex = 1;
			this.label4.Text = "Left form:";
			//
			// label3
			//
			this.label3.Location = new System.Drawing.Point(184, 308);
			this.label3.Name = "label3";
			this.label3.TabIndex = 7;
			this.label3.Text = "Bottom form:";
			//
			// label2
			//
			this.label2.Location = new System.Drawing.Point(480, 120);
			this.label2.Name = "label2";
			this.label2.TabIndex = 5;
			this.label2.Text = "Right form:";
			//
			// label1
			//
			this.label1.Location = new System.Drawing.Point(184, 40);
			this.label1.Name = "label1";
			this.label1.TabIndex = 3;
			this.label1.Text = "Top form:";
			//
			// tabPage2
			//
			this.tabPage2.Controls.Add(this.label10);
			this.tabPage2.Controls.Add(this.groupBox1);
			this.tabPage2.Controls.Add(this.label7);
			this.tabPage2.Controls.Add(this.label6);
			this.tabPage2.Controls.Add(this.label5);
			this.tabPage2.Controls.Add(this.numericX);
			this.tabPage2.Controls.Add(this.comboPosition);
			this.tabPage2.Controls.Add(this.numericY);
			this.tabPage2.Controls.Add(this.checkFit);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(648, 382);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Dialog Position";
			//
			// label10
			//
			this.label10.Location = new System.Drawing.Point(400, 16);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(232, 48);
			this.label10.TabIndex = 13;
			this.label10.Text = "If checked, dialog is always visible on the screen even if you specify dialog pos" +
				"ition outside of the screen bounds.";
			//
			// groupBox1
			//
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.numericUpDown2);
			this.groupBox1.Controls.Add(this.numericUpDown1);
			this.groupBox1.Controls.Add(this.rbOwnerCenter);
			this.groupBox1.Controls.Add(this.rbAbsolute);
			this.groupBox1.Controls.Add(this.rbDefault);
			this.groupBox1.Controls.Add(this.rbMainFormCenter);
			this.groupBox1.Controls.Add(this.rbCenter);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(256, 280);
			this.groupBox1.TabIndex = 12;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Specify dialog\'s position";
			//
			// label9
			//
			this.label9.Location = new System.Drawing.Point(88, 248);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(32, 23);
			this.label9.TabIndex = 15;
			this.label9.Text = "Y:";
			//
			// label8
			//
			this.label8.Location = new System.Drawing.Point(88, 216);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(32, 23);
			this.label8.TabIndex = 14;
			this.label8.Text = "X:";
			//
			// numericUpDown2
			//
			this.numericUpDown2.Location = new System.Drawing.Point(120, 248);
			this.numericUpDown2.Name = "numericUpDown2";
			this.numericUpDown2.Size = new System.Drawing.Size(88, 20);
			this.numericUpDown2.TabIndex = 13;
			//
			// numericUpDown1
			//
			this.numericUpDown1.Location = new System.Drawing.Point(120, 216);
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(88, 20);
			this.numericUpDown1.TabIndex = 12;
			//
			// rbOwnerCenter
			//
			this.rbOwnerCenter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.rbOwnerCenter.Location = new System.Drawing.Point(16, 144);
			this.rbOwnerCenter.Name = "rbOwnerCenter";
			this.rbOwnerCenter.Size = new System.Drawing.Size(184, 24);
			this.rbOwnerCenter.TabIndex = 10;
			this.rbOwnerCenter.Text = "Owner Form center";
			//
			// rbAbsolute
			//
			this.rbAbsolute.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.rbAbsolute.Location = new System.Drawing.Point(16, 184);
			this.rbAbsolute.Name = "rbAbsolute";
			this.rbAbsolute.Size = new System.Drawing.Size(224, 24);
			this.rbAbsolute.TabIndex = 11;
			this.rbAbsolute.Text = "Specify absolute screen coordinates:";
			//
			// rbDefault
			//
			this.rbDefault.Checked = true;
			this.rbDefault.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.rbDefault.Location = new System.Drawing.Point(16, 24);
			this.rbDefault.Name = "rbDefault";
			this.rbDefault.Size = new System.Drawing.Size(168, 24);
			this.rbDefault.TabIndex = 7;
			this.rbDefault.TabStop = true;
			this.rbDefault.Text = "Default dialog position";
			//
			// rbMainFormCenter
			//
			this.rbMainFormCenter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.rbMainFormCenter.Location = new System.Drawing.Point(16, 104);
			this.rbMainFormCenter.Name = "rbMainFormCenter";
			this.rbMainFormCenter.Size = new System.Drawing.Size(136, 24);
			this.rbMainFormCenter.TabIndex = 9;
			this.rbMainFormCenter.Text = "MainForm center";
			//
			// rbCenter
			//
			this.rbCenter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.rbCenter.Location = new System.Drawing.Point(16, 64);
			this.rbCenter.Name = "rbCenter";
			this.rbCenter.TabIndex = 8;
			this.rbCenter.Text = "Screen center";
			//
			// DialogDesignerForm
			//
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(674, 464);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DialogDesignerForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Dialog Workshop .NET - Dialog Designer";
			this.Load += new System.EventHandler(this.DialogDesignerForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericX)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion


        private void button1_Click(object sender, System.EventArgs e)
        {
            try
            {
                AboutForm form = new AboutForm();
                form.labelVer.Text = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();//.GetAssembly(this.GetType()).GetName().Version.ToString());
                form.ShowDialog();
            }
            catch
            {
            }
        }

        private void DialogDesignerForm_Load(object sender, System.EventArgs e)
        {
            if (_instance is CaCommonDialog)
                dlgparams = ((CaCommonDialog)_instance).CustDlgParams;
            else
            {
                Type t = _instance.GetType();
                PropertyInfo pi = t.GetProperty("CustDlgParams");
                object val = pi.GetValue(_instance, new object[] {});
                dlgparams = (CaCustDlgParams)val;
            }
            if (dlgparams == null)
                throw new Exception("public CustDlgParams property not found");

            object[] attrs = _instance.GetType().GetCustomAttributes(typeof(FeatureSupportAttribute), true);
            for (int i = 0; i < attrs.Length; i++ )
            {
                if (attrs[i] is TopFormSupportAttribute)
                {
                    TopFormSupportAttribute tops = attrs[i] as TopFormSupportAttribute;
                    if (!tops.Supported)
                        comboTop.Enabled = false;
                }
                if (attrs[i] is LeftFormSupportAttribute)
                {
                    LeftFormSupportAttribute lefts = attrs[i] as LeftFormSupportAttribute;
                    if (!lefts.Supported)
                        comboLeft.Enabled = false;
                }
                if (attrs[i] is RightFormSupportAttribute)
                {
                    RightFormSupportAttribute rights = attrs[i] as RightFormSupportAttribute;
                    if (!rights.Supported)
                        comboRight.Enabled = false;
                }
                if (attrs[i] is BottomFormSupportAttribute)
                {
                    BottomFormSupportAttribute bottoms = attrs[i] as BottomFormSupportAttribute;
                    if (!bottoms.Supported)
                        comboBottom.Enabled = false;
                }
            }

            if (comboLeft.Enabled)
                comboLeft.Text = dlgparams.LeftFormClassName;
            if (comboTop.Enabled)
                comboTop.Text = dlgparams.TopFormClassName;
            if (comboRight.Enabled)
                comboRight.Text = dlgparams.RightFormClassName;
            if (comboBottom.Enabled)
                comboBottom.Text = dlgparams.BottomFormClassName;

            comboPosition.SelectedIndex = (int)dlgparams.StartPosition;
            //numericX.Value = dlgparams.PosParams.ShiftX;
            //numericY.Value = dlgparams.PosParams.ShiftY;
            checkFit.Checked = dlgparams.PosParams.FitToScreen;
            textCaption.Text = dlgparams.GetDialogTitle();

            ISite site = ((IComponent)Instance).Site;
            if (site == null)
                return;
            IDesignerEventService service = site.GetService(typeof(IDesignerEventService)) as IDesignerEventService;
            IDesignerHost hostservice = site.GetService(typeof(IDesignerHost)) as IDesignerHost;
            int idx = hostservice.RootComponentClassName.LastIndexOf('.');
            string namespaceName = "";
            if (idx > -1)
                namespaceName = hostservice.RootComponentClassName.Substring(0, idx);
            if (service == null)
                return;
            ArrayList items = new ArrayList();
            foreach (IDesignerHost host in service.Designers)
            {
                if (host.RootComponentClassName == null || host.RootComponentClassName == "")
                    continue;

                int idx1 = host.RootComponentClassName.LastIndexOf('.');
                string namespaceName1 = host.RootComponentClassName.Substring(0, idx1);
                if (namespaceName != namespaceName1)
                    continue;

                IContainer container = host.Container;
                if (container != null)
                {
                    string formName = "";
                    for (int i = 0; i < container.Components.Count; i++)
                    {
                        IComponent comp = container.Components[i];
                        if (comp is Form && comp.Site != null)
                        {
                            formName = namespaceName + "." + comp.Site.Name;
                            if (items.IndexOf(formName) == -1)
                                items.Add(formName);
                            break;
                        }
                    }
                }
            }
            items.Sort();
            comboLeft.Items.AddRange(items.ToArray());
            comboTop.Items.AddRange(items.ToArray());
            comboRight.Items.AddRange(items.ToArray());
            comboBottom.Items.AddRange(items.ToArray());

			numericUpDown1.Value = dlgparams.PosParams.CustomLocation.X;
			numericUpDown2.Value = dlgparams.PosParams.CustomLocation.Y;

			rbDefault.Checked = (dlgparams.StartPosition == DialogPosition.Default);
			rbCenter.Checked = (dlgparams.StartPosition == DialogPosition.ScreenCenter);
			rbMainFormCenter.Checked = (dlgparams.StartPosition == DialogPosition.MainFormCenter);
			rbOwnerCenter.Checked = (dlgparams.StartPosition == DialogPosition.OwnerFormSenter);
			rbAbsolute.Checked = (dlgparams.StartPosition == DialogPosition.Custom);
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            dlgparams.SetDialogTitle(textCaption.Text);
            dlgparams.LeftFormClassName = comboLeft.Text;
            dlgparams.TopFormClassName = comboTop.Text;
            dlgparams.RightFormClassName = comboRight.Text;
            dlgparams.BottomFormClassName = comboBottom.Text;
            dlgparams.PosParams.FitToScreen = checkFit.Checked;
            dlgparams.StartPosition = (DialogPosition)comboPosition.SelectedIndex;
            //dlgparams.PosParams.ShiftX = (int)(numericX.Value);
            //dlgparams.PosParams.ShiftY = (int)(numericY.Value);
			dlgparams.PosParams.CustomLocation = new Point((int)numericUpDown1.Value, (int)numericUpDown2.Value);

			if (rbDefault.Checked)
			{
				dlgparams.StartPosition = DialogPosition.Default;
			}
			else if (rbCenter.Checked)
			{
				dlgparams.StartPosition = DialogPosition.ScreenCenter;
			}
			else if (rbMainFormCenter.Checked)
			{
				dlgparams.StartPosition = DialogPosition.MainFormCenter;
			}
			else if (rbOwnerCenter.Checked)
			{
				dlgparams.StartPosition = DialogPosition.OwnerFormSenter;
			}
			else if (rbAbsolute.Checked)
			{
				dlgparams.StartPosition = DialogPosition.Custom;
			}
		}

        // properties
        public CommonDialog Instance
        {
            get {return _instance;}
            set {_instance = value;}
        }
	}
}
