#region Copyright -------------------------------------------------------
// Copyright © 2008, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Abe Hoekstra, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.General/General/MyPath.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace OptimaliseRing.General
{
  public static class MyPath
  {
    /// <summary>
    /// Constant specifying the maximum path length.
    /// </summary>
    private const int MAX_PATH = 260;

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

    /// <summary>
    /// Returns the pathName relative to the program directory name
    /// </summary>
    /// <param name="pathName">String representing a path.</param>
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
    /// <param name="pathName">Name of the file.</param>
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
  }

}
