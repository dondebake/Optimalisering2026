//---------------------------------------------------------------------------
// Dialog Workshop for .NET Framework
// Copyright (c) 2002 - 2003, COMPONENTAGE Software,
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
using System.Windows.Forms;
using System.Diagnostics;

namespace ComponentAge.Dialogs
{
	/// <summary>
	/// Summary description for DesignerFormEx.
	/// </summary>
	internal class DesignerFormEx : System.Windows.Forms.Form
	{
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.LinkLabel linkLabel2;
		private System.Windows.Forms.Label label5;
        private PictureBox pictureBox1;
        private Label label3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DesignerFormEx()
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DesignerFormEx));
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // button2
            //
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(423, 269);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Continue";
            //
            // label1
            //
            this.label1.Location = new System.Drawing.Point(29, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(453, 56);
            this.label1.TabIndex = 2;
            this.label1.Text = "This is fully-functional demo version of Dialog Workshop .NET. \r\nYou may legally " +
                "use it during 30 days before purchasing the licensed version. \r\nPlease visit our" +
                " homepage for additional information.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // linkLabel1
            //
            this.linkLabel1.Location = new System.Drawing.Point(144, 203);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(222, 23);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://www.componentage.com";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            //
            // label2
            //
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(86, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(320, 23);
            this.label2.TabIndex = 4;
            this.label2.Text = "Dialog Workshop .NET is not freeware!";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // linkLabel2
            //
            this.linkLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.linkLabel2.Location = new System.Drawing.Point(163, 226);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(184, 23);
            this.linkLabel2.TabIndex = 7;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "Order right now!";
            this.linkLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            //
            // label5
            //
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.ForeColor = System.Drawing.Color.DarkOrange;
            this.label5.Location = new System.Drawing.Point(87, 157);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(336, 32);
            this.label5.TabIndex = 8;
            this.label5.Text = "Install the registered version after purchasing and recompile this application to" +
                " remove this nag screen.";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // pictureBox1
            //
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(51, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(409, 62);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 269);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(262, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Copyright (c) 2001-2006, COMPONENTAGE Software";
            //
            // DesignerFormEx
            //
            this.AcceptButton = this.button2;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(510, 300);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DesignerFormEx";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dialog Workshop .NET - Not Registered";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		#endregion

        private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.componentage.com");
        }

		private void linkLabel2_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
            Process.Start(@"http://shareit1.element5.com/product.html?productid=160986&languageid=1&backlink=http%3A%2F%2Fwww.componentage.com&currencies=USD");
		}
	}
}
