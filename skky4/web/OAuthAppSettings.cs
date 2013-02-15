using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.web
{
	public class OAuthAppSettings
	{
		public string ApplicationID { get; set; }
		public string ApplicationKey { get; set; }
		public string ApplicationDomain { get; set; }
		public string SharedSecret { get; set; }
	}
}
