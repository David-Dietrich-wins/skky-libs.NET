using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Types;
using skky.Conversions;

namespace skky.db
{
	public partial class EcGeneral
	{
		public static EcGeneral UpdateOrAdd(long fbid, string sName, double dAmount
			, Namer.Type etype, ConversionBase.ConversionIdentifiers converter, bool isMetric)
		{
			EcGeneral ecc = null;

			try
			{
				using (var db = new ObjectsDataContext())
				{
					var iquery = (from ec in db.EcGenerals
								  where ec.idFbUser == fbid && ec.name == sName
								  select ec);
					if (iquery.Count() > 0)
					{
						ecc = iquery.Single();
					}
					else
					{
						ecc = new EcGeneral()
						{
							idFbUser = fbid,
							name = sName,
							amount = dAmount,
							converter = (int)converter,
							emissionsType = (int)etype,
							metric = isMetric,
							createdOn = DateTime.Now,
						};

						db.EcGenerals.InsertOnSubmit(ecc);
					}

					ecc.amount = dAmount;
					ecc.converter = (int)converter;
					ecc.emissionsType = (int)etype;
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

		public static List<EcGeneral> RetrieveAll(long fbid)
		{
			using (var db = new ObjectsDataContext())
			{
				return (from ec in db.EcGenerals
						where ec.idFbUser == fbid
						select ec).ToList();
			}
		}
		public static List<EcGeneral> RetrieveAll(long fbid, string sName)
		{
			using (var db = new ObjectsDataContext())
			{
				return (from ec in db.EcGenerals
						where ec.idFbUser == fbid && ec.name == sName
						select ec).ToList();
			}
		}
	}
}
