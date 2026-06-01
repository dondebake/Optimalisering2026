using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace ComponentAge.Dialogs.Design
{
    public class DockedFormPropertyEditor : ComponentAge.Dialogs.Design.DropDownPropertyEditorBase
    {
        // member fields
        private ITypeDescriptorContext _context = null;
        private IServiceProvider _provider = null;

        // constructor
        public DockedFormPropertyEditor()
            : base()
        {
        }

        // overrides
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (!ContextIsOk(context, provider))
                return value;
            _context = context;
            _provider = provider;
            FillListControl();
            return ShowDropDownControl(value);
        }

        protected internal override void FillListControl()
        {
            base.FillListControl();

            AddListItem("", "(none)");

            string[] names = DesignTimeServices.GetOpenedForms(_provider);

            for (int i = 0; i < names.Length; i++)
            {
                AddListItem(names[i]);
            }
        }
    }
}
