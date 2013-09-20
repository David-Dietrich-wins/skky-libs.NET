using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace skky.util
{
	public static class FileHelper
	{
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
	}
}
