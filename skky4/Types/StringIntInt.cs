using System;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class StringIntInt : StringInt
	{
		public StringIntInt()
		{ }

		public StringIntInt(string sValue, int iValue, int i2Value)
			: base(sValue, iValue)
		{
			int2Value = i2Value;
		}

		[DataMember]
		public int int2Value { get; set; }
	}
}
