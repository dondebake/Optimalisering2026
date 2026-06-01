//---------------------------------------------------------------------------
// Dialog Workshop for .NET Framework
// Copyright (c) 2002 - 2004, COMPONENTAGE Software,
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
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Collections;

using ComponentAge.Dialogs.Design;
using ComponentAge.PlatformServices;

namespace ComponentAge.Dialogs
{
	/// <summary>
	/// Summary description for caCustDlgParams.
	/// </summary>
    [ToolboxItem(false)]
    [Description(DialogConsts.strCaCustDlgParams_TypeDesc)]
    public class CaCustDlgParams : Component
    {
        // member fields
        Form _topForm;
        Form _leftForm;
        Form _rightForm;
        Form _bottomForm;
        string _leftClassName = "";
        string _topClassName = "";
        string _rightClassName = "";
        string _bottomClassName = "";
        CaDlgPosParams _posParams = new CaDlgPosParams();
        internal int IncX = 0;
        internal int IncY = 0;
        bool _autoCreate = true;
        DialogPosition _position = DialogPosition.Default;
        CommonDialog _dialog;
        Form _leftDTForm;
        Form _topDTForm;
        Form _rightDTForm;
        Form _bottomDTForm;
        Form _owner;
        System.Drawing.Icon _icon;
        internal Win32.RECT _selRect;
        internal Win32.RECT _dlgRect;

        // constructor
        [EditorBrowsable(EditorBrowsableState.Never)]
        public CaCustDlgParams(CommonDialog dlg) :
            base()
        {
            _dialog = dlg;
        }

        // overrides
        public override string ToString ()
        {
            return "[Extended dialog properties]";
        }

