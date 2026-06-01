using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ComponentAge.Dialogs
{
	/// <summary>
	/// Summary description for RecentFilesTabForm.
	/// </summary>
	internal class RecentFilesTabForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		internal System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabBrowse;
		private System.Windows.Forms.TabPage tabRecent;
		CaFileDialogBase _dlg;

		public RecentFilesTabForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabBrowse = new System.Windows.Forms.TabPage();
            this.tabRecent = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            //
            // tabControl1
            //
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.tabControl1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                      this.tabBrowse,
                                                                                      this.tabRecent});
            this.tabControl1.Location = new System.Drawing.Point(1, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(290, 22);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            //
            // tabBrowse
            //
            this.tabBrowse.Location = new System.Drawing.Point(4, 22);
            this.tabBrowse.Name = "tabBrowse";
            this.tabBrowse.Size = new System.Drawing.Size(282, 0);
            this.tabBrowse.TabIndex = 0;
            this.tabBrowse.Text = "Browse";
            //
            // tabRecent
            //
            this.tabRecent.Location = new System.Drawing.Point(4, 22);
            this.tabRecent.Name = "tabRecent";
            this.tabRecent.Size = new System.Drawing.Size(282, -4);
            this.tabRecent.TabIndex = 1;
            this.tabRecent.Text = "Recent";
            //
            // RecentFilesTabForm
            //
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(292, 22);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.tabControl1});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(0, 5);
            this.Name = "RecentFilesTabForm";
            this.ShowInTaskbar = false;
            this.Text = "RecentFilesTabForm";
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (tabControl1.SelectedIndex == 0)
			{
				_dlg.ShowCustomSelectionCtrl(false);
				_dlg.ShowStdSelectionCtrl(true);
			}
			else
			{
				_dlg.ShowCustomSelectionCtrl(true);
				_dlg.ShowStdSelectionCtrl(false);
			}
		}

		// properties
		internal CaFileDialogBase Dialog { get {return _dlg;} set {_dlg = value;} }

        internal string[] TabCaptions
        {
            get
            {
                return new string[]{ tabControl1.TabPages[0].Text, tabControl1.TabPages[1].Text };
            }
            set
            {
                if (value.Length != 2)
                {
                    throw new ArgumentOutOfRangeException("Length", "Invalid length of tab captions array.");
                }

                tabControl1.TabPages[0].Text = value[0];
                tabControl1.TabPages[1].Text = value[1];
            }
        }

	}
}
