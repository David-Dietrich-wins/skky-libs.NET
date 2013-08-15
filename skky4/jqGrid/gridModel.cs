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
	}
}
