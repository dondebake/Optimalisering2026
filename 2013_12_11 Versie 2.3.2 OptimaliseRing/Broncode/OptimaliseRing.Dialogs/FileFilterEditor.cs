using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace ComponentAge.Dialogs
{
	internal class FileFilterEditor : UITypeEditor
	{
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			CaFileDialogBase dlg = context.Instance as CaFileDialogBase;
			FilterEditorForm form = new FilterEditorForm();
			form.Filter = dlg.Filter;
			if (form.ShowDialog() == DialogResult.OK)
				return form.Filter;
			return dlg.Filter;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
	}
}
