using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class KilogramsPerM3ToPoundsPerImperialGallon : ConversionBase
	{
		public const double Const_ConversionFactor = 99.77637266;

		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.KilogramsPerM3ToPoundsPerImperialGallon;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Kilograms per Cubic Meter" : "Pounds per Gallon");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "kg/m3" : "lb/g");
		}

		public override double ConvertToMetric(double units)
		{
			return units * Const_ConversionFactor;		// US gallon 119.8264273
		}
		public override double ConvertToStandard(double units)
		{
			if (units == 0)
				return 0;

			return units / Const_ConversionFactor;		// US gallon;
		}
	}
}
