using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace skky.jqGrid
{
	public class gridModelObjectArray
	{
		[DataContract]
		public class Row
		{
			[DataMember]
			public object id;

			[DataMember]
			public object[] cell;

			public Row(int numCells)
			{
				cell = new object[numCells];
			}
		}

		public gridModelObjectArray()
		{
			sord = "asc";
			rows = new List<Row>();
		}
		public Row getNewRow(int numCells)
		{
			return new Row(numCells);
		}

		[DataMember]
		public int page;

		[DataMember]
		public int total;

		[DataMember(IsRequired=false)]
		public int? records;

		[DataMember]
		public int? sidx;

		[DataMember]
		public string sord;

		[DataMember]
		public List<Row> rows;

		[DataMember]
		public Dictionary<string, string> userdata;
	}
}
