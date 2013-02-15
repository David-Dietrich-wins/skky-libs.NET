using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Conversions
{
	[DataContract]
	public class MeasureObject
	{
		[DataMember]
		public int id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string ShortName { get; set; }
	}
}
