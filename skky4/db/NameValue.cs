using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
	public partial class NameValue
	{
		public NameValue(string sName, string sValue)
		{
			Name = sName;
			Value = sValue;
		}

		public static NameValue Add(string name, string value)
		{
			NameValue nv = null;
			if (!string.IsNullOrEmpty(name))
			{
				try
				{
					using (var db = new ObjectsDataContext())
					{
						nv = new NameValue()
						{
							Name = name,
							Value = value,
							//createdOn = DateTime.Now,
						};

						db.NameValues.InsertOnSubmit(nv);
						db.SubmitChanges();
					}
				}
				catch (Exception ex)
				{
					skky.util.Trace.Critical(ex);
					throw;
				}
			}

			return nv;
		}
	}
}
