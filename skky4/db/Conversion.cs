using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
	public partial class Conversion
	{
		public static List<Conversion> GetAll()
		{
			using (var db = new ObjectsDataContext())
			{
				var list = from conv in db.Conversions
						   orderby conv.Name
						   select conv;

				return list.ToList();
			}
		}
	}
}
