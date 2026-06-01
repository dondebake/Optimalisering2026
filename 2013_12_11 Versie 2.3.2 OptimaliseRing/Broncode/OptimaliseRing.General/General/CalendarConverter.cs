#region Copyright -------------------------------------------------------
// Copyright © 2008, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Johan Ansink, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.General/General/CalendarConverter.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion

using System;

namespace OptimaliseRing.General
{
	[Serializable]
	sealed public class CalendarConverter
	{
		private static DateTime modifiedJulianDateZero = new DateTime (1858, 11, 17);
		private static long     modifiedJulianDateZeroTicks = new DateTime (1858, 11, 17).Ticks;

		/// <summary>
		/// Converts a DateTime object to modified julian date
		/// Modified Julian Date is the number of days since November 17, 1858
		/// </summary>
		/// <param name="GregorianDate">DateTime object</param>
		/// <returns>Modified Julian Date (days since November 17, 1858)</returns>
		public static double Gregorian2ModifiedJulian(DateTime gregorianDate)
		{
			long ticks = gregorianDate.Ticks - modifiedJulianDateZeroTicks;
			double result = ((double) ticks) / ((double) TimeSpan.TicksPerDay);
			return result;
		}

		/// <summary>
		/// Converts a modified julian date to a DateTime object
		/// Modified Julian Date is the number of days since November 17, 1858
		/// </summary>
		/// <param name="GregorianDate">Modified Julian Date (days since November 17, 1858)</param>
		/// <returns>DateTime object</returns>
		public static DateTime ModifiedJulian2Gregorian(double modifiedJulianDate)
		{
			return modifiedJulianDateZero.AddDays (modifiedJulianDate);
		}
	}
}
