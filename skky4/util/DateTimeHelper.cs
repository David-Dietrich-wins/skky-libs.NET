using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace skky.util
{
	public static class DateTimeHelper
	{
		public const string PublicDateFormat = "yyyy-MM-dd";
		public const string PublicDateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffzzz";

		public const string CONST_DefaultDateFormat = ﻿"M/d/yy";
		public const string CONST_DefaultDateTimeFormat = ﻿"M/d/yy h:mm:ss tt";

		public static readonly string[] CONST_MonthNames = {
															   "January",
															   "February",
															   "March",
															   "April",
															   "May",
															   "June",
															   "July",
															   "August",
															   "September",
															   "October",
															   "November",
															   "December"
												 };

		public static readonly long DatetimeMinTimeTicks =
		   (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

		public static string GetDefaultDateString(DateTime? dtDateTime)
		{
			if (dtDateTime.HasValue && dtDateTime.Value != DateTime.MinValue)
			{
				return dtDateTime.Value.ToString(CONST_DefaultDateFormat);
			}

			return string.Empty;
		}
		public static string GetDefaultDateTimeString(DateTime? dtDateTime)
		{
			if (dtDateTime.HasValue && dtDateTime.Value != DateTime.MinValue)
			{
				return dtDateTime.Value.ToString(CONST_DefaultDateTimeFormat);
			}

			return string.Empty;
		}

		public static string ToPublicDateTimeFormat(this DateTime dt)
		{
			string result = null;
			if (null != dt && dt != DateTime.MinValue)
			{
				result = dt.ToString(PublicDateTimeFormat, CultureInfo.InvariantCulture);
			}

			return result;
		}

		public static DateTime ToDateTime(this string str)
		{
			DateTime date = DateTime.MinValue;
			if (!string.IsNullOrWhiteSpace(str))
				if (DateTime.TryParse(str.Trim(), out date))
					return date;

			return DateTime.MinValue;
		}
		public static DateTime? ToNullableDateTime(this string str)
		{
			DateTime date = (str ?? string.Empty).ToDateTime();
			if (date == DateTime.MinValue)
				return null;

			return date;
		}
		//public static DateTime GetDateTimeFromObject(this object o)
		//{
		//    if (o != null)
		//    {
		//        try
		//        {
		//            return (DateTime)o;
		//        }
		//        catch (Exception)
		//        {
		//            return o.ToString().ToDateTime();
		//        }
		//    }

		//    return new DateTime();
		//}

		public static int ToDateKey(this DateTime dt)
		{
			return dt == null ? 0 : dt.Year * 10000 + dt.Month * 100 + dt.Day;
		}
		public static DateTime FromDateKey(this int dateKey)
		{
			string s = dateKey.ToString();
			if (!string.IsNullOrEmpty(s))
			{
				int yearLength = 0;

				if (s.Length == 6)
					yearLength = 2;
				else if (s.Length == 8)
					yearLength = 4;

				if(yearLength > 0)
				{
					int year = s.Left(yearLength).ToInteger();
					int month = s.Mid(yearLength, 2).ToInteger();
					int day = s.Right(2).ToInteger();

					return new DateTime(year, month, day);
				}
			}

			return DateTime.MinValue;
		}
		public static string ToSummaryString(this DateTime instance)
		{
			if (null == instance)
				return string.Empty;

			return instance.ToString(PublicDateFormat);
		}
		public static long? ToUnixTimestamp(this DateTime? dateTime)
		{
			if (!dateTime.HasValue)
				return null;

			return (long)(dateTime.Value - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
		}

		public static long ToJavaScriptMilliseconds(this DateTime dt)
		{
			return (long)((dt.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000);
		}

		/// <summary>
		/// Returns a DateTime based on the month passed in.
		/// If the month is greater than the current month, the year is changed to last year.
		/// As we only want to select from months that have Lead data.
		/// 
		/// </summary>
		/// <param name="month">The long text of the month to return with the year.</param>
		/// <returns>A DateTime representing a Month and a Year.</returns>
		public static DateTime getMonthBasedDateTime(string month, int year = 0)
		{
			DateTime dtNow = DateTime.Now;
			if (!string.IsNullOrEmpty(month))
			{
				if (year < 1)
					year = dtNow.Year;
				DateTime dtParsed = DateTime.ParseExact(month + "/" + year, "MMMM/yyyy", null);
				return (dtParsed > dtNow ? dtParsed.AddMonths(-1) : dtParsed);
			}

			return dtNow;
		}
		/// <summary>
		/// Returns a DateTime based on the month passed in.
		/// 
		/// </summary>
		/// <param name="month">The long text of the month to return with the year.</param>
		/// <returns>A DateTime representing a Month and a Year.</returns>
		public static DateTime GetMonthYearBasedDateTime(string monthYear)
		{
			DateTime dtNow = DateTime.Now;
			if (!string.IsNullOrEmpty(monthYear))
			{
				DateTime dtParsed = DateTime.ParseExact(monthYear, "MMMM yyyy", null);
				return (dtParsed > dtNow ? dtParsed.AddMonths(-1) : dtParsed);
			}

			return dtNow;
		}

		public static string DefaultMonthText()
		{
			DateTime dtMonth = getMonthBasedDateTime(string.Empty);

			return dtMonth.ToString("MMMM");
		}

		public static string DefaultMonthYearText()
		{
			DateTime dtMonth = getMonthBasedDateTime(string.Empty);

			return dtMonth.ToString("MMMM yyyy");
		}
		public static int DefaultMonthInt()
		{
			DateTime dtMonth = getMonthBasedDateTime(string.Empty);

			return dtMonth.Month;
		}

		/// <summary>
		/// Returns a new DateTime with the first day of the month and year passed in.
		/// </summary>
		/// <param name="monthYear">Month year string.</param>
		/// <returns>DateTime of the month/year passed in. If monthYear is invalid, returns the first day of the previous month</returns>
		public static DateTime GetStartDateFromMonthYear(string monthYear, bool usePreviousMonthIfBadString = true)
		{
			DateTime? dt = monthYear.ToNullableDateTime();
			if (null == dt)
			{
				DateTime dtNow = DateTime.Now;
				if (usePreviousMonthIfBadString)
					dt = dtNow.AddMonths(-1);
				else
					dt = dtNow;
			}

			dt = new DateTime(dt.Value.Year, dt.Value.Month, 1);

			return dt.Value;
		}

		/// <summary>
		/// We want to set everything to the first of the next month passed in.
		/// If an empty DateTime is passed in, use the current month.
		/// </summary>
		/// <param name="dt">A month and year DateTime to get the next month for.</param>
		/// <returns>A month and year one month ahead with the day being the first.</returns>
		public static DateTime GetFirstDayOfNextMonth(DateTime? dt)
		{
			if (null == dt)
			{
				DateTime dtNow = DateTime.Now;
				dt = new DateTime(dtNow.Year, dtNow.Month, 1);
			}
			else
			{
				dt = new DateTime(dt.Value.Year, dt.Value.Month, 1);
			}

			return dt.Value.AddMonths(1);
		}

		public static string TimeDifferenceMessage(DateTime dtStart, DateTime dtEnd)
		{
			string msg = string.Empty;
			TimeSpan ts = dtEnd - dtStart;
			if (ts.Minutes > 0)
				msg += ts.Minutes.ToString() + "m";
			if (ts.Seconds > 0 || ts.Minutes > 0)
			{
				if (!string.IsNullOrEmpty(msg))
					msg += " ";

				msg += ts.Seconds.ToString() + "s";
			}
			if (ts.Milliseconds > 0 || ts.Seconds > 0 || ts.Minutes > 0)
			{
				if (!string.IsNullOrEmpty(msg))
					msg += " ";

				msg += ts.Milliseconds.ToString() + "ms";
			}

			return msg;
		}
	}
}
