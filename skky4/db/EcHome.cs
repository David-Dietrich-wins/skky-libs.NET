using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
	public partial class EcHome
	{
		public static EcHome UpdateOrAdd(EcHome home)
		{
			EcHome ecc = null;
			DateTime now = DateTime.Now;
			try
			{
				using (var db = new ObjectsDataContext())
				{
					var iquery = (from ec in db.EcHomes
								  where ec.idFbUser == home.idFbUser && ec.name == home.name
								  select ec);
					if (iquery.Count() > 0)
					{
						ecc = iquery.Single();
					}
					else
					{
						ecc = CopyFrom(home);
						ecc.createdOn = now;
						ecc.updatedOn = now;

						db.EcHomes.InsertOnSubmit(ecc);
					}

					long idTemp = ecc.id;
					string nameTemp = ecc.name;
					DateTime createdOnTemp = ecc.createdOn;

					Copy(home, ecc);
					ecc.id = idTemp;
					ecc.name = nameTemp;
					ecc.createdOn = createdOnTemp;
					ecc.updatedOn = now;

					db.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				skky.util.Trace.Critical(ex);
				throw;
			}

			return ecc;
		}

		public static void Copy(EcHome from, EcHome to)
		{
			to.createdOn = from.createdOn;
			to.electricMonthly = from.electricMonthly;
			to.electricPrice = from.electricPrice;
			to.fuelOilConverter = from.fuelOilConverter;
			to.fuelOilMonthly = from.fuelOilMonthly;
			to.fuelOilPrice = from.fuelOilPrice;
			to.id = from.id;
			to.idFbUser = from.idFbUser;
			to.meat = from.meat;
			to.meatConverter = from.meatConverter;
			to.metric = from.metric;
			to.name = from.name;
			to.naturalGasConverter = from.naturalGasConverter;
			to.naturalGasMonthly = from.naturalGasMonthly;
			to.naturalGasPrice = from.naturalGasPrice;
			to.numPeople = from.numPeople;
			to.recycle = from.recycle;
			to.recycleConverter = from.recycleConverter;
			to.waste = from.waste;
			to.wasteConverter = from.wasteConverter;
			to.updatedOn = from.updatedOn;
		}

		public static EcHome CopyFrom(EcHome home)
		{
			EcHome ecc = new EcHome();
			Copy(home, ecc);

			return ecc;
		}
		public static List<EcHome> RetrieveAll(long fbid)
		{
			using (var db = new ObjectsDataContext())
			{
				return (from ec in db.EcHomes
						where ec.idFbUser == fbid
						select ec).ToList();
			}
		}
		public static List<EcHome> RetrieveAll(long fbid, string name)
		{
			using (var db = new ObjectsDataContext())
			{
				return (from ec in db.EcHomes
						where ec.idFbUser == fbid && ec.name == name
						select ec).ToList();
			}
		}
	}
}
