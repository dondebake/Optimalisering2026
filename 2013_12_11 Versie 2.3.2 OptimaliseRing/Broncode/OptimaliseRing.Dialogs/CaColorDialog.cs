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
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Threading;

using ComponentAge.PlatformServices;

namespace ComponentAge.Dialogs
{

    [ToolboxItem(false)]
    [Description(DialogConsts.strCaColorDialogItemCaptions_TypeDesc)]
    public class CaColorDialogItemCaptions : CaDlgItemCaptions
    {
		// fields
        string _lum;
        string _red;
        string _blue;
        string _color;
        string _solid;
        string _hue;
        string _sat;
        string _green;
        string _definecustomcolors;
        string _addtocustomcolors;

		// properties
        [ItemID(719)]
		[Description(DialogConsts.strCaColorDialogItemCaptions_DefineCustomColors)]
        public string DefineCustomColors
        {
            get {return _definecustomcolors;}
            set {_definecustomcolors = value;}
        }

		[ItemID(712)]
		[Description(DialogConsts.strCaColorDialogItemCaptions_AddToCustomColors)]
        public string AddToCustomColors
        {
            get {return _addtocustomcolors;}
            set {_addtocustomcolors = value;}
        }

		[ItemID(730)]
		[Description(DialogConsts.strCaColorDialogItemCaptions_Color)]
        public string Color
        {
            get {return _color;}
            set {_color = value;}
        }

		[ItemID(731)]
		[Description(DialogConsts.strCaColorDialogItemCaptions_Solid)]
        public string Solid
        {
            get {return _solid;}
            set {_solid = value;}
        }

		[ItemID(723)]
		[Description(DialogConsts.strCaColorDialogItemCaptions_Hue)]
        public string Hue
        {
            get {return _hue;}
            set {_hue = value;}
        }

        [ItemID(724)]
		[Description(DialogConsts.strCaColorDialogItemCaptions_Sat)]
        public string Sat
        {
            get {return _sat;}
            set {_sat = value;}
        }

        [ItemID(725)]
		[Description(DialogConsts.strCaColorDialogItemCaptions_Lum)]
        public string Lum
        {
            get {return _lum;}
            set {_lum = value;}
        }

        [ItemID(726)]
		[Description(DialogConsts.strCaColorDialogItemCaptions_Red)]
        public string Red
        {
            get {return _red;}
            set {_red = value;}
        }

        [ItemID(727)]
		[Description(DialogConsts.strCaColorDialogItemCaptions_Green)]
        public string Green
        {
            get {return _green;}
            set {_green = value;}
        }

        [ItemID(728)]
		[Description(DialogConsts.strCaColorDialogItemCaptions_Blue)]
        public string Blue
        {
            get {return _blue;}
            set {_blue = value;}
        }
    }

    [DefaultEvent("Show")]
    [DefaultProperty("CustDlgParams")]
    [Designer(typeof(CaCommonDialogDesigner))]
    [LicenseProvider(typeof(LicUuidLicenseProvider))]
    [Guid("d3459511-0f42-4a86-9c27-16e459fe1351")]
    [TopFormSupport(false)]
    [LeftFormSupport(false)]
    [RightFormSupport(false)]
    [System.Drawing.ToolboxBitmap(typeof(CaColorDialog))]
	[Description(DialogConsts.strCaColorDialog_TypeDesc)]
	public sealed class CaColorDialog : ColorDialog, ICommonDialog
	{
		static CaColorDialog HookOwner = null;
		static CaColorDialog prevHookOwner = null;
        CaCustDlgParams _dlgParams;
        IntPtr _handle;
        string _title;
        CaColorDialogItemCaptions _dlgCaptions = new CaColorDialogItemCaptions();
        bool _modeless;
        Control _modelessParent;
        AutoResetEvent _invokeCompleteEvent = new AutoResetEvent(false);
        CaModelessDialog _modelessDialog;

		// ctor
		public CaColorDialog()
            : base()
		{
            _dlgParams = new CaCustDlgParams(this);
		}

        // methods
        protected override bool RunDialog (IntPtr hwndOwner)
        {
			prevHookOwner = HookOwner;
			HookOwner = this;
            // automatically open custom color pane
            if (AllowFullOpen && !FullOpen && (CustDlgParams.BottomForm != null || CustDlgParams.BottomFormClassName != ""))
                AllowFullOpen = false;
            bool ok = base.RunDialog(hwndOwner);
			HookOwner = prevHookOwner;
			return ok;
        }
		[EditorBrowsable(EditorBrowsableState.Never)]
        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            IntPtr ret = base.HookProc(hWnd, msg, wparam, lparam);
            if (msg == Win32.WM_INITDIALOG)
            {
                _handle = hWnd;
                _dlgParams.OnInitDialog();
                OnShow();
            }
            else if (msg == Win32.WM_DESTROY)
                OnHide();
            return ret;
        }

        // event/event handlers
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler InitDialog;

		[Description(DialogConsts.strCaCommonDialog_Show)]
		public event EventHandler Show;

        internal void OnShow ()
        {
            if (InitDialog != null)
                InitDialog(this, new EventArgs());

            if (Show != null)
                Show(this, new EventArgs());
        }

		[Description(DialogConsts.strCaCommonDialog_Hide)]
        public event EventHandler Hide;

        internal void OnHide ()
        {
            CustDlgParams.OnHide();
            if (Hide != null)
                Hide(this, new EventArgs());
            _handle = IntPtr.Zero;
        }

        // ICommonDialog impl
        [Description(DialogConsts.strCaCommonDialog_CloseDialog)]
        void ICommonDialog.CloseDialog (DialogResult result)
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
		[Description(DialogConsts.strPropDesc_ActiveDialog)]
		public static CaColorDialog ActiveDialog { get {return HookOwner;} }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(CustDlgParamsEditor), typeof(UITypeEditor))]
        [Description(DialogConsts.strCaCommonDialog_CustDlgParams)]
        public CaCustDlgParams CustDlgParams
        {
            get
            {
                return _dlgParams;
            }
        }

        [Browsable(false)]
		[Description(DialogConsts.strCaCommonDialog_Handle)]
        public IntPtr Handle
        {
            get {return _handle;}
        }

        [Description(DialogConsts.strCaCommonDialog_Title)]
        public string Title
        {
            get {return _title;}
            set
            {
                _title = value;
                if (_handle != IntPtr.Zero)
                    Win32.SetWindowText(_handle, value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public bool IsDesignMode
        {
            get {return DesignMode;}
        }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description(DialogConsts.strPropDesc_DlgItemsCaptions)]
        public CaColorDialogItemCaptions DlgItemsCaptions
        {
            get {return _dlgCaptions;}
        }
	}
}
