using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.util
{
	public static class FileHelper
	{
		public static string getFileDateStyle(DateTime? dateTime = null)
		{
			string fileDateStyle = string.Empty;
		
			DateTime dt = (dateTime.HasValue ? dateTime.Value : DateTime.Now);
			int year = (dt.Year % 1000);
			int month = dt.Month;
			int day = dt.Day;

			fileDateStyle += year.ToString().PadLeft(2, '0');
			fileDateStyle += month.ToString().PadLeft(2, '0');
			fileDateStyle += day.ToString().PadLeft(2, '0');

			return fileDateStyle;
		}
		public static string getFileDateTimeStyle(DateTime? dateTime = null)
		{
			string fileDateStyle = string.Empty;

			DateTime dt = (dateTime.HasValue ? dateTime.Value : DateTime.Now);
			int year = (dt.Year % 1000);
			int month = dt.Month;
			int day = dt.Day;
			int hour = dt.Hour;
			int min = dt.Minute;
			int sec = dt.Second;

			fileDateStyle += year.ToString().PadLeft(2, '0');
			fileDateStyle += month.ToString().PadLeft(2, '0');
			fileDateStyle += day.ToString().PadLeft(2, '0');
			fileDateStyle += hour.ToString().PadLeft(2, '0');
			fileDateStyle += min.ToString().PadLeft(2, '0');
			fileDateStyle += sec.ToString().PadLeft(2, '0');

			return fileDateStyle;
		}
		public static string getFileDateTimeWithGuid(string extension = null, DateTime? dateTime = null)
		{
			string str = getFileDateTimeStyle(dateTime);
			str += ".";
			str += Guid.NewGuid().ToString();
			str += ".";
			str += (string.IsNullOrEmpty(extension) ? "txt" : extension);

			return str;
		}
		public static string getFileNameWithDateTime(string prefix, string extension = null, DateTime? dateTime = null)
		{
			string str = prefix ?? string.Empty;
			if (!string.IsNullOrEmpty(str))
				str += ".";
			str += getFileDateTimeStyle(dateTime);
			str += ".";
			str += (string.IsNullOrEmpty(extension) ? "txt" : extension);

			return str;
		}
	}
}
