using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Types;
using skky.util;

namespace skky.db
{
	public partial class TotalHotelEmission
	{
		public static IEnumerable<StringInt> GetCitiesWithData(SkkyCallParams emissionsSettings)
		{
			if (emissionsSettings == null)
				throw new Exception("TotalHotelEmissions.GetCitiesWithData: NULL EmissionsSettings passed in.");

			List<StringInt> listStringInt = new List<StringInt>();
			string sql = DirectAccess.QueryWithCity("GetHotelEmissionsCitiesWithData", emissionsSettings, null);
			System.Type[] props =
			{
				typeof(int),
				typeof(string),
				typeof(string),
			};

			var drlist = DirectAccess.GetDataRows(sql, props);
			if (drlist != null)
			{
				foreach (var item in drlist)
				{
					StringInt si = new StringInt(item.stringValue, item.intValueOrDefault);
					listStringInt.Add(si);
				}
			}

			return listStringInt;
		}
		/*
		public static IEnumerable<StringInt> GetCitiesWithData()
		{
			using (var db = new ObjectsDataContext())
			{
				var list = from cty in db.Cities
						   where (from emissions in db.TotalHotelEmissions
								  where emissions.CityCode == cty.Code
								  select emissions.CityCode).Contains(cty.Code)
						   orderby cty.Name
						   select new StringInt()
						   {
							   intValue = cty.id,
							   stringValue = cty.FullName
						   };

				return list.ToList();
			}
		}
		*/
		public static List<PropertyManager> GetCO2ReportByVendor(string vendor)
		{
		    using (var db = new ObjectsDataContext())
		    {
		        var list = (from emissions in db.TotalHotelEmissions
		                    where emissions.TotalCO2 > 0
		                    where emissions.VendorName == vendor
							orderby emissions.StartDateTime descending
							select new PropertyManager
		                    {
		                        stringValue = emissions.VendorName,
		                        intValue = emissions.RoomNights ?? 0,
		                        doubleValue = emissions.TotalCO2 ?? 0.0,
								dateTimeValue = emissions.StartDateTime,
		                    });

				return list.ToList();
		    }
		}

		/// <summary>
		/// Queries every start city and summarizes its emissions based on the settings passed in.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="startCity"></param>
		/// <returns>A collection of information detailing the emissions for each city pair.</returns>
		public static IEnumerable<StringIntDoubleDateTime> GetDetailByDepartment(SkkyCallParams settings, int departmentId)
		{
			string sql = DirectAccess.QueryWithCity("GetHotelEmissions", settings.AccountNumber, departmentId, null, settings.DateRange);
			int emissionsField = 21 + settings.GetEmissionsFieldOffset();
			return DirectAccess.GetStringIntDoubleDateTimes(sql, 8, 20, emissionsField, 6);
		}
		public static DataRowManager GetSummaryByDepartment(string accountNumber, City city, DateSettings dateRange, int emissionField)
		{
			string sql = "dbo.GetHotelEmissionsSummaryByDepartment " + (string.IsNullOrEmpty(accountNumber) ? "NULL" : accountNumber.WrapInSingleQuotes()) + ", ";
			if (city == null)
				sql += "NULL";
			else
				sql += city.Code.WrapInSingleQuotes();

			sql += DirectAccess.GetSQLDateRange(dateRange);

			var list = DirectAccess.GetIntStringIntDoubleList(sql, 0, 1, 6, emissionField);

			return new DataRowManager(list);
		}
		public static DataRowManager EmissionsOverTime(string accountNumber, int departmentId, City city, DateSettings dateRange, int emissionField)
		{
			string sql = DirectAccess.QueryWithCity("HotelCO2OverTime", accountNumber, departmentId, city, dateRange);
			var list = DirectAccess.GetDateIntDoubleList(sql, 0, 5, emissionField);

			return new DataRowManager(list);
		}

		/// <summary>
		/// Queries every start city and summarizes its emissions based on the settings passed in.
		/// </summary>
		/// <param name="settings"></param>
		/// <returns>A collection summarizing all cities and their requested emissions data.</returns>
		public static IEnumerable<StringDouble> GetSummaryByCity(SkkyCallParams settings, City cty)
		{
			string sql = DirectAccess.QueryWithCity("GetHotelEmissionsSummaryByCityCode", settings.AccountNumber, settings.DepartmentId, cty, settings.DateRange);
			int emissionsField = 2 + (int)settings.GetEmissionType();
			return DirectAccess.GetStringDoubles(sql, 1, emissionsField);
		}

