using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Drawing;

namespace skky.util
{
	public static class XMLHelper
	{
		public const string CONST_Break = "<br />";
		public const string CONST_Class = "class";
		public const string CONST_Div = "div";
		public const string CONST_NbSp = "&nbsp;";
		public const string CONST_hidden = "hidden";
		public const string CONST_id = "id";
		public const string CONST_Label = "label";
		public const string CONST_name = "name";
		public const string CONST_onclick = "onclick";
		public const string CONST_option = "option";
		public const string CONST_Strong = "strong";
		public const string CONST_Style = "style";
		public const string CONST_table = "table";
		public const string CONST_td = "td";
		public const string CONST_tr = "tr";
		public const string CONST_value = "value";

		#region Extension Methods
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
		public static string AddTagAnchor(string value, IDictionary<string, string> attributes)
		{
			return AddTag("a", value, attributes);
		}
		public static string AddTag(string tagName, string value, IDictionary<string, string> attributes)
		{
			return StartTag(tagName, attributes) + (value ?? string.Empty) + EndTag(tagName);
		}
		public static string AddTag(string tagName, string value, string attributeName, string attributeValue)
		{
			return StartTag(tagName, attributeName, attributeValue) + (value ?? string.Empty) + EndTag(tagName);
		}
		public static string AddTag(string tagName, string value, string className = null)
		{
			if(string.IsNullOrEmpty(className))
				return AddTag(tagName, value, string.Empty, string.Empty);

			return AddTag(tagName, value, "class", className);
		}
		public static string AddDiv(string value, string className = null, IDictionary<string, string> attributes = null)
		{
			if (null != attributes && null != className)
			{
				attributes.Add(CONST_Class, className);
				return AddTag(CONST_Div, value, attributes);
			}

			return AddTag(CONST_Div, value, className);
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

		#region HTML encoding and decoding
		public static string Base64EncodeString(string strUrl)
		{
			return Convert.ToBase64String(Encoding.ASCII.GetBytes(strUrl));
		}
		public static string Base64EncodeGuid(Guid guid)
		{
			return Base64EncodeString(guid.ToString());
		}

		public static string Base64DecodeString(string strUrl)
		{
			return Encoding.ASCII.GetString(Convert.FromBase64String(strUrl));
		}
		public static Guid Base64DecodeGuid(string strUrl)
		{
			return new Guid(Base64DecodeString(strUrl));
		}

		//** base64 decodes a utf16 string thats been converted to utf8, then base64 encoded.  .net only deals in utf8 when base64 enc/decoding
		public static string Utf16Base64Decode(string value)
		{
			var utf8value = Convert.FromBase64String(value);
			return Encoding.Unicode.GetString(utf8value);
		}

		public static string Utf16Base64Encode(string value)
		{
			var utf8value = Encoding.UTF8.GetBytes(value);
			return Convert.ToBase64String(utf8value);
		}
		#endregion

		#region string based HTML builders
		public static string AddTagTD(string str, string className = null)
		{
			if (string.IsNullOrEmpty(str))
				str = CONST_NbSp;

			return AddTag(CONST_td, str, className);
		}

		public static string AddHidden(string id, string value)
		{
			return "<input type=" + CONST_hidden.WrapInQuotes() + " " + CONST_name.EqualsQuotedValue(id) + " " + CONST_id.EqualsQuotedValue(id) + (string.IsNullOrEmpty(value) ? string.Empty : " " + CONST_value.EqualsQuotedValue(value)) + " />";
		}

		/// <summary>
		/// Returns an option tag with the selected attribute set if the option value matches the selectedOption.
		/// </summary>
		/// <param name="name">The name of the option item.</param>
		/// <param name="selectedOption">The selected option.</param>
		/// <returns>An option tag with the selected attribute added if value = selectedOption or name = selectedOption if value is not passed in.</returns>
		public static string AddOption(string name, string selectedOption)
		{
			return AddOption(name, null, selectedOption);
		}
		/// <summary>
		/// Returns an option tag with the selected attribute set if the option value matches the selectedOption.
		/// </summary>
		/// <param name="name">The name of the option item.</param>
		/// <param name="value">The value of the option item. Can be empty and no value will be output.</param>
		/// <param name="selectedOption">The selected option.</param>
		/// <returns>An option tag with the selected attribute added if value = selectedOption or name = selectedOption if value is not passed in.</returns>
		public static string AddOption(string name, string value, string selectedOption)
		{
			string s = "<" + CONST_option;
			if (!string.IsNullOrEmpty(value))
				s += " value=\"" + value + "\"";
			if (!string.IsNullOrEmpty(selectedOption)
				&& ((!string.IsNullOrEmpty(value) && selectedOption == value)
				|| selectedOption == name))
				s += " selected=\"selected\"";
			s += ">";
			s += name;
			s += EndTag(CONST_option);;

			return s;
		}

		public static string AddTableRowIfValue(string name, int value, string tdclass = null)
		{
			if (value == 0)
				return string.Empty;

			return AddTableRow(name, value.ToString());
		}
		public static string AddTableRowIfValue(string name, string value, string tdclass = null)
		{
			if (string.IsNullOrEmpty(value))
				return string.Empty;

			return AddTableRow(name, value, tdclass);
		}
		public static string AddTableRow(string name, string value, string tdclass = null, string trclass = null)
		{
			string s = AddTagTD(name, tdclass);
			s += AddTagTD(value, tdclass);

			return AddTag(CONST_tr, s, trclass);
		}

		public static string BuildHref(string text, string url)
		{
			return BuildHref(text, url, null);
		}
		public static string BuildHref(string text, string url, string target)
		{
			string str = string.Empty;
			if (!string.IsNullOrEmpty(url))
			{
				if (!url.Contains("//"))
					url = "http://" + url;

				str = "<a";
				str += AddAttribute("href", url);
				if (target != null)
					str += AddAttribute("target", string.IsNullOrEmpty(target) ? "_blank" : target);
				str += ">";
			}

			if (!string.IsNullOrEmpty(text))
			{
				str += text;
				str += "</a>";
			}

			return str;
		}
		public static string Href(string text, string url)
		{
			return Href(text, url, null);
		}
		public static string Href(string text, string url, string target)
		{
			string str = string.Empty;
			if (!string.IsNullOrEmpty(url))
			{
				//if (!url.Contains("//"))
				//	url = "http://" + url;

				str = "<a";
				str += AddAttribute("href", url);
				if (target != null)
					str += AddAttribute("target", string.IsNullOrEmpty(target) ? "_blank" : target);
				str += ">";
			}

			if (!string.IsNullOrEmpty(text))
			{
				str += text;
				str += "</a>";
			}

			return str;
		}
		public static string GetHREFImage(string alt, string url, string imagePath)
		{
			return GetHREFImage(alt, url, imagePath);
		}
		public static string GetHREFImage(string alt, string url, string imagePath, int width, int height)
		{
			string str = string.Empty;
			if (!string.IsNullOrEmpty(url))
			{
				if (!url.Contains("//"))
					url = "http://" + url;

				str = "<a";
				str += AddAttribute("href", url);
				str += AddAttribute("target", "_blank");
				str += ">";
			}

			str += BuildImg(imagePath, width, height, alt);

			if (!string.IsNullOrEmpty(url))
				str += "</a>";

			return str;
		}
		public static string BuildImg(string imagePath, int width, int height, string alt)
		{
			string str = string.Empty;
			if (string.IsNullOrEmpty(imagePath))
			{
				str = alt;
			}
			else
			{
				str += "<img";
				str += AddAttribute("src", imagePath);
				if (height > 0)
					str += AddAttribute("height", height.ToString());
				if (width > 0)
					str += AddAttribute("width", width.ToString());
				str += AddAttribute("alt", alt);
				str += AddAttribute("border", "0");
				str += " />";
			}

			return str;
		}

		public static string Strong(string str)
		{
			return AddTag(CONST_Strong, str);
		}
		public static string Bold(string str)
		{
			return AddTag("b", str);
		}
		public static string Label(string str, Color color)
		{
			return AddTag(CONST_Label, str, CONST_Style, "background-color:" + color.ToHtmlString() + ";");
		}

		public static string PadRightNbsp(string s, int totalLength)
		{
			string str = s ?? string.Empty;
			int spacesToAdd = 1;
			if (str.Length < totalLength)
				spacesToAdd = totalLength - str.Length;

			for (int i = 0; i < spacesToAdd; ++i)
			{
				str += CONST_NbSp;
			}

			return str;
		}

		public static string buildURL(string startURL, string file)
		{
			if (startURL == null)
				startURL = "";

			if (file == null)
				file = "";

			if (startURL.EndsWith("/") && file.StartsWith("/"))
				file = file.Mid(1);

			return startURL + file;
		}
		#endregion

		#region XML Serialization
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

			MemoryStream ms = null;
			try
			{
				ms = new MemoryStream();

				XmlSerializer xml = new XmlSerializer(o.GetType());
				xml.Serialize(ms, o);
				ms.Seek(0, SeekOrigin.Begin);

				using (var sr = new StreamReader(ms))
				{
					ms = null;

					return sr.ReadToEnd();
				}
			}
			finally
			{
				if (null != ms)
					ms.Dispose();
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
			StringReader sr = null;
			
			try
			{
				sr = new StringReader(xml);

				using (XmlReader reader = XmlReader.Create(sr))
				{
					sr = null;

					return XDocument.Load(reader);
				}
			}
			finally
			{
				if (null != sr)
					sr.Dispose();
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
		#endregion
	}
}
