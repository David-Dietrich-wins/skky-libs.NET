using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Types;

namespace skky.db
{
	public partial class Department
	{
		public static List<StringInt> GetDepartments(string accountNumber)
		{
			if (string.IsNullOrEmpty(accountNumber) || accountNumber == "All")
				return new List<StringInt>();

			using (var db = new ObjectsDataContext())
			{
				var list = from depts in db.Departments
						   where depts.Account.Number == accountNumber
						   orderby depts.Name
						   select new StringInt
						   {
							   intValue = depts.id,
							   stringValue = depts.Name
						   };

				return list.ToList();
			}
		}

		public static string GetDepartmentName(int departmentId)
		{
			using (var db = new ObjectsDataContext())
			{
				var list = from depts in db.Departments
						   where depts.id == departmentId
						   select depts.Name;

				return list.FirstOrDefault();
			}
		}
	}
}
