using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
	public partial class FbInvite
	{
		public static void AddInvites(long fbuid, List<int> ids)
		{
			if (ids != null && ids.Count() > 0)
			{
				DateTime now = DateTime.Now;

				foreach (int targetuid in ids)
				{
					FbInvite fbi = null;
					try
					{
						using (var db = new ObjectsDataContext())
						{
							var iquery = (from fb in db.FbInvites
										  where fb.fbidFrom == fbuid && fb.fbidTo == targetuid
										  select fb);
							if (iquery.Count() > 0)
								fbi = iquery.Single();

							if (fbi == null)
							{
								fbi = new FbInvite()
								{
									accepted = 0,
									createdOn = now,
									fbidFrom = fbuid,
									fbidTo = targetuid,
								};

								db.FbInvites.InsertOnSubmit(fbi);
							}

							fbi.accepted = 0;
							fbi.createdOn = now;

							db.SubmitChanges();
						}

						WriteToLog(fbi, now);
					}
					catch (Exception ex)
					{
						skky.util.Trace.Critical(ex);
						throw;
					}
				}
			}
		}

		public static void AcceptInvite(long fbuidFrom, long fbuidTo)
		{
			DateTime now = DateTime.Now;
			FbInvite fbi = null;

			try
			{
				using (var db = new ObjectsDataContext())
				{
					var iquery = (from fb in db.FbInvites
								  where fb.fbidFrom == fbuidFrom && fb.fbidTo == fbuidTo
								  select fb);
					if (iquery.Count() > 0)
						fbi = iquery.Single();

					if (fbi == null)
					{
						fbi = new FbInvite()
						{
							accepted = 0,
							createdOn = now,
							fbidFrom = fbuidFrom,
							fbidTo = fbuidTo,
						};

						db.FbInvites.InsertOnSubmit(fbi);
					}

					fbi.accepted++;
					db.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				skky.util.Trace.Critical(ex);
				throw;
			}

			try
			{
				WriteToLog(fbi, now);
			}
			catch (Exception ex)
			{
				skky.util.Trace.Critical(ex);
				throw;
			}
		}

		private static FbInviteLog CopyFrom(FbInvite fbi, DateTime dtLog)
		{
			FbInviteLog fbil = new FbInviteLog()
			{
				accepted = fbi.accepted,
				createdOn = fbi.createdOn,
				fbidFrom = fbi.fbidFrom,
				fbidTo = fbi.fbidTo,
				idFbInvite = fbi.id,
				logTime = dtLog,
			};

			return fbil;
		}

		private static FbInviteLog WriteToLog(FbInvite fbi, DateTime dtLog)
		{
			using (var db = new ObjectsDataContext())
			{
				FbInviteLog fil = CopyFrom(fbi, dtLog);

				db.FbInviteLogs.InsertOnSubmit(fil);
				db.SubmitChanges();

				return fil;
			}
		}
	}
}
