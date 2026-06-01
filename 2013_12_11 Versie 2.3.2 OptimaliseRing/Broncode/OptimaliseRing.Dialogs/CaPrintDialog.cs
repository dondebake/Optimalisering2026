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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Text;
using System.Reflection;

using ComponentAge.PlatformServices;

namespace ComponentAge.Dialogs
{
    [ToolboxItem(false)]
	[Description(DialogConsts.strCaPrintDialogItemCaptions_TypeDesc)]
    public class CaPrintDialogItemCaptions : CaDlgItemCaptions
    {
        string _s1 = "";
        string _s2 = "";
        string _s3 = "";
        string _s4 = "";
        string _s5 = "";
        string _s6 = "";
        string _s7 = "";
        string _s8 = "";
        string _s9 = "";
        string _s10 = "";
        string _s11 = "";
        string _s12 = "";
        string _s13 = "";
        string _s14 = "";
        string _s15 = "";
        string _s16 = "";

        [ItemID(1075)]
		[Description(DialogConsts.strCaPrintDialogItemCaptions_GroupPrinter)]
        public string GroupPrinter { get {return _s1;} set {_s1 = value;} }
        [ItemID(1093)]
		[Description(DialogConsts.strCaPrintDialogItemCaptions_PrnName)]
        public string PrnName { get {return _s2;} set {_s2 = value;} }
        [ItemID(1095)]
		[Description(DialogConsts.strCaPrintDialogItemCaptions_PrnStatus)]
        public string PrnStatus { get {return _s3;} set {_s3 = value;} }
        [ItemID(1094)]
		[Description(DialogConsts.strCaPrintDialogItemCaptions_PrnType)]
        public string PrnType { get {return _s4;} set {_s4 = value;} }
        [ItemID(1097)]
		[Description(DialogConsts.strCaPrintDialogItemCaptions_PrnWhere)]
        public string PrnWhere { get {return _s5;} set {_s5 = value;} }
        [ItemID(1096)]
		[Description(DialogConsts.strCaPrintDialogItemCaptions_PrnComment)]
        public string PrnComment { get {return _s6;} set {_s6 = value;} }
        [ItemID(1040)]
		[Description(DialogConsts.strCaPrintDialogItemCaptions_PrintToFile)]
        public string PrintToFile { get {return _s7;} set {_s7 = value;} }
        [ItemID(1072)]
		[Description(DialogConsts.strCaPrintDialogItemCaptions_GroupRange)]
        public string GroupRange { get {return _s8;} set {_s8 = value;} }
        [ItemID(1056)]
		[Description(DialogConsts.strCaPrintDialogItemCaptions_RngAll)]
        public string RngAll { get {return _s9;} set {_s9 = value;} }
        [ItemID(1058)]
		[Description(DialogConsts.strCaPrintDialogItemCaptions_RngPages)]
        public string RngPages { get {return _s10;} set {_s10 = value;} }
        [ItemID(1089)]
		[Description(DialogConsts.strCaPrintDialogItemCaptions_RngPagesFrom)]
        public string RngPagesFrom { get {return _s11;} set {_s11 = value;} }
        [ItemID(1090)]
		[Description(DialogConsts.strCaPrintDialogItemCaptions_RngPagesTo)]
        public string RngPagesTo { get {return _s12;} set {_s12 = value;} }
        [ItemID(1057)]
		[Description(DialogConsts.strCaPrintDialogItemCaptions_RngSelection)]
        public string RngSelection { get {return _s13;} set {_s13 = value;} }
        [ItemID(1073)]
		[Description(DialogConsts.strCaPrintDialogItemCaptions_GroupCopies)]
        public string GroupCopies { get {return _s14;} set {_s14 = value;} }
        [ItemID(1092)]
		[Description(DialogConsts.strCaPrintDialogItemCaptions_CopNumbers)]
        public string CopNumber { get {return _s15;} set {_s15 = value;} }
        [ItemID(1041)]
		[Description(DialogConsts.strCaPrintDialogItemCaptions_CopCollate)]
        public string CopCollate { get {return _s16;} set {_s16 = value;} }
    }

