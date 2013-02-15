using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[Serializable]
	[DataContract]
	public class LatitudeLongitude : ILatitudeLongitude
	{
		public const double Const_1DegreeLatKm = 110.5743;
		public const double Const_1DegreeLatMi = 68.70768;
		public const double Const_1MinuteLatKm = 1.8429;
		public const double Const_1MinuteLatMi = 1.1451;
		public const double Const_1SecondLatMt = 30.7151;	// Meters
		public const double Const_1SecondLatFt = 100.771;	// Feet

		public const double Const_1DegreeLngKm = 111.6939;
		public const double Const_1DegreeLngMi = 69.40337;
		public const double Const_1MinuteLngKm = 1.8616;
		public const double Const_1MinuteLngMi = 1.1567;
		public const double Const_1SecondLngMt = 31.0261;	// Meters
		public const double Const_1SecondLngFt = 101.792;	// Feet

		public const double Const_1MileOfLatitude = (0.868 / 60.0);	// degrees
		public const double Const_1MileOfLongitude = (1.375 / 60.0);	// degrees

		public LatitudeLongitude()
		{ }
		public LatitudeLongitude(double latitude, double longitude)
		{
			lat = latitude;
			lng = longitude;
		}

		public static LatitudeLongitude parse(string latitude, string longitude)
		{
			double dlat = 0.0;
			bool b = double.TryParse((latitude ?? string.Empty).Trim(), out dlat);
			if (b)
			{
				double dlng = 0.0;
				b = double.TryParse((longitude ?? string.Empty).Trim(), out dlng);
				if (b)
					return new LatitudeLongitude(dlat, dlng);
			}

			return null;
		}

		[DataMember]
		public double lat { get; set; }
		[DataMember]
		public double lng { get; set; }

		public LatitudeLongitude GetLatitudeLongitude()
		{
			return this;
		}

		public double GetLatitude()
		{
			return lat;
		}

		public double GetLongitude()
		{
			return lng;
		}

		public bool isEmpty()
		{
			return (lat == 0 && lng == 0);
		}

		public ILatitudeLongitude GetClosest(IEnumerable<ILatitudeLongitude> list)
		{
			ILatitudeLongitude closest = null;
			if (list != null)
			{
				double distance = -1;
				foreach (var ll in list)
				{
					double curdist = getDistanceInKm(ll.GetLatitudeLongitude());
					if (distance < 0 || (curdist < distance))
					{
						distance = curdist;
						closest = ll;
					}
				}
			}

			return closest;
		}

		public static double getMilesOfLatitude(double miles)
		{
			return miles * Const_1MileOfLatitude;
		}
		public static double getMilesOfLongitude(double miles)
		{
			return miles * Const_1MileOfLongitude;
		}

		public double getLowerLatInMi(double miles)
		{
			return lat - getMilesOfLatitude(miles);
		}
		public double getUpperLatInMi(double miles)
		{
			return lat + getMilesOfLatitude(miles);
		}
		public double getLowerLngInMi(double miles)
		{
			return lng - getMilesOfLongitude(miles);
		}
		public double getUpperLngInMi(double miles)
		{
			return lng + getMilesOfLongitude(miles);
		}

		public double getDistanceInMi(double lat2, double lng2)
		{
			return getDistanceInMi(this.lat, this.lng, lat2, lng2);
		}

		public double getDistanceInKm(double lat2, double lng2)
		{
			return getDistanceInKm(this.lat, this.lng, lat2, lng2);
		}

		public double getDistanceInNm(double lat2, double lng2)
		{
			return getDistanceInNm(this.lat, this.lng, lat2, lng2);
		}

		public double getDistanceInMi(LatitudeLongitude latlng)
		{
			return (latlng != null ? getDistanceInMi(latlng.lat, latlng.lng) : 0);
		}

		public double getDistanceInKm(LatitudeLongitude latlng)
		{
			return (latlng != null ? getDistanceInKm(latlng.lat, latlng.lng) : 0);
		}

		public double getDistanceInNm(LatitudeLongitude latlng)
		{
			return (latlng != null ? getDistanceInNm(latlng.lat, latlng.lng) : 0);
		}

		public static double getDistanceInMi(LatitudeLongitude lat1, LatitudeLongitude lat2)
		{
			return getDistanceInMi(lat1.lat, lat1.lng, lat2.lat, lat2.lng);
		}

		public static double getDistanceInKm(LatitudeLongitude lat1, LatitudeLongitude lat2)
		{
			return getDistanceInKm(lat1.lat, lat1.lng, lat2.lat, lat2.lng);
		}

		public static double getDistanceInNm(LatitudeLongitude lat1, LatitudeLongitude lat2)
		{
			return getDistanceInNm(lat1.lat, lat1.lng, lat2.lat, lat2.lng);
		}

		// The Formula for Latitude Distance at a Given Latitude (theta) in Km:
		// 1° of Latitude = 111.13295 - 0.55982 * cos(2 * theta) + 0.00117 * cos(4 * theta)

		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		//::                                                                         ::
		//::  This routine calculates the distance between two points (given the     ::
		//::  latitude/longitude of those points). It is being used to calculate     ::
		//::  the distance between two ZIP Codes or Postal Codes using our           ::
		//::  ZIPCodeWorld(TM) and PostalCodeWorld(TM) products.                     ::
		//::                                                                         ::
		//::  Definitions:                                                           ::
		//::    South latitudes are negative, east longitudes are positive           ::
		//::                                                                         ::
		//::  Passed to function:                                                    ::
		//::    lat1, lon1 = Latitude and Longitude of point 1 (in decimal degrees)  ::
		//::    lat2, lon2 = Latitude and Longitude of point 2 (in decimal degrees)  ::
		//::    unit = the unit you desire for results                               ::
		//::           where: 'M' is statute miles                                   ::
		//::                  'K' is kilometers (default)                            ::
		//::                  'N' is nautical miles                                  ::
		//::  United States ZIP Code/ Canadian Postal Code databases with latitude & ::
		//::  longitude are available at http://www.zipcodeworld.com                 ::
		//::                                                                         ::
		//::  For enquiries, please contact sales@zipcodeworld.com                   ::
		//::                                                                         ::
		//::  Official Web site: http://www.zipcodeworld.com                         ::
		//::                                                                         ::
		//::  Hexa Software Development Center ? All Rights Reserved 2004            ::
		//::                                                                         ::
		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		public static double getDistanceInMi(double lat1, double lon1, double lat2, double lon2)
		{
			double theta = lon1 - lon2;
			double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
			dist = Math.Acos(dist);
			dist = rad2deg(dist);
			dist = dist * 60 * 1.1515;

			return dist;
		}
		public static double getDistanceInKm(double lat1, double lon1, double lat2, double lon2)
		{
			double d = getDistanceInMi(lat1, lon1, lat2, lon2);
			return d * 1.609344;
		}
		public static double getDistanceInNm(double lat1, double lon1, double lat2, double lon2)
		{
			double d = getDistanceInMi(lat1, lon1, lat2, lon2);
			return d * 0.8684;
		}

		public static double getNauticalMilesFromStatute(double dStatuteMiles)
		{
			return dStatuteMiles * 1.1515; // * 8 / 7
		}
		public static double getStatuteMilesFromNautical(double dNauticalMiles)
		{
			return dNauticalMiles * 0.875; // * 7 / 8
		}

		//::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		//::  This function converts decimal degrees to radians             ::
		//::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		private static double deg2rad(double deg)
		{
			return (deg * Math.PI / 180.0);
		}

		//::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		//::  This function converts radians to decimal degrees             ::
		//::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		private static double rad2deg(double rad)
		{
			return (rad * 180 / Math.PI);
		}

		public string getPrintable()
		{
			return ("Lat: " + lat.ToString() + ", Lng: " + lng.ToString());
		}

		//public static void main(String args[])
		//{
		//    double dlat = 32.9697;
		//    double dlng = -96.80322;
		//    double dlat2 = 29.46786;
		//    double dlng2 = -98.53506;

		//    printToSystemStatic(getDistanceInMi(dlat, dlng, dlat2, dlng2) + " Miles");
		//    printToSystemStatic(getDistanceInKm(dlat, dlng, dlat2, dlng2) + " Kilometers");
		//    printToSystemStatic(getDistanceInNm(dlat, dlng, dlat2, dlng2) + " Nautical Miles");
		//    printToSystemStatic("\n");

		//    dlng2 = dlng;
		//    printToSystemStatic(getDistanceInMi(dlat, dlng, dlat2, dlng2) + " Miles");
		//    printToSystemStatic(getDistanceInKm(dlat, dlng, dlat2, dlng2) + " Kilometers");
		//    printToSystemStatic(getDistanceInNm(dlat, dlng, dlat2, dlng2) + " Nautical Miles");
		//    printToSystemStatic("\n");

		//    //double dTemp = 0.014492753623188406; // Roughly (1.0 / 69)
		//    double dTemp = 0.014473875; // Roughly (1.0 / 69)
		//    dlat2 = dlat + dTemp;
		//    //dlng2 = dlng + (1.0 / 60);
		//    printToSystemStatic(getDistanceInMi(dlat, dlng, dlat2, dlng2) + " Miles");
		//    printToSystemStatic(getDistanceInKm(dlat, dlng, dlat2, dlng2) + " Kilometers");
		//    printToSystemStatic(getDistanceInNm(dlat, dlng, dlat2, dlng2) + " Nautical Miles");
		//    printToSystemStatic("\n");
		//}
	}
}
