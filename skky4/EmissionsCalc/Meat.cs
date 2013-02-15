using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Conversions;

namespace skky.EmissionsCalc
{
	public static class Meat
	{
		public const int CO2PerYearOneHalfPoundPerDay = 3274;
		public const int CO2PerYearOnePoundPerDay = CO2PerYearOneHalfPoundPerDay * 2;
		public const double CO2PerPound = (CO2PerYearOnePoundPerDay / 365);
		//public const double CO2PerKilogram = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilogramsToPounds, false, true, CO2PerPound);

		public static double Convert(bool sourceIsMetric, bool returnInMetric, double units)
		{
			return ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilogramsToPounds, sourceIsMetric, returnInMetric, CO2PerPound * units);
		}
		public static double ConvertAnnualFromDaily(bool sourceIsMetric, bool returnInMetric, double daily)
		{
			return ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilogramsToPounds, sourceIsMetric, returnInMetric, CO2PerPound * daily * 365);
		}
	}
}