    [DefaultEvent("Show")]
    [DefaultProperty("CustDlgParams")]
    [Designer(typeof(CaCommonDialogDesigner))]
    [LicenseProvider(typeof(LicUuidLicenseProvider))]
    [Guid("24969f7d-2ea3-4a3d-a759-96b3192782fa")]
    [System.Drawing.ToolboxBitmap(typeof(CaPrintDialog))]
	[Description(DialogConsts.strCaPrintDialog_TypeDesc)]
    public sealed class CaPrintDialog : CaCommonDialog
    {
        // member fields
		Win32.PRINTDLG structDlg;
        static CaPrintDialog HookOwner = null;
        CaPrintDialogItemCaptions _dlgCaptions = new CaPrintDialogItemCaptions();

        // constructor
        public CaPrintDialog() : base()
        {
        }

        private static void UpdatePrinterSettings(Win32.PRINTDLG data, PrinterSettings settings, PageSettings pageSettings)
        {
            int local0;
            settings.SetHdevmode(data.hDevMode);
            settings.SetHdevnames(data.hDevNames);
            if (pageSettings != null)
                pageSettings.SetHdevmode(data.hDevMode);
            local0 = data.Flags;
            if (settings.Copies == 1)
                settings.Copies = data.nCopies;
            settings.PrintRange = (PrintRange)(local0 & 3);
        }

        private int GetFlags ()
        {
            int res = Win32.PD_ENABLEPRINTHOOK;
            if (!_allowSomePages)
                res |= Win32.PD_NOPAGENUMS;
            if (!_allowPrintToFile)
                res |= Win32.PD_DISABLEPRINTTOFILE;
            if (!_allowSelection)
                res |= Win32.PD_NOSELECTION;
            res |= (int)_ps.PrintRange;
            if (_printToFile)
                res |= Win32.PD_PRINTTOFILE;
            if (ShowHelp)
                res |= Win32.PD_SHOWHELP;
            if (!_showNetwork)
                res |= Win32.PD_NONETWORKBUTTON;
            if (_ps.Collate)
                res |= Win32.PD_COLLATE;
            return res;
        }

		[Description(DialogConsts.strCaPrintDialog_RunDialog)]
        protected override bool RunDialog (IntPtr owner)
        {
            bool retVal = false;
            CaPrintDialog prevHookOwner = HookOwner;
            try
            {
                HookOwner = this;
                structDlg = new Win32.PRINTDLG();
                structDlg.lStructSize = Marshal.SizeOf(structDlg);
                structDlg.hInstance = Process.GetCurrentProcess().Handle;
                structDlg.Flags = GetFlags();
                structDlg.hwndOwner = owner;
                structDlg.nCopies = _ps.Copies;
                try
                {
                    if (this.PageSettings == null)
                        structDlg.hDevMode = _ps.GetHdevmode();
                    else
                        structDlg.hDevMode = _ps.GetHdevmode(this.PageSettings);
                    structDlg.hDevNames = _ps.GetHdevnames();
                }
                catch
                {
                    structDlg.hDevMode = IntPtr.Zero;
                    structDlg.hDevNames = IntPtr.Zero;
                }
                structDlg.lpfnPrintHook = new PrintHookProc(PrintHookProcBase);
                try
                {
                    if (_allowSomePages)
                    {
                        if (_ps.FromPage < _ps.MinimumPage || _ps.FromPage > _ps.MaximumPage)
                            throw new ArgumentException("FromPage propty is out of range");
                        if (_ps.ToPage < _ps.MinimumPage || _ps.ToPage > _ps.MaximumPage)
                            throw new ArgumentException("ToPage property is out of range");
                        if (_ps.ToPage < _ps.FromPage)
                            throw new ArgumentException("To/FromPage properties are invalid");
                        structDlg.nMinPage = (short)_ps.MinimumPage;
                        structDlg.nMaxPage = (short)_ps.MaximumPage;
                        structDlg.nFromPage = (short)_ps.FromPage;
                        structDlg.nToPage = (short)_ps.ToPage;
                    }
                    retVal = Win32.ExecPrintDlg(structDlg);
					if (retVal)
					{
						UpdatePrinterSettings(structDlg, _ps, PageSettings);
						PrintToFile = ((structDlg.Flags & Win32.PD_PRINTTOFILE) == Win32.PD_PRINTTOFILE);
						_ps.PrintToFile = PrintToFile;
						if (AllowSomePages)
						{
							_ps.FromPage = structDlg.nFromPage;
							_ps.ToPage = structDlg.nToPage;
						}
					}
					else
					{
						int n = Win32.CommDlgExtendedError();
						if (n != 0)
							throw new ApplicationException(string.Format(DialogConsts.strXFunctionFailed, "PrintDlg", n.ToString()));
					}
                }
                finally
                {
                    Win32.GlobalFree(structDlg.hDevMode);
                    Win32.GlobalFree(structDlg.hDevNames);
                }
            }
            finally
            {
                HookOwner = prevHookOwner;
            }
            return retVal;
        }

