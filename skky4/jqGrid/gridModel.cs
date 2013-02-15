using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace skky.jqGrid
{
	public class gridModel
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
			sord = "asc";
			rows = new List<Row>();
		}

		[DataMember]
		public int page { get; set; }

		[DataMember]
		public int total { get; set; }

//		[DataMember(IsRequired=false)]
		[DataMember]
		public int records { get; set; }

		[DataMember]
		public string sidx { get; set; }

		[DataMember]
		public string sord { get; set; }

		[DataMember]
		public List<Row> rows { get; set; }

		[DataMember]
		public Dictionary<string, string> userdata { get; set; }
	}
}