		/// <summary>
		/// Queries every start city and summarizes its emissions based on the settings passed in.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="startCity"></param>
		/// <returns>A collection of information detailing the emissions for each city pair.</returns>
		public static IEnumerable<StringIntDoubleDateTime> GetDetailForCity(SkkyCallParams settings, string cityCode)
		{
			string sql = DirectAccess.QueryWithCity("GetHotelEmissions", settings.AccountNumber, settings.DepartmentId, skky.db.City.GetCity(cityCode), settings.DateRange);
			int emissionsField = 21 + settings.GetEmissionsFieldOffset();
			return DirectAccess.GetStringIntDoubleDateTimes(sql, 8, 20, emissionsField, 6);
		}
		/// <summary>
		/// Queries every start city and summarizes its emissions based on the settings passed in.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="startCity"></param>
		/// <returns>A collection of information detailing the emissions for each city pair.</returns>
		public static IEnumerable<StringIntDoubleDateTime> GetDetailByZipCode(SkkyCallParams settings, string zipCode)
		{
			string sql = DirectAccess.QueryByZipCode("GetHotelEmissionsDetailByZipCode", settings, zipCode);
			int emissionsField = 4 + settings.GetEmissionsFieldOffset();
			return DirectAccess.GetStringIntDoubleDateTimes(sql, 0, 8, emissionsField, 3);
		}

		/// <summary>
		/// Queries every start city and summarizes its emissions based on the settings passed in.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="startCity"></param>
		/// <returns>A collection of information detailing the emissions for each city pair.</returns>
		public static IEnumerable<StringIntDoubleDateTime> GetDetailByVendor(SkkyCallParams settings, string cityCode, string vendor)
		{
			string sql = DirectAccess.QueryByVendor("GetHotelEmissionsDetailByVendor", settings, skky.db.City.FromCode(cityCode), vendor);
			int emissionsField = 4 + settings.GetEmissionsFieldOffset();
			return DirectAccess.GetStringIntDoubleDateTimes(sql, 0, 8, emissionsField, 3);
		}
		public static IEnumerable<StringIntDoubleDateTime> GetDetailByProperty(SkkyCallParams settings, string cityCode, string propertyName)
		{
			string sql = DirectAccess.QueryByVendor("GetHotelEmissionsDetailByProperty", settings, skky.db.City.FromCode(cityCode), propertyName);
			int emissionsField = 4 + settings.GetEmissionsFieldOffset();
			return DirectAccess.GetStringIntDoubleDateTimes(sql, 0, 9, emissionsField, 3);
		}

		public static string GetSummaryByCity(string accountNumber, int departmentId, City city, DateSettings dateRange)
		{
			return DirectAccess.QueryWithCity("GetHotelEmissionsSummaryByCityCode", accountNumber, departmentId, city, dateRange);
		}
		public static string GetSummaryByProperty(string accountNumber, int departmentId, City city, DateSettings dateRange, string propertyName)
		{
			string sql = DirectAccess.QueryWithCity("GetHotelEmissionsSummaryByProperty", accountNumber, departmentId, city, dateRange);
			if (propertyName == null)
				sql += ", NULL";
			else
				sql += "'" + propertyName + "'";

			return sql;
		}
		public static string GetSummaryByVendor(string accountNumber, int departmentId, City city, DateSettings dateRange)
		{
			return DirectAccess.QueryWithCity("GetHotelEmissionsSummaryByVendor", accountNumber, departmentId, city, dateRange);
		}
		public static string GetSummaryByZipCode(string accountNumber, int departmentId, DateSettings dateRange)
		{
			string sql = "dbo.GetHotelEmissionsSummaryByZipCode " + DirectAccess.GetAccountNumberString(accountNumber) + ", " + departmentId.ToString();
			sql += DirectAccess.GetSQLDateRange(dateRange);

			return sql;
		}
		public static string GetSummaryInZipCode(string accountNumber, int departmentId, string zipCode, DateSettings dateRange)
		{
			string sql = "dbo.GetHotelEmissionsSummaryInZipCode " + DirectAccess.GetAccountNumberString(accountNumber) + ", " + departmentId.ToString();
			sql += ", '" + (zipCode ?? string.Empty) + "'";
			sql += DirectAccess.GetSQLDateRange(dateRange);

			return sql;
		}

		public static DataRowManager GetForCities(string accountNumber, int departmentId, City city, DateSettings dateRange, int emissionsField)
		{
			string sql = GetSummaryByCity(accountNumber, departmentId, city, dateRange);
			var list = DirectAccess.GetStringStringIntDoubleList(sql, 0, 1, 6, emissionsField);

			return new DataRowManager(list);
		}

		public static DataRowManager GetByVendor(string accountNumber, int departmentId, City city, DateSettings dateRange, int emissionsField)
		{
			string sql = GetSummaryByVendor(accountNumber, departmentId, city, dateRange);
			var list = DirectAccess.GetStringIntDoubleList(sql, 0, 5, emissionsField);

			return new DataRowManager(list);
		}

