using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Types;
using skky.util;

namespace skky.db
{
	public partial class TotalFlightEmission
	{
		public static IEnumerable<StringInt> GetCitiesWithData(SkkyCallParams emissionsSettings)
		{
			if (emissionsSettings == null)
				throw new Exception("TotalFlightEmissions.GetCitiesWithData: NULL EmissionsSettings passed in.");

			List<StringInt> listStringInt = new List<StringInt>();
			string sql = DirectAccess.QueryWithCity("GetFlightEmissionsCitiesWithData", emissionsSettings, null);
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
						   where (from emissions in db.TotalFlightEmissions
								  where emissions.StartCity == cty.Code
								  select emissions.StartCity).Contains(cty.Code)
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
		public static List<StringIntDouble> GetCO2ReportForCity()
		{
			City city = City.FromCode("LAX");
			using (var db = new ObjectsDataContext())
			{
				var list = from emissions in db.TotalFlightEmissions
						   //join equiptype in db.EquipmentTypes on emissions.EquipmentType equals equiptype.Code
						   where emissions.StartCity == city.Code || emissions.EndCity == city.Code
						   where emissions.TotalCO2 > 0
						   //where emissions.EquipmentType == equipment.Code
						   orderby emissions.TotalCO2 descending
						   select new StringIntDouble
						   {
							   doubleValue = emissions.TotalCO2 ?? 0.0,
							   intValue = emissions.Miles,
							   stringValue = emissions.StartCity
						   };

				if (list.Count() > 0)
					return list.ToList();
			}

			return new List<StringIntDouble>();
		}
		public static DataRowManager GetEmissionTypeForCitiesByPlaneType(string accountNumber, int departmentId, EquipmentType equipment, DateSettings dateRange, int emissionsField)
		{
			string sql = "dbo.GetFlightEmissionsCitySummaryByEquipment " + DirectAccess.GetAccountNumberString(accountNumber) + ", " + departmentId.ToString() + ", ";
			if (equipment == null)
				sql += "NULL";
			else
				sql += "'" + equipment.Code + "'";

			sql += DirectAccess.GetSQLDateRange(dateRange);

			var list = DirectAccess.GetStringStringIntDoubleList(sql, 0, 1, 6, emissionsField);
			return new DataRowManager(list);
		}

		public static DataRowManager EmissionsOverTime(string accountNumber, int departmentId, City city, DateSettings dateRange, int emissionField)
		{
			string sql = DirectAccess.QueryWithCity("AirCO2OverTime", accountNumber, departmentId, city, dateRange);
			var list = DirectAccess.GetDateIntDoubleList(sql, 0, 5, emissionField);

			return new DataRowManager(list);
		}
		public static DataRowManager EmissionsOverTimeAll(string accountNumber, int departmentId, City city, DateSettings dateRange)
		{
			string sql = DirectAccess.QueryWithCity("AirCO2OverTime", accountNumber, departmentId, city, dateRange);
			System.Type[] props =
			{
				typeof(DateTime),
				typeof(double),
				typeof(double),
				typeof(double),
				typeof(double),
				typeof(int),
			};

			var list = DirectAccess.GetDataRows(sql, props);
			return new DataRowManager(list);
		}
		public static DataRowManager GetSummaryByDepartment(string accountNumber, City startCity, City endCity, DateSettings dateRange, int emissionField)
		{
			string sql = "dbo.GetFlightEmissionsSummaryByDepartment " + (string.IsNullOrEmpty(accountNumber) ? "NULL" : accountNumber.WrapInSingleQuotes()) + ", ";
			if (startCity == null)
				sql += "NULL";
			else
				sql += startCity.Code.WrapInSingleQuotes();

			sql += ", ";
			if (endCity == null)
				sql += "NULL";
			else
				sql += endCity.Code.WrapInSingleQuotes();

			sql += DirectAccess.GetSQLDateRange(dateRange);

			var list = DirectAccess.GetIntStringIntDoubleList(sql, 0, 1, 6, emissionField);

			return new DataRowManager(list);
		}
		public static DataRowManager AirCO2OverTime2(int accountId, int departmentId, City city)
		{
			DataRowManager ds = null;
			using (var db = new ObjectsDataContext())
			{
				if (city == null)
				{
					var list = from emissions in db.TotalFlightEmissions
							   where emissions.TotalCO2 > 0
							   group emissions by new DateTime(emissions.StartDateTime.Year, emissions.StartDateTime.Month, emissions.StartDateTime.Day) into g
							   let totalEmissions = g.Sum(x => x.TotalCO2)
							   let totalMiles = g.Sum(y => y.Miles)
							   orderby g.Key descending
							   select new PropertyManager
							   {
								   intValue = totalMiles,
								   doubleValue = totalEmissions ?? 0.0,
								   dateTimeValue = g.Key,
							   };

					ds = new DataRowManager(list);
				}
				else
				{
					var list = from emissions in db.TotalFlightEmissions
							   where emissions.TotalCO2 > 0
							   where ((emissions.StartCity == city.Code) || (emissions.EndCity == city.Code))
							   group emissions by new DateTime(emissions.StartDateTime.Year, emissions.StartDateTime.Month, emissions.StartDateTime.Day) into g
							   let totalEmissions = g.Sum(x => x.TotalCO2)
							   let totalMiles = g.Sum(y => y.Miles)
							   orderby g.Key descending
							   select new PropertyManager
							   {
								   intValue = totalMiles,
								   doubleValue = totalEmissions ?? 0.0,
								   dateTimeValue = g.Key,
							   };

					ds = new DataRowManager(list);
				}
			}

			if (ds == null)
				ds = new DataRowManager();

			return ds;
		}

		public static string GetSummaryByEquipment(string accountNumber, int departmentId, City city, DateSettings dateRange)
		{
			return DirectAccess.QueryWithCity("GetFlightEmissionsSummaryByEquipment", accountNumber, departmentId, city, dateRange);
		}
		public static DataRowManager GetEmissionTypeByPlaneType(string accountNumber, int departmentId, City city, DateSettings dateRange, int emissionsField)
		{
			string sql = GetSummaryByEquipment(accountNumber, departmentId, city, dateRange);
			var list = DirectAccess.GetStringStringIntDoubleList(sql, 0, 1, 6, emissionsField);

			return new DataRowManager(list);
		}

		/// <summary>
		/// Returns the emissions from every city from the give equipment type.
		/// </summary>
		/// <param name="accountNumber"></param>
		/// <param name="departmentId"></param>
		/// <param name="equip"></param>
		/// <returns></returns>
		public static List<EmissionMarker> GetCitySummaryByEquipment(string accountNumber, int departmentId, EquipmentType equip, DateSettings dateRange)
		{
			string sql = DirectAccess.QueryWithEquipment("GetFlightEmissionsCitySummaryByEquipment", accountNumber, departmentId, equip, dateRange);
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

			if (markerList.Count() > 0)
				return markerList.ToList();

			return new List<EmissionMarker>();
		}

		/// <summary>
		/// Queries start city emissions for a specific EquipmentType.
		/// If no startCity is passed, it returns all data for every city.
		/// </summary>
		/// <param name="settings">The user settings that scale the parameters for this report.</param>
		/// <param name="startCity">The startCity to report on. If none specified, return data for all cities.</param>
		/// <param name="equipmentType">The desired equipment type to report on.</param>
		/// <returns>A collection of information detailing the emissions for each city pair.</returns>
		public static IEnumerable<StringIntDoubleDateTime> GetDetailByEquipment(SkkyCallParams settings, City cityCode, EquipmentType equipmentType)
		{
			string sql = DirectAccess.QueryEquipmentByEmissionsSettings("GetFlightEmissionsDetailByEquipment", settings, cityCode, equipmentType);
			int emissionsField = 5 + settings.GetEmissionsFieldOffset();
			return DirectAccess.GetStringIntDoubleDateTimes(sql, 3, 9, emissionsField, 4);
		}

		/// <summary>
		/// Queries every start city and summarizes its emissions based on the settings passed in.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="startCity"></param>
		/// <returns>A collection of information detailing the emissions for each city pair.</returns>
		public static IEnumerable<StringIntDoubleDateTime> GetDetailByDepartment(SkkyCallParams settings, int departmentId)
		{
			string sql = DirectAccess.QueryWithCity("GetFlightEmissions", settings.AccountNumber, departmentId, null, settings.DateRange);
			int emissionsField = 11 + settings.GetEmissionsFieldOffset();
			return DirectAccess.GetStringIntDoubleDateTimes(sql, 9, 8, emissionsField, 5);
		}

		/// <summary>
		/// Queries every start city and summarizes its emissions based on the settings passed in.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="startCity"></param>
		/// <returns>A collection of information detailing the emissions for each city pair.</returns>
		public static IEnumerable<StringIntDoubleDateTime> GetDetailForStartCity(SkkyCallParams settings, string startCity)
		{
			return GetDetailForStartCity(settings, City.GetCity(startCity));
		}
		/// <summary>
		/// Queries every start city and summarizes its emissions based on the settings passed in.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="startCity"></param>
		/// <returns>A collection of information detailing the emissions for each city pair.</returns>
		public static IEnumerable<StringIntDoubleDateTime> GetDetailForStartCity(SkkyCallParams settings, City startCity)
		{
			string sql = DirectAccess.QueryWithCity("GetFlightEmissions", settings.AccountNumber, settings.DepartmentId, startCity, settings.DateRange);
			int emissionsField = 11 + settings.GetEmissionsFieldOffset();
			return DirectAccess.GetStringIntDoubleDateTimes(sql, 10, 8, emissionsField, 5);
		}

		/// <summary>
		/// Queries every city and summarizes its emissions by airline based on the settings passed in.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="city"></param>
		/// <returns>A collection of information detailing the emissions for each city pair.</returns>
		public static IEnumerable<StringIntDoubleDateTime> GetDetailByAirline(SkkyCallParams settings, string city, string sCode)
		{
			return GetDetailByAirline(settings, City.FromCode(city), sCode);
		}
		/// <summary>
		/// Queries every city and summarizes its emissions by airline based on the settings passed in.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="city"></param>
		/// <returns>A collection of information detailing the emissions for each city pair.</returns>
		public static IEnumerable<StringIntDoubleDateTime> GetDetailByAirline(SkkyCallParams settings, City city, string sCode)
		{
			string sql = DirectAccess.QueryWithCities("GetFlightEmissionsDetailByAirline", settings.AccountNumber, settings.DepartmentId, city, city, settings.DateRange);
			if (!string.IsNullOrEmpty(sCode))
				sql += ", " + sCode.WrapInSingleQuotes();

			int emissionsField = 5 + settings.GetEmissionsFieldOffset();
			return DirectAccess.GetStringIntDoubleDateTimes(sql, 3, 10, emissionsField, 4);
		}

		public static string GetSummaryByAirline(string accountNumber, int departmentId, City startCity, City endCity, DateSettings dateRange)
		{
			return DirectAccess.QueryWithCities("GetFlightEmissionsSummaryByAirline", accountNumber, departmentId, startCity, endCity, dateRange);
		}
		public static DataRowManager GetSummaryByAirline(string accountNumber, int departmentId, City startCity, City endCity, DateSettings dateRange, int emissionsField)
		{
			string sql = GetSummaryByAirline(accountNumber, departmentId, startCity, endCity, dateRange);
			var list = DirectAccess.GetStringStringIntDoubleList(sql, 0, 1, 7, emissionsField);

			return new DataRowManager(list);
		}

		/// <summary>
		/// Queries every end city and summarizes its emissions based on the settings passed in.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="endCity"></param>
		/// <returns>A collection of information detailing the emissions for each city pair.</returns>
		public static IEnumerable<StringIntDoubleDateTime> GetDetailForEndCity(SkkyCallParams settings, string endCity)
		{
			return GetDetailForEndCity(settings, City.GetCity(endCity));
		}
		/// <summary>
		/// Queries every end city and summarizes its emissions based on the settings passed in.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="endCity"></param>
		/// <returns>A collection of information detailing the emissions for each city pair.</returns>
		public static IEnumerable<StringIntDoubleDateTime> GetDetailForEndCity(SkkyCallParams settings, City endCity)
		{
			string sql = DirectAccess.QueryWithCity("GetFlightEmissionsByEndCity", settings.AccountNumber, settings.DepartmentId, endCity, settings.DateRange);
			int emissionsField = 11 + settings.GetEmissionsFieldOffset();
			return DirectAccess.GetStringIntDoubleDateTimes(sql, 9, 8, emissionsField, 5);
		}

		/// <summary>
		/// Queries every start city and summarizes its emissions based on the settings passed in.
		/// </summary>
		/// <param name="settings"></param>
		/// <returns>A collection summarizing all cities and their requested emissions data.</returns>
		public static IEnumerable<StringDouble> GetSummaryByStartCity(SkkyCallParams settings, City cty)
		{
			string sql = DirectAccess.QueryWithCity("GetFlightEmissionsSummaryByStartCity", settings.AccountNumber, settings.DepartmentId, cty, settings.DateRange);
			int emissionsField = 2 + settings.GetEmissionsFieldOffset();
			return DirectAccess.GetStringDoubles(sql, 1, emissionsField);
		}

		public static DataRowManager GetEmissionTypeByStartCity(string accountNumber, int departmentId, DateSettings dateRange, int emissionsField)
		{
			string sql = DirectAccess.QueryWithCity("GetFlightEmissionsSummaryByStartCity", accountNumber, departmentId, null, dateRange);
			List<PropertyManager> list = DirectAccess.GetStringStringIntDoubleList(sql, 0, 1, 6, emissionsField);

			return new DataRowManager(list);
		}
		public static DataRowManager GetEmissionTypeByEndCity(string accountNumber, int departmentId, DateSettings dateRange, int emissionsField)
		{
			string sql = DirectAccess.QueryWithCity("GetFlightEmissionsSummaryByEndCity", accountNumber, departmentId, null, dateRange);
			List<PropertyManager> list = DirectAccess.GetStringStringIntDoubleList(sql, 0, 1, 6, emissionsField);

			return new DataRowManager(list);
		}

		public static List<EmissionMarker> GetEmissionMarkersByStartCity(string accountNumber, int departmentId, DateSettings dateRange)
		{
			string sql = DirectAccess.QueryWithCity("GetFlightEmissionsSummaryByStartCity", accountNumber, departmentId, null, dateRange);
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

			if(markerList.Count() > 0)
				return markerList.ToList();

			return new List<EmissionMarker>();
		}

		public static List<EmissionMarker> GetEmissionMarkersByEndCity(string accountNumber, int departmentId, DateSettings dateRange)
		{
			string sql = DirectAccess.QueryWithCity("GetFlightEmissionsSummaryByEndCity", accountNumber, departmentId, null, dateRange);
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

			if (markerList.Count() > 0)
				return markerList.ToList();

			return new List<EmissionMarker>();
		}
	}
}
