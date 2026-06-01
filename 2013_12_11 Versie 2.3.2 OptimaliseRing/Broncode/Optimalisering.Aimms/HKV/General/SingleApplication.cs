#region Copyright -------------------------------------------------------
// Copyright © 2006, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Abe Hoekstra, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/HKV/General/SingleApplication.cs 3     31-03-09 10:30 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Windows.Forms;

namespace Hkv.General
{
  /// <summary>
  /// Summary description for SingleApp.
  /// </summary>
  public sealed class SingleApplication
  {
    private SingleApplication()
    {

    }
    /// <summary>
    /// Imports
    /// </summary>

    [DllImport("user32.dll")]
    private static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern int SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int IsIconic(IntPtr hWnd);

    /// <summary>
    /// GetCurrentInstanceWindowHandle
    /// </summary>
    /// <returns></returns>
    private static IntPtr GetCurrentInstanceWindowHandle()
    {
      IntPtr hWnd = IntPtr.Zero;
      Process process = Process.GetCurrentProcess();
      Process[] processes = Process.GetProcessesByName(process.ProcessName);
      foreach (Process _process in processes)
      {
        // Get the first instance that is not this instance, has the
        // same process name and was started from the same file name
        // and location. Also check that the process has a valid
        // window handle in this session to filter out other user's
        // processes.
        if (_process.Id != process.Id &&
          _process.MainModule.FileName == process.MainModule.FileName &&
          _process.MainWindowHandle != IntPtr.Zero)
        {
          hWnd = _process.MainWindowHandle;
          break;
        }
      }
      return hWnd;
    }
    /// <summary>
    /// SwitchToCurrentInstance
    /// </summary>
    private static void SwitchToCurrentInstance()
    {
      IntPtr hWnd = GetCurrentInstanceWindowHandle();
      if (hWnd != IntPtr.Zero)
      {
        // Restore window if minimised. Do not restore if already in
        // normal or maximised window state, since we don't want to
        // change the current state of the window.
        if (IsIconic(hWnd) != 0)
        {
          ShowWindow(hWnd, SW_RESTORE);
        }

        // Set foreground window.
        SetForegroundWindow(hWnd);
      }
    }

    /// <summary>
    /// Execute a form base application if another instance already running on
    /// the system activate previous one
    /// </summary>
    /// <param name="frmMain">main form</param>
    /// <returns>true if no previous instance is running</returns>
    public static bool Run(System.Windows.Forms.Form frmMain)
    {
      if (IsAlreadyRunning())
      {
        //set focus on previously running app
        SwitchToCurrentInstance();
        return false;
      }
      Application.Run(frmMain);
      return true;
    }

    /// <summary>
    /// for console base application
    /// </summary>
    /// <returns></returns>
    public static bool Run()
    {
      if (IsAlreadyRunning())
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// check if given exe alread running or not
    /// </summary>
    /// <returns>returns true if already running</returns>
    private static bool IsAlreadyRunning()
    {
      String strLoc = Assembly.GetExecutingAssembly().Location;
      FileSystemInfo fileInfo = new FileInfo(strLoc);
      String sExeName = fileInfo.Name;
      bool bCreatedNew;

      mutex = new Mutex(true, "Global\\" + sExeName, out bCreatedNew);
      if (bCreatedNew)
        mutex.ReleaseMutex();

      return !bCreatedNew;
    }

    static Mutex mutex;
    const int SW_RESTORE = 9;
    /// <summary>
    /// Starts the help.
    /// </summary>
    /// <param name="helpfile">The helpfile.</param>
    /// <param name="programfile">The programfile.</param>
    /// <param name="titel">The titel.</param>
    public static void StartHelp(string helpfile, string programfile, string titel)
    {

      IntPtr handle = Hkv.General.SingleApplication.GetCurrentInstanceWindowHandle(programfile, titel);

      if (handle != IntPtr.Zero)
      {
        Hkv.General.SingleApplication.SwitchToCurrentInstance(handle);
      }
      else
      {
        Process helpProcess = new Process();
        ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(helpfile);
        helpProcess.StartInfo = myProcessStartInfo;
        helpProcess.Start();
      }
    }

    /// <summary>
    /// Gets the current instance window handle.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="titel">The titel.</param>
    /// <returns></returns>
    private static IntPtr GetCurrentInstanceWindowHandle(string file, string titel)
    {
      IntPtr hWnd = IntPtr.Zero;
      Process[] processes = Process.GetProcesses();

      foreach (Process process in processes)
      {
        Console.WriteLine(process.MainWindowTitle);

        // Get the first instance that is not this instance, has the
        // same process name and was started from the same file name
        // and location. Also check that the process has a valid
        // window handle in this session to filter out other user's
        // processes.
        if (process.MainWindowHandle != IntPtr.Zero)
        {
          if (process.MainModule.ModuleName == file)
          {
            if (process.MainWindowTitle.ToLower().Contains(titel.ToLower()))
            {
              hWnd = process.MainWindowHandle;
              break;
            }
          }
        }
      }
      return hWnd;
    }

    /// <summary>
    /// Switches to current instance.
    /// </summary>
    /// <param name="hWnd">The h WND.</param>
    private static void SwitchToCurrentInstance(IntPtr hWnd)
    {

      if (hWnd != IntPtr.Zero)
      {
        // Restore window if minimised. Do not restore if already in
        // normal or maximised window state, since we don't want to
        // change the current state of the window.
        if (IsIconic(hWnd) != 0)
        {
          ShowWindow(hWnd, SW_RESTORE);
        }

        // Set foreground window.
        SetForegroundWindow(hWnd);
      }
    }

  }
}
