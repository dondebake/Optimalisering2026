namespace ComponentAge.Dialogs
{
   partial class AboutForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         this.linkLabel2 = new System.Windows.Forms.LinkLabel();
         this.label4 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.linkLabel1 = new System.Windows.Forms.LinkLabel();
         this.label3 = new System.Windows.Forms.Label();
         this.labelVer = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.button1 = new System.Windows.Forms.Button();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.SuspendLayout();
         //
         // pictureBox1
         //
         this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
         this.pictureBox1.Location = new System.Drawing.Point(31, 2);
         this.pictureBox1.Name = "pictureBox1";
         this.pictureBox1.Size = new System.Drawing.Size(401, 65);
         this.pictureBox1.TabIndex = 18;
         this.pictureBox1.TabStop = false;
         //
         // linkLabel2
         //
         this.linkLabel2.Location = new System.Drawing.Point(123, 198);
         this.linkLabel2.Name = "linkLabel2";
         this.linkLabel2.Size = new System.Drawing.Size(184, 23);
         this.linkLabel2.TabIndex = 17;
         this.linkLabel2.TabStop = true;
         this.linkLabel2.Text = "techsupport@componentage.com";
         this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
         //
         // label4
         //
         this.label4.Location = new System.Drawing.Point(3, 174);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(100, 23);
         this.label4.TabIndex = 16;
         this.label4.Text = "Homepage:";
         //
         // label2
         //
         this.label2.Location = new System.Drawing.Point(3, 198);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(112, 23);
         this.label2.TabIndex = 15;
         this.label2.Text = "Technical support: ";
         //
         // linkLabel1
         //
         this.linkLabel1.Location = new System.Drawing.Point(123, 174);
         this.linkLabel1.Name = "linkLabel1";
         this.linkLabel1.Size = new System.Drawing.Size(176, 23);
         this.linkLabel1.TabIndex = 13;
         this.linkLabel1.TabStop = true;
         this.linkLabel1.Text = "http://www.componentage.com";
         this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
         //
         // label3
         //
         this.label3.Location = new System.Drawing.Point(91, 142);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(280, 23);
         this.label3.TabIndex = 12;
         this.label3.Text = "Copyright (c) 2001-2006, COMPONENTAGE Software";
         this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         //
         // labelVer
         //
         this.labelVer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
         this.labelVer.Location = new System.Drawing.Point(181, 110);
         this.labelVer.Name = "labelVer";
         this.labelVer.Size = new System.Drawing.Size(100, 23);
         this.labelVer.TabIndex = 11;
         this.labelVer.Text = "Version 1.3";
         this.labelVer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         //
         // label1
         //
         this.label1.Font = new System.Drawing.Font("Haettenschweiler", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
         this.label1.ForeColor = System.Drawing.Color.Black;
         this.label1.Location = new System.Drawing.Point(7, 70);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(448, 40);
         this.label1.TabIndex = 10;
         this.label1.Text = "Dialog Workshop .NET";
         this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         //
         // button1
         //
         this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.button1.Location = new System.Drawing.Point(194, 230);
         this.button1.Name = "button1";
         this.button1.Size = new System.Drawing.Size(75, 23);
         this.button1.TabIndex = 14;
         this.button1.Text = "OK";
         this.button1.Click += new System.EventHandler(this.button1_Click);
         //
         // AboutForm
         //
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(468, 265);
         this.Controls.Add(this.pictureBox1);
         this.Controls.Add(this.linkLabel2);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.button1);
         this.Controls.Add(this.linkLabel1);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.labelVer);
         this.Controls.Add(this.label1);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "AboutForm";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "About Dialog Workshop .NET";
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.PictureBox pictureBox1;
      private System.Windows.Forms.LinkLabel linkLabel2;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.LinkLabel linkLabel1;
      private System.Windows.Forms.Label label3;
      public System.Windows.Forms.Label labelVer;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Button button1;
   }
}
