#region Copyright -------------------------------------------------------
// Copyright © 2006, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Abe Hoekstra, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/HKV/General/MyPath.cs 3     31-03-09 10:30 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace Hkv.General
{
  public static class MyPath
  {
    /// <summary>
    /// Constant specifying the maximum path length.
    /// </summary>
    private const int MAX_PATH = 260;

    public static bool CheckFileIsWritable(string fileName)
    {
      if (fileName.Length > 0)
      {
        if (System.IO.File.Exists(fileName))
        {
          System.IO.FileAttributes attributes = System.IO.File.GetAttributes(fileName);
          if (!((attributes & System.IO.FileAttributes.ReadOnly) == System.IO.FileAttributes.ReadOnly))
          {
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Returns the absolute pathName.
    /// </summary>
    /// <param name="pathName">String representing a path.</param>
    public static String AbsoluteName(String pathName)
    {
      if (pathName == null || pathName.Length == 0) return String.Empty;

      String retval = pathName;

      if (retval[0] == '.')
      {
        retval = Path.Combine(ProgramDirectoryName(), retval);
      }
      retval = Unc2Path(retval);
      return Path.GetFullPath(retval);
    }

    public static string ExtractFilename(string path)
    {
      string[] split = path.Split('\\');
      return split[split.Length - 1];
    }

    /// <summary>
    /// Returns the pathName relative to the program directory name
    /// </summary>
    /// <param name="pathName">String representing a path.</param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToUpper")]
    public static String RelativeName(string pathName)
    {
      if (pathName == null || pathName.Length == 0) return String.Empty;

      if (pathName.ToUpper().StartsWith(ProgramDirectoryName().ToUpper()))
      {
        return "." + Path.DirectorySeparatorChar + pathName.Substring(ProgramDirectoryName().Length);

      }
      return pathName;
    }

    /// <summary>
    /// Returns the UNC name of the specified path.
    /// </summary>
    /// <param name="pathName">String representing a path.</param>
    public static String UncName(String pathName)
    {
      return PathToUnc(pathName);
    }

    /// <summary>
    /// This function retrieves the short path form of the specified path.
    /// </summary>
    /// <param name="pathName">String representing a path.</param>
    public static String ShortName(String pathName)
    {
      if (pathName == null || pathName.Length == 0) return String.Empty;

      StringBuilder shortNameBuffer = new StringBuilder(MAX_PATH);
      uint bufferSize = (uint)shortNameBuffer.Capacity;

      uint result = NativeMethods.GetShortPathName(pathName, shortNameBuffer, bufferSize);
      if (result == 0)
      {
        // Failed to convert to a short name. Does the file exist?
        return pathName;
      }
      return shortNameBuffer.ToString();

    }

    /// <summary>
    /// This function converts the specified path to its long form. If no long path is found, this function simply returns the specified name.
    /// </summary>
    /// <param name="pathName">String representing a path.</param>
    public static String LongName(String pathName)
    {
      if (pathName == null || pathName.Length == 0) return String.Empty;

      StringBuilder longNameBuffer = new StringBuilder(MAX_PATH);
      uint bufferSize = (uint)longNameBuffer.Capacity;

      uint result = NativeMethods.GetLongPathName(pathName, longNameBuffer, bufferSize);
      if (result == 0)
      {
        // Failed to convert to a short name. Does the file exist?
        return pathName;
      }
      return longNameBuffer.ToString();

    }

    /// <summary>
    /// Returns the UNC path for a mapped drive.
    /// </summary>
    /// <param name="pathName">String representing a path.</param>
    /// <returns>The UNC path (if available)</returns>
    public static String PathToUnc(String pathName)
    {
      if (pathName == null || pathName.Length == 0) return String.Empty;

      pathName = Path.GetFullPath(pathName);
      if (!IsValidFilePath(pathName)) return pathName;

      NativeMethods.WNetGetUniversalNameReturnValues nRet = NativeMethods.WNetGetUniversalNameReturnValues.ErrorSuccess;
      NativeMethods.UniversalNameInfo rni = new NativeMethods.UniversalNameInfo();
      int bufferSize = Marshal.SizeOf(rni);

      nRet = NativeMethods.WNetGetUniversalName(pathName, NativeMethods.InfoLevels.UniversalNameInfoLevel,
        ref rni, ref bufferSize);

      if (nRet == NativeMethods.WNetGetUniversalNameReturnValues.ErrorMoreData)
      {
        IntPtr pBuffer = Marshal.AllocHGlobal(bufferSize);
        try
        {
          nRet = NativeMethods.WNetGetUniversalName(pathName, NativeMethods.InfoLevels.UniversalNameInfoLevel,
            pBuffer, ref bufferSize);

          if (nRet == NativeMethods.WNetGetUniversalNameReturnValues.ErrorSuccess)
          {
            rni = (NativeMethods.UniversalNameInfo)Marshal.PtrToStructure(pBuffer,
              typeof(NativeMethods.UniversalNameInfo));
          }
        }
        finally
        {
          Marshal.FreeHGlobal(pBuffer);
        }
      }

      switch (nRet)
      {
        case NativeMethods.WNetGetUniversalNameReturnValues.ErrorSuccess:
          return rni.lpUniversalName;

        default:
          return pathName;
      }
    }

    /// <summary>
    /// Returns the program directory name.
    /// </summary>
    public static String ProgramDirectoryName()
    {
      try
      {
        Assembly a = Assembly.GetEntryAssembly();
        if (a == null)
        {
          a = Assembly.GetExecutingAssembly();
        }
        return Unc2Path(System.IO.Path.GetDirectoryName(a.Location));

      }
      catch
      {
        return String.Empty;  // if all else fails
      }
    }
    /// <summary>
    /// Returns true if pathName is a valid local file-name of the form:
    /// X:\, where X is a drive letter from A-Z
    /// </summary>
    /// <param name="pathName">The pathName to check</param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.Char.ToUpper(System.Char)")]
    public static bool IsValidFilePath(String pathName)
    {
      if (null == pathName || 0 == pathName.Length) return false;

      char drive = char.ToUpper(pathName[0]);
      if ('A' > drive || drive > 'Z')
        return false;

      else if (Path.VolumeSeparatorChar != pathName[1])
        return false;
      else if (Path.DirectorySeparatorChar != pathName[2])
        return false;
      else
        return true;
    }

    /// <summary>
    /// Returns pathName with mapped drive letter of possible.
    /// </summary>
    /// <param name="pathName">The path to map.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToUpper")]
    private static String Unc2Path(String pathName)
    {
      for (char Drive = 'A'; Drive <= 'Z'; Drive++)
      {
        String Map = Drive.ToString() + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar;
        switch (NativeMethods.GetDriveType(Map))
        {
          case NativeMethods.DriveTypes.DriveRemote:

            String UNCName = PathToUnc(Map);

            if (pathName.ToUpper().StartsWith(UNCName.ToUpper()))
            {
              return Drive.ToString() + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar + pathName.Substring(UNCName.Length);
            }

            break;
        }
      }
      return pathName;
    }

    private sealed class NativeMethods
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

}
