using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Security.Cryptography;
using System.IO;
using System.Drawing.Imaging;
using System.Net.Mail;
using System.Xml;
using System.Collections.Specialized;
using System.Reflection;
using System.Dynamic;
using System.ComponentModel;

namespace skky.util
{
	public static class Extensions
	{
		#region String extensions
		public static string Left(this string str, int length)
		{
			//we start at 0 since we want to get the characters starting from the
			//left and with the specified lenght and assign it to a variable
			if (!string.IsNullOrEmpty(str) && length > 0 && str.Length > length)
				return str.Substring(0, length);
			//return the result of the operation
			return str ?? string.Empty;
		}
		public static string Right(this string str, int length)
		{
			//start at the index based on the lenght of the sting minus
			//the specified lenght and assign it a variable
			if (!string.IsNullOrEmpty(str) && length > 0 && str.Length > length)
				return str.Substring(str.Length - length, length);
			//return the result of the operation
			return str;
		}

		public static string Mid(this string str, int startIndex)
		{
			//start at the specified index and return all characters after it
			//and assign it to a variable
			string s = str ?? string.Empty;

			if (s.Length < startIndex)
				return string.Empty;
			else if (startIndex < 1)
				return string.Empty;

			return s.Substring(startIndex);
		}
		public static string Mid(this string str, int startIndex, int length)
		{
			//start at the specified index in the string ang get N number of
			//characters depending on the lenght and assign it to a variable
			if (!string.IsNullOrEmpty(str) && (length > 0) && ((str.Length - startIndex) >= length))
				return str.Substring(startIndex, length);
			//return the result of the operation
			return str;
		}

		public static string MiddleOf(this string str, string left, string right)
		{
			if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(left) && !string.IsNullOrEmpty(right))
			{
				int posn = 0;
				posn = str.IndexOf(left);
				if (posn >= 0)
				{
					string sTemp = str.Mid(posn + left.Length);
					posn = sTemp.IndexOf(right);
					if (posn >= 0)
					{
						return sTemp.Left(posn);
					}
				}
			}

			return string.Empty;
		}

		public static int InStr(this string str, string sSearch)
		{
			return str.InStr(0, sSearch);
		}

		public static int InStr(this string str, int iStart, string sSearch)
		{
			int iPos = -1;
			if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(sSearch))
			{
				if (iStart < str.Length)
				{
					iPos = str.IndexOf(sSearch, iStart);
				}
			}

