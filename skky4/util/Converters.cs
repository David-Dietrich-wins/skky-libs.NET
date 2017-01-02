using System;
using System.Collections.Generic;
using System.Linq;
using skky.Conversions;
using skky.Types;

namespace skky.util
{
	public static class Converters
	{
		public static void ConvertToMetricInt(StringInt stringInt, IConversion converter)
		{
			if (stringInt != null && converter != null)
			{
				double d = converter.ConvertToMetric(stringInt.intValue);
				stringInt.intValue = (int)d;
			}
		}
		public static void ConvertToStandardInt(StringInt stringInt, IConversion converter)
		{
			if (stringInt != null && converter != null)
			{
				double d = converter.ConvertToStandard(stringInt.intValue);
				stringInt.intValue = (int)d;
			}
		}
		public static void ConvertToMetricInt(IEnumerable<StringInt> listStringInt, IConversion converter)
		{
			for (int i = 0; i < listStringInt.Count(); ++i)
			{
				StringInt si = listStringInt.ElementAt(i);
				ConvertToMetricInt(si, converter);
			}
		}
		public static void ConvertToStandardInt(IEnumerable<StringInt> listStringInt, IConversion converter)
		{
			for (int i = 0; i < listStringInt.Count(); ++i)
			{
				StringInt si = listStringInt.ElementAt(i);
				ConvertToStandardInt(si, converter);
			}
		}

		public static void ConvertToMetricDouble(StringIntDouble stringIntDouble, IConversion converter)
		{
			if (stringIntDouble != null && converter != null)
			{
				double d = converter.ConvertToMetric(stringIntDouble.doubleValue);
				stringIntDouble.doubleValue = d;
			}
		}
		public static void ConvertToStandardDouble(StringIntDouble stringIntDouble, IConversion converter)
		{
			if (stringIntDouble != null && converter != null)
			{
				double d = converter.ConvertToStandard(stringIntDouble.doubleValue);
				stringIntDouble.doubleValue = d;
			}
		}
		public static void ConvertToMetricDouble(IEnumerable<StringDouble> listStringDouble, IConversion converter)
		{
			for (int i = 0; i < listStringDouble.Count(); ++i)
			{
				StringDouble sd = listStringDouble.ElementAt(i);
				if (sd != null)
				{
					double d = converter.ConvertToMetric(sd.doubleValue);
					sd.doubleValue = d;
				}
			}
		}
		public static void ConvertToStandardDouble(IEnumerable<StringDouble> listStringDouble, IConversion converter)
		{
			for (int i = 0; i < listStringDouble.Count(); ++i)
			{
				StringDouble sd = listStringDouble.ElementAt(i);
				if (sd != null)
				{
					double d = converter.ConvertToStandard(sd.doubleValue);
					sd.doubleValue = d;
				}
			}
		}
		public static void ConvertToMetricDouble(IEnumerable<StringIntDouble> listStringIntDouble, IConversion converter)
		{
			for (int i = 0; i < listStringIntDouble.Count(); ++i)
			{
				StringIntDouble sd = listStringIntDouble.ElementAt(i);
				if (sd != null)
				{
					double d = converter.ConvertToMetric(sd.doubleValue);
					sd.doubleValue = d;
				}
			}
		}
		public static void ConvertToStandardDouble(IEnumerable<StringIntDouble> listStringIntDouble, IConversion converter)
		{
			for (int i = 0; i < listStringIntDouble.Count(); ++i)
			{
				StringIntDouble sd = listStringIntDouble.ElementAt(i);
				if (sd != null)
				{
					double d = converter.ConvertToStandard(sd.doubleValue);
					sd.doubleValue = d;
				}
			}
		}

