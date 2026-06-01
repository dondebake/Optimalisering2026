#region Copyright -------------------------------------------------------
// Copyright © 2008, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Johan Ansink, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.UI/Classes/Map/ColorConverter.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.Drawing;

namespace OptimaliseRing.UI
{
  /// <summary>
  ///
  /// </summary>
	[Serializable]
	sealed public class ColorConverter
	{
    /// <summary>
    /// Convert a UInt32 color to a System.Drawing.Color
    /// </summary>
    /// <param name="color">The UInt32color.</param>
    /// <returns>System.Drawing.Color</returns>
    public static Color UInt32ToColor(UInt32 color)
    {
      string hex = color.ToString("X6");

      byte a = 255;
      byte b = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
      byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
      byte r = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

      Color.FromArgb(a, r, g, b);

      return Color.FromArgb(a, r, g, b);
    }

    /// <summary>
    /// Convert a System.Drawing.Color to a UInt32
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns></returns>
    public static UInt32 ColorToUInt32(Color color)
    {
      string hex = ToHex(color);

      UInt32 kleur = uint.Parse(hex, System.Globalization.NumberStyles.HexNumber);

      return kleur;
    }

    /// <summary>
    /// Toes the hex.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns></returns>
    public static string ToHex(Color color)
    {
      return string.Format("{0:X2}{1:X2}{2:X2}", color.B, color.G, color.R);
    }

	}
}
