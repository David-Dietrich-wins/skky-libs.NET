using System;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class StringObject
	{
		public StringObject()
		{ }

		public StringObject(string sValue, object oValue)
		{
			stringValue = sValue;
			objValue = oValue;
		}

		[DataMember]
		public string stringValue { get; set; }

		[DataMember]
		public object objValue { get; set; }
	}
}
