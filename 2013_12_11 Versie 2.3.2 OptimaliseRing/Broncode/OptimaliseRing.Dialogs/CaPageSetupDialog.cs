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
using System.Drawing;
using ComponentAge.PlatformServices;

namespace ComponentAge.Dialogs
{
	[Description(DialogConsts.strPageRegion_TypeDesc)]
	public enum PageRegion
	{
		BeforeDraw,
		FullPage,
		MinMargin,
		Margin,
		GreekText,
		EnvStamp,
		YAFullPage
	}


	[Description(DialogConsts.strCaFontDialogItemCaptions_TypeDesc)]
	[ToolboxItem(false)]
    public class CaPageSetupDialogItemCaptions : CaDlgItemCaptions
    {
		// fields
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

		// properties
		[ItemID(1073)]
		[Description(DialogConsts.strCaPageSetupDialogItemCaptions_GroupPaper)]
		public string GroupPaper { get {return _s1;} set {_s1 = value;} }
		[ItemID(1072)]
		[Description(DialogConsts.strCaPageSetupDialogItemCaptions_GroupOrientation)]
		public string GroupOrientstion { get {return _s2;} set {_s2 = value;} }
		[ItemID(1075)]
		[Description(DialogConsts.strCaPageSetupDialogItemCaptions_GroupMargins)]
		public string GroupMargins { get {return _s3;} set {_s3 = value;} }
		[ItemID(1089)]
		[Description(DialogConsts.strCaPageSetupDialogItemCaptions_PaperSize)]
		public string PaperSize { get {return _s4;} set {_s4 = value;} }
		[ItemID(1090)]
		[Description(DialogConsts.strCaPageSetupDialogItemCaptions_PaperSource)]
		public string PaperSource { get {return _s5;} set {_s5 = value;} }
		[ItemID(1056)]
		[Description(DialogConsts.strCaPageSetupDialogItemCaptions_OrientPortrait)]
		public string OrientPortrait { get {return _s6;} set {_s6 = value;} }
		[ItemID(1057)]
		[Description(DialogConsts.strCaPageSetupDialogItemCaptions_OrientLandscape)]
		public string OrientLandscape { get {return _s7;} set {_s7 = value;} }
		[ItemID(1102)]
		[Description(DialogConsts.strCaPageSetupDialogItemCaptions_MarginLeft)]
		public string MarginLeft { get {return _s8;} set {_s8 = value;} }
		[ItemID(1104)]
		[Description(DialogConsts.strCaPageSetupDialogItemCaptions_MarginTop)]
		public string MarginTop { get {return _s9;} set {_s9 = value;} }
		[ItemID(1103)]
		[Description(DialogConsts.strCaPageSetupDialogItemCaptions_MarginRight)]
		public string MarginRight { get {return _s10;} set {_s10 = value;} }
		[ItemID(1105)]
		[Description(DialogConsts.strCaPageSetupDialogItemCaptions_MarginBottom)]
		public string MarginBottom { get {return _s11;} set {_s11 = value;} }
		[ItemID(1026)]
		[Description(DialogConsts.strCaPageSetupDialogItemCaptions_Printer)]
		public string Printer { get {return _s12;} set {_s12 = value;} }



    }

	[Description(DialogConsts.strDrawPageEventArgs_TypeDesc)]
	public class DrawPageEventArgs : System.EventArgs
	{
		// fields
		Rectangle _rect;
		Graphics _g;
		PageRegion _reg;
		bool _cancel;

		// ctor
		internal DrawPageEventArgs (PageRegion region, Graphics g, Win32.RECT r)
		{
			_g = g;
			_reg = region;
			_rect = new Rectangle(r.left, r.top, r.right - r.left, r.bottom - r.top);
		}

