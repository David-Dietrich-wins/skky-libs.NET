using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
	public partial class EcCar
	{
		public static EcCar UpdateOrAdd(long fbid, string sName, double dDistance, double dFuelEconomy, bool isDiesel, bool isMetric)
		{
			EcCar ecc = null;

			try
			{
				using (var db = new ObjectsDataContext())
				{
					var iquery = (from ec in db.EcCars
								  where ec.idFbUser == fbid && ec.name == sName
								  select ec);
					if (iquery.Count() > 0)
					{
						ecc = iquery.Single();
					}
					else
					{
						ecc = new EcCar()
						{
							idFbUser = fbid,
							name = sName,
							distance = dDistance,
							fuelEconomy = dFuelEconomy,
							diesel = isDiesel,
							metric = isMetric,
							createdOn = DateTime.Now,
						};

						db.EcCars.InsertOnSubmit(ecc);
					}

					ecc.distance = dDistance;
					ecc.fuelEconomy = dFuelEconomy;
					ecc.diesel = isDiesel;
					ecc.metric = isMetric;

					db.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				skky.util.Trace.Critical(ex);
			}

			return ecc;
		}

		public static List<EcCar> RetrieveAll(long fbid)
		{
			using (var db = new ObjectsDataContext())
			{
				return (from ec in db.EcCars
						where ec.idFbUser == fbid
						select ec).ToList();
			}
		}
	}
}
