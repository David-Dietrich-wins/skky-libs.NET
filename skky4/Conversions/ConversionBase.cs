using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public class ConversionBase : IConversion
	{
		// These need to match the Conversion table id's.
		public enum ConversionIdentifiers
		{
			None = 0,
			CelsiusToFahrenheit = 1,
			CubicKilometersToCubicMiles = 2,
			CubicMetersPerSecondToGallonsPerMinute = 3,
			CubicMetersToAcreFeet = 4,
			CubicMetersToCubicFeet = 5,
			CubicMetersToGallons = 6,
			GramsToPounds = 7,
			KilogramsPerLitreToPoundsPerUKGallon = 8,
			KilogramsPerLitreToPoundsPerUSGallon = 9,
			KilogramsPerM3ToPoundsPerImperialGallon = 10,
			KilogramsPerM3ToPoundsPerUSGallon = 11,
			KilogramsToPounds = 12,
			KilogramsToTons = 13,
			KilometersPerLitreToMilesPerGallon = 14,
			KilometersToMiles = 15,
			LitersToGallons = 16,
			OneHundredKilometersPerLitreToMilesPerGallon = 17,
			SquareKilometersToSquareMiles = 18,
			SquareMetersToSquareFeet = 19,
			KilogramsPerKilometerToPoundsPerMile = 20,
		};

		public virtual ConversionIdentifiers GetIdentifier()
		{
			throw new Exception("No ConversionIdentifier configured for type.");
		}

		public virtual double ConvertToMetric(double units)
		{
			throw new Exception("ConvertToMetric ConversionBase");
		}
		public virtual double ConvertToMetricOneUnit()
		{
			return ConvertToMetric(1);
		}

		public virtual double ConvertToStandard(double units)
		{
			throw new Exception("ConvertToStandard ConversionBase");
		}
		public virtual double ConvertToStandardOneUnit()
		{
			return ConvertToStandard(1);
		}

		public virtual double ConvertAsSingleUnit(bool sourceIsMetric, bool returnAsMetric, double units)
		{
			return Convert(returnAsMetric, sourceIsMetric, units);
		}

		public virtual double Convert(bool sourceIsMetric, bool returnAsMetric, double units)
		{
			// Both are the same, nothing to convert.
			if (sourceIsMetric == returnAsMetric)
				return units;

			if (sourceIsMetric)		// Target is standard.
				return ConvertToStandard(units);

			return ConvertToMetric(units);	// Finally must be converted to metric.
		}
		public static double Convert(ConversionIdentifiers id, bool sourceIsMetric, bool returnAsMetric, double units)
		{
			ConversionBase cb = GetConversionObjectFromIdentifier(id);

			return cb.Convert(sourceIsMetric, returnAsMetric, units);
		}
		public static double ConvertSafe(ConversionIdentifiers id, bool sourceIsMetric, bool returnAsMetric, double units)
		{
			try
			{
				ConversionBase cb = GetConversionObjectFromIdentifier(id);

				return cb.Convert(sourceIsMetric, returnAsMetric, units);
			}
			catch (Exception)
			{ }

			return units;
		}
		public static double Convert(int id, bool sourceIsMetric, bool returnAsMetric, double units)
		{
			return Convert((ConversionIdentifiers)id, sourceIsMetric, returnAsMetric, units);
		}
		public static double ConvertSafe(int id, bool sourceIsMetric, bool returnAsMetric, double units)
		{
			return ConvertSafe((ConversionIdentifiers)id, sourceIsMetric, returnAsMetric, units);
		}

		public static double ConvertAsSingleUnit(ConversionIdentifiers id, bool sourceIsMetric, bool returnAsMetric, double units)
		{
			ConversionBase cb = GetConversionObjectFromIdentifier(id);

			return cb.ConvertAsSingleUnit(sourceIsMetric, returnAsMetric, units);
		}
		public static double ConvertAsSingleUnitSafe(ConversionIdentifiers id, bool sourceIsMetric, bool returnAsMetric, double units)
		{
			try
			{
				ConversionBase cb = GetConversionObjectFromIdentifier(id);

				return cb.ConvertAsSingleUnit(sourceIsMetric, returnAsMetric, units);
			}
			catch (Exception)
			{ }

			return units;
		}
		public static double ConvertAsSingleUnit(int id, bool sourceIsMetric, bool returnAsMetric, double units)
		{
			return ConvertAsSingleUnit((ConversionIdentifiers)id, sourceIsMetric, returnAsMetric, units);
		}
		public static double ConvertAsSingleUnitSafe(int id, bool sourceIsMetric, bool returnAsMetric, double units)
		{
			return ConvertAsSingleUnitSafe((ConversionIdentifiers)id, sourceIsMetric, returnAsMetric, units);
		}

		public static ConversionBase GetConversionObjectFromIdentifier(ConversionIdentifiers id)
		{
			ConversionBase conversionObject = null;
			switch (id)
			{
				case ConversionIdentifiers.CelsiusToFahrenheit:
					conversionObject = new CelsiusToFahrenheit();
					break;
				case ConversionIdentifiers.CubicKilometersToCubicMiles:
					conversionObject = new CubicKilometersToCubicMiles();
					break;
				case ConversionIdentifiers.CubicMetersPerSecondToGallonsPerMinute:
					conversionObject = new CubicMetersPerSecondToGallonsPerMinute();
					break;
				case ConversionIdentifiers.CubicMetersToAcreFeet:
					conversionObject = new CubicMetersToAcreFeet();
					break;
				case ConversionIdentifiers.CubicMetersToCubicFeet:
					conversionObject = new CubicMetersToCubicFeet();
					break;
				case ConversionIdentifiers.CubicMetersToGallons:
					conversionObject = new CubicMetersToGallons();
					break;
				case ConversionIdentifiers.GramsToPounds:
					conversionObject = new GramsToPounds();
					break;
				case ConversionIdentifiers.KilogramsPerLitreToPoundsPerUKGallon:
					conversionObject = new KilogramsPerLitreToPoundsPerUKGallon();
					break;
				case ConversionIdentifiers.KilogramsPerLitreToPoundsPerUSGallon:
					conversionObject = new KilogramsPerLitreToPoundsPerUSGallon();
					break;
				case ConversionIdentifiers.KilogramsPerM3ToPoundsPerImperialGallon:
					conversionObject = new KilogramsPerM3ToPoundsPerImperialGallon();
					break;
				case ConversionIdentifiers.KilogramsPerM3ToPoundsPerUSGallon:
					conversionObject = new KilogramsPerM3ToPoundsPerUSGallon();
					break;
				case ConversionIdentifiers.KilogramsToPounds:
					conversionObject = new KilogramsToPounds();
					break;
				case ConversionIdentifiers.KilogramsToTons:
					conversionObject = new KilogramsToTons();
					break;
				case ConversionIdentifiers.KilometersPerLitreToMilesPerGallon:
					conversionObject = new KilometersPerLitreToMilesPerGallon();
					break;
				case ConversionIdentifiers.KilometersToMiles:
					conversionObject = new KilometersToMiles();
					break;
				case ConversionIdentifiers.LitersToGallons:
					conversionObject = new LitersToGallons();
					break;
				case ConversionIdentifiers.OneHundredKilometersPerLitreToMilesPerGallon:
					conversionObject = new OneHundredKilometersPerLitreToMilesPerGallon();
					break;
				case ConversionIdentifiers.SquareKilometersToSquareMiles:
					conversionObject = new SquareKilometersToSquareMiles();
					break;
				case ConversionIdentifiers.SquareMetersToSquareFeet:
					conversionObject = new SquareMetersToSquareFeet();
					break;
				case ConversionIdentifiers.KilogramsPerKilometerToPoundsPerMile:
					conversionObject = new KilogramsPerKilometerToPoundsPerMile();
					break;
				default:
					break;
			}

			if(conversionObject == null)
				throw new Exception("No valid ConversionIdentifier specified: " + id.ToString() + ".");

			return conversionObject;
		}
	}
}
