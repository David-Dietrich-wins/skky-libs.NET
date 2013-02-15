using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky
{
	public static class app
	{
		public static skky.web.config WebConfig;

		static app()
		{
			WebConfig = new skky.web.config();
		}
	}
}
