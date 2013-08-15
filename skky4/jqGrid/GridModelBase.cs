using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace skky.jqGrid
{
	public class GridModelBase
	{
		public GridModelBase()
		{
			sord = "asc";
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
		public Dictionary<string, string> userdata { get; set; }
	}
}
