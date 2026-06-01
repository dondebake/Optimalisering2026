//---------------------------------------------------------------------------
// Dialog Workshop for .NET Framework
// Copyright (c) COMPONENTAGE Software,
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
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace ComponentAge.Dialogs
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class CustDlgParamsEditor : UITypeEditor
	{
		public CustDlgParamsEditor()
		{
        }

        public override object EditValue (ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            DialogDesignerForm form = new DialogDesignerForm();
            CaCommonDialog dlg = (CaCommonDialog)context.Instance;
            form.Instance = dlg;
            if (form.ShowDialog() == DialogResult.OK)
                context.OnComponentChanged();
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle (ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
	}
}
