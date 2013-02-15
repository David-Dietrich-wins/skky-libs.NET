using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class GuidStringIntDouble : GuidStringInt
	{
		public GuidStringIntDouble()
		{ }

		public GuidStringIntDouble(Guid gValue, string sValue, int iValue, double dValue)
			: base(gValue, sValue, iValue)
		{
			doubleValue = dValue;
		}

		[DataMember]
		public double doubleValue { get; set; }
	}
}
