using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ComponentAge.Dialogs
{
	/// <summary>
	/// Summary description for RecentListCtrl.
	/// </summary>
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ToolboxItem(false)]
	public class RecentListCtrl : System.Windows.Forms.UserControl
	{
		internal System.Windows.Forms.ListView listList;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		CaFileDialogBase _dlg = null;

		public RecentListCtrl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call

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

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.listList = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            //
            // listList
            //
            this.listList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                       this.columnHeader1,
                                                                                       this.columnHeader2});
            this.listList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listList.FullRowSelect = true;
            this.listList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listList.Name = "listList";
            this.listList.Size = new System.Drawing.Size(400, 264);
            this.listList.TabIndex = 0;
            this.listList.View = System.Windows.Forms.View.Details;
            this.listList.DoubleClick += new System.EventHandler(this.listList_DoubleClick);
            this.listList.SelectedIndexChanged += new System.EventHandler(this.listList_SelectedIndexChanged);
            //
            // columnHeader1
            //
            this.columnHeader1.Text = "File";
            this.columnHeader1.Width = 130;
            //
            // columnHeader2
            //
            this.columnHeader2.Text = "Folder";
            this.columnHeader2.Width = 300;
            //
            // RecentListCtrl
            //
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.listList});
            this.Name = "RecentListCtrl";
            this.Size = new System.Drawing.Size(400, 264);
            this.ResumeLayout(false);

        }
		#endregion

		private void listList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (listList.SelectedItems.Count == 0)
				return;
			System.IO.FileInfo fi = listList.SelectedItems[0].Tag as System.IO.FileInfo;
            if (fi != null)
			    Dialog.FilenameText = fi.FullName;
            Dialog.OnRecentSelectionChanged();
		}

        private void listList_DoubleClick(object sender, System.EventArgs e)
        {
            if (listList.SelectedItems.Count > 0)
            {
                Dialog.CloseDialog(DialogResult.OK);
            }
        }

		// properties
		public CaFileDialogBase Dialog { get {return _dlg;} set {_dlg = value;} }
	}
}