			return iPos;
		}

		public static string Prefill(this string s, string sPrefill, int size)
		{
			string str = s ?? string.Empty;
			if (!string.IsNullOrEmpty(sPrefill) && size > 0)
			{
				while (str.Length < size)
				{
					str = sPrefill + str;
				}
			}

			return str;
		}
		public static string Postfill(this string s, string sPostfill, int size)
		{
			string str = s ?? string.Empty;
			if (!string.IsNullOrEmpty(sPostfill) && size > 0)
			{
				while (str.Length < size)
				{
					str += sPostfill;
				}
			}

			return str;
		}

		/// <summary>
		/// Wraps a string with a prefix string and a suffix string.
		/// Guaranteed to always return a non-null string.
		/// </summary>
		/// <param name="str">The string to wrap.</param>
		/// <param name="prefix">The prefix string to wrap str.</param>
		/// <param name="suffix">The suffix string to wrap str.</param>
		/// <param name="wrapIfEmpty">Set to false and if str is null or empty, no wrapping will occur</param>
		/// <returns>A wrapped string with the prefix and suffix, unless the string is empty and wrapIfEmpty is false, then an empty string is returned. Always returns a non-null string.</returns>
		public static string Wrap(this string str, string prefix, string suffix, bool wrapIfEmpty = true)
		{
			if (!wrapIfEmpty && string.IsNullOrEmpty(str))
				return string.Empty;

			return (prefix ?? string.Empty) + (str ?? string.Empty) + (suffix ?? string.Empty);
		}
		public static string Wrap(this string str, string wrapper, bool wrapIfEmpty = true)
		{
			return Wrap(str, wrapper, wrapper, wrapIfEmpty);
		}
		public static string WrapInBraces(this string str, bool wrapIfEmpty = true)
		{
			return Wrap(str, "{", "}", wrapIfEmpty);
		}
		public static string WrapInBrackets(this string str, bool wrapIfEmpty = true)
		{
			return Wrap(str, "[", "]", wrapIfEmpty);
		}
		public static string WrapInParens(this string str, bool wrapIfEmpty = true)
		{
			return Wrap(str, "(", ")", wrapIfEmpty);
		}
		public static string WrapInQuotes(this string str, bool wrapIfEmpty = true)
		{
			return Wrap(str, "\"", wrapIfEmpty);
		}
		public static string WrapInSingleQuotes(this string str, bool wrapIfEmpty = true)
		{
			return Wrap(str, "\'", wrapIfEmpty);
		}
		public static string AddTag(this string str, string tagName)
		{
			return XMLHelper.AddTag(tagName, str);
		}
		#endregion

		//public static bool IsNumeric(this string str)
		//{
		//    string s = str ?? string.Empty;
		//    if (s.Length > 0)
		//    {
		//        for (int i = 0; i < s.Length; ++i)
		//        {
		//            string sTemp = s.Substring(i, 1);
		//            if ("0123456789".IndexOf(sTemp) < 0)
		//                return false;
		//        }

		//        return true;
		//    }
		//    return false;
		//}
		//public static string CopyNumerics(this string src)
		//{
		//    string dest = string.Empty;
		//    string CurChar = string.Empty;

		//    int StrLen = src.Length;
		//    for (int i = 0; i < StrLen; ++i)
		//    {
		//        CurChar = src.Substring(i, 1);
		//        if (CurChar.IsNumeric())
		//        {
		//            dest += CurChar;
		//        }
		//        else
		//        {
		//            break;
		//        }
		//    }

		//    return dest;
		//}

		//public static int IndexOf(this string str, string find)
		//{
		//    if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(find))
		//        return str.IndexOf(find);

		//    return -1;
		//}
		//public static int Val(string s)
		//{
		//    string str = CopyNumerics(s);
		//    return str.GetInteger();
		//}

		/// <summary>
		/// Replaces characters that interfere with Javascript functions.
		/// Came from the jQuery AutoComplete routine. 's came back and were escaped to &#39; Unicode entities.
		/// jQuery AutoComplete needs an \ in front of the character to show properly in an input field.
		/// </summary>
		/// <param name="str">str to replace Javascript characters.</param>
		/// <returns>String with \ escaped characters for Javascript ('").</returns>
		public static string ReplaceForJavascript(this string str)
		{
			if (string.IsNullOrEmpty(str))
				return string.Empty;

			str = str.Replace("'", "\\'");
			str = str.Replace("\"", "\\\"");

			return str;
		}

		public static string PrependCommaSpace(this string str, bool prependCommaSpace)
		{
			if (!prependCommaSpace)
				return str;

			return Wrap(str, ", ", string.Empty);
		}

		public static string EqualsValue(this string str, string value)
		{
			if (value == null)
				return str;

			return str + "=" + value;
		}
		public static string EqualsQuotedValue(this string str, string value)
		{
			if (value == null)
				return str;

			return str + "=" + value.WrapInQuotes();
		}

		public static string LogValue(this string str, string value, string delimiter = ": ", string suffix = ".\n")
		{
			string msg = (str ?? string.Empty) + (delimiter ?? string.Empty) + (value ?? string.Empty);
			msg += (suffix ?? string.Empty);

			return msg;
		}

		public static bool IsNumber(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return false;

			Regex r = new Regex(@"(^[-+]?\d+(,?\d*)*\.?\d*([Ee][-+]\d*)?$)|(^[-+]?\d?(,?\d*)*\.\d+([Ee][-+]\d*)?$)");
			return r.Match(str.Trim()).Success;
		}
		public static bool IsNonNegativeNumber(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return true;

			return str.IsPositiveNumber();
		}
		public static bool IsPositiveNumber(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return false;

			Regex r = new Regex(@"(^[+]?\d+(,?\d*)*\.?\d*([Ee][+]\d*)?$)|(^[+]?\d?(,?\d*)*\.\d+([Ee][+]\d*)?$)");
			return r.Match(str.Trim()).Success;
		}
		public static bool IsInteger(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return false;

			int i = 0;
			return int.TryParse(GetIntegerReady(str), out i);
			//Regex r = new Regex(@"(^[-+]?\d+(,?\d*)+)|(^[-+]?\d?(,?\d*)*)");
			//return r.Match(str).Success;
		}
		public static bool IsNonNegativeInteger(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return true;

			return str.IsPositiveInteger();
		}
		public static bool IsPositiveInteger(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return false;

			int i = 0;
			bool b = int.TryParse(GetIntegerReady(str), out i);
			if (b && i >= 0)
				return true;

			return false;
			//Regex r = new Regex(@"(^[+]?\d+(,?\d*)+)|(^[+]?\d?(,?\d*)*)");
			//return r.Match(str).Success;
		}

		public static string GetIntegerReady(string sint)
		{
			if (string.IsNullOrWhiteSpace(sint))
				return string.Empty;

			return sint.Trim().Replace(",", "");
		}

		#region Basic type converters
		public static bool? ToBooleanNullable(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return null;

			return str.ToBoolean();
		}
		public static bool ToBoolean(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return false;

			bool b = false;
			string s = str.Trim().ToLower();
			bool brc = bool.TryParse(s, out b);
			if (brc)
				return b;

			if (s.Equals("on"))
			{
				b = true;
			}
			else
			{
				switch (s.Substring(0, 1))
				{
					case "t":
					case "y":
					case "1":
					case "2":
					case "3":
					case "4":
					case "5":
					case "6":
					case "7":
					case "8":
					case "9":
						b = true;
						break;
					default:
						b = false;
						break;
				}
			}

			return b;
		}
		public static decimal? ToDecimalNullable(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return null;

			return ToDecimal(str);
		}
		public static decimal ToDecimal(this string str)
		{
			decimal d = 0;
			if (!string.IsNullOrWhiteSpace(str) && decimal.TryParse(GetIntegerReady(str), out d))
				return d;

			return 0;
		}
		public static double? ToDoubleNullable(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return null;

			return ToDouble(str);
		}
		public static double ToDouble(this string str)
		{
			double d = 0;
			if (!string.IsNullOrWhiteSpace(str) && double.TryParse(GetIntegerReady(str), out d))
				return d;

			return 0;
		}
		public static float? ToFloatNullable(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return null;

			return ToFloat(str);
		}
		public static float ToFloat(this string str)
		{
			float d = 0;
			if (!string.IsNullOrWhiteSpace(str) && float.TryParse(GetIntegerReady(str), out d))
				return d;

			return 0;
		}
		public static Guid? ToGuidNullable(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return null;

			return str.ToGuid();
		}
		public static Guid ToGuid(this string str)
		{
			Guid g;
			if (!string.IsNullOrWhiteSpace(str) && Guid.TryParse(str, out g))
				return g;

			return Guid.Empty;
		}
		public static int? ToIntegerNullable(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return null;

			return str.ToInteger();
		}
		public static int ToInteger(this string str)
		{
			int i = 0;
			if (!string.IsNullOrWhiteSpace(str) && int.TryParse(GetIntegerReady(str), out i))
				return i;

			return 0;
		}
		public static long? ToLongNullable(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return null;

			return str.ToLong();
		}
		public static long ToLong(this string str)
		{
			long l = 0;
			if (!string.IsNullOrWhiteSpace(str) && long.TryParse(GetIntegerReady(str), out l))
				return l;

			return 0;
		}
		public static short? ToShortNullable(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return null;

			return ToShort(str);
		}
		public static short ToShort(this string str)
		{
			short d = 0;
			if (!string.IsNullOrWhiteSpace(str) && short.TryParse(GetIntegerReady(str), out d))
				return d;

			return 0;
		}
		public static TimeSpan ToTimeSpan(this string str)
		{
			TimeSpan? ts = ToTimeSpanNullable(str);

			return (null == ts ? new TimeSpan() : ts.Value);
		}
		public static TimeSpan? ToTimeSpanNullable(this string str)
		{
			TimeSpan ts;
			if (!string.IsNullOrWhiteSpace(str) && TimeSpan.TryParse(str, out ts))
				return ts;

			return null;
		}
		#endregion

		public static string ToHexString(this byte byteToHex)
		{
			return string.Format("{0:x2}", byteToHex);
		}
		public static string ToHexString(this byte[] ba)
		{
			if (ba == null)
				return string.Empty;

			StringBuilder hex = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba)
				hex.AppendFormat("{0:x2}", b);

			return hex.ToString();
		}

		public static Color ToColor(this string str)
		{
			return str.ToColor(Color.Empty);
		}
		public static Color ToColor(this string color, Color defaultColor)
		{
			Color c = defaultColor;
			if (!string.IsNullOrEmpty(color))
			{
				try
				{
					int i = 0;
					bool b = int.TryParse(color, out i);
					if (b)
						c = Color.FromArgb(i);
					else
						c = Color.FromName(color);
				}
				catch (Exception ex)
				{
					skky.util.Trace.Warning("GetColor could not parse color: " + color + ".", ex);
				}
			}

			return c;
		}

		public static int FromHex(this char ch)
		{
			switch (ch)
			{
				//case '0':
				//    return 0;
				case '1':
					return 1;
				case '2':
					return 2;
				case '3':
					return 3;
				case '4':
					return 4;
				case '5':
					return 5;
				case '6':
					return 6;
				case '7':
					return 7;
				case '8':
					return 8;
				case '9':
					return 9;
				case 'a':
					return 10;
				case 'b':
					return 11;
				case 'c':
					return 12;
				case 'd':
					return 13;
				case 'e':
					return 14;
				case 'f':
					return 15;
			}

			return 0;
		}
		public static string ToHtmlString(this Color c)
		{
			if (c == Color.Empty)
				return string.Empty;

			return "#" + c.R.ToHexString() + c.G.ToHexString() + c.B.ToHexString();
		}
		public static Color FromHtmlString(this string str)
		{
			if (!string.IsNullOrEmpty(str))
			{
				string s = str.Trim();
				s = s.Trim('#').ToLower();
				if (s.Length == 3 || s.Length == 6)
				{
					int charCount = 0;
					char[] ch = new char[s.Length];
					for (charCount = 0; charCount < s.Length; ++charCount)
					{
						if (!"abcdef012346789".Contains(s.Mid(charCount, 1)))
							break;

						ch[charCount] = s[charCount];
					}

					int hex = 0;
					int red = 0;
					int green = 0;
					int blue = 0;
					if (charCount == 3)
					{
						hex = ch[0].FromHex();
						red = ((hex * 16) + hex);
						hex = ch[1].FromHex();
						green = ((hex * 16) + hex);
						hex = ch[2].FromHex();
						blue = ((hex * 16) + hex);
					}
					else if(charCount == 6)
					{
						hex = ch[0].FromHex() * 16;
						red = hex + ch[1].FromHex();
						hex = ch[2].FromHex() * 16;
						green = hex + ch[3].FromHex();
						hex = ch[4].FromHex() * 16;
						blue = hex + ch[5].FromHex();
					}
					else
					{
						return Color.Empty;
					}

					return Color.FromArgb(red, green, blue);
				}
			}

			return Color.Empty;
		}

		/// <summary>
		/// Converts a comma-delimited string to an array of strings.
		/// Optionally you can pass a string of characters to trim each string in the returned array.
		/// </summary>
		/// <param name="sCommaList">A comma-delimited string.</param>
		/// <param name="trimChars">A string of characters to trim from each string array element.</param>
		/// <returns>A List of strings.</returns>
		public static List<string> ToStringList(this string sCommaList, string trimChars = " ")
		{
			Regex regex = new Regex("(?<=,(\"|\')).*?(?=(\"|\'),)|(^.*?(?=,))|((?<=,).*?(?=,))|((?<=,).*?$)");

			List<string> lint = new List<string>();
			if (!string.IsNullOrEmpty(sCommaList))
			{
				char[] trimCharArray = null;
				if (!string.IsNullOrEmpty(trimChars))
					trimCharArray = trimChars.ToCharArray();

				Match match = regex.Match(sCommaList);
				while (match.Success)
				{
					if (null == trimCharArray)
						lint.Add(match.Value);
					else
						lint.Add((match.Value ?? string.Empty).Trim(trimCharArray));

					match = match.NextMatch();
				}

				if (lint.Count < 1 && !string.IsNullOrEmpty(sCommaList))
				{
					if(null == trimCharArray)
						lint.Add(sCommaList);
					else
						lint.Add(sCommaList.Trim(trimCharArray));
				}
			}

			return lint;
		}
		public static List<int> ToIntegerList(this IEnumerable<string> strList)
		{
			List<int> lst = new List<int>();
			if (null != strList)
			{
				foreach (var str in strList)
					lst.Add(str.ToInteger());
			}

			return lst;
		}
		public static List<int> ToIntegerList(this string sCommaList)
		{
			Regex regex = new Regex("(?<=,(\"|\')).*?(?=(\"|\'),)|(^.*?(?=,))|((?<=,).*?(?=,))|((?<=,).*?$)");

			List<int> lint = new List<int>();
			if (!string.IsNullOrEmpty(sCommaList))
			{
				if (sCommaList.Contains(','))
				{
					Match match = regex.Match(sCommaList);
					while (match.Success)
					{
						lint.Add(match.Value.ToInteger());
						//Console.WriteLine ( j++ + " \t" + match);
						match = match.NextMatch();
					}
				}
				else
				{
					if (sCommaList.Trim().IsInteger())
					{
						lint.Add(sCommaList.Trim().ToInteger());
					}
				}
			}

			return lint;
		}

		public static byte[] Encode(this string str, int encoding)
		{
			return (str ?? string.Empty).Encode(Encoding.GetEncoding(encoding));
		}
		public static byte[] Encode(this string str, Encoding encoding)
		{
			return encoding.GetBytes(str ?? string.Empty);
		}
		public static byte[] EncodeAscii(this string str)
		{
			return (str ?? string.Empty).Encode(new ASCIIEncoding());
		}
		public static byte[] EncodeUnicode(this string str)
		{
			return (str ?? string.Empty).Encode(new UnicodeEncoding());
		}

		/// <summary>
		/// Combines a base URL with a relative path.
		/// Accounts for leading and trailing / characters so that only 1 / ever separates paths.
		/// </summary>
		/// <param name="baseUrl">The base Url (and path).</param>
		/// <param name="relativePath">The additional sub path to append to the base URL.</param>
		/// <returns>The combined baseUrl and relativePath with any extraneous / characters removed.</returns>
		public static string UrlCombine(this string baseUrl, string relativePath)
		{
			if (null == baseUrl || baseUrl.Length == 0)
			{
				return (relativePath ?? string.Empty);
			}

			if (null == relativePath || relativePath.Length == 0)
			{
				return baseUrl;
			}

			return string.Format("{0}/{1}", baseUrl.TrimEnd('/', '\\'), relativePath.TrimStart('/', '\\'));
		}

		#region LinkedResourceCollection.AddGif
		public static void AddGif(this LinkedResourceCollection instance, Bitmap bitmap, string contentID)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			if (bitmap == null || bitmap.Size.Width < 1 || bitmap.Size.Height < 1)
				throw new ApplicationException("bitmap is null or has no data");
			if (string.IsNullOrEmpty(contentID))
				throw new ArgumentNullException("contentID");

			if (!instance.Contains(contentID))
			{
				MemoryStream gif = new MemoryStream();
				bitmap.Save(gif, ImageFormat.Gif);
				gif.Seek(0, SeekOrigin.Begin);
				instance.Add(new LinkedResource(gif, "image/gif") { ContentId = contentID });
			}
		}
		#endregion

		#region LinkedResourceCollection.Contains(string)
		public static bool Contains(this LinkedResourceCollection instance, string contentID)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			if (string.IsNullOrEmpty(contentID))
				throw new ArgumentNullException("contentID");

			foreach (var resource in instance)
				if (resource.ContentId.Equals(contentID, StringComparison.InvariantCultureIgnoreCase))
					return true;

			return false;
		}
		#endregion

		#region NameValueCollection
		public static T FillObject<T>(this NameValueCollection coll, T item = default(T))
		{
			List<string> updatedFields;

			return FillObject<T>(coll, item, out updatedFields);
		}

		/// <summary>
		/// Fills an object from a NameValueCollection. Typically a web request form.
		/// </summary>
		/// <typeparam name="T">An object that has a default constructor.</typeparam>
		/// <param name="coll">The NameValueCollection to pull data from.</param>
		/// <param name="item">The object to fill. If null, a new object using the default constructor is generated.</param>
		/// <returns>The filled object. Will never be null.</returns>
		public static T FillObject<T>(this NameValueCollection coll, T item, out List<string> updatedFields)
		{
			if (null == coll)
				throw new Exception("Null collection received when trying to save form data.");

			if (null == item)
				item = default(T);

			updatedFields = new List<string>();

			Type t = item.GetType();
			foreach (string formDataKey in coll)
			{
				PropertyInfo pi = t.GetProperty(formDataKey.Replace("[]", ""), BindingFlags.Public | BindingFlags.Instance);
				if (null != pi && pi.CanWrite)
				{
					updatedFields.Add(formDataKey);

					string formDataValue = coll[formDataKey];

					Type propType = pi.PropertyType;
					if (propType.IsArray || (propType.IsGenericType && (propType.GetGenericTypeDefinition() == typeof(List<>))))
					{
						if (!string.IsNullOrEmpty(formDataValue))
						{
							if (propType == typeof(int) || propType == typeof(System.Nullable<Int32>))
								pi.SetValue(item, formDataValue.ToIntegerList());
							else
								pi.SetValue(item, formDataValue.ToStringList());
						}
					}
					else
					{
						if (propType == typeof(bool))
							pi.SetValue(item, formDataValue.ToBoolean());
						else if (propType == typeof(System.Nullable<Boolean>))
							pi.SetValue(item, formDataValue.ToBooleanNullable());
						else if (propType == typeof(decimal))
							pi.SetValue(item, formDataValue.ToDecimal());
						else if (propType == typeof(System.Nullable<decimal>))
							pi.SetValue(item, formDataValue.ToDecimalNullable());
						else if (propType == typeof(double))
							pi.SetValue(item, formDataValue.ToDouble());
						else if (propType == typeof(System.Nullable<double>))
							pi.SetValue(item, formDataValue.ToDoubleNullable());
						else if (propType == typeof(float) || propType == typeof(Single))
							pi.SetValue(item, formDataValue.ToFloat());
						else if (propType == typeof(System.Nullable<float>) || propType == typeof(System.Nullable<Single>))
							pi.SetValue(item, formDataValue.ToFloatNullable());
						else if (propType == typeof(short))
							pi.SetValue(item, formDataValue.ToShort());
						else if (propType == typeof(System.Nullable<Int16>))
							pi.SetValue(item, formDataValue.ToShortNullable());
						else if (propType == typeof(int) || propType == typeof(Int32))
							pi.SetValue(item, formDataValue.ToInteger());
						else if (propType == typeof(System.Nullable<Int32>))
							pi.SetValue(item, formDataValue.ToIntegerNullable());
						else if (propType == typeof(long) || propType == typeof(Int64))
							pi.SetValue(item, formDataValue.ToLong());
						else if (propType == typeof(System.Nullable<Int64>))
							pi.SetValue(item, formDataValue.ToLongNullable());
						else if (propType == typeof(Guid))
							pi.SetValue(item, formDataValue.ToGuid());
						else if (propType == typeof(System.Nullable<Guid>))
							pi.SetValue(item, formDataValue.ToGuidNullable());
						else if (propType == typeof(DateTime))
							pi.SetValue(item, formDataValue.ToDateTime());
						else if (propType == typeof(System.Nullable<DateTime>))
							pi.SetValue(item, formDataValue.ToDateTimeNullable());
						else if (propType == typeof(TimeSpan))
							pi.SetValue(item, formDataValue.ToTimeSpan());
						else if (propType == typeof(System.Nullable<TimeSpan>))
							pi.SetValue(item, formDataValue.ToTimeSpanNullable());
						else
							pi.SetValue(item, formDataValue);
					}
				}
			}

			return item;
		}
		#endregion

		public static dynamic ToDynamic(this object value)
		{
			IDictionary<string, object> expando = new ExpandoObject();

			if (null != value)
			{
				foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
					expando.Add(property.Name, property.GetValue(value));
			}

			return expando as ExpandoObject;
		}

		public static string GetExceptionMessage(this Exception ex)
		{
			if (null != ex)
			{
				if (null != ex.InnerException && !string.IsNullOrEmpty(ex.InnerException.Message))
					return GetExceptionMessage(ex.InnerException);

				return ex.Message;
			}

			return string.Empty;
		}
	}
}
