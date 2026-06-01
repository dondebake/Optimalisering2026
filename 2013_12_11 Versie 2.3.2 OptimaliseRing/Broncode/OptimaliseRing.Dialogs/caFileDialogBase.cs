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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Design;
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.IO;
using Microsoft.Win32;

using ComponentAge.PlatformServices;

namespace ComponentAge.Dialogs
{
   [Description(DialogConsts.strCommandEventArgs_TypeDesc)]
   public class CommandEventArgs : System.EventArgs
   {
      // fields
      private int _id = 0;

      // constructor
      [Description(DialogConsts.strCommandEventArgs_Ctor)]
      public CommandEventArgs(int id)
      {
         _id = id;
      }

      // properties
      [Description(DialogConsts.strCommandEventArgs_CommandID)]
      public int CommandID { get { return _id; } }
   }

   [Description(DialogConsts.strCommandCancelEventArgs_TypeDesc)]
   public class CommandCancelEventArgs : CommandEventArgs
   {
      // fields
      private bool _cancel;

      // ctor
      [Description(DialogConsts.strCommandCancelEventArgs_Ctor)]
      public CommandCancelEventArgs(int id)
         : base(id)
      {
      }

      // properties
      [Description(DialogConsts.strCommandCancelEventArgs_Cancel)]
      public bool Cancel { get { return _cancel; } set { _cancel = value; } }
   }

   [Description(DialogConsts.strCommandEventHandler_TypeDesc)]
   public delegate void CommandEventHandler(object sender, CommandEventArgs e);

   [Description(DialogConsts.strCommandCancelEventHandler_TypeDesc)]
   public delegate void CommandCancelEventHandler(object sender, CommandCancelEventArgs e);

   [Description(DialogConsts.strDisplayNameRequestFlags_TypeDesc)]
   public class DisplayNameRequestFlags
   {
      [Description(DialogConsts.strDisplayNameRequestFlags_Normal)]
      public static readonly int Normal = 0x0000;
      [Description(DialogConsts.strDisplayNameRequestFlags_InFolder)]
      public static readonly int InFolder = 0x0001;
      [Description(DialogConsts.strDisplayNameRequestFlags_ForEditing)]
      public static readonly int ForEditing = 0x1000;
      [Description(DialogConsts.strDisplayNameRequestFlags_ForAddressBar)]
      public static readonly int ForAddressBar = 0x4000;
      [Description(DialogConsts.strDisplayNameRequestFlags_ForParsing)]
      public static readonly int ForParsing = 0x8000;
   }

   [Description(DialogConsts.strFileDialogToolButtonEnum_TypeDesc)]
   public enum FileDialogToolButton
   {
      LastVisitedFolder,
      GotoUpLevel,
      CreateFolder
   }

   [Description(DialogConsts.strFileDialogControl_TypeDesc)]
   public enum FileDialogControl
   {
      [ItemID(Win32.IDCANCEL)]
      ButtonCancel,
      [ItemID(Win32.pshHelp)]
      ButtonHelp,
      [ItemID(Win32.IDOK)]
      ButtonOk,
      [ItemID(Win32.chx1)]
      CheckReadOnly,
      [ItemID(Win32.cmb2)]
      ComboLookIn,
      [ItemID(Win32.cmb13)]
      ComboFileName,
      [ItemID(Win32.cmb1)]
      ComboFileType,
      [ItemID(Win32.lst1)]
      ListFiles,
      [ItemID(Win32.stc4)]
      StaticLookIn,
      [ItemID(Win32.stc3)]
      StaticFileName,
      [ItemID(Win32.stc2)]
      StaticFileType,
   }

   [ToolboxItem(false)]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [Description(DialogConsts.strCaPlacesBarOptions_TypeDesc)]
   public class CaPlacesBarOptions
   {
      // fields
      bool _visible = true;
      string[] _captions;

      // properties
      [DefaultValue(true)]
      [Description(DialogConsts.strCaPlacesBarOptions_Visible)]
      public bool Visible
      {
         get { return _visible; }
         set { _visible = value; }
      }
      [Description(DialogConsts.strCaPlacesBarOptions_Captions)]
      public string[] Captions
      {
         get { return _captions; }
         set { _captions = value; }
      }
   }

   [ToolboxItem(false)]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [Description(DialogConsts.strCaListViewOptions_TypeDesc)]
   public class CaListViewOptions
   {
      // fields
      bool _apply;
      bool _gridLines;
      bool _flatSB;
      bool _fullRowSelect;
      bool _enambleRename = true;
      bool _enableDelete = true;
      bool _hand;
      bool _underlineCold;
      bool _underlineHot;
      bool _track;
      bool _popupOnEmpty = true;
      bool _popupOnSelection = true;
      //bool _showFiles = true;  NEXT VERSION
      //bool _showFolders = true;

      // properties
      [DefaultValue(false)]
      [Description(DialogConsts.strCaListViewOptions_Enabled)]
      public bool Enabled
      {
         get { return _apply; }
         set { _apply = value; }
      }
      [DefaultValue(false)]
      [Description(DialogConsts.strCaListViewOptions_GridLines)]
      public bool GridLines
      {
         get { return _gridLines; }
         set { _gridLines = value; }
      }
      [DefaultValue(false)]
      [Description(DialogConsts.strCaListViewOptions_FlatScrollBars)]
      public bool FlatScrollBars
      {
         get { return _flatSB; }
         set { _flatSB = value; }
      }
      [DefaultValue(false)]
      [Description(DialogConsts.strCaListViewOptions_FullRowSelect)]
      public bool FullRowSelect
      {
         get { return _fullRowSelect; }
         set { _fullRowSelect = value; }
      }
      [DefaultValue(true)]
      [Description(DialogConsts.strCaListViewOptions_EnableRename)]
      public bool EnableRename
      {
         get { return _enambleRename; }
         set { _enambleRename = value; }
      }
      [DefaultValue(true)]
      [Description(DialogConsts.strCaListViewOptions_EnableDelete)]
      public bool EnableDelete
      {
         get { return _enableDelete; }
         set { _enableDelete = value; }
      }
      [DefaultValue(false)]
      [Description(DialogConsts.strCaListViewOptions_UnderlineCold)]
      public bool UnderlineCold
      {
         get { return _underlineCold; }
         set { _underlineCold = value; }
      }
      [DefaultValue(false)]
      [Description(DialogConsts.strCaListViewOptions_UnderlineHot)]
      public bool UnderlineHot
      {
         get { return _underlineHot; }
         set { _underlineHot = value; }
      }
      [DefaultValue(false)]
      [Description(DialogConsts.strCaListViewOptions_HandPoint)]
      public bool HandPoint
      {
         get { return _hand; }
         set { _hand = value; }
      }
      [DefaultValue(false)]
      [Description(DialogConsts.strCaListViewOptions_HotTrack)]
      public bool HotTrack
      {
         get { return _track; }
         set { _track = value; }
      }
      [DefaultValue(true)]
      [Description(DialogConsts.strCaListViewOptions_PopupOnEmpty)]
      public bool PopupOnEmpty
      {
         get { return _popupOnEmpty; }
         set { _popupOnEmpty = value; }
      }
      [DefaultValue(true)]
      [Description(DialogConsts.strCaListViewOptions_PopupOnSelection)]
      public bool PopupOnSelection
      {
         get { return _popupOnSelection; }
         set { _popupOnSelection = value; }
      }
   }

   [ToolboxItem(false)]
   [Description(DialogConsts.strCaFileDialogItemCaptions_TypeDesc)]
   public class CaFileDialogItemCaptions : CaDlgItemCaptions
   {
      // fields
      string _readOnly;
      string _lookIn;
      string _filesOfType;
      string _fileName;

      // properties
      [ItemID(Win32.stc4)]
      [Description(DialogConsts.strCaFileDialogItemCaptions_LookIn)]
      public string LookIn
      {
         get { return _lookIn; }
         set { _lookIn = value; }
      }
      [ItemID(Win32.chx1)]
      [Description(DialogConsts.strCaFileDialogItemCaptions_ReadOnly)]
      public string ReadOnly
      {
         get { return _readOnly; }
         set { _readOnly = value; }
      }
      [ItemID(Win32.stc2)]
      [Description(DialogConsts.strCaFileDialogItemCaptions_FilesOfType)]
      public string FilesOfType
      {
         get { return _filesOfType; }
         set { _filesOfType = value; }
      }
      [ItemID(Win32.stc3)]
      [Description(DialogConsts.strCaFileDialogItemCaptions_FileName)]
      public string FileName
      {
         get { return _fileName; }
         set { _fileName = value; }
      }
   }

