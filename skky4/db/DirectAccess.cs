using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Types;
using skky.util;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace skky.db
{
	public delegate System.Data.Linq.DataContext DataContextDelegate();

	public class DirectAccess
	{
		public const string Const_Null = "NULL";
		private const int Const_DefaultCommandTimeout = 180;

		public static string getConnectionString()
		{
			return Properties.Settings.Default.skkyConnectionString;
		}

		public static System.Data.Linq.DataContext ObjectsDataContextDelegate()
		{
			return new ObjectsDataContext();
		}

		public static int ExecuteNonQuery(string sql, bool trapException)
		{
			try
			{
				if (!string.IsNullOrEmpty(sql))
				{
					using (var db = new ObjectsDataContext())
					{
						return ExecuteNonQuery(sql, db, trapException);
					}
				}
			}
			catch (Exception ex)
			{
				skky.util.Trace.Critical(sql);
				skky.util.Trace.Critical(ex);

				if (!trapException)
					throw;
			}

			return 0;
		}
		public static int ExecuteNonQuery(string sql, ObjectsDataContext db, bool trapException)
		{
			if (!string.IsNullOrEmpty(sql) && db != null)
			{
				try
				{
					using (var sqlConnection = new SqlConnection(db.Connection.ConnectionString))
					{
						skky.util.Trace.Information(sql);
						sqlConnection.Open();
						using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
						{
							sqlCommand.CommandTimeout = Const_DefaultCommandTimeout;
							return sqlCommand.ExecuteNonQuery();
						}
					}
				}
				catch (Exception ex)
				{
					skky.util.Trace.Critical(sql);
					skky.util.Trace.Critical(ex);

					if(!trapException)
						throw;
				}
			}

			return 0;
		}

		public static string GetAccountNumberString(string accountNumber)
		{
			if (string.IsNullOrEmpty(accountNumber))
				accountNumber = string.Empty;
			if (accountNumber == "All")
				accountNumber = string.Empty;

			if (string.IsNullOrEmpty(accountNumber))
				return Const_Null;

			return accountNumber.WrapInSingleQuotes();
		}

		public static string GetSQLDateTimeString(DateTime? dateTime)
		{
			if (dateTime == null)
				return Const_Null;

			DateTime dt = (DateTime)dateTime;

			string sql = dt.ToShortDateString() + " " + dt.ToLongTimeString();
			return sql.WrapInSingleQuotes();
		}
		public static string GetSQLDateRange(DateSettings dateRange)
		{
			if (dateRange == null)
				return ", NULL, NULL";

			string sql = ", " + (dateRange.HasStartDate() ? GetSQLDateTimeString(dateRange.GetStartDateTime()) : Const_Null);
			sql += ", " + (dateRange.HasEndDate() ? GetSQLDateTimeString(dateRange.GetEndDateTime()) : Const_Null);

			return sql;
		}
		public static string QueryWithCity(string storedProc, string accountNumber, int departmentId, City city, DateSettings dateRange)
		{
			string sql = "dbo." + storedProc + " " + GetAccountNumberString(accountNumber) + ", " + departmentId.ToString() + ", ";
			if (city == null)
				sql += Const_Null;
			else
				sql += city.Code.WrapInSingleQuotes();

			sql += GetSQLDateRange(dateRange);

			return sql;
		}
		public static string QueryWithCity(string storedProc, SkkyCallParams settings, City city)
		{
			string sql = "dbo." + storedProc + " " + ((int)settings.GetEmissionType()).ToString()
				+ ", " + GetAccountNumberString(settings.AccountNumber)
				+ ", " + settings.DepartmentId.ToString() + ", ";
			if (city == null)
				sql += Const_Null;
			else
				sql += city.Code.WrapInSingleQuotes();

			sql += GetSQLDateRange(settings.DateRange);

			return sql;
		}

		public static string QueryWithCities(string storedProc, string accountNumber, int departmentId, City startCity, City endCity, DateSettings dateRange)
		{
			string sql = "dbo." + storedProc + " " + GetAccountNumberString(accountNumber) + ", " + departmentId.ToString();
			sql += ", ";
			if (startCity == null)
				sql += Const_Null;
			else
				sql += startCity.Code.WrapInSingleQuotes();
			sql += ", ";
			if (endCity == null)
				sql += Const_Null;
			else
				sql += endCity.Code.WrapInSingleQuotes();

			sql += GetSQLDateRange(dateRange);

			return sql;
		}
		public static string QueryWithCities(string storedProc, SkkyCallParams settings, City startCity, City endCity)
		{
			string sql = "dbo." + storedProc + " " + ((int)settings.GetEmissionType()).ToString()
				+ ", " + GetAccountNumberString(settings.AccountNumber)
				+ ", " + settings.DepartmentId.ToString();
			sql += ", ";
			if (startCity == null)
				sql += Const_Null;
			else
				sql += startCity.Code.WrapInSingleQuotes();
			sql += ", ";
			if (endCity == null)
				sql += Const_Null;
			else
				sql += endCity.Code.WrapInSingleQuotes();

			sql += GetSQLDateRange(settings.DateRange);

			return sql;
		}
		public static string QueryByVendor(string storedProc, SkkyCallParams settings, City city, string vendor)
		{
			string sql = "dbo." + storedProc
				+ " " + GetAccountNumberString(settings.AccountNumber)
				+ ", " + settings.DepartmentId.ToString() + ", ";
			if (city == null)
				sql += Const_Null;
			else
				sql += city.Code.WrapInSingleQuotes();

			sql += GetSQLDateRange(settings.DateRange);

			sql += ", ";
			if (vendor == null)
				sql += Const_Null;
			else
				sql += vendor.WrapInSingleQuotes();

			return sql;
		}
		public static string QueryByZipCode(string storedProc, SkkyCallParams settings, string zipCode)
		{
			string sql = "dbo." + storedProc
				+ " " + GetAccountNumberString(settings.AccountNumber)
				+ ", " + settings.DepartmentId.ToString();

			sql += GetSQLDateRange(settings.DateRange);

			sql += ", ";
			if (zipCode == null)
				sql += Const_Null;
			else
				sql += zipCode.WrapInSingleQuotes();

			return sql;
		}
		public static string QueryEquipmentByEmissionsSettings(string storedProc, SkkyCallParams settings, City city, EquipmentType equipmentType)
		{
			string sql = "dbo." + storedProc
				+ " " + GetAccountNumberString(settings.AccountNumber)
				+ ", " + settings.DepartmentId.ToString() + ", ";
			if (city == null)
				sql += Const_Null;
			else
				sql += city.Code.WrapInSingleQuotes();

			sql += GetSQLDateRange(settings.DateRange);

			sql += ", ";
			if (equipmentType == null)
				sql += Const_Null;
			else
				sql += equipmentType.Code.WrapInSingleQuotes();

			return sql;
		}
		public static string QueryWithEquipment(string storedProc, string accountNumber, int departmentId, EquipmentType equip, DateSettings dateRange)
		{
			string sql = "dbo." + storedProc + " " + GetAccountNumberString(accountNumber) + ", " + departmentId.ToString() + ", ";
			if (equip == null)
				sql += Const_Null;
			else
				sql += equip.Code.WrapInSingleQuotes();

			sql += GetSQLDateRange(dateRange);

			return sql;
		}

		public static IEnumerable<StringInt> GetStringInts(string sql, int stringField, int intField)
		{
			List<KeyValuePair<int, System.Type>> kvpList = new List<KeyValuePair<int, System.Type>>();
			kvpList.Add(new KeyValuePair<int, System.Type>(stringField, typeof(string)));
			kvpList.Add(new KeyValuePair<int, System.Type>(intField, typeof(int)));

			var list = GetDataRows(sql, kvpList);
			return from i in list
				   select new StringInt()
				   {
					   stringValue = i.stringValue,
					   intValue = i.intValueOrDefault
				   };
		}
		public static IEnumerable<StringDouble> GetStringDoubles(string sql, int stringField, int doubleField)
		{
			List<KeyValuePair<int, System.Type>> kvpList = new List<KeyValuePair<int, System.Type>>();
			kvpList.Add(new KeyValuePair<int, System.Type>(stringField, typeof(string)));
			kvpList.Add(new KeyValuePair<int, System.Type>(doubleField, typeof(double)));

			var list = GetDataRows(sql, kvpList);
			return from i in list
				   select new StringDouble()
				   {
					   stringValue = i.stringValue,
					   doubleValue = i.doubleValueOrDefault
				   };
		}
		public static IEnumerable<StringIntDoubleDateTime> GetStringIntDoubleDateTimes(string sql, int stringField, int intField, int doubleField, int dateTimeField)
		{
			List<KeyValuePair<int, System.Type>> kvpList = new List<KeyValuePair<int, System.Type>>();
			kvpList.Add(new KeyValuePair<int, System.Type>(stringField, typeof(string)));
			kvpList.Add(new KeyValuePair<int, System.Type>(intField, typeof(int)));
			kvpList.Add(new KeyValuePair<int, System.Type>(doubleField, typeof(double)));
			kvpList.Add(new KeyValuePair<int, System.Type>(dateTimeField, typeof(DateTime)));

			var list = GetDataRows(sql, kvpList);
			return from i in list
				   select new StringIntDoubleDateTime()
				   {
					   stringValue = i.stringValue,
					   doubleValue = i.doubleValueOrDefault,
					   intValue = i.intValueOrDefault,
					   dateTimeValue = i.dateTimeValueOrDefault,
				   };
		}

		public static List<PropertyManager> GetStringIntList(string sql, int stringField, int intField)
		{
			List<KeyValuePair<int, System.Type>> kvpList = new List<KeyValuePair<int, System.Type>>();
			kvpList.Add(new KeyValuePair<int, System.Type>(stringField, typeof(string)));
			kvpList.Add(new KeyValuePair<int, System.Type>(intField, typeof(int)));

			return GetDataRows(sql, kvpList);
		}
		public static List<PropertyManager> GetStringDoubleList(string sql, int stringField, int intField)
		{
			List<KeyValuePair<int, System.Type>> kvpList = new List<KeyValuePair<int, System.Type>>();
			kvpList.Add(new KeyValuePair<int, System.Type>(stringField, typeof(string)));
			kvpList.Add(new KeyValuePair<int, System.Type>(intField, typeof(int)));

			return GetDataRows(sql, kvpList);
		}
		public static List<PropertyManager> GetStringIntDoubleList(string sql, int stringField, int intField, int doubleField)
		{
			List<KeyValuePair<int, System.Type>> kvpList = new List<KeyValuePair<int, System.Type>>();
			kvpList.Add(new KeyValuePair<int, System.Type>(stringField, typeof(string)));
			kvpList.Add(new KeyValuePair<int, System.Type>(intField, typeof(int)));
			kvpList.Add(new KeyValuePair<int, System.Type>(doubleField, typeof(double)));

			return GetDataRows(sql, kvpList);
		}

		public static List<PropertyManager> GetStringStringIntDoubleList(string sql, int stringField, int string2Field, int intField, int doubleField)
		{
			List<KeyValuePair<int, System.Type>> kvpList = new List<KeyValuePair<int, System.Type>>();
			kvpList.Add(new KeyValuePair<int, System.Type>(stringField, typeof(string)));
			kvpList.Add(new KeyValuePair<int, System.Type>(string2Field, typeof(string)));
			kvpList.Add(new KeyValuePair<int, System.Type>(intField, typeof(int)));
			kvpList.Add(new KeyValuePair<int, System.Type>(doubleField, typeof(double)));

			return GetDataRows(sql, kvpList);
		}

		public static List<PropertyManager> GetIntStringIntDoubleList(string sql, int intField, int stringField, int int2Field, int doubleField)
		{
			List<KeyValuePair<int, System.Type>> kvpList = new List<KeyValuePair<int, System.Type>>();
			kvpList.Add(new KeyValuePair<int, System.Type>(intField, typeof(int)));
			kvpList.Add(new KeyValuePair<int, System.Type>(stringField, typeof(string)));
			kvpList.Add(new KeyValuePair<int, System.Type>(int2Field, typeof(int)));
			kvpList.Add(new KeyValuePair<int, System.Type>(doubleField, typeof(double)));

			return GetDataRows(sql, kvpList);
		}

		public static List<PropertyManager> GetDateIntDoubleList(string sql, int dateField, int intField, int doubleField)
		{
			List<KeyValuePair<int, System.Type>> kvpList = new List<KeyValuePair<int, System.Type>>();
			kvpList.Add(new KeyValuePair<int, System.Type>(dateField, typeof(DateTime)));
			kvpList.Add(new KeyValuePair<int, System.Type>(intField, typeof(int)));
			kvpList.Add(new KeyValuePair<int, System.Type>(doubleField, typeof(double)));

			return GetDataRows(sql, kvpList);
		}

		/// <summary>
		/// Calls GetDataRows by ordering the PropertyTypes sequentually.
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="props"></param>
		/// <returns></returns>
		public static List<PropertyManager> GetDataRows(string sql, IEnumerable<System.Type> props)
		{
			if (!string.IsNullOrEmpty(sql) && props != null && props.Count() > 0)
			{
				List<KeyValuePair<int, System.Type>> kvpList = new List<KeyValuePair<int, System.Type>>();
				for (int i = 0; i < props.Count(); ++i)
				{
					System.Type p = props.ElementAt(i);
					kvpList.Add(new KeyValuePair<int, System.Type>(i, p));
				}

				return GetDataRows(sql, kvpList);
			}

			return new List<PropertyManager>();
		}

		/// <summary>
		/// Builds a List of DataRows dynamically based on the property types and column offsets passed in.
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="props"></param>
		/// <returns></returns>
		/// 
		public static List<PropertyManager> GetDataRows(string sql, List<KeyValuePair<int, System.Type>> props)
		{
			return GetDataRows(ObjectsDataContextDelegate, sql, props);
		}
		public static List<PropertyManager> GetDataRows(DataContextDelegate dcd, string sql, List<KeyValuePair<int, System.Type>> props)
		{
			List<PropertyManager> sidList = new List<PropertyManager>();

			if (!string.IsNullOrEmpty(sql))
			{
				using (var db = dcd())
				{
					using (var sqlConnection = new SqlConnection(db.Connection.ConnectionString))
					{
						skky.util.Trace.Information(sql);

						sqlConnection.Open();
						using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
						{
							sqlCommand.CommandTimeout = Const_DefaultCommandTimeout;
							using (SqlDataReader reader = sqlCommand.ExecuteReader())
							{
								if (reader.HasRows)
								{
									while (reader.Read())
									{
										PropertyManager sid = new PropertyManager();

										foreach (var kvp in props)
										{
											int columnOffset = kvp.Key;
											System.Type p = kvp.Value;

											if (Property.IsDateTimeType(p))
											{
												SqlDateTime sdt = reader.GetSqlDateTime(columnOffset);
												if (sdt.IsNull)
													sid.dateTimeValue = null;
												else
													sid.dateTimeValue = sdt.Value;
											}
											else if (Property.IsDoubleType(p))
											{
												SqlDouble sd = reader.GetSqlDouble(columnOffset);
												if (sd.IsNull)
													sid.doubleValue = null;
												else
													sid.doubleValue = sd.Value;
											}
											else if (Property.IsLongType(p))
											{
												SqlInt64 si = reader.GetSqlInt64(columnOffset);
												if (si.IsNull)
													sid.longValue = null;
												else
													sid.longValue = si.Value;
											}
											else if (Property.IsIntType(p))
											{
												SqlInt32 si = reader.GetSqlInt32(columnOffset);
												if (si.IsNull)
													sid.intValue = null;
												else
													sid.intValue = si.Value;
											}
											else if (Property.IsStringType(p))
											{
												SqlString ss = reader.GetSqlString(columnOffset);
												if (ss.IsNull)
													sid.stringValue = null;
												else
													sid.stringValue = ss.Value;
											}
											else if (Property.IsGuidType(p))
											{
												SqlGuid sg = reader.GetSqlGuid(columnOffset);
												if (sg.IsNull)
													sid.guidValue = null;
												else
													sid.guidValue = sg.Value;
											}
										}

										sidList.Add(sid);
									}
								}
							}
						}
					}
				}
			}

			return sidList;
		}
	}
}
