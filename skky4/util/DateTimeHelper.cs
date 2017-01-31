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

		public static readonly DateTime UnixEpoch =
			new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	
		/// <summary>
		/// Returns a string in the format M/d/yy.
		/// </summary>
		/// <param name="dtDateTime">DateTime object to make a string from.</param>
		/// <returns>A string in the format M/d/yy.</returns>
		public static string GetDefaultDateString(DateTime? dtDateTime)
		{
			if (dtDateTime.HasValue && dtDateTime.Value != DateTime.MinValue)
			{
				return dtDateTime.Value.ToString(CONST_DefaultDateFormat);
			}

			return string.Empty;
		}

		/// <summary>
		/// Returns a string in the format M/d/yy h:mm:ss tt.
		/// </summary>
		/// <param name="dtDateTime">DateTime object to make a string from.</param>
		/// <param name="tzoMinutes">Timezone offset in minutes (positive number west of UTC).</param>
		/// <returns>A string in the format M/d/yy h:mm:ss tt.</returns>
		public static string GetDefaultDateTimeString(DateTime? dtDateTime, int tzoMinutes = 0)
		{
			return GetString(dtDateTime, tzoMinutes, CONST_DefaultDateTimeFormat);
		}

		public static string GetString(DateTime? dtDateTime, int tzoMinutes = 0, string format = "M/d/yyyy")
		{
			if (dtDateTime.HasValue)
			{
				if (dtDateTime == DateTime.MinValue || dtDateTime == DateTime.MaxValue)
					return dtDateTime.Value.ToString(format);
				else
					return dtDateTime.Value.AddMinutes(0 - tzoMinutes).ToString(format);
			}

			return string.Empty;
		}

		public static string ToPublicDateTimeFormat(this DateTime dt, int tzoMinutes = 0)
		{
			string result = null;
			if (null != dt && dt != DateTime.MinValue)
				result = dt.AddMinutes(0 - tzoMinutes).ToString(PublicDateTimeFormat, CultureInfo.InvariantCulture);

			return result;
		}

		/// <summary>
		/// Returns a valid date if the string supplied is a valid parseable date.
		/// </summary>
		/// <param name="str">A string to parse for a date time.</param>
		/// <param name="getFirstOfMonth">If true, don't parse day. Set the day to 1.</param>
		/// <returns>Returns a date if str is valid. Otherwise null.</returns>
		public static DateTime? ToDateTimeNullable(this string str, bool getFirstOfMonth = false)
		{
			if (string.IsNullOrWhiteSpace(str))
				return null;

			return str.ToDateTime(getFirstOfMonth);
		}
		/// <summary>
		/// Returns a valid date if the string supplied is a valid parseable date.
		/// </summary>
		/// <param name="str">A string to parse for a date time.</param>
		/// <param name="getFirstOfMonth">If true, don't parse day. Set the day to 1.</param>
		/// <returns>Returns a date if str is valid. Otherwise DateTime.MinValue.</returns>
		public static DateTime ToDateTime(this string str, bool getFirstOfMonth = false)
		{
			DateTime date = DateTime.MinValue;
			if (!string.IsNullOrWhiteSpace(str))
				if (!DateTime.TryParse(str.Trim(), out date))
					return DateTime.MinValue;

			if (getFirstOfMonth)
				date = new DateTime(date.Year, date.Month, 1);

			return date;
		}

		public static long GetUnixTimestampMillis(DateTime? dt = null)
		{
			if (!dt.HasValue)
				dt = DateTime.UtcNow;

			return (long)(dt.Value - UnixEpoch).TotalMilliseconds;
		}

		public static DateTime DateTimeFromUnixTimestampMillis(long millis)
		{
			return UnixEpoch.AddMilliseconds(millis);
		}

		public static long GetUnixTimestampSeconds(DateTime? dt = null)
		{
			if (!dt.HasValue)
				dt = DateTime.UtcNow;

			return ToUnixTimestamp(dt);
		}

		public static DateTime DateTimeFromUnixTimestampSeconds(long seconds)
		{
			return UnixEpoch.AddSeconds(seconds);
		}

		public static long ToUnixTimestamp(this DateTime? dateTime)
		{
			if (null == dateTime)
				return 0;

			return (long)(dateTime.Value.ToUniversalTime() - UnixEpoch).TotalSeconds;
		}
		public static long ToUnixTimestampMillis(this DateTime? dateTime)
		{
			if (null == dateTime)
				return 0;

			return (long)(dateTime.Value.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
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
		public static DateTime GetDateFromMonthYear(string monthYear, bool usePreviousMonthIfBadString = true)
		{
			DateTime? dt = monthYear.ToDateTimeNullable();
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

		public static DateTime DateAtMidnight(DateTime dt)
		{
			if (null == dt)
				dt = new DateTime();

			return new DateTime(dt.Year, dt.Month, dt.Day);
		}
	}
}
