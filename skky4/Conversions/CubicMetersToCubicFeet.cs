using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class CubicMetersToCubicFeet : ConversionBase
	{
		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.CubicMetersToCubicFeet;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Cubic Meters" : "Cubic Feet");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "m3" : "ft3");
		}

		public override double ConvertToMetric(double units)
		{
			return units * 0.02832;
		}
		public override double ConvertToStandard(double units)
		{
			return units * 35.314;
		}
	}
}
