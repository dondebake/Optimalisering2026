#region Copyright -------------------------------------------------------
// Copyright © 2008, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Abe Hoekstra, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.General/General/WindowsPathApi.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace OptimaliseRing.General
{
  /// <summary>
  /// Summary description for NativeMethods.
  /// </summary>
  public sealed class NativeMethods
  {
    #region Enumerated types

    public enum InfoLevels : int
    {
      /// <summary>
      /// The function will store a UniversalNameInfo data structure in the buffer.
      /// </summary>
      UniversalNameInfoLevel = 1,
      /// <summary>
      /// The function will store a REMOTE_NAME_INFO data structure in the buffer.
      /// </summary>
      RemoteNameInfoLevel = 2
    }

    public enum WNetGetUniversalNameReturnValues : int
    {
      /// <summary>
      /// indicates success.
      /// </summary>
      ErrorSuccess = 0,
      /// <summary>
      /// The string pointed to by lpLocalPath is invalid.
      /// </summary>
      ErrorBadDevice = 1200,
      /// <summary>
      /// There is no current connection to the remote device, but there is a remembered, or persistent, connection to it.
      /// </summary>
      ErrorConnectionUnavail = 1201,
      /// <summary>
      /// The buffer pointed to by lpBuffer is too small. The function sets the variable pointed to by lpBufferSize to the required buffer size. More entries are available with subsequent calls.
      /// </summary>
      ErrorMoreData = 234,
      /// <summary>
      /// No network present.
      /// </summary>
      ErrorNoNetwork = 1222,
      /// <summary>
      /// The device specified by lpLocalPath is not redirected.
      /// </summary>
      ErrorNotConnected = 2250
    }

    public enum DriveTypes : uint
    {
      /// <summary>
      /// The drive type cannot be determined.
      /// </summary>
      DriveUnknown = 0,
      /// <summary>
      /// The root path is invalid, for example, no volume is mounted at the path.
      /// </summary>
      DriveNotRootDir,
      /// <summary>
      /// The drive is a type that has removable media, for example, a floppy drive or removable hard disk.
      /// </summary>
      DriveRemovable,
      /// <summary>
      /// The drive is a type that cannot be removed, for example, a fixed hard drive.
      /// </summary>
      DriveFixed,
      /// <summary>
      /// The drive is a remote (network) drive.
      /// </summary>
      DriveRemote,
      /// <summary>
      /// The drive is a CD-ROM drive.
      /// </summary>
      DriveCdrom,
      /// <summary>
      /// The drive is a RAM disk.
      /// </summary>
      DriveRamdisk
    };

    #endregion

    #region Structures

    /// <summary>Unc name</summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct UniversalNameInfo
    {
      /// <summary>
      /// Long pointer to a zero-terminated UNC name string.
      /// </summary>
      [MarshalAs(UnmanagedType.LPTStr)]
      public String lpUniversalName;
    }
    #endregion

    #region Functions
    /// <summary>
    /// This function maps a local path for a network resource to a data structure containing the Universal Name Convention based name.
    /// </summary>
    /// <param name="lpLocalPath">Long pointer to a null-terminated string that is a local name for a network resource. It must point to the subfolder in the \NETWORK directory.</param>
    /// <param name="dwInfoLevel">[in] Specifies the type of data structure that the function will store in the buffer pointed to by lpBuffer. This parameter can be one of the following values.</param>
    /// <param name="lpBuffer">[out] Long pointer to a buffer that receives the type of data structure specified by the dwInfoLevel parameter.</param>
    /// <param name="lpBufferSize">[in, out] Long pointer to a variable that specifies the size in bytes of the buffer pointed to by lpBuffer.</param>
    [DllImport("mpr", CharSet = CharSet.Auto)]
    internal static extern WNetGetUniversalNameReturnValues WNetGetUniversalName(String lpLocalPath,
      InfoLevels dwInfoLevel, ref UniversalNameInfo lpBuffer, ref int lpBufferSize);

    /// <summary>
    /// This function maps a local path for a network resource to a data structure containing the Universal Name Convention based name.
    /// </summary>
    /// <param name="lpLocalPath">Long pointer to a null-terminated string that is a local name for a network resource. It must point to the subfolder in the \NETWORK directory.</param>
    /// <param name="dwInfoLevel">[in] Specifies the type of data structure that the function will store in the buffer pointed to by lpBuffer. This parameter can be one of the following values.</param>
    /// <param name="lpBuffer">[out] Long pointer to a buffer that receives the type of data structure specified by the dwInfoLevel parameter.</param>
    /// <param name="lpBufferSize">[in, out] Long pointer to a variable that specifies the size in bytes of the buffer pointed to by lpBuffer.</param>
    [DllImport("mpr", CharSet = CharSet.Auto)]
    internal static extern WNetGetUniversalNameReturnValues WNetGetUniversalName(String lpLocalPath,
      InfoLevels dwInfoLevel, IntPtr lpBuffer, ref int lpBufferSize);


    /// <summary>
    /// The GetDriveType function determines whether a disk drive is a removable, fixed, CD-ROM, RAM disk, or network drive.
    /// </summary>
    /// <param name="drive">
    /// Pointer to a null-terminated string that specifies the root directory of the disk to return information about.
    /// A trailing backslash is required. If this parameter is NULL, the function uses the root of the current directory.
    /// </param>
    [DllImport("Kernel32", CharSet = CharSet.Auto)]
    internal static extern DriveTypes GetDriveType(String drive);

    /// <summary>
    /// The GetShortpathName function retrieves the short path form of the specified path.
    /// </summary>
    /// <param name="lpszLongPath">[in] Pointer to a null-terminated path string. The function retrieves the short form of this path.</param>
    /// <param name="lpszShortPath">[out] Pointer to a buffer to receive the null-terminated short form of the path specified by lpszLongPath.</param>
    /// <param name="cchBuffer">[in] Size of the buffer pointed to by lpszShortPath, in TCHARs.</param>
    [DllImport("kernel32")]
    internal static extern uint GetShortPathName(String lpszLongPath,
       [Out] StringBuilder lpszShortPath, uint cchBuffer);

    /// <summary>
    /// The GetLongpathName function converts the specified path to its long form. If no long path is found, this function simply returns the specified name.
    /// </summary>
    /// <param name="lpszShortPath">[in] Pointer to a null-terminated path to be converted.</param>
    /// <param name="lpszLongPath">[out] Pointer to the buffer to receive the long path. You can use the same buffer you used for the lpszShortPath parameter.</param>
    /// <param name="cchBuffer">[in] Size of the buffer, in TCHARs.</param>
    [DllImport("kernel32")]
    internal static extern uint GetLongPathName(String lpszShortPath,
      [Out] StringBuilder lpszLongPath, uint cchBuffer);
    #endregion

    private NativeMethods()
    { // disallow instantiation of this class
    }


  }
}
