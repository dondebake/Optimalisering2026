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
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using Microsoft.Win32;
using System.Threading;
using ComponentAge.PlatformServices;

namespace ComponentAge.Dialogs
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ItemIDAttribute : Attribute
    {
        int _id;

        public ItemIDAttribute (int id) : base()
        {
            _id = id;
        }

        public int ItemID
        {
            get {return _id;}
            set {_id = value;}
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class FeatureSupportAttribute : Attribute
    {
        bool _supported;

        public FeatureSupportAttribute (bool supported) : base()
        {
            _supported = supported;
        }

        public bool Supported
        {
            get {return _supported;}
            set {_supported = value;}
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class LeftFormSupportAttribute : FeatureSupportAttribute
    {
        public LeftFormSupportAttribute (bool supported) : base (supported) {}
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class TopFormSupportAttribute : FeatureSupportAttribute
    {
        public TopFormSupportAttribute (bool supported) : base (supported) {}
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class RightFormSupportAttribute : FeatureSupportAttribute
    {
        public RightFormSupportAttribute (bool supported) : base (supported) {}
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class BottomFormSupportAttribute : FeatureSupportAttribute
    {
        public BottomFormSupportAttribute (bool supported) : base (supported) {}
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class FullPreviewSupportAttribute : FeatureSupportAttribute
    {
        public FullPreviewSupportAttribute (bool supported) : base (supported) {}
    }

    [Description(DialogConsts.strDialogPosition_TypeDesc)]
    public enum DialogPosition
    {
        Default,
        ScreenCenter,
        MainFormCenter,
        OwnerFormSenter,
		Custom
    }

    [ToolboxItem(false)]
    [Description(DialogConsts.strCaDlgItemCaptions_TypeDesc)]
	[TypeConverter(typeof(ExpandableObjectConverter))]
    public class CaDlgItemCaptions
    {
        // fields
        string _ok;
        string _cancel;
        string _help;

        // methods
        internal void SetCaptions (IntPtr hwnd)
        {
            PropertyInfo[] props = this.GetType().GetProperties();
            for (int i = 0; i < props.Length; i++ )
            {
                string val = (string)props[i].GetValue(this, new object[] {});
                if (val != "" && val != null)
                {
                    object[] ids = props[i].GetCustomAttributes(typeof(ItemIDAttribute), true);
                    for (int j = 0; j < ids.Length; j++ )
                    {
                        if (ids[j] is ItemIDAttribute)
                        {
                            ItemIDAttribute attr = ids[j] as ItemIDAttribute;
                            IntPtr dlgItem = Win32.GetDlgItem(hwnd, attr.ItemID);
                            Win32.SetWindowText(dlgItem, val);
                            break;
                        }
                    }
                }
            }
        }

        // properties
        [ItemID(Win32.IDOK)]
        [Description(DialogConsts.strCaDlgItemCaptions_OK)]
        public string OK
        {
            get {return _ok;}
            set {_ok = value;}
        }
        [ItemID(Win32.IDCANCEL)]
        [Description(DialogConsts.strCaDlgItemCaptions_Cancel)]
        public string Cancel
        {
            get {return _cancel;}
            set {_cancel = value;}
        }
        [ItemID(1038)]
        [Description(DialogConsts.strCaDlgItemCaptions_Help)]
        public string Help
        {
            get {return _help;}
            set {_help = value;}
        }
    }

    public interface ICommonDialog
    {
        void CloseDialog (DialogResult result);
        DialogResult ShowDialog ();
        IntPtr Handle { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        event EventHandler InitDialog;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        bool IsModeless { get; set; }
        /// <summary>
        /// Control specified in ModelessDialog.Show(parent, ..) parent argument.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        Control ModelessParentControl  { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        AutoResetEvent InvokeCompleteEvent { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        CaModelessDialog ModelessDialog { get; set; }
    }

    [DefaultEvent("Show")]
    [DefaultProperty("CustDlgParams")]
    [Designer(typeof(CaCommonDialogDesigner))]
    [Description(DialogConsts.strCaCommonDialog_TypeDesc)]
    public abstract class CaCommonDialog : CommonDialog, ICommonDialog
    {
        // member fields
        CaCustDlgParams _dlgParams;
        IntPtr _handle = IntPtr.Zero;
        string _title;
        License _license = null;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public IntPtr prevDlgProc;
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected WindowProcedure newDlgProc;
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected bool closingDialog;
        internal IntPtr _hooker; // HHOOK
        internal IntPtr _keyboardHooker; // HHOOK
        internal static CaCommonDialog _currentHooker;
        internal static CaCommonDialog _currentKeyboardHooker;
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected bool fullTimeHook = false;
        protected bool fullTimeKeyboardHook = false;
        private bool _showHelp;
        private Control _modelessParent;
        private AutoResetEvent _invokeCompleteEvent = new AutoResetEvent(false);
        private bool _modeless;
        private CaModelessDialog _modelessDialog;
        private HookProc _wndHookInstance;
        private HookProc _keybHookProcInstanse;


        [Description(DialogConsts.strCaCommonDialog_Ctor)]
        public CaCommonDialog ()
        {
            _dlgParams = new CaCustDlgParams(this);
            _license = LicenseManager.Validate(this.GetType(), this);
        }

        protected override void Dispose (bool disposing)
        {
            if (_license != null)
            {
                _license.Dispose();
                _license = null;
            }
            base.Dispose(disposing);
        }

        // methods
        internal static void CreateControlCopy (Control c, Control parent)
        {
            System.Type t = c.GetType();
            Control newC = Activator.CreateInstance(t, new object[] {}) as Control;
            newC.Parent = parent;
            newC.Name = c.Name;
            for (int i = 0; i < c.Controls.Count; i++ )
                CreateControlCopy(c.Controls[i], newC);
        }
        internal static void SetControlProperties (Control c, Control container)
        {
            int idx = -1;
            for (int i = 0; i < container.Controls.Count; i++ )
            {
                if (container.Controls[i].Name == c.Name)
                {
                    idx = i;
                    break;
                }
            }
            if (idx == -1)
                return;
            Control newC = container.Controls[idx];
            System.Type t = c.GetType();
            PropertyInfo[] props = t.GetProperties();
            for (int i = 0; i < props.Length; i++ )
            {
                if (props[i].CanRead && props[i].CanWrite &&
                    (props[i].PropertyType.IsPrimitive
                    || props[i].PropertyType == typeof(Font)
                    || props[i].PropertyType == typeof(Color)
                    || props[i].PropertyType == typeof(string)
                    || props[i].PropertyType.IsEnum
                    ))
                {
                    props[i].SetValue(newC, props[i].GetValue(c, new object[] {}), new object[] {});
                }
            }
            for (int i = 0; i < c.Controls.Count; i++ )
                SetControlProperties(c.Controls[i], newC);
        }

        private void OnResizeWindow (int lparam)
        {
            int topw = 0;
            int lefth = 0;
            int righth = 0;
            if (_dlgParams.TopForm != null)
            {
                topw = Win32.LOWORD(lparam);
                if (_dlgParams.LeftForm != null)
                    topw = topw - _dlgParams.LeftForm.Width;
            }
            if (_dlgParams.RightForm != null)
            {
                if (_dlgParams.TopForm != null)
                    topw = topw - _dlgParams.RightForm.Width;
                righth = Win32.HIWORD(lparam);
            }
            if (_dlgParams.LeftForm != null)
                lefth = Win32.HIWORD(lparam);
            if (_dlgParams.BottomForm != null)
            {
                if (_dlgParams.RightForm != null)
                    righth = righth - _dlgParams.BottomForm.Height;
                if (_dlgParams.LeftForm != null)
                    lefth = lefth - _dlgParams.BottomForm.Height;
                _dlgParams.BottomForm.Width = Win32.LOWORD(lparam);
            }
            if (_dlgParams.TopForm != null)
                _dlgParams.TopForm.Width = topw;
            if (_dlgParams.LeftForm != null)
                _dlgParams.LeftForm.Height = lefth;
            if (_dlgParams.RightForm != null)
                _dlgParams.RightForm.Height = righth;
        }

        internal static void ResizeWindow(IntPtr hwnd, int w, int h)
        {
            Win32.SetWindowPos(hwnd, Win32.HWND_TOP, 0, 0, w, h, Win32.SWP_NOMOVE | Win32.SWP_NOACTIVATE);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int MoveLeftProc (IntPtr hctrl, IntPtr lparam)
        {
            Win32.RECT r1 = new Win32.RECT(0,0,0,0);
            Win32.GetWindowRect(hctrl, ref r1);
            Win32.POINT p1 = new Win32.POINT();
            p1.x = r1.left;
            p1.y = r1.top;
            IntPtr hParent = Win32.GetParent(hctrl);
            Win32.ScreenToClient(hParent, ref p1);
            Win32.SetWindowPos(hctrl, Win32.HWND_TOP, p1.x + (int)(lparam) + Win32.GetSystemMetrics(Win32.SM_CYDLGFRAME), p1.y, 0, 0, Win32.SWP_NOSIZE | Win32.SWP_NOZORDER);
            Win32.RedrawWindow(hctrl, ref r1, IntPtr.Zero, 0);
            return 1;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int RepaintCtrlProc (IntPtr hctrl, IntPtr lparam)
        {
            Win32.RECT r1 = new Win32.RECT(0,0,0,0);
            Win32.GetWindowRect(hctrl, ref r1);
            Win32.RedrawWindow(hctrl, ref r1, IntPtr.Zero, 0);//Win32.RDW_INVALIDATE);
            return 1;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int MoveDownProc (IntPtr hctrl, IntPtr lparam)
        {
            Win32.RECT r1 = new Win32.RECT(0,0,0,0);
            Win32.GetWindowRect(hctrl, ref r1);
            Win32.POINT p1 = new Win32.POINT();
            p1.x = r1.left;
            p1.y = r1.top;
            IntPtr hParent = Win32.GetParent(hctrl);
            Win32.ScreenToClient(hParent, ref p1);
            Win32.SetWindowPos(hctrl, Win32.HWND_TOP, p1.x, p1.y + (int)(lparam) + Win32.GetSystemMetrics(Win32.SM_CXDLGFRAME), 0, 0, Win32.SWP_NOSIZE | Win32.SWP_NOZORDER);
            Win32.RedrawWindow(hctrl, ref r1, IntPtr.Zero, 0);
            return 1;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void SetHandle (IntPtr handle)
        {
            _handle = handle;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void InstallHookProc (bool InstallOnFullDialogTime)
        {
            _currentHooker = this;
            fullTimeHook = InstallOnFullDialogTime;
            _wndHookInstance = new HookProc(InternalHookProc);
            _hooker = Win32.SetWindowsHookEx(Win32.WH_CALLWNDPROC, _wndHookInstance, IntPtr.Zero, DesignMode ? 0 : AppDomain.GetCurrentThreadId());
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void InstallHookProc ()
        {
            // unhook immediatelly after WM_INITDIALOG: works by default for Shell About, etc dialogs
            // For Open/Save: _fullTimeHook is true. It means that
            // Unhook... is in OnHide() member.
            InstallHookProc(false);
        }

        internal IntPtr InternalKeyboardHookProc(int nCode, IntPtr wparam, IntPtr lparam)
        {
            IntPtr retVal = IntPtr.Zero;
            if (nCode < 0)
                return Win32.CallNextHookEx(_keyboardHooker, nCode, wparam, lparam);

            if (!OnHookKeyboardEvent(wparam, lparam))
            {
                return new IntPtr(1);
            }

            return Win32.CallNextHookEx(_keyboardHooker, nCode, wparam, lparam);
        }

        internal IntPtr InternalHookProc (int nCode, IntPtr wparam, IntPtr lparam)
        {
            IntPtr retVal = IntPtr.Zero;
            if (nCode < 0)
            {
                return Win32.CallNextHookEx(_hooker, nCode, wparam, lparam);
            }

            Win32.CWPSTRUCT s = new Win32.CWPSTRUCT();
            Marshal.PtrToStructure(lparam, s);
            if (_currentHooker != null && !_currentHooker.fullTimeHook && s != null && s.message == Win32.WM_INITDIALOG)
            {
                _currentHooker.SetHandle(s.hwnd);
                Win32.UnhookWindowsHookEx(_hooker);
                _currentHooker.CustDlgParams.OnInitDialog(); // dock forms
                _currentHooker._hooker = IntPtr.Zero;
                _currentHooker.SetupNewDialogProc();
                _currentHooker.OnShow();
                _currentHooker = null;
            }
            else
            {
                OnHookEvent(s.hwnd, s.message, s.wparam, s.lparam);
            }

            return Win32.CallNextHookEx(_hooker, nCode, wparam, lparam);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void InstallKeyboardHookProc(bool InstallOnFullDialogTime)
        {
            _currentKeyboardHooker = this;
            fullTimeKeyboardHook = InstallOnFullDialogTime;
            _keybHookProcInstanse = new HookProc(InternalKeyboardHookProc);
            _keyboardHooker = Win32.SetWindowsHookEx(Win32.WH_KEYBOARD, _keybHookProcInstanse, IntPtr.Zero, DesignMode ? 0 : AppDomain.GetCurrentThreadId());
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void InstallKeyboardHookProc()
        {
            // unhook immediatelly after WM_INITDIALOG: works by default for Shell About, etc dialogs
            // For Open/Save: _fullTimeHook is true. It means that
            // Unhook... is in OnHide() member.
            InstallKeyboardHookProc(true);
        }

        /// <summary>
        /// Return true if message should be processed, otherwise - false.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual bool OnHookKeyboardEvent(IntPtr wparam, IntPtr lparam)
        {
            // empty body in this component
            return true;
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnHookEvent (IntPtr wnd, int message, IntPtr wparam, IntPtr lparam)
        {
            // empty body in this component
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual int OnDialogMessage (IntPtr wnd, int message, IntPtr wparam, IntPtr lparam)
        {
            if (message == Win32.WM_COMMAND)
            {
				if (Win32.LOWORD(wparam) == Win32.IDCANCEL && Win32.HIWORD(wparam) == Win32.BN_CLICKED)
				{
					if (CancelDialog != null)
					{
						CancelEventArgs e = new CancelEventArgs();
						CancelDialog(this, e);
						if (e.Cancel)
						{
							return 0;
						}
					}
				}
				if (Win32.LOWORD(wparam) == Win32.IDOK && Win32.HIWORD(wparam) == Win32.BN_CLICKED)
				{
					if (AcceptDialog != null)
					{
						CancelEventArgs e = new CancelEventArgs();
						AcceptDialog(this, e);
						if (e.Cancel)
						{
							return 0;
						}
					}
				}
            }


            int ok = Win32.CallWindowProc(prevDlgProc, wnd, message, wparam, lparam);

            if (message == Win32.WM_SIZE)
            {
                OnResize();
            }

            return ok;
        }

        private void OnResize()
        {
            if (Resize != null)
            {
                Resize(this, EventArgs.Empty);
            }
        }

        private int NewDialogProc (IntPtr hwnd, int message, IntPtr wparam, IntPtr lparam)
        {
            if (message == Win32.WM_DESTROY)
            {
                OnHide();
                int retval = OnDialogMessage(hwnd, message, wparam, lparam);
                _handle = IntPtr.Zero;
                return retval;
            }

            return OnDialogMessage(hwnd, message, wparam, lparam);
        }

        internal void SetupNewDialogProc ()
        {
            prevDlgProc = Win32.GetWindowLong1(Handle, Win32.GWL_WNDPROC);
            newDlgProc = new WindowProcedure(NewDialogProc);
            Win32.SetWindowLong(Handle, Win32.GWL_WNDPROC, newDlgProc);
        }

        // event/event handlers
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public event EventHandler InitDialog;

        [Description(DialogConsts.strCaCommonDialog_Show)]
        public event EventHandler Show;

        [Description(DialogConsts.strCaCommonDialog_Resize)]
        public event EventHandler Resize;

        [Description(DialogConsts.strCaCommonDialog_CancelDialog)]
        public event CancelEventHandler CancelDialog;

		[Description(DialogConsts.strCaCommonDialog_AcceptDialog)]
		public event CancelEventHandler AcceptDialog;


        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnShow ()
        {
            OnInitDialog();
            if (Show != null)
                Show(this, new EventArgs());
        }

        private void InvokeInitDialog(object sender, System.EventArgs e)
        {
            if (InitDialog != null)
                InitDialog(sender, EventArgs.Empty);
        }

        private void OnInitDialog()
        {
            ICommonDialog dlg = this as ICommonDialog;
            InvokeInitDialog(dlg, null);
        }

        [Description(DialogConsts.strCaCommonDialog_Hide)]
        public event EventHandler Hide;

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected internal void OnHide ()
        {
            CustDlgParams.OnHide();
            if (Hide != null)
                Hide(this, new EventArgs());
            if (_hooker != IntPtr.Zero && fullTimeHook)
            {
                Win32.UnhookWindowsHookEx(_hooker);
                _hooker = IntPtr.Zero;
                _currentHooker = null;
            }

            if (_keyboardHooker != IntPtr.Zero && fullTimeKeyboardHook)
            {
                Win32.UnhookWindowsHookEx(_keyboardHooker);
                _keyboardHooker = IntPtr.Zero;
                _currentKeyboardHooker = null;
            }
        }

        [Description(DialogConsts.strCaCommonDialog_Reset)]
        public override void Reset ()
        {
            CustDlgParams.Reset();
            Type t = this.GetType();
            PropertyInfo[] props = t.GetProperties();
            for (int i = 0; i < props.Length; i++ )
            {
                PropertyInfo pi = props[i];
                object[] attrs = pi.GetCustomAttributes(true);
                if (pi.PropertyType == typeof(string) || pi.PropertyType.IsEnum)
                {
                    if (pi.Name.Equals("Name"))
                        continue;
                    for (int j = 0; j < attrs.Length; j++ )
                    {
                        if (attrs[j] is DefaultValueAttribute)
                        {
                            DefaultValueAttribute attr = attrs[j] as DefaultValueAttribute;
                            pi.SetValue(this, attr.Value, new object[] {});
                            break;
                        }
                    }
                }
            }
        }

        public void Focus()
        {
            if (Handle != IntPtr.Zero)
            {
                Win32.SetFocus(Handle);
            }
        }

        // ICommonDialog impl
        [Description(DialogConsts.strCaCommonDialog_CloseDialog)]
        public void CloseDialog (DialogResult result)
        {
            if (result == DialogResult.OK)
                Win32.SendMessage(Win32.GetDlgItem(Handle, Win32.IDOK), Win32.BM_CLICK, IntPtr.Zero, IntPtr.Zero);
            else if (result == DialogResult.Cancel)
                Win32.SendMessage(Win32.GetDlgItem(Handle, Win32.IDCANCEL), Win32.BM_CLICK, IntPtr.Zero, IntPtr.Zero);
        }
        bool ICommonDialog.IsModeless { get {return _modeless;} set {_modeless = value;} }
        Control ICommonDialog.ModelessParentControl { get { return _modelessParent; } set { _modelessParent = value; } }
        AutoResetEvent ICommonDialog.InvokeCompleteEvent { get { return _invokeCompleteEvent; } }
        CaModelessDialog ICommonDialog.ModelessDialog { get { return _modelessDialog; } set { _modelessDialog = value; } }

        // properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(CustDlgParamsEditor), typeof(UITypeEditor))]
        [Description(DialogConsts.strCaCommonDialog_CustDlgParams)]
        public CaCustDlgParams CustDlgParams { get {return _dlgParams;} }

        [Browsable(false)]
        [Description(DialogConsts.strCaCommonDialog_Handle)]
        public IntPtr Handle { get {return _handle;} }

        [Description(DialogConsts.strCaCommonDialog_Title)]
        [DefaultValue("")]
        public string Title
        {
            get {return _title;}
            set
            {
                _title = value;
                if (Handle != IntPtr.Zero)
                    Win32.SetWindowText(Handle, _title);
            }
        }

        internal bool IsDesignMode
        {
            get {return DesignMode;}
        }

        [DefaultValue(false)]
        [Description(DialogConsts.strCaCommonDialog_ShowHelp)]
        public virtual bool ShowHelp
        {
            get {return _showHelp;}
            set {_showHelp = value;}
        }

        [Description(DialogConsts.strCaCommonDialog_ActualTitle)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual string ActualTitle
        {
            get
            {
                if (Handle != IntPtr.Zero)
                {
                    int n = Win32.GetWindowTextLength(Handle);
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(n + 1);
                    Win32.GetWindowText(Handle, sb, n);
                    return sb.ToString();
                }
                else
                    return Title;
            }
        }

        [Browsable(false)]
        [Description("Gets the position and size of the OK button.")]
        public System.Drawing.Rectangle OkButtonRect
        {
            get
            {
                if (Handle == IntPtr.Zero)
                {
                    throw new InvalidOperationException("OK Button rectangle can not be obtained when dialog is closed.");
                }

                IntPtr buttonHandle = IntPtr.Zero;
                buttonHandle = Win32.GetDlgItem(Handle, Win32.IDOK);

                Win32.RECT r = new Win32.RECT(0, 0, 0, 0);
                Win32.GetWindowRect(buttonHandle, ref r);
                Win32.POINT p = new Win32.POINT(r.left, r.top);
                Win32.ScreenToClient(Handle, ref p);

                return new System.Drawing.Rectangle(p.x, p.y, r.right - r.left, r.bottom - r.top);
            }
        }

        [Browsable(false)]
        [Description("Gets the position and size of the Cancel button.")]
        public System.Drawing.Rectangle CancelButtonRect
        {
            get
            {
                if (Handle == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Cancel button rectangle can not be obtained when dialog is closed.");
                }

                IntPtr buttonHandle = IntPtr.Zero;
                buttonHandle = Win32.GetDlgItem(Handle, Win32.IDCANCEL);

                Win32.RECT r = new Win32.RECT(0, 0, 0, 0);
                Win32.GetWindowRect(buttonHandle, ref r);
                Win32.POINT p = new Win32.POINT(r.left, r.top);
                Win32.ScreenToClient(Handle, ref p);

                return new System.Drawing.Rectangle(p.x, p.y, r.right - r.left, r.bottom - r.top);
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class CaCommonDialogDesigner : ComponentDesigner
    {
        // member fields
        private DesignerVerb _verbDesigner;
        private DesignerVerb _verbPreview;

        // virtual methods
        public override DesignerVerbCollection Verbs
        {
            get
            {
                bool supportDockPreview = true;
                object[] attrs = Component.GetType().GetCustomAttributes(typeof(FullPreviewSupportAttribute), true);
                for (int i = 0; i < attrs.Length; i++ )
                {
                    FullPreviewSupportAttribute a =  (FullPreviewSupportAttribute)attrs[i];
                    if (!a.Supported)
                    {
                        supportDockPreview = false;
                        break;
                    }
                }
                DesignerVerbCollection verbs = base.Verbs;
                if (_verbDesigner == null)
                    _verbDesigner = new DesignerVerb(DialogConsts.strTemplateDesigner, new System.EventHandler(OnDesign));
                if (verbs.IndexOf(_verbDesigner) == -1)
                    verbs.Add(_verbDesigner);
                if (_verbPreview == null)
                    _verbPreview = new DesignerVerb(supportDockPreview ? DialogConsts.strPreview : DialogConsts.strPreview2, new System.EventHandler(OnPreview));
                if (verbs.IndexOf(_verbPreview) == -1 && !(Component is CaPageSetupDialog))
                    verbs.Add(_verbPreview);
                return verbs;
            }
        }

#if DOTNET2
        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            base.InitializeNewComponent(defaultValues);

            CommonDialog component = Component as CommonDialog;
            if (component == null)
                return;
            if (Component.Site == null)
                return;
            IContainer container = Component.Site.Container;
            if (container == null)
                return;
            for (int i = 0; i < container.Components.Count; i++)
            {
                IComponent comp = container.Components[i];
                if (comp is Form)
                {
                    Type t = component.GetType();
                    PropertyInfo pi = t.GetProperty("CustDlgParams");
                    object o = pi.GetValue(component, new object[] { });
                    CaCustDlgParams dlgparams = (CaCustDlgParams)o;
                    dlgparams.OwnerForm = (Form)comp;
                    break;
                }
            }
        }
#else
        public override void OnSetComponentDefaults ()
        {
            base.OnSetComponentDefaults();
            CommonDialog component = Component as CommonDialog;
                if (component == null)
                    return;
            if (Component.Site == null)
                return;
            IContainer container = Component.Site.Container;
            if (container == null)
                return;
            for (int i = 0; i < container.Components.Count; i++)
            {
                IComponent comp = container.Components[i];
                if (comp is Form)
                {
                    Type t = component.GetType();
                    PropertyInfo pi = t.GetProperty("CustDlgParams");
                    object o = pi.GetValue(component, new object[] {});
                    CaCustDlgParams dlgparams = (CaCustDlgParams)o;
                    dlgparams.OwnerForm = (Form)comp;
                    break;
                }
            }
        }
#endif

        // event handlers
        internal void OnDesign (object sender, System.EventArgs e)
        {
            DialogDesignerForm form = new DialogDesignerForm();
            CommonDialog dlg = (CommonDialog)Component;
            form.Instance = dlg;
            if (form.ShowDialog() == DialogResult.OK)
                RaiseComponentChanged(null, null, null);
        }
        internal void OnPreview (object sender, System.EventArgs e)
        {
            string skey = "Software\\COMPONENTAGE Software\\Dialog Workshop .NET\\1.0";
            string strPreviewKey = "PreviewWarning";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(skey, true);
            bool showWarn = true;
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey(skey);
                key.SetValue(strPreviewKey, true);
            }
            else
                showWarn = bool.Parse((string)key.GetValue(strPreviewKey, true));
            bool showIt = true;
            if (showWarn)
            {
                PreviewWarnForm warnDialog = new PreviewWarnForm();
                showIt = (warnDialog.ShowDialog() == DialogResult.Yes);
                key.SetValue(strPreviewKey, warnDialog.checkBox1.Checked);
            }
            if (showIt)
            {
                try
                {
                    CommonDialog dlg = (CommonDialog)Component;
                    dlg.ShowDialog();
                }
                catch{}
            }
        }
    }
}
