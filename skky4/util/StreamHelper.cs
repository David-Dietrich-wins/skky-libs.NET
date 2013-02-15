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
			return GetString(stream, encoding, true);
		}
		static public string GetString(Stream stream, int encoding, bool closeStream)
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
				{
					if (closeStream)
						stream.Close();
				}
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
				throw e;
			}

			Stream respStream = null;

			try
			{
				respStream = Response.GetResponseStream();
				// Get the file saved to the destination file path.
				CopyStreamToDisk(respStream, filePath);
			}
			catch (Exception e)
			{
				Debug.WriteLine("Error writing to:  " + filePath + e.Message);
				//TODO: Add Trace Logging
				// EventLogUtil.WriteToLog(Assembly.GetExecutingAssembly().ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + " encountered an error." + e.Message, EventLogEntryType.Error);
				throw e;
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

		#region CopyStreamToDisk
		/// <summary>
		/// Creates a file from a stream by saving that stream-data to the specified filePath.
		/// </summary>
		/// <param name="stream">The stream containing the file.</param>
		/// <param name="filePath">The destination file path to create.</param>
		public static void CopyStreamToDisk(Stream stream, string filePath)
		{
			byte[] buffer = new byte[4096];
			int length;

			//Copy to a temp file first so that if anything goes wrong with the network
			//while downloading the file, it does not actually update the real file  on the disk.
			//This essentially gives us transaction like semantics.
			Random rand = new Random();
			string tempPath = Environment.GetEnvironmentVariable("temp") + "\\";
			tempPath += filePath.Remove(0, filePath.LastIndexOf("\\") + 1);
			tempPath += rand.Next(10000).ToString() + ".tmp";

			FileStream fs = System.IO.File.Open(tempPath, FileMode.Create, FileAccess.ReadWrite);

			length = stream.Read(buffer, 0, 4096);
			while (length > 0)
			{
				fs.Write(buffer, 0, length);
				length = stream.Read(buffer, 0, 4096);
			}
			fs.Close();

			// If the file that we need to write exists, delete it first.
			if (System.IO.File.Exists(filePath))
			{
				System.IO.File.Delete(filePath);
			}
			// Perform a copy and a delete because on XP there were permission issues
			// specifically around inheritable permissions in the destination folder.
			System.IO.File.Copy(tempPath, filePath, true);
			System.IO.File.Delete(tempPath);

		}
		public static long SaveToDisk(string str, string filePath)
		{
			byte[] buffer = new byte[4096];

			//Copy to a temp file first so that if anything goes wrong with the network
			//while downloading the file, it does not actually update the real file  on the disk.
			//This essentially gives us transaction like semantics.

			FileStream fs = System.IO.File.Open(filePath, FileMode.Create, FileAccess.ReadWrite);
			MemoryStream stream = new MemoryStream(str.Encode(0));
			int length = stream.Read(buffer, 0, 4096);
			long totalLength = length;
			while (length > 0)
			{
				fs.Write(buffer, 0, length);
				length = stream.Read(buffer, 0, 4096);
				totalLength += length;
			}

			fs.Close();

			return totalLength;
		}
		public static long SaveToDiskInDateDirectory(string str, string filePath = null, string fileName = null)
		{
			DateTime dt = DateTime.Now;
			string dirToSaveIn =  System.IO.Path.Combine(filePath ?? "c:\\temp", FileHelper.getFileDateStyle(dt));
			if (!Directory.Exists(dirToSaveIn))
				Directory.CreateDirectory(dirToSaveIn);

			if (string.IsNullOrEmpty(fileName))
			{
				fileName = FileHelper.getFileDateTimeWithGuid("txt", dt);
			}
			// Make the full file path.
			string exportFileNameWithPath = System.IO.Path.Combine(dirToSaveIn, fileName);

			return SaveToDisk(str, exportFileNameWithPath);
		}
		#endregion
	}
}
