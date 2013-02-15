using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace skky.web
{
	[DataContract]
	public class PopupMenu
	{
		public PopupMenu()
		{ }

		[DataMember]
		public string variableName { get; set; }

		[DataMember]
		public string innerHTML { get; set; }
	}
}
