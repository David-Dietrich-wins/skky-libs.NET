using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class CubicMetersToAcreFeet : ConversionBase
	{
		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.CubicMetersToAcreFeet;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Cubic Meters" : "Acre Feet");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "m3" : "af");
		}

		public override double ConvertToMetric(double units)
		{
			return units * 1233;
		}
		public override double ConvertToStandard(double units)
		{
			return units * 0.0008107;
		}
	}
}