		// properties
		[Description(DialogConsts.strDrawPageEventArgs_Graphics)]
		public Graphics Graphics { get {return _g;} }
		[Description(DialogConsts.strDrawPageEventArgs_Rect)]
		public Rectangle Bounds { get {return _rect;} }
		[Description(DialogConsts.strDrawPageEventArgs_PageRegion)]
		public PageRegion Region { get {return _reg;} }
		[Description(DialogConsts.strDrawPageEventArgs_Cancel)]
		public bool Cancel { get {return _cancel;} set {_cancel = value;} }
	}

	[Description(DialogConsts.strDrawPageEventHandler_TypeDesc)]
	public delegate void DrawPageEventHandler (object sender, DrawPageEventArgs e);

	[DefaultEvent("Show")]
	[DefaultProperty("CustDlgParams")]
	[Designer(typeof(CaCommonDialogDesigner))]
	[LicenseProvider(typeof(LicUuidLicenseProvider))]
	[Guid("C139A08E-9DE9-40bf-B89C-345421D801EF")]
	[TopFormSupport(false)]
	[LeftFormSupport(false)]
	[FullPreviewSupport(false)]
	[System.Drawing.ToolboxBitmap(typeof(CaPageSetupDialog))]
	[Description(DialogConsts.strCaPageSetupDialog_TypeDesc)]
	public sealed class CaPageSetupDialog : CaCommonDialog, ISupportInitialize
	{
		// fields
		Win32.PAGESETUPDLG structDlg;
		internal static CaPageSetupDialog HookOwner = null;
		CaPageSetupDialogItemCaptions _dlgItemCaptions = new CaPageSetupDialogItemCaptions();
		PrinterSettings _ps = new PrinterSettings();
		PrintDocument _doc = null;
		System.Drawing.Printing.Margins _minMargins = new System.Drawing.Printing.Margins(0,0,0,0);
		PageSettings _pageSettings;
		PrinterUnit _measureUnit = PrinterUnit.Display;
		bool _allowMargins = true;
		bool _useDefaultMinMargins = true;
		bool _useDefaultMargins = true;
		bool _allowWarning = true;
		bool _allowPagePainting = true;
		bool _allowPrinter = true;
		bool _showNetwork;
		bool _allowPaper = true;
		bool _allowOrinetation = true;
		bool _init = true;

		// ctor
		public CaPageSetupDialog () :
			base()
		{
		}

		// ISupportInitialize
		public void BeginInit ()
		{
			_init = false;
		}
		public void EndInit ()
		{
			_init = true;
		}

		public void RedrawPage ()
		{
			if (Handle == IntPtr.Zero)
				return;
			Win32.RECT r = new Win32.RECT(0, 0, 200, 200);
			Win32.RedrawWindow(Handle, ref r, IntPtr.Zero, 0x0002); //
		}
		private int Convert (int x)
		{
			if (MeasureUnit == PrinterUnit.Display)
				return PrinterUnitConvert.Convert(x, PrinterUnit.Display, PrinterUnit.ThousandthsOfAnInch);
			else if (MeasureUnit == PrinterUnit.TenthsOfAMillimeter)
				return PrinterUnitConvert.Convert(x, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.HundredthsOfAMillimeter);
			return x;
		}
		protected override bool RunDialog (IntPtr owner)
		{
			if (_pageSettings == null)
				throw new Exception(DialogConsts.strXSpecifyPageSettings);
			CaPageSetupDialog prevHookOwner = HookOwner;
			HookOwner = this;
			structDlg = new Win32.PAGESETUPDLG();
			structDlg.lStructSize = Marshal.SizeOf(typeof(Win32.PAGESETUPDLG));
			SetFlags(ref structDlg.Flags);
			structDlg.hwndOwner = owner;
			structDlg.hInstance = Process.GetCurrentProcess().Handle;
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
			if (MinMargins != null)
				structDlg.rtMinMargin = new Win32.RECT(Convert(_minMargins.Left), Convert(_minMargins.Top), Convert(_minMargins.Right), Convert(_minMargins.Bottom));
			if (PageSettings != null)
			{
				structDlg.rtMargin = new Win32.RECT(Convert(PageSettings.Margins.Left), Convert(PageSettings.Margins.Top), Convert(PageSettings.Margins.Right), Convert(PageSettings.Margins.Bottom));
				structDlg.ptPaperSize = new Win32.POINT(Convert(PageSettings.PaperSize.Width), Convert(PageSettings.PaperSize.Height));
			}
			structDlg.lpfnPagePaintHook = new PagePaintHookProc(PaintPageHookProcBase);
			structDlg.lpfnPageSetupHook = new PageSetupHookProc(PageSetupHookProcBase);
			bool retVal = Win32.ExecPageSetupDlg(ref structDlg);
			if (retVal)
				UpdatePrinterSettings(structDlg, PageSettings, _ps);
			HookOwner = prevHookOwner;
			if (!retVal)
			{
				int n = Win32.CommDlgExtendedError();
				if (n != 0)
					throw new ApplicationException(string.Format(DialogConsts.strXFunctionFailed, "PageSetupDlg", n.ToString()));
			}
			return retVal;
		}

