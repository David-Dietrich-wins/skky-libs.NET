using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class idName
	{
		[DataMember]
		public int id;

		[DataMember]
		public string name;

		public idName()
		{ }

		public idName(int theid, string theName)
		{
			id = theid;
			name = theName;
		}
	}
}
