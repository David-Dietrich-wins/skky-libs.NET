using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class KilometersToMiles : ConversionBase
	{
		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.KilometersToMiles;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Kilometers" : "Miles");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "km" : "mi");
		}

		public override double ConvertToMetric(double units)
		{
			return units * 1.609;
		}
		public override double ConvertToStandard(double units)
		{
			return units * 0.621371192;
		}
	}
}
