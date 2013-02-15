using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using skkyWeb.Charts;
using skky.Types;
using skky.util;
using System.Reflection;

namespace skkyWeb.Charts
{
	public class SeriesDataReflection<T> : SeriesData
	{
		public delegate string HREFDelegate(int offset, T item);

		public enum Conversion
		{
			String,
			Date,
			Integer,
			Float,
			Double
		};

		private string stringName;
		private string objectName;
		private List<T> listItems;

		public Conversion xConversion = Conversion.String;

		public SeriesDataReflection(SeriesSettings ss, List<T> thelistItems, string StringType, string ObjectType)
			: base(ss)
		{
			listItems = thelistItems;
			stringName = StringType;
			objectName = ObjectType;
			ss.HREFIsJavascript = true;
		}

		public HREFDelegate myDelegate = null;

		public override int RowCount()
		{
			return listItems.Count();
		}

		public override StringObject getPoint(int offset)
		{
			T item = listItems.ElementAt(offset);
			PropertyInfo piString = item.GetType().GetProperty(stringName);
			PropertyInfo piObject = item.GetType().GetProperty(objectName);
			//Type type = typeof(T);
			//FieldInfo fieldString = type.GetField(stringName);
			//FieldInfo fieldObject = type.GetField(objectName);

			StringObject so = new StringObject();

			Object xValue = piString.GetValue(item, null);
			if (xConversion == Conversion.Date)
			{
				if (xValue.GetType() == typeof(int))
				{
					int dateKey = (int)xValue;
					DateTime dt = dateKey.FromDateKey();
					so.stringValue = dt.ToString("MMM dd");
				}
				else
				{
					so.stringValue = xValue.ToString();
				}
			}
			else
			{
				so.stringValue = xValue.ToString();
			}
			so.objValue = piObject.GetValue(item, null);

			return so;
		}
		public string GetNumberFormatString(string doubleFieldName, out double min, out double max, out int precision, string formatStart = "N")
		{
			string strFormat = (string.IsNullOrEmpty(formatStart) ? "N" : formatStart);

			precision = GetMaxWithPrecision(doubleFieldName, out min, out max);

			if (max > 0 && max < 1)
			{
				strFormat = ".";
				strFormat += string.Empty.Prefill("0", precision);
			}
			else
			{
				strFormat += "0";
			}

			return strFormat;
		}
		private int GetMaxWithPrecision(string doubleFieldName, out double min, out double max)
		{
			int precisionMax = 0;
			int numRows = RowCount();

			max = numRows > 0 ? double.MinValue : 0;
			min = numRows > 0 ? double.MaxValue : 0;
			int i = 0;
			for (i = 0; i < numRows; ++i)
			{
				T item = listItems.ElementAt(i);
				PropertyInfo pi = item.GetType().GetProperty(doubleFieldName);
				double? d = pi.GetValue(item, null) as double?;
				if (d.HasValue)
				{
					if (d.Value > max)
						max = d.Value;

					if (d.Value < min)
						min = d.Value;
				}
			}

			if (max > 0 && max < 1)
			{
				string testdec = Convert.ToString(max);
				int periodOffset = (testdec.IndexOf(".") + 1); // the first numbers plus decimal point
				int precision = ((testdec.Length) - periodOffset);     //total length minus beginning numbers and decimal = number of decimal points
				if (precision > precisionMax)
					precisionMax = precision;

				if (precisionMax < 2)
					precisionMax = 2;
				else if (precisionMax > 4)
					precisionMax = 4;
			}

			return precisionMax;
		}

		public override string GetToolTip(int offset)
		{
			if (!string.IsNullOrEmpty(Settings.ToolTipFormat))
			{
				T item = listItems.ElementAt(offset);

				return string.Format(Settings.ToolTipFormat, GetObjects(Settings.ToolTipPropertyNames, item));
			}

			return string.Empty;
		}

		protected object[] GetObjects(IEnumerable<string> propertyNames, T item)
		{
			if (propertyNames == null || propertyNames.Count() < 1)
				return null;

			object[] o = new object[propertyNames.Count()];
			int i = 0;
			foreach (var name in propertyNames)
			{
				PropertyInfo pi = item.GetType().GetProperty(name);
				o[i] = pi.GetValue(item, null);
				++i;
			}

			return o;
		}

		public override string GetHREF(int offset)
		{
			string str = string.Empty;
			string format = Settings.HREF;
			T item = listItems.ElementAt(offset);

			if (myDelegate != null)
			{
				str = myDelegate(offset, item);
			}
			else if (!string.IsNullOrEmpty(format))
			{
				if (Settings.HREFIsJavascript)
				{
					if (format.Contains("{") && format.Contains("}"))
					{
						str += string.Format(format, GetObjects(Settings.HREFPropertyNames, item));
					}
					else
					{
						str += format;	// Name of the JavaScript function.
						str += "(this";

						var props = GetObjects(Settings.HREFPropertyNames, item);
						if (null != props)
						{
							foreach (var prop in props)
							{
								str += ", ";

								if (prop.GetType() == typeof(string))
									str += prop.ToString().WrapInSingleQuotes();
								else
									str += prop.ToString();
							}
						}
						str += ");";
					}
				}
				else
				{
					if (format.Contains("{") && format.Contains("}"))
					{
						str += string.Format(format, GetObjects(Settings.HREFPropertyNames, item));
					}
					else
					{
						str = format;
					}
				}
			}

			return str;
		}
	}
}
