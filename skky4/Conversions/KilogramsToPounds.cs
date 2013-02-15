using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class KilogramsToPounds : ConversionBase
	{
		public const double Const_ConversionLb_Kg = 0.45359237;

		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.KilogramsToPounds;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Kilograms" : "Pounds");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "kg" : "lb");
		}

		public override double ConvertToMetric(double units)
		{
			return units * Const_ConversionLb_Kg;
		}
		public override double ConvertToStandard(double units)
		{
			if (units == 0)
				return 0;

			return units / Const_ConversionLb_Kg;
		}
	}
}
