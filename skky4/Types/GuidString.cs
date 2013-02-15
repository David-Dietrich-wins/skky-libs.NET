using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class GuidString
	{
		public GuidString()
		{ }

		public GuidString(Guid gValue, string sValue)
		{
			guidValue = gValue;
			stringValue = sValue;
		}

		[DataMember]
		public Guid guidValue { get; set; }

		[DataMember]
		public string stringValue { get; set; }
	}
}
