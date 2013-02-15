using System;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class StringInt
	{
		public StringInt()
		{ }

		public StringInt(string sValue, int iValue)
		{
			stringValue = sValue;
			intValue = iValue;
		}

		[DataMember]
		public string stringValue { get; set; }

		[DataMember]
		public int intValue { get; set; }
	}
}
