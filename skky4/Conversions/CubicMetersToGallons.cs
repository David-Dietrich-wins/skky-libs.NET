using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class CubicMetersToGallons : ConversionBase
	{
		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.CubicMetersToGallons;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Cubic Meters" : "Gallons");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "m3" : "g");
		}

		public override double ConvertToMetric(double units)
		{
			return 0.0037854118;
		}
		public override double ConvertToStandard(double units)
		{
			return units * 264.172051242;
		}
	}
}