		internal void SetFlags (ref int flags)
		{
			int n = Win32.PSD_ENABLEPAGESETUPHOOK;// | Win32.PSD_ENABLEPAGESETUPTEMPLATEHANDLE;
			if (!AllowMargins) n |= Win32.PSD_DISABLEMARGINS;
			if (!AllowOrientation) n |= Win32.PSD_DISABLEORIENTATION;
			if (!AllowPaper) n |= Win32.PSD_DISABLEPAPER;
			if (!AllowPrinter) n |= Win32.PSD_DISABLEPRINTER;
			if (!UseDefaultMargins) n |= Win32.PSD_MARGINS;
			if (!UseDefaultMinMargins && MinMargins != null) n |= Win32.PSD_MINMARGINS;
			if (!AllowWarning) n |= Win32.PSD_NOWARNING;
			if (ShowHelp) n |= Win32.PSD_SHOWHELP;
			if (!ShowNetwork) n |= Win32.PSD_NONETWORKBUTTON;
			if (!AllowPagePainting)
				n |= Win32.PSD_DISABLEPAGEPAINTING;
			else
				n |= Win32.PSD_ENABLEPAGEPAINTHOOK;
			if (MeasureUnit == PrinterUnit.Display || MeasureUnit == PrinterUnit.ThousandthsOfAnInch)
				n |= Win32.PSD_INTHOUSANDTHSOFINCHES;
			if (MeasureUnit == PrinterUnit.HundredthsOfAMillimeter || MeasureUnit == PrinterUnit.TenthsOfAMillimeter)
				n |= Win32.PSD_INHUNDREDTHSOFMILLIMETERS;
			flags = n;
		}

		private void UpdatePrinterSettings (Win32.PAGESETUPDLG data, PageSettings pageSettings, PrinterSettings printerSettings)
		{
			Margins local0;
			pageSettings.SetHdevmode(data.hDevMode);
			if (printerSettings != null)
			{
				printerSettings.SetHdevmode(data.hDevMode);
				printerSettings.SetHdevnames(data.hDevNames);
			}
			local0 = new Margins();
			local0.Left = data.rtMargin.left;
			local0.Top = data.rtMargin.top;
			local0.Right = data.rtMargin.right;
			local0.Bottom = data.rtMargin.bottom;
			System.Drawing.Size p = new System.Drawing.Size();
			p.Height = data.ptPaperSize.y;
			p.Width = data.ptPaperSize.x;
			if ((data.Flags & Win32.PSD_INHUNDREDTHSOFMILLIMETERS) == 1)
			{
				pageSettings.Margins = PrinterUnitConvert.Convert(local0, PrinterUnit.HundredthsOfAMillimeter, MeasureUnit);
				pageSettings.PaperSize.Height = PrinterUnitConvert.Convert(p.Height, PrinterUnit.HundredthsOfAMillimeter, MeasureUnit);
				pageSettings.PaperSize.Width = PrinterUnitConvert.Convert(p.Width, PrinterUnit.HundredthsOfAMillimeter, MeasureUnit);
			}
			else if ((data.Flags & Win32.PSD_INTHOUSANDTHSOFINCHES) == 1)
			{
				pageSettings.Margins = PrinterUnitConvert.Convert(local0, PrinterUnit.ThousandthsOfAnInch, MeasureUnit);
				pageSettings.PaperSize.Height = PrinterUnitConvert.Convert(p.Height, PrinterUnit.ThousandthsOfAnInch, MeasureUnit);
				pageSettings.PaperSize.Width = PrinterUnitConvert.Convert(p.Width, PrinterUnit.ThousandthsOfAnInch, MeasureUnit);
			}
		}

