using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ComponentAge.Dialogs
{
   public partial class AboutForm : Form
   {
      public AboutForm()
      {
         InitializeComponent();
      }

      private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         System.Diagnostics.Process.Start("http://www.componentage.com");
      }

      private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         System.Diagnostics.Process.Start("mailto:techsupport@componentage.com");
      }

      private void button1_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.OK;
      }
   }
}
