using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Types;

namespace skky.db
{
	public partial class UserRoute
	{
		public static List<StringIntDoubleDateTime> GetRouteNames(int skkyUserId)
		{
			using (var db = new ObjectsDataContext())
			{
				var iquery = from u in db.UserRoutes
							 where u.idUser == skkyUserId
							 orderby u.routename
							 select new StringIntDoubleDateTime
							 {
								 dateTimeValue = u.starttime,
								 doubleValue = u.id,
								 intValue = u.id,
								 stringValue = u.routename,
							 };

				if(iquery.Count() > 0)
					return iquery.ToList();
			}

			return new List<StringIntDoubleDateTime>();
		}
		public static List<UserRoute> GetRoutes(int skkyUserId)
		{
			using (var db = new ObjectsDataContext())
			{
				var iquery = from u in db.UserRoutes
							 where u.idUser == skkyUserId
							 orderby u.routename
							 select u;

				if (iquery.Count() > 0)
					return iquery.ToList();
			}

			return new List<UserRoute>();
		}
	}
}
