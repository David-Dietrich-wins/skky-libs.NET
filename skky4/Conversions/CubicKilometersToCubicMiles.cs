using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class CubicKilometersToCubicMiles : ConversionBase
	{
		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.CubicKilometersToCubicMiles;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Cubic Kilometers" : "Cubic Miles");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "km3" : "m3");
		}

		public override double ConvertToMetric(double units)
		{
			return units * 4.168;
		}
		public override double ConvertToStandard(double units)
		{
			return units * 0.2399;
		}
	}
}
