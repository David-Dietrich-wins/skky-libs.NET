using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace skky.util
{
	public static class ExcelHelper
	{
		public static string GetExcelColumnString(DataRow row, string columnName)
		{
			string s = string.Empty;
			if (null != row && !string.IsNullOrEmpty(columnName))
			{
				try
				{
					var col = row[columnName];
					if (null != col)
						s = col.ToString();
				}
				catch (Exception ex)
				{
					skky.util.Trace.Warning("Error getting Excel column named: " + columnName + ".", ex);
					throw ex;
				}
			}

			return s;
		}
		public static string GetExcelColumnStringSafe(DataRow row, string columnName)
		{
			if (null != row && !string.IsNullOrEmpty(columnName))
			{
				try
				{
					return GetExcelColumnString(row, columnName);
				}
				catch
				{ }
			}

			return string.Empty;
		}
		public static bool? GetExcelColumnBoolean(DataRow row, string columnName)
		{
			string s = GetExcelColumnString(row, columnName);
			if (!string.IsNullOrEmpty(s))
			{
				s = s.Trim().ToLower();

				bool b = false;
				bool bSuccess = bool.TryParse(s, out b);
				if (bSuccess)
					return b;

				int i = 0;
				switch (s)
				{
					case "t":
					case "true":
					case "y":
					case "yes":
						return true;

					case "f":
					case "false":
					case "n":
					case "no":
						return false;

					default:
						bSuccess = int.TryParse(s, out i);
						if (bSuccess)
							return (i == 0 ? false : true);
						break;
				}
			}

			return null;
		}

		public static int? GetExcelColumnInt(DataRow row, string columnName)
		{
			string s = GetExcelColumnRegexString(row, columnName);
			if (!string.IsNullOrEmpty(s))
			{
				int i = 0;
				bool b = int.TryParse(s, out i);
				if (b)
					return i;
			}

			return null;
		}
		public static double? GetExcelColumnDouble(DataRow row, string columnName)
		{
			string s = GetExcelColumnString(row, columnName);
			if (!string.IsNullOrEmpty(s))
			{
				double dbl = 0.0;
				bool b = double.TryParse(s, out dbl);
				if (b)
					return dbl;
			}

			return null;
		}
		public static decimal? GetExcelColumnDecimal(DataRow row, string columnName)
		{
			string s = GetExcelColumnString(row, columnName);
			if (!string.IsNullOrEmpty(s))
			{
				decimal dcl = 0.0M;
				bool b = decimal.TryParse(s, out dcl);
				if (b)
					return dcl;
			}

			return null;
		}
		public static DateTime? GetExcelColumnDateTime(DataRow row, string columnName)
		{
			string s = GetExcelColumnString(row, columnName);
			if (!string.IsNullOrEmpty(s))
			{

				DateTime dt = DateTime.Now;
				bool b = DateTime.TryParse(s, out dt);
				if (b)
					return dt;
			}

			return null;
		}

		public static string FindRegexColumnName(DataRow row, string columnName)
		{
			if (!string.IsNullOrEmpty(columnName)
				&& !(null == row || null == row.Table || null == row.Table.Columns))
			{
				int colnum = 0;
				// Look for exact matches first.
				int lenColumnName = columnName.Length;
				for (colnum = 0; colnum < row.Table.Columns.Count; ++colnum)
				{
					var col = row.Table.Columns[colnum];
					if (null != col && !string.IsNullOrEmpty(col.ColumnName))
					{
						var right = col.ColumnName.Right(lenColumnName);
						if (right == columnName)
							return col.ColumnName;
					}
				}

				var regex = new Regex(columnName);
				for (colnum = 0; colnum < row.Table.Columns.Count; ++colnum)
				{
					var col = row.Table.Columns[colnum];
					if (regex.IsMatch(col.ColumnName))
					{
						return col.ColumnName;
					}
				}
			}

			return string.Empty;
		}

		public static bool? GetExcelColumnRegexBoolean(DataRow row, string columnName)
		{
			string str = FindRegexColumnName(row, columnName);
			return GetExcelColumnBoolean(row, str);
		}
		public static string GetExcelColumnRegexString(DataRow row, string columnName)
		{
			string str = FindRegexColumnName(row, columnName);
			return GetExcelColumnString(row, str);
		}
		public static int? GetExcelColumnRegexInt(DataRow row, string columnName)
		{
			string str = FindRegexColumnName(row, columnName);
			return GetExcelColumnInt(row, str);
		}
		public static double? GetExcelColumnRegexDouble(DataRow row, string columnName)
		{
			string str = FindRegexColumnName(row, columnName);
			return GetExcelColumnDouble(row, str);
		}
		public static decimal? GetExcelColumnRegexDecimal(DataRow row, string columnName)
		{
			string str = FindRegexColumnName(row, columnName);
			return GetExcelColumnDecimal(row, str);
		}
		public static DateTime? GetExcelColumnRegexDateTime(DataRow row, string columnName)
		{
			string str = FindRegexColumnName(row, columnName);
			return GetExcelColumnDateTime(row, str);
		}
	}
}