        internal static int SetupHookProcBase (IntPtr hdlg, int msg, IntPtr wparam, IntPtr lparam)
        {
            return 0;
        }

        internal static int PrintHookProcBase (IntPtr hdlg, int msg, IntPtr wparam, IntPtr lparam)
        {
            if (msg == Win32.WM_INITDIALOG)
            {
                HookOwner.SetHandle(hdlg);
                HookOwner.prevDlgProc = Win32.GetWindowLong1(HookOwner.Handle, Win32.GWL_WNDPROC);
                HookOwner.newDlgProc = new WindowProcedure(NewDlgProcInst);
                Win32.SetWindowLong(HookOwner.Handle, Win32.GWL_WNDPROC, HookOwner.newDlgProc);
                HookOwner.CustDlgParams.OnInitDialog();
                HookOwner.OnShow();
            }
            return 0;
        }

        internal bool OnPrinterOk ()
        {
            CancelEventArgs e = new CancelEventArgs();
            if (PrinterOk != null)
                PrinterOk(this, e);
            return e.Cancel;
        }
        internal void OnPrinterChanged ()
        {
            if (PrinterChanged != null)
                PrinterChanged(this, EventArgs.Empty);
        }
        internal bool OnBeforePrinterSetup ()
        {
            CancelEventArgs e = new CancelEventArgs(false);
            if (BeforePrinterSetup != null)
                BeforePrinterSetup(this, e);
            return e.Cancel;
        }
        internal void OnAfterPrinterSetup ()
        {
            if (AfterPrinterSetup != null)
                AfterPrinterSetup(this, EventArgs.Empty);
        }

        internal static int NewDlgProcInst (IntPtr hdlg, int msg, IntPtr wparam, IntPtr lparam)
        {
            bool processMsg = true;
            if ((msg == Win32.WM_COMMAND) && (Win32.LOWORD(wparam) == Win32.IDOK) && (Win32.HIWORD(wparam) == Win32.BN_CLICKED))
            {
                if (HookOwner.OnPrinterOk())
                    processMsg = false;
            }
            else if ((msg == Win32.WM_COMMAND) && (Win32.LOWORD(wparam) == Win32.cmb4) && (Win32.HIWORD(wparam) == Win32.CBN_SELCHANGE))
                HookOwner.OnPrinterChanged();
            else if (msg == Win32.WM_DESTROY)
                HookOwner.OnHide();
            else if ((msg == Win32.WM_COMMAND) && (Win32.LOWORD(wparam) == Win32.psh2) && (Win32.HIWORD(wparam) == Win32.BN_CLICKED))
            {
                if (HookOwner.OnBeforePrinterSetup())
                    processMsg = false;
                if (processMsg)
                {
                    int rval = Win32.CallWindowProc(HookOwner.prevDlgProc, HookOwner.Handle, msg, wparam, lparam);
                    HookOwner.OnAfterPrinterSetup();
                    return rval;
                }
            }

            if (processMsg)
                return Win32.CallWindowProc(HookOwner.prevDlgProc, HookOwner.Handle, msg, wparam, lparam);
            else
                return 0;
        }

