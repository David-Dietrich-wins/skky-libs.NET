using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class LitersToGallons : ConversionBase
	{
		public const double Const_ConversionG_L = 3.7854118;

		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.LitersToGallons;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Litres" : "Gallons");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "l" : "g");
		}

		public override double ConvertToMetric(double units)
		{
			return units * Const_ConversionG_L;
		}
		public override double ConvertToStandard(double units)
		{
			if (units == 0)
				return 0;

			return units / Const_ConversionG_L;
		}
	}
}
