using System;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class StringLong
	{
		public StringLong()
		{ }

		public StringLong(string sValue, long lValue)
		{
			stringValue = sValue;
			longValue = lValue;
		}

		[DataMember]
		public string stringValue { get; set; }

		[DataMember]
		public long longValue { get; set; }
	}
}
