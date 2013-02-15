using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class CelsiusToFahrenheit : ConversionBase
	{
		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.CelsiusToFahrenheit;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Celsius" : "Farenheit");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "C" : "F");
		}

		public override double ConvertToMetric(double units)
		{
			return (((units - 32d) * 5d) / 9d);
		}
		public override double ConvertToStandard(double units)
		{
			return ((units * 1.8d) + 32d);
		}
	}
}
