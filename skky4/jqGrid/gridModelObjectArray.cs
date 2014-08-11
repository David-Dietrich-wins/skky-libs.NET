using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace skky.jqGrid
{
	public class gridModelObjectArray : GridModelBase
	{
		[DataContract]
		public class Row
		{
			[DataMember]
			public object id;

			[DataMember]
			public object[] cell;

			public Row(object oid, int numCells)
			{
				id = oid;
				cell = new object[numCells];
			}
			public Row(object oid, object[] cells)
			{
				id = oid;
				cell = cells;
			}

			public Row(int numCells)
				: this(null, numCells)
			{ }
		}

		public gridModelObjectArray()
		{
			rows = new List<Row>();
		}
		public Row getNewRow(int numCells)
		{
			return new Row(numCells);
		}
		public Row AddRow(object rowId, object[] objects)
		{
			Row row = new Row(rowId, objects);
			rows.Add(row);

			return row;
		}

		[DataMember]
		public List<Row> rows;
	}
}
