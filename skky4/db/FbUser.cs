using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
	public partial class FbUser
	{
		public static FbUser Find(long fbuid)
		{
			using (var db = new ObjectsDataContext())
			{
				return db.FbUsers.First(fb => fb.id == fbuid);
			}
		}
		public static bool IsUserAdded(long fbuid)
		{
			using (var db = new ObjectsDataContext())
			{
				var iquery = (from fb in db.FbUsers
							 where fb.id == fbuid
							 select fb.id);
				if (iquery.Count() > 0)
					return true;
			}

			return false;
		}

		public static FbUser JoinWithSkkyAccount(long fbuid, Guid SkkyId)
		{
			FbUser fbNew = null;
			try
			{
				using (var db = new ObjectsDataContext())
				{
					DateTime now = DateTime.Now;

					var newUser = (from fb in db.FbUsers
									where fb.id == fbuid
									select fb);
					if (newUser.Count() > 0)
						fbNew = newUser.Single();

					if (fbNew == null)
					{
						fbNew = new FbUser()
						{
							id = fbuid,
							createdOn = now,
							deleted = 0,
							endedOn = null,
							Notes = "",
							numActions = 0,
							UserID = SkkyId,
						};

						db.FbUsers.InsertOnSubmit(fbNew);
					}

					fbNew.UserID = SkkyId;
					++fbNew.numActions;

					FbUserLog userLog = CopyFrom(fbNew, now);

					fbNew.FbUserLogs.Add(userLog);
					db.FbUserLogs.InsertOnSubmit(userLog);

					db.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				skky.util.Trace.Critical(ex);
				throw;
			}

			return fbNew;
		}
		public static bool UnjoinWithSkkyAccount(long fbuid, Guid SkkyId)
		{
			FbUser fbNew = null;
			try
			{
				using (var db = new ObjectsDataContext())
				{
					DateTime now = DateTime.Now;

					var newUser = (from fb in db.FbUsers
								   where fb.id == fbuid && fb.UserID == SkkyId
								   select fb);
					if (newUser.Count() > 0)
						fbNew = newUser.Single();

					if (fbNew == null)
						return false;

					fbNew.UserID = null;
					++fbNew.numActions;

					FbUserLog userLog = CopyFrom(fbNew, now);

					fbNew.FbUserLogs.Add(userLog);
					db.FbUserLogs.InsertOnSubmit(userLog);

					db.SubmitChanges();

					return true;
				}
			}
			catch (Exception ex)
			{
				skky.util.Trace.Critical(ex);
				throw;
			}
		}

		public static void AddUser(long fbuid, string notes)
		{
			if (fbuid < 1)
				throw new Exception("Invalid user id of 0 attempting to insert into the user table.");

			try
			{
				FbUser fbNew = null;
				using (var db = new ObjectsDataContext())
				{
					DateTime now = DateTime.Now;

					var newUser = (from fb in db.FbUsers
									where fb.id == fbuid
									select fb);
					if (newUser.Count() > 0)
						fbNew = newUser.Single();

					if (fbNew == null)
					{
						fbNew = new FbUser()
						{
							id = fbuid,
							createdOn = now,
							deleted = 0,
							endedOn = null,
							Notes = notes ?? "",
							numActions = 0,
							UserID = null,
						};

						db.FbUsers.InsertOnSubmit(fbNew);
					}

					FbUserLog userLog = CopyFrom(fbNew, now);

					fbNew.FbUserLogs.Add(userLog);
					db.FbUserLogs.InsertOnSubmit(userLog);

					db.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				skky.util.Trace.Critical(ex);
				throw;
			}
		}
		public static void DeleteUser(long fbuid)
		{
			try
			{
				using (var db = new ObjectsDataContext())
				{
					DateTime now = DateTime.Now;

					FbUser user = (from fb in db.FbUsers
								   where fb.id == fbuid
								   select fb).Single();
					if (user == null)
					{
						user = new FbUser()
						{
							createdOn = now,
							deleted = 1,
							endedOn = now,
							Notes = "",
							numActions = 0,
							UserID = null,
						};

						db.FbUsers.InsertOnSubmit(user);
					}

					user.endedOn = now;

					FbUserLog userLog = CopyFrom(user, now);

					user.FbUserLogs.Add(userLog);
					db.FbUserLogs.InsertOnSubmit(userLog);

					db.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				skky.util.Trace.Critical(ex);
				throw;
			}
		}

		private static FbUserLog CopyFrom(FbUser user, DateTime dtLog)
		{
			FbUserLog userLog = new FbUserLog()
			{
				createdOn = user.createdOn,
				deleted = user.deleted,
				endedOn = user.endedOn,
				idFbUser = user.id,
				Notes = user.Notes,
				numActions = user.numActions,
				UserID = user.UserID,
				logTime = dtLog,
			};

			return userLog;
		}
	}
}