        // methods
        internal void OnInitDialog ()
        {
            IntPtr h = GetDialogHandle();
            Win32.GetClientRect(h, ref _dlgRect);
            Debug.Assert(h != IntPtr.Zero);  // do not allow to init if Handle was not assigned in descedants
            PropertyInfo prop = this.Dialog.GetType().GetProperty("DlgItemsCaptions");
            CaDlgItemCaptions captions = null;
            if (prop != null)
                captions = prop.GetValue(this.Dialog, new object[] {}) as CaDlgItemCaptions;
            if (captions != null)
                captions.SetCaptions(h);
            ConnectForms(h);
            SetInitialDialogPosition();
            string title = GetDialogTitle();
            if (title != "" && title != null && h != IntPtr.Zero)
                Win32.SetWindowText(h, title);
        }
        internal string GetDialogTitle ()
        {
            Type t = _dialog.GetType();
            PropertyInfo i = t.GetProperty("Title");
            object o = i.GetValue(_dialog, new object[] {});
            return (string)o;
        }
        internal void SetDialogTitle (string val)
        {
            Type t = _dialog.GetType();
            PropertyInfo i = t.GetProperty("Title");
            i.SetValue(_dialog, val, new object[] {});
        }
        internal Form GetDialogOwner ()
        {
            if (IsDesignMode)
                return null;
            return this.OwnerForm;
        }
        internal IntPtr GetDialogHandle ()
        {
            Type t = this._dialog.GetType();
            PropertyInfo i = t.GetProperty("Handle");
            object o = i.GetValue(_dialog, new object[] {});
            return (IntPtr)o;
        }
        internal void OnHide ()
        {
            if (IsDesignMode)
            {
                if (_topDTForm != null)
                {
                    TopForm = null;
                    _topDTForm.Dispose();
                }
                if (_leftDTForm != null)
                {
                    LeftForm = null;
                    _leftDTForm.Dispose();
                }
                if (_rightDTForm != null)
                {
                    RightForm = null;
                    _rightDTForm.Dispose();
                }
                if (_bottomDTForm != null)
                {
                    BottomForm = null;
                    _bottomDTForm.Dispose();
                }
            }
            else
            {
                if (TopForm != null)
                {
                    TopForm.Visible = false;
                    TopForm.Parent = null;
                }
                if (LeftForm != null)
                {
                    LeftForm.Visible = false;
                    LeftForm.Parent = null;
                }
                if (RightForm != null)
                {
                    RightForm.Visible = false;
                    RightForm.Parent = null;
                }
                if (BottomForm != null)
                {
                    BottomForm.Visible = false;
                    BottomForm.Parent = null;
                }
            }
        }
        internal void Reset ()
        {
            AutoCreateDockedForms = true;
            LeftForm = null;
            LeftFormClassName = "";
            TopForm = null;
            TopFormClassName = "";
            RightForm = null;
            RightFormClassName = "";
            BottomForm = null;
            BottomFormClassName = "";
            StartPosition = DialogPosition.Default;
            PosParams.FitToScreen = true;
            //PosParams.ShiftX = 0;
            //PosParams.ShiftY = 0;
        }
        internal bool TrackContextMenus (int x, int y)
        {
            System.Windows.Forms.Control c = WinFormsServices.FindControlAt(x, y);
            if (c != null && c.ContextMenu != null)
                return true;
            return false;
        }
        internal void SetInitialDialogPosition ()
        {
            Win32.RECT r = new Win32.RECT(0, 0, 0, 0);
            Win32.RECT r1 = new Win32.RECT(0, 0, 0, 0);
            Win32.GetWindowRect(GetDialogHandle(), ref r);
			if (StartPosition == DialogPosition.ScreenCenter)
				WinFormsServices.CenterWindow(GetDialogHandle());
			else if (StartPosition == DialogPosition.MainFormCenter && !IsDesignMode)
				WinFormsServices.CenterWindow(GetDialogHandle(), Process.GetCurrentProcess().MainWindowHandle);
			else if (StartPosition == DialogPosition.OwnerFormSenter && !IsDesignMode)
			{
				if (GetDialogOwner() != null)
					WinFormsServices.CenterWindow(GetDialogHandle(), GetDialogOwner().Handle);
				else
					WinFormsServices.CenterWindow(GetDialogHandle());
			}
			else if (StartPosition == DialogPosition.Custom && !IsDesignMode)
			{
                Win32.SetWindowPos(GetDialogHandle(), Win32.HWND_TOP, PosParams.CustomLocation.X, PosParams.CustomLocation.Y, 0, 0, Win32.SWP_NOSIZE | Win32.SWP_NOZORDER/* | Win32.SWP_NOACTIVATE*/);
			}
            if (PosParams.FitToScreen)
            {
                Win32.GetWindowRect(GetDialogHandle(), ref r);
                WinFormsServices.FitRectToScreen(ref r);
                Win32.SetWindowPos(GetDialogHandle(), Win32.HWND_TOP, r.left, r.top, 0, 0, Win32.SWP_NOSIZE | Win32.SWP_NOZORDER/* | Win32.SWP_NOACTIVATE*/);
            }
            // setting icon here
            if (_icon != null)
                Win32.SendMessage(GetDialogHandle(), Win32.WM_SETICON, IntPtr.Zero/*ICON_SMALL*/, _icon.Handle);
        }
        internal void OnResizeWindow (IntPtr wparam, IntPtr lparam)
        {
            int topw = 0;
            int lefth = 0;
            int righth = 0;
            if (TopForm != null)
            {
                topw = Win32.LOWORD(lparam);
                if (LeftForm != null)
                    topw = topw - LeftForm.Width;
            }
            if (RightForm != null)
            {
                if (TopForm != null)
                    topw = topw - RightForm.Width;
                righth = Win32.HIWORD(lparam);
            }
            if (LeftForm != null)
            {
                lefth = Win32.HIWORD(lparam);
            }
            if (BottomForm != null)
            {
                if (RightForm != null)
                    righth = righth - BottomForm.Height;
                if (LeftForm != null)
                    lefth = lefth - BottomForm.Height;
                BottomForm.Width = Win32.LOWORD(lparam);
            }
            if (TopForm != null)
                TopForm.Width = topw;
            if (LeftForm != null)
                LeftForm.Height = lefth;
            if (RightForm != null)
                RightForm.Height = righth;
            _selRect.bottom = _selRect.bottom + (Win32.HIWORD(lparam) - _dlgRect.bottom);
            _selRect.right = _selRect.right + (Win32.LOWORD(lparam) - _dlgRect.right);
            _dlgRect.bottom = Win32.HIWORD(lparam);
            _dlgRect.right = Win32.LOWORD(lparam);
        }

