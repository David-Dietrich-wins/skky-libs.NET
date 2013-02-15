using System;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class StringDouble
	{
		public StringDouble()
		{ }

		public StringDouble(string sValue, double dValue)
		{
			stringValue = sValue;
			doubleValue = dValue;
		}

		[DataMember]
		public string stringValue { get; set; }

		[DataMember]
		public double doubleValue { get; set; }
	}
}
