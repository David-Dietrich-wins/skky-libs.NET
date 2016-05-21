using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization.Json;

namespace skky.util
{
	public static class DcsWrapper
	{
		//private static bool isInitialized;
		private static XmlWriterSettings writerSettings = new XmlWriterSettings();

		static DcsWrapper()
		{
			writerSettings.OmitXmlDeclaration = true;
			writerSettings.Indent = true;
			writerSettings.IndentChars = "\t";
		}

		public static List<T> GetNonNullList<T>(IEnumerable<T> instance)
		{
			if (instance != null)
				return instance.ToList();

			return new List<T>();
		}
		public static IEnumerable<T> GetNonNullIEnumerable<T>(IEnumerable<T> instance)
		{
			if (instance == null)
				return new List<T>();

			return instance;
		}

		#region GetXml<T>
		public static string GetXml<T>(T instance)
		{
			DataContractSerializer dcs = new DataContractSerializer(typeof(T));

			StringBuilder sb = new StringBuilder(Environment.NewLine);

			using (XmlWriter writer = XmlWriter.Create(sb, writerSettings))
				dcs.WriteObject(writer, instance);

			return sb.ToString();
		}
		#endregion

		#region GetJson<T>
		public static string GetJson<T>(T instance)
		{
			MemoryStream stream = null;
			XmlWriter writer = null;

			try
			{
				DataContractJsonSerializer dcs = new DataContractJsonSerializer(typeof(T));

				stream = new MemoryStream();
				writer = JsonReaderWriterFactory.CreateJsonWriter(stream);

				dcs.WriteObject(writer, instance);
				writer.Flush();
				stream.Seek(0, SeekOrigin.Begin);

				using (StreamReader reader = new StreamReader(stream))
				{
					stream = null;

					return reader.ReadToEnd();
				}
			}
			finally
			{
				if (null != writer)
					writer.Dispose();

				if (null != stream)
					stream.Dispose();
			}
		} 
		#endregion


		#region GetObject<T>
		public static T GetObject<T>(string xml)
		{
			DataContractSerializer dcs = new DataContractSerializer(typeof(T));
			StringReader sr = null;

			try
			{
				sr = new StringReader(xml);

				using (XmlReader reader = XmlReader.Create(sr))
				{
					sr = null;
					return (T)dcs.ReadObject(reader);
				}
			}
			finally
			{
				if (null != sr)
					sr.Dispose();
			}
		}
		public static T GetObjectFromJson<T>(string json)
		{
			DataContractJsonSerializer dcs = new DataContractJsonSerializer(typeof(T));
			MemoryStream ms = null;

			try
			{
				ms = new MemoryStream(Encoding.Default.GetBytes(json));
				using (var reader = JsonReaderWriterFactory.CreateJsonReader(ms, new XmlDictionaryReaderQuotas()))
				{
					ms = null;

					return (T)dcs.ReadObject(reader);
				}
			}
			finally
			{
				if (null != ms)
					ms.Dispose();
			}
		}

		public static T GetObject<T>(Stream xml)
		{
			DataContractSerializer dcs = new DataContractSerializer(typeof(T));
			StreamReader sr = null;

			try
			{
				sr = new StreamReader(xml);

				using (XmlReader reader = XmlReader.Create(sr))
				{
					sr = null;

					return (T)dcs.ReadObject(reader);
				}
			}
			finally
			{
				if(null != sr)
					sr.Dispose();
			}
		}
		public static T GetObjectFromJson<T>(Stream json)
		{
			DataContractJsonSerializer dcs = new DataContractJsonSerializer(typeof(T));

			using (var reader = JsonReaderWriterFactory.CreateJsonReader(json, new XmlDictionaryReaderQuotas()))
			{
				return (T)dcs.ReadObject(reader);
			}
		}
		#endregion

		#region GetObject
		public static object GetObjectFromJson(string json, Type type)
		{
			DataContractJsonSerializer dcs = new DataContractJsonSerializer(type);
			StringReader sr = null;

			try
			{
				sr = new StringReader(json);
				using (XmlReader reader = XmlReader.Create(sr))
				{
					sr = null;

					return dcs.ReadObject(reader);
				}
			}
			finally
			{
				if (null != sr)
					sr.Dispose();
			}
		}
		public static object GetObject(string xml, Type type)
		{
			DataContractSerializer dcs = new DataContractSerializer(type);
			StringReader sr = null;

			try
			{
				sr = new StringReader(xml);

				using (XmlReader reader = XmlReader.Create(sr))
				{
					sr = null;

					return dcs.ReadObject(reader);
				}
			}
			finally
			{
				if (null != sr)
					sr.Dispose();
			}
		}
		#endregion
	}
}
