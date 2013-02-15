using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.Types;

namespace skky.EmissionsCalc
{
	[DataContract]
	public class Consumption
	{
		[DataMember]
		public string fuel { get; set; }

		[DataMember]
		public Namer rate { get; set; }

		[DataMember]
		public Namer volume { get; set; }

		[DataMember]
		public Namer time { get; set; }
	}
}
