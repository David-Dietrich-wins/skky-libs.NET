using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Types;
using skky.db;

namespace skky.EmissionsCalc
{
	public static class Trip
	{
		public static EmissionsPoint CalcEmissions(IEnumerable<LatitudeLongitude> latlngs, bool isMetric)
		{
			EmissionsPoint emTotal = new EmissionsPoint();
			emTotal.IsMetric = isMetric;
			if (latlngs != null)
			{
				for (int i = 0; i < latlngs.Count(); ++i)
				{
					if (i > 0)
					{
						LatitudeLongitude llPrevious = latlngs.ElementAt(i - 1);
						LatitudeLongitude llCurrent = latlngs.ElementAt(i);

						EmissionsPoint legEmissions = AirlineEmission.CalcEmissions(llPrevious, llCurrent, isMetric);
						emTotal.Add(legEmissions);
					}
				}
			}

			return emTotal;
		}

		public static List<UserMarkerLocation> GetMapRoute(int routeId)
		{
			using (var dbContext = new ObjectsDataContext())
			{
				/*
				 * use deferred loading so that we only get what we want
				 * as well as so that the WebService serializer does not attempt to access
				 * the data context 
				 */
				dbContext.DeferredLoadingEnabled = false;

				/*
				 * for now use the admin user. 
				 * @todo save to correct user when user infrastructure is in place 
				 */
				//User admininstratorUser = dbContext.Users.First(user => user.login == "Administrator");
				IQueryable<UserMarkerLocation> locations = from loc in dbContext.UserMarkerLocations
														   where loc.idUserRoute == routeId
														   orderby loc.pointnumber
														   select loc;
				if(locations.Count() > 0)
					return locations.ToList<UserMarkerLocation>();
			}

			return new List<UserMarkerLocation>();
		}

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
