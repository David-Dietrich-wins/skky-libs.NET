using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace skky.util
{
	public class Trace
	{
		//string methodName = System.Reflection.MethodInfo.GetCurrentMethod().DeclaringType.Name;	// For static methods
		//string methodName = MethodBase.GetCurrentMethod().Name;			// For object methods (this.)

		private static InternalTrace trace = new InternalTrace();

		private class InternalTrace : TraceBase
		{
			public override string Name
			{
				get { return Assembly.GetExecutingAssembly().GetName().Name; }
			}
		}

		public static string GetMethodMessage(string methodName, string msg)
		{
			string str = (methodName ?? "");
			if (!string.IsNullOrEmpty(str))
				str += ": ";

			str += (msg ?? "");

			return str;
		}
		public static string printMethodAndClassName(string className, string methodName)
		{
			string msg = string.Empty;
			if (!string.IsNullOrWhiteSpace(className))
			{
				msg += className;
				msg += ".";
			}

			msg += methodName ?? string.Empty;

			return msg;
		}

		public static string GetBreak(bool showAsHtml = false)
		{
			if (showAsHtml)
				return "<br />";

			return "\n";
		}

		public static string GetExceptionMessageString(Exception ex, bool showAsHtml = false)
		{
			string str = string.Empty;
			if (null != ex)
			{
				str = ex.Message;
				if (null != ex.InnerException)
				{
					if (showAsHtml)
						str += "<br />";
					else
						str += "\n";

					str += GetBreak(showAsHtml) + GetExceptionMessageString(ex.InnerException);
				}
			}

			return str;
		}

		#region Base
		public static ITrace Base
		{
			get { return trace; }
		}
		#endregion

		#region Method Name Wrappers
		public static void MethodInformation(string className, string methodName, string format, params object[] args)
		{
			string s = printMethodAndClassName(className, methodName);

			Information(GetMethodMessage(s, format), args);
		}

		public static string MethodInformation(string className, string methodName, string msg)
		{
			string s = printMethodAndClassName(className, methodName);

			string rmsg = GetMethodMessage(s, msg);
			Information(rmsg);

			return rmsg;
		}
		public static string MethodInformation(string methodName, string msg)
		{
			return MethodInformation(string.Empty, methodName, msg);
		}

		public static void MethodException(string className, string methodName, Exception ex, string format, params object[] args)
		{
			string s = GetMethodMessage(printMethodAndClassName(className, methodName), GetExceptionMessageString(ex) + format);

			Critical_Var(s, args);
		}
		public static string MethodException(string className, string methodName, Exception ex, string msg = "")
		{
			string s = printMethodAndClassName(className, methodName);

			return Critical(ex, GetMethodMessage(s, msg));
		}
		public static string MethodException(string methodName, Exception ex, string msg = "")
		{
			return MethodException(string.Empty, methodName, ex, msg);
		}
		#endregion

		#region Information
		public static void Information(string message)
		{
			trace.Information(message);
		}

		public static void Information(string format, params object[] args)
		{
			trace.Information(format, args);
		}
		#endregion

		#region Verbose
		public static void Verbose(string message)
		{
			trace.Verbose(message);
		}

		public static void Verbose(string format, params object[] args)
		{
			trace.Verbose(format, args);
		}
		#endregion

		#region Error
		public static void Error(string message)
		{
			trace.Error(message);
		}

		public static void Error(string format, params object[] args)
		{
			trace.Error(format, args);
		}

		public static void Error(Exception exception)
		{
			trace.Error(exception);
		}
		#endregion

		#region Warning
		public static void Warning(string message)
		{
			trace.Warning(message);
		}

		public static void Warning(string format, params object[] args)
		{
			trace.Warning(format, args);
		}

		public static void Warning(Exception exception)
		{
			trace.Warning(exception);
		}
		public static void Warning(string msg, Exception exception)
		{
			trace.Warning((msg ?? "") + ": " + exception.ToString());
		}
		#endregion

		#region Critical
		public static void Critical(string message)
		{
			trace.Critical(message);
		}

		public static void Critical_Var(string format, params object[] args)
		{
			trace.Critical(format, args);
		}

		public static string Critical(Exception exception, string message = "")
		{
			string s = string.Empty;
			if (!string.IsNullOrEmpty(message))
				s = message + ": ";

			s += GetExceptionMessageString(exception);

			trace.Critical(exception);
			trace.Critical(s);

			return s;
		}
		#endregion

		#region TraceData
		public static void TraceData(TraceEventType eventType, object data)
		{
			trace.TraceData(eventType, data);
		}

		public static void TraceData(TraceEventType eventType, params object[] data)
		{
			trace.TraceData(eventType, data);
		}
		#endregion

		#region TraceEvent
		public static void TraceEvent(TraceEventType eventType)
		{
			trace.TraceEvent(eventType);
		}

		public static void TraceEvent(TraceEventType eventType, string message)
		{
			trace.TraceEvent(eventType, message);
		}

		public static void TraceEvent(TraceEventType eventType, string format, params object[] data)
		{
			trace.TraceEvent(eventType, format, data);
		}

		#endregion

		#region Start
		public static void Start(string message)
		{
			trace.Start(message);
		}

		public static void Start(string format, params object[] args)
		{
			trace.Start(format, args);
		}
		#endregion

		#region Stop
		public static void Stop(string message)
		{
			trace.Stop(message);
		}

		public static void Stop(string format, params object[] args)
		{
			trace.Stop(format, args);
		}
		#endregion

		#region TraceTransfer
		public static void TraceTransfer(string message, Guid relatedActivityId)
		{
			trace.TraceTransfer(message, relatedActivityId);
		}
		#endregion

		#region Utility Methods
		private void AddTraceOptionAllListeners(TraceSource trace, TraceOptions options)
		{
			foreach (TraceListener listener in trace.Listeners)
			{
				string listenerTypeName = listener.GetType().FullName;
				listener.TraceOutputOptions |= options;
			}
		}

		private void RemoveTraceOptionsAllListeners(TraceSource trace)
		{
			foreach (TraceListener listener in trace.Listeners)
				listener.TraceOutputOptions = TraceOptions.None;
		}

		public static bool IsVerboseEnabled
		{
			get { return trace.IsVerboseEnabled; }
		}
		#endregion
	}
}
