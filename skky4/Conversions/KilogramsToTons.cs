using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class KilogramsToTons : ConversionBase
	{
		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.KilogramsToTons;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Kilograms" : "Tons");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "kg" : "t");
		}

		public override double ConvertToMetric(double units)
		{
			return (units * 2000.0d * 0.4536d);
		}
		public override double ConvertToStandard(double units)
		{
			return ((units * 2.2d) / 2000.0d);
		}
	}
}
