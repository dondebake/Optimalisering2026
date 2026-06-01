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
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;
using System.Threading;
using ComponentAge.PlatformServices;

namespace ComponentAge.Dialogs
{
    [Description(DialogConsts.strFolderDialogControl_TypeDesc)]
    public enum FolderDialogControl
    {
        [ItemID(Win32.IDCANCEL)]
        ButtonCancel,
        [ItemID(Win32.pshHelp)]
        ButtonHelp,
        [ItemID(Win32.IDOK)]
        ButtonOk,
        [ItemID(Win32.lst1)]
        TreeFolders
    }

    [LicenseProvider(typeof(LicUuidLicenseProvider))]
    [Guid("97b8ffcb-a46a-4156-879e-d1ac6c2182ec")]
    [System.Drawing.ToolboxBitmap(typeof(CaFolderDialog))]
	[Description(DialogConsts.strCaFolderDialog_TypeDesc)]
	public sealed class CaFolderDialog : CaCommonDialog
	{
        // private members
        static CaFolderDialog HookOwner;
        bool _computersOnly;
        bool _printersOnly;
        bool _dontGoBelowDomain;
        bool _newFolderButton;
        bool _newUserInterface;
        bool _editBox;
        bool _showFiles;
        bool _returnFSAncestors = true;
        bool _returnOnlyFSDirs = true;
        internal string _selectedFolder;
        SpecialFolder _specRootFolder;
        string _rootFolder;
        string _statusText;
        string _fullFolderName;
        string _displayName;

      CaAboutDialogItemCaptions _capts = new CaAboutDialogItemCaptions();
        // constructor
		public CaFolderDialog() :
			base()
		{
		}

        // methods
        [Description(DialogConsts.strCaFileDialogBase_ShowControl)]
        public void ShowControl (FolderDialogControl ctrl, bool show)
        {
			FieldInfo fi = ctrl.GetType().GetField(Enum.GetName(typeof(FolderDialogControl), ctrl));
			ItemIDAttribute[] attrs = (ItemIDAttribute[])fi.GetCustomAttributes(typeof(ItemIDAttribute), true) ;
			if (attrs.Length > 0)
				Win32.ShowWindow(Win32.GetDlgItem(Handle, attrs[0].ItemID), show ? Win32.SW_SHOW : Win32.SW_HIDE);
        }

        protected override bool RunDialog (IntPtr owner)
        {
			bool extendMe = (CustDlgParams.TopForm != null || CustDlgParams.TopFormClassName.Length > 0 ||
				CustDlgParams.LeftForm != null || CustDlgParams.LeftFormClassName.Length > 0 ||
				CustDlgParams.RightForm != null || CustDlgParams.RightFormClassName.Length > 0 ||
				CustDlgParams.BottomForm != null || CustDlgParams.BottomFormClassName.Length > 0);
			if (extendMe && NewUserInterface)
				throw new ApplicationException(DialogConsts.strXCannotUseDockedFormsWithNewUserInterface);
            CaFolderDialog prevHookOwner = HookOwner;
            HookOwner = this;
            Win32.BROWSEINFO info = new Win32.BROWSEINFO();
            if (RootFolder == "" || RootFolder == null)
            {
                IntPtr pidl = new IntPtr();
                Win32.SHGetSpecialFolderLocation(IntPtr.Zero, (int)SpecRootFolder, ref pidl);
                info.pidlRoot = pidl;
            }
            else
                info.pidlRoot = ShellUtils.GetPidlFromPath(owner, RootFolder);

            info.hwndOwner = owner;
            if (ReturnFSAncestors) info.flags = info.flags | Win32.BIF_RETURNFSANCESTORS;
            if (ReturnOnlyFSDirs) info.flags = info.flags | Win32.BIF_RETURNONLYFSDIRS;
            if (ReturnComputers) info.flags = info.flags | Win32.BIF_BROWSEFORCOMPUTER;
            if (ReturnPrinters) info.flags = info.flags | Win32.BIF_BROWSEFORPRINTER;
            if (DontGoBelowDomain) info.flags = info.flags | Win32.BIF_DONTGOBELOWDOMAIN;
            if (ShowEditBox) info.flags = info.flags | Win32.BIF_EDITBOX;
            if (ShowFiles) info.flags = info.flags | Win32.BIF_BROWSEINCLUDEFILES;
            if (!ShowNewFolderButton) info.flags = info.flags | Win32.BIF_NONEWFOLDERBUTTON;
			if (NewUserInterface) info.flags = info.flags | Win32.BIF_NEWDIALOGSTYLE;
            info.lpfn = new OFNHookProc(OFNHookProcBase);
            info.title = _statusText;
            IntPtr retVal = Win32.SHBrowseForFolder(info);
            bool returnValue = (retVal != IntPtr.Zero);
            if (returnValue)
            {
                _displayName = info.displayName;
                StringBuilder sb = new StringBuilder(2048);
                Win32.SHGetPathFromIDList(retVal, sb);
                _fullFolderName = sb.ToString();
            }
            HookOwner = prevHookOwner;
            //return true;
            return returnValue; //HKV 12-11-2009
        }

