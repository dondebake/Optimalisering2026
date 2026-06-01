//---------------------------------------------------------------------------
// Dialog Workshop for .NET Framework
// Copyright (c) COMPONENTAGE Software,
// all rights reserved
//
// http://www.componentage.com
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
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;

using ComponentAge.PlatformServices;

namespace ComponentAge.Dialogs
{
    [System.Drawing.ToolboxBitmap(typeof(CaOpenFileDialog))]
#if !DEBUG
    [LicenseProvider(typeof(LicUuidLicenseProvider))]
#endif
    [Guid("ef6400a9-2938-4d65-a8fd-0659d07bbdb5")]
    [Description(DialogConsts.strCaOpenFileDialog_TypeDesc)]
	public sealed class CaOpenFileDialog : ComponentAge.Dialogs.CaFileDialogBase
	{
		// fields
        bool _showReadOnly;
        bool _readOnlyChecked;

		// ctor
		public CaOpenFileDialog():
            base()
		{
		}

		// methods
        protected override void SetFlags (ref int flags)
        {
            base.SetFlags(ref flags);
            if (!ShowReadOnly) flags |= Win32.OFN_HIDEREADONLY;
            if (ReadOnlyChecked) flags |= Win32.OFN_READONLY;
        }
        protected override bool CallFileDialog (ref Win32.OpenFileNameEx ofnEx)
        {
            return Win32.GetOpenFileName(ofnEx);
        }

        [Description(DialogConsts.strCaOpenFileDialog_OpenFile)]
        public Stream OpenFile ()
        {
            FileStream fs = new FileStream(this.FileName, FileMode.Open, FileAccess.Read);
            return fs;
        }
        [Description(DialogConsts.strCaFileDialogBase_RunDialog)]
        protected override bool RunDialog (IntPtr owner)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT
                &&
                Environment.OSVersion.Platform != PlatformID.Win32Windows)
            {
                OpenFileDialog d = new OpenFileDialog();
                d.AddExtension = AddExtension;
                d.CheckFileExists = CheckFileExists;
                d.CheckPathExists = CheckPathExists;
                d.FileName = FileName;
                d.DefaultExt = DefaultExt;
                d.InitialDirectory = InitialDirectory;
                d.ReadOnlyChecked = ReadOnlyChecked;
                d.Multiselect = Multiselect;
                if (d.ShowDialog() == DialogResult.OK)
                {
                    FileNames = d.FileNames;
                    FileName = d.FileName;
                    _readOnlyChecked = d.ReadOnlyChecked;
                    return true;
                }
                return false;
            }

