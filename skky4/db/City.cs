using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Types;
using skky.util;

namespace skky.db
{
	public partial class City : ILatitudeLongitude
	{
		protected class AllCities
		{
			static AllCities()
			{ }

			static public readonly List<City> Cities = All();

			private static List<City> All()
			{
				List<City> cities = null;
				using (var db = new ObjectsDataContext())
				{
					var list = from cty in db.Cities
							   orderby cty.Name, cty.State, cty.Country
							   select cty;

					cities = list.ToList();
				}

				return cities;
			}
		}

		public static List<City> All()
		{
			return AllCities.Cities;
		}

		// ILatitudeLongitude overrides
		public LatitudeLongitude GetLatitudeLongitude()
		{
			return new LatitudeLongitude(lat, lng);
		}

		public double GetLatitude()
		{
			return lat;
		}

		public double GetLongitude()
		{
			return lng;
		}
		// END: ILatitudeLongitude overrides

		public static string GetFullCityName(City cty)
		{
			string str = cty.Region;
			if (!string.IsNullOrEmpty(cty.Region))
				str += cty.Region.PrependCommaSpace(!string.IsNullOrEmpty(str));

			if (!string.IsNullOrEmpty(cty.Country))
				str += cty.Country.PrependCommaSpace(!string.IsNullOrEmpty(str));

			if (string.IsNullOrEmpty(str))
				str = cty.Name;
			else
				str = cty.Name + str.PrependCommaSpace(true);

			return str;
		}
		public static string GetCompleteCityName(string city, string state, string region, string country)
		{
			string str = state ?? string.Empty;

			if (!string.IsNullOrEmpty(region))
			{
				if (!string.IsNullOrEmpty(str))
				{
					if (!string.IsNullOrEmpty(state))
						str += ",";
					str += " ";
				}
				str += region;
			}

			if (!string.IsNullOrEmpty(country))
			{
				if (!string.IsNullOrEmpty(str))
				{
					if (!string.IsNullOrEmpty(region))
						str += ",";
					str += " ";
				}
				str += country;
			}

			if (!string.IsNullOrEmpty(str))
			{
				if (!string.IsNullOrEmpty(city))
					str = (city ?? string.Empty) + ", " + str;
			}
			else
			{
				str = (city ?? string.Empty);
			}

			return str;
		}

		public static List<StringInt> NameIds()
		{
			var list = from cty in All()
					   orderby cty.Name
					   let cityString = GetFullCityName(cty)
					   select new StringInt
					   {
						   stringValue = cityString,
						   intValue = cty.id,
					   };

			return list.ToList();
		}

		public static City FromId(int id)
		{
			if (id > 0)
			{
				var list = from cty in All()
						   where cty.id == id
						   select cty;

				if (list.Count() > 0)
					return list.First();
			}

			return null;
		}
		public static City FromCode(string cityCode)
		{
			if (!string.IsNullOrEmpty(cityCode))
			{
				var list = from cty in All()
						   where cty.Code == cityCode
						   select cty;

				if (list.Count() > 0)
					return list.First();
			}

			return null;
		}

		public static City FromName(string fullCityName)
		{
			City cty = null;
			if (!string.IsNullOrEmpty(fullCityName))
			{
				var cityFromCode = from city in All()
								   where city.Name.ToLower() == fullCityName.ToLower()
								   select city;
				if (cityFromCode.Any())
				{
					cty = cityFromCode.First();
				}
			}

			return cty;
		}

		public static string getCodeFromName(string cityName)
		{
			if (!string.IsNullOrEmpty(cityName))
			{
				if (cityName.Length == 3)	// It's already a city code.
					return cityName;		// We should check for failed searches and save new cities that are not found.

				City cty = FromName(cityName);
				if (cty != null)
					return cty.Code;
			}

			return string.Empty;
		}
		public static string getNameFromCode(string cityCode)
		{
			if (!string.IsNullOrEmpty(cityCode))
			{
				if (cityCode.Length != 3)	// It's already a city code.
					return cityCode;		// We should check for failed searches and save new cities that are not found.

				City cty = FromCode(cityCode);
				if (cty != null)
					return cty.Name;
			}

			return string.Empty;
		}

		public static City GetCity(int id, string code, string name)
		{
			City cty = FromId(id);
			if (cty == null)
				cty = GetCity(code, name);

			return cty;
		}
		public static City GetCity(string name)
		{
			City cty = null;
			if (!string.IsNullOrEmpty(name))
			{
				cty = FromName(name);
				if (cty == null)
					cty = FromCode(name);
				if (cty == null)
				{
					int id = 0;
					if (int.TryParse(name, out id))
						cty = FromId(id);
				}
			}

			return cty;
		}
		public static City GetCity(string code, string name)
		{
			City cty = null;
			if (!string.IsNullOrEmpty(code))
			{
				if (code.Length == 3)
					cty = FromCode(code);
				else
					cty = FromName(code);

				if (cty != null)
					return cty;
			}

			if (!string.IsNullOrEmpty(name))
			{
				cty = FromName(name);
				if (cty == null && name.Length == 3)
					cty = FromCode(name);
			}

			return cty;
		}
		public static string getFullName(string code, string name)
		{
			City cty = GetCity(code, name);
			if (cty != null)
				return cty.FullName;

			return string.Empty;
		}

