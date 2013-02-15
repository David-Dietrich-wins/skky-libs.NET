using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class SquareMetersToSquareFeet : ConversionBase
	{
		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.SquareMetersToSquareFeet;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Square Meters" : "Square Feet");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "m2" : "ft2");
		}

		public override double ConvertToMetric(double units)
		{
			return units * 0.09294;
		}
		public override double ConvertToStandard(double units)
		{
			return units * 10.76;
		}
	}
}