        // properties
		[Description(DialogConsts.strPropDesc_ActiveDialog)]
		public static CaPrintDialog ActiveDialog { get {return HookOwner;} }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description(DialogConsts.strPropDesc_DlgItemsCaptions)]
        public CaPrintDialogItemCaptions DlgItemsCaptions
        {
            get {return _dlgCaptions;}
        }
        [Description(DialogConsts.strCaPrintDialog_SelectedPrinter)]
		[Browsable(false)]
        public string SelectedPrinter
        {
            get
            {
                if (Handle != IntPtr.Zero)
                {
                    IntPtr h = Win32.GetDlgItem(Handle, Win32.cmb4);
                    if (h != IntPtr.Zero)
                    {
                        int len = Win32.GetWindowTextLength(h);
                        if (len <= 0)
                            len = 2048;
                        StringBuilder text = new StringBuilder(1024);
                        Win32.GetWindowText(h, text, 1023);
                        return text.ToString();
                    }
                }
                return "";
            }
        }
        bool _allowPrintToFile = true;
        [DefaultValue(true)]
        [Description(DialogConsts.strCaPrintDialog_AllowPrintToFile)]
        public bool AllowPrintToFile
        {
            get {return _allowPrintToFile;}
            set {_allowPrintToFile = value;}
        }
        bool _allowSelection;
        [DefaultValue(false)]
        [Description(DialogConsts.strCaPrintDialog_AllowSelection)]
        public bool AllowSelection
        {
            get {return _allowSelection;}
            set {_allowSelection = value;}
        }
        bool _allowSomePages;
        [DefaultValue(false)]
        [Description(DialogConsts.strCaPrintDialog_AllowSomePages)]
        public bool AllowSomePages
        {
            get {return _allowSomePages;}
            set {_allowSomePages = value;}
        }
        PrintDocument _doc = null;
        [Description(DialogConsts.strCaPrintDialog_Document)]
        [DefaultValue(null)]
        public PrintDocument Document
        {
            get {return _doc;}
            set
            {
                _doc = value;
                if (_doc == null)
                {
                    _ps = new PrinterSettings();
                    return;
                }
                _ps = _doc.PrinterSettings;
            }
        }
        PrinterSettings _ps = new PrinterSettings();
        [Browsable(false)]
        [Description(DialogConsts.strCaPrintDialog_PrinterSettings)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PrinterSettings PrinterSettings
        {
            get {return _ps;}
            set
            {
                _ps = value;
                _doc = null;
                if (_ps == null)
                    _ps = new PrinterSettings();
            }
        }
        private PageSettings PageSettings
        {
            get
            {
                if (Document == null)
                    return null;
                return Document.DefaultPageSettings;
            }
        }

		bool _printToFile;
        [DefaultValue(false)]
        [Description(DialogConsts.strCaPrintDialog_PrintToFile)]
        public bool PrintToFile
        {
            get {return _printToFile;}
            set {_printToFile = value;}
        }

		bool _showNetwork;
        [DefaultValue(false)]
        [Description(DialogConsts.strCaPrintDialog_ShowNetwork)]
        public bool ShowNetwork
        {
            get {return _showNetwork;}
            set {_showNetwork = value;}
        }

        //events
		[Description(DialogConsts.strCaPrintDialog_PrinterOk)]
        public event CancelEventHandler PrinterOk;

		[Description(DialogConsts.strCaPrintDialog_PrinterChanged)]
        public event EventHandler PrinterChanged;

        [Description(DialogConsts.strCaPrintDialog_BeforePrinterSetup)]
        public event CancelEventHandler BeforePrinterSetup;

        [Description(DialogConsts.strCaPrintDialog_AfterPrinterSetup)]
        public event EventHandler AfterPrinterSetup;
	}
}