		public static void ConvertIntUnits(StringInt stringInt, IConversion converter, bool dbFieldIsMetric, bool wantMetric)
		{
			if (!dbFieldIsMetric && wantMetric)
				ConvertToMetricInt(stringInt, converter);
			else if (dbFieldIsMetric && !wantMetric)
				ConvertToStandardInt(stringInt, converter);
		}
		public static void ConvertIntUnits(IEnumerable<StringInt> listStringInt, IConversion converter, bool dbFieldIsMetric, bool wantMetric)
		{
			if (!dbFieldIsMetric && wantMetric)
				ConvertToMetricInt(listStringInt, converter);
			else if (dbFieldIsMetric && !wantMetric)
				ConvertToStandardInt(listStringInt, converter);
		}
		//public static void ConvertDoubleUnits(IEnumerable<StringDouble> listStringDouble, IConversion converter, bool dbFieldIsMetric, bool wantMetric)
		//{
		//    if (!dbFieldIsMetric && wantMetric)
		//        ConvertToMetricDouble(listStringDouble, converter);
		//    else if (dbFieldIsMetric && !wantMetric)
		//        ConvertToStandardDouble(listStringDouble, converter);
		//}
		public static void ConvertDoubleUnits(StringIntDouble stringIntDouble, IConversion converter, bool dbFieldIsMetric, bool wantMetric)
		{
			if (!dbFieldIsMetric && wantMetric)
				ConvertToMetricDouble(stringIntDouble, converter);
			else if (dbFieldIsMetric && !wantMetric)
				ConvertToStandardDouble(stringIntDouble, converter);
		}
		public static void ConvertDoubleUnits(IEnumerable<StringIntDouble> listStringIntDouble, IConversion converter, bool dbFieldIsMetric, bool wantMetric)
		{
			if (!dbFieldIsMetric && wantMetric)
				ConvertToMetricDouble(listStringIntDouble, converter);
			else if (dbFieldIsMetric && !wantMetric)
				ConvertToStandardDouble(listStringIntDouble, converter);
		}

		public static string SingleQuoteGuidList(IEnumerable<Guid> guids, bool addSingleQuotesAroundSources = false, string defaultIfNone = null)
		{
			if (null == guids || !guids.Any())
				return string.Empty;

			return SingleQuoteStringList(guids.Select(x => x.ToString()));
		}
		public static string SingleQuoteStringList(IEnumerable<string> strs, bool addSingleQuotesAroundSources = false, string defaultIfNone = null)
		{
			string str = string.Empty;
			if (null != strs && strs.Count() > 0)
				str = Join(",", strs, i => "'" + i.ToString() + "'");

			if (!string.IsNullOrWhiteSpace(str) && addSingleQuotesAroundSources)
			{
				str = str.Replace("'", "''");
				str = str.WrapInSingleQuotes();
			}
			//string str = (sources == null ? Helper.CONST_Null : sources.Aggregate((a, x) => a + ",'" + x + "'"));

			if (string.IsNullOrEmpty(str))
				str = defaultIfNone;

			return str;
		}
		public static string Join<T>(string delimiter, IEnumerable<T> collection, Func<T, string> convert)
		{
			return string.Join(delimiter,
				collection.Select(convert).ToArray());
		}

		/// <summary>
		/// Converts a value from a value in the Original Range to the New Range.
		/// First used to convert different ranges to RGB values 0-255.
		/// </summary>
		/// <param name="originalRangeMin"></param>
		/// <param name="originalRangeMax"></param>
		/// <param name="newRangeMin"></param>
		/// <param name="newRangeMax"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public static long Remap(long originalRangeMin, long originalRangeMax
			, long newRangeMin, long newRangeMax, long val)
		{
			long NewValue = 0;

			long OldRange = originalRangeMax - originalRangeMin;
			if (OldRange == 0)
			{
				NewValue = newRangeMin;
			}
			else
			{
				long NewRange = newRangeMax - newRangeMin;

				NewValue = (((val - originalRangeMin) * NewRange) / OldRange) + newRangeMin;
			}

			return NewValue;
		}

		public static double CelsiusToFahrenheit(double celsius)
		{
			return ((celsius * 9) / 5) + 32;
		}
		public static double FahrenheitToCelsius(double fahrenheit)
		{
			return ((fahrenheit - 32) * 5) / 9;
		}
	}
}