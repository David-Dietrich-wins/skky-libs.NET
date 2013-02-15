using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace skky.util
{
	public static class Parser
	{
		public const string sClassName = "Parser";

		public static string[] SplitAndTrimString(string str, char splitChar = ',')
		{
			string[] ret = null;
			if (!string.IsNullOrEmpty(str))
			{
				ret = str.Split(',');
				if (null != ret)
				{
					for (int i = 0; i < ret.Length; ++i)
						ret[i] = ret[i].Trim();
				}
			}

			if (null == ret)
				ret = new string[0];

			return ret;
		}
	}
}
