using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class GuidStringDouble : GuidString
	{
		public GuidStringDouble()
		{ }

		public GuidStringDouble(Guid gValue, string sValue, double dValue)
			: base(gValue, sValue)
		{
			doubleValue = dValue;
		}

		[DataMember]
		public double doubleValue { get; set; }
	}
}
