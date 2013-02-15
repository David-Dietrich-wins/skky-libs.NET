using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.util;

namespace skky.Types
{
	[DataContract]
	public class DataRowManager
	{
		public DataRowManager()
		{ }

		public DataRowManager(IEnumerable<PropertyManager> list)
		{
			if (list != null)
				listDataRows = list.ToList();
		}

		private List<PropertyManager> listDataRows;
		private List<PropertyManager> DataRows
		{
			get
			{
				if (listDataRows == null)
					listDataRows = new List<PropertyManager>();

				return listDataRows;
			}

			set
			{
				listDataRows = value;
			}
		}

		public int RowCount()
		{
			return DataRows.Count;
		}

		public KeyValuePairCollection<int, System.Type> GetDescription()
		{
			return GetFirstRow().GetDescription();
		}
		public System.Type GetPropertyDescription(int propertyNum)
		{
			return GetFirstProperty(propertyNum).PropertyType;
		}

		public PropertyManager GetDataRow(int offset)
		{
			if (offset < 0 || offset >= RowCount())
				throw new Exception(string.Format("DataRowManager attempted to access an invalid row #%d.", offset));

			return DataRows[offset];
		}
		public Property GetProperty(int offset, int propertyNum)
		{
			PropertyManager dr = GetDataRow(offset);
			if(dr != null)
				return dr.GetProperty(propertyNum);

			return null;
		}
		public void AddProperty(int offset, Property p)
		{
			PropertyManager dr = GetDataRow(offset);
			if (dr != null)
				dr.Add(p);
		}

		public PropertyManager GetFirstRow()
		{
			if (RowCount() < 1)
				return new PropertyManager();

			return DataRows[0];
		}
		public PropertyManager GetLastRow()
		{
			if (RowCount() < 1)
				return new PropertyManager();

			return GetDataRow(RowCount() - 1);
		}
		public Property GetFirstProperty(int propertyNum)
		{
			return GetFirstRow().GetProperty(propertyNum);
		}
		public Property GetLastProperty(int propertyNum)
		{
			return GetLastRow().GetProperty(propertyNum);
		}

		public void Take(int NumberOfPoints)
		{
			if (NumberOfPoints > 0)
				DataRows = DataRows.Take(NumberOfPoints).ToList();
		}

		public void EliminateAnyValueLowerThan(int propertyNumber, double dMin)
		{
			if (DataRows.Any())
			{
				var listSorted = DataRows.Where((dr, index) => dr.GetProperty(propertyNumber).doubleValue >= dMin); // (from d in DataRows where d.doubleValue >= dMin select d);
				DataRows = listSorted.ToList();
			}
		}

		// Sorting Algorithms
		public void SortHighestToLowestOnPropertyDouble(int propertyNum)
		{
			if (DataRows.Any())
			{
				var listSorted = DataRows.OrderByDescending(dr => dr.GetProperty(propertyNum).doubleValue);
				DataRows = listSorted.ToList();
			}
		}
		public void SortLowestToHighestOnPropertyDouble(int propertyNum)
		{
			if (DataRows.Any())
			{
				var listSorted = DataRows.OrderBy(dr => dr.GetProperty(propertyNum).doubleValue);
				DataRows = listSorted.ToList();
			}
		}
		public void SortHighestToLowestOnDouble()
		{
			if (DataRows.Any())
			{
				var listSorted = DataRows.OrderByDescending(dr => dr.doubleValue);
				DataRows = listSorted.ToList();
			}
		}
		public void SortHighestToLowestOnDate()
		{
			if (DataRows.Any())
			{
				var listSorted = DataRows.OrderByDescending(dr => dr.dateTimeValue);
				DataRows = listSorted.ToList();
			}
		}
		public void SortHighestToLowestOnInt()
		{
			if (DataRows.Any())
			{
				var listSorted = DataRows.OrderByDescending(dr => dr.intValue);
				DataRows = listSorted.ToList();
			}
		}
		public void SortLowestToHighestOnDouble()
		{
			if (DataRows.Any())
			{
				var listSorted = DataRows.OrderBy(dr => dr.doubleValue);
				DataRows = listSorted.ToList();
			}
		}
		public void SortLowestToHighestOnInt()
		{
			if (DataRows.Any())
			{
				var listSorted = DataRows.OrderBy(dr => dr.intValue);
				DataRows = listSorted.ToList();
			}
		}
		public void SortLowestToHighestOnDate()
		{
			if (DataRows.Any())
			{
				var listSorted = DataRows.OrderBy(dr => dr.dateTimeValue);
				DataRows = listSorted.ToList();
			}
		}

		public void AppendString(string str)
		{
			for (int i = 0; i < RowCount(); ++i)
			{
				PropertyString ps = new PropertyString(str);
				AddProperty(i, ps);
			}
		}

