using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace skky.util
{
	static public class StreamHelper
	{
		/// <summary>The default size for buffers.</summary>
		public const int Const_BufferSize = 65536;

		static public string GetString(Stream stream)
		{
			return GetString(stream, 0);
		}
		static public string GetStringWithoutEncoding(Stream stream)
		{
			if (stream != null)
			{
				using (StreamReader sr = new StreamReader(stream))
					return sr.ReadToEnd();
			}

			return string.Empty;
		}
		static public string GetString(Stream stream, int encoding)
		{
			if (stream != null)
			{
				Encoding enc = System.Text.Encoding.GetEncoding(encoding);
				try
				{
					using(StreamReader sr = new StreamReader(stream, enc))
						return sr.ReadToEnd();
				}
				finally
				{ }
			}

			return string.Empty;
		}

		static public MemoryStream GetMemoryStream(Stream stream, bool closeStream)
		{
			return GetMemoryStream(stream, closeStream, Const_BufferSize);
		}
		static public MemoryStream GetMemoryStreamFlush(Stream stream, int len)
		{
			MemoryStream ms = new MemoryStream(len);

			if (stream != null && len > 0)
			{
				StreamWriter w = new StreamWriter(ms);
				string content = GetString(stream);
				w.Write(content);
				w.Flush();

				ms.Seek(0, SeekOrigin.Begin);
				//ms.Position = 0; // rewind stream for reading
			}

			return ms;
		}

		static public MemoryStream GetMemoryStream(Stream stream, bool closeStream, int bufferSize)
		{
			MemoryStream ms = new MemoryStream();

			if (stream != null)
			{
				try
				{
					if (bufferSize < 1)
						bufferSize = Const_BufferSize;

					// Get the response into a byte array.
					byte[] bytes = new byte[bufferSize];
					int numBytes;
					while ((numBytes = stream.Read(bytes, 0, bufferSize)) > 0)
					{
						ms.Write(bytes, 0, numBytes);
					}
				}
				catch (Exception)
				{
					throw;
				}
				finally
				{
					if (closeStream)
						stream.Close();
				}
			}

			return ms;
		}

		static public long WriteStringEncoded(Stream stream, string str)
		{
			return WriteStringEncoded(stream, str, 0, true);
		}
		static public long WriteStringEncoded(Stream stream, string str, int encoding, bool closeStream)
		{
			long length = 0;
			byte[] buffer = str.Encode(encoding);
			if (buffer != null)
			{
				length += buffer.Length;
				if (stream != null)
				{
					try
					{
						stream.Write(buffer, 0, buffer.Length);
					}
					finally
					{
						if (closeStream)
							stream.Close();
					}
				}
			}

			return length;
		}

		#region DownloadFile
		/// <summary>
		/// Downloads the file at the specified uri and saves it at the specified
		/// filepath. The filepath must include the name.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="filePath"></param>
		public static void DownloadFile(string url, string filePath)
		{
			HttpWebResponse Response;

			//Retrieve the File
			HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(url);
			Request.Headers.Add("Translate: f");
			Request.Credentials = CredentialCache.DefaultCredentials;
			try
			{
				Response = (HttpWebResponse)Request.GetResponse();
			}
			catch (WebException e)
			{
				// Check to see if the remote host return a response
				if (e.Response != null)
				{
					e.Response.Close();
				}
				Debug.WriteLine("Error accessing Url " + url);
				// EventLogUtil.WriteToLog(Assembly.GetExecutingAssembly().ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + " encountered an error. " + e.Message, EventLogEntryType.Error);
				throw;
			}

			Stream respStream = null;

			try
			{
				respStream = Response.GetResponseStream();
				// Get the file saved to the destination file path.
				FileHelper.CopyStreamToDisk(respStream, filePath);
			}
			catch (Exception e)
			{
				Debug.WriteLine("Error writing to:  " + filePath + e.Message);
				//TODO: Add Trace Logging
				// EventLogUtil.WriteToLog(Assembly.GetExecutingAssembly().ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + " encountered an error." + e.Message, EventLogEntryType.Error);
				throw;
			}
			finally
			{
				if (respStream != null)
				{
					respStream.Close();
				}
				if (Response != null)
				{
					Response.Close();
				}
			}
		}
		#endregion
	}
}
