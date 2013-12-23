using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.Serialization;

namespace skky.util
{
	public static class XMLHelper
	{
		#region Extenstion Methods
		public static void WriteHrTag(this XmlWriter writer)
		{
			writer.WriteStartElement("hr");
			writer.WriteEndElement();
		}
		public static void WriteBreakTag(this XmlWriter writer)
		{
			writer.WriteStartElement("br");
			writer.WriteEndElement();
		}
		public static void WriteNonBreakingSpaceEntity(this XmlWriter writer)
		{
			writer.WriteEntityRef("nbsp");
		}
		#endregion

		public static string StartTag(string tagName)
		{
			return StartTag(tagName, string.Empty, string.Empty);
		}
		public static string StartTag(string tagName, string attributeName, string attributeValue)
		{
			if (string.IsNullOrEmpty(tagName))
				return string.Empty;

			string str = tagName.Trim() + AddAttribute(attributeName, attributeValue);
			return str.Wrap("<", ">");
		}
		public static string StartTag(string tagName, IDictionary<string, string> attributes)
		{
			if (string.IsNullOrEmpty(tagName))
				return string.Empty;

			string str = tagName.Trim() + AddAttributes(attributes);
			return str.Wrap("<", ">");
		}

		public static string AddAttribute(string attributeName, string attributeValue)
		{
			if (string.IsNullOrEmpty(attributeName))
				return string.Empty;

			return " " + attributeName.Trim().EqualsQuotedValue(attributeValue);
		}
		public static string AddAttributes(IDictionary<string, string> attributes)
		{
			string str = string.Empty;
			if (attributes != null && attributes.Count > 0)
			{
				foreach (var item in attributes)
					str += AddAttribute(item.Key, item.Value);
			}

			return str;
		}

		public static string EmptyTag(string tagName)
		{
			if (string.IsNullOrEmpty(tagName))
				return string.Empty;

			return tagName.Trim().Wrap("<", " />");
		}
		public static string EndTag(string tagName)
		{
			if (string.IsNullOrEmpty(tagName))
				return string.Empty;

			return tagName.Trim().Wrap("</", ">");
		}
		public static string AddTag(string tagName, string value, IDictionary<string, string> attributes)
		{
			return StartTag(tagName) + (value ?? string.Empty) + EndTag(tagName);
		}
		public static string AddTag(string tagName, string value, string attributeName, string attributeValue)
		{
			return StartTag(tagName, attributeName, attributeValue) + (value ?? string.Empty) + EndTag(tagName);
		}
		public static string AddTag(string tagName, string value)
		{
			return AddTag(tagName, value, string.Empty, string.Empty);
		}
		public static string AddTag(string tagName, string value, string className)
		{
			if(string.IsNullOrEmpty(className))
				return AddTag(tagName, value, string.Empty, string.Empty);

			return AddTag(tagName, value, "class", className);
		}
		public static string MakeEntitySafe(string xml)
		{
			if (string.IsNullOrEmpty(xml))
			{
				string s = xml.Replace("&", "&amp;");
				s = s.Replace("<", "&lt;");
				s = s.Replace(">", "&gt;");

				return s;
			}

			return string.Empty;
		}

		public static string ScrubXml(string strXml)
		{
			strXml = strXml.Replace("&", "&amp;");
			strXml = strXml.Replace("\"", "");
			strXml = strXml.Replace("'", "");

			return strXml;
		}

        public static T Deserialize<T>(string xml)
		{
			T inst;
			using (StringReader sr = new StringReader(xml))
			{
				XmlSerializer reader = new XmlSerializer(typeof(T));
				inst = (T)reader.Deserialize(sr);
			}

			return inst;
		}
		public static string Serialize(object o)
		{
			if (o == null)
				return string.Empty;

			using (var ms = new MemoryStream())
			{
				XmlSerializer xml = new XmlSerializer(o.GetType());
				xml.Serialize(ms, o);
				ms.Seek(0, SeekOrigin.Begin);
				using (var sr = new StreamReader(ms))
				{
					return sr.ReadToEnd();
				}
			}
		}

        public static T DeserializeWithDataContract<T>(Stream stream)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());

            return (T)dcs.ReadObject(reader);
        }
        public static T DeserializeFromFileWithDataContract<T>(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                return DeserializeWithDataContract<T>(fs);
            }
        }
        public static void SerializeWithDataContract(Stream stream, object o)
        {
            if (null != o)
            {
                var xdw = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8);

                var dcs = new DataContractSerializer(o.GetType());
                dcs.WriteObject(stream, o);
            }
        }
        public static void SerializeToFileWithDataContract(string fileName, object o)
        {
            using (var ms = new FileStream(fileName, FileMode.Append))
            {
                SerializeWithDataContract(ms, o);
            }
        }

		public static XDocument getXDocument(string xml)
		{
			using (StringReader sr = new StringReader(xml))
			using (XmlReader reader = XmlReader.Create(sr))
			{
				return XDocument.Load(reader);
			}
		}
		public static T DeserializeXDocument<T>(XDocument doc)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

			using (var reader = doc.Root.CreateReader())
			{
				return (T)xmlSerializer.Deserialize(reader);
			}
		}
		public static XDocument SerializeXDocument<T>(T value)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

			XDocument doc = new XDocument();
			using (var writer = doc.CreateWriter())
			{
				xmlSerializer.Serialize(writer, value);
			}

			return doc;
		}
	}
}
