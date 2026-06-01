using System;
using System.Text;
using System.Runtime.InteropServices;

namespace ComponentAge.PlatformServices
{
    internal class ShellUtils
    {
        internal static IntPtr GetPidlFromPath(IntPtr wnd, string path)
        {
            IShellFolder shf = null;
            Win32.SHGetDesktopFolder(ref shf);
            int eaten = 0;
            int attr = 0;
            IntPtr pidl = IntPtr.Zero;
            shf.ParseDisplayName(wnd, IntPtr.Zero, path, out eaten, out pidl, ref attr);
            return pidl;
        }

        internal static string GetFolderFromPidl(IntPtr wnd, int flags)
        {
            IntPtr plist = IntPtr.Zero;
            int n = Win32.SendMessage(wnd, Win32.CDM_GETFOLDERIDLIST, IntPtr.Zero, plist).ToInt32();
            plist = Marshal.AllocCoTaskMem(n);
            n = Win32.SendMessage(wnd, Win32.CDM_GETFOLDERIDLIST, new IntPtr(n), plist).ToInt32();
            IntPtr pidlRelative = IntPtr.Zero;
            PlatformServices.STRRET str = new STRRET();
            StringBuilder sb = new StringBuilder(Win32.MAX_PATH + 1);

            IntPtr ptrParent;

            int hres = Win32.SHBindToParent(plist, IID_IShellFolder, out ptrParent, ref pidlRelative);

            System.Type shellFolderType = typeof(IShellFolder);
            Object obj = System.Runtime.InteropServices.Marshal.GetTypedObjectForIUnknown(ptrParent, shellFolderType);
            IShellFolder ishellParent = (IShellFolder)obj;

            ishellParent.GetDisplayNameOf(pidlRelative, flags, out str);
            Win32.StrRetToBuf(ref str, pidlRelative, sb, Win32.MAX_PATH);

            if (plist != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(plist);
            }

            if (ishellParent != null)
            {
                Marshal.ReleaseComObject(ishellParent);
            }

            return sb.ToString();
        }

        public static Guid IID_IShellFolder = new Guid("{000214E6-0000-0000-C000-000000000046}");
    }
}
