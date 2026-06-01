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
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

using ComponentAge.PlatformServices;

namespace ComponentAge.Dialogs
{
    [ToolboxItem(false)]
	[Description(DialogConsts.strCaPickIconDialogItemCaptions_TypeDesc)]
    public class CaPickIconDialogItemCaptions : CaDlgItemCaptions
    {
        // member fields
        string _browse;

        // properties
        [ItemID(12288)]
		[Description(DialogConsts.strCaPickIconDialogItemCaptions_Browse)]
        public string Browse
        {
            get {return _browse;}
            set {_browse = value;}
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public new string Help
        {
            get {return base.Help;}
            set {base.Help = value;}
        }
    }

	[DefaultEvent("Show")]
	[DefaultProperty("CustDlgParams")]
	[Designer(typeof(CaCommonDialogDesigner))]
    [LicenseProvider(typeof(LicUuidLicenseProvider))]
    [Guid("363dbe43-7b58-4018-83ef-40ae7a98ffee")]
    [System.Drawing.ToolboxBitmap(typeof(CaPickIconDialog))]
    [FullPreviewSupport(false)]
	[Description(DialogConsts.strCaPickIconDialog_TypeDesc)]
	public sealed class CaPickIconDialog : CaCommonDialog
	{
        // fields
        static CaPickIconDialog HookOwner;
        string _fileName = "";
        int _iconIndex;
        CaPickIconDialogItemCaptions _capts = new CaPickIconDialogItemCaptions();

		public CaPickIconDialog() :
			base()
		{
		}

        protected override bool RunDialog (IntPtr owner)
        {
            CaPickIconDialog prevHookOwner = HookOwner;
            HookOwner = this;
            InstallHookProc();
            bool ret = Win32.PickIconDlg(owner, _fileName, 4096, ref _iconIndex);
            HookOwner = prevHookOwner;
            return ret;
        }

        // properties
        [Description(DialogConsts.strCaPickIconDialog_IconIndex)]
        public int IconIndex
        {
            get {return _iconIndex;}
            set {_iconIndex = value;}
        }
        [Description(DialogConsts.strCaPickIconDialog_FileName)]
        public string FileName
        {
            get {return _fileName;}
            set {_fileName = value;}
        }

        [Description(DialogConsts.strPropDesc_DlgItemsCaptions)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CaPickIconDialogItemCaptions DlgItemsCaptions
        {
            get {return _capts;}
        }
		[Description(DialogConsts.strPropDesc_ActiveDialog)]
        public static CaPickIconDialog ActiveDialog
        {
            get {return HookOwner;}
        }
    }
}
