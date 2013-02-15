using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Types;

namespace skky.EmissionsCalc
{
	public static class Car
	{
		// Average per gallon - http://www.epa.gov/oms/climate/420f05001.htm
		// http://en.wikipedia.org/wiki/Fuel_economy_in_automobiles
		//Gasoline carbon content per gallon: 2,421 grams  (5.33739 lb)
		//Diesel carbon content per gallon: 2,778 grams  (6.12444 lb)
		//Finally, to calculate the CO2 emissions from a gallon of fuel, the carbon emissions are multiplied by the ratio of the molecular weight of CO2 (m.w. 44) to the molecular weight of carbon (m.w.12): 44/12.
		//CO2 emissions from a gallon of gasoline = 2,421 grams x 0.99 x (44/12) = 8,788 grams = 8.8 kg/gallon = 19.4 pounds/gallon
		//CO2 emissions from a gallon of diesel = 2,778 grams x 0.99 x (44/12) = 10,084 grams = 10.1 kg/gallon = 22.2 pounds/gallon
		public const double Const_CO2PoundsPerGallonGasoline = 19.4;
		public const double Const_CO2PoundsPerGallonDiesel = 22.23;
		public const double Const_CO2PoundsPerGallonJetFuel = 22.1;

		public const double Const_CO2KilogramsPerLitreGasoline = 2.32;
		public const double Const_CO2KilogramsPerLitreDiesel = 2.66;

		public static double CO2PoundsPerGallon(bool isDiesel)
		{
			return (isDiesel ? Const_CO2PoundsPerGallonDiesel : Const_CO2PoundsPerGallonGasoline);
		}
		public static double CO2KilogramsPerLitre(bool isDiesel)
		{
			return (isDiesel ? Const_CO2KilogramsPerLitreDiesel : Const_CO2KilogramsPerLitreGasoline);
		}

		/// <summary>
		/// Calculates CO2 per year based on distance.
		/// </summary>
		/// <param name="distance">How far traveled in miles or kilometers.</param>
		/// <param name="fuelEconomy">Fuel economy in MPG or 100km/L</param>
		/// <param name="isDiesel">Is a diesel engine.</param>
		/// <param name="isMetric">distance and fuelEconomy are metric.</param>
		/// <param name="returnMetric">Return in Metric or Standard.</param>
		/// <returns>CO2 per year.</returns>
		public static double CO2Emissions(double distance, double fuelEconomy, bool isDiesel, bool isMetric)
		{
			return Emissions(EmissionsHelper.CO2.NamerType, distance, fuelEconomy, isDiesel, isMetric);
		}
		//public static double Emissions(EmissionsHelper.EmissionType etype, double distance, double fuelEconomy, bool isDiesel, bool isMetric)
		//{
		//    double emissions = Emissions(etype, distance, fuelEconomy, isDiesel, isMetric);

		//    if (isMetric != returnMetric)
		//    {
		//        skky.Conversions.KilogramsPerLitreToPoundsPerUSGallon kgpl = new skky.Conversions.KilogramsPerLitreToPoundsPerUSGallon();
		//        emissions = kgpl.ConvertFromTo(isMetric, returnMetric, emissions);
		//    }

		//    return emissions;
		//}
		public static double Emissions(Namer.Type etype, double distance, double fuelEconomy, bool isDiesel, bool isMetric)
		{
			double totalEmissions = 0;
			if (fuelEconomy != 0)
			{
				if(etype == EmissionsHelper.CO2.NamerType)
					totalEmissions = (isMetric ? CO2KilogramsPerLitre(isDiesel) : CO2PoundsPerGallon(isDiesel));

				totalEmissions *= distance;

				// Fuel economy is measured in L/100km for metric.
				// MPG for standard.
				if (isMetric)
				{
					totalEmissions *= fuelEconomy;
					totalEmissions /= 100;
				}
				else
				{
					totalEmissions /= fuelEconomy;
				}

			}

			return totalEmissions;
		}
	}
}
