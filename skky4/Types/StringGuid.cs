using System;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class StringGuid
	{
		public StringGuid()
		{ }

		public StringGuid(string sValue, Guid gValue)
		{
			stringValue = sValue;
			guidValue = gValue;
		}

		[DataMember]
		public string stringValue { get; set; }

		[DataMember]
		public Guid guidValue { get; set; }
	}
}
