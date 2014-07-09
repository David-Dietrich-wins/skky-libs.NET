using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using skky.Types;

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

		public static string GetExcelColumnString(ReturnStatus rs, int lineNumber, DataRow row, string columnName)
		{
			string s = string.Empty;
			if (null == row)
			{
				rs.ErrorMessage.Add("Attempting to search for an Excel column when there is no row present.");
			}
			//rs.ErrorMessage.Add("Attempting to search for an Excel column that is empty and not named. You must provide a column name.");
			else if (!string.IsNullOrEmpty(columnName))
			{
				try
				{
					if (row.Table.Columns.Contains(columnName))
					{
						var col = row[columnName];
						if (null != col)
							s = col.ToString();
					}
				}
				catch (Exception ex)
				{
					rs.ErrorMessage.Add("Error on line " + lineNumber.ToString() + " retrieving String for column: " + columnName + ". Data read is " + s + ".");
					skky.util.Trace.Warning("Line #" + lineNumber.ToString() + ". Error getting Excel column named: " + columnName + ".", ex);
				}
			}

			return s;
		}
		public static bool GetExcelColumnBooleanNoNull(ReturnStatus rs, int lineNumber, DataRow row, string columnName)
		{
			bool? b = GetExcelColumnBoolean(rs, lineNumber, row, columnName);

			return b ?? false;
		}
		public static bool? GetExcelColumnBoolean(ReturnStatus rs, int lineNumber, DataRow row, string columnName)
		{
			string s = GetExcelColumnString(rs, lineNumber, row, columnName);
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

						rs.ErrorMessage.Add("Error on line " + lineNumber.ToString() + " parsing Boolean on column: " + columnName + ". Data read is " + s + ".");
						break;
				}
			}

			return null;
		}

		public static bool isAcceptableNumericEntry(string s)
		{
			s = (s ?? string.Empty).Trim().ToLower();
			if (string.IsNullOrEmpty(s))
				return true;

			if ("na" == s)
				return true;
			if ("n/a" == s)
				return true;

			return false;
		}

		public static int? GetExcelColumnInt(ReturnStatus rs, int lineNumber, DataRow row, string columnName)
		{
			string s = GetExcelColumnRegexString(rs, lineNumber, row, columnName);
			if (!string.IsNullOrEmpty(s))
			{
				int i = 0;
				bool b = int.TryParse(s, out i);
				if (b)
					return i;

				if (!isAcceptableNumericEntry(s))
					rs.ErrorMessage.Add("Error on line " + lineNumber.ToString() + " parsing Integer on column: " + columnName + ". Data read is " + s + ".");
			}

			return null;
		}
		public static double? GetExcelColumnDouble(ReturnStatus rs, int lineNumber, DataRow row, string columnName)
		{
			string s = GetExcelColumnString(rs, lineNumber, row, columnName);
			if (!string.IsNullOrEmpty(s))
			{
				double dbl = 0.0;
				bool b = double.TryParse(s, out dbl);
				if (b)
					return dbl;

				if (!isAcceptableNumericEntry(s))
					rs.ErrorMessage.Add("Error on line " + lineNumber.ToString() + " parsing DateTime on column: " + columnName + ". Data read is " + s + ".");
			}

			return null;
		}
		public static decimal? GetExcelColumnDecimal(ReturnStatus rs, int lineNumber, DataRow row, string columnName)
		{
			string s = GetExcelColumnString(rs, lineNumber, row, columnName);
			if (!string.IsNullOrEmpty(s))
			{
				decimal dcl = 0.0M;
				bool b = decimal.TryParse(s, out dcl);
				if (b)
					return dcl;

				if (!isAcceptableNumericEntry(s))
					rs.ErrorMessage.Add("Error on line " + lineNumber.ToString() + " parsing Double on column: " + columnName + ". Data read is " + s + ".");
			}

			return null;
		}
		public static DateTime? GetExcelColumnDateTime(ReturnStatus rs, int lineNumber, DataRow row, string columnName)
		{
			string s = GetExcelColumnString(rs, lineNumber, row, columnName);
			if (!string.IsNullOrEmpty(s))
				s = s.Trim();

			if (!string.IsNullOrEmpty(s))
			{
				DateTime dt = DateTime.Now;
				bool b = DateTime.TryParse(s, out dt);
				if (b)
					return dt;

				rs.ErrorMessage.Add("Error on line " + lineNumber.ToString() + " parsing DateTime on column: " + columnName + ". Data read is " + s + ".");
			}

			return null;
		}

		public static string FindRegexColumnName(ReturnStatus rs, int lineNumber, DataRow row, string columnName)
		{
			if (!string.IsNullOrEmpty(columnName)
				&& !(null == row || null == row.Table || null == row.Table.Columns))
			{
				int colnum = 0;
				int colCount = row.Table.Columns.Count;
				// Look for exact matches first.
				int lenColumnName = columnName.Length;
				for (colnum = 0; colnum < colCount; ++colnum)
				{
					var col = row.Table.Columns[colnum];
					if (null != col && !string.IsNullOrEmpty(col.ColumnName))
					{
						var right = col.ColumnName.Right(lenColumnName);
						if (right == columnName)
							return col.ColumnName;
					}
				}

				var regex = new Regex(columnName.Replace("(", "\\(").Replace(")", "\\)"));
				for (colnum = 0; colnum < colCount; ++colnum)
				{
					var col = row.Table.Columns[colnum];
					if (regex.IsMatch(col.ColumnName))
					{
						return col.ColumnName;
					}
				}
			}

			//rs.ErrorMessage.Add("Unable to find Excel column named like: " + columnName ?? "NO COLUMN NAME PROVIDED, on line #" + lineNumber.ToString() + ".");
			return string.Empty;
		}

		public static bool? GetExcelColumnRegexBoolean(ReturnStatus rs, int lineNumber, DataRow row, string columnName)
		{
			string str = FindRegexColumnName(rs, lineNumber, row, columnName);
			return GetExcelColumnBoolean(rs, lineNumber, row, str);
		}
		public static string GetExcelColumnRegexString(ReturnStatus rs, int lineNumber, DataRow row, string columnName)
		{
			string str = FindRegexColumnName(rs, lineNumber, row, columnName);
			return GetExcelColumnString(rs, lineNumber, row, str);
		}
		public static int? GetExcelColumnRegexInt(ReturnStatus rs, int lineNumber, DataRow row, string columnName)
		{
			string str = FindRegexColumnName(rs, lineNumber, row, columnName);
			return GetExcelColumnInt(rs, lineNumber, row, str);
		}
		public static double? GetExcelColumnRegexDouble(ReturnStatus rs, int lineNumber, DataRow row, string columnName)
		{
			string str = FindRegexColumnName(rs, lineNumber, row, columnName);
			return GetExcelColumnDouble(rs, lineNumber, row, str);
		}
		public static decimal? GetExcelColumnRegexDecimal(ReturnStatus rs, int lineNumber, DataRow row, string columnName)
		{
			string str = FindRegexColumnName(rs, lineNumber, row, columnName);
			return GetExcelColumnDecimal(rs, lineNumber, row, str);
		}
		public static DateTime? GetExcelColumnRegexDateTime(ReturnStatus rs, int lineNumber, DataRow row, string columnName)
		{
			string str = FindRegexColumnName(rs, lineNumber, row, columnName);
			return GetExcelColumnDateTime(rs, lineNumber, row, str);
		}
	}
}
