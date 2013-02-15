using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class KilogramsPerLitreToPoundsPerUKGallon : ConversionBase
	{
		public const double Const_ConversionFactorKgl_lbg = 0.099776373;

		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.KilogramsPerLitreToPoundsPerUKGallon;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Kilograms per Litre" : "Pounds per Gallon");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "kg/l" : "lb/g");
		}

		public override double ConvertToMetric(double units)
		{
			return units * Const_ConversionFactorKgl_lbg;		// UK gallon 99.776373
		}
		public override double ConvertToStandard(double units)
		{
			if (units == 0)
				return 0;

			return units / Const_ConversionFactorKgl_lbg;
		}
	}
}
