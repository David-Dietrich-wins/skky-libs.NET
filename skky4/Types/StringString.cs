using System;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class StringString
	{
		public StringString()
		{ }

		public StringString(string sValue, string s2Value)
		{
			stringValue = sValue;
			string2Value = s2Value;
		}

		[DataMember]
		public string stringValue { get; set; }

		[DataMember]
		public string string2Value { get; set; }
	}
}
