using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace skky.jqGrid
{
	public class gridInitModel
	{
		public gridInitModel()
		{
			JSON = "success";
			colNames = new List<string>();
			colModel = new List<colModel>();
		}

		[DataMember]
		public string JSON { get; set; }

		[DataMember]
		public List<string> colNames { get; set; }

		[DataMember]
		public List<colModel> colModel { get; set; }

		[DataMember]
		public gridModel gridModel { get; set; }
	}
}