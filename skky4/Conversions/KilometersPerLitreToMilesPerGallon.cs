using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class KilometersPerLitreToMilesPerGallon : ConversionBase
	{
		public const double Const_MPGtoKmL = 0.425143706225;

		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.KilometersPerLitreToMilesPerGallon;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Kilometers per Litre" : "Miles per Gallon");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "km/l" : "MPG");
		}

		public override double ConvertToMetric(double units)
		{
			return units * Const_MPGtoKmL;
		}
		public override double ConvertToStandard(double units)
		{
			if (units == 0)
				return 0;

			return units / Const_MPGtoKmL;
		}
	}
}
