using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace skky.util
{
	public interface ITrace
	{
		void Information(string message);
		void Information(string format, params object[] args);
		void Verbose(string message);
		void Verbose(string format, params object[] args);
		void Error(string message);
		void Error(string format, params object[] args);
		void Error(Exception exception);
		void Warning(string message);
		void Warning(string format, params object[] args);
		void Warning(Exception exception);
		void Critical(string message);
		void Critical(string format, params object[] args);
		void Critical(Exception exception);
		void Start(string message);
		void Start(string format, params object[] data);
		void Stop(string message);
		void Stop(string format, params object[] data);
		void TraceData(TraceEventType type, object data);
		void TraceData(TraceEventType type, params object[] data);
		void TraceEvent(TraceEventType type);
		void TraceEvent(TraceEventType type, string message);
		void TraceEvent(TraceEventType type, string format, params object[] data);
		void TraceTransfer(string message, Guid relatedActivityID);
		bool IsVerboseEnabled { get; }
		string Name { get; }
		TraceSource Source { get; }
	}
}
