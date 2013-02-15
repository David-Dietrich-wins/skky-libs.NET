using System;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class StringIntDouble : StringInt
	{
		public StringIntDouble()
		{ }

		public StringIntDouble(string sValue, int iValue, double dValue)
			: base(sValue, iValue)
		{
			doubleValue = dValue;
		}

		[DataMember]
		public double doubleValue { get; set; }
	}
}
