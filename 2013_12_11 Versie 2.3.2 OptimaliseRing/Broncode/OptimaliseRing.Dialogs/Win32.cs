using System;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ComponentAge.PlatformServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class Win32
    {
        // Common consts
        public static readonly int MAX_PATH = 1024;

        public static readonly int MF_BYCOMMAND = 0x00000000;
        public static readonly int MF_GRAYED    = 0x00000001;


        public static readonly int HWND_TOP = 0;
        public static readonly int HWND_BOTTOM = 1;
        public static readonly int HWND_TOPMOST = -1;
        public static readonly int HWND_NOTOPMOST = -2;

        public static readonly int SW_SHOW = 5;
        public static readonly int SW_HIDE = 0;
        public static readonly int SWP_NOSIZE = 1;
        public static readonly int SWP_NOMOVE = 2;
        public static readonly int SWP_NOZORDER = 4;
        public static readonly int SWP_NOREDRAW = 8;
        public static readonly int SWP_NOACTIVATE = 0x00000010;
        public static readonly int SWP_SHOWWINDOW = 0x00000040;
        public static readonly int SWP_HIDEWINDOW = 0x00000080;
        public static readonly int SWP_FRAMECHANGED = 0x20;

        public static readonly int GWL_STYLE = -16;
        public static readonly int GWL_WNDPROC = -4;
        public static readonly int DWL_DLGPROC = 4;
        public static readonly int DWL_MSGRESULT = 0;
        public static readonly int DWL_USER = 8;

        public static readonly int GW_CHILD = 5;
        public static readonly int GW_ENABLEDPOPUP = 6;

        public static readonly int WS_TABSTOP = 0x10000;
        public static readonly int WS_CHILD = 0x40000000;
        public static readonly int WS_CLIPCHILDREN = 0x2000000;
        public static readonly int WS_CAPTION = 0xC00000;
        public static readonly int WS_THICKFRAME = 0x40000;
        public static readonly int WS_SYSMENU = 0x80000;
        //public static readonly int WS_POPUP            = 0x800000000;
        public static readonly int WS_BORDER = 0x800000;
        //public static readonly int WS_POPUPWINDOW      = WS_POPUP | WS_BORDER | WS_SYSMENU;

        public const int WH_CALLWNDPROC = 4;
        public const int WH_CALLWNDPROCRET = 12;
        public const int WH_KEYBOARD = 2;

        public const int VK_DELETE = 0x2E;
        public const int VK_F5 = 0x74;

        public static readonly int SM_CXDLGFRAME = 7;
        public static readonly int SM_CYDLGFRAME = 8;
        public static readonly int SM_CXSCREEN = 0;
        public static readonly int SM_CYSCREEN = 1;
        public static readonly int VER_PLATFORM_WIN32_WINDOWS = 1;
        public static readonly int VER_PLATFORM_WIN32_NT = 2;

        // Messages
        public static readonly int WM_SYSKEYDOWN = 0x0104;
        public static readonly int WM_KEYDOWN = 0x0100;
        public static readonly int WM_INITDIALOG = 0x00000110;
        public static readonly int WM_GETMINMAXINFO = 0x0024;
        public static readonly int WM_DESTROY = 0x0002;
        public static readonly int WM_NOTIFY = 0x004E;
        public static readonly int WM_CONTEXTMENU = 0x007B;
        public static readonly int WM_COMMAND = 0x0111;
        public static readonly int WM_USER = 0x0400;
        public static readonly int WM_NCPAINT = 0x0085;
        public static readonly int WM_PAINT = 0x000F;
        public static readonly int WM_SIZE = 0x0005;
        public static readonly int WM_HELP = 0x0053;
        public static readonly int BM_CLICK = 0x00F5;
        public static readonly int WM_GETTEXT = 0x000D;
        public static readonly int WM_SETTEXT = 0x000C;
        public static readonly int WM_SETICON = 0x0080;
        public static readonly int WM_CLOSE = 0x0010;
        public static readonly int WM_PSD_PAGESETUPDLG = WM_USER;
        public static readonly int WM_PSD_FULLPAGERECT = WM_USER + 1;
        public static readonly int WM_PSD_MINMARGINRECT = WM_USER + 2;
        public static readonly int WM_PSD_MARGINRECT = WM_USER + 3;
        public static readonly int WM_PSD_GREEKTEXTRECT = WM_USER + 4;
        public static readonly int WM_PSD_ENVSTAMPRECT = WM_USER + 5;
        public static readonly int WM_PSD_YAFULLPAGERECT = WM_USER + 6;
        public static readonly int WM_ERASEBKGND = 0x0014;
        public static readonly int WM_INITMENUPOPUP = 0x0117;
        public static readonly int WM_STYLECHANGED = 0x007D;

        public static readonly int EN_CHANGE = 0x0300;
        public static readonly int CBN_EDITCHANGE = 5;
        public static readonly int CBN_SELCHANGE = 1;
        public static readonly int CBN_DROPDOWN = 7;

        public const int IDOK = 1;
        public const int IDCANCEL = 2;
        public const int IDHELP = 3;
        public const int BN_CLICKED = 0;
        public const int CB_SETEDITSEL = 0x0142;


        // Common dialog consts
        public static readonly int OFN_READONLY = 0x00000001;
        public static readonly int OFN_OVERWRITEPROMPT = 0x00000002;
        public static readonly int OFN_HIDEREADONLY = 0x00000004;
        public static readonly int OFN_NOCHANGEDIR = 0x00000008;
        public static readonly int OFN_SHOWHELP = 0x00000010;
        public static readonly int OFN_ENABLEHOOK = 0x00000020;
        public static readonly int OFN_NOVALIDATE = 0x00000100;
        public static readonly int OFN_ALLOWMULTISELECT = 0x00000200;
        public static readonly int OFN_EXTENSIONDIFFERENT = 0x00000400;
        public static readonly int OFN_PATHMUSTEXISTS = 0x00000800;
        public static readonly int OFN_FILEMUSTEXISTS = 0x00001000;
        public static readonly int OFN_CREATEPROMPT = 0x00002000;
        public static readonly int OFN_NOREADONLYRETURN = 0x00008000;
        public static readonly int OFN_NOTESTFILECREATE = 0x00010000;
        public static readonly int OFN_NONETWORKBUTTON = 0x00020000;
        public static readonly int OFN_EXPLORER = 0x00080000;
        public static readonly int OFN_NODEREFERENCELINKS = 0x00100000;
        public static readonly int OFN_ENABLEINCLUDENOTIFY = 0x00400000;
        public static readonly int OFN_DONTADDTORECENT = 0x02000000;
        public static readonly int OFN_FORCESHOWHIDDEN = 0x10000000;
        public static readonly int OFN_ENABLESIZING = 0x00800000;
        public static readonly int OFN_EX_NOPLACESBAR = 0x00000001;

        public static readonly int CDN_FIRST = -601;
        public static readonly int CDN_INITDONE = CDN_FIRST - 0;
        public static readonly int CDN_SELCHANGE = CDN_FIRST - 1;
        public static readonly int CDN_FOLDERCHANGE = CDN_FIRST - 2;
        public static readonly int CDN_SHAREVIOLATION = CDN_FIRST - 3;
        public static readonly int CDN_HELP = CDN_FIRST - 4;
        public static readonly int CDN_FILEOK = CDN_FIRST - 5;
        public static readonly int CDN_TYPECHANGE = CDN_FIRST - 6;
        public static readonly int CDN_INCLUDEITEM = CDN_FIRST - 7;

        public static readonly int CDM_FIRST = WM_USER + 100;
        public static readonly int CDM_LAST = WM_USER + 200;
        public static readonly int CDM_GETSPEC = CDM_FIRST + 0;
        public static readonly int CDM_GETFILEPATH = CDM_FIRST + 1;
        public static readonly int CDM_GETFOLDEREPATH = CDM_FIRST + 2;
        public static readonly int CDM_GETFOLDERIDLIST = CDM_FIRST + 3;
        public static readonly int CDM_SETCONTROLTEXT = CDM_FIRST + 4;
        public static readonly int CDM_HIDECONTROL = CDM_FIRST + 5;
        public static readonly int CDM_SETDEFEXT = CDM_FIRST + 6;

        public const int lst1 = 0x0460;
        public const int lst2 = 0x0461;
        public const int edt1 = 0x0480;
        public const int edt2 = 0x0481;
        public const int edt3 = 0x0482;
        public const int cmb1 = 0x0470;
        public const int cmb13 = 0x047c;
        public const int chx1 = 0x0410;
        public const int stc2 = 0x0441;
        public const int stc3 = 0x0442;
        public const int stc4 = 0x0443;
        public const int cmb2 = 0x0471;
        public const int cmb4 = 0x0473;
        public const int pshHelp = 0x040e;
        public const int psh2 = 0x0401;

        public static readonly int BIF_RETURNONLYFSDIRS = 0x0001;  // For finding a folder to start document searching
        public static readonly int BIF_DONTGOBELOWDOMAIN = 0x0002;  // For starting the Find Computer
        public static readonly int BIF_STATUSTEXT = 0x0004;   // Top of the dialog has 2 lines of text for BROWSEINFO.lpszTitle and one line if
        public static readonly int BIF_RETURNFSANCESTORS = 0x0008;
        public static readonly int BIF_EDITBOX = 0x0010;   // Add an editbox to the dialog
        public static readonly int BIF_VALIDATE = 0x0020;   // insist on valid result (or CANCEL)
        public static readonly int BIF_NEWDIALOGSTYLE = 0x0040;   // Use the new dialog layout with the ability to resize
        public static readonly int BIF_USENEWUI = (BIF_NEWDIALOGSTYLE | BIF_EDITBOX);
        public static readonly int BIF_BROWSEINCLUDEURLS = 0x0080;   // Allow URLs to be displayed or entered. (Requires BIF_USENEWUI)
        public static readonly int BIF_UAHINT = 0x0100;   // Add a UA hint to the dialog, in place of the edit box. May not be combined with BIF_EDITBOX
        public static readonly int BIF_NONEWFOLDERBUTTON = 0x0200;   // Do not add the "New Folder" button to the dialog.  Only applicable with BIF_NEWDIALOGSTYLE.
        public static readonly int BIF_NOTRANSLATETARGETS = 0x0400;   // don't traverse target as shortcut
        public static readonly int BIF_BROWSEFORCOMPUTER = 0x1000;  // Browsing for Computers.
        public static readonly int BIF_BROWSEFORPRINTER = 0x2000;  // Browsing for Printers
        public static readonly int BIF_BROWSEINCLUDEFILES = 0x4000;  // Browsing for Everything
        public static readonly int BIF_SHAREABLE = 0x8000;  // sharable resources displayed (remote shares, requires BIF_USENEWUI)

        // message from browseforfolder procedure
        public static readonly int BFFM_INITIALIZED = 1;
        public static readonly int BFFM_SELCHANGED = 2;
        public static readonly int BFFM_SETSELECTIONA = WM_USER + 102;
        public static readonly int BFFM_SETSELECTIONW = WM_USER + 103;

        // printdlg
        public static readonly int PD_ALLPAGES = 0x00000000;
        public static readonly int PD_SELECTION = 0x00000001;
        public static readonly int PD_PAGENUMS = 0x00000002;
        public static readonly int PD_NOSELECTION = 0x00000004;
        public static readonly int PD_NOPAGENUMS = 0x00000008;
        public static readonly int PD_COLLATE = 0x00000010;
        public static readonly int PD_PRINTTOFILE = 0x00000020;
        public static readonly int PD_PRINTSETUP = 0x00000040;
        public static readonly int PD_NOWARNING = 0x00000080;
        public static readonly int PD_RETURNDC = 0x00000100;
        public static readonly int PD_RETURNIC = 0x00000200;
        public static readonly int PD_RETURNDEFAULT = 0x00000400;
        public static readonly int PD_SHOWHELP = 0x00000800;
        public static readonly int PD_ENABLEPRINTHOOK = 0x00001000;
        public static readonly int PD_ENABLESETUPHOOK = 0x00002000;
        public static readonly int PD_ENABLEPRINTTEMPLATE = 0x00004000;
        public static readonly int PD_ENABLESETUPTEMPLATE = 0x00008000;
        public static readonly int PD_ENABLEPRINTTEMPLATEHANDLE = 0x00010000;
        public static readonly int PD_ENABLESETUPTEMPLATEHANDLE = 0x00020000;
        public static readonly int PD_USEDEVMODECOPIES = 0x00040000;
        public static readonly int PD_USEDEVMODECOPIESANDCOLLATE = 0x00040000;
        public static readonly int PD_DISABLEPRINTTOFILE = 0x00080000;
        public static readonly int PD_HIDEPRINTTOFILE = 0x00100000;
        public static readonly int PD_NONETWORKBUTTON = 0x00200000;

        // list view
        public static readonly int LVM_FIRST = 0x1000;
        public static readonly int LVM_GETBKCOLOR = LVM_FIRST;
        public static readonly int LVM_SETBKCOLOR = LVM_FIRST + 1;
        public static readonly int LVM_GETITEMCOUNT = LVM_FIRST + 4;
        public static readonly int LVM_GETSELECTEDCOUNT = LVM_FIRST + 50;
        public static readonly int LVM_SORTITEMS = LVM_FIRST + 48;
        public static readonly int LVM_ARRANGEITEMS = LVM_FIRST + 22;
        public static readonly int LVM_GETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 55;
        public static readonly int LVM_SETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 54;
        public static readonly int LVM_GETVIEW = LVM_FIRST + 143;
        public static readonly int LVM_SETVIEW = LVM_FIRST + 142;

        public static readonly int LV_VIEW_ICON = 0x0000;
        public static readonly int LV_VIEW_DETAILS = 0x0001;
        public static readonly int LV_VIEW_SMALLICON = 0x0002;
        public static readonly int LV_VIEW_LIST = 0x0003;
        public static readonly int LV_VIEW_TILE = 0x0004;
        public static readonly int LV_VIEW_MAX = 0x0004;

        public static readonly int LVS_ICON = 0x0000;
        public static readonly int LVS_REPORT = 0x0001;
        public static readonly int LVS_SMALLICON = 0x0002;
        public static readonly int LVS_LIST = 0x0003;

        public static readonly int LVS_EX_FULLROWSELECT = 0x00000020;
        public static readonly int LVS_EX_FLATSB = 0x00000100;
        public static readonly int LVS_EX_GRIDLINES = 0x00000001;
        public static readonly int LVS_EX_TRACKSELECT = 0x00000008;
        public static readonly int LVS_EX_UNDERLINECOLD = 0x00001000;
        public static readonly int LVS_EX_UNDERLINEHOT = 0x00000800;
        public static readonly int LVS_EX_ONECLICKACTIVATE = 0x00000040;
        public static readonly int LVS_EX_TWOCLICKACTIVATE = 0x00000080;
        public static readonly int LVS_EDITLABELS = 0x0200;
        public static readonly int LVS_TYPEMASK = 0x0003;

        public static readonly int SHVIEW_LARGEICON = 28713;
        public static readonly int SHVIEW_SMALLICON = 28714;
        public static readonly int SHVIEW_LIST = 28715;
        public static readonly int SHVIEW_REPORT = 28716;
        public static readonly int SHVIEW_THUMBNAIL = 28717;
        public static readonly int SHVIEW_TILE = 28718;

        // run dialog
        public static readonly int RFF_NOBROWSE = 1;
        public static readonly int RFF_NODEFAULT = 2;
        public static readonly int RFF_CALCDIRECTORY = 4;
        public static readonly int RFF_NOLABEL = 8;

        // find/replace
        public static readonly string FINDMSGSTRING = "commdlg_FindReplace";

        public static readonly int FR_DOWN = 0x00000001;
        public static readonly int FR_WHOLEWORD = 0x00000002;
        public static readonly int FR_MATCHCASE = 0x00000004;
        public static readonly int FR_FINDNEXT = 0x00000008;
        public static readonly int FR_REPLACE = 0x00000010;
        public static readonly int FR_REPLACEALL = 0x00000020;
        public static readonly int FR_DIALOGTERM = 0x00000040;
        public static readonly int FR_SHOWHELP = 0x00000080;
        public static readonly int FR_ENABLEHOOK = 0x00000100;
        public static readonly int FR_ENABLETEMPLATE = 0x00000200;
        public static readonly int FR_NOUPDOWN = 0x00000400;
        public static readonly int FR_NOMATCHCASE = 0x00000800;
        public static readonly int FR_NOWHOLEWORD = 0x00001000;
        public static readonly int FR_ENABLETEMPLATEHANDLE = 0x00002000;
        public static readonly int FR_HIDEUPDOWN = 0x00004000;
        public static readonly int FR_HIDEMATCHCASE = 0x00008000;
        public static readonly int FR_HIDEWHOLEWORD = 0x00010000;


        public static readonly int PSD_DEFAULTMINMARGINS = 0x00000000; // default (printer's)
        public static readonly int PSD_INWININIINTLMEASURE = 0x00000000; // 1st of 4 possible
        public static readonly int PSD_MINMARGINS = 0x00000001; // use caller's
        public static readonly int PSD_MARGINS = 0x00000002; // use caller's
        public static readonly int PSD_INTHOUSANDTHSOFINCHES = 0x00000004; // 2nd of 4 possible
        public static readonly int PSD_INHUNDREDTHSOFMILLIMETERS = 0x00000008; // 3rd of 4 possible
        public static readonly int PSD_DISABLEMARGINS = 0x00000010;
        public static readonly int PSD_DISABLEPRINTER = 0x00000020;
        public static readonly int PSD_NOWARNING = 0x00000080; // must be same as PD_*
        public static readonly int PSD_DISABLEORIENTATION = 0x00000100;
        public static readonly int PSD_RETURNDEFAULT = 0x00000400; // must be same as PD_*
        public static readonly int PSD_DISABLEPAPER = 0x00000200;
        public static readonly int PSD_SHOWHELP = 0x00000800; // must be same as PD_*
        public static readonly int PSD_ENABLEPAGESETUPHOOK = 0x00002000; // must be same as PD_*
        public static readonly int PSD_ENABLEPAGESETUPTEMPLATE = 0x00008000; // must be same as PD_*
        public static readonly int PSD_ENABLEPAGESETUPTEMPLATEHANDLE = 0x00020000; // must be same as PD_*
        public static readonly int PSD_ENABLEPAGEPAINTHOOK = 0x00040000;
        public static readonly int PSD_DISABLEPAGEPAINTING = 0x00080000;
        public static readonly int PSD_NONETWORKBUTTON = 0x00200000; // must be same as PD_*

        internal static readonly int TB_SETBUTTONINFOW = WM_USER + 64;
        internal static readonly int TB_SETBUTTONINFOA = WM_USER + 66;

        internal static int TB_SETBUTTONINFO
        {
            get
            {
                if (Marshal.SystemDefaultCharSize == 2)
                {
                    return TB_SETBUTTONINFOW;
                }
                else
                {
                    return TB_SETBUTTONINFOA;
                }
            }
        }

        internal static readonly int TB_ENABLEBUTTON = WM_USER + 1;
        internal static readonly int TB_GETSTATE = WM_USER + 18;
        internal static readonly int TB_SETSTATE = WM_USER + 17;
        internal static readonly int TBSTATE_ENABLED = 0x04;
        internal static readonly uint TBIF_IMAGE = 0x00000001;
        internal static readonly uint TBIF_TEXT = 0x00000002;
        internal static readonly uint TBIF_STATE = 0x00000004;
        internal static readonly uint TBIF_STYLE = 0x00000008;
        internal static readonly uint TBIF_LPARAM = 0x00000010;
        internal static readonly uint TBIF_COMMAND = 0x00000020;
        internal static readonly uint TBIF_SIZE = 0x00000040;
        internal static readonly uint TBIF_BYINDEX = 0x80000000;

        // Win32 structures
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Runtime.InteropServices.ComVisible(false)]
        public class PRINTDLG
        {
            public System.Int32 lStructSize;
            public IntPtr hwndOwner = IntPtr.Zero;
            public IntPtr hDevMode = IntPtr.Zero;
            public IntPtr hDevNames = IntPtr.Zero;
            public IntPtr hDC = IntPtr.Zero;
            public System.Int32 Flags = 0;
            public System.Int16 nFromPage = 0;
            public System.Int16 nToPage = 0;
            public System.Int16 nMinPage = 0;
            public System.Int16 nMaxPage = 0;
            public System.Int16 nCopies = 0;
            public IntPtr hInstance = IntPtr.Zero;
            public System.IntPtr lCustData = IntPtr.Zero;
            public PrintHookProc lpfnPrintHook;
            public SetupHookProc lpfnSetupHook;
            public IntPtr lpPrintTemplateName = IntPtr.Zero;
            public IntPtr lpSetupTemplateName = IntPtr.Zero;
            public IntPtr hPrintTemplate = IntPtr.Zero;
            public IntPtr hSetupTemplate = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class BROWSEINFO
        {
            public IntPtr hwndOwner = IntPtr.Zero;
            public IntPtr pidlRoot = IntPtr.Zero;
            public string displayName = new string(' ', 2048);
            public string title;
            public int flags = 0;
            public OFNHookProc lpfn;
            public IntPtr lparam;
            public int image = 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class OpenFileNameEx
        {
            public int lStructSize;
            public IntPtr hwndOwner;
            public IntPtr hInstance;
            public string lpstrFilter;
            public IntPtr lpstrCustomFilter;
            public int nMaxCustFilter;
            public int nFilterIndex;
            public IntPtr lpstrFile;
            public int nMaxFile;
            public IntPtr lpstrFileTitle;
            public int nMaxFileTitle;
            public string lpstrInitialDir;
            public string lpstrTitle;
            public int Flags;
            public short nFileOffset;
            public short nFileExtension;
            public string lpstrDefExt;
            public IntPtr lCustData;
            public OFNHookProc hook;
            public string templateName;
            public IntPtr pvReserved;
            public int dwReserved;
            public int FlagsEx;

            public OpenFileNameEx()
            {
                this.lStructSize = Marshal.SizeOf(typeof(OpenFileNameEx));
                this.lpstrCustomFilter = IntPtr.Zero;
                this.nMaxFile = 260;
                this.lpstrFileTitle = IntPtr.Zero;
                this.nMaxFileTitle = 260;
                this.lCustData = IntPtr.Zero;
                this.pvReserved = IntPtr.Zero;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct OpenFileName
        {
            public int structSize;// = 0;
            public IntPtr dlgOwner;// = IntPtr.Zero;
            public IntPtr instance;// = IntPtr.Zero;
            public string filter;// = null;
            public string customFilter;// = null;
            public int maxCustFilter;// = 0;
            public int filterIndex;// = 0;
            [MarshalAs(UnmanagedType.LPTStr, SizeConst = 200000)]
            public string file;// = null;
            public int maxFile;// = 0;
            [MarshalAs(UnmanagedType.LPTStr, SizeConst = 2048)]
            public string fileTitle;// = null;
            public int maxFileTitle;// = 0;
            public string initialDir;// = null;
            public string title;// = null;
            public int flags;// = 0;
            public short fileOffset;// = 0;
            public short fileExtension;// = 0;
            public string defExt;// = null;
            public IntPtr custData;// = IntPtr.Zero;
            public OFNHookProc hook;
            public string templateName;// = null;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class NMHDR
        {
            public IntPtr hwndFrom = IntPtr.Zero;
            public int idFrom = 0;
            public int code = 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class OFNOTIFY
        {
            public NMHDR hdr = null;
            public IntPtr ofn = IntPtr.Zero;
            public IntPtr pszFile = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct PAGESETUPDLG
        {
            public int lStructSize;
            public IntPtr hwndOwner;
            public IntPtr hDevMode;
            public IntPtr hDevNames;
            public int Flags;
            public Win32.POINT ptPaperSize;
            public Win32.RECT rtMinMargin;
            public Win32.RECT rtMargin;
            public IntPtr hInstance;
            public IntPtr lCustData;
            public PageSetupHookProc lpfnPageSetupHook;
            public PagePaintHookProc lpfnPagePaintHook;
            public string lpPageSetupTemplateName;
            public IntPtr hPageSetupTemplate;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ITEMIDLIST
        {
            internal IntPtr mkid;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            internal int x;
            internal int y;
            internal POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class CWPSTRUCT
        {
            public IntPtr lparam = IntPtr.Zero;
            public IntPtr wparam = IntPtr.Zero;
            public int message = 0;
            public IntPtr hwnd = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class OSVersionInfo
        {
            public int OSVersionInfoSize;
            public int majorVersion = 0;
            public int minorVersion = 0;
            public int buildNumber = 0;
            public int platformId = 0;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public String versionString = "";
        }

        // Toolbar
        [StructLayout(LayoutKind.Sequential)]
        internal struct TBBUTTONINFO
        {
            public uint cbSize;
            public uint dwMask;
            public int idCommand;
            public int iImage;
            public byte fsState;
            public byte fsStyle;
            public short cx;
            public UIntPtr lParam;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszText;
            public int cchText;
        }

        [StructLayout(LayoutKind.Sequential)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct RECT
        {
            internal int left;
            internal int top;
            internal int right;
            internal int bottom;

            internal RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            internal RECT(Rectangle r)
            {
                this.left = r.Left;
                this.top = r.Top;
                this.right = r.Left + r.Width;
                this.bottom = r.Top + r.Height;
            }

            internal static RECT FromXYWH(int x, int y, int width, int height)
            {
                return new RECT(x, y, x + width, y + height);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class FINDREPLACE
        {
            public int lStructSize = 0;
            public IntPtr hwndOwner = IntPtr.Zero;
            public IntPtr hInstance = IntPtr.Zero;
            public int Flags = 0;
            public String lpstrFindWhat = null;
            public String lpstrReplaceWith = null;
            public short wFindWhatLen = 0;
            public short wReplaceWithLen = 0;
            public IntPtr lCustData = IntPtr.Zero;
            public IntPtr lpfnHook = IntPtr.Zero;
            public String lpTemplateName = null;
        }


        // Helper methods/properties
        internal static IntPtr IntPtrFromPoint(Point pt)
        {
            return (IntPtr)(pt.X | (pt.Y << 16));
        }
        internal static Point PointFromIntPtr(IntPtr intPtr)
        {
            return new Point((int)intPtr & 0xff, (int)intPtr >> 16);
        }

        internal static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff;
        }

        internal static int HIWORD(IntPtr n)
        {
            return HIWORD((int)n);
        }

        internal static int LOWORD(int n)
        {
            return n & 0xffff;
        }

        internal static int LOWORD(IntPtr n)
        {
            return LOWORD((int)n);
        }
        internal static long MAKELONG(ushort a, ushort b)
        {
            return ((int)(((ushort)(a)) | ((int)((ushort)(b))) << 16));
        }


        // Imports
        [DllImport("KERNEL32.DLL")]
        internal static extern IntPtr GlobalFree(IntPtr handle);

        internal static bool PostMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam)
        {
            if (Marshal.SystemDefaultCharSize == 2)
            {
                return PostMessageW(hWnd, wMsg, wParam, lParam);
            }

            return PostMessageA(hWnd, wMsg, wParam, lParam);
        }

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "PostMessageA", CharSet = CharSet.Ansi)]
        internal static extern bool PostMessageA(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "PostMessageW", CharSet = CharSet.Unicode)]
        internal static extern bool PostMessageW(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        internal static IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam)
        {
            if (Marshal.SystemDefaultCharSize == 2)
            {
                return SendMessageW(hWnd, wMsg, wParam, lParam);
            }

            return SendMessageA(hWnd, wMsg, wParam, lParam);
        }

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "SendMessageA", CharSet = CharSet.Ansi)]
        internal static extern IntPtr SendMessageA(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "SendMessageW", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SendMessageW(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        internal static IntPtr SendFileDialogMessage(IntPtr hWnd, int wMsg, int len, StringBuilder str)
        {
            if (Marshal.SystemDefaultCharSize == 2)
            {
                return SendFileDialogMessageW(hWnd, wMsg, len, str);
            }

            return SendFileDialogMessageA(hWnd, wMsg, len, str);
        }

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "SendMessageA", CharSet = CharSet.Ansi)]
        internal static extern IntPtr SendFileDialogMessageA(IntPtr hWnd, int wMsg, int len, StringBuilder str);

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "SendMessageW", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SendFileDialogMessageW(IntPtr hWnd, int wMsg, int len, StringBuilder str);

        internal static IntPtr SendToolBarMessage(IntPtr hWnd, int wMsg, int index, ref TBBUTTONINFO info)
        {
            if (Marshal.SystemDefaultCharSize == 2)
            {
                return SendToolBarMessageW(hWnd, wMsg, index, ref info);
            }

            return SendToolBarMessageA(hWnd, wMsg, index, ref info);
        }

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "SendMessageA", CharSet = CharSet.Ansi)]
        internal static extern IntPtr SendToolBarMessageA(IntPtr hWnd, int wMsg, int index, ref TBBUTTONINFO info);

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "SendMessageW", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SendToolBarMessageW(IntPtr hWnd, int wMsg, int index, ref TBBUTTONINFO info);

        //[DllImport("ComponentAge.PlatformServices.Dll", ExactSpelling = true, EntryPoint = "GetSelectedFolderFromPidl", CharSet = CharSet.Ansi)]
        //internal static extern int GetSelectedFolderFromPidl(IntPtr hWnd, StringBuilder b, int flags);

        internal static IntPtr SendMessageWithString(IntPtr hWnd, int wMsg, int len, string str)
        {
            if (Marshal.SystemDefaultCharSize == 2)
            {
                return SendMessageWithStringW(hWnd, wMsg, len, str);
            }

            return SendMessageWithStringA(hWnd, wMsg, len, str);
        }

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "SendMessageA", CharSet = CharSet.Ansi)]
        internal static extern IntPtr SendMessageWithStringA(IntPtr hWnd, int wMsg, int len, string str);

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "SendMessageW", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SendMessageWithStringW(IntPtr hWnd, int wMsg, int len, string str);

        internal static IntPtr SendMessageWithPidl(IntPtr hWnd, int wMsg, int len, out IntPtr pidl)
        {
            if (Marshal.SystemDefaultCharSize == 2)
            {
                return SendMessageWithPidlW(hWnd, wMsg, len, out pidl);
            }

            return SendMessageWithPidlA(hWnd, wMsg, len, out pidl);
        }

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "SendMessageA", CharSet = CharSet.Ansi)]
        internal static extern IntPtr SendMessageWithPidlA(IntPtr hWnd, int wMsg, int len, out IntPtr pidl);

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "SendMessageW", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SendMessageWithPidlW(IntPtr hWnd, int wMsg, int len, out IntPtr pidl);


        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "MoveWindow", CharSet = CharSet.Auto)]
        internal static extern IntPtr MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, int bRepaint);

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "SetWindowPos", CharSet = CharSet.Auto)]
        internal static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "GetClientRect", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetClientRect(IntPtr hWnd, ref RECT rect);

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "GetWindowRect", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "EnumChildWindows", CharSet = CharSet.Auto)]
        internal static extern IntPtr EnumChildWindows(IntPtr hWndParent, WndEnumProc proc, IntPtr lParam);

        [DllImport("USER32.DLL", ExactSpelling = true, EntryPoint = "GetParent", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("USER32.DLL", EntryPoint = "GetSystemMetrics", CharSet = CharSet.Auto)]
        internal static extern int GetSystemMetrics(int nIndex);

        [DllImport("USER32.DLL", EntryPoint = "RedrawWindow", CharSet = CharSet.Auto)]
        internal static extern int RedrawWindow(IntPtr hWnd, ref RECT rectUpdate, IntPtr hrgnUpdate, int flags);

        [DllImport("USER32.DLL", EntryPoint = "UpdateWindow", CharSet = CharSet.Auto)]
        internal static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("USER32.DLL", EntryPoint = "ScreenToClient", CharSet = CharSet.Auto)]
        internal static extern bool ScreenToClient(IntPtr hWnd, ref POINT point);

        [DllImport("USER32.DLL", EntryPoint = "BringWindowToTop", CharSet = CharSet.Auto)]
        internal static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("USER32.DLL", EntryPoint = "SetParent", CharSet = CharSet.Auto)]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("USER32.DLL", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        internal static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("USER32.DLL", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        internal static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, WindowProcedure NewWinProc);

        [DllImport("USER32.DLL", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("USER32.DLL", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetWindowLong1(IntPtr hWnd, int nIndex);

        [DllImport("USER32.DLL", EntryPoint = "CallWindowProc", CharSet = CharSet.Auto)]
        internal static extern int CallWindowProc(IntPtr WndProc, IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("kernel32")]
        public static extern bool GetVersionEx([In, Out] OSVersionInfo osvi);

        [DllImport("USER32.DLL", EntryPoint = "WindowFromPoint", CharSet = CharSet.Auto)]
        public static extern IntPtr WindowFromPoint(ref POINT pnt);

        [DllImport("USER32.DLL", EntryPoint = "GetDlgItem", CharSet = CharSet.Auto)]
        public static extern IntPtr GetDlgItem(IntPtr hdlg, int id);

        [DllImport("USER32.DLL", EntryPoint = "GetFocus", CharSet = CharSet.Auto)]
        public static extern IntPtr GetFocus();

        [DllImport("USER32.DLL", EntryPoint = "SetFocus", CharSet = CharSet.Auto)]
        public static extern IntPtr SetFocus(IntPtr wnd);

        [DllImport("USER32.DLL", EntryPoint = "SetWindowText", CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, string text);

        [DllImport("USER32.DLL", EntryPoint = "GetWindowText", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int len);

        [DllImport("USER32.DLL", EntryPoint = "GetWindowTextLength", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hwnd);

        [DllImport("SHELL32.DLL", EntryPoint = "SHBrowseForFolder", CharSet = CharSet.Auto)]
        public static extern IntPtr SHBrowseForFolder([In, Out] BROWSEINFO info);

        [DllImport("OLE32.DLL", EntryPoint = "OleInitialize", CharSet = CharSet.Auto)]
        public static extern void OleInitialize();

        [DllImport("SHELL32.DLL", EntryPoint = "SHGetPathFromIDList", CharSet = CharSet.Auto)]
        public static extern bool SHGetPathFromIDList(IntPtr pidl, StringBuilder path);

        [DllImport("SHELL32.DLL", EntryPoint = "SHGetSpecialFolderLocation", CharSet = CharSet.Auto)]
        public static extern bool SHGetSpecialFolderLocation(IntPtr owner, int nFolder, ref IntPtr pidl);

        [DllImport("SHELL32.DLL", EntryPoint = "SHGetSpecialFolderPath", CharSet = CharSet.Auto)]
        public static extern bool SHGetSpecialFolderPath(IntPtr owner, StringBuilder sb, int nFolder, bool fCreate);

        [DllImport("SHELL32.DLL", EntryPoint = "ShellAbout", CharSet = CharSet.Auto)]
        public static extern int ShellAbout(IntPtr hwndOwner, string text, string other, IntPtr icon);

        [DllImport("USER32.DLL", EntryPoint = "SetWindowsHookEx", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hinst, int threadId);

        [DllImport("USER32.DLL", EntryPoint = "CallNextHookEx", CharSet = CharSet.Auto)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wparam, IntPtr lparam);

        [DllImport("USER32.DLL", EntryPoint = "UnhookWindowsHookEx", CharSet = CharSet.Auto)]
        public static extern bool UnhookWindowsHookEx(IntPtr hook);

        [DllImport("COMDLG32.DLL", EntryPoint = "PrintDlg", CharSet = CharSet.Auto)]
        public static extern bool ExecPrintDlg([In, Out] PRINTDLG structDlg);

        [DllImport("COMDLG32.DLL", EntryPoint = "PageSetupDlg", CharSet = CharSet.Auto)]
        public static extern bool ExecPageSetupDlg([In, Out] ref PAGESETUPDLG structDlg);

        [DllImport("USER32.DLL", EntryPoint = "FindWindowEx", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childAfter, string className, IntPtr window); // to pass NULL as last param there are IntPtr instead of string

        [DllImport("SHELL32.DLL", EntryPoint = "#62", CharSet = CharSet.Auto)]
        public static extern bool PickIconDlg(IntPtr parent, string file, int maxfile, ref int iconIndex);

        [DllImport("SHELL32.DLL", EntryPoint = "#61", CharSet = CharSet.Auto)]
        public static extern bool RunFileDlg(IntPtr parent, IntPtr icon, string dir, string title, string desc, int options);

        [DllImport("COMDLG32.DLL", EntryPoint = "FindText", CharSet = CharSet.Auto)]
        public static extern IntPtr FindText(FINDREPLACE fr);

        [DllImport("USER32.DLL", EntryPoint = "GetWindow", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindow(IntPtr hwnd, int cmd);

        [DllImport("USER32.DLL", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(IntPtr hwnd, int cmd);

        [DllImport("USER32.DLL", EntryPoint = "EnableWindow", CharSet = CharSet.Auto)]
        public static extern bool EnableWindow(IntPtr hwnd, bool bEnable);

        [DllImport("USER32.DLL", EntryPoint = "GetClassName", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hwnd, StringBuilder sb, int maxCount);

        [DllImport("USER32.DLL")]
        public static extern int RegisterWindowMessage(string str);

        [DllImport("COMDLG32.DLL")]
        public static extern int CommDlgExtendedError();

        [DllImport("OLE32.DLL")]
        public static extern void CoInitialize();

        [DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetOpenFileName([In, Out] OpenFileNameEx ofn);

        //[DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

        [DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetSaveFileName([In, Out] OpenFileNameEx ofn);

        //[DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);

        [DllImport("shell32.dll")]
        public static extern int SHGetDesktopFolder(ref IShellFolder ppshf);

        [DllImport("shell32.dll")]
        public static extern Int32 SHBindToParent(
            IntPtr pidl,
            [MarshalAs(UnmanagedType.LPStruct)]
            Guid riid,
            out IntPtr ppv,
            ref IntPtr ppidlLast);

        [DllImport("shell32.dll")]
        public static extern Int32 SHGetFolderLocation(
            IntPtr hwndOwner,
            Int32 nFolder,
            IntPtr hToken,
            Int32 dwReserved,
            out IntPtr ppidl);

        [DllImport("shell32.dll")]
        public static extern Int32 SHGetMalloc(
            out IntPtr hObject);    // Address of a pointer that receives the Shell's
                                    // IMalloc interface pointer.

        [DllImport("shlwapi")]
        public static extern Int32 StrRetToBuf(ref STRRET pstr, IntPtr pidl, StringBuilder pszBuf, int cchBuf);

        [DllImport("user32.dll")]
        public static extern Int32 GetMenuItemCount(IntPtr hmenu);

        [DllImport("user32.dll")]
        public static extern Int32 GetMenuItemID(IntPtr hMenu, int nPos);

        [DllImport("user32.dll")]
        public static extern bool EnableMenuItem(IntPtr hMenu, Int32 uIDEnableItem, Int32 uEnable);
    }
}
