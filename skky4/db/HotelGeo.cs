using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Types;
using skky.util;
using System.Data.SqlClient;

namespace skky.db
{
    public partial class HotelGeo : ILatitudeLongitude
    {
		protected class AllHotels
		{
			static AllHotels()
			{ }

			internal static readonly List<HotelGeo> HotelGeos = GetHotelGeos();

			private static List<HotelGeo> GetHotelGeos()
			{
				List<HotelGeo> geos = null;
				using (var db = new ObjectsDataContext())
				{
					db.DeferredLoadingEnabled = false;	// Kills the web service dispose() if not disabled.

					var list = from htl in db.HotelGeos
							   select htl;

					geos = list.ToList();
					geos.ForEach(x => x.Trim());
				}

				return geos;
			}
		}

		protected static List<HotelGeo> GetAllHotels()
		{
			return AllHotels.HotelGeos;
		}

		public void Trim()
		{
			cityCode = cityCode.Trim();
			countryCode = countryCode.Trim();
		}
		public string GetFullAddress()
		{
			string address = (address1 ?? (address2 ?? (address3 ?? string.Empty)));

			string fullCity = City.GetCompleteCityName(city, state, region, country);

			if (!string.IsNullOrEmpty(address) && !string.IsNullOrEmpty(fullCity))
				return address + ", " + fullCity;

			return address ?? fullCity;
		}

		public void PrepareUpdate(HotelGeo hg)
		{
			if (hg != null)
			{
				hotelname = hg.hotelname;
				address1 = hg.address1;
				address2 = hg.address2;
				address3 = hg.address3;
				city = hg.city;
				cityCode = hg.cityCode;
				region = hg.region;
				state = hg.state;
				country = hg.country;
				countryCode = hg.countryCode;
				postalcode = hg.postalcode;
				email = hg.email;
				phone = hg.phone;
				fax = hg.fax;
				sabrecode = hg.sabrecode;
				amadeuscode = hg.amadeuscode;
				apollocode = hg.apollocode;
				worldspancode = hg.worldspancode;
				lat = hg.lat;
				lng = hg.lng;
			}
		}

		public static void UpdateLatLng(int idHotelGeo, double lat, double lng)
		{
			using (var db = new ObjectsDataContext())
			{
				HotelGeo hg = db.HotelGeos.Single(x => x.id == idHotelGeo);
				if (hg != null)
				{
					hg.lat = lat;
					hg.lng = lng;
					db.SubmitChanges();
				}
			}
		}

		public static IEnumerable<HotelGeo> GetPropertyByGDS(int idUser, int context, int page, int gdsCode, string propertyCode)
		{
			int numFound = 0;
			IEnumerable<HotelGeo> list = null;
			IEnumerable<HotelGeo> listHotelGeo = null;
			try
			{
				switch (gdsCode)
				{
					case 1:
						list = from htl in GetAllHotels()
							   where htl.amadeuscode.ToLower() == propertyCode.ToLower()
							   select htl;
						break;
					case 2:
						list = from htl in GetAllHotels()
							   where htl.apollocode.ToLower() == propertyCode.ToLower()
							   select htl;
						break;
					case 3:
						list = from htl in GetAllHotels()
							   where htl.sabrecode.ToLower() == propertyCode.ToLower()
							   select htl;
						break;
					case 4:
						list = from htl in GetAllHotels()
							   where htl.worldspancode.ToLower() == propertyCode.ToLower()
							   select htl;
						break;
					default:
						list = new List<HotelGeo>();
						break;
				}

				listHotelGeo = DcsWrapper.GetNonNullIEnumerable<HotelGeo>(list);
				numFound = listHotelGeo.Count();
			}
			finally
			{
				string sql = "dbo.HotelGeoLookupByPropInsert "
					+ idUser.ToString()
					+ context.ToString().PrependCommaSpace(true)
					+ page.ToString().PrependCommaSpace(true)
					+ gdsCode.ToString().PrependCommaSpace(true)
					+ (propertyCode ?? string.Empty).WrapInSingleQuotes().PrependCommaSpace(true)
					+ numFound.ToString().PrependCommaSpace(true);
				DirectAccess.ExecuteNonQuery(sql, true);
			}
			return listHotelGeo;
		}

		public static List<HotelGeo> getHotelsInCity(string city)
		{
			var list = from htl in GetAllHotels()
					   where htl.city == city
					   select htl;

			return DcsWrapper.GetNonNullList<HotelGeo>(list);
		}

		public static List<StringInt> GetHotelCities()
		{
			List<StringInt> strlist = new List<StringInt>();

			var list = from cty in GetAllHotels()
					   where !string.IsNullOrEmpty(cty.city) && cty.lat != 0 && cty.lng != 0
					   group cty by new { cty.city, cty.state, cty.country } into myCity
					   let csc = myCity.Key.city + (string.IsNullOrEmpty(myCity.Key.state) ? string.Empty : ", " + myCity.Key.state)
									+ (!string.IsNullOrEmpty(myCity.Key.city) && !string.IsNullOrEmpty(myCity.Key.state) ? ", " : " ")
									+ myCity.Key.country
					   let iid = myCity.Max(c => c.id)
					   select new StringInt
					   {
						   intValue = iid,
						   stringValue = csc,
					   };

			return list.OrderBy(p => p.stringValue).ToList();
		}
		public static List<string> GetHotelCities2()
		{
			List<string> strlist = new List<string>();
			foreach (var cty in GetAllHotels())
			{
				if (!string.IsNullOrEmpty(cty.city))
				{
					string str = cty.city;
					str += (string.IsNullOrEmpty(cty.state) ? "" : ", " + cty.state);
					if (!string.IsNullOrEmpty(str))
					{
						str += (string.IsNullOrEmpty(cty.state) ? ", " : " ");
					}
					str += cty.country;

					strlist.Add(str);
				}
			}

			return strlist.Distinct().OrderBy(p => p).ToList();
		}
		public static HotelGeo GetFromID(int id)
		{
			if (id > 0)
			{
				var list = from cty in GetAllHotels()
						   where cty.id == id
						   select cty;

				if (list.Count() > 0)
					return list.First();
			}

			return null;
		}

        public static List<HotelGeo> findNearestHotelsInMiles(LatitudeLongitude latlng, double miles)
        {
            if (latlng != null && !latlng.isEmpty())
            {
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

                using (var db = new ObjectsDataContext())
                {
                    var list = from htl in GetAllHotels()
//db.HotelGeos
                               where htl.lat >= dlatLower && htl.lat <= dlatUpper
                                    && htl.lng >= dlngLower && htl.lng <= dlngUpper
                               select htl;

					if (list.Count() > 0)
						return list.ToList();
                }
            }

			return new List<HotelGeo>();
        }

		public static List<HotelGeo> GetEmptyCityCodes()
		{
			return GetAllHotels().Where(x => x.cityCode == "").ToList();
		}
		public static List<HotelGeo> GetEmptyLatLngs()
		{
			return GetAllHotels().Where(x => x.lat == 0 && x.lng == 0).ToList();
		}

		#region ILatitudeLongitude Members

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

		#endregion
	}
}
