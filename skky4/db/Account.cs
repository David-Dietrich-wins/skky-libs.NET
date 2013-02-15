using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Types;

namespace skky.db
{
	public partial class Account
	{
		public static List<StringString> GetAccounts(int idCustomer)
		{
			using (var db = new ObjectsDataContext())
			{
				if (idCustomer > 0)
				{
					var list = from accounts in db.Accounts
							   where accounts.CustomerId == idCustomer
							   orderby accounts.Name
							   select new StringString
							   {
								   stringValue = accounts.Name,
								   string2Value = accounts.Number,
							   };

					return list.ToList();
				}
				else
				{
					var list = from accounts in db.Accounts
							   orderby accounts.Name
							   select new StringString
							   {
								   stringValue = accounts.Name,
								   string2Value = accounts.Number,
							   };

					return list.ToList();
				}
			}
		}
	}
}
