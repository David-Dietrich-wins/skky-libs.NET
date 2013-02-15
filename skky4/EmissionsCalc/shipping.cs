using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Types;
using skky.db;
using skky.Conversions;

namespace skky.EmissionsCalc
{
	public static class shipping
	{
		public static EmissionsPoint CalcEmissions(IEnumerable<LatitudeLongitude> latlngs, double weight, bool isMetric)
		{
			EmissionsPoint emTotal = new EmissionsPoint();
			emTotal.IsMetric = isMetric;
			if (latlngs != null)
			{
				AirlineEmission ae = AirlineEmission.GetEmissions("UPS747");
				if (ae != null)
				{
					for (int i = 0; i < latlngs.Count(); ++i)
					{
						if (i > 0)
						{
							LatitudeLongitude llPrevious = latlngs.ElementAt(i - 1);
							LatitudeLongitude llCurrent = latlngs.ElementAt(i);

							EmissionsPoint legEmissions = ae.getEmissionsPointStandard(llPrevious, llCurrent);
							legEmissions.MultiplyEmissions(ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilogramsToPounds, isMetric, false, weight));
							//legEmissions.MultiplyEmissions(weight);
							legEmissions.DivideEmissions(2000);	// UPS747 is per ton.

							legEmissions.Distance = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilometersToMiles, false, isMetric, legEmissions.Distance);
							//legEmissions.CO2 = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilometersToMiles, false, isMetric, legEmissions.CO2);
							//legEmissions.CH4 = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilometersToMiles, false, isMetric, legEmissions.CH4);
							//legEmissions.H2O = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilometersToMiles, false, isMetric, legEmissions.H2O);
							//legEmissions.NOx = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilometersToMiles, false, isMetric, legEmissions.NOx);
							//legEmissions.SOx = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilometersToMiles, false, isMetric, legEmissions.SOx);

							//legEmissions.CO2 = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilogramsToPounds, false, isMetric, legEmissions.CO2);
							//legEmissions.CH4 = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilogramsToPounds, false, isMetric, legEmissions.CH4);
							//legEmissions.H2O = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilogramsToPounds, false, isMetric, legEmissions.H2O);
							//legEmissions.NOx = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilogramsToPounds, false, isMetric, legEmissions.NOx);
							//legEmissions.SOx = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilogramsToPounds, false, isMetric, legEmissions.SOx);
							emTotal.Add(legEmissions);
						}
					}
				}
			}

			return emTotal;
		}
		/*
The following table shows the amount of CO2 (in grams) emitted per metric ton of freight and per km of transportation: 
Air plane (air cargo), average Cargo B747 	500 g 
Modern lorry or truck	60 to 150 g 
Modern train 	30 to 100 g 
Modern ship (sea freight) 	10 to 40 g 
Airship (Zeppelin, Cargolifter ) as planned 	55 g 
The values for air cargo has been taken from Lufthansa Air cargo, who operates a modern fleet.

1.1 lb / km
.5 lb / mi
		 */

		public static EmissionsPoint CalcEmissions(IEnumerable<UserMarkerLocation> umls, bool isMetric)
		{
			EmissionsPoint emTotal = new EmissionsPoint();
			emTotal.IsMetric = isMetric;
			if (umls != null)
			{
				for (int i = 0; i < umls.Count(); ++i)
				{
					if (i > 0)
					{
						UserMarkerLocation umlPrevious = umls.ElementAt(i - 1);
						UserMarkerLocation umlCurrent = umls.ElementAt(i);

						LatitudeLongitude llPrevious = new LatitudeLongitude(umlPrevious.lat, umlPrevious.lng);
						LatitudeLongitude llCurrent = new LatitudeLongitude(umlCurrent.lat, umlCurrent.lng);

						EmissionsPoint legEmissions = AirlineEmission.CalcEmissions(llPrevious, llCurrent, isMetric);
						emTotal.Add(legEmissions);
					}
				}
			}

			return emTotal;
		}
	}
}