		internal static int PaintPageHookProcBase (IntPtr hdlg, int msg, IntPtr wparam, IntPtr lparam)
		{
			if (CaPageSetupDialog.HookOwner != null)
			{
				if (msg == Win32.WM_PSD_PAGESETUPDLG)
					return HookOwner.OnDrawPage(PageRegion.BeforeDraw, null, new Win32.RECT(0, 0, 0, 0));
				PageRegion region = PageRegion.FullPage;
				if (msg == Win32.WM_PSD_MINMARGINRECT)
					region = PageRegion.MinMargin;
				else if (msg == Win32.WM_PSD_MARGINRECT)
					region = PageRegion.Margin;
				else if (msg == Win32.WM_PSD_GREEKTEXTRECT)
					region = PageRegion.GreekText;
				else if (msg == Win32.WM_PSD_ENVSTAMPRECT)
					region = PageRegion.EnvStamp;
				else if (msg == Win32.WM_PSD_YAFULLPAGERECT)
					region = PageRegion.YAFullPage;
				Graphics g = Graphics.FromHdc(wparam);
				Win32.RECT rect = (Win32.RECT)Marshal.PtrToStructure(lparam, typeof(Win32.RECT));
				int n = HookOwner.OnDrawPage(region, g, rect);
				g.Dispose();
				return n;
			}
			return 0;
		}

		internal static int PageSetupHookProcBase (IntPtr hdlg, int msg, IntPtr wparam, IntPtr lparam)
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

		internal static int NewDlgProcInst (IntPtr hdlg, int msg, IntPtr wparam, IntPtr lparam)
		{
			bool processMsg = true;
			if ((msg == Win32.WM_COMMAND) && (Win32.LOWORD(wparam) == Win32.IDOK) && (Win32.HIWORD(wparam) == Win32.BN_CLICKED))
			{
				if (HookOwner.OnPageSetupOk())
					processMsg = false;
			}
			else if (msg == Win32.WM_DESTROY)
				HookOwner.OnHide();

			if (processMsg)
				return Win32.CallWindowProc(HookOwner.prevDlgProc, HookOwner.Handle, msg, wparam, lparam);
			else
				return 0;
		}

		// events/event handlers
		[Description(DialogConsts.strCaPageSetupDialog_PageSetupOk)]
		public event CancelEventHandler PageSetupOk;

		internal bool OnPageSetupOk ()
		{
			CancelEventArgs e = new CancelEventArgs();
			if (PageSetupOk != null)
				PageSetupOk(this, e);
			return e.Cancel;
		}

		[Description(DialogConsts.strCaPageSetupDialog_DrawPage)]
		public event DrawPageEventHandler DrawPage;

		internal int OnDrawPage (PageRegion region, Graphics g, Win32.RECT rect)
		{
			if (DrawPage != null)
			{
				DrawPageEventArgs e = new DrawPageEventArgs(region, g, rect);
				DrawPage(this, e);
				return e.Cancel ? 1 : 0;
			}
			return 0;
		}