		public static LatitudeLongitude getLatLngFromId(int cityId)
		{
			var list = from cty in All()
					   where cty.id == cityId
					   select new LatitudeLongitude
					   {
						   lat = cty.lat,
						   lng = cty.lng
					   };

			if (list.Count() > 0)
				return list.First();

			return null;
		}
		public static LatitudeLongitude getLatLng(string cityCode)
		{
			if (!string.IsNullOrEmpty(cityCode))
			{
				var list = from cty in All()
						   where cty.Code == cityCode
						   select new LatitudeLongitude
						   {
							   lat = cty.lat,
							   lng = cty.lng
						   };

				if (list.Count() > 0)
					return list.First();
			}

			return null;
		}

		public static List<LatitudeLongitude> CityPairsFromCodes(string city1, string city2)
		{
			if (!string.IsNullOrEmpty(city1) && !string.IsNullOrEmpty(city2))
			{
				var list = from cty in All()
						   where cty.Code == city1 || cty.Code == city2
						   orderby city1
						   select new LatitudeLongitude
						   {
							   lat = cty.lat,
							   lng = cty.lng
						   };

				if (list != null)
				{
					return list.ToList();
				}
			}

			return new List<LatitudeLongitude>();
		}
		static public double DistanceInMi(string cityStart, string cityEnd)
		{
			IEnumerable<LatitudeLongitude> list = CityPairsFromCodes(cityStart, cityEnd);
			if (list.Count() == 2)
			{
				var newlist = list.ToList();
				LatitudeLongitude llStart = newlist.ElementAt(0);
				LatitudeLongitude llEnd = newlist.ElementAt(1);
				return LatitudeLongitude.getDistanceInMi(llStart, llEnd);
			}

			return 0;
		}
		static public double DistanceInKm(string cityStart, string cityEnd)
		{
			IEnumerable<LatitudeLongitude> list = CityPairsFromCodes(cityStart, cityEnd);
			if (list.Count() == 2)
			{
				var newlist = list.ToList();
				LatitudeLongitude llStart = newlist.ElementAt(0);
				LatitudeLongitude llEnd = newlist.ElementAt(1);
				return LatitudeLongitude.getDistanceInKm(llStart, llEnd);
			}

			return 0;
		}
		static public double DistanceInNm(string cityStart, string cityEnd)
		{
			IEnumerable<LatitudeLongitude> list = CityPairsFromCodes(cityStart, cityEnd);
			if (list.Count() == 2)
			{
				var newlist = list.ToList();
				LatitudeLongitude llStart = newlist.ElementAt(0);
				LatitudeLongitude llEnd = newlist.ElementAt(1);
				return LatitudeLongitude.getDistanceInNm(llStart, llEnd);
			}

			return 0;
		}

		public static List<City> fromIDs(int[] ids)
		{
			List<City> cities = new List<City>();
			if (ids != null)
			{
				for (int i = 0; i < ids.Length; ++i)
				{
					City cty = FromId(ids[i]);
					if (cty == null)
						cty = new City();

					cities.Add(cty);
				}
			}

			return cities;
		}

		public static City FindNearestCity(double lat, double lng)
		{
			LatitudeLongitude latlng = new LatitudeLongitude(lat, lng);
			int miles = 20;
			// Build a square of miles.
			double dlatLower = latlng.getLowerLatInMi(miles);
			double dlatUpper = latlng.getUpperLatInMi(miles);
			double dlngLower = latlng.getLowerLngInMi(miles);
			double dlngUpper = latlng.getUpperLngInMi(miles);

			if (dlatLower > dlatUpper)
			{
				double dTemp = dlatLower;
				dlatLower = dlatUpper;
				dlatUpper = dTemp;
			}
			if (dlngLower > dlngUpper)
			{
				double dTemp = dlngLower;
				dlngLower = dlngUpper;
				dlngUpper = dTemp;
			}

			var list = from city in All()
					   where city.lat >= dlatLower && city.lat <= dlatUpper
							&& city.lng >= dlngLower && city.lng <= dlngUpper
					   select (ILatitudeLongitude)city;

			ILatitudeLongitude ill = latlng.GetClosest(list);
			if (ill != null)
				return (City)ill;

			return null;
		}

		public static void CopyCity(City from, City to)
		{
			if (from != null && to != null)
			{
				to.Code = from.Code;
				to.Country = from.Country;
				to.FullName = from.FullName;
				to.id = from.id;
				to.lat = from.lat;
				to.lng = from.lng;
				to.Name = from.Name;
				to.Region = from.Region;
				to.State = from.State;
			}
		}
	}
}