            return base.RunDialog(owner);
        }

        //properties
        [Description(DialogConsts.strPropDesc_ActiveDialog)]
        public static CaOpenFileDialog ActiveDialog
        {
            get {return HookOwner as CaOpenFileDialog;}
        }
        [DefaultValue(false)]
        [Description(DialogConsts.strCaOpenFileDialog_ShowReadOnly)]
        public bool ShowReadOnly
        {
            get {return _showReadOnly;}
            set {_showReadOnly = value;}
        }

        [DefaultValue(false)]
        [Description(DialogConsts.strCaOpenFileDialog_ReadOnlyChecked)]
        public bool ReadOnlyChecked
        {
            get {return _readOnlyChecked;}
            set {_readOnlyChecked = value;}
        }
	}


    [Description(DialogConsts.strCaSaveFileDialog_TypeDesc)]
    [System.Drawing.ToolboxBitmap(typeof(ComponentAge.Dialogs.CaSaveFileDialog))]
    [Guid("a5fe829c-515b-4016-8a53-24951b215b71")]
    [LicenseProvider(typeof(LicUuidLicenseProvider))]
    public sealed class CaSaveFileDialog : ComponentAge.Dialogs.CaFileDialogBase
    {
		// fields
        private bool _overwritePrompt = true;
        private bool _createPrompt = false;

		// ctor
        public CaSaveFileDialog() :
            base()
        {
        }

        // overrides
        protected override void OnShow()
        {
            base.OnShow();

            if (DlgItemsCaptions.OK != null && DlgItemsCaptions.OK.Length > 0)
            {
                IntPtr dlgItem = Win32.GetDlgItem(Handle, Win32.IDOK);
                Win32.SetWindowText(dlgItem, DlgItemsCaptions.OK);
            }
        }

		// methods
        protected override bool CallFileDialog (ref Win32.OpenFileNameEx ofnEx)
        {
            //return Win32.ShowSaveFileDialog(ref ofnEx);
            return Win32.GetSaveFileName(ofnEx);
        }

        protected override bool OnFileOk ()
        {
            if (!CreatePrompt)
                return base.OnFileOk();
            string[] ss = SelectedFiles;
            System.Collections.Specialized.StringCollection nonExistentFiles = new System.Collections.Specialized.StringCollection();
            for (int i = 0; i < ss.Length; i++)
            {
                if (!(new System.IO.FileInfo(ss[i])).Exists)
                    nonExistentFiles.Add(ss[i]);
            }
            if (nonExistentFiles.Count == 1)
            {
                if (MessageBox.Show(string.Format(LocalizableConsts.strCreatePrompt, new object[]{nonExistentFiles[0]}), Title, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                    return true;
            }
            else if (nonExistentFiles.Count > 1)
            {
                string fileNames = nonExistentFiles[0];
                for (int i = 1; i < nonExistentFiles.Count; i++)
                {
                    fileNames += ", " + nonExistentFiles[i];
                }
                if (MessageBox.Show(string.Format(LocalizableConsts.strCreatePrompt2, new object[]{fileNames}), Title, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                    return true;
            }
            return base.OnFileOk();
        }


		[Description(DialogConsts.strCaSaveFileDialog_OpenFile)]
        public Stream OpenFile ()
        {
            FileStream fs = new FileStream(this.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            return fs;
        }

		protected override void SetFlags (ref int flags)
        {
            base.SetFlags(ref flags);
            if (CreatePrompt) flags |= Win32.OFN_CREATEPROMPT;
            if (OverwritePrompt) flags |= Win32.OFN_OVERWRITEPROMPT;
        }

		[Description(DialogConsts.strCaFileDialogBase_RunDialog)]
        protected override bool RunDialog (IntPtr owner)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT
                &&
                Environment.OSVersion.Platform != PlatformID.Win32Windows)
            {
                SaveFileDialog d = new SaveFileDialog();
                d.AddExtension = AddExtension;
                d.CheckFileExists = CheckFileExists;
                d.CheckPathExists = CheckPathExists;
                d.FileName = FileName;
                d.DefaultExt = DefaultExt;
                d.InitialDirectory = InitialDirectory;
                d.CreatePrompt = CreatePrompt;
                d.OverwritePrompt = OverwritePrompt;
                if (d.ShowDialog() == DialogResult.OK)
                {
                    FileNames = d.FileNames;
                    FileName = d.FileName;
                    _overwritePrompt = d.OverwritePrompt;
                    _createPrompt = d.CreatePrompt;
                    return true;
                }
                return false;
            }

            return base.RunDialog(owner);
        }

        //properties
		[Description(DialogConsts.strPropDesc_ActiveDialog)]
        public static CaSaveFileDialog ActiveDialog
        {
            get {return HookOwner as CaSaveFileDialog;}
        }

        [DefaultValue(false)]
        [Description(DialogConsts.strCaSaveFileDialog_CreatePrompt)]
        public bool CreatePrompt
        {
            get {return _createPrompt;}
            set {_createPrompt = value;}
        }

		[DefaultValue(true)]
        [Description(DialogConsts.strCaSaveFileDialog_OverwritePrompt)]
        public bool OverwritePrompt
        {
            get {return _overwritePrompt;}
            set {_overwritePrompt = value;}
        }
    }
}
