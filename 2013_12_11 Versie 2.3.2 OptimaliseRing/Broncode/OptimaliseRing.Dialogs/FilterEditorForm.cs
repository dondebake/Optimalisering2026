using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ComponentAge.Dialogs
{
	internal class FilterEditorForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.DataGrid dataGrid1;
		private System.Windows.Forms.Label label1;
		private System.Data.DataSet dataSet1;
		private System.Data.DataTable dataTable1;
		private System.Data.DataColumn dataColumn1;
		private System.Data.DataColumn dataColumn2;
		private System.Windows.Forms.DataGridTableStyle dataGridTableStyle1;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn1;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FilterEditorForm()
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
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.dataGrid1 = new System.Windows.Forms.DataGrid();
			this.dataSet1 = new System.Data.DataSet();
			this.dataTable1 = new System.Data.DataTable();
			this.dataColumn1 = new System.Data.DataColumn();
			this.dataColumn2 = new System.Data.DataColumn();
			this.dataGridTableStyle1 = new System.Windows.Forms.DataGridTableStyle();
			this.dataGridTextBoxColumn1 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn2 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
			this.SuspendLayout();
			//
			// button1
			//
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point(256, 192);
			this.button1.Name = "button1";
			this.button1.TabIndex = 0;
			this.button1.Text = "Cancel";
			//
			// button2
			//
			this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button2.Location = new System.Drawing.Point(176, 192);
			this.button2.Name = "button2";
			this.button2.TabIndex = 1;
			this.button2.Text = "OK";
			//
			// dataGrid1
			//
			this.dataGrid1.AllowSorting = false;
			this.dataGrid1.DataMember = "Table1";
			this.dataGrid1.DataSource = this.dataSet1;
			this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid1.Location = new System.Drawing.Point(8, 24);
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.Size = new System.Drawing.Size(328, 160);
			this.dataGrid1.TabIndex = 2;
			this.dataGrid1.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
																								  this.dataGridTableStyle1});
			//
			// dataSet1
			//
			this.dataSet1.DataSetName = "NewDataSet";
			this.dataSet1.Locale = new System.Globalization.CultureInfo("en-US");
			this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
																		  this.dataTable1});
			//
			// dataTable1
			//
			this.dataTable1.Columns.AddRange(new System.Data.DataColumn[] {
																			  this.dataColumn1,
																			  this.dataColumn2});
			this.dataTable1.TableName = "Table1";
			//
			// dataColumn1
			//
			this.dataColumn1.ColumnName = "Filter name";
			//
			// dataColumn2
			//
			this.dataColumn2.ColumnName = "Filter";
			//
			// dataGridTableStyle1
			//
			this.dataGridTableStyle1.AllowSorting = false;
			this.dataGridTableStyle1.DataGrid = this.dataGrid1;
			this.dataGridTableStyle1.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
																												  this.dataGridTextBoxColumn1,
																												  this.dataGridTextBoxColumn2});
			this.dataGridTableStyle1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGridTableStyle1.MappingName = "Table1";
			//
			// dataGridTextBoxColumn1
			//
			this.dataGridTextBoxColumn1.Format = "";
			this.dataGridTextBoxColumn1.FormatInfo = null;
			this.dataGridTextBoxColumn1.HeaderText = "Filter name";
			this.dataGridTextBoxColumn1.MappingName = "Filter name";
			this.dataGridTextBoxColumn1.Width = 175;
			//
			// dataGridTextBoxColumn2
			//
			this.dataGridTextBoxColumn2.Format = "";
			this.dataGridTextBoxColumn2.FormatInfo = null;
			this.dataGridTextBoxColumn2.HeaderText = "Filter";
			this.dataGridTextBoxColumn2.MappingName = "Filter";
			this.dataGridTextBoxColumn2.Width = 75;
			//
			// label1
			//
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.TabIndex = 3;
			this.label1.Text = "Filters:";
			//
			// FilterEditorForm
			//
			this.AcceptButton = this.button2;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.button1;
			this.ClientSize = new System.Drawing.Size(344, 227);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.dataGrid1,
																		  this.label1,
																		  this.button2,
																		  this.button1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FilterEditorForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Filter Editor";
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		// properties
		public string Filter
		{
			get
			{
				string s = "";
				for (int i = 0; i < dataTable1.Rows.Count; i++)
					s += dataTable1.Rows[i][0].ToString() + "|" + dataTable1.Rows[i][1].ToString() + "|";
				if (s.Length > 0)
					s = s.Substring(0, s.Length-1);
				return s;
			}
			set
			{
				int beginPos = 0;
				int endPos = value.IndexOf("|");
				System.Collections.Specialized.StringCollection strings = new System.Collections.Specialized.StringCollection();
				while (endPos > -1)
				{
					strings.Add(value.Substring(beginPos, endPos - beginPos));
					beginPos = endPos + 1;
					endPos = value.IndexOf("|", beginPos + 1);
				}
				if (beginPos > 0)
				{
					strings.Add(value.Substring(beginPos));
				}
				dataTable1.Rows.Clear();
				for (int i = 0; i < strings.Count / 2; i++)
				{
					dataTable1.Rows.Add(new object[]{strings[i * 2], strings[i * 2 + 1]});
				}
			}
		}
	}
}
