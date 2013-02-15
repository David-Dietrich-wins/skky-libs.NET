using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace skky.web
{
	[DataContract]
	public class json
	{
		[DataMember]
		public string innerHTML { get; set; }

		[DataMember]
		public string responseText { get; set; }

		public static bool IsJSONArray(string test)
		{
			return test.StartsWith("{") && !(test.StartsWith("{*") || test.StartsWith("["));
		}

		public static void AddJSONArray(IDictionary<string, string> dict, string key, List<String> value)
		{
			if (value.Count > 0)
				dict.Add(key, ConvertToJSONArray(value));
		}
		public static string ConvertToJSONArray(List<string> list)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("[\"");
			foreach(var item in list)
			{
				builder.Append(EscapeJsonString(item));
				builder.Append(",");
			}

			builder.Replace(",", "\"]", builder.Length - 1, 1);
			return builder.ToString();
		}

		public static string EscapeJsonString(string originalString)
		{
			if (IsJSONArray(originalString))
				return originalString;

			return originalString.Replace("\\/", "/").Replace("/", "\\/").Replace("\\\"", "\"").Replace("\"", "\\\"");
		}
	}
}
