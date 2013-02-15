using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class KeyValue
	{
		[DataMember]
		public string key { get; set; }

		[DataMember]
		public string value { get; set; }
	}
}
