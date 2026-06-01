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
   [Description(DialogConsts.strCaAboutDialogItemCaptions_TypeDesc)]
   public class CaAboutDialogItemCaptions : CaDlgItemCaptions
   {
      [Browsable(false)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      public new string Help
      {
         get { return base.Help; }
         set { base.Help = value; }
      }

      [Browsable(false)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      public new string Cancel
      {
         get { return base.Cancel; }
         set { base.Cancel = value; }
      }
   }

   [LicenseProvider(typeof(LicUuidLicenseProvider))]
   [Guid("390317ee-8999-4d4d-8c7b-0939c8121c5c")]
   [System.Drawing.ToolboxBitmap(typeof(CaAboutDialog))]
   [FullPreviewSupport(false)]
   [Description(DialogConsts.strCaAboutDialog_TypeDesc)]
   public sealed class CaAboutDialog : CaCommonDialog
   {
      // private members
      static CaAboutDialog HookOwner;
      string _first = "";
      string _desc = "";
      Icon _icon;
      CaAboutDialogItemCaptions _capts = new CaAboutDialogItemCaptions();

      // constructor
      public CaAboutDialog()
         : base()
      {
      }

      // overrides
      protected override bool RunDialog(IntPtr owner)
      {
         CaAboutDialog prevHookOwner = HookOwner;
         HookOwner = this;
         InstallHookProc();
         string text = Title + "#" + _first;
         int ret = Win32.ShellAbout(owner, text, _desc, _icon != null ? _icon.Handle : IntPtr.Zero);
         HookOwner = prevHookOwner;
         return (ret != 0);
      }

      // properties
      [Description(DialogConsts.strCaAboutDialog_Description)]
      public string Description
      {
         get { return _desc; }
         set { _desc = value; }
      }

      [Description(DialogConsts.strCaAboutDialog_FirstLineText)]
      public string FirstLineText
      {
         get { return _first; }
         set { _first = value; }
      }

      [Description(DialogConsts.strCaAboutDialog_Icon)]
      public Icon Icon
      {
         get { return _icon; }
         set
         {
            if (value != null)
               _icon = new Icon(value, value.Width, value.Height);
            else
               _icon = null;
         }
      }

      [Description(DialogConsts.strPropDesc_ActiveDialog)]
      public static CaAboutDialog ActiveDialog
      {
         get { return HookOwner; }
      }

      [Description(DialogConsts.strPropDesc_DlgItemsCaptions)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
      public CaAboutDialogItemCaptions DlgItemsCaptions
      {
         get { return _capts; }
      }
   }
}
