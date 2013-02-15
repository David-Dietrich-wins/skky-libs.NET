using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class OneHundredKilometersPerLitreToMilesPerGallon : ConversionBase
	{
		public const double Const_ConversionBase = 235.2145;

		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.OneHundredKilometersPerLitreToMilesPerGallon;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Litres per 100 Kilometers" : "Miles per Gallon");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "L/100 km" : "MPG");
		}

		public override double ConvertToMetric(double units)
		{
			if (units == 0)
				return 0;

			return Const_ConversionBase / units;
		}
		public override double ConvertToStandard(double units)
		{
			if (units == 0)
				return 0;

			return Const_ConversionBase / units;
		}
	}
}
