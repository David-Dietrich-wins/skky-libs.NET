using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace skkyWeb.Security
{
    [DataContract]
    public class UserWebSettings : skky.Types.SkkyCallParams
	{
		public UserWebSettings()
		{
			SelectedSources = new List<string>();
		}

		[DataMember]
		public int PlaceHolder { get; set; }

		public void PreSave()
		{ }
	}
}
