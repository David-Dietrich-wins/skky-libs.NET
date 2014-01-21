using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using skky.util;
using skky.web;
using skkyWeb.util;

namespace skkyWeb.Yahoo
{
	public class PlaceFinder
	{
		public const string CONST_GeoCodeURL = "http://where.yahooapis.com/geocode?q={1}&appid={0}";
		private static readonly OAuthAppSettings skkynetApplication = new OAuthAppSettings()
		{
			ApplicationID = "XqwWIY7k",
			ApplicationKey = "dj0yJmk9bHlMMDVlQzUzUzA5JmQ9WVdrOVdIRjNWMGxaTjJzbWNHbzlPVEF4TnpZeE16WXkmcz1jb25zdW1lcnNlY3JldCZ4PWVh",
			ApplicationDomain = "www.skky.net",
			SharedSecret = "b762bf0c097f0a110027450f5b49a26f5f16dffb"
		};

		private OAuthAppSettings curOAuthAppSettings = skkynetApplication;

		public string xmlResponse { get; set; }

		public string getGeoCodeURL(string address)
		{
			string encodedAddress = HttpUtility.UrlEncode(address ?? "");
			return string.Format(CONST_GeoCodeURL, curOAuthAppSettings.ApplicationID, encodedAddress);
		}
		public ResultSet GeoCode(string address)
		{
			// XML to .Net class 
			//>xsd.exe YahooGeoCodeResponse.xml /outputdir:y
			//>cd y
			//>xsd YahooGeoCodeResponse.xsd /c /n:skky.Types
			//
			//Find the coordinates of a street address:
			//http://where.yahooapis.com/geocode?q=1600+Pennsylvania+Avenue,+Washington,+DC&appid=[yourappidhere]

			//Find the street address nearest to a point:
			//http://where.yahooapis.com/geocode?q=38.898717,+-77.035974&gflags=R&appid=[yourappidhere] 
			//string placeFinderURL = "http://where.yahooapis.com/geocode?q=1600+Pennsylvania+Avenue,+Washington,+DC&appid=";
			XMLHttpRequestor requestor = null;
			ResultSet rs = null;
			string sendURL = getGeoCodeURL(address);
			try
			{
				requestor = new XMLHttpRequestor(sendURL);
				rs = requestor.HttpGetFromXML<ResultSet>();
				xmlResponse = requestor.xmlResponse;
			}
			finally
			{
				try
				{
					XDocument xmldoc = XMLHelper.getXDocument(xmlResponse);

				}
				catch (Exception ex)
				{
					skky.util.Trace.Critical(ex);
				}
			}

			return rs;
		}
	}
}
