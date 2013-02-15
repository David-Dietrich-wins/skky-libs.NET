using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.Types;
using System.Collections;
using System.ServiceModel.Web;

namespace skky.web
{
	[DataContract]
//	[KnownType(typeof(PopupMenu))]
//	[KnownType(typeof(Hashtable))]
	public class skkyjson
	{
		public skkyjson()
		{
			popupMenus = new KeyValuePairCollection<string, PopupMenu>();
			nameValues = new KeyValuePairCollection<string, object>();
		}

		[DataMember]
		public string innerHTML { get; set; }

		[DataMember]
		public string outerHTML { get; set; }

		[DataMember]
		public double doubleValue { get; set; }

		[DataMember]
		public KeyValuePairCollection<string, PopupMenu> popupMenus { get; private set; }

		[DataMember]
		public KeyValuePairCollection<string, object> nameValues { get; private set; }
	}
}
