using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Management;

namespace skkyWeb.util
{
	public class WebError : WebErrorEvent
	{
		public WebError(string customerName, string user, string msg, object eventSource, Exception ex)
			: this(customerName, user, msg, eventSource, 0, ex)
		{ }
		public WebError(string customerName, string user, string msg, object eventSource, int eventCode, Exception ex)
			: base((customerName ?? "Unknown Customer") + " : " + (user ?? "Unknown User") + " generated message " + (msg ?? "No Message"), eventSource, WebEvent.WebErrorCodeBase + eventCode, ex)
		{ }

		public WebError(string user, string msg, object eventSource, Exception ex)
			: this(user, msg, eventSource, 0, ex)
		{ }
		public WebError(string user, string msg, object eventSource, int eventCode, Exception ex)
			: base((user ?? "Unknown User") + " generated message " + (msg ?? "No Message"), eventSource, WebEvent.WebErrorCodeBase + eventCode, ex)
		{ }
	}

	public class ChartErrorEvent : WebErrorEvent
	{
		public ChartErrorEvent(string chartName, string msg, object eventSource, Exception e)
			: this(null, null, chartName, msg, eventSource, 0, e)
		{ }
		public ChartErrorEvent(string customerName, string user, string chartName, string msg
					, object eventSource, Exception e)
			: this(customerName, user, chartName, msg, eventSource, 0, e)
		{ }

		public ChartErrorEvent(string customerName, string user, string chartName, string msg
					, object eventSource, int eventCode, Exception e)
			: base((customerName ?? "Unknown Customer") + " : " + (user ?? "Unknown User") + " generated  " + (chartName ?? "Unknown Chart") + " chart error. " + msg
			, eventSource, WebEvent.ChartErrorEventCodeBase + eventCode, e)
		{ }
	}
}
