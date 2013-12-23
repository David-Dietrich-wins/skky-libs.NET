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
	}
}