        internal Form GetDesignTimeClassInstance (string name)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            ISite site = this.Dialog.Site;
            if (site == null)
                return null;
            IDesignerEventService service = site.GetService(typeof(IDesignerEventService)) as IDesignerEventService;
            IDesignerHost hostservice = site.GetService(typeof(IDesignerHost)) as IDesignerHost;
            int idx = hostservice.RootComponentClassName.LastIndexOf('.');
            string namespaceName = "";
            if (idx > -1)
                namespaceName = hostservice.RootComponentClassName.Substring(0, idx);
            if (service == null)
                return null;
            ArrayList items = new ArrayList();
            foreach (IDesignerHost host in service.Designers)
            {
                if (host.RootComponentClassName == null || host.RootComponentClassName == "")
                    continue;

                if (host.RootComponentClassName == hostservice.RootComponentClassName)
                    continue;

                int idx1 = host.RootComponentClassName.LastIndexOf('.');
                string namespaceName1 = host.RootComponentClassName.Substring(0, idx1);
                if (namespaceName != namespaceName1)
                    continue;

                IContainer container = host.Container;
                if (container != null)
                {
                    string formName = "";
                    for (int i = 0; i < container.Components.Count; i++)
                    {
                        IComponent comp = container.Components[i];
                        if (comp is Form && comp.Site != null)
                        {
                            formName = namespaceName + "." + comp.Site.Name;
                            if (formName.Equals(name))
                            {
                                Form ori = comp as Form;
                                Form f1 = new Form();
                                f1.FormBorderStyle = FormBorderStyle.None;
                                f1.Width = ori.Width;
                                f1.Height = ori.Height;
                                try
                                {
                                    for (int j = 0; j < ori.Controls.Count; j++ )
                                    {
                                        CaCommonDialog.CreateControlCopy(ori.Controls[j], f1);
                                    }
                                    for (int j = 0; j < ori.Controls.Count; j++ )
                                        CaCommonDialog.SetControlProperties(ori.Controls[j], f1);
                                }
                                catch
                                {
                                    Label l = new Label();
                                    l.Parent = f1;
                                    l.Dock = DockStyle.Fill;
                                    l.Text = "Docked form can not be instantiated correctly in design-time. Only width and height properties have correct values.";
                                    l.BringToFront();
                                }
                                return f1;
                            }
                        }
                    }
                }
            }
            return null;
        }

        internal bool TopFormSupported ()
        {
            object[] attrs = Dialog.GetType().GetCustomAttributes(typeof(TopFormSupportAttribute), true);
            for (int i = 0; i < attrs.Length; i++ )
            {
                if (attrs[i] is TopFormSupportAttribute)
                {
                    TopFormSupportAttribute at = (TopFormSupportAttribute)attrs[i];
                    return at.Supported;
                }
            }
            return true;
        }
        internal bool LeftFormSupported ()
        {
            object[] attrs = Dialog.GetType().GetCustomAttributes(typeof(LeftFormSupportAttribute), true);
            for (int i = 0; i < attrs.Length; i++ )
            {
                if (attrs[i] is LeftFormSupportAttribute)
                {
                    LeftFormSupportAttribute at = (LeftFormSupportAttribute)attrs[i];
                    return at.Supported;
                }
            }
            return true;
        }
        internal bool RightFormSupported ()
        {
            object[] attrs = Dialog.GetType().GetCustomAttributes(typeof(RightFormSupportAttribute), true);
            for (int i = 0; i < attrs.Length; i++ )
            {
                if (attrs[i] is RightFormSupportAttribute)
                {
                    RightFormSupportAttribute at = (RightFormSupportAttribute)attrs[i];
                    return at.Supported;
                }
            }
            return true;
        }
        internal bool BottomFormSupported ()
        {
            object[] attrs = Dialog.GetType().GetCustomAttributes(typeof(BottomFormSupportAttribute), true);
            for (int i = 0; i < attrs.Length; i++ )
            {
                if (attrs[i] is BottomFormSupportAttribute)
                {
                    BottomFormSupportAttribute at = (BottomFormSupportAttribute)attrs[i];
                    return at.Supported;
                }
            }
            return true;
        }

