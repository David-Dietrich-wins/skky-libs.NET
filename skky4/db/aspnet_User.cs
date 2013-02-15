using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
	public partial class aspnet_User
	{
		public static aspnet_User findFromId(Guid guid)
		{
			using(var db = new ASPNetDbDataContext())
			{
				var list = (from users in db.aspnet_Users
							where users.UserId == guid
							select users);

				if (list.Count() > 0)
					return list.First();
			}

			return null;
		}
	}
}
