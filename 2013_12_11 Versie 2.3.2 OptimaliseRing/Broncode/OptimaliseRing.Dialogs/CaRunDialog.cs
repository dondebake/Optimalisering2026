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
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

using ComponentAge.PlatformServices;

namespace ComponentAge.Dialogs
{
    [ToolboxItem(false)]
	[Description(DialogConsts.strCaRunDialogItemCaptions_TypeDesc)]
    public class CaRunDialogItemCaptions : CaDlgItemCaptions
    {
		// fields
        string _browse = "";

		// properties
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)] // to hide this caption since this dialog does not contain this button
        public new string Help
        {
            get {return base.Help;}
            set {base.Help = value;}
        }

        [ItemID(12288)]
		[Description(DialogConsts.strCaRunDialogItemCaptions_Browse)]
        public string Browse
        {
            get {return _browse;}
            set {_browse = value;}
        }
    }

	[DefaultEvent("Show")]
	[DefaultProperty("CustDlgParams")]
	[Designer(typeof(CaCommonDialogDesigner))]
    [LicenseProvider(typeof(LicUuidLicenseProvider))]
    [Guid("f281fc5e-f518-4531-a0e9-95d024104cde")]
    [System.Drawing.ToolboxBitmap(typeof(CaRunDialog))]
    [FullPreviewSupport(false)]
	[Description(DialogConsts.strCaRunDialog_TypeDesc)]
	public sealed class CaRunDialog : CaCommonDialog
	{
        // private members
        static CaRunDialog HookOwner;
        string _desc = "";
        string _intialDir = "";
        Icon _icon;
        bool _showLabel = true;
        bool _showBrowseButton = true;
        bool _showDefaultItem = true;
        bool _calcDir = true;
        CaRunDialogItemCaptions _capts = new CaRunDialogItemCaptions();

		public CaRunDialog() :
			base()
		{
		}

        protected override bool RunDialog (IntPtr owner)
        {
            CaRunDialog prevHookOwner = HookOwner;
            HookOwner = this;
            InstallHookProc();
            int options = 0;
            if (!ShowBrowseButton)
                options = options | Win32.RFF_NOBROWSE;
            if (!ShowLabel)
                options = options | Win32.RFF_NOLABEL;
            if (CalcWorkingDirectory)
                options = options | Win32.RFF_CALCDIRECTORY;
            if (!ShowDefaultItem)
                options = options | Win32.RFF_NODEFAULT;

            Win32.RunFileDlg(owner, _icon != null ? _icon.Handle: IntPtr.Zero, _intialDir, Title, _desc, options);
            HookOwner = prevHookOwner;
            return true;
        }

        // properties
        [Description(DialogConsts.strCaRunDialog_Description)]
        public string Description
        {
            get {return _desc;}
            set {_desc = value;}
        }

		[Description(DialogConsts.strCaRunDialog_InitialDirectory)]
        public string InitialDirectory
        {
            get {return _intialDir;}
            set {_intialDir = value;}
        }

		[Description(DialogConsts.strCaRunDialog_CalcDir)]
        [DefaultValue(true)]
        public bool CalcWorkingDirectory
        {
            get {return _calcDir;}
            set {_calcDir = value;}
        }

		[Description(DialogConsts.strCaRunDialog_ShowBrowseButton)]
        [DefaultValue(true)]
        public bool ShowBrowseButton
        {
            get {return _showBrowseButton;}
            set {_showBrowseButton = value;}
        }

		[Description(DialogConsts.strCaRunDialog_ShowLabel)]
        [DefaultValue(true)]
        public bool ShowLabel
        {
            get {return _showLabel;}
            set {_showLabel = value;}
        }

		[Description(DialogConsts.strCaRunDialog_ShowDefaultItem)]
        [DefaultValue(true)]
        public bool ShowDefaultItem
        {
            get {return _showDefaultItem;}
            set {_showDefaultItem = value;}
        }

		[Description(DialogConsts.strCaRunDialog_Icon)]
        public Icon Icon
        {
            get {return _icon;}
            set
            {
                if (value != null)
                    _icon = new Icon(value, value.Width, value.Height);
                else
                    _icon = null;
            }
        }

		[Description(DialogConsts.strPropDesc_ActiveDialog)]
        public static CaRunDialog ActiveDialog
        {
            get {return HookOwner;}
        }

        [Description(DialogConsts.strPropDesc_DlgItemsCaptions)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CaRunDialogItemCaptions DlgItemsCaptions
        {
            get {return _capts;}
        }

    }
}
