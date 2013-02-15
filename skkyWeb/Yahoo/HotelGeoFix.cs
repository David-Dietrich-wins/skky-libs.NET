using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.db;
using skky.Types;

namespace skkyWeb.Yahoo
{
	public static class HotelGeoFix
	{
		public delegate void MessageHandler(string message);

		public static int FixupHotelGeos(MessageHandler mh)
		{
			int successfulConversions = 0;
			PlaceFinder pf = new PlaceFinder();
			List<HotelGeo> hgList = HotelGeo.GetEmptyLatLngs().Take(10).ToList();

			Console.WriteLine(hgList.Count.ToString());

			hgList.ForEach(a =>
			{
				if (ProcessAddress(mh, pf, ref a))
					++successfulConversions;
			});

			return successfulConversions;
		}

		private static bool ProcessAddress(MessageHandler mh, PlaceFinder pf, ref HotelGeo hg)
		{
			bool bSuccess = false;

			string fullAddress = hg.GetFullAddress();
			ResultSet rs = pf.GeoCode(fullAddress);
			if (rs != null && rs.Result != null && rs.Result.Count() > 0)
			{
				var res = rs.Result[0];

				LatitudeLongitude ll = LatitudeLongitude.parse(res.latitude, res.longitude);
				if (ll != null)
				{
					hg.lat = ll.lat;
					hg.lng = ll.lng;

					bSuccess = true;

					HotelGeo.UpdateLatLng(hg.id, ll.lat, ll.lng);
					if (mh != null)
					{
						mh("Added Lat/Lng for " + fullAddress);
						mh("id " + hg.id.ToString() + ": " + HotelGeo.GetFromID(hg.id).GetLatitudeLongitude().getPrintable());
					}
				}
			}

			return bSuccess;
		}
	}
}
