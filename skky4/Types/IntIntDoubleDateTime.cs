using System;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class IntIntDoubleDateTime
	{
		public IntIntDoubleDateTime()
		{ }

		public IntIntDoubleDateTime(int iValue, int i2Value, double dValue, DateTime dtValue)
		{
			intValue = iValue;
			int2Value = i2Value;
			dValue = doubleValue;
			dateTimeValue = dtValue;
		}

		[DataMember]
		public int intValue { get; set; }

		[DataMember]
		public int int2Value { get; set; }

		[DataMember]
		public double doubleValue { get; set; }

		[DataMember]
		public DateTime dateTimeValue { get; set; }
	}
}