		public void AppendPerUnitProperty(int propDividend, int propDivisor)
		{
			for (int i = 0; i < RowCount(); ++i)
			{
				Property pDividend = GetProperty(i, propDividend);
				Property pDivisor = GetProperty(i, propDivisor);
				if (pDividend != null
					&& pDivisor != null
					&& pDividend.doubleValue.HasValue
					&& pDivisor.doubleValue.HasValue)
				{
					double dDividend = pDividend.doubleValue.Value;
					double dDivisor = pDivisor.doubleValue.Value;

					PropertyDouble pd = new PropertyDouble(dDivisor == 0.0 ? 0.0 : dDividend / dDivisor);
					AddProperty(i, pd);
				}
			}
		}

		public void PrepareForChartingOnDouble(int NumberOfPoints)
		{
			SortHighestToLowestOnDouble();
			Take(NumberOfPoints);
			if (DataRows.Any())
			{
				PropertyManager dr = GetFirstRow();
				if (dr.doubleValue == 0d)
					DataRows.Clear();
				else
					SortLowestToHighestOnDouble();
			}
		}
		public void PrepareForChartingOnDate(int NumberOfPoints)
		{
			SortHighestToLowestOnDate();
			Take(NumberOfPoints);
			SortLowestToHighestOnDate();
		}

		/// <summary>
		/// We want to return if we should use long number formats with the trailing .00s.
		/// </summary>
		/// <returns></returns>
		public bool IsLongNumberFormat(int propertyNumber)
		{
			return IsLongNumberFormat(propertyNumber, 0d, 20d);
		}
		public bool IsLongNumberFormat(int propertyNumber, double minimumThreshold, double maximumThreshold)
		{
			foreach(var item in DataRows)
			{
				Property p = item.GetProperty(propertyNumber);
				if (p.IsNumberType())
				{
					double? d = p.doubleValue;
					if (d.HasValue && (d < minimumThreshold || d > maximumThreshold))
						return false;
				}
				else
				{
					break;
				}
			}

			return true;
		}

		public string GetFormatString(int propertyNumber)
		{
			return GetFirstProperty(propertyNumber).GetFormatString(IsLongNumberFormat(propertyNumber));
		}
		// The formatNumber is the offset for the string.format() function.
		// The propertyNumber is the property to retrieve any extra formatting we want done. Like numbers.
		// i.e., formatNumber:propertyNumber = {3:#,###.00}
		public string GetFormatString(int formatNumber, int propertyNumber)
		{
			string str = GetFormatString(propertyNumber);
			if (!string.IsNullOrEmpty(str))
				str = ":" + str;

			str = formatNumber.ToString() + str;

			return str.WrapInBraces();
		}
		public string CurrencyFormatString(int doubleFieldOffset)
		{
			string strFormat = "C";

			int precisionMax = 0;
			double max = GetMaxWithPrecision(doubleFieldOffset, out precisionMax);

			if (max > 0 && max < 1)
			{
				//strFormat += precisionMax.ToString();
				strFormat += precisionMax.ToString();
			}
			else if(max > 1000)
			{
				strFormat += "0";
			}

			return strFormat;
		}
		public string NumberFormatLT0(int doubleFieldOffset, string formatStart)
		{
			string strFormat = (string.IsNullOrEmpty(formatStart) ? "N" : formatStart);

			int precisionMax = 0;
			double max = GetMaxWithPrecision(doubleFieldOffset, out precisionMax);

			if(max > 0 && max < 1)
			{
				//strFormat += precisionMax.ToString();
				strFormat = ".";
				strFormat += string.Empty.Prefill("0", precisionMax);
			}
			else
			{
				strFormat += "0";
			}

			return strFormat;
		}
		private double GetMaxWithPrecision(int doubleFieldOffset, out int precisionMax)
		{
			precisionMax = 0;

			double max = 0;
			double minNot0 = 0;
			int i = 0;
			for (i = 0; i < RowCount(); ++i)
			{
				Property prop = GetProperty(i, doubleFieldOffset);
				double d = prop.doubleValueOrDefault;

				if (d > max)
					max = d;

				if (minNot0 == 0)
					minNot0 = d;
				else if (d < minNot0)
					minNot0 = d;
			}

			if (max > 0 && max < 1)
			{
				for (i = 0; i < RowCount(); ++i)
				{
					Property prop = GetProperty(i, doubleFieldOffset);
					double d = prop.doubleValueOrDefault;
					string testdec = Convert.ToString(d);
					int s = (testdec.IndexOf(".") + 1); // the first numbers plus decimal point
					int precision = ((testdec.Length) - s);     //total length minus beginning numbers and decimal = number of decimal points
					if (precision > precisionMax)
						precisionMax = precision;
				}

				//string testdec = Convert.ToString(max);
				//int s = (testdec.IndexOf(".") + 1); // the first numbers plus decimal point
				//int precision = ((testdec.Length) - s);     //total length minus beginning numbers and decimal = number of decimal points
				if (precisionMax < 2)
					precisionMax = 2;
			}

			return max;
		}
	}
}
