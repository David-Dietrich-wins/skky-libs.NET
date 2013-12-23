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
			string json = string.Empty;
			DataContractJsonSerializer dcs = new DataContractJsonSerializer(typeof(T));

			using(MemoryStream stream = new MemoryStream())
			using (XmlWriter writer = JsonReaderWriterFactory.CreateJsonWriter(stream))
			{
				dcs.WriteObject(writer, instance);
				writer.Flush();
				stream.Seek(0, SeekOrigin.Begin);
				using (StreamReader reader = new StreamReader(stream))
					json = reader.ReadToEnd();
			}

			return json;
		} 
		#endregion


		#region GetObject<T>
		public static T GetObject<T>(string xml)
		{
			T instance;
			DataContractSerializer dcs = new DataContractSerializer(typeof(T));

			using (StringReader sr = new StringReader(xml))
			using (XmlReader reader = XmlReader.Create(sr))
			{
				instance = (T)dcs.ReadObject(reader);
			}

			return instance;
		}
		public static T GetObjectFromJson<T>(string json)
		{
			T instance;
			DataContractJsonSerializer dcs = new DataContractJsonSerializer(typeof(T));

			using (MemoryStream ms = new MemoryStream(Encoding.Default.GetBytes(json)))
			{
				using (var reader = JsonReaderWriterFactory.CreateJsonReader(ms, new XmlDictionaryReaderQuotas()))
				{
					instance = (T)dcs.ReadObject(reader);
				}
			}

			return instance;
		}

		public static T GetObject<T>(Stream xml)
		{
			T instance;
			DataContractSerializer dcs = new DataContractSerializer(typeof(T));

			using (StreamReader sr = new StreamReader(xml))
			using (XmlReader reader = XmlReader.Create(sr))
			{
				instance = (T)dcs.ReadObject(reader);
			}

			return instance;
		}
		public static T GetObjectFromJson<T>(Stream json)
		{
			T instance;
			DataContractJsonSerializer dcs = new DataContractJsonSerializer(typeof(T));

			using (var reader = JsonReaderWriterFactory.CreateJsonReader(json, new XmlDictionaryReaderQuotas()))
			{
				instance = (T)dcs.ReadObject(reader);
			}

			return instance;
		}
		#endregion

		#region GetObject
		public static object GetObjectFromJson(string json, Type type)
		{
			object instance;
			DataContractJsonSerializer dcs = new DataContractJsonSerializer(type);

			using (StringReader sr = new StringReader(json))
			using (XmlReader reader = XmlReader.Create(sr))
			{
				instance = dcs.ReadObject(reader);
			}

			return instance;
		}
		public static object GetObject(string xml, Type type)
		{
			object instance;
			DataContractSerializer dcs = new DataContractSerializer(type);

			using (StringReader sr = new StringReader(xml))
			using (XmlReader reader = XmlReader.Create(sr))
			{
				instance = dcs.ReadObject(reader);
			}

			return instance;
		}
		#endregion
	}
}
