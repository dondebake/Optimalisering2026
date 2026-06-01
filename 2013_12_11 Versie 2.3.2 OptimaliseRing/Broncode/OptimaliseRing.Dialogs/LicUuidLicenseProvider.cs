//---------------------------------------------------------------------------
// Dialog Workshop for .NET Framework
// Copyright (c) 2002 - 2003, COMPONENTAGE Software,
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
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace ComponentAge.Dialogs
{
    internal class UuidLicense : License
    {
        string _key = "";
        public UuidLicense (string key) : base()
        {
            _key = key;
        }
        public override void Dispose ()
        {
        }
        public override string LicenseKey
        {
            get
            {
                return _key;
            }
        }
    }

	internal class LicUuidLicenseProvider : LicenseProvider
	{
		private static bool _shownag = true;

		public LicUuidLicenseProvider() : base ()
		{
		}

		public override License GetLicense(
            LicenseContext context,
            Type type,
            object instance,
            bool allowExceptions
            )
        {
			string guid = GetGuid(type);
#if !REGISTERED
			if (_shownag && context.UsageMode == LicenseUsageMode.Designtime)
			{
				DesignerFormEx form  = new DesignerFormEx();
				form.ShowDialog();
				_shownag = false;
			}
			else if (_shownag && context.UsageMode == LicenseUsageMode.Runtime)
			{
				DialogDesignerFormEx form  = new DialogDesignerFormEx();
				form.ShowDialog();
				form.BringToFront();
				_shownag = false;
			}
#else
			// for registered version check Licenes\<GUID> in design-time and
			// do not show anything in run-time
            bool PlacesBarSupported = false;
            if (context.UsageMode == LicenseUsageMode.Runtime)
                PlacesBarSupported = true;   // run-time
            bool check1 = false;
            bool check2 = false;
            if (!PlacesBarSupported)
            {
				try
				{
					string s1 = Environment.GetFolderPath(Environment.SpecialFolder.System);
					string licFileName = s1 + '\\' + DialogConsts.strAssemblyName + ".lic";
					StreamReader reader = new StreamReader(licFileName);
					char[] buf = new char [reader.BaseStream.Length];
					reader.ReadBlock(buf, 0, (int)reader.BaseStream.Length);
					string s2 = new string (buf);
					string[] strs = s2.Split(new char[] {'\n'}, 10);
					for (int i = 0; i < strs.GetLength(0); i++ )
					{
						string candidate = strs[i].Replace('\r', ' ').Trim();
						if (candidate.ToLower().Equals(guid.ToLower()))
						{
							check1 = true;
							break;
						}
					}
				}
				catch{}
            }
            if (!PlacesBarSupported && check1)
            {
                RegistryKey regKey = Registry.ClassesRoot.OpenSubKey("Licenses\\" + guid, false);
                if (regKey != null) // key exists
                    check2 = true;
            }
			if ((!check1 || !check2) && !PlacesBarSupported) // desing-time
			{
				if (_shownag)
				{
					DesignerFormEx form  = new DesignerFormEx();
					form.ShowDialog();
					_shownag = false;
				}
				return null;
			}
#endif
            return new UuidLicense(guid);
        }
		protected string GetGuid (Type type)
        {
            string guid = "";
            object[] attr = type.GetCustomAttributes(true);
            for (int i = 0; i < attr.Length; i++ )
            {
                if (attr[i] is GuidAttribute)
                {
                    GuidAttribute guida = (GuidAttribute)attr[i];
                    guid = guida.Value;
                    break;
                }
            }
            return guid;
        }
    }
}