        // properties
		[Description(DialogConsts.strPropDesc_ActiveDialog)]
		public static CaPageSetupDialog ActiveDialog { get {return HookOwner;} }
        [Browsable(false)]
        [Description(DialogConsts.strCaPageSetupDialog_PrinterSettings)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PrinterSettings PrinterSettings
        {
            get {return _ps;}
            set
            {
                _ps = value;
                _doc = null;
            }
        }

		[Description(DialogConsts.strCaPageSetupDialog_PageSettings)]
		[Browsable(false)]
        public PageSettings PageSettings
        {
            get
            {
                return _pageSettings;
            }
            set
            {
                _pageSettings = value;
                _doc = null;
            }
        }

		[Description(DialogConsts.strCaPageSetupDialog_Document)]
        [DefaultValue(null)]
        public PrintDocument Document
        {
            get {return _doc;}
            set
            {
                _doc = value;
                if (_doc != null)
                {
                    _pageSettings = _doc.DefaultPageSettings;
                    _ps = _doc.PrinterSettings;
                }
            }
        }

		[Description(DialogConsts.strCaPageSetupDialog_MinMargins)]
        public System.Drawing.Printing.Margins MinMargins
        {
            get {return _minMargins;}
            set
            {
                if (value == null)
                    value = new Margins(0, 0, 0, 0);
                _minMargins = value;
				if (_init)
					UseDefaultMinMargins = false;
            }
        }

		[Description(DialogConsts.strCaPageSetupDialog_AllowMargins)]
		[DefaultValue(true)]
		public bool AllowMargins
		{
			get {return _allowMargins;}
			set {_allowMargins = value;}
		}

        [Description(DialogConsts.strCaPageSetupDialog_UseDefaultMinMargins)]
        [DefaultValue(true)]
        public bool UseDefaultMinMargins
        {
            get {return _useDefaultMinMargins;}
            set {_useDefaultMinMargins = value;}
        }

        [Description(DialogConsts.strCaPageSetupDialog_AllowWarning)]
        [DefaultValue(true)]
        public bool AllowWarning
        {
            get {return _allowWarning;}
            set {_allowWarning = value;}
        }

        [Description(DialogConsts.strCaPageSetupDialog_AllowPagePainting)]
        [DefaultValue(true)]
        public bool AllowPagePainting
        {
            get {return _allowPagePainting;}
            set {_allowPagePainting = value;}
        }

        [DefaultValue(false)]
        [Description(DialogConsts.strCaPageSetupDialog_ShowNetwork)]
        public bool ShowNetwork
        {
            get {return _showNetwork;}
            set {_showNetwork = value;}
        }

        [DefaultValue(true)]
        [Description(DialogConsts.strCaPageSetupDialog_AllowPaper)]
        public bool AllowPaper
        {
            get {return _allowPaper;}
            set {_allowPaper = value;}
        }

        [DefaultValue(true)]
        [Description(DialogConsts.strCaPageSetupDialog_AllowPrinter)]
        public bool AllowPrinter
        {
            get {return _allowPrinter;}
            set {_allowPrinter = value;}
        }

        [DefaultValue(true)]
        [Description(DialogConsts.strCaPageSetupDialog_AllowOrientation)]
        public bool AllowOrientation
        {
            get {return _allowOrinetation;}
            set {_allowOrinetation = value;}
        }

        [DefaultValue(true)]
        [Description(DialogConsts.strCaPageSetupDialog_UseDefaultMargins)]
        public bool UseDefaultMargins
        {
            get {return _useDefaultMargins;}
            set {_useDefaultMargins = value;}
        }

		[DefaultValue(PrinterUnit.Display)]
        [Description(DialogConsts.strCaPageSetupDialog_MeasureUnits)]
        public PrinterUnit MeasureUnit
        {
            get {return _measureUnit;}
            set {_measureUnit = value;}
        }

    }
}
