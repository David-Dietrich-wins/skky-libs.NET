using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using skky.util;

namespace skkyWeb.Google
{
	public static class Geocode
	{
		public const string CONST_GoogleGeocodeUrl = "http://maps.google.com/maps/api/geocode/json?address={0}&sensor=false";
		public const string CONST_GoogleOk = "OK";

		public static GeocodeResponse Decode(string address, string city, string state, string zip)
		{
			return Decode(address + ", " + city + " " + state + ", " + zip);
		}

		public static GeocodeResponse Decode(string address)
		{
			if(string.IsNullOrWhiteSpace(address))
				address = "1600 Amphitheatre Parkway, Mountain View, CA";
			//address = "1600+Amphitheatre+Parkway,+Mountain+View,+CA";

			string encodedAddress = HttpUtility.UrlEncode(address ?? string.Empty);

			string url = string.Format(CONST_GoogleGeocodeUrl, encodedAddress);
			WebClient wc = new WebClient();
			string result = wc.DownloadString(url);

			GeocodeResponse gcr = DcsWrapper.GetObjectFromJson<GeocodeResponse>(result);

			return gcr;
		}
	}
}