        internal void ConnectForms (IntPtr Handle)
        {
            if (AutoCreateDockedForms)
            {
                Assembly asm = Assembly.GetEntryAssembly();
                if (asm == null)
                    asm = Assembly.GetExecutingAssembly();
                if (asm != null)
                {
                    if (   ((LeftForm == null && LeftFormClassName != "")
                        || (LeftForm != null && !LeftFormClassName.Equals(LeftForm.GetType().FullName)) && LeftFormClassName != "")
                        &&
                        LeftFormSupported()
                        )
                    {
                        object o = asm.CreateInstance(LeftFormClassName);
                        if (o == null && IsDesignMode)
                        {
                            o = GetDesignTimeClassInstance(LeftFormClassName);
                            _leftDTForm = o as Form;
                        }
                        if (o is Form)
                        {
                            ((Form)o).Visible = false;
                            ((Form)o).FormBorderStyle = FormBorderStyle.None;
                            LeftForm = (Form)o;
                        }
                    }
                    if (   ((RightForm == null && RightFormClassName != "")
                        || (RightForm != null && !RightFormClassName.Equals(RightForm.GetType().FullName)) && RightFormClassName != "")
                        &&
                        RightFormSupported()
                        )
                    {
                        object o = asm.CreateInstance(RightFormClassName);
                        if (o == null && IsDesignMode)
                        {
                            o = GetDesignTimeClassInstance(RightFormClassName);
                            _rightDTForm = o as Form;
                        }
                        if (o is Form)
                        {
                            ((Form)o).Visible = false;
                            ((Form)o).FormBorderStyle = FormBorderStyle.None;
                            RightForm = (Form)o;
                        }
                    }
                    if (   ((TopForm == null && TopFormClassName != "")
                        || (TopForm != null && !TopFormClassName.Equals(TopForm.GetType().FullName)) && TopFormClassName != "")
                        &&
                        TopFormSupported()
                        )
                    {
                        object o = asm.CreateInstance(TopFormClassName);
                        if (o == null && IsDesignMode)
                        {
                            o = GetDesignTimeClassInstance(TopFormClassName);
                            _topDTForm = o as Form;
                        }
                        if (o is Form)
                        {
                            ((Form)o).Visible = false;
                            ((Form)o).FormBorderStyle = FormBorderStyle.None;
                            TopForm = (Form)o;
                        }
                    }
                    if (   ((BottomForm == null && BottomFormClassName != "")
                        || (BottomForm != null && !BottomFormClassName.Equals(BottomForm.GetType().FullName)) && BottomFormClassName != "")
                        &&
                        BottomFormSupported()
                        )
                    {
                        object o = asm.CreateInstance(BottomFormClassName);
                        if (o == null && IsDesignMode)
                        {
                            o = GetDesignTimeClassInstance(BottomFormClassName);
                            _bottomDTForm = o as Form;
                        }
                        if (o is Form)
                        {
                            ((Form)o).Visible = false;
                            ((Form)o).FormBorderStyle = FormBorderStyle.None;
                            BottomForm = (Form)o;
                        }
                    }
                }
            }

            Win32.RECT winRect = new Win32.RECT(0,0,0,0);
            Win32.RECT clientRect = new Win32.RECT(0,0,0,0);
            Win32.GetWindowRect(Handle, ref winRect);
            Win32.GetClientRect(Handle, ref clientRect);

            int leftW = 0;
            int rightW = 0;
            int topH = 0;
            int bottomH = 0;

            if (LeftForm != null)
            {
                leftW = LeftForm.ClientSize.Width;
                Win32.EnumChildWindows(Handle, new WndEnumProc(CaCommonDialog.MoveLeftProc), (IntPtr)leftW);
            }
            if (TopForm != null)
            {
                topH = TopForm.ClientSize.Height;
                Win32.EnumChildWindows(Handle, new WndEnumProc(CaCommonDialog.MoveDownProc), (IntPtr)topH);
            }
            if (RightForm != null)
                rightW = RightForm.ClientSize.Width;
            if (BottomForm != null)
                bottomH = BottomForm.ClientSize.Height;

            // layout windows:
            if (LeftForm != null)
            {
                Win32.SetParent(LeftForm.Handle, Handle);
                int oldStyle = Win32.GetWindowLong(LeftForm.Handle, Win32.GWL_STYLE);
                Win32.SetWindowLong(LeftForm.Handle, Win32.GWL_STYLE, oldStyle | Win32.WS_CHILD);
                LeftForm.Top = 0;
                LeftForm.Left = 0;
                LeftForm.Height = topH + clientRect.bottom; //clientRect.bottom;
                Win32.SetWindowPos(LeftForm.Handle, Win32.HWND_TOP, 0, 0, 0, 0, Win32.SWP_NOSIZE);
                LeftForm.Visible = true;
            }
            if (TopForm != null)
            {
                Win32.SetParent(TopForm.Handle, Handle);
                int oldStyle = Win32.GetWindowLong(TopForm.Handle, Win32.GWL_STYLE);
                Win32.SetWindowLong(TopForm.Handle, Win32.GWL_STYLE, oldStyle | Win32.WS_CHILD);
                TopForm.Top = 0;
                TopForm.Left = leftW;
                TopForm.Width = clientRect.right;
                Win32.SetWindowPos(TopForm.Handle, Win32.HWND_TOP, leftW, 0, 0, 0, Win32.SWP_NOSIZE);
                TopForm.Visible = true;
            }
            if (RightForm != null)
            {
                Win32.SetParent(RightForm.Handle, Handle);
                int oldStyle = Win32.GetWindowLong(RightForm.Handle, Win32.GWL_STYLE);
                Win32.SetWindowLong(RightForm.Handle, Win32.GWL_STYLE, oldStyle | Win32.WS_CHILD);
                RightForm.Height = clientRect.bottom + topH;
                Win32.SetWindowPos(RightForm.Handle, Win32.HWND_TOP, leftW + clientRect.right, 0, 0, 0, Win32.SWP_NOSIZE);
                RightForm.Visible = true;
            }
            if (BottomForm != null)
            {
                Win32.SetParent(BottomForm.Handle, Handle);
                int oldStyle = Win32.GetWindowLong(BottomForm.Handle, Win32.GWL_STYLE);
                Win32.SetWindowLong(BottomForm.Handle, Win32.GWL_STYLE, oldStyle | Win32.WS_CHILD);
                BottomForm.Width = leftW + rightW + clientRect.right;
                Win32.SetWindowPos(BottomForm.Handle, Win32.HWND_TOP, 0, topH + clientRect.bottom, 0, 0, Win32.SWP_NOSIZE);
                BottomForm.Visible = true;
            }

            CaCommonDialog.ResizeWindow(Handle, leftW + winRect.right - winRect.left + rightW,
                topH + winRect.bottom - winRect.top + bottomH);

            IncX = leftW + rightW;
            IncY = topH + bottomH;
        }



