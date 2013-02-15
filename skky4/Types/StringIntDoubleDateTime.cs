using System;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class StringIntDoubleDateTime: StringIntDouble
	{
		public StringIntDoubleDateTime()
		{ }

		public StringIntDoubleDateTime(string sValue, int iValue, double dValue, DateTime dtValue)
			: base(sValue, iValue, dValue)
		{
			dateTimeValue = dtValue;
		}

		[DataMember]
		public DateTime dateTimeValue { get; set; }
	}
}
