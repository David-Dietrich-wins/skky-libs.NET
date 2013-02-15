using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class CubicMetersPerSecondToGallonsPerMinute : ConversionBase
	{
		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.CubicMetersPerSecondToGallonsPerMinute;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Cubic Meters per Second" : "Gallons per Minute");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "lps" : "gpm");
		}

		public override double ConvertToStandard(double units)
		{
			return units * 15850.0;
		}

		public override double ConvertToMetric(double units)
		{
			if (units == 0)
				return 0;

			return units / 15850.0;
		}
	}
}
