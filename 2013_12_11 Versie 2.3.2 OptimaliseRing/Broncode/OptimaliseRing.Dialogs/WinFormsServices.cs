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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;


namespace ComponentAge.PlatformServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate int OFNHookProc(IntPtr wnd, int msg, IntPtr wparam, IntPtr lparam);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate long GetLongProc(IntPtr wnd);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate int WindowProcedure (IntPtr wnd, int msg, IntPtr wparam, IntPtr lparam);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate int PrintHookProc (IntPtr wnd, int msg, IntPtr wparam, IntPtr lparam);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate int SetupHookProc (IntPtr wnd, int msg, IntPtr wparam, IntPtr lparam);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate int PagePaintHookProc(IntPtr wnd, int msg, IntPtr wparam, IntPtr lparam);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate int PageSetupHookProc(IntPtr wnd, int msg, IntPtr wparam, IntPtr lparam);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate int WndEnumProc(IntPtr wnd, IntPtr lparam);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate IntPtr HookProc(int nCode, IntPtr wparam, IntPtr lparam);

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class WinFormsServices
	{
        internal static Control FindControlAt (int x, int y)
        {
            Win32.POINT pnt = new Win32.POINT();
            pnt.x = x;
            pnt.y = y;
            IntPtr p = Win32.WindowFromPoint(ref pnt);
            if (p == IntPtr.Zero)
                return null;
            return Control.FromHandle(p);
        }
        internal static void CenterWindow (IntPtr handle, IntPtr parent)
        {
            Win32.RECT r = new Win32.RECT(0,0,0,0);
            Win32.RECT r1 = new Win32.RECT(0,0,0,0);
            Win32.GetWindowRect(handle, ref r);
            Win32.GetWindowRect(parent, ref r1);
            Win32.SetWindowPos(handle, Win32.HWND_TOP,
                r.left - ((r.right+r.left)/2 - (r1.right+r1.left)/2),
                r.top - ((r.bottom+r1.top)/2 - (r1.bottom+r1.top)/2),
                0, 0, Win32.SWP_NOSIZE | Win32.SWP_NOZORDER | Win32.SWP_NOACTIVATE);

        }
        internal static void CenterWindow (IntPtr handle)
        {
            Win32.RECT r = new Win32.RECT(0,0,0,0);
            Win32.GetWindowRect(handle, ref r);
            Win32.RECT r1 = new Win32.RECT((Win32.GetSystemMetrics(Win32.SM_CXSCREEN) - r.right + r.left) / 2,
                                                   (Win32.GetSystemMetrics(Win32.SM_CYSCREEN) - r.bottom + r.top) / 2,
                                                   r.right - r.left, r.bottom - r.top);
            FitRectToScreen(ref r1);
            Win32.SetWindowPos(handle, Win32.HWND_TOP, r1.left, r1.top, 0, 0, Win32.SWP_NOACTIVATE | Win32.SWP_NOSIZE | Win32.SWP_NOZORDER);
        }
        internal static void FitRectToScreen (ref Win32.RECT r)
        {
            int x = 0;
            int y = 0;
            int delta = 0;
            x = Win32.GetSystemMetrics(Win32.SM_CXSCREEN);
            y = Win32.GetSystemMetrics(Win32.SM_CYSCREEN);
            if (r.right > x)
            {
                delta = r.right - r.left;
                r.right = x;
                r.left = r.right - r.left;
            }
            if (r.left < 0)
            {
                delta = r.right - r.left;
                r.left = 0;
                r.right = r.left + delta;
            }
            if (r.bottom > y)
            {
                delta = r.bottom - r.top;
                r.bottom = y;
                r.top = r.bottom - delta;
            }
            if (r.top < 0)
            {
                delta = r.bottom - r.top;
                r.top = 0;
                r.bottom = r.top + delta;
            }
        }
	}
}
