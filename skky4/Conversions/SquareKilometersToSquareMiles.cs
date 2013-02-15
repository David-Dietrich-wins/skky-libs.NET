using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class SquareKilometersToSquareMiles : ConversionBase
	{
		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.SquareKilometersToSquareMiles;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Square Kilometers" : "Square Miles");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "km2" : "mi2");
		}

		public override double ConvertToMetric(double units)
		{
			return units * 2.590;
		}
		public override double ConvertToStandard(double units)
		{
			return units * 0.3861;
		}
	}
}
