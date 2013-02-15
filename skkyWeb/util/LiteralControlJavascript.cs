using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace skkyWeb.util
{
	public class LiteralControlJavaScript : LiteralControl
	{
		public LiteralControlJavaScript(string content)
		{
			this.Text = GetWrappedScript(content);
		}

		public static string GetWrappedScript(string content)
		{
			if (string.IsNullOrEmpty(content))
				content = string.Empty;

			if (content.IndexOf("<script>") == -1)
				content = "<script language=\"javascript\" type=\"text/javascript\">" + content + "</script>";

			return content;
		}
		public static Control Add(Control parent, string content)
		{
			return parent.AddChild(string.IsNullOrEmpty(content) ? null : new LiteralControlJavaScript(content));
		}
	}
}
