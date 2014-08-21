using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace skky.jqGrid
{
	public class gridModel : GridModelBase
	{
		[DataContract]
		public class Row
		{
			[DataMember]
			public object id { get; set; }

			[DataMember]
			public List<string> cell { get; set; }

			public Row()
			{
				cell = new List<string>();
			}
		}

		public gridModel()
		{
			rows = new List<Row>();
		}

		[DataMember]
		public List<Row> rows { get; set; }

		#region JqGrid support methods
		public static string GetSafeString(string jqgridString)
		{
			string str = (jqgridString ?? string.Empty).Replace("'", "\\'");

			return str;
		}
		public static string GetSelectItemString(int selectId, string selectValue, bool prependSemicolon = true)
		{
			string str = (prependSemicolon ? ";" : string.Empty);

			str += selectId + ":" + GetSafeString(selectValue);

			return str;
		}
		#endregion
	}
}
