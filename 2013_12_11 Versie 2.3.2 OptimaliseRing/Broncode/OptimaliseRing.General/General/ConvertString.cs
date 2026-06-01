#region Copyright -------------------------------------------------------
// Copyright © 2005 HKV lijn in water, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Project    : OptimaliseRing.General
//
// Author(s)  : Abe Hoekstra, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.General/General/ConvertString.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Microsoft.VisualBasic;

namespace OptimaliseRing.General
{
   /// <summary>
   /// Class used to convert strings to numbers (reals, ints etc.)
   /// </summary>
   public static class ConvertString
   {
      /// <summary>
      /// Converteer een string naar een int16
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
      /// Converteer een string naar een int32
      /// </summary>
      /// <param name="textIn">De te converteren tekst</param>
      public static Int32 ToInt32(String textIn)
      {
         CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
         double value = System.Convert.ToDouble(textIn, ci);

         Int32 retval = retval = Convert.ToInt32(Math.Max(Math.Min(value, (double)Int32.MaxValue), (double)Int32.MinValue));

         //Int32 retval = retval = (Int32)Math.Max(Math.Min(value, (double)Int32.MaxValue), (double)Int32.MinValue);

         return retval;
      }

      /// <summary>
      /// Converteer een string naar een int64
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
      /// Converteer een string naar een boolean
      /// </summary>
      /// <param name="textIn">De te converteren tekst</param>
      public static Boolean ToBoolean(String textIn)
      {
         CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
         Boolean boolValue = System.Convert.ToBoolean(textIn, ci);
         return boolValue;
      }

      /// <summary>
      /// Converteer een string naar een Double
      /// </summary>
      /// <param name="textIn">De te converteren tekst</param>
      public static Double ToDouble(String textIn)
      {
         Double doubleVal = double.PositiveInfinity;
         if (string.Compare(textIn, double.MinValue.ToString(), true) == 0
           || (textIn.Contains("79769313486232E+308") && textIn.Contains("-")))
         {
           doubleVal = double.MinValue;
         }
         else if (string.Compare(textIn, double.MaxValue.ToString(), true) == 0
           || textIn.Contains("79769313486232E+308"))
         {
           doubleVal = double.MaxValue;
         }
         else if (string.Compare(textIn, "infinity", true) != 0)
         {
            CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            doubleVal = System.Convert.ToDouble(Number2Culture(textIn, ci), ci);
         }
         return doubleVal;
      }
      /// <summary>
      /// Converteer een string naar een Double
      /// </summary>
      /// <param name="textIn">De te converteren tekst</param>
      /// <param name="ci">Een CultureInfo object, hieruit wordt het decimaalscheidingssymbool gehaald.</param>
      public static Double ToDouble(String textIn, CultureInfo ci)
      {
         Double doubleVal = System.Convert.ToDouble(Number2Culture(textIn, ci), ci);
         return doubleVal;
      }
      /// <summary>
      /// Converteer een string naar een Single
      /// </summary>
      /// <param name="textIn">De te converteren tekst</param>
      public static Single ToSingle(String textIn)
      {
         CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
         Single singleVal = System.Convert.ToSingle(Number2Culture(textIn, ci), ci);
         return singleVal;
      }
      /// <summary>
      /// Converteer een string naar een Single
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
      /// <param name="textIn">een string waarvan verondersteld wordt dat deze een getal representeerd.</param>
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
   }
}
