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
	[Description(DialogConsts.strCaFontDialogItemCaptions_TypeDesc)]
	[ToolboxItem(false)]
    public class CaFontDialogItemCaptions : CaDlgItemCaptions
    {
		// fields
        string _font;
        string _fontStyle;
        string _size;
        string _effects;
        string _sample;
        string _strikeout;
        string _underline;
        string _script;
        string _color;
        string _sampleText;
        string _apply;

		// properties
        [ItemID(1088)]
		[Description(DialogConsts.strCaFontDialogItemCaptions_Font)]
        public string Font
        {
            get {return _font;}
            set {_font = value;}
        }
        [ItemID(1089)]
		[Description(DialogConsts.strCaFontDialogItemCaptions_FontStyle)]
        public string FontStyle
        {
            get {return _fontStyle;}
            set {_fontStyle = value;}
        }
        [ItemID(1090)]
		[Description(DialogConsts.strCaFontDialogItemCaptions_Size)]
        public string Size
        {
            get {return _size;}
            set {_size = value;}
        }
        [ItemID(1072)]
		[Description(DialogConsts.strCaFontDialogItemCaptions_Effects)]
        public string Effects
        {
            get {return _effects;}
            set {_effects = value;}
        }
        [ItemID(1073)]
		[Description(DialogConsts.strCaFontDialogItemCaptions_Sample)]
        public string Sample
        {
            get {return _sample;}
            set {_sample = value;}
        }
        [ItemID(1040)]
		[Description(DialogConsts.strCaFontDialogItemCaptions_Strikeout)]
        public string Strikeout
        {
            get {return _strikeout;}
            set {_strikeout = value;}
        }
        [ItemID(1041)]
		[Description(DialogConsts.strCaFontDialogItemCaptions_Underline)]
        public string Underline
        {
            get {return _underline;}
            set {_underline = value;}
        }
        [ItemID(1092)]
		[Description(DialogConsts.strCaFontDialogItemCaptions_SampleText)]
        public string SampleText
        {
            get {return _sampleText;}
            set {_sampleText = value;}
        }
        [ItemID(1091)]
		[Description(DialogConsts.strCaFontDialogItemCaptions_Color)]
        public string Color
        {
            get {return _color;}
            set {_color = value;}
        }
        [ItemID(1094)]
		[Description(DialogConsts.strCaFontDialogItemCaptions_Script)]
        public string Script
        {
            get {return _script;}
            set {_script = value;}
        }
        [ItemID(1026)]
		[Description(DialogConsts.strCaFontDialogItemCaptions_Apply)]
        public string Apply
        {
            get {return _apply;}
            set {_apply = value;}
        }
    }

    [DefaultEvent("Show")]
    [DefaultProperty("CustDlgParams")]
    [Designer(typeof(CaCommonDialogDesigner))]
    [LicenseProvider(typeof(LicUuidLicenseProvider))]
    [Guid("62bb8128-2eb6-474b-9958-c092c1740ed2")]
    [TopFormSupport(false)]
    [LeftFormSupport(false)]
    [System.Drawing.ToolboxBitmap(typeof(CaFontDialog))]
    [Description(DialogConsts.strCaFontDialog_TypeDesc)]
	public sealed class CaFontDialog : FontDialog, ICommonDialog
	{
		// fields
        CaCustDlgParams _dlgParams;
        IntPtr _handle;
        string _title;
        CaFontDialogItemCaptions _dlgCaptions = new CaFontDialogItemCaptions();
        static CaFontDialog HookOwner = null;
		static CaFontDialog prevHookOwner = null;
        Control _modelessParent;
        AutoResetEvent _invokeCompleteEvent = new AutoResetEvent(false);
        CaModelessDialog _modelessDialog;
        bool _modeless;

		public CaFontDialog()
            : base()
		{
            _dlgParams = new CaCustDlgParams(this);
		}

        // methods
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            IntPtr r = base.HookProc(hWnd, msg, wparam, lparam);
			if (msg == Win32.WM_INITDIALOG)
			{
				prevHookOwner = HookOwner;
				HookOwner = this;
				_handle = hWnd;
				_dlgParams.OnInitDialog();
				OnShow();
			}
			else if (msg == Win32.WM_DESTROY)
			{
				OnHide();
				HookOwner = prevHookOwner;
			}
            return r;
        }

        // event/event handlers
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler InitDialog;

        [Description(DialogConsts.strCaCommonDialog_Show)]
        public event EventHandler Show;

		[Description(DialogConsts.strCaCommonDialog_Hide)]
		public event EventHandler Hide;

		internal void OnShow ()
        {
            if (InitDialog != null)
                InitDialog(this, new EventArgs());

            if (Show != null)
                Show(this, new EventArgs());
        }

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
                Win32.SendMessageA(Win32.GetDlgItem(Handle, Win32.IDOK), Win32.BM_CLICK, IntPtr.Zero, IntPtr.Zero);
            else if (result == DialogResult.Cancel)
                Win32.SendMessageA(Win32.GetDlgItem(Handle, Win32.IDCANCEL), Win32.BM_CLICK, IntPtr.Zero, IntPtr.Zero);
        }
        bool ICommonDialog.IsModeless { get {return _modeless;} set {_modeless = value;} }
        Control ICommonDialog.ModelessParentControl { get { return _modelessParent; } set { _modelessParent = value; } }
        AutoResetEvent ICommonDialog.InvokeCompleteEvent { get { return _invokeCompleteEvent; } }
        CaModelessDialog ICommonDialog.ModelessDialog { get { return _modelessDialog; } set { _modelessDialog = value; } }

        // properties
		[Description(DialogConsts.strPropDesc_ActiveDialog)]
		public static CaFontDialog ActiveDialog { get {return HookOwner;} }
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
        public CaFontDialogItemCaptions DlgItemsCaptions
        {
            get {return _dlgCaptions;}
        }
	}
}
