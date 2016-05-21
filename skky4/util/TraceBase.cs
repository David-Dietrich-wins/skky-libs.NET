using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Reflection;

namespace skky.util
{
	public abstract class TraceBase : ITrace, IDisposable
	{
		#region Constructors

		protected TraceBase()
		{
			sourceID = null;
			sourceIDString = "";
		}

		~TraceBase()
		{
			Dispose(false);
		}

		protected TraceBase(string sourceID)
		{
			this.sourceID = sourceID;
			sourceIDString = GetSourceString(sourceID);
		}

		protected TraceBase(Uri sourceUrl)
			: this(UrlToSourceID(sourceUrl))
		{
		}

		#endregion

		#region Fields

		protected TraceSource traceSource;
		protected String sourceID;
		protected String sourceIDString;
		private static int eventID; // tracked across instances
		private static Stack<int> suspendedEventIDs = new Stack<int>();
		public abstract string Name { get; }

		#endregion

		#region ITrace Members

		#region Information

		public void Information(string message)
		{
			if (Source.Switch.ShouldTrace(TraceEventType.Information))
				Source.TraceEvent(TraceEventType.Information, eventID, sourceIDString + message);
		}

		public void Information(string format, params object[] args)
		{
			if (Source.Switch.ShouldTrace(TraceEventType.Information))
				Source.TraceEvent(TraceEventType.Information, eventID, sourceIDString + format, args);
		}

		#endregion

		#region Verbose
		public void Verbose(string message)
		{
			if (Source.Switch.ShouldTrace(TraceEventType.Verbose))
				Source.TraceEvent(TraceEventType.Verbose, eventID, sourceIDString + message);
		}

		public void Verbose(string format, params object[] args)
		{
			if (Source.Switch.ShouldTrace(TraceEventType.Verbose))
				Source.TraceEvent(TraceEventType.Verbose, eventID, sourceIDString + format, args);
		}
		#endregion

		#region Error
		public void Error(string message)
		{
			if (Source.Switch.ShouldTrace(TraceEventType.Error))
				Source.TraceEvent(TraceEventType.Error, eventID, sourceIDString + message);
			//totalExceptionsCounter.Increment();
			//exceptionsPerSecCounter.Increment();
		}

		public void Error(string format, params object[] args)
		{
			if (Source.Switch.ShouldTrace(TraceEventType.Error))
				Source.TraceEvent(TraceEventType.Error, eventID, sourceIDString + format, args);
			//totalExceptionsCounter.Increment();
			//exceptionsPerSecCounter.Increment();
		}

		public void Error(Exception exception)
		{
			if (Source.Switch.ShouldTrace(TraceEventType.Error))
				Source.TraceData(TraceEventType.Error, eventID, new TraceExceptionMessage(sourceID, exception));
			//totalExceptionsCounter.Increment();
			//exceptionsPerSecCounter.Increment();
		}
		#endregion

		#region Warning
		public void Warning(string message)
		{
			if (Source.Switch.ShouldTrace(TraceEventType.Warning))
				Source.TraceEvent(TraceEventType.Warning, eventID, sourceIDString + message);
		}

		public void Warning(string format, params object[] args)
		{
			if (Source.Switch.ShouldTrace(TraceEventType.Warning))
				Source.TraceEvent(TraceEventType.Warning, eventID, sourceIDString + format, args);
		}

		public void Warning(Exception exception)
		{
			if (Source.Switch.ShouldTrace(TraceEventType.Warning))
				Source.TraceData(TraceEventType.Warning, eventID, new TraceExceptionMessage(sourceID, exception));
		}
		#endregion

		#region Critical
		public void Critical(string message)
		{
			if (Source.Switch.ShouldTrace(TraceEventType.Critical))
				Source.TraceEvent(TraceEventType.Critical, eventID, sourceIDString + message);
		}

		public void Critical(string format, params object[] args)
		{
			if (Source.Switch.ShouldTrace(TraceEventType.Critical))
				Source.TraceEvent(TraceEventType.Critical, eventID, sourceIDString + format, args);
		}

		public void Critical(Exception exception)
		{
			if (Source.Switch.ShouldTrace(TraceEventType.Critical))
				Source.TraceData(TraceEventType.Critical, eventID, new TraceExceptionMessage(sourceID, exception));
		}

		#endregion

		#region TraceData
		public void TraceData(TraceEventType eventType, object data)
		{
			if (Source.Switch.ShouldTrace(eventType))
				Source.TraceData(eventType, eventID, data);
		}

		public void TraceData(TraceEventType eventType, params object[] data)
		{
			if (Source.Switch.ShouldTrace(eventType))
				Source.TraceData(eventType, eventID, data);
		}
		#endregion

		#region TraceEvent
		public void TraceEvent(TraceEventType eventType)
		{
			if (Source.Switch.ShouldTrace(eventType))
				Source.TraceEvent(eventType, eventID);
		}

		public void TraceEvent(TraceEventType eventType, string message)
		{
			if (Source.Switch.ShouldTrace(eventType))
				Source.TraceEvent(eventType, eventID, sourceIDString + message);
		}

		public void TraceEvent(TraceEventType eventType, string format, params object[] args)
		{
			if (Source.Switch.ShouldTrace(eventType))
				Source.TraceEvent(eventType, eventID, sourceIDString + format, args);
		}
		#endregion

		//TODO: Find a better way to support this functionality, using the id in this manner is incorrect.
		// ID should only be used to correlate related trace events
		#region TraceMail
		/// <summary>
		/// Special purpose trace event for sending thru email
		/// </summary>
		/// <param name="eventType"></param>
		/// <param name="message"></param>
		public void TraceMail(TraceEventType eventType, string message)
		{
			if (Source.Switch.ShouldTrace(eventType))
				Source.TraceEvent(eventType, -1, message);
		}

		/// <summary>
		/// Special purpose trace event for sending thru email
		/// </summary>
		/// <param name="eventType"></param>
		/// <param name="message"></param>
		public void TraceMail(TraceEventType eventType, string format, params object[] args)
		{
			if (Source.Switch.ShouldTrace(eventType))
				Source.TraceEvent(eventType, -1, format, args);
		}
		#endregion

		#region TraceTransfer
		public void TraceTransfer(string message, Guid relatedActivityId)
		{
			if (Source.Switch.ShouldTrace(TraceEventType.Transfer))
				Source.TraceTransfer(eventID, sourceIDString + message, relatedActivityId);
		}
		#endregion

		#region Start
		public void Start(string message)
		{
			Interlocked.Increment(ref eventID);

			if (Source.Switch.ShouldTrace(TraceEventType.Start))
				Source.TraceEvent(TraceEventType.Start, eventID, sourceIDString + message);
		}

		public void Start(string format, params object[] data)
		{
			Interlocked.Increment(ref eventID);

			if (Source.Switch.ShouldTrace(TraceEventType.Start))
				Source.TraceEvent(TraceEventType.Start, eventID, sourceIDString + format, data);
		}
		#endregion

		#region Stop
		public void Stop(string message)
		{
			Interlocked.Decrement(ref eventID);

			if (Source.Switch.ShouldTrace(TraceEventType.Stop))
				Source.TraceEvent(TraceEventType.Stop, eventID, sourceIDString + message);
		}

		public void Stop(string format, params object[] data)
		{
			Interlocked.Decrement(ref eventID);

			if (Source.Switch.ShouldTrace(TraceEventType.Stop))
				Source.TraceEvent(TraceEventType.Stop, eventID, sourceIDString + format, data);
		}
		#endregion

		#region Suspend
		public void Suspend(string message)
		{
			suspendedEventIDs.Push(eventID);

			if (Source.Switch.ShouldTrace(TraceEventType.Suspend))
				Source.TraceEvent(TraceEventType.Suspend, eventID, sourceIDString + message);

			Interlocked.Decrement(ref eventID);
		}

		public void Suspend(string format, params object[] args)
		{
			suspendedEventIDs.Push(eventID);

			if (Source.Switch.ShouldTrace(TraceEventType.Suspend))
				Source.TraceEvent(TraceEventType.Suspend, eventID, sourceIDString + format, args);

			Interlocked.Decrement(ref eventID);
		}
		#endregion

		#region Resume
		public void Resume(string message)
		{
			if (suspendedEventIDs.Count > 0)
			{
				eventID = suspendedEventIDs.Pop();

				if (Source.Switch.ShouldTrace(TraceEventType.Resume))
					Source.TraceEvent(TraceEventType.Resume, eventID, sourceIDString + message);
			}
		}

		public void Resume(string format, params object[] args)
		{
			if (suspendedEventIDs.Count > 0)
			{
				eventID = suspendedEventIDs.Pop();

				if (Source.Switch.ShouldTrace(TraceEventType.Resume))
					Source.TraceEvent(TraceEventType.Resume, eventID, sourceIDString + format, args);
			}
		}
		#endregion

		#region TraceSource

		public TraceSource Source
		{
			get
			{
				if (traceSource == null)
					traceSource = new TraceSource(Name);
				return traceSource;
			}
		}

		#endregion


		#region IsVerboseEnabled
		public bool IsVerboseEnabled
		{
			get { return Source.Switch.ShouldTrace(TraceEventType.Verbose); }
		}
		#endregion

		#endregion

		#region Support

		/// <summary>
		/// Encode the source ID string.
		/// </summary>
		/// <param name="sourceID">A unique identifier for a source.</param>
		/// <returns>The encoded string.</returns>
		public static String GetSourceString(String sourceID)
		{
			return ("[" + sourceID + "]");
		}

		/// <summary>
		/// Encode a message with the specified source destination.
		/// </summary>
		/// <param name="sourceID">The unique identifier for the</param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static String EncodeSourceMessage(String sourceID, String message)
		{
			return GetSourceString(sourceID) + message;
		}

		public static String UrlToSourceID(Uri sourceUrl)
		{
			String sourceID = sourceUrl.Host;
			if (String.IsNullOrEmpty(sourceID))
			{
				return null;
			}
			if (sourceID.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
			{
				sourceID = sourceID.Substring(4);
			}
			int dot = sourceID.LastIndexOf('.');
			sourceID = sourceID.Substring(0, dot);
			return sourceID;
		}

		#endregion

		#region IDisposable Members
		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !disposed)
			{
				lock (traceSource)
				{
					traceSource.Flush();
					traceSource.Close();
					disposed = true;
				}
			}
		}

		protected bool disposed;
		#endregion
	}

	public class SourceIDTrace : TraceBase
	{
		#region Fields

		private static Dictionary<String, SourceIDTrace> sourceTraceList = new Dictionary<String, SourceIDTrace>();
		private string sourceName;

		#endregion

		#region Properties

		public override string Name
		{
			get
			{
				return sourceName;
			}
		}

		#endregion

		#region Constructors

		public SourceIDTrace(String sourceID)
			: this(Assembly.GetExecutingAssembly().GetName().Name, sourceID)
		{ }

		public SourceIDTrace(String sourceName, String sourceID)
			: base(sourceID)
		{
			this.sourceName = sourceName;
		}

		public SourceIDTrace(Uri sourceUrl)
			: this(TraceBase.UrlToSourceID(sourceUrl))
		{ }

		public SourceIDTrace(String sourceName, Uri sourceUrl)
			: this(sourceName, TraceBase.UrlToSourceID(sourceUrl))
		{ }

		#endregion

		#region Static Methods

		/// <summary>
		/// Gets a trace object for a specified source name and sourceID.
		/// </summary>
		/// <param name="sourceName">The tracing source name.</param>
		/// <param name="sourceID">The source identifier.</param>
		/// <returns>A source ID based tracing object.</returns>
		public static SourceIDTrace GetTrace(String sourceName, String sourceID)
		{
			String index = EncodeIndex(sourceName, sourceID);
			if (!sourceTraceList.ContainsKey(index))
			{
				sourceTraceList.Add(index, new SourceIDTrace(sourceName, sourceID));
			}
			return sourceTraceList[index];
		}

		public static void RemoveTrace(String sourceName, String sourceID)
		{
			sourceTraceList.Remove(EncodeIndex(sourceName, sourceID));
		}

		private static String EncodeIndex(String sourceName, String sourceID)
		{
			return (String.IsNullOrEmpty(sourceName) ? Assembly.GetExecutingAssembly().GetName().Name : sourceName) +
				   (String.IsNullOrEmpty(sourceID) ? "" : sourceID);
		}

		#endregion

	}

	public class TraceExceptionMessage
	{
		#region Fields

		public string sourceID;
		public Exception e;

		#endregion

		#region Constructors

		public TraceExceptionMessage(String sourceID, Exception e)
		{
			this.sourceID = sourceID;
			this.e = e;
		}

		#endregion

		#region Object Overrides

		public override string ToString()
		{
			return e.ToString();
		}

		#endregion
	}
}
