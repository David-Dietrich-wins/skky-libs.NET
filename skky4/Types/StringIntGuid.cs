using System;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class StringIntGuid
	{
		public StringIntGuid()
		{ }

		public StringIntGuid(string sValue, int iValue, Guid gValue)
		{
			stringValue = sValue;
			intValue = iValue;
			guidValue = gValue;
		}

		[DataMember]
		public string stringValue { get; set; }

		[DataMember]
		public int intValue { get; set; }

		[DataMember]
		public Guid guidValue { get; set; }
	}
}
