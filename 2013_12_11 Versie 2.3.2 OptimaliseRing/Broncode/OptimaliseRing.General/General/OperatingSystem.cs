#region Copyright -------------------------------------------------------
// Copyright © 2008, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Johan Ansink, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.General/General/OperatingSystem.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion

using System;

namespace OptimaliseRing.General
{
  /// <summary>
  /// A class to identify the current operating system.
  /// </summary>
  sealed public class OperatingSystem
  {

    /// <summary>
    /// Initializes a new instance of the <see cref="T:OperatingSystem"/> class.
    /// </summary>
    private OperatingSystem()
    {
    }

    /// <summary>
    /// Gets the operating sytem version
    /// </summary>
    /// <value>The version.</value>
    static public Version Version
    {
      get { return System.Environment.OSVersion.Version; }
    }

    /// <summary>
    /// Gets the platform ID.
    /// </summary>
    /// <value>The platform.</value>
    static public PlatformID Platform
    {
      get { return System.Environment.OSVersion.Platform; }
    }

    /// <summary>
    /// Gets the human-readable operating system name.
    /// </summary>
    /// <value>The name.</value>
    static public string Name
    {
      get
      {
        string temp = "Unknown";

        switch (Platform)
        {
          case PlatformID.Win32NT:
            switch (Version.Major)
            {
              case 3: temp = "Windows NT 3.51"; break;
              case 4: temp = "Windows NT 4.0"; break;
              case 5:
                switch (Version.Minor)
                {
                  case 0: temp = "Windows 2000"; break;
                  case 1: temp = "Windows XP"; break;
                  case 2: temp = "Windows 2003"; break;
                }
                break;
              case 6: temp = "Windows Longhorn"; break;
            }
            break;

          case PlatformID.Win32Windows:
            switch (Version.Minor)
            {
              case 0: temp = "Windows 95"; break;
              case 10: temp = "Windows 98"; break;
              case 90: temp = "Windows Me"; break;
            }
            break;

          case PlatformID.Win32S:
            temp = "Windows";
            break;

          default:
            temp = Platform.ToString();
            break;
        }

        return temp;
      }
    }

  }
}
