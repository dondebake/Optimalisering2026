using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Security.Permissions;

namespace ComponentAge.Dialogs.Design
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ListPair
    {
        // member fields
        private object _value;
        private string _stringVal = "";

        // constructor
        public  ListPair(object value, string strVal)
        {
            _value = value;
            _stringVal = strVal;
        }

        // methods
        public override string ToString()
        {
            return _stringVal;
        }

        // properties
        public object Value { get {return _value;} }
        public string StringValue { get {return _stringVal;} }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
	public class DropDownPropertyEditorBase : System.Drawing.Design.UITypeEditor
	{
        // member fields
        private IWindowsFormsEditorService _edSvc = null;
        private DropDownListCtrl _listControl = new DropDownListCtrl();
        internal object _originalValue;
        private ArrayList _listSource = new ArrayList();

        // constructor
		public DropDownPropertyEditorBase ()
		{
            _listControl.Editor = this;
		}

        // overrides
        [PermissionSetAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public override UITypeEditorEditStyle GetEditStyle (ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
                return UITypeEditorEditStyle.DropDown;
            return base.GetEditStyle(context);
        }

        // methods
        internal bool ContextIsOk (ITypeDescriptorContext context, IServiceProvider provider)
        {
            if (context != null
                && context.Instance != null
                && provider != null)
            {
                _edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                _listControl.Service = _edSvc;
                return true;
            }
            return false;
        }
        internal object ShowDropDownControl (object prevValue)
        {
            return ShowDropDownControl(prevValue, prevValue.ToString());
        }
        internal object ShowDropDownControl (object prevValue, string stringValue)
        {
            _listControl.listItems.DataSource = _listSource;
            _listControl.listItems.ValueMember = "Value";
            _listControl.listItems.DisplayMember = "StringValue";
            _originalValue = prevValue;
            for (int i = 0; i < _listSource.Count; i++ )
            {
                if (((ListPair)_listSource[i]).StringValue == stringValue)
                {
                    _listControl.listItems.SelectedIndex = i;
                    break;
                }
            }
            _edSvc.DropDownControl(_listControl);
            return _originalValue;
        }
        protected internal virtual void OnSelectItem (object selection, ref object value)
        {
            value = selection;
        }
        protected internal virtual void FillListControl ()
        {
            // descedants should override this member
            _listControl.listItems.DataSource = null;
            _listControl.listItems.ValueMember = "";
            _listControl.listItems.DisplayMember = "";
            _listSource.Clear();
        }
        internal void AddListItem (object value)
        {
            AddListItem(value, value != null ? value.ToString() : "");
        }
        internal void AddListItem (object value, string stringVal)
        {
            _listSource.Add(new ListPair(value, stringVal));
        }
	}
}
