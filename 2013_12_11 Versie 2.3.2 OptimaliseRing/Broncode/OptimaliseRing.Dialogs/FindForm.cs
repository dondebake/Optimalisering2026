using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ComponentAge.Dialogs
{
	/// <summary>
	/// Summary description for FindForm.
	/// </summary>
	public class FindForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.CheckBox checkMatchCase;
		private System.Windows.Forms.CheckBox checkMatchWord;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.ComponentModel.IContainer components;

		public FindForm()
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FindForm));
			this.label1 = new System.Windows.Forms.Label();
			this.checkMatchCase = new System.Windows.Forms.CheckBox();
			this.checkMatchWord = new System.Windows.Forms.CheckBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.button5 = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			//
			// label1
			//
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.TabIndex = 0;
			this.label1.Text = "Fi&nd what:";
			//
			// checkMatchCase
			//
			this.checkMatchCase.Location = new System.Drawing.Point(8, 64);
			this.checkMatchCase.Name = "checkMatchCase";
			this.checkMatchCase.TabIndex = 2;
			this.checkMatchCase.Text = "Match &case";
			//
			// checkMatchWord
			//
			this.checkMatchWord.Location = new System.Drawing.Point(8, 88);
			this.checkMatchWord.Name = "checkMatchWord";
			this.checkMatchWord.Size = new System.Drawing.Size(128, 24);
			this.checkMatchWord.TabIndex = 3;
			this.checkMatchWord.Text = "Match &whole word";
			//
			// button1
			//
			this.button1.Location = new System.Drawing.Point(416, 8);
			this.button1.Name = "button1";
			this.button1.TabIndex = 6;
			this.button1.Text = "&Find Next";
			//
			// button2
			//
			this.button2.Image = ((System.Drawing.Bitmap)(resources.GetObject("button2.Image")));
			this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.button2.ImageIndex = 0;
			this.button2.ImageList = this.imageList1;
			this.button2.Location = new System.Drawing.Point(416, 32);
			this.button2.Name = "button2";
			this.button2.TabIndex = 7;
			this.button2.Text = "&Replace";
			//
			// imageList1
			//
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(10, 10);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Silver;
			//
			// button3
			//
			this.button3.Location = new System.Drawing.Point(416, 56);
			this.button3.Name = "button3";
			this.button3.TabIndex = 8;
			this.button3.Text = "Replace &All";
			//
			// button4
			//
			this.button4.Location = new System.Drawing.Point(416, 88);
			this.button4.Name = "button4";
			this.button4.TabIndex = 9;
			this.button4.Text = "Close";
			//
			// comboBox1
			//
			this.comboBox1.Location = new System.Drawing.Point(96, 8);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(312, 21);
			this.comboBox1.TabIndex = 10;
			this.comboBox1.Text = "comboBox1";
			//
			// button5
			//
			this.button5.Location = new System.Drawing.Point(416, 120);
			this.button5.Name = "button5";
			this.button5.TabIndex = 11;
			this.button5.Text = "Help";
			//
			// label2
			//
			this.label2.Location = new System.Drawing.Point(8, 40);
			this.label2.Name = "label2";
			this.label2.TabIndex = 15;
			this.label2.Text = "Re&place with:";
			//
			// comboBox2
			//
			this.comboBox2.Location = new System.Drawing.Point(96, 40);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(312, 21);
			this.comboBox2.TabIndex = 16;
			this.comboBox2.Text = "comboBox2";
			//
			// groupBox1
			//
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.radioButton2,
																					this.radioButton1});
			this.groupBox1.Location = new System.Drawing.Point(160, 72);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(240, 64);
			this.groupBox1.TabIndex = 17;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Direction";
			//
			// radioButton1
			//
			this.radioButton1.Location = new System.Drawing.Point(16, 24);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.TabIndex = 0;
			this.radioButton1.Text = "&Up";
			//
			// radioButton2
			//
			this.radioButton2.Location = new System.Drawing.Point(128, 24);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.TabIndex = 1;
			this.radioButton2.Text = "&Down";
			//
			// FindForm
			//
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(498, 150);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox1,
																		  this.comboBox2,
																		  this.label2,
																		  this.button5,
																		  this.comboBox1,
																		  this.button4,
																		  this.button3,
																		  this.button2,
																		  this.button1,
																		  this.checkMatchWord,
																		  this.checkMatchCase,
																		  this.label1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "FindForm";
			this.Text = "Find";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