		[EditorBrowsable(EditorBrowsableState.Never)]
        public static int OFNHookProcBase (IntPtr hdlg, int msg, IntPtr wparam, IntPtr lparam)
        {
            if (msg == Win32.BFFM_INITIALIZED)
            {
                HookOwner.SetHandle(hdlg);
                HookOwner.prevDlgProc = Win32.GetWindowLong1(HookOwner.Handle, Win32.GWL_WNDPROC);
                HookOwner.newDlgProc = new WindowProcedure(NewDlgProcInst);
                Win32.SetWindowLong(HookOwner.Handle, Win32.GWL_WNDPROC, HookOwner.newDlgProc);
                HookOwner.CustDlgParams.OnInitDialog();
                HookOwner.OnShow();
            }
            else if (msg == Win32.BFFM_SELCHANGED)
            {
                if (wparam != IntPtr.Zero && HookOwner.Handle != IntPtr.Zero)
                {
                    StringBuilder b = new StringBuilder(2048);
                    Win32.SHGetPathFromIDList(wparam, b);
                    HookOwner._selectedFolder = b.ToString();
                    HookOwner.OnSelectionChanged();
                }
            }
            return 0;
        }

		[EditorBrowsable(EditorBrowsableState.Never)]
        public static int NewDlgProcInst (IntPtr hdlg, int msg, IntPtr wparam, IntPtr lparam)
        {
            if (msg == Win32.WM_DESTROY)
                HookOwner.OnHide();
            else if (msg == Win32.WM_COMMAND)
            {
                if (Win32.LOWORD(wparam) == Win32.IDOK && Win32.HIWORD(wparam) == Win32.BN_CLICKED)
                {
                    if (HookOwner.OnFolderOk())
                        return 0;
                    int retval = Win32.CallWindowProc(HookOwner.prevDlgProc, HookOwner.Handle, msg, wparam, lparam);
                    return retval;
                }
            }
            else if (msg == Win32.WM_HELP)
            {
                EventArgs e = new EventArgs();
                HookOwner.OnHelpRequest(e);
            }

            int ret = Win32.CallWindowProc(HookOwner.prevDlgProc, HookOwner.Handle, msg, wparam, lparam);
            return ret;
        }

        // events
		[Description(DialogConsts.strCaFolderDialog_SelectionChanged)]
        public event EventHandler SelectionChanged;

		[Description(DialogConsts.strCaFolderDialog_FolderOk)]
        public event CancelEventHandler FolderOk;

        // events triggers
        internal void InvokeOnSelectionChanged(object sender, System.EventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, new EventArgs());
        }

        internal void OnSelectionChanged ()
        {
            InvokeOnSelectionChanged(null, null);
        }

        internal void InvokeOnFolderOk(object sender, CancelEventArgs e)
        {
            if (FolderOk != null)
                FolderOk(this, e);
        }

        internal bool OnFolderOk ()
        {
            CancelEventArgs e = new CancelEventArgs();
            InvokeOnFolderOk(this, e);
            return e.Cancel;
        }

        // properties
        [DefaultValue(false)]
		[Description(DialogConsts.strCaFolderDialog_NewUserInterface)]
        public bool NewUserInterface
        {
            get {return _newUserInterface;}
            set {_newUserInterface = value;}
        }

		[DefaultValue(false)]
        [Description(DialogConsts.strCaFolderDialog_ShowEditBox)]
        public bool ShowEditBox
        {
            get {return _editBox;}
            set {_editBox = value;}
        }

        [DefaultValue(false)]
        public bool ShowNewFolderButton
        {
            get {return _newFolderButton;}
            set {_newFolderButton = value;}
        }

		[DefaultValue(false)]
        [Description(DialogConsts.strCaFolderDialog_ReturnComputers)]
        public bool ReturnComputers
        {
            get {return _computersOnly;}
            set
            {
                _computersOnly = value;
                if (value)
                    _printersOnly = false;
            }
        }

		[DefaultValue(false)]
        [Description(DialogConsts.strCaFolderDialog_ReturnPrinters)]
        public bool ReturnPrinters
        {
            get {return _printersOnly;}
            set
            {
                _printersOnly = value;
                if (value)
                    _computersOnly = false;
            }
        }

		[DefaultValue(false)]
        [Description(DialogConsts.strCaFolderDialog_DontGoBelowDomain)]
        public bool DontGoBelowDomain
        {
            get {return _dontGoBelowDomain;}
            set {_dontGoBelowDomain = value;}
        }

		[DefaultValue(false)]
        [Description(DialogConsts.strCaFolderDialog_ShowFiles)]
        public bool ShowFiles
        {
            get {return _showFiles;}
            set {_showFiles = value;}
        }

		[DefaultValue(true)]
        [Description(DialogConsts.strCaFolderDialog_ReturnOnlyFSDirs)]
        public bool ReturnOnlyFSDirs
        {
            get {return _returnOnlyFSDirs;}
            set {_returnOnlyFSDirs = value;}
        }

		[DefaultValue(true)]
        [Description(DialogConsts.strCaFolderDialog_ReturnFSAncestors)]
        public bool ReturnFSAncestors
        {
            get {return _returnFSAncestors;}
            set {_returnFSAncestors = value;}
        }

		[Description(DialogConsts.strPropDesc_ActiveDialog)]
        public static CaFolderDialog ActiveDialog
        {
            get {return HookOwner;}
        }

        [Browsable(false)]
		[Description(DialogConsts.strCaFolderDialog_SelectedFolder)]
        public string SelectedFolder
        {
            get {return _selectedFolder;}
            set
            {
                if (Handle != IntPtr.Zero)
                {
                    _selectedFolder = value;
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                        Win32.SendMessageWithStringW(Handle, Win32.BFFM_SETSELECTIONW, 1, _selectedFolder);
                    else
                        Win32.SendMessageWithStringA(Handle, Win32.BFFM_SETSELECTIONA, 1, _selectedFolder);
                }
            }
        }

		[Description(DialogConsts.strCaFolderDialog_SpecRootFolder)]
        public SpecialFolder SpecRootFolder
        {
            get {return _specRootFolder;}
            set {_specRootFolder = value;}
        }

		[Description(DialogConsts.strCaFolderDialog_RootFolder)]
        public string RootFolder
        {
            get {return _rootFolder;}
            set {_rootFolder = value;}
        }

		[Description(DialogConsts.strCaFolderDialog_StatusText)]
        public string StatusText
        {
            get {return _statusText;}
            set {_statusText = value;}
        }

		[Description(DialogConsts.strCaFolderDialog_FullFolderName)]
        [Browsable(false)]
        public string FullFolderName
        {
            get {return _fullFolderName;}
        }

		[Description(DialogConsts.strCaFolderDialog_DisplayName)]
        [Browsable(false)]
        public string DisplayName
        {
            get {return _displayName;}
        }

      [Description(DialogConsts.strCaFolderDialog_DlgItemsCaptions)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
      public CaDlgItemCaptions DlgItemsCaptions
      {
         get { return _capts; }
      }
    }
}
