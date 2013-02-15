using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class GuidStringInt : GuidString
	{
		public GuidStringInt()
		{ }

		public GuidStringInt(Guid gValue, string sValue, int iValue)
			: base(gValue, sValue)
		{
			intValue = iValue;
		}

		[DataMember]
		public int intValue { get; set; }
	}
}
