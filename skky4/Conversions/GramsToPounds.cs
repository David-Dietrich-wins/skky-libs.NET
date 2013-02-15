using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class GramsToPounds : ConversionBase
	{
		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.GramsToPounds;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Grams" : "Pounds");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "g" : "lb");
		}

		public override double ConvertToMetric(double units)
		{
			return units * 453.6;
		}
		public override double ConvertToStandard(double units)
		{
			return units * .0022;
		}
	}
}
