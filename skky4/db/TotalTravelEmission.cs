using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Types;
using skky.util;

namespace skky.db
{
	public partial class TotalTravelEmission
	{
		public static IEnumerable<StringInt> GetCitiesWithData(SkkyCallParams emissionsSettings)
		{
			if (emissionsSettings == null)
				throw new Exception("TotalTravelEmissions.GetCitiesWithData: NULL EmissionsSettings passed in.");

			List<StringInt> listStringInt = new List<StringInt>();
			string sql = DirectAccess.QueryWithCity("GetTravelEmissionsCitiesWithData", emissionsSettings, null);
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
						   where (from emissions in db.TotalTravelEmissions
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
		public static EmissionsPoint GetCorporateEmissionsTotals(string accountNumber, DateSettings ds)
		{
			if (string.IsNullOrEmpty(accountNumber))
			{
				//throw new Exception("Invalid account number when trying to retrieve emissions totals.");
				using (var db = new ObjectsDataContext())
				{
					if (ds == null || (ds != null && ds.ShouldShowAllDates()))
					{
						var list = from tot in db.TotalTravelEmissions
								   group tot by 1 into g
								   select new EmissionsPoint
								   {
									   CH4 = (double)g.Sum(x => x.TotalCH4),
									   CO2 = (double)g.Sum(x => x.TotalCO2),
									   H2O = (double)g.Sum(x => x.TotalH2O),
									   NOx = (double)g.Sum(x => x.TotalNOx),
									   IsMetric = false,
								   };

						return list.SingleOrDefault();
					}
					else if (ds.OnlyHasStartDate())
					{
						var list = from tot in db.TotalTravelEmissions
								   where tot.StartDateTime >= ds.GetStartDateTime()
								   group tot by 1 into g
								   select new EmissionsPoint
								   {
									   CH4 = (double)g.Sum(x => x.TotalCH4),
									   CO2 = (double)g.Sum(x => x.TotalCO2),
									   H2O = (double)g.Sum(x => x.TotalH2O),
									   NOx = (double)g.Sum(x => x.TotalNOx),
									   IsMetric = false,
								   };

						return list.SingleOrDefault();
					}
					else if (ds.OnlyHasEndDate())
					{
						var list = from tot in db.TotalTravelEmissions
								   where tot.StartDateTime <= ds.GetEndDateTime()
								   group tot by 1 into g
								   select new EmissionsPoint
								   {
									   CH4 = (double)g.Sum(x => x.TotalCH4),
									   CO2 = (double)g.Sum(x => x.TotalCO2),
									   H2O = (double)g.Sum(x => x.TotalH2O),
									   NOx = (double)g.Sum(x => x.TotalNOx),
									   IsMetric = false,
								   };

						return list.SingleOrDefault();
					}
					else
					{
						var list = from tot in db.TotalTravelEmissions
								   where tot.StartDateTime >= ds.GetStartDateTime() && tot.StartDateTime <= ds.GetEndDateTime()
								   group tot by 1 into g
								   select new EmissionsPoint
								   {
									   CH4 = (double)g.Sum(x => x.TotalCH4),
									   CO2 = (double)g.Sum(x => x.TotalCO2),
									   H2O = (double)g.Sum(x => x.TotalH2O),
									   NOx = (double)g.Sum(x => x.TotalNOx),
									   IsMetric = false,
								   };

						return list.SingleOrDefault();
					}
				}
			}
			else
			{
				using (var db = new ObjectsDataContext())
				{
					if (ds == null || (ds != null && ds.ShouldShowAllDates()))
					{
						var list = from tot in db.TotalTravelEmissions
								   where tot.AccountNumber.ToLower() == accountNumber.ToLower()
								   group tot by 1 into g
								   select new EmissionsPoint
								   {
									   CH4 = (double)g.Sum(x => x.TotalCH4),
									   CO2 = (double)g.Sum(x => x.TotalCO2),
									   H2O = (double)g.Sum(x => x.TotalH2O),
									   NOx = (double)g.Sum(x => x.TotalNOx),
									   IsMetric = false,
								   };

						return list.SingleOrDefault();
					}
					else if (ds.OnlyHasStartDate())
					{
						var list = from tot in db.TotalTravelEmissions
								   where tot.AccountNumber.ToLower() == accountNumber.ToLower()
									&& tot.StartDateTime >= ds.GetStartDateTime()
								   group tot by 1 into g
								   select new EmissionsPoint
								   {
									   CH4 = (double)g.Sum(x => x.TotalCH4),
									   CO2 = (double)g.Sum(x => x.TotalCO2),
									   H2O = (double)g.Sum(x => x.TotalH2O),
									   NOx = (double)g.Sum(x => x.TotalNOx),
									   IsMetric = false,
								   };

						return list.SingleOrDefault();
					}
					else if (ds.OnlyHasEndDate())
					{
						var list = from tot in db.TotalTravelEmissions
								   where tot.AccountNumber.ToLower() == accountNumber.ToLower()
									&& tot.StartDateTime <= ds.GetEndDateTime()
								   group tot by 1 into g
								   select new EmissionsPoint
								   {
									   CH4 = (double)g.Sum(x => x.TotalCH4),
									   CO2 = (double)g.Sum(x => x.TotalCO2),
									   H2O = (double)g.Sum(x => x.TotalH2O),
									   NOx = (double)g.Sum(x => x.TotalNOx),
									   IsMetric = false,
								   };

						return list.SingleOrDefault();
					}
					else
					{
						var list = from tot in db.TotalTravelEmissions
								   where tot.AccountNumber.ToLower() == accountNumber.ToLower()
									&& tot.StartDateTime >= ds.GetStartDateTime() && tot.StartDateTime <= ds.GetEndDateTime()
								   group tot by 1 into g
								   select new EmissionsPoint
								   {
									   CH4 = (double)g.Sum(x => x.TotalCH4),
									   CO2 = (double)g.Sum(x => x.TotalCO2),
									   H2O = (double)g.Sum(x => x.TotalH2O),
									   NOx = (double)g.Sum(x => x.TotalNOx),
									   IsMetric = false,
								   };

						return list.SingleOrDefault();
					}
				}
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
			string sql = DirectAccess.QueryWithCity("GetTravelEmissionsDetail", settings.AccountNumber, departmentId, null, settings.DateRange);
			int emissionsField = 7 + settings.GetEmissionsFieldOffset();
			return DirectAccess.GetStringIntDoubleDateTimes(sql, 5, 6, emissionsField, 3);
		}

		/// <summary>
		/// Queries every start city and summarizes its emissions based on the settings passed in.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="startCity"></param>
		/// <returns>A collection of information detailing the emissions for each city pair.</returns>
		public static IEnumerable<StringIntDoubleDateTime> GetDetailForCity(SkkyCallParams settings, string cityCode)
		{
			string sql = DirectAccess.QueryWithCity("GetTravelEmissionsDetail", settings.AccountNumber, settings.DepartmentId, skky.db.City.GetCity(cityCode), settings.DateRange);
			int emissionsField = 7 + settings.GetEmissionsFieldOffset();
			return DirectAccess.GetStringIntDoubleDateTimes(sql, 5, 6, emissionsField, 3);
		}

		/// <summary>
		/// Queries every start city and summarizes its emissions based on the settings passed in.
		/// </summary>
		/// <param name="settings"></param>
		/// <returns>A collection summarizing all cities and their requested emissions data.</returns>
		public static IEnumerable<StringDouble> GetSummaryByCity(SkkyCallParams settings, City cty)
		{
			string sql = DirectAccess.QueryWithCity("GetTravelEmissionsSummaryByCityCode", settings.AccountNumber, settings.DepartmentId, cty, settings.DateRange);
			int emissionsField = 2 + (int)settings.GetEmissionType();
			return DirectAccess.GetStringDoubles(sql, 1, emissionsField);
		}

		public static DataRowManager GetByCity(string accountNumber, int departmentId, City city, DateSettings dateRange, int emissionField)
		{
			string sql = DirectAccess.QueryWithCity("GetTravelEmissionsSummaryByCityCode", accountNumber, departmentId, city, dateRange);
			var list = DirectAccess.GetStringStringIntDoubleList(sql, 0, 1, 6, emissionField);

			return new DataRowManager(list);
		}

		public static List<EmissionMarker> GetEmissionMarkersByCity(string accountNumber, int departmentId, DateSettings dateRange)
		{
			string sql = DirectAccess.QueryWithCity("GetTravelEmissionsSummaryByCityCode", accountNumber, departmentId, null, dateRange);
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

		public static DataRowManager GetSummaryByDepartment(string accountNumber, City city, DateSettings dateRange, int emissionField)
		{
			string sql = "dbo.GetTravelEmissionsSummaryByDepartment " + (string.IsNullOrEmpty(accountNumber) ? "NULL" : accountNumber.WrapInSingleQuotes()) + ", ";
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
			string sql = DirectAccess.QueryWithCity("TravelCO2OverTime", accountNumber, departmentId, city, dateRange);
			var list = DirectAccess.GetDateIntDoubleList(sql, 0, 5, emissionField);

			return new DataRowManager(list);
		}
		/*		
				public static DataRowManager GetTravelEmissionsSummaryByDepartment(string accountNumber, int departmentId, City city)
				{
					string sql = "dbo.GetTravelEmissionsSummaryByDepartment " + (string.IsNullOrEmpty(accountNumber) ? "NULL" : accountNumber.WrapInSingleQuotes()) + ", " + departmentId.ToString() + ", ";
					if (city == null)
						sql += "NULL";
					else
						sql += city.Code.WrapInSingleQuotes();

					sql += ", NULL, NULL";
					System.Type[] props = {
														 typeof(stringValue,
														 typeof(doubleValue,
														 typeof(doubleValue,
														 typeof(doubleValue,
														 typeof(doubleValue,
														 typeof(intValue,
														 typeof(doubleValue,
														 typeof(doubleValue,
													 };
					var list = DirectAccess.GetDataRows(sql, props);

					return new DataRowManager(list);
				}
		 */
	}
}
