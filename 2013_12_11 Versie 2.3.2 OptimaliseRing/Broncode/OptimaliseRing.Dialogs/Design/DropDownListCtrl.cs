using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ComponentAge.Dialogs.Design
{
    [ToolboxItem(false)]
	public class DropDownListCtrl : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

      /// <summary>
      /// Initializes a new instance of the <see cref="DropDownListCtrl"/> class.
      /// </summary>
		public DropDownListCtrl()
		{
			// This call is required by the Windows.Forms DevExpress.XtraEditors.XtraForm Designer.
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

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.listItems = new System.Windows.Forms.ListBox();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            //
            // listItems
            //
            this.listItems.BackColor = System.Drawing.SystemColors.Window;
            this.listItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listItems.HorizontalScrollbar = true;
            this.listItems.IntegralHeight = false;
            this.listItems.Name = "listItems";
            this.listItems.Size = new System.Drawing.Size(150, 150);
            this.listItems.TabIndex = 0;
            this.listItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listItems_KeyDown);
            this.listItems.Click += new System.EventHandler(this.listItems_Click);
            //
            // columnHeader1
            //
            this.columnHeader1.Width = 146;
            //
            // DropDownListCtrl
            //
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.listItems});
            this.Name = "DropDownListCtrl";
            this.ResumeLayout(false);

        }
		#endregion

        // member fields
        internal System.Windows.Forms.ListBox listItems;
		private IWindowsFormsEditorService _service;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private DropDownPropertyEditorBase _editor;

        // member functions
        private void CloseEditor ()
        {
            if (_service != null)
                _service.CloseDropDown();
        }

        // event handlers
        private void listItems_Click(object sender, System.EventArgs e)
        {
            if (listItems.SelectedItems.Count == 1)
                _editor.OnSelectItem(((ListPair)listItems.SelectedItems[0]).Value, ref _editor._originalValue);
            CloseEditor();
        }

        private void listItems_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                if (listItems.SelectedItems.Count == 1)
                    _editor.OnSelectItem(((ListPair)listItems.SelectedItems[0]).Value, ref _editor._originalValue);
            CloseEditor();
        }

        // properties
        internal IWindowsFormsEditorService Service {get{return _service;} set{_service = value;}}
        internal DropDownPropertyEditorBase Editor {get{return _editor;} set{_editor = value;}}
	}
}
