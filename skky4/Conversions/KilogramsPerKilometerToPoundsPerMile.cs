using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class KilogramsPerKilometerToPoundsPerMile : ConversionBase
	{
		public override ConversionIdentifiers GetIdentifier()
		{
			return ConversionIdentifiers.KilogramsPerKilometerToPoundsPerMile;
		}

		public static string GetLongName(bool isMetric)
		{
			return (isMetric ? "Kilograms per Kilometer" : "Pounds Per Mile");
		}
		public static string GetShortName(bool isMetric)
		{
			return (isMetric ? "kg/km" : "lb/mi");
		}

		public override double ConvertToMetric(double units)
		{
			return Convert(ConversionBase.ConversionIdentifiers.KilogramsToPounds, false, true,
				Convert(ConversionBase.ConversionIdentifiers.KilometersToMiles, true, false, units));
		}
		public override double ConvertToStandard(double units)
		{
			return Convert(ConversionBase.ConversionIdentifiers.KilogramsToPounds, true, false,
				Convert(ConversionBase.ConversionIdentifiers.KilometersToMiles, false, true, units));
		}
	}
}
