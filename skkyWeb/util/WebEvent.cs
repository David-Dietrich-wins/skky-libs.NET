using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Management;

namespace skkyWeb.util
{
	public class WebEvent : WebAuditEvent
	{
		public static readonly int WebEventCodeBase = WebEventCodes.WebExtendedBase + 1000;
		public static readonly int LogOnAuditEventCodeBase = WebEventCodes.WebExtendedBase + 2000;
		public static readonly int LogOffAuditEventCodeBase = WebEventCodes.WebExtendedBase + 3000;
		public static readonly int WebErrorCodeBase = WebEventCodes.WebExtendedBase + 4000;
		public static readonly int ChartErrorEventCodeBase = WebEventCodes.WebExtendedBase + 5000;

		public WebEvent(string message, object eventSource)
			: this(message, eventSource, 0)
		{ }
		public WebEvent(string message, object eventSource, int eventCode)
			: base(message, eventSource, WebEventCodeBase + eventCode)
		{ }
	}

	public class LogOnAuditEvent : WebAuditEvent
	{
		public LogOnAuditEvent(string message, object eventSource)
			: this(message, eventSource, 0)
		{ }
		public LogOnAuditEvent(string message, object eventSource, int eventCode)
			: base(message, eventSource, WebEvent.LogOnAuditEventCodeBase + eventCode)
		{ }

		public override void FormatCustomEventDetails(WebEventFormatter formatter)
		{
			formatter.AppendLine("");
			formatter.IndentationLevel += 1;
			formatter.AppendLine("*** Login Event Start ***");
			formatter.AppendLine(Message);
			formatter.AppendLine("*** Login Event End ***");
			formatter.IndentationLevel -= 1;
		}
	}

	public class LogOffAuditEvent : WebAuditEvent
	{
		public LogOffAuditEvent(string message, object eventSource)
			: this(message, eventSource, 0)
		{ }
		public LogOffAuditEvent(string message, object eventSource, int eventCode)
			: base(message, eventSource, WebEvent.LogOffAuditEventCodeBase + eventCode)
		{ }

		public override void FormatCustomEventDetails(WebEventFormatter formatter)
		{
			formatter.AppendLine("");
			formatter.IndentationLevel += 1;
			formatter.AppendLine("*** Logout Event Start ***");
			formatter.AppendLine(Message);
			formatter.AppendLine("*** Logout Event End ***");
			formatter.IndentationLevel -= 1;
		}
	}
}