		public static DataRowManager GetByZipCodes(string accountNumber, int departmentId, DateSettings dateRange, int emissionsField)
		{
			string sql = GetSummaryByZipCode(accountNumber, departmentId, dateRange);
			var list = DirectAccess.GetStringIntDoubleList(sql, 0, 5, emissionsField);

			return new DataRowManager(list);
		}
		public static DataRowManager GetInZipCode(string accountNumber, int departmentId, string zipCode, DateSettings dateRange, int emissionsField)
		{
			string sql = GetSummaryInZipCode(accountNumber, departmentId, zipCode, dateRange);
			var list = DirectAccess.GetStringIntDoubleList(sql, 0, 5, emissionsField);

			return new DataRowManager(list);
		}

		public static List<EmissionMarker> GetEmissionMarkersByCity(string accountNumber, int departmentId, DateSettings dateRange)
		{
			string sql = GetSummaryByCity(accountNumber, departmentId, null, dateRange);
			System.Type[] props =
			{
				typeof(string),
				typeof(string),
				typeof(double),
				typeof(double),
				typeof(double),
				typeof(double),
				typeof(int),
				typeof(double),
				typeof(double),
			};
			var list = DirectAccess.GetDataRows(sql, props);
			var markerList = from a in list
							 //orderby a.GetProperty(3) descending
							 select new EmissionMarker
							 {
								 Code = a.GetProperty(0).stringValue,
								 Name = a.GetProperty(1).stringValue,
								 CO2 = a.GetProperty(2).doubleValueOrDefault,
								 CH4 = a.GetProperty(3).doubleValueOrDefault,
								 H2O = a.GetProperty(4).doubleValueOrDefault,
								 NOx = a.GetProperty(5).doubleValueOrDefault,
								 Distance = a.GetProperty(6).intValueOrDefault,
								 lat = a.GetProperty(7).doubleValueOrDefault,
								 lng = a.GetProperty(8).doubleValueOrDefault,
							 };

			return markerList.ToList();
		}

		public static List<EmissionMarker> GetEmissionMarkersByPropertyOrig(string accountNumber, int departmentId)
		{
			//string sql = DirectAccess.QueryByVendor("GetHotelEmissionsDetailByPropertyWithMarkers", settings, skky.db.City.FromCode(cityCode), propertyName);
			using (var db = new ObjectsDataContext())
			{
				var list = (from emissions in db.TotalHotelEmissions
							group emissions by emissions.VendorName into g
							let totalCO2 = g.Sum(x => x.TotalCO2)
							let totalCH4 = g.Sum(x => x.TotalCH4)
							let totalNOx = g.Sum(x => x.TotalNOx)
							let totalH2O = g.Sum(x => x.TotalH2O)
							let cityCode = g.Key
							orderby totalCO2 descending
							select new EmissionMarker
							{
								Name = cityCode,
								Code = g.First().CityCode,
								CO2 = totalCO2 ?? 0.0,
								CH4 = totalCH4 ?? 0.0,
								H2O = totalH2O ?? 0.0,
								NOx = totalNOx ?? 0.0,
							});

				return DcsWrapper.GetNonNullList<EmissionMarker>(list);
			}
		}

		public static List<EmissionMarker> GetEmissionMarkersByProperty(SkkyCallParams settings, string cityCode, string propertyName)
		{
			string sql = DirectAccess.QueryByVendor("GetHotelEmissionsSummaryByPropertyWithMarkers", settings, skky.db.City.FromCode(cityCode), propertyName);
			System.Type[] props =
			{
				typeof(string),		// VendorName (the hotel's full name)
				typeof(string),		// Code (the IANA city code)
				typeof(string),		// FullName (the city's full name)
				typeof(double),		// lat (Latitude)
				typeof(double),		// lng (Longitude)
				typeof(double),		// TotalCO2
				typeof(double),		// TotalCH4
				typeof(double),		// TotalH2O
				typeof(double),		// TotalNOx
				typeof(double),		// TotalSOx
				typeof(int),		// Room Nights
			};
			var list = DirectAccess.GetDataRows(sql, props);
			var markerList = from a in list
							 //orderby a.GetProperty(3) descending
							 select new EmissionMarker
							 {
								 Name = a.GetProperty(0).stringValue,
								 Code = a.GetProperty(1).stringValue,
								 lat = a.GetProperty(3).doubleValueOrDefault,
								 lng = a.GetProperty(4).doubleValueOrDefault,
								 CO2 = a.GetProperty(5).doubleValueOrDefault,
								 CH4 = a.GetProperty(6).doubleValueOrDefault,
								 H2O = a.GetProperty(7).doubleValueOrDefault,
								 NOx = a.GetProperty(8).doubleValueOrDefault,
								 SOx = a.GetProperty(9).doubleValueOrDefault,
								 Distance = a.GetProperty(10).intValueOrDefault,
							 };

			return markerList.ToList();
		}
	}
}
