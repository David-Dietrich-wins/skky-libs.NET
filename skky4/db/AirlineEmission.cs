using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Conversions;
using skky.Types;

namespace skky.db
{
    public partial class AirlineEmission
    {
        private static readonly AirlineEmission DefaultEmissionsPerMile = GetEmissions("default");
		private static readonly AirlineEmission DefaultEmissionsPerKilometer = new AirlineEmission()
		{
			// Must convert from miles to kilometers. It looks backward in the code, but we need 1 km, not 1 mile converted to kilometers. Which would be 1.6 km.
			// Then from pounds to kilograms.
			AircraftModel = DefaultEmissionsPerMile.AircraftModel,
			CH4permile = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilogramsPerKilometerToPoundsPerMile, false, true, DefaultEmissionsPerMile.CH4permile),
			CO2permile = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilogramsPerKilometerToPoundsPerMile, false, true, DefaultEmissionsPerMile.CO2permile),
			H2Opermile = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilogramsPerKilometerToPoundsPerMile, false, true, DefaultEmissionsPerMile.H2Opermile),
			id = DefaultEmissionsPerMile.id,
			NOxpermile = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilogramsPerKilometerToPoundsPerMile, false, true, DefaultEmissionsPerMile.NOxpermile),
		};

        public static AirlineEmission GetEmissions(string aircraftModel)
        {
            using (var db = new ObjectsDataContext())
            {
                var result = from emissions in db.AirlineEmissions
                             where emissions.AircraftModel == aircraftModel.ToLower()
                             select emissions;
                if (result.Count() > 0)
                    return result.First();
            }

            return DefaultEmissions;
        }

        public static AirlineEmission DefaultEmissions
        {
			get
			{
				return DefaultEmissionsPerMile;
			}
        }

		public static EmissionsPoint CalcEmissions(LatitudeLongitude start, LatitudeLongitude end, bool isMetric)
		{
			AirlineEmission ae = (isMetric ? AirlineEmission.DefaultEmissionsPerKilometer : AirlineEmission.DefaultEmissions);
			return ae.getEmissionsPoint(start, end, isMetric);
		}

		public EmissionsPoint getEmissionsPoint(LatitudeLongitude start, LatitudeLongitude end, bool isMetric)
		{
			EmissionsPoint em = new EmissionsPoint();
			if (start != null && end != null)
			{
				// Get the distance in miles 
				double miles = start.getDistanceInMi(end);
				em.Distance = ConversionBase.ConvertSafe(ConversionBase.ConversionIdentifiers.KilometersToMiles, false, isMetric, miles);
				// Now conversions are in native conversion mode. Simple multiply.
				em.CH4 = em.Distance * CH4permile;
				em.CO2 = em.Distance * CO2permile;
				em.H2O = em.Distance * H2Opermile;
				em.NOx = em.Distance * NOxpermile;
				//em.SOx = em.Distance * SOxpermile;
			}

			return em;
		}

		public EmissionsPoint getEmissionsPointStandard(LatitudeLongitude start, LatitudeLongitude end)
		{
			EmissionsPoint em = new EmissionsPoint();
			if (start != null && end != null)
			{
				// Get the distance in miles 
				double miles = start.getDistanceInMi(end);
				em.Distance = miles;
				// Now conversions are in native conversion mode. Simple multiply.
				em.CH4 = em.Distance * CH4permile;
				em.CO2 = em.Distance * CO2permile;
				em.H2O = em.Distance * H2Opermile;
				em.NOx = em.Distance * NOxpermile;
				//em.SOx = em.Distance * SOxpermile;
			}

			return em;
		}

		public static double DefaultCO2(bool isMetric)
		{
			return isMetric ? DefaultEmissionsPerKilometer.CO2permile : DefaultEmissionsPerMile.CO2permile;
		}
		public static double DefaultCH4(bool isMetric)
		{
			return isMetric ? DefaultEmissionsPerKilometer.CH4permile : DefaultEmissionsPerMile.CH4permile;
		}
		public static double DefaultH2O(bool isMetric)
		{
			return isMetric ? DefaultEmissionsPerKilometer.H2Opermile : DefaultEmissionsPerMile.H2Opermile;
		}
		public static double DefaultNOx(bool isMetric)
		{
			return isMetric ? DefaultEmissionsPerKilometer.NOxpermile : DefaultEmissionsPerMile.NOxpermile;
		}
	}
}
