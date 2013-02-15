using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace skky.jqGrid
{
	public class gridInitModelObjectArray
	{
		public gridInitModelObjectArray()
		{
			JSON = "success";
			colNames = new List<string>();
			colModel = new List<colModel>();
		}

		[DataMember]
		public string title;

		[DataMember]
		public string JSON;

		[DataMember]
		public List<string> colNames;

		[DataMember]
		public List<colModel> colModel;

		[DataMember]
		public gridModelObjectArray gridModel;
	}
}