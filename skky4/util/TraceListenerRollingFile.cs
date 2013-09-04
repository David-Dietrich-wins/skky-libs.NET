using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Globalization;

namespace skky.util
{
	public sealed class TraceListenerRollingFile : TraceListener, IDisposable
	{
		#region fields

		private const string DEFAULT_LOG = "DEFAULT";

		private string path = AppDomain.CurrentDomain.BaseDirectory;
		private static long fileNameCheckSeconds = 10;
		private static string recordDelimeter = String.Empty;
		private static string fieldDelimeter = "\t";
		private static string subFieldDelimeter = ", ";
		private static string dateTimeFormatStyle = "h:mm:ss tt"; // t = 3:32:14 PM

		//		private string fileNamePattern = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName + "_{yyyyMMdd_HH}.log";
		//		private string rolledFileName = string.Empty;
		//		private long fileNameCheckTicks = 0;
		//		internal TextWriter writer;

		public string genericLogPattern;
		//        private LogFileData genericLogFile = new LogFileData(null);
		private Dictionary<String, LogFileData> logFileList = new Dictionary<string, LogFileData>();

		#endregion

		#region Properties

		#region Configuration Settings
		/// <summary>
		/// Reads the "RollingFileTraceListener.FilesToKeep" setting in configuration, if not found defaults to 48.
		/// </summary>
		private static int FilesToKeep
		{
			get
			{
				if (filesToKeep == 0)
				{
					string setting = ConfigurationManager.AppSettings["TraceListenerRollingFile.FilesToKeep"];
					filesToKeep = !String.IsNullOrEmpty(setting) ? int.Parse(setting) : 48;
				}
				return filesToKeep;
			}
		}
		private static int filesToKeep;

		//TODO: there's some problem with this, where the writer has to get flushed every time.  So defaulting to true;
		/// <summary>
		/// Reads the "RollingFileTraceListener.AutoFlush" setting in configuration, if not found defaults to false.
		/// </summary>
		private static bool AutoFlush
		{
			get
			{
				if (autoFlush == null)
				{
					string setting = ConfigurationManager.AppSettings["TraceListenerRollingFile.AutoFlush"];
					autoFlush = !String.IsNullOrEmpty(setting) ? bool.Parse(setting) : true;
				}
				return autoFlush.Value;
			}
		}
		private static Nullable<bool> autoFlush = null;
		#endregion

		/*public TextWriter Writer
		{
			get { return writer; }
			set { writer = value; }
		}*/

		#endregion

		#region Constructors

		public TraceListenerRollingFile()
			: base("RollingFile")
		{ }

		public TraceListenerRollingFile(Stream stream) : this(stream, string.Empty) { }

		public TraceListenerRollingFile(TextWriter writer) : this(writer, string.Empty) { }

		public TraceListenerRollingFile(string initializeData) : this(initializeData, string.Empty) { }

		// this is the constructor called by the configuration system
		public TraceListenerRollingFile(string initializeData, string name)
			: base(name)
		{
			lock (typeof(TraceListenerRollingFile))
			{
				if (!string.IsNullOrEmpty(initializeData))
				{
					EnvironmentHelper.TryExpandEnvironmentVariables(initializeData, out initializeData);
					initializeData = initializeData.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

					string fullPath = string.Empty;

					if (initializeData.IndexOfAny(new char[] { Path.DirectorySeparatorChar }) > 0)
						if (!Path.IsPathRooted(initializeData))
							fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, initializeData);
						else
							fullPath = Path.GetFullPath(initializeData);

					FileInfo fileInfo = new FileInfo(fullPath);
					if (fileInfo.Directory.Exists)
					{
						path = fileInfo.Directory.FullName;
						genericLogPattern = fileInfo.Name;
					}
				}
				EnsureWriter(GetLogFile(DEFAULT_LOG));
			}
		}

		public TraceListenerRollingFile(Stream stream, string name)
			: base(name)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			GetLogFile(DEFAULT_LOG).writer = new StreamWriter(stream);
		}

		public TraceListenerRollingFile(TextWriter writer, string name)
			: base(name)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			GetLogFile(DEFAULT_LOG).writer = writer;

		}
		#endregion

		#region EnsureWriter

		internal bool EnsureWriter(LogFileData logFile)
		{
			bool writerIsGood = true;

			CheckRollOver(logFile);

			if (logFile.writer == null)
			{
				writerIsGood = false;

				if (logFile.rolledFileName == null) return writerIsGood;

				DeleteOldTraceFiles(logFile);
				writerIsGood = CreateTraceFile(logFile);

				if (!writerIsGood) logFile.fileNamePattern = null;
			}
			return writerIsGood;
		}

		private bool CreateTraceFile(LogFileData logFile)
		{
			bool writerIsGood = false;
			Encoding encoding = TraceListenerRollingFile.GetEncodingWithFallback(new UTF8Encoding(false));
			string fullPath = Path.Combine(logFile.GetPath(path), logFile.rolledFileName);
			string directoryName = Path.GetDirectoryName(fullPath);
			string fileName = Path.GetFileName(fullPath);

			// attempt to initialize the file twice
			for (int i = 0; i < 2; i++)
			{
				try
				{
					logFile.writer = new StreamWriter(fullPath, true, encoding, 0x1000);
					writerIsGood = true;
					break;
				}
				catch (IOException)
				{
					fileName = fileName + "." + Guid.NewGuid().ToString();
					fullPath = Path.Combine(directoryName, fileName);
				}
				catch (UnauthorizedAccessException)
				{
					break;
				}
				catch (Exception)
				{
					break;
				}
			}
			return writerIsGood;
		}

		private void CheckRollOver(LogFileData logFile)
		{
			// Check for filename roll-over, every 10 seconds
			if (logFile.fileNameCheckTicks < DateTime.Now.Ticks)
			{
				string newFileName = TraceListenerRollingFile.ExpandFormatPattern(logFile.fileNamePattern);

				if (logFile.rolledFileName != newFileName)
				{
					Close();
					logFile.rolledFileName = newFileName;
				}
				logFile.fileNameCheckTicks = DateTime.Now.Ticks + TimeSpan.FromSeconds(fileNameCheckSeconds).Ticks;
			}
		}

		private void DeleteOldTraceFiles(LogFileData logFile)
		{
			// the file list is sorted by the key (DateTime)
			SortedList<DateTime, FileInfo> files = new SortedList<DateTime, FileInfo>();

			FileInfo fileInfo = new FileInfo(Path.Combine(logFile.GetPath(path), logFile.rolledFileName));
			DirectoryInfo directoryInfo = new DirectoryInfo(fileInfo.DirectoryName);
			string wildcard = WildcardFormatPattern(logFile.fileNamePattern, "*").Replace(fileInfo.DirectoryName + "\\", string.Empty);
			string zeroFile = WildcardFormatPattern(logFile.fileNamePattern, "0").Replace(fileInfo.DirectoryName + "\\", string.Empty);
			FileInfo[] foundFiles = directoryInfo.GetFiles(wildcard, SearchOption.TopDirectoryOnly);

			foreach (FileInfo file in foundFiles)
			{
				if (!file.Name.Equals(zeroFile))
					files.Add(file.CreationTime, file);
			}

			// remove the oldest files, up to the retained count
			if (FilesToKeep < files.Count)
				for (int i = 0; i < files.Count - FilesToKeep; i++)
					files[files.Keys[i]].Delete();
		}

		private static Encoding GetEncodingWithFallback(Encoding encoding)
		{
			return encoding;
		}
		#endregion

		#region base class overrides

		#region TraceEvent
		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
		{
			TraceEvent(eventCache, source, eventType, id, string.Empty);
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{

			if (Filter == null || Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null))
			{
				try
				{
					string outMessage;
					LogFileData logFile;
					logFile = GetLogFile(message, out outMessage);
					WriteHeader(logFile, eventCache, source, eventType, id);
					WriteLine(logFile, outMessage);
					WriteFooter(logFile, eventCache);
				}
				catch { }
			}
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
		{
			if (Filter == null || Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null))
			{
				try
				{
					string outFormat;
					LogFileData logFile;
					logFile = GetLogFile(format, out outFormat);
					WriteHeader(logFile, eventCache, source, eventType, id);
					if (args != null)
						WriteLine(logFile, string.Format(CultureInfo.InvariantCulture, outFormat, args));
					else
						WriteLine(logFile, outFormat);
					WriteFooter(logFile, eventCache);
				}
				catch { }
			}
		}


		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			if (Filter == null || Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
			{
				try
				{

					if (data != null && data is Exception)
					{
						LogFileData logFile = GetLogFile(DEFAULT_LOG);
						Exception ex = (Exception)data;
						WriteHeader(logFile, eventCache, source, eventType, id);
						WriteLine(logFile, ex.Message);
						//					WriteLine("Type=" + data.GetType().ToString());
						WriteFooter(logFile, eventCache, ex);
					}
					else if (data is TraceExceptionMessage)
					{
						TraceExceptionMessage traceMessage = (TraceExceptionMessage)data;
						LogFileData logFile = GetLogFile(traceMessage.sourceID);
						WriteHeader(logFile, eventCache, source, eventType, id);
						WriteLine(logFile, traceMessage.e.Message);
						//					WriteLine("Type=" + data.GetType().ToString());
						WriteFooter(logFile, eventCache, traceMessage.e);
					}
					else
					{
						LogFileData logFile = GetLogFile(DEFAULT_LOG);
						WriteHeader(logFile, eventCache, source, eventType, id);
						WriteLine(logFile, data != null ? data.ToString() : string.Empty);
						WriteFooter(logFile, eventCache);
					}
				}
				catch { }
			}
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
		{
			if (Filter == null || Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, data))
			{
				try
				{
					LogFileData logFile = GetLogFile(DEFAULT_LOG);
					WriteHeader(logFile, eventCache, source, eventType, id);
					StringBuilder message = new StringBuilder();
					if (data != null)
					{
						for (int i = 0; i < data.Length; i++)
						{
							if (i != 0)
								message.Append(subFieldDelimeter);
							if (data[i] != null)
								message.Append(data[i].ToString());
						}
					}
					WriteLine(logFile, message.ToString());
					WriteFooter(logFile, eventCache);
				}
				catch { }
			}
		}

		#endregion

		private bool IsEnabled(TraceOptions traceOptions)
		{
			return ((traceOptions & TraceOutputOptions) != TraceOptions.None);
		}

		#region LineReader
		private class LineReader : IEnumerable<string>
		{
			public string Text { get; set; }

			public LineReader(string text)
			{
				Text = text;
			}

			public IEnumerator<string> GetEnumerator()
			{
				using (var reader = new StringReader(Text))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
						yield return line;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
		#endregion

		#region Log File Maintenance

		private LogFileData GetLogFile(String sourceID)
		{
			if (String.IsNullOrEmpty(sourceID))
			{
				sourceID = DEFAULT_LOG;
			}
			if (!logFileList.ContainsKey(sourceID))
			{
				if (sourceID.Equals(DEFAULT_LOG))
				{
					logFileList.Add(sourceID, new LogFileData(sourceID, genericLogPattern));
				}
				else
				{
					logFileList.Add(sourceID, new LogFileData(sourceID));
				}
			}
			return logFileList[sourceID];
		}

		private LogFileData GetLogFile(string message, out string outMessage)
		{
			string sourceID;
			if ((!String.IsNullOrEmpty(message)) && (message[0] == '[') && message.Contains(']'))
			{
				int end = message.IndexOf(']');
				sourceID = message.Substring(1, end - 1);
				outMessage = message.Substring(end + 1);
			}
			else
			{
				sourceID = DEFAULT_LOG;
				outMessage = message;
			}
			return GetLogFile(sourceID);
		}

		#endregion

		#region Write Methods

		public override void Write(string message)
		{
			string outMessage;
			LogFileData logFile;
			logFile = GetLogFile(message, out outMessage);
			Write(logFile, outMessage);
		}

		public override void WriteLine(string message)
		{
			string outMessage;
			LogFileData logFile;
			logFile = GetLogFile(message, out outMessage);
			WriteLine(logFile, outMessage);
		}

		public void Write(LogFileData logFile, string message)
		{
			if (EnsureWriter(logFile))
			{
				WriteIndent(logFile);
				logFile.writer.Write(message);
				if (AutoFlush) logFile.writer.Flush();
			}
		}

		public void WriteLine(LogFileData logFile, string message)
		{
			if (EnsureWriter(logFile))
			{
				WriteIndent(logFile);
				logFile.writer.WriteLine(message);
				if (AutoFlush) logFile.writer.Flush();
				base.NeedIndent = true;
			}
		}

		private void WriteIndent(LogFileData logFile)
		{
			if (NeedIndent)
			{
				StringBuilder buffer = new StringBuilder();
				buffer.Append(' ', IndentLevel * IndentSize);
				logFile.writer.Write(buffer.ToString());
				NeedIndent = false;
			}
		}

		#endregion

		#region WriteHeader

		private void WriteHeader(LogFileData logFile, TraceEventCache eventCache, string source, TraceEventType eventType, int id)
		{
			Write(logFile,
				String.Format("{0}{2}{1}[{3}]{1}{4,-11}{1}{5,-11}{1}"
				, recordDelimeter // 0
				, fieldDelimeter // 1
				, eventCache.DateTime.ToLocalTime().ToString(dateTimeFormatStyle, CultureInfo.InvariantCulture) // 2
				, id // 3
				, "(" + eventCache.ProcessId + ":" + eventCache.ThreadId + ")"// 4
				, eventType.ToString() // 5
			));
		}

		private void WriteFooter(LogFileData logFile, TraceEventCache eventCache)
		{
			if (eventCache != null)
			{
				IndentLevel++;

				if (IsEnabled(TraceOptions.ProcessId))
					WriteLine(logFile, "ProcessId=" + eventCache.ProcessId);

				if (IsEnabled(TraceOptions.LogicalOperationStack))
				{
					Write(logFile, "LogicalOperationStack=");
					Stack stack1 = eventCache.LogicalOperationStack;
					bool flag1 = true;

					foreach (object obj1 in stack1)
					{
						if (!flag1) Write(logFile, subFieldDelimeter);
						else flag1 = false;

						Write(logFile, obj1.ToString());
					}
					WriteLine(logFile, string.Empty);
				}
				if (IsEnabled(TraceOptions.ThreadId))
					WriteLine(logFile, "ThreadId=" + eventCache.ThreadId);
				if (IsEnabled(TraceOptions.Timestamp))
					WriteLine(logFile, "Timestamp=" + eventCache.Timestamp);
				if (IsEnabled(TraceOptions.Callstack))
					WriteLine(logFile, "Callstack=" + eventCache.Callstack);

				IndentLevel--;
			}
		}

		private void WriteFooter(LogFileData logFile, TraceEventCache eventCache, Exception exception)
		{
			WriteFooter(logFile, eventCache, exception, true);
		}


		private void WriteFooter(LogFileData logFile, TraceEventCache eventCache, Exception exception, bool printStack)
		{
			if (eventCache != null && exception != null)
			{
				IndentLevel++;

				WriteLine(logFile, "      Type=" + exception.GetType().ToString());

				if (printStack && !string.IsNullOrEmpty(exception.StackTrace))
					WriteStackTrace(logFile, exception);

				if (IsEnabled(TraceOptions.ProcessId))
					WriteLine(logFile, "  ProcessId=" + eventCache.ProcessId);

				if (printStack && this.IsEnabled(TraceOptions.LogicalOperationStack))
				{
					Write(logFile, "LogicalOperationStack=");
					Stack stack1 = eventCache.LogicalOperationStack;
					bool flag1 = true;

					foreach (object obj1 in stack1)
					{
						if (!flag1)
							Write(logFile, subFieldDelimeter);
						else
							flag1 = false;

						Write(logFile, obj1.ToString());
					}

					WriteLine(logFile, string.Empty);
				}

				if (IsEnabled(TraceOptions.ThreadId))
					WriteLine(logFile, "   ThreadId=" + eventCache.ThreadId);

				if (IsEnabled(TraceOptions.Timestamp))
					WriteLine(logFile, "  Timestamp=" + eventCache.Timestamp);

				if (exception.InnerException != null && !(exception is FileNotFoundException))
				{
					WriteLine(logFile, "Inner Exception=" + exception.InnerException.Message);
					WriteFooter(logFile, eventCache, exception.InnerException, false);
				}

				IndentLevel--;
			}
		}

		private void WriteStackTrace(LogFileData logFile, Exception exception)
		{
			if (exception != null && !string.IsNullOrEmpty(exception.StackTrace))
			{
				WriteLine(logFile, "StackTrace=");
				IndentLevel++;

				var lines = from line in new LineReader(exception.StackTrace)
							let inIndex = line.IndexOf(" in ")
							let methodName = (inIndex == -1 ? line : line.Substring(0, inIndex)).TrimStart()
							let path = inIndex == -1 ? null : line.Substring(inIndex, line.Length - inIndex).Remove(0, 4)
							let pathParts = path == null ? null : path.Split(':')
							let number = pathParts == null ? null : pathParts[2].Split(' ')[1]
							let filePath = pathParts == null ? null : path.Remove(path.LastIndexOf(':'))
							select new
							{
								MethodName = methodName,
								FilePath = filePath,
								Number = number,
							};

				foreach (var line in lines)
				{
					WriteLine(logFile, line.MethodName);
					if (!string.IsNullOrEmpty(line.FilePath))
					{
						IndentLevel++;
						WriteLine(logFile, "File: " + line.FilePath);
						WriteLine(logFile, "Line: " + line.Number);
						IndentLevel--;
					}
				}

				WriteLine(logFile, string.Empty);

				IndentLevel--;
			}
		}

		#endregion

		#region clean up

		public override void Close()
		{
			foreach (LogFileData logFile in logFileList.Values)
			{
				if (logFile.writer != null)
				{
					logFile.writer.Flush();
					logFile.writer.Close();
				}
				logFile.writer = null;
			}
		}

		public new void Dispose()
		{
			Console.WriteLine("RollingFileTraceListener.Dispose()");
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Flush();
				Close();
			}
		}

		public override void Flush()
		{
			foreach (LogFileData logFile in logFileList.Values)
			{
				if (EnsureWriter(logFile))
				{
					logFile.writer.Flush();
				}
			}
		}

		#endregion

		#endregion

		#region Filename DateTime Format expansion

		private static string ExtractFormatPattern(string data)
		{
			string pattern = string.Empty;

			if (!string.IsNullOrEmpty(data))
			{
				int start = data.IndexOf("{");
				int end = data.IndexOf("}");

				if (start > 0 && end > 0)
					pattern = data.Substring(start + 1, end - start - 1);
			}

			return pattern;
		}

		private static string WildcardFormatPattern(string data, string wildcard)
		{
			string pattern = ExtractFormatPattern(data);

			if (pattern.Length > 0)
				return data.Replace("{" + pattern + "}", wildcard);
			else
				return data;
		}

		private static string ExpandFormatPattern(string data)
		{
			string pattern = ExtractFormatPattern(data);

			if (pattern.Length > 0)
				return data.Replace("{" + pattern + "}", DateTime.Now.ToString(pattern));
			else
				return data;
		}
		#endregion

		#region LogFileData Class

		public class LogFileData
		{
			#region Fields

			public string fileNamePattern;
			public string rolledFileName = string.Empty;
			public long fileNameCheckTicks = 0;
			public TextWriter writer;
			public bool sourceFlag;

			#endregion

			#region Properties

			public bool IsSource
			{
				get
				{
					return sourceFlag;
				}
			}

			#endregion

			#region Constructors

			public LogFileData(String sourceID)
			{
				fileNamePattern = sourceID + "_{yyyyMMdd}.log";
				sourceFlag = true;
			}

			public LogFileData(String sourceID, String fileNamePattern)
			{
				this.fileNamePattern = fileNamePattern;
			}

			#endregion

			public String GetPath(string path)
			{
				return sourceFlag ? Path.Combine(path, "sources") : path;
			}
		}

		#endregion
	}
}