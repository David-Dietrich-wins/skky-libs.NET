using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
	public partial class FbUserSetting
	{
		public static readonly string CONST_metric = "metric";
		public static FbUserSetting FindSetting(long fbuid, string name)
		{
			FbUserSetting fbSetting = null;
			using (var db = new ObjectsDataContext())
			{
				try
				{
					var iquery = (from fb in db.FbUserSettings
							where fb.fbid == fbuid && fb.name == name
							select fb);
					if (iquery.Count() > 0)
						return iquery.Single();

					DateTime now = DateTime.Now;
					fbSetting = new FbUserSetting()
					{
						createdOn = now,
						fbid = fbuid,
						name = CONST_metric,
						value = "false",
					};

					db.FbUserSettings.InsertOnSubmit(fbSetting);

					FbUserSettingLog settingLog = CopyFrom(fbSetting, now);

					fbSetting.FbUserSettingLogs.Add(settingLog);
					db.FbUserSettingLogs.InsertOnSubmit(settingLog);

					db.SubmitChanges();
				}
				catch (Exception ex)
				{
					skky.util.Trace.Critical(ex);
				}
			}

			return fbSetting;
		}
		public static List<FbUserSetting> FindSettings(long fbuid)
		{
			using (var db = new ObjectsDataContext())
			{
				try
				{
					var iquery = (from fb in db.FbUserSettings
								  where fb.fbid == fbuid
								  select fb);
					if (iquery.Count() > 0)
						return iquery.ToList();
				}
				catch (Exception ex)
				{
					skky.util.Trace.Critical(ex);
				}
			}

			FbUserSetting fbSetting = FindSetting(fbuid, CONST_metric);
			List<FbUserSetting> list = new List<FbUserSetting>();
			if (fbSetting != null)
				list.Add(fbSetting);

			return list;
		}

		public static void Update(long fbuid, bool bMetric)
		{
			try
			{
				FbUserSetting fbSetting = null;
				using (var db = new ObjectsDataContext())
				{
					var settingIquery = (from fb in db.FbUserSettings
										 where fb.fbid == fbuid && fb.name == CONST_metric
										 select fb);
					if (settingIquery.Count() > 0)
						fbSetting = settingIquery.Single();

					DateTime now = DateTime.Now;
					if (fbSetting == null)
					{
						fbSetting = new FbUserSetting()
						{
							fbid = fbuid,
							createdOn = now,
//							name = CONST_metric,
//							value = bMetric.ToString(),
						};

						db.FbUserSettings.InsertOnSubmit(fbSetting);
					}

					fbSetting.name = CONST_metric;
					fbSetting.value = bMetric.ToString();

					FbUserSettingLog settingLog = CopyFrom(fbSetting, now);

					fbSetting.FbUserSettingLogs.Add(settingLog);
					db.FbUserSettingLogs.InsertOnSubmit(settingLog);

					db.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				skky.util.Trace.Critical(ex);
				throw;
			}
		}

		private static FbUserSettingLog CopyFrom(FbUserSetting user, DateTime dtLog)
		{
			FbUserSettingLog userLog = new FbUserSettingLog()
			{
				createdOn = user.createdOn,
				fbid = user.fbid,
				idFbUserSetting = user.id,
				name = user.name,
				value = user.value,
				logTime = dtLog,
			};

			return userLog;
		}
	}
}
