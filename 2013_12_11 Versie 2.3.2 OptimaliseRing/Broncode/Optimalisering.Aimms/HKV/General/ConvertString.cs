#region Copyright -------------------------------------------------------
// Copyright © 2006, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Abe Hoekstra, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/HKV/General/ConvertString.cs 2     30-03-09 13:16 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Microsoft.VisualBasic;

namespace Hkv.General
{
  /// <summary>
  /// Class used to convert strings to numbers (reals, ints etc.)
  /// </summary>
  public static class ConvertString
  {

    /// <summary>
    /// Converteer een String naar een int16
    /// </summary>
    /// <param name="textIn">De te converteren tekst</param>
    /// <returns>Een int16</returns>
    public static Int16 ToInt16(String textIn)
    {
      CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
      double value = System.Convert.ToDouble(textIn, ci);
      Int16 retval = retval = (Int16)Math.Max(Math.Min(value, (double)Int16.MaxValue), (double)Int16.MinValue);

      return retval;
    }

    /// <summary>
    /// Converteer een String naar een int32
    /// </summary>
    /// <param name="textIn">De te converteren tekst</param>
    public static Int32 ToInt32(String textIn)
    {
      CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
      double value = System.Convert.ToDouble(textIn, ci);
      Int32 retval = retval = (Int32)Math.Max(Math.Min(value, (double)Int32.MaxValue), (double)Int32.MinValue);

      return retval;
    }

    /// <summary>
    /// Converteer een String naar een int64
    /// </summary>
    /// <param name="textIn">De te converteren tekst</param>
    public static Int64 ToInt64(String textIn)
    {
      CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
      double value = System.Convert.ToDouble(textIn, ci);
      Int64 retval = retval = (Int64)Math.Max(Math.Min(value, (double)Int64.MaxValue), (double)Int64.MinValue);

      return retval;
    }

    /// <summary>
    /// Converteer een String naar een Double
    /// </summary>
    /// <param name="textIn">De te converteren tekst</param>
    public static Double ToDouble(String textIn)
    {
      CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
      Double doubleVal = System.Convert.ToDouble(Number2Culture(textIn, ci), ci);
      return doubleVal;
    }
    /// <summary>
    /// Converteer een String naar een Double
    /// </summary>
    /// <param name="textIn">De te converteren tekst</param>
    /// <param name="ci">Een CultureInfo object, hieruit wordt het decimaalscheidingssymbool gehaald.</param>
    public static Double ToDouble(String textIn, CultureInfo ci)
    {
      Double doubleVal = System.Convert.ToDouble(Number2Culture(textIn, ci), ci);
      return doubleVal;
    }
    /// <summary>
    /// Converteer een String naar een Single
    /// </summary>
    /// <param name="textIn">De te converteren tekst</param>
    public static Single ToSingle(String textIn)
    {
      CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
      Single singleVal = System.Convert.ToSingle(Number2Culture(textIn, ci), ci);
      return singleVal;
    }
    /// <summary>
    /// Converteer een String naar een Single
    /// </summary>
    /// <param name="textIn">De te converteren tekst</param>
    /// <param name="ci">Een CultureInfo object, hieruit wordt het decimaalscheidingssymbool gehaald.</param>
    public static Single ToSingle(String textIn, CultureInfo ci)
    {
      Single singleVal = System.Convert.ToSingle(Number2Culture(textIn, ci), ci);
      return singleVal;
    }


    /// <summary>
    /// Vervang in een tekst, die een getal voorstelt, het decimaalscheidingssymbool door het decimaalscheidingssymbool in de gegegeven Culture
    /// </summary>
    /// <param name="textIn">een String waarvan verondersteld wordt dat deze een getal representeerd.</param>
    /// <param name="ci">Een CultureInfo object, hieruit wordt het decimaalscheidingssymbool gehaald.</param>
    /// <returns>De tekst met het juiste decimaalscheidingssymbool</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    public static String Number2Culture(String textIn, CultureInfo ci)
    {
      if (!String.IsNullOrEmpty(textIn) & ci != null)
      {
        // Het karakter wat geen -0123456789 of eE is, wordt
        // verondersteld het decimaalscheidingssymbool te zijn.
        String valid = "-0123456789eE";
        int j = -1;
        for (int i = 0; i < textIn.Length & j == -1; i++)
        {
          if (valid.IndexOf(textIn.Substring(i, 1)) == -1)
          {
            // Karakter op positie i wordt verondersteld het decimaal-
            // scheidingssymbool te zijn.
            j = i;
          }
        }
        String retval = textIn;
        if (j != -1)
        {
          // Vervang het symbool door het decimaalscheidingssymbool uit de gegeven CultureInfo
          retval = retval.Replace(textIn.Substring(j, 1), ci.NumberFormat.NumberDecimalSeparator);
        }
        return retval;
      }
      else
      {
        return null;
      }

    }

    /// <summary>
    /// Vervang in een tekst, die een getal voorstelt, het decimaalscheidingssymbool door een punt.
    /// </summary>
    /// <param name="textIn">een String waarvan verondersteld wordt dat deze een getal representeerd.</param>
    /// <returns>De tekst met als decimaalscheidingssymbool en punt</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    public static String ToPoint(String textIn)
    {
      if (!String.IsNullOrEmpty(textIn))
      {
        // Het karakter wat geen -0123456789 of eE is, wordt
        // verondersteld het decimaalscheidingssymbool te zijn.
        String valid = "-0123456789eE";
        int j = -1;
        for (int i = 0; i < textIn.Length & j == -1; i++)
        {
          if (valid.IndexOf(textIn.Substring(i, 1)) == -1)
          {
            // Karakter op positie i wordt verondersteld het decimaal-
            // scheidingssymbool te zijn.
            j = i;
          }
        }
        String retval = textIn;
        if (j != -1)
        {
          // Vervang het symbool door het decimaalscheidingssymbool uit de gegeven CultureInfo
          retval = retval.Replace(textIn.Substring(j, 1), ".");
        }
        return retval;
      }
      else
      {
        return null;
      }

    }

  }
}
