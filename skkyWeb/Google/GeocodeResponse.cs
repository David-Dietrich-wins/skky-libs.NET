using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skkyWeb.Google
{
	public class GeocodeResponse
	{
		public string status { get; set; }
		public results[] results { get; set; }

		public bool isOk
		{
			get
			{
				return Geocode.CONST_GoogleOk == status;
			}
		}

		public bool hasLatLng
		{
			get
			{
				if(null != results && results.Count() > 0)
				{
					results r = results.First();
					if(null != r.geometry
						&& null != r.geometry.location
						&& null != r.geometry.location.lat
						&& null != r.geometry.location.lng)
					{
						return true;
					}
				}

				return false;
			}
		}

		public string GetSQLPoint()
		{
			if (hasLatLng)
			{
				results r = results.First();
				return r.geometry.location.GetSQLPoint();
			}

			return string.Empty;
		}
	}

	public class results
	{
		public string formatted_address { get; set; }
		public geometry geometry { get; set; }
		public string[] types { get; set; }
		public address_component[] address_components { get; set; }
	}

	public class geometry
	{
		public string location_type { get; set; }
		public location location { get; set; }
	}

	public class location
	{
		public double? lat { get; set; }
		public double? lng { get; set; }
		//public string lat { get; set; }
		//public string lng { get; set; }

		public string GetSQLPoint()
		{
			if (null == lat)
				throw new Exception("Invalid latitude passed to GetSQLPoint().");

			if (null == lng)
				throw new Exception("Invalid longitude passed to GetSQLPoint().");

			string point = lat.Value.ToString();
			point += " ";
			point += lng.Value.ToString();

			return "POINT(" + point + ")";
		}
	}

	public class address_component
	{
		public string long_name { get; set; }
		public string short_name { get; set; }
		public string[] types { get; set; }
	}
}