        // properties
        [Description(DialogConsts.strCaCustDlgParams_Icon)]
        public System.Drawing.Icon Icon
        {
            get {return _icon;}
            set
            {
                if (value != null)
                    _icon = (System.Drawing.Icon)value.Clone();
                else
                    _icon = null;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description(DialogConsts.strCaCustDlgParams_PosParams)]
        public CaDlgPosParams PosParams
        {
            get {return _posParams;}
        }

        [Browsable(false)]
        [Description(DialogConsts.strCaCustDlgParams_TopForm)]
        [DefaultValue(null)]
        public Form TopForm
        {
            get {return _topForm;}
            set {_topForm = value;}
        }

        [Browsable(false)]
        [Description(DialogConsts.strCaCustDlgParams_LeftForm)]
        [DefaultValue(null)]
        public Form LeftForm
        {
            get {return _leftForm;}
            set {_leftForm = value;}
        }

        [Browsable(false)]
        [Description(DialogConsts.strCaCustDlgParams_RightForm)]
        [DefaultValue(null)]
        public Form RightForm
        {
            get {return _rightForm;}
            set {_rightForm = value;}
        }

        [Browsable(false)]
        [Description(DialogConsts.strCaCustDlgParams_BottomForm)]
        [DefaultValue(null)]
        public Form BottomForm
        {
            get {return _bottomForm;}
            set {_bottomForm = value;}
        }
        [DefaultValue(true)]
        [Description(DialogConsts.strCaCustDlgParams_AutoCreateDockedForms)]
        public bool AutoCreateDockedForms
        {
            get {return _autoCreate;}
            set {_autoCreate = value;}
        }

        [Description(DialogConsts.strCaCustDlgParams_BottomFormClassName)]
        [DefaultValue("")]
        [Editor(typeof(DockedFormPropertyEditor), typeof(UITypeEditor))]
        public string BottomFormClassName
        {
            get {return _bottomClassName;}
            set {_bottomClassName = value;}
        }

        [Description(DialogConsts.strCaCustDlgParams_LeftFormClassName)]
        [DefaultValue("")]
        [Editor(typeof(DockedFormPropertyEditor), typeof(UITypeEditor))]
        public string LeftFormClassName
        {
            get {return _leftClassName;}
            set {_leftClassName = value;}
        }

        [Description(DialogConsts.strCaCustDlgParams_TopFormClassName)]
        [DefaultValue("")]
        [Editor(typeof(DockedFormPropertyEditor), typeof(UITypeEditor))]
        public string TopFormClassName
        {
            get {return _topClassName;}
            set {_topClassName = value;}
        }

        [Description(DialogConsts.strCaCustDlgParams_RightFormClassName)]
        [DefaultValue("")]
        [Editor(typeof(DockedFormPropertyEditor), typeof(UITypeEditor))]
        public string RightFormClassName
        {
            get {return _rightClassName;}
            set {_rightClassName = value;}
        }

        [DefaultValue(DialogPosition.Default)]
        [Description(DialogConsts.strCaCustDlgParams_StartPosition)]
        public DialogPosition StartPosition
        {
            get {return _position;}
            set {_position = value;}
        }

        internal CommonDialog Dialog
        {
            get {return _dialog;}
            set {_dialog = value;}
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Form OwnerForm
        {
            get {return _owner;}
            set {_owner = value;}
        }

        internal bool IsDesignMode
        {
            get
            {
                if (Dialog is CaCommonDialog)
                {
                    return ((CaCommonDialog)Dialog).IsDesignMode;
                }
                else
                {
                    Type t = _dialog.GetType();
                    PropertyInfo pi = t.GetProperty("IsDesignMode");
                    object o = pi.GetValue(Dialog, new object[] {});
                    return ((bool)o);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(DialogConsts.strCaCustDlgParams_Location)]
        public System.Drawing.Point Location
        {
            get
            {
                System.Drawing.Point pnt = new System.Drawing.Point(0, 0);

                ICommonDialog dlg = Dialog as ICommonDialog;
                if (dlg != null && dlg.Handle != IntPtr.Zero)
                {
                    Win32.RECT rect = new Win32.RECT(0, 0, 0, 0);
                    Win32.GetWindowRect(dlg.Handle, ref rect);
                    pnt.X = rect.left;
                    pnt.Y = rect.top;
                }

                return pnt;
            }
            set
            {
                if (IsDesignMode)
                {
                    return;
                }

                ICommonDialog dlg = Dialog as ICommonDialog;
                if (dlg != null && dlg.Handle != IntPtr.Zero)
                {
                    Win32.SetWindowPos(dlg.Handle, 0, value.X, value.Y, 0, 0, Win32.SWP_NOSIZE | Win32.SWP_NOZORDER);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(DialogConsts.strCaCustDlgParams_Size)]
        public System.Drawing.Size Size
        {
            get
            {
                System.Drawing.Size pnt = new System.Drawing.Size(0, 0);

                ICommonDialog dlg = Dialog as ICommonDialog;
                if (dlg != null && dlg.Handle != IntPtr.Zero)
                {
                    Win32.RECT rect = new Win32.RECT(0, 0, 0, 0);
                    Win32.GetWindowRect(dlg.Handle, ref rect);
                    pnt.Width = rect.right - rect.left;
                    pnt.Height = rect.bottom - rect.top;
                }

                return pnt;
            }
            set
            {
                if (IsDesignMode)
                {
                    return;
                }

                ICommonDialog dlg = Dialog as ICommonDialog;
                if (dlg != null && dlg.Handle != IntPtr.Zero)
                {
                    Win32.SetWindowPos(dlg.Handle, 0, 0, 0, value.Width, value.Height, Win32.SWP_NOMOVE | Win32.SWP_NOZORDER);
                }

                if (dlg.ModelessParentControl != null && dlg.ModelessDialog != null)
                {
                    dlg.ModelessParentControl.Size = value;
                    dlg.ModelessDialog.OnModelessParentResize(null, null);
                }
            }
        }

        internal Win32.RECT SelRect
        {
            get
            {
                return _selRect;
            }
            set
            {
                _selRect = value;
            }
        }
	}

	[ToolboxItem(false)]
    [Description(DialogConsts.strCaDlgPosParams_TypeDesc)]
    public class CaDlgPosParams : Component
    {
        // fields
        //int _shiftX = 0;
        //int _shiftY = 0;
        bool _fit = true;
		System.Drawing.Point _customLocation = new Point(0, 0);

        public CaDlgPosParams () :
            base()
        {
        }

        // properties
        [DefaultValue(true)]
        [Description(DialogConsts.strCaDlgPosParams_FitToScreen)]
        public bool FitToScreen { get{return _fit;} set{_fit = value;} }

        /*[DefaultValue(0)]
        [Description(DialogConsts.strCaDlgPosParams_ShiftX)]
        public int ShiftX { get{return _shiftX;} set {_shiftX = value;} }

        [DefaultValue(0)]
        [Description(DialogConsts.strCaDlgPosParams_ShiftY)]
        public int ShiftY { get{return _shiftY;} set {_shiftY = value;} }*/

		[DefaultValue("0, 0")]
		[Description(DialogConsts.strCaDlgPosParams_CustomLocation)]
		public Point CustomLocation { get{return _customLocation;} set {_customLocation = value;} }
    }
}
