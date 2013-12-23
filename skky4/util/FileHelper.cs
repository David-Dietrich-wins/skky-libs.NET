using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace skky.util
{
	public static class FileHelper
	{
		public const string CONST_DefaultTempDirectory = "C:\\temp";
		public const string Const_DefaultFileForStrings = CONST_DefaultTempDirectory + "\\temp.txt";
		public const string Const_DefaultFileForHtmlStrings = CONST_DefaultTempDirectory + "\\temp.htm";
		public const string Const_DefaultFileForXmlStrings = CONST_DefaultTempDirectory + "\\temp.xml";

		﻿public static string GetContentType(string ext = null)
		{
			switch ((ext ?? string.Empty).Replace(".", "").ToLower())
			{
				case "gif":
					return "image/gif";
				case "jpg":
					return "image/jpeg";
				case "bmp":
					return "image/bmp";
				case "png":
					return "image/png";
			}

			return string.Empty;
		}
		 
		public static string GetContentType(string path = null, string ext = null)
		{
			if (ext == null && path == null)
				return string.Empty;

			if (ext == null && path != null)
				ext = System.IO.Path.GetExtension(path);

			return GetContentType(ext);
		}

		/// <summary>
		/// Used to Create a directory path if needed.
		/// Verifies that the directory exists first, if not then one is created.
		/// </summary>
		/// <param name="path">The full path name of the directory to create.</param>
		/// <returns>True or False indicator of whether the directory was created or not.</returns>
		public static bool CreateDirectoryPath(string path)
		{
			bool created = false;
			if (!Directory.Exists(path))
			{
				created = true;
				Directory.CreateDirectory(path);
			}

			return created;
		}

		public static string getFileDateStyle(DateTime? dateTime = null)
		{
			string fileDateStyle = string.Empty;
		
			DateTime dt = (dateTime.HasValue ? dateTime.Value : DateTime.Now);
			int year = (dt.Year % 1000);
			int month = dt.Month;
			int day = dt.Day;

			fileDateStyle += year.ToString().PadLeft(2, '0');
			fileDateStyle += month.ToString().PadLeft(2, '0');
			fileDateStyle += day.ToString().PadLeft(2, '0');

			return fileDateStyle;
		}
		public static string getFileDateTimeStyle(DateTime? dateTime = null)
		{
			string fileDateStyle = string.Empty;

			DateTime dt = (dateTime.HasValue ? dateTime.Value : DateTime.Now);
			int year = (dt.Year % 1000);
			int month = dt.Month;
			int day = dt.Day;
			int hour = dt.Hour;
			int min = dt.Minute;
			int sec = dt.Second;

			fileDateStyle += year.ToString().PadLeft(2, '0');
			fileDateStyle += month.ToString().PadLeft(2, '0');
			fileDateStyle += day.ToString().PadLeft(2, '0');
			fileDateStyle += hour.ToString().PadLeft(2, '0');
			fileDateStyle += min.ToString().PadLeft(2, '0');
			fileDateStyle += sec.ToString().PadLeft(2, '0');

			return fileDateStyle;
		}
		public static string getFileDateTimeWithGuid(string extension = null, DateTime? dateTime = null)
		{
			string str = getFileDateTimeStyle(dateTime);
			str += ".";
			str += Guid.NewGuid().ToString();
			str += ".";
			str += (string.IsNullOrEmpty(extension) ? "txt" : extension);

			return str;
		}
		public static string getFileNameWithDateTime(string prefix, string extension = null, DateTime? dateTime = null)
		{
			string str = prefix ?? string.Empty;
			if (!string.IsNullOrEmpty(str))
				str += ".";
			str += getFileDateTimeStyle(dateTime);
			str += ".";
			str += (string.IsNullOrEmpty(extension) ? "txt" : extension);

			return str;
		}

		#region Extension methods for writing to Temporary files.
		public static long WriteToTempFile(this string str)
		{
			return WriteToFile(str, Const_DefaultFileForStrings);
		}
		public static long WriteToTempHtmlFile(this string str)
		{
			return WriteToFile(str, Const_DefaultFileForHtmlStrings);
		}
		public static long WriteToTempXmlFile(this string str)
		{
			return WriteToFile(str, Const_DefaultFileForXmlStrings);
		}
		public static long WriteToFile(this string str, string fileName)
		{
			if (!string.IsNullOrEmpty(fileName))
				return SaveToDisk(str, fileName);

			return 0L;
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

			//Copy to a temp file first so that if anything goes wrong with the network
			//while downloading the file, it does not actually update the real file  on the disk.
			//This essentially gives us transaction like semantics.
			Random rand = new Random();
			string tempPath = Environment.GetEnvironmentVariable("temp") + "\\";
			tempPath += filePath.Remove(0, filePath.LastIndexOf("\\") + 1);
			tempPath += rand.Next(10000).ToString() + ".tmp";

			FileStream fs = File.Open(tempPath, FileMode.Create, FileAccess.ReadWrite);

			int length = stream.Read(buffer, 0, 4096);
			while (length > 0)
			{
				fs.Write(buffer, 0, length);
				length = stream.Read(buffer, 0, 4096);
			}
			fs.Close();

			// If the file that we need to write exists, delete it first.
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			// Perform a copy and a delete because on XP there were permission issues
			// specifically around inheritable permissions in the destination folder.
			File.Copy(tempPath, filePath, true);
			File.Delete(tempPath);
		}
		public static long SaveToDisk(string str, string filePath)
		{
			long totalLength = 0;
			if (!string.IsNullOrEmpty(str))
			{
				byte[] buffer = new byte[4096];

				//Copy to a temp file first so that if anything goes wrong with the network
				//while downloading the file, it does not actually update the real file  on the disk.
				//This essentially gives us transaction like semantics.

				FileStream fs = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite);
				MemoryStream stream = new MemoryStream((str ?? string.Empty).Encode(0));
				int length = stream.Read(buffer, 0, 4096);
				totalLength = length;
				while (length > 0)
				{
					fs.Write(buffer, 0, length);
					length = stream.Read(buffer, 0, 4096);
					totalLength += length;
				}

				fs.Close();
			}

			return totalLength;
		}
		public static long SaveToDiskInDateDirectory(string str, string filePath = null, string fileName = null)
		{
			DateTime dt = DateTime.Now;
			string dirToSaveIn = Path.Combine(filePath ?? CONST_DefaultTempDirectory, FileHelper.getFileDateStyle(dt));
			if (!Directory.Exists(dirToSaveIn))
				Directory.CreateDirectory(dirToSaveIn);

			if (string.IsNullOrEmpty(fileName))
			{
				fileName = FileHelper.getFileDateTimeWithGuid("txt", dt);
			}
			// Make the full file path.
			string exportFileNameWithPath = Path.Combine(dirToSaveIn, fileName);

			return SaveToDisk(str, exportFileNameWithPath);
		}
		#endregion
	}
}