   [Description(DialogConsts.strCaFileDialogBase_TypeDesc)]
   public abstract class CaFileDialogBase : CaCommonDialog
   {
      //fields
      [EditorBrowsable(EditorBrowsableState.Never)]
      protected Win32.OpenFileNameEx ofnEx;
      [EditorBrowsable(EditorBrowsableState.Never)]
      protected Win32.OpenFileName ofn;
      [EditorBrowsable(EditorBrowsableState.Never)]
      protected static CaFileDialogBase HookOwner = null;
      CaFileDialogItemCaptions _dlgItemCaptions = new CaFileDialogItemCaptions();
      string _fileName = "";
      string[] _fileNames = new string[] { };
      bool _derefLinks = true;
      bool _validateNames = true;
      bool _restoreDir = true;
      bool _addExt = true;
      bool _multiSelect;
      string _defExt;
      bool _checkFileExists = true;
      bool _checkPathExists = true;
      string _filter = "";
      string _initialDir = "";
      int _filterIndex = 1;
      static int _cx = 0;
      static int _cy = 0;
      bool _autoFree = true;
      CaListViewOptions _lvOptions = new CaListViewOptions();
      CaPlacesBarOptions _pbOptions = new CaPlacesBarOptions();
      string _lastVisitedFolder;
      string[] _lastVisitedFiles;
      bool _dontAddToRecent;
      RecentListCtrl _recentList = new RecentListCtrl();
      bool _showRecent;
      RecentFilesTabForm _recentTopForm = new RecentFilesTabForm();
      IntPtr _intHandle = IntPtr.Zero;
      string _recentRegKey = "";
      int _recentListCount = 10;
      int _currentFilterIndex;
      FileListViewStyle _initialFileListViewStyle = FileListViewStyle.Default;
      bool _firstTimeShow = true;
      bool _firstTimeShow2 = true;
      ContextMenu _customOkButtonMenu = null;
#if DOTNET2
      ContextMenuStrip _customOkButtonMenuStrip;
#endif
      DropDownButton _customOkBtn = new DropDownButton();
      bool _enableSizing = true;
      private Interop.CharBuffer _charBuffer;
      private IntPtr _originalListViewProc;
      private IntPtr _originalDefViewProc;
      private WindowProcedure _newDefViewProc;
      private WindowProcedure _newListViewProc;
      private IntPtr _newDefViewProcInst = IntPtr.Zero;
      private IntPtr _newListViewProcInst = IntPtr.Zero;
      private IntPtr _defViewHandle = IntPtr.Zero;
      private IntPtr _listViewHandle = IntPtr.Zero;
      //private bool _enableVistaVisualStyle;

      // constructor
      public CaFileDialogBase()
         : base()
      {
         _recentTopForm.Dialog = this;
         _recentList.Dialog = this;

         _customOkBtn.Click += new EventHandler(OnOkButtonClick);
      }

      // overrides
      protected override bool OnHookKeyboardEvent(IntPtr wparam, IntPtr lparam)
      {
         if (wparam.ToInt32() == Win32.VK_DELETE && !ListViewHandle.Equals(IntPtr.Zero))
         {
            IntPtr focus = Win32.GetFocus();
            IntPtr p = this._keyboardHooker;
            bool equal = (p.ToInt32() == 0);
            if (focus.Equals(ListViewHandle) && !equal)
            {
               return false;
            }
         }

         return base.OnHookKeyboardEvent(wparam, lparam);
      }

      protected override void OnHookEvent(IntPtr wnd, int message, IntPtr wparam, IntPtr lparam)
      {
         base.OnHookEvent(wnd, message, wparam, lparam);

         if (ApplyVistaVisualStyle())
         {
            if (message == Win32.WM_INITDIALOG)
            {
               OnWmInitDialog(wnd);
               return;
            }
         }

         if (ListViewHandle.Equals(IntPtr.Zero) || DefViewHandle.Equals(IntPtr.Zero))
         {
            return;
         }

         bool refresh = false;

         if (IsWinVista() && wnd.ToInt32() == this.Handle.ToInt32())
         {
            if (_defViewHandle.ToInt32() != DefViewHandle.ToInt32())
            {
               refresh = true;
            }
         }

         IntPtr dlgHandle = Win32.GetParent(Win32.GetParent(wnd));
         if (!dlgHandle.Equals(IntPtr.Zero) && message == Win32.LVM_SORTITEMS)
         {
            refresh = true;
         }

         if (message == 4146)
         {
            refresh = true;
         }

         if (refresh)
         {
            RecreateHwnds();
         }
      }

      //methods
      private bool RecreateHwnds()
      {
         bool recreated = false;

         IntPtr defViewHandle = this.DefViewHandle;
         if (defViewHandle.ToInt32() != _defViewHandle.ToInt32())
         {
            Win32.SendMessage(_defViewHandle, Win32.WM_DESTROY, IntPtr.Zero, IntPtr.Zero);

            _defViewHandle = defViewHandle;
            IntPtr originalDefViewProc = Win32.GetWindowLong1(defViewHandle, Win32.GWL_WNDPROC);
            _originalDefViewProc = originalDefViewProc;
            _newDefViewProc = new WindowProcedure(NewDefViewProc);
            _newDefViewProcInst = Win32.SetWindowLong(_defViewHandle, Win32.GWL_WNDPROC, _newDefViewProc);

            recreated = true;
         }

         IntPtr listViewHandle = this.ListViewHandle;
         if ((listViewHandle.ToInt32() != _listViewHandle.ToInt32()) || recreated)
         {
            Win32.SendMessage(_listViewHandle, Win32.WM_DESTROY, IntPtr.Zero, IntPtr.Zero);

            _listViewHandle = listViewHandle;
            IntPtr originalListViewProc = Win32.GetWindowLong1(listViewHandle, Win32.GWL_WNDPROC);
            _originalListViewProc = originalListViewProc;
            _newListViewProc = new WindowProcedure(NewListViewProc);
            _newListViewProcInst = Win32.SetWindowLong(_listViewHandle, Win32.GWL_WNDPROC, _newListViewProc);

            recreated = true;
         }

         return recreated;
      }

      private void PostApplyCustomListViewStyles()
      {
         if (!ListViewHandle.Equals(IntPtr.Zero))
         {
            Win32.PostMessage(Handle, Win32.WM_USER + 1976, IntPtr.Zero, IntPtr.Zero);
         }
      }

      private void ApplyCustomListViewStyles()
      {
         int style = Win32.GetWindowLong(ListViewHandle, Win32.GWL_STYLE);
         int newstyle = style & (int)GetListViewStyle();

         if (newstyle != style)
         {
            Win32.SetWindowLong(ListViewHandle, Win32.GWL_STYLE, style);
         }

         int[] checkIt = { Win32.LVS_EX_FULLROWSELECT,
                       Win32.LVS_EX_FLATSB,
                       Win32.LVS_EX_GRIDLINES,
                       Win32.LVS_EX_ONECLICKACTIVATE,
                       Win32.LVS_EX_TWOCLICKACTIVATE,
                       Win32.LVS_EX_UNDERLINECOLD,
                       Win32.LVS_EX_UNDERLINEHOT,
                       Win32.LVS_EX_TRACKSELECT };

         int exStyles = Win32.SendMessage(ListViewHandle, Win32.LVM_GETEXTENDEDLISTVIEWSTYLE, IntPtr.Zero, IntPtr.Zero).ToInt32();
         int exStyles1 = (int)this.GetListViewExStyle();
         for (int i = 0; i < 8; i++)
         {
            if ((exStyles1 & checkIt[i]) == checkIt[i])
            {
               exStyles = exStyles | checkIt[i];
            }
         }

         if (exStyles1 != exStyles)
         {
            Win32.SendMessage(ListViewHandle, Win32.LVM_SETEXTENDEDLISTVIEWSTYLE, IntPtr.Zero, new IntPtr(exStyles));
         }
      }

      protected override void OnShow()
      {
         base.OnShow();

         RefreshCustomOkButtonPosition();

         if (DlgItemsCaptions.OK != null && DlgItemsCaptions.OK.Length > 0)
         {
            Win32.SendMessageWithString(Handle, Win32.CDM_SETCONTROLTEXT, Win32.IDOK, DlgItemsCaptions.OK);
         }

         _firstTimeShow = true;
         _firstTimeShow2 = true;
      }

      // methods
      private void OnOkButtonClick(object sender, System.EventArgs e)
      {
         CloseDialog(DialogResult.OK);
      }
      private bool IsWinXP()
      {
         Win32.OSVersionInfo osvi = new Win32.OSVersionInfo();
         osvi.OSVersionInfoSize = Marshal.SizeOf(osvi);
         if (!Win32.GetVersionEx(osvi))
            return false;
         if (osvi.platformId == Win32.VER_PLATFORM_WIN32_NT)
            if ((osvi.majorVersion > 5) || (osvi.majorVersion == 5 && osvi.minorVersion >= 1))
               return true;
         return false;
      }

      private bool IsWinVista()
      {
         Win32.OSVersionInfo osvi = new Win32.OSVersionInfo();
         osvi.OSVersionInfoSize = Marshal.SizeOf(osvi);

         if (!Win32.GetVersionEx(osvi))
         {
            return false;
         }

         if (osvi.platformId == Win32.VER_PLATFORM_WIN32_NT)
         {
            if (osvi.majorVersion > 5)
            {
               return true;
            }
         }

         return false;
      }

      private bool PlacesBarSupported()
      {
         Win32.OSVersionInfo osvi = new Win32.OSVersionInfo();
         osvi.OSVersionInfoSize = Marshal.SizeOf(osvi);
         if (!Win32.GetVersionEx(osvi))
            return false;
         if (osvi.platformId == Win32.VER_PLATFORM_WIN32_NT)
            if (osvi.majorVersion >= 5)
               return true;
         if (osvi.platformId == Win32.VER_PLATFORM_WIN32_WINDOWS)
            if ((osvi.majorVersion == 4 && osvi.minorVersion >= 90) || (osvi.majorVersion >= 5))
               return true;
         return false;
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      protected long GetListViewStyle()
      {
         if (_lvOptions.Enabled)
         {
            if (!_lvOptions.EnableRename)
               return ~Win32.LVS_EDITLABELS;
         }
         return 0;
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      protected long GetListViewExStyle()
      {
         int newL = 0;
         if (_lvOptions.Enabled)
         {
            if (_lvOptions.FullRowSelect)
               newL = newL | Win32.LVS_EX_FULLROWSELECT;
            if (_lvOptions.FlatScrollBars)
               newL = newL | Win32.LVS_EX_FLATSB;
            if (_lvOptions.GridLines)
               newL = newL | Win32.LVS_EX_GRIDLINES;
            if (_lvOptions.HotTrack)
               newL = newL | Win32.LVS_EX_TRACKSELECT;
            if (_lvOptions.UnderlineCold)
               newL = newL | Win32.LVS_EX_UNDERLINECOLD;
            if (_lvOptions.UnderlineHot)
               newL = newL | Win32.LVS_EX_UNDERLINEHOT;
            if (_lvOptions.HandPoint)
               newL = newL | Win32.LVS_EX_ONECLICKACTIVATE;
         }
         return newL;
      }

      [Description(DialogConsts.strCaFileDialogBase_RunDialog)]
      protected override bool RunDialog(IntPtr owner)
      {
         this._customOkBtn.Reset();

         // UNDER_CONSTRUCTION: This limitation will be removed in the next major product overwrite
         if (HookOwner != null)
         {
            throw new ApplicationException(DialogConsts.strXCannotCreate2ndFileDialog);
         }

         if (_showRecent)
         {
            CustDlgParams.TopForm = _recentTopForm;
            if (_recentRegKey.Length > 0)
            {
               _recentList.listList.Items.Clear();
               RegistryKey key = Registry.CurrentUser.OpenSubKey(_recentRegKey);
               if (key != null)
               {
                  string[] valNames = key.GetValueNames();
                  for (int i = 0; i < valNames.Length; i++)
                  {
                     System.IO.FileInfo fi = new System.IO.FileInfo(valNames[i]);
                     ListViewItem item = _recentList.listList.Items.Add(new ListViewItem(new string[] { fi.Name, fi.Directory.FullName }));
                     item.Tag = fi;
                  }

                  key.Close();
               }
            }
         }

         ofnEx = new Win32.OpenFileNameEx();

         _charBuffer = Interop.CharBuffer.CreateBuffer(0x2000);
         if (_fileNames != null && _fileNames.Length > 0)
         {
            _charBuffer.PutString(_fileNames[0]);
         }
         else if (_fileName.Length > 0)
         {
            _charBuffer.PutString(_fileName);
         }

         ofnEx.lStructSize = Marshal.SizeOf(typeof(Win32.OpenFileNameEx));
         if ((Environment.OSVersion.Platform != PlatformID.Win32NT) || (Environment.OSVersion.Version.Major < 5))
         {
            ofnEx.lStructSize = 0x4c;
         }

         ofnEx.hwndOwner = owner;
         ofnEx.hInstance = Process.GetCurrentProcess().Handle;

         ofnEx.lpstrFilter = Filter != null ? Filter.Replace("|", "\0") + "\0\0" : "\0\0";
         ofnEx.nFilterIndex = _filterIndex;
         ofnEx.lpstrCustomFilter = IntPtr.Zero;
         ofnEx.nMaxCustFilter = 0;

         //ofnEx.fileOffset = 0;
         //ofnEx.fileExtension = 0;
         ofnEx.lCustData = IntPtr.Zero;
         ofnEx.templateName = null;
         ofnEx.pvReserved = IntPtr.Zero;
         ofnEx.dwReserved = 0;

         ofnEx.lpstrFile = this._charBuffer.AllocCoTaskMem();
         ofnEx.nMaxFile = 2000;
         ofnEx.lpstrInitialDir = InitialDirectory;
         ofnEx.lpstrTitle = this.Title;
         if (this.DefaultExt != null && this.AddExtension)
         {
            ofnEx.lpstrDefExt = this.DefaultExt;
         }

         if (!ApplyVistaVisualStyle())
         {
            ofnEx.hook = new OFNHookProc(OFNHookProcBase);
         }

         if (PlacesBarSupported() && !PlacesBar.Visible)
            ofnEx.FlagsEx = Win32.OFN_EX_NOPLACESBAR;
         else
            ofnEx.FlagsEx = 0;
         SetFlags(ref ofnEx.Flags);

         closingDialog = false;
         bool retVal = true;
         _currentFilterIndex = _filterIndex;
         try
         {
            HookOwner = this;
            InstallHookProc(true);
            retVal = CallFileDialog(ref ofnEx);
         }
         finally
         {
            _charBuffer = null;
            HookOwner = null;
            SetHandle(IntPtr.Zero);
            if (ofnEx.lpstrFile != IntPtr.Zero)
            {
               Marshal.FreeCoTaskMem(ofnEx.lpstrFile);
            }
         }

         if (_autoFree)
            GC.Collect();

         return retVal;
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      protected abstract bool CallFileDialog(ref Win32.OpenFileNameEx ofn);

      private string[] GetMultiselectFiles(Interop.CharBuffer charBuffer)
      {
         string text1 = charBuffer.GetString();
         string text2 = charBuffer.GetString();
         if (text2.Length == 0)
         {
            return new string[] { text1 };
         }
         if (text1[text1.Length - 1] != '\\')
         {
            text1 = text1 + @"\";
         }
         ArrayList list1 = new ArrayList();
         while (true)
         {
            if ((text2[0] != '\\') && (((text2.Length <= 3) || (text2[1] != ':')) || (text2[2] != '\\')))
            {
               text2 = text1 + text2;
            }
            list1.Add(text2);
            text2 = charBuffer.GetString();
            if (text2.Length <= 0)
            {
               string[] textArray1 = new string[list1.Count];
               list1.CopyTo(textArray1, 0);
               return textArray1;
            }
         }
      }

      internal void ShowCustomSelectionCtrl(bool bShow)
      {
         if (Handle == IntPtr.Zero)
            return;
         if (_recentList == null)
            return;
         if (bShow)
         {
            _recentList.Parent = null;
            Win32.SetParent(_recentList.Handle, base.Handle);
            Win32.RECT rect = this.CustDlgParams._selRect;
            _recentList.Width = rect.right;
            _recentList.Height = rect.bottom;
            Win32.RECT r = GetFileListViewRect();
            _recentList.Left = r.left;
            _recentList.Top = r.top;
            _recentList.Show();
            _recentList.Invalidate();
            if (_recentList.listList.Items.Count > 0 && _recentList.listList.SelectedItems.Count == 0)
            {
               _recentList.listList.Items[0].Selected = true;
               _recentList.listList.Focus();
            }
         }
         else
         {
            _recentList.Hide();
            _recentList.Parent = null;

         }
      }

      internal Win32.RECT GetFileListViewRect()
      {
         Win32.RECT rect1 = new Win32.RECT(0, 0, 0, 0);
         IntPtr h1 = Win32.GetDlgItem(Handle, Win32.lst1);
         Win32.GetWindowRect(h1, ref rect1);
         Win32.POINT p = new Win32.POINT(rect1.left, rect1.top);
         Win32.ScreenToClient(Handle, ref p);
         Win32.POINT p1 = new Win32.POINT(rect1.right, rect1.bottom);
         Win32.ScreenToClient(Handle, ref p1);
         return new Win32.RECT(p.x, p.y, p1.x, p1.y);
      }

      internal void ShowStdSelectionCtrl(bool bShow)
      {
         if (Handle == IntPtr.Zero)
            return;
         IntPtr h1, h2, h11, h21;
         StringBuilder p = new StringBuilder(512);
         Win32.RECT selRect = new Win32.RECT(0, 0, 0, 0);
         h1 = Win32.GetDlgItem(Handle, Win32.lst1);
         h2 = Win32.GetDlgItem(Handle, Win32.lst2);
         h11 = Win32.GetWindow(h1, Win32.GW_CHILD);
         h21 = Win32.GetWindow(h2, Win32.GW_CHILD);
         if (bShow)
         {
            if (_recentList != null)
               _recentList.Visible = false;
            try
            {
               Win32.GetClassName(h1, p, 127);
               selRect = CustDlgParams._selRect;
               if (string.Compare(p.ToString(), "SHELLDLL_DefView", true) == 0)
               {
                  Win32.SetWindowPos(h2, Win32.HWND_BOTTOM, selRect.left, selRect.top, selRect.right, selRect.bottom, Win32.SWP_HIDEWINDOW | Win32.SWP_NOMOVE);
                  Win32.SetWindowPos(h1, Win32.HWND_TOP, selRect.left, selRect.top, selRect.right, selRect.bottom, Win32.SWP_SHOWWINDOW | Win32.SWP_NOMOVE);
                  Win32.SetWindowPos(h11, Win32.HWND_TOP, selRect.left, selRect.top, selRect.right, selRect.bottom, Win32.SWP_SHOWWINDOW | Win32.SWP_NOMOVE);
                  Win32.ShowWindow(h1, Win32.SW_SHOW);
                  Win32.ShowWindow(h11, Win32.SW_SHOW);
               }
               else
               {
                  Win32.SetWindowPos(h1, Win32.HWND_BOTTOM, selRect.left, selRect.top, selRect.right, selRect.bottom, Win32.SWP_HIDEWINDOW | Win32.SWP_NOMOVE);
                  Win32.SetWindowPos(h2, Win32.HWND_TOP, selRect.left, selRect.top, selRect.right, selRect.bottom, Win32.SWP_SHOWWINDOW | Win32.SWP_NOMOVE);
                  Win32.SetWindowPos(h21, Win32.HWND_TOP, selRect.left, selRect.top, selRect.right, selRect.bottom, Win32.SWP_SHOWWINDOW | Win32.SWP_NOMOVE);
                  Win32.ShowWindow(h2, Win32.SW_SHOW);
                  Win32.ShowWindow(h21, Win32.SW_SHOW);
               }
            }
            finally
            {
            }
            Win32.EnableWindow(h1, true);
            Win32.EnableWindow(h2, true);
         }
         else
         {
            Win32.SetWindowPos(h1, Win32.HWND_BOTTOM, 0, 0, 0, 0, Win32.SWP_HIDEWINDOW | Win32.SWP_NOMOVE | Win32.SWP_NOSIZE);
            Win32.SetWindowPos(h2, Win32.HWND_BOTTOM, 0, 0, 0, 0, Win32.SWP_HIDEWINDOW | Win32.SWP_NOMOVE | Win32.SWP_NOSIZE);
            Win32.EnableWindow(h1, false);
            Win32.EnableWindow(h2, false);
         }
      }

      private void OnWmInitDialog(IntPtr hdlg)
      {
         SetHandle(hdlg);
         _intHandle = hdlg;

         if (ListViewOptions.Enabled && !ListViewOptions.EnableDelete)
         {
            InstallKeyboardHookProc(true);
         }

         SetupNewDialogProc();
         CustDlgParams.OnInitDialog();
         Win32.POINT tp = new Win32.POINT(0, 0);
         Win32.RECT r1 = new Win32.RECT(0, 0, 0, 0);
         Win32.RECT r2 = new Win32.RECT(0, 0, 0, 0);
         IntPtr hlv = Win32.GetDlgItem(Handle, Win32.lst1);
         Win32.GetWindowRect(hlv, ref r1);
         Win32.GetClientRect(hlv, ref r2);
         tp.x = r1.left;
         tp.y = r1.top;
         Win32.ScreenToClient(hdlg, ref tp);
         Win32.RECT newRect = new Win32.RECT(0, 0, 0, 0);
         newRect.right = r2.right + Win32.GetSystemMetrics(Win32.SM_CXDLGFRAME);
         newRect.bottom = r2.bottom + Win32.GetSystemMetrics(Win32.SM_CYDLGFRAME);
         newRect.top = tp.y;
         newRect.left = tp.x;
         CustDlgParams.SelRect = newRect;
      }

      internal int OFNHookProcBase(IntPtr hdlg, int msg, IntPtr wparam, IntPtr lparam)
      {
         if (msg == Win32.WM_INITDIALOG)
         {
            OnWmInitDialog(Win32.GetParent(hdlg));
         }
         else if (msg == Win32.WM_NOTIFY)
         {
            Win32.OFNOTIFY ofn1 = new Win32.OFNOTIFY();
            Marshal.PtrToStructure(lparam, ofn1);
            if (ofn1 != null)
            {
               if (ofn1.hdr.code == Win32.CDN_INITDONE)
               {
                  OnShow();
               }
               else if (ofn1.hdr.code == Win32.CDN_FOLDERCHANGE)
               {
                  if (_pbOptions.Captions != null)
                  {
                     for (int i = 0; i < _pbOptions.Captions.Length; i++)
                     {
                        if (_pbOptions.Captions[i] != "" && _pbOptions.Captions[i] != null)
                           SetPlacesBarItemCaption(Win32.GetDlgItem(Handle, 0x4A0), i, _pbOptions.Captions[i]);
                     }
                  }
                  OnFolderChanged();
               }
               else if (ofn1.hdr.code == Win32.CDN_SELCHANGE)
               {
                  OnSelectionChanged();
               }
               else if (ofn1.hdr.code == Win32.CDN_TYPECHANGE)
               {
                  Win32.OpenFileNameEx x = (Win32.OpenFileNameEx)(Marshal.PtrToStructure(ofn1.ofn,
                                      typeof(Win32.OpenFileNameEx)));
                  int index = x.nFilterIndex;
                  if (index != _currentFilterIndex)
                  {
                     _currentFilterIndex = index;
                     OnTypeChanged();
                  }
               }
               else if (ofn1.hdr.code == Win32.CDN_FILEOK)
               {
                  bool ok = DoFileOk(ofn1.ofn);

                  if (ok)
                  {
                     bool cancelClosing = OnFileOk();
                     if (cancelClosing)
                     {
                        ok = false;
                     }
                  }
                  if (!ok)
                  {
                     Win32.SetWindowLong(hdlg, Win32.DWL_MSGRESULT, -1);
                     return -1;
                  }
                  else
                  {
                     _lastVisitedFolder = GetSelectedFolderAs(DisplayNameRequestFlags.ForParsing);
                     _lastVisitedFiles = SelectedFiles;
                     if (_showRecent && _lastVisitedFiles.Length > 0)
                     {
                        if (_recentRegKey.Length > 0)
                        {
                           RegistryKey key = Registry.CurrentUser.CreateSubKey(_recentRegKey);
                           if (key != null)
                           {
                              for (int i = 0; i < _lastVisitedFiles.Length; i++)
                              {
                                 System.IO.FileInfo fi = new System.IO.FileInfo(_lastVisitedFiles[i]);
                                 key.SetValue(fi.FullName, fi.Name);
                              }
                              string[] values = key.GetValueNames();
                              for (int i = 0; i < values.Length - _recentListCount; i++)
                                 key.DeleteValue(values[i], false);
                              key.Close();
                           }
                        }
                        else
                        {
                           for (int i = 0; i < _lastVisitedFiles.Length; i++)
                           {
                              System.IO.FileInfo fi = new System.IO.FileInfo(_lastVisitedFiles[i]);
                              ListViewItem item = new ListViewItem(new string[] { fi.Name, fi.Directory.FullName });
                              item.Tag = fi;
                              _recentList.listList.Items.Add(item);
                           }
                           int idx = 0;
                           while (idx < _recentList.listList.Items.Count)
                           {
                              ListViewItem item1 = _recentList.listList.Items[idx];
                              int idx1 = idx + 1;
                              while (idx1 < _recentList.listList.Items.Count)
                              {
                                 if (item1.Text == _recentList.listList.Items[idx1].Text && item1.SubItems[0].Text == _recentList.listList.Items[idx1].SubItems[0].Text)
                                 {
                                    _recentList.listList.Items.RemoveAt(idx1);
                                    break;
                                 }
                                 idx1++;
                              }
                              idx++;
                           }
                           while (_recentList.listList.Items.Count > _recentListCount)
                              _recentList.listList.Items.RemoveAt(0);
                        }
                     }
                  }
               }
               else if (ofn1.hdr.code == Win32.CDN_HELP)
               {
                  EventArgs e = new EventArgs();
                  OnHelpRequest(e);
               }
            }
         }
         return 0;
      }

      private void SetPlacesBarItemCaption(IntPtr toolBar, int index, string text)
      {
         Win32.TBBUTTONINFO info = new Win32.TBBUTTONINFO();
         info.cbSize = (uint)Marshal.SizeOf(info);
         info.dwMask = Win32.TBIF_BYINDEX | Win32.TBIF_TEXT;
         info.pszText = text;

         Win32.SendToolBarMessage(toolBar, Win32.TB_SETBUTTONINFO, index, ref info);
      }

      private void RefreshCustomOkButtonPosition()
      {
#if DOTNET2
         if (_customOkButtonMenu != null || _customOkButtonMenuStrip != null)
#else
            if (_customOkButtonMenu != null)
#endif
         {
            _customOkBtn.Width = 300;
            IntPtr buttonHandle = IntPtr.Zero;
            buttonHandle = Win32.GetDlgItem(Handle, Win32.IDOK);

            Win32.RECT r = new Win32.RECT(0, 0, 0, 0);
            Win32.GetWindowRect(buttonHandle, ref r);
            Win32.POINT p = new Win32.POINT(r.left, r.top);
            Win32.ScreenToClient(Handle, ref p);

            IntPtr newParent = Win32.GetParent(_customOkBtn.Handle);

            if (newParent != Handle)
            {
               Win32.SetParent(_customOkBtn.Handle, Handle);

               int oldStyle = Win32.GetWindowLong(_customOkBtn.Handle, Win32.GWL_STYLE);
               Win32.SetWindowLong(_customOkBtn.Handle, Win32.GWL_STYLE, oldStyle | Win32.WS_CHILD);

               Win32.ShowWindow(buttonHandle, 0);

               Win32.SetWindowPos(_customOkBtn.Handle, Win32.GetDlgItem(Handle, Win32.cmb1).ToInt32(), p.x, p.y, r.right - r.left, r.bottom - r.top, Win32.SWP_SHOWWINDOW);

               StringBuilder b = new StringBuilder(255);
               Win32.GetWindowText(Win32.GetDlgItem(Handle, Win32.IDOK), b, 254);
               _customOkBtn.Text = b.ToString().Replace("&", "");
               if (DlgItemsCaptions.OK != null && DlgItemsCaptions.OK.Length > 0)
               {
                  _customOkBtn.Text = DlgItemsCaptions.OK;
               }
            }

            _customOkBtn.Left = p.x;
            _customOkBtn.Top = p.y;
            _customOkBtn.Width = r.right - r.left;

         }
      }

      private int NewListViewProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam)
      {
         if (msg == Win32.WM_STYLECHANGED && !IsWinXP())
         {
            PostApplyCustomListViewStyles();
         }
         if (msg == Win32.WM_DESTROY)
         {
            Win32.SetWindowLong(_listViewHandle, Win32.GWL_WNDPROC, _originalListViewProc.ToInt32());
            _listViewHandle = IntPtr.Zero;
            return Win32.CallWindowProc(_originalListViewProc, hwnd, msg, wparam, lparam);
         }

         return Win32.CallWindowProc(_originalListViewProc, hwnd, msg, wparam, lparam);
      }

      private int NewDefViewProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam)
      {
         if ((msg == Win32.WM_COMMAND) && IsWinXP())
         {
            if (wparam.ToInt32() >= (int)SHVIEW.SHVIEW_LARGEICON && wparam.ToInt32() <= (int)SHVIEW.SHVIEW_TILE)
            {
               PostApplyCustomListViewStyles();
            }
         }
         else if (msg == Win32.WM_DESTROY)
         {
            Win32.SetWindowLong(_defViewHandle, Win32.GWL_WNDPROC, _originalDefViewProc.ToInt32());
            _defViewHandle = IntPtr.Zero;
            return Win32.CallWindowProc(_originalDefViewProc, hwnd, msg, wparam, lparam);
         }
         else if (msg == Win32.WM_CONTEXTMENU)
         {
            int n = Win32.SendMessage(ListViewHandle, Win32.LVM_GETSELECTEDCOUNT, IntPtr.Zero, IntPtr.Zero).ToInt32();
            if (_lvOptions.Enabled && !_lvOptions.PopupOnEmpty && n == 0)
            {
               return 1;
            }
            if (_lvOptions.Enabled && !_lvOptions.PopupOnSelection && n > 0)
            {
               return 1;
            }
         }
         else if (msg == Win32.WM_INITMENUPOPUP)
         {
            if (_lvOptions.Enabled && !_lvOptions.EnableRename)
            {
               int n = Win32.SendMessage(ListViewHandle, Win32.LVM_GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero).ToInt32();
               if (Win32.LOWORD(lparam) == 0 && n > 0)
               {
                  IntPtr menu = wparam;
                  int mc = Win32.GetMenuItemCount(menu);
                  int id = Win32.GetMenuItemID(menu, mc - 1 - 2);
                  if (id == 30738 || id == 30995)
                  {
                     Win32.EnableMenuItem(menu, id, Win32.MF_BYCOMMAND | Win32.MF_GRAYED);
                  }
               }
            }
            if (_lvOptions.Enabled && !_lvOptions.EnableDelete)
            {
               int n = Win32.SendMessage(ListViewHandle, Win32.LVM_GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero).ToInt32();
               if (Win32.LOWORD(lparam) == 0 && n > 0)
               {
                  IntPtr menu = wparam;
                  int mc = Win32.GetMenuItemCount(menu);
                  int id = Win32.GetMenuItemID(menu, mc - 1 - 3);
                  if (id == 30737 || id == 30994)
                  {
                     Win32.EnableMenuItem(menu, id, Win32.MF_BYCOMMAND | Win32.MF_GRAYED);
                  }
               }
            }
         }

         return Win32.CallWindowProc(_originalDefViewProc, hwnd, msg, wparam, lparam);
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      protected override int OnDialogMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam)
      {
         /*if (msg == 0x052C)
         {
            if (ApplyVistaVisualStyle())
            {
               OnSelectionChanged();
            }
         }*/
         if (msg == Win32.WM_USER + 1)
         {
            RefreshCustomOkButtonPosition();
         }
         else if (msg == Win32.WM_USER + 1976)
         {
            ApplyCustomListViewStyles();
         }
         else if (msg == Win32.WM_PAINT)
         {
            IntPtr h = GetEdit();
            Win32.RECT r = new Win32.RECT(0, 0, 0, 0);
            Win32.GetWindowRect(h, ref r);
            Win32.SetWindowPos(h, Win32.HWND_TOP, r.left, r.top, r.right - r.left, r.bottom - r.top, Win32.SWP_NOZORDER | Win32.SWP_NOMOVE);
            //Win32.EnumChildWindows(Handle, new WndEnumProc(CaCommonDialog.RepaintCtrlProc), IntPtr.Zero);
         }
         else if (msg == Win32.WM_NCPAINT)
         {
            if (_firstTimeShow2 && _cx > 0)
            {
               Win32.SetWindowPos(Handle, Win32.HWND_TOP, 0, 0, _cx + CustDlgParams.IncX, _cy + CustDlgParams.IncY, Win32.SWP_NOMOVE);
               _firstTimeShow2 = false;
            }

            Win32.EnumChildWindows(Handle, new WndEnumProc(CaCommonDialog.RepaintCtrlProc), IntPtr.Zero);
         }
         else if (msg == Win32.WM_GETMINMAXINFO)
         {
            if (closingDialog)
               return 0;
         }
         else if (msg == Win32.WM_DESTROY)
         {
            closingDialog = true;
            Win32.RECT wndRect = new Win32.RECT(0, 0, 0, 0);
            Win32.GetWindowRect(Handle, ref wndRect);
            Win32.SetWindowPos(Handle, Win32.HWND_TOP, 0, 0, wndRect.right - wndRect.left - CustDlgParams.IncX, wndRect.bottom - wndRect.top - CustDlgParams.IncY, Win32.SWP_NOMOVE);
            _cx = wndRect.right - wndRect.left - CustDlgParams.IncX;
            _cy = wndRect.bottom - wndRect.top - CustDlgParams.IncY;
         }
         else if (msg == Win32.WM_CONTEXTMENU)
         {
            Win32.POINT pnt = new Win32.POINT();
            pnt.x = Win32.LOWORD(lparam);
            pnt.y = Win32.HIWORD(lparam);
            if (CustDlgParams.TrackContextMenus(pnt.x, pnt.y))
               return 0;
         }
         else if (msg == Win32.WM_SIZE)
         {
            if (_showRecent && _recentList != null)
            {
               Win32.RECT rect = CustDlgParams._dlgRect;
               _recentList.Width = _recentList.Width + (Win32.LOWORD(lparam) - rect.right);
               _recentList.Height = _recentList.Height + (Win32.HIWORD(lparam) - rect.bottom);
            }

            Win32.EnumChildWindows(Handle, new WndEnumProc(CaCommonDialog.RepaintCtrlProc), IntPtr.Zero);
            CustDlgParams.OnResizeWindow(wparam, lparam);

            Win32.PostMessage(Handle, Win32.WM_USER + 1, IntPtr.Zero, IntPtr.Zero);
         }
         else if (msg == Win32.WM_COMMAND)
         {
            if (Win32.LOWORD(wparam) == Win32.cmb13 && Win32.HIWORD(wparam) == Win32.CBN_EDITCHANGE)
            {
               int retval = base.OnDialogMessage(Handle, msg, wparam, lparam);
               OnFilenameTextChanged();
               return retval;
            }
            if (Win32.LOWORD(wparam) == Win32.edt1 && Win32.HIWORD(wparam) == Win32.EN_CHANGE)
            {
               int retval = base.OnDialogMessage(Handle, msg, wparam, lparam);
               OnFilenameTextChanged();
               return retval;
            }
            if (Win32.LOWORD(wparam) == Win32.cmb2 && Win32.HIWORD(wparam) == Win32.CBN_DROPDOWN)
            {
               string folder = GetSelectedFolderAs(DisplayNameRequestFlags.ForParsing);
               int ok = base.OnDialogMessage(Handle, msg, wparam, lparam);
               if (string.Compare(folder, GetSelectedFolderAs(DisplayNameRequestFlags.ForParsing), true) != 0)
                  SelectedFolder = folder;
               return ok;
            }
            if (Win32.LOWORD(wparam) >= 41060 && Win32.LOWORD(wparam) <= 41064 && Win32.HIWORD(wparam) == Win32.BN_CLICKED)
            {
               if (OnPlacesBarClicking(Win32.LOWORD(wparam) - 41060))
                  return 0;
               int retval = base.OnDialogMessage(Handle, msg, wparam, lparam);
               OnPlacesBarClicked(Win32.LOWORD(wparam) - 41060);
               return retval;
            }
            else
               if (Win32.LOWORD(wparam) == DialogConsts.idCreateFolder && Win32.HIWORD(wparam) == Win32.BN_CLICKED)
               {
                  if (OnNewFolderCreating())
                     return 0;
                  int retval = base.OnDialogMessage(Handle, msg, wparam, lparam);
                  OnNewFolderCreated();
                  return retval;
               }
            if (Win32.LOWORD(wparam) == DialogConsts.idShowDesktop && Win32.HIWORD(wparam) == Win32.BN_CLICKED)
            {
               if (OnDesktopNavigating())
                  return 0;
               int retval = base.OnDialogMessage(Handle, msg, wparam, lparam);
               OnDesktopNavigated();
               return retval;
            }
            if (Win32.LOWORD(wparam) == DialogConsts.idUpOneLevel && Win32.HIWORD(wparam) == Win32.BN_CLICKED)
            {
               if (OnUpLevelNavigating())
                  return 0;
               int retval = base.OnDialogMessage(Handle, msg, wparam, lparam);
               OnUpLevelNavigated();
               return retval;
            }
            if (Win32.LOWORD(wparam) == DialogConsts.idLastVisitedFolder && Win32.HIWORD(wparam) == Win32.BN_CLICKED)
            {
               if (OnLastVisitedFolderNavigating())
                  return 0;
               int retval = base.OnDialogMessage(Handle, msg, wparam, lparam);
               OnLastVisitedFolderNavigated();
               return retval;
            }
         }

         return base.OnDialogMessage(Handle, msg, wparam, lparam);
      }

      protected bool ApplyVistaVisualStyle()
      {
         //return (_enableVistaVisualStyle && IsWinVista());
         return false;
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      protected virtual void SetFlags(ref int flags)
      {
         if (ApplyVistaVisualStyle())
         {
            flags = Win32.OFN_EXPLORER;
         }
         else
         {
            flags = Win32.OFN_ENABLEHOOK | Win32.OFN_EXPLORER;
         }
         if (CheckFileExists) flags |= Win32.OFN_FILEMUSTEXISTS;
         if (CheckPathExists) flags |= Win32.OFN_PATHMUSTEXISTS;
         if (!DereferenceLinks) flags |= Win32.OFN_NODEREFERENCELINKS;
         if (RestoreDirectory) flags |= Win32.OFN_NOCHANGEDIR;
         if (ShowHelp) flags |= Win32.OFN_SHOWHELP;
         if (!ValidateNames) flags |= Win32.OFN_NOVALIDATE;
         if (Multiselect) flags |= Win32.OFN_ALLOWMULTISELECT;
         if (DontAddToRecent) flags |= Win32.OFN_DONTADDTORECENT;
         if (EnableSizing) flags |= Win32.OFN_ENABLESIZING;
      }

      internal IntPtr GetEdit()
      {
         IntPtr p = IntPtr.Zero;
         if (Handle != IntPtr.Zero)
         {
            p = Win32.GetDlgItem(Handle, Win32.edt1);
            if (p == IntPtr.Zero)
               p = Win32.GetDlgItem(Handle, Win32.cmb13);
         }
         return p;
      }

      [Description(DialogConsts.strCaFileDialogBase_Refresh)]
      public void Refresh()
      {
         IntPtr defView = Win32.FindWindowEx(Handle, IntPtr.Zero, "SHELLDLL_DefView", IntPtr.Zero);
         IntPtr listView = Win32.FindWindowEx(defView, IntPtr.Zero, "SysListView32", IntPtr.Zero);
         Win32.PostMessage(listView, Win32.WM_KEYDOWN, new IntPtr(Win32.VK_F5), IntPtr.Zero);
      }

      [Description(DialogConsts.strCaFileDialogBase_ShowControl)]
      public void ShowControl(FileDialogControl ctrl, bool show)
      {
         if (ctrl == FileDialogControl.ListFiles)
            ShowStdSelectionCtrl(show);
         else
         {
            FieldInfo fi = ctrl.GetType().GetField(Enum.GetName(typeof(FileDialogControl), ctrl));
            ItemIDAttribute[] attrs = (ItemIDAttribute[])fi.GetCustomAttributes(typeof(ItemIDAttribute), true);
            if (attrs.Length > 0)
               Win32.ShowWindow(Win32.GetDlgItem(Handle, attrs[0].ItemID), show ? Win32.SW_SHOW : Win32.SW_HIDE);
         }
      }

      [Description(DialogConsts.strCaFileDialogBase_EnableToolButton)]
      public void EnableToolButton(FileDialogToolButton button, bool enabled)
      {
         IntPtr handle = Win32.FindWindowEx(Handle, IntPtr.Zero, "ToolbarWindow32", IntPtr.Zero);
         if (Win32.GetDlgItem(Handle, 0x000004A0) == handle) // PlacesBar found instead of Toolbar
            handle = Win32.FindWindowEx(Handle, handle, "ToolbarWindow32", IntPtr.Zero);
         IntPtr enabled1 = new IntPtr(Win32.MAKELONG((enabled ? (ushort)1 : (ushort)0), 0));
         if (button == FileDialogToolButton.LastVisitedFolder)
            Win32.SendMessage(handle, Win32.TB_ENABLEBUTTON, new IntPtr(40971), enabled1);
         if (button == FileDialogToolButton.GotoUpLevel)
            Win32.SendMessage(handle, Win32.TB_ENABLEBUTTON, new IntPtr(40961), enabled1);
         if (button == FileDialogToolButton.CreateFolder)
            Win32.SendMessage(handle, Win32.TB_ENABLEBUTTON, new IntPtr(40962), enabled1);
      }

      // events
      [Description(DialogConsts.strCaFileDialogBase_TypeChanged)]
      public event EventHandler TypeChanged;

      [Description(DialogConsts.strCaFileDialogBase_FolderChanged)]
      public event EventHandler FolderChanged;

      [Description(DialogConsts.strCaFileDialogBase_SelectionChanged)]
      public event EventHandler SelectionChanged;

      [Description(DialogConsts.strCaFileDialogBase_RecentSelectionChanged)]
      public event EventHandler RecentSelectionChanged;

      [Description(DialogConsts.strCaFileDialogBase_FilenameTextChanged)]
      public event EventHandler FilenameTextChanged;

      [Description(DialogConsts.strCaFileDialogBase_FileOk)]
      public event CancelEventHandler FileOk;

      [Description(DialogConsts.strCaFileDialogBase_UpLevelNavigated)]
      public event EventHandler UpLevelNavigated;

      [Description(DialogConsts.strCaFileDialogBase_UpLevelNavigating)]
      public event CancelEventHandler UpLevelNavigating;

      [Description(DialogConsts.strCaFileDialogBase_LastVisitedFolderNavigated)]
      public event EventHandler LastVisitedFolderNavigated;

      [Description(DialogConsts.strCaFileDialogBase_LastVisitedFolderNavigating)]
      public event CancelEventHandler LastVisitedFolderNavigating;

      [Description(DialogConsts.strCaFileDialogBase_NewFolderCreated)]
      public event EventHandler NewFolderCreated;

      [Description(DialogConsts.strCaFileDialogBase_NewFolderCreating)]
      public event CancelEventHandler NewFolderCreating;

      [Description(DialogConsts.strCaFileDialogBase_DesktopNavigated)]
      public event EventHandler DesktopNavigated;

      [Description(DialogConsts.strCaFileDialogBase_DesktopNavigating)]
      public event CancelEventHandler DesktopNavigating;

      [Description(DialogConsts.strCaFileDialogBase_PlacesBarClicking)]
      public event CommandCancelEventHandler PlacesBarClicking;

      [Description(DialogConsts.strCaFileDialogBase_PlacesBarClicked)]
      public event CommandEventHandler PlacesBarClicked;

      //[Browsable(false)]
      //[EditorBrowsable(EditorBrowsableState.Never)]
      //public event OpenFileNameEventHandler OpenFileNameInitialized;

      //event triggers
      internal void OnTypeChanged()
      {
         if (TypeChanged != null)
            TypeChanged(this, new EventArgs());
      }
      internal void OnFolderChanged()
      {
         //RecreateHwnds();

         if (_firstTimeShow)
         {
            EffectiveListViewStyle = _initialFileListViewStyle;
            _firstTimeShow = false;
         }

         if (_showRecent && SelectedTabIndex == 1)
         {
            ShowStdSelectionCtrl(false);
            ShowCustomSelectionCtrl(true);
         }

         if (ListViewOptions.Enabled)
         {
            ApplyCustomListViewStyles();
         }

         if (FolderChanged != null)
            FolderChanged(this, new EventArgs());
      }

      internal void OnSelectionChanged()
      {
         if (SelectionChanged != null)
            SelectionChanged(this, new EventArgs());
      }

      internal void OnRecentSelectionChanged()
      {
         if (RecentSelectionChanged != null)
            RecentSelectionChanged(this, new EventArgs());
      }

      internal void OnNewFolderCreated()
      {
         if (NewFolderCreated != null)
            NewFolderCreated(this, new EventArgs());
      }

      internal void OnLastVisitedFolderNavigated()
      {
         if (LastVisitedFolderNavigated != null)
            LastVisitedFolderNavigated(this, new EventArgs());
      }

      internal void OnUpLevelNavigated()
      {
         if (UpLevelNavigated != null)
            UpLevelNavigated(this, new EventArgs());
      }

      internal void OnDesktopNavigated()
      {
         if (DesktopNavigated != null)
            DesktopNavigated(this, new EventArgs());
      }

      internal void OnFilenameTextChanged()
      {
         if (FilenameTextChanged != null)
            FilenameTextChanged(this, new EventArgs());
      }

      internal bool OnDesktopNavigating()
      {
         CancelEventArgs e = new CancelEventArgs();
         if (DesktopNavigating != null)
            DesktopNavigating(this, e);
         return e.Cancel;
      }

      internal bool OnNewFolderCreating()
      {
         CancelEventArgs e = new CancelEventArgs();
         if (NewFolderCreating != null)
            NewFolderCreating(this, e);
         return e.Cancel;
      }

      internal bool OnLastVisitedFolderNavigating()
      {
         CancelEventArgs e = new CancelEventArgs();
         if (LastVisitedFolderNavigating != null)
            LastVisitedFolderNavigating(this, e);
         return e.Cancel;
      }

      internal bool OnUpLevelNavigating()
      {
         CancelEventArgs e = new CancelEventArgs();
         if (UpLevelNavigating != null)
            UpLevelNavigating(this, e);
         return e.Cancel;
      }

      protected virtual bool OnFileOk()
      {
         CancelEventArgs e = new CancelEventArgs();
         try
         {
            if (FileOk != null)
               FileOk(this, e);
         }
         catch (Exception e1)
         {
            e.Cancel = true;
            MessageBox.Show(e1.Message, "Fout", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }

         return e.Cancel;
      }

      internal bool DoFileOk(IntPtr ofn)
      {
         bool retFlag = true;
         Win32.OpenFileNameEx ofnEx = (Win32.OpenFileNameEx)Marshal.PtrToStructure(ofn, typeof(Win32.OpenFileNameEx));
         try
         {
            _filterIndex = ofnEx.nFilterIndex;
            _charBuffer.PutCoTaskMem(ofnEx.lpstrFile);
            if (!this.Multiselect)
            {
               _fileNames = new string[] { _charBuffer.GetString() };
            }
            else
            {
               _fileNames = GetMultiselectFiles(_charBuffer);
            }

            ProcessFileNames();
         }
         finally
         {
         }

         return retFlag;
      }

      private void ProcessFileNames()
      {
         _fileName = "";
         if (_fileNames.Length > 0)
         {
            _fileName = _fileNames[0];
         }
      }

      internal bool OnPlacesBarClicking(int id)
      {
         CommandCancelEventArgs e = new CommandCancelEventArgs(id);
         if (PlacesBarClicking != null)
            PlacesBarClicking(this, e);
         return e.Cancel;
      }

      internal void OnPlacesBarClicked(int id)
      {
         CommandEventArgs e = new CommandEventArgs(id);
         if (PlacesBarClicked != null)
            PlacesBarClicked(this, e);
      }

      // properties
      [DefaultValue(true)]
      [Description(DialogConsts.strCaFileDialogBase_CheckFileExists)]
      public virtual bool CheckFileExists
      {
         get { return _checkFileExists; }
         set { _checkFileExists = value; }
      }

      [DefaultValue(true)]
      [Description(DialogConsts.strCaFileDialogBase_CheckPathExists)]
      public virtual bool CheckPathExists
      {
         get { return _checkPathExists; }
         set { _checkPathExists = value; }
      }

      [DefaultValue(true)]
      [Description(DialogConsts.strCaFileDialogBase_AddExtension)]
      public virtual bool AddExtension
      {
         get { return _addExt; }
         set { _addExt = value; }
      }

      [Description(DialogConsts.strCaFileDialogBase_DefaultExt)]
      public virtual string DefaultExt
      {
         get { return _defExt; }
         set { _defExt = value; }
      }

      [DefaultValue(true)]
      [Description(DialogConsts.strCaFileDialogBase_DereferenceLinks)]
      public virtual bool DereferenceLinks
      {
         get { return _derefLinks; }
         set { _derefLinks = value; }
      }

      [Description(DialogConsts.strCaFileDialogBase_FileName)]
      public virtual string FileName
      {
         get { return _fileName; }
         set
         {
            _fileName = value;
            if (value == null)
            {
               this._fileNames = null;
            }
            else
            {
               this._fileNames = new string[] { value };
            }
         }
      }

      [Browsable(false)]
      [Description(DialogConsts.strCaFileDialogBase_FileNames)]
      public virtual string[] FileNames
      {
         get { return _fileNames; }
         set
         {
            _fileNames = value;
         }
      }

      [DefaultValue("")]
      [Description(DialogConsts.strCaFileDialogBase_Filter)]
      [Editor(typeof(FileFilterEditor), typeof(UITypeEditor))]
      public virtual string Filter
      {
         get { return _filter; }
         set { _filter = value; }
      }

      [Description(DialogConsts.strCaFileDialogBase_FilterIndex)]
      [DefaultValue(1)]
      public virtual int FilterIndex
      {
         get { return _filterIndex; }
         set { _filterIndex = value; }
      }
      [Description(DialogConsts.strCaFileDialogBase_FilterIndex)]
      [Browsable(false)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public virtual int FiterIndex
      {
         get { return _filterIndex; }
         set { _filterIndex = value; }
      }
      [Description(DialogConsts.strCaFileDialogBase_InitialDirectory)]
      public virtual string InitialDirectory
      {
         get { return _initialDir; }
         set { _initialDir = value; }
      }

      [DefaultValue(false)]
      [Description(DialogConsts.strCaFileDialogBase_MultiSelect)]
      public virtual bool Multiselect
      {
         get { return _multiSelect; }
         set
         {
            _multiSelect = value;
            _recentList.listList.MultiSelect = _multiSelect;
         }
      }

      [DefaultValue(true)]
      [Description(DialogConsts.strCaFileDialogBase_RestoreDirectory)]
      public virtual bool RestoreDirectory
      {
         get { return _restoreDir; }
         set { _restoreDir = value; }
      }

      [DefaultValue(true)]
      [Description(DialogConsts.strCaFileDialogBase_ValidateNames)]
      public virtual bool ValidateNames
      {
         get { return _validateNames; }
         set { _validateNames = value; }
      }

      [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
      [Description(DialogConsts.strPropDesc_DlgItemsCaptions)]
      public CaFileDialogItemCaptions DlgItemsCaptions { get { return _dlgItemCaptions; } }

      [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
      [Description(DialogConsts.strCaFileDialogBase_ListViewOptions)]
      public CaListViewOptions ListViewOptions { get { return _lvOptions; } }

      [Browsable(false)]
      [Description(DialogConsts.strCaFileDialogBase_FilenameText)]
      public string FilenameText
      {
         get
         {
            if (Handle != IntPtr.Zero)
            {
               int len = Win32.GetWindowTextLength(GetEdit());
               if (len <= 0)
                  len = 2048;
               StringBuilder text = new StringBuilder(len + 2);
               Win32.GetWindowText(GetEdit(), text, len + 1);
               return text.ToString();
            }
            else
               return "";
         }
         set
         {
            if (Handle != IntPtr.Zero)
            {
               Win32.SetWindowText(GetEdit(), value);
            }
         }
      }

      [Browsable(false)]
      [Description(DialogConsts.strCaFileDialogBase_SelectedFiles)]
      public string[] SelectedFiles
      {
         get
         {
            if (Handle == IntPtr.Zero)
               return _lastVisitedFiles;

            string filterExt = DefaultExt;
            if (SelectedFilterIndex > 0)
            {
               DataTable filters = ExportFilterDefinitions();
               if (filters.Rows.Count >= SelectedFilterIndex)
               {
                  string filter = (string)filters.Rows[SelectedFilterIndex - 1][1];
                  int lastDotIndex = filter.LastIndexOf(".");
                  if (lastDotIndex >= 0 && lastDotIndex < filter.Length - 1)
                  {
                     string x = filter.Substring(lastDotIndex + 1);
                     if (x.Length > 0 && x != "*")
                        filterExt = x;
                  }
               }
            }

            StringBuilder b0 = new StringBuilder();
            string selFolder = SelectedFolder;
            if (selFolder.Length > 0 && selFolder[selFolder.Length - 1] != Path.DirectorySeparatorChar)
               selFolder = selFolder + Path.DirectorySeparatorChar;
            if (selFolder.Length == 0)
            {
               selFolder = GetSelectedFolderAs(DisplayNameRequestFlags.ForParsing);
               if (selFolder.Length > 0 && selFolder[selFolder.Length - 1] != '/')
                  selFolder = selFolder + '/';
            }
            IntPtr len = Win32.SendFileDialogMessage(Handle, Win32.CDM_GETSPEC, 0, b0);
            b0.Length = len.ToInt32() + 1;
            Win32.SendFileDialogMessage(Handle, Win32.CDM_GETSPEC, len.ToInt32(), b0);
            string[] files = b0.ToString().Split(new char[] { '"' }); // without directory
            ArrayList list = new ArrayList();
            for (int i = 0; i < files.Length; i++)
            {
               files[i] = files[i].Replace('"', ' ');
               files[i] = files[i].Trim();
               if (files[i].Length > 0)
               {
                  try
                  {
                     System.IO.FileInfo fi = new System.IO.FileInfo(files[i]);

                     if (files[i].StartsWith(selFolder) && fi.FullName == files[i])
                        list.Add(FormattedFileName(files[i], filterExt));
                     else
                     {
                        if (fi.DirectoryName.Length > 0)
                        {
                           list.Add(FormattedFileName(files[i], filterExt));
                        }
                        else
                        {
                           list.Add(selFolder + FormattedFileName(files[i], filterExt));
                        }
                     }
                  }
                  catch
                  {
                     list.Add(files[i]);
                  }
               }
            }
            string[] result = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
               result[i] = list[i].ToString();
            return result;
         }
      }

      internal string FormattedFileName(string fname, string filterExt)
      {
         System.IO.FileInfo fi = new System.IO.FileInfo(fname);
         if (fi.Extension.Length == 0 && AddExtension)
         {
            // add extension
            string ext = filterExt;
            if (ext == null || ext.Length == 0 || ext == "*")
               ext = DefaultExt;
            string fname2 = fname;
            if (fname[fname.Length - 1] == '.')
               fname2 = fname.Substring(0, fname.Length - 1);
            if (ext != null && ext.Length > 0)
               fname2 = fname2 + "." + ext;
            if (fi.DirectoryName.Length > 0)
               return System.IO.Path.Combine(fi.DirectoryName, fname2);
            else
               return fname2;
         }
         else
            return fname;
      }
      internal DataTable ExportFilterDefinitions()
      {
         string value = Filter;
         DataTable dataTable1 = new DataTable();
         dataTable1.Columns.Add("Filter Name", typeof(string));
         DataColumn c = dataTable1.Columns.Add("Filter", typeof(string));
         c.MaxLength = 1024;
         int beginPos = 0;
         int endPos = value.IndexOf("|");
         System.Collections.Specialized.StringCollection strings = new System.Collections.Specialized.StringCollection();
         while (endPos > -1)
         {
            strings.Add(value.Substring(beginPos, endPos - beginPos));
            beginPos = endPos + 1;
            endPos = value.IndexOf("|", beginPos + 1);
         }
         if (beginPos > 0)
         {
            strings.Add(value.Substring(beginPos));
         }
         dataTable1.Rows.Clear();
         for (int i = 0; i < strings.Count / 2; i++)
         {
            dataTable1.Rows.Add(new object[] { strings[i * 2], strings[i * 2 + 1] });
         }
         return dataTable1;
      }

      [Browsable(false)]
      [Description(DialogConsts.strCaFileDialogBase_SelectedFile)]
      public string SelectedFile
      {
         get
         {
            if (Handle == IntPtr.Zero)
            {
               if (_lastVisitedFiles != null && _lastVisitedFiles.Length > 0)
                  return _lastVisitedFiles[0];
               else
                  return "";
            }

            string[] selFiles = SelectedFiles;
            if (selFiles.Length > 0)
               return selFiles[0];
            else
               return "";
         }
      }

      [Browsable(false)]
      public int SelectedFilterIndex
      {
         get
         {
            return _currentFilterIndex;
         }
      }

      [Browsable(false)]
      [Description(DialogConsts.strCaFileDialogBase_SelectedFolder)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public string SelectedFolder
      {
         get
         {
            if (Handle == IntPtr.Zero)
               return _lastVisitedFolder;

            StringBuilder b = new StringBuilder();
            IntPtr len = Win32.SendFileDialogMessage(Handle, Win32.CDM_GETFOLDEREPATH, 0, b);
            b.Length = len.ToInt32() + 1;
            Win32.SendFileDialogMessage(Handle, Win32.CDM_GETFOLDEREPATH, len.ToInt32(), b);
            return b.ToString();
         }
         set
         {
            if (Handle == IntPtr.Zero)
               return;

            bool canChange = false;
            if (value.Length > 6 && string.Compare(value.Substring(0, 6), "ftp://", true) == 0)
               canChange = true;
            else if (value.Length > 7 && string.Compare(value.Substring(0, 7), "http://", true) == 0)
               canChange = true;
            else if (value.Length > 8 && string.Compare(value.Substring(0, 8), "https://", true) == 0)
               canChange = true;
            else if (value != "")
            {
               try
               {
                  DirectoryInfo info = new DirectoryInfo(value);
                  if (info.Exists)
                     canChange = true;
               }
               catch
               {
                  if (value.Substring(0, 3) == @"::{")
                     canChange = true;
                  else
                     canChange = false;
               }
            }
            if (canChange)
            {
               IntPtr edit = GetEdit();
               IntPtr focus = Win32.SetFocus(edit);
               StringBuilder b = new StringBuilder(Win32.MAX_PATH + 1);
               Win32.SendFileDialogMessage(edit, Win32.WM_GETTEXT, Win32.MAX_PATH, b);
               Win32.SendMessageWithString(edit, Win32.WM_SETTEXT, 0, value);
               Win32.SendMessage(Win32.GetDlgItem(Handle, Win32.IDOK), Win32.BM_CLICK, IntPtr.Zero, IntPtr.Zero);
               Win32.SendMessageWithString(edit, Win32.WM_SETTEXT, 0, b.ToString());
               Win32.SetFocus(focus);
            }
         }
      }

      [Description(DialogConsts.strCaFileDialogBase_GetSelectedFolderAs)]
      public string GetSelectedFolderAs(int flags)
      {
         return ShellUtils.GetFolderFromPidl(Handle, flags);
      }

      [Description(DialogConsts.strCaFileDialogBase_SelectedSpecialFolder)]
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public SpecialFolder SelectedSpecialFolder
      {
         get
         {
            string[] guids = {"::{20D04FE0-3AEA-1069-A2D8-08002B30309D}",   // my computer
                              "::{208D2C60-3AEA-1069-A2D7-08002B30309D}"};  // my network places

            if (GetSelectedFolderAs(DisplayNameRequestFlags.ForParsing) == guids[0])
               return SpecialFolder.Drives;
            if (GetSelectedFolderAs(DisplayNameRequestFlags.ForParsing) == guids[1])
               return SpecialFolder.Network;
            System.Type t = typeof(SpecialFolder);
            FieldInfo[] fields = t.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
               if (fields[i].FieldType == typeof(SpecialFolder))
               {
                  StringBuilder sb = new StringBuilder(2048);
                  Win32.SHGetSpecialFolderPath(Handle, sb, (int)fields[i].GetValue(null), false);
                  if (sb.ToString().Trim().ToLower() == SelectedFolder.ToLower())
                     return (SpecialFolder)(fields[i].GetValue(null));
               }
            }
            return SpecialFolder.None;
         }
         set
         {
            StringBuilder sb = new StringBuilder(2048);
            Win32.SHGetSpecialFolderPath(Handle, sb, (int)value, false);
            if (sb.ToString().Trim().Length > 0)
               SelectedFolder = sb.ToString().Trim();
            else if (value == SpecialFolder.Drives)
            {
               SelectedFolder = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
            }
            else if (value == SpecialFolder.Network)
            {
               SelectedFolder = "::{208D2C60-3AEA-1069-A2D7-08002B30309D}";
            }
         }
      }

      [Description(DialogConsts.strCaFileDialogBase_CollectGarbage)]
      [DefaultValue(true)]
      public bool CollectGarbage
      {
         get { return _autoFree; }
         set { _autoFree = value; }
      }

      [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
      [Description(DialogConsts.strCaFileDialogBase_PlacesBar)]
      public CaPlacesBarOptions PlacesBar
      {
         get { return _pbOptions; }
      }

      [Description(DialogConsts.strCaFileDialogBase_DontAddToRecent)]
      [DefaultValue(false)]
      public bool DontAddToRecent
      {
         get { return _dontAddToRecent; }
         set { _dontAddToRecent = value; }
      }

      [Description(DialogConsts.strCaFileDialogBase_ShowRecentFilesList)]
      [DefaultValue(false)]
      public bool ShowRecentFilesList
      {
         get { return _showRecent; }
         set { _showRecent = value; }
      }

      [Description(DialogConsts.strCaFileDialogBase_RecentFilesListRegKey)]
      [DefaultValue("")]
      public string RecentFilesListRegKey
      {
         get { return _recentRegKey; }
         set { _recentRegKey = value; }
      }

      [Description(DialogConsts.strCaFileDialogBase_RecentFilesListCount)]
      [DefaultValue(10)]
      public int RecentFilesListCount
      {
         get { return _recentListCount; }
         set { _recentListCount = value; }
      }
      [Browsable(false)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      public UserControl RecentListControl
      {
         get { return _recentList; }
         set { _recentList = value as RecentListCtrl; }
      }

      [Browsable(false)]
      [Description(DialogConsts.strCaFileDialogBase_SelectedTabIndex)]
      public int SelectedTabIndex
      {
         get
         {
            if (Handle == IntPtr.Zero || !_showRecent)
               return -1;
            return _recentTopForm.tabControl1.SelectedIndex;
         }
         set
         {
            if (Handle == IntPtr.Zero || !_showRecent)
               return;
            ShowStdSelectionCtrl(value == 0);
            ShowCustomSelectionCtrl(value == 1);
            _recentTopForm.tabControl1.SelectedIndex = value;
         }
      }

      [DefaultValue(FileListViewStyle.Default)]
      [Description("Gets or sets the style for the file list view control.")]
      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public FileListViewStyle EffectiveListViewStyle
      {
         get
         {
            FileListViewStyle res = FileListViewStyle.List;

            if (Handle != IntPtr.Zero)
            {
               if (IsWinXP())
               {
                  IntPtr st0 = Win32.SendMessageA(ListViewHandle, Win32.LVM_GETVIEW, IntPtr.Zero, IntPtr.Zero);

                  int st = st0.ToInt32();

                  if (st == Win32.LV_VIEW_ICON)
                  {
                     res = FileListViewStyle.LargeIcon;
                  }
                  else if (st == Win32.LV_VIEW_LIST)
                  {
                     res = FileListViewStyle.List;
                  }
                  else if (st == Win32.LV_VIEW_SMALLICON)
                  {
                     res = FileListViewStyle.SmallIcon;
                  }
                  else if (st == Win32.LV_VIEW_DETAILS)
                  {
                     res = FileListViewStyle.Details;
                  }
                  else if (st == Win32.LV_VIEW_TILE)
                  {
                     res = FileListViewStyle.Tiles;
                  }
                  else
                  {
                     res = FileListViewStyle.Thumbnails;
                  }
               }
               else
               {
                  int st0 = Win32.GetWindowLong(ListViewHandle, Win32.GWL_STYLE);
                  int st = st0 & Win32.LVS_TYPEMASK;
                  if (st == Win32.LVS_ICON)
                     res = FileListViewStyle.LargeIcon;
                  else if (st == Win32.LVS_REPORT)
                     res = FileListViewStyle.Details;
                  else if (st == Win32.LVS_SMALLICON)
                     res = FileListViewStyle.SmallIcon;
                  else if (st == Win32.LVS_LIST)
                     res = FileListViewStyle.List;
               }
            }

            return res;
         }
         set
         {
            if (Handle != IntPtr.Zero)
            {
               int val = Win32.SHVIEW_LARGEICON;
               if (value == FileListViewStyle.Default)
               {
                  return;
               }
               else if (value == FileListViewStyle.SmallIcon)
               {
                  val = Win32.SHVIEW_SMALLICON;
               }
               else if (value == FileListViewStyle.List)
               {
                  val = Win32.SHVIEW_LIST;
               }
               else if (value == FileListViewStyle.Details)
               {
                  val = Win32.SHVIEW_REPORT;
               }
               else if (value == FileListViewStyle.Tiles)
               {
                  val = Win32.SHVIEW_TILE;
               }
               else if (value == FileListViewStyle.Thumbnails)
               {
                  val = Win32.SHVIEW_THUMBNAIL;
               }

               Win32.SendMessage(DefViewHandle, Win32.WM_COMMAND, new IntPtr(val), IntPtr.Zero);
            }
         }
      }

      internal IntPtr ListViewHandle
      {
         get
         {
            return Win32.FindWindowEx(DefViewHandle, IntPtr.Zero, "SysListView32", IntPtr.Zero);
         }
      }

      internal IntPtr DefViewHandle
      {
         get
         {
            return Win32.FindWindowEx(Handle, IntPtr.Zero, "SHELLDLL_DefView", IntPtr.Zero);
         }
      }


      [DefaultValue(FileListViewStyle.Default)]
      [Description("Gets or sets the initial style for the file list view control.")]
      public FileListViewStyle InitialFileListViewStyle
      {
         get
         {
            return _initialFileListViewStyle;
         }
         set
         {
            _initialFileListViewStyle = value;
         }
      }

      [Description("Custom OK button's drop-down menu.")]
#if DOTNET2
      [Browsable(false)]
#endif
      public ContextMenu OkButtonMenu
      {
         get
         {
            return _customOkButtonMenu;
         }
         set
         {
            _customOkButtonMenu = value;
            _customOkBtn.DropDownMenu = value;
         }
      }

#if DOTNET2
      [Description("Custom OK button's drop-down menu.")]
      public ContextMenuStrip OkButtonMenuStrip
      {
         get
         {
            return _customOkButtonMenuStrip;
         }
         set
         {
            _customOkButtonMenuStrip = value;
            _customOkBtn.DropDownMenuStrip = value;
         }
      }
#endif

      [Description("Gets or sets custom captions for Browse and Recent tabs displayed when ShowRecentFilesList property is set to true.")]
      public string[] TabCaptions
      {
         get
         {
            return _recentTopForm.TabCaptions;
         }
         set
         {
            _recentTopForm.TabCaptions = value;
         }
      }

      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [DefaultValue(true)]
      public bool EnableSizing
      {
         get
         {
            return _enableSizing;
         }
         set
         {
            _enableSizing = value;
         }
      }
   }
}
