using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.util;

namespace skky.Types
{
	[DataContract]
	public class DataTypeDescription
	{
		public enum DataUsages
		{
			Nothing,
			Currency,
			Date,
			Number,
			Percentage,
			String
		};

		public DataTypeDescription(int propertyNum, System.Type dt)
			: this(propertyNum, dt, GetBestDataUsageGuess(dt))
		{ }
		public DataTypeDescription(int propertyNum, System.Type dt, DataUsages du)
		{
			PropertyNumber = propertyNum;
			PropertyType = dt;
			DataUsage = du;
		}

		[DataMember]
		public System.Type PropertyType { get; set; }

		[DataMember]
		public DataUsages DataUsage { get; set; }

		[DataMember]
		public int PropertyNumber { get; set; }

		public bool IsTypeNothing()
		{
			return IsDataUsageNothing();
		}
		public bool IsDataUsageNothing()
		{
			return DataUsage == DataUsages.Nothing;
		}

		public bool IsPercentage()
		{
			return DataUsage == DataUsages.Percentage;
		}
		public bool IsCurrency()
		{
			return DataUsage == DataUsages.Currency;
		}
		public bool IsNumber()
		{
			return DataUsage == DataUsages.Number;
		}

		public bool IsUsedAsNumber()
		{
			return (IsNumber() || IsCurrency() || IsPercentage());
		}

		public static DataTypeDescription GetDate(int PropertyNumber = 0)
		{
			return new DataTypeDescription(PropertyNumber, typeof(DateTime), DataUsages.Date);
		}
		public static DataTypeDescription GetPercentage(int PropertyNumber = 0)
		{
			return new DataTypeDescription(PropertyNumber, typeof(double), DataUsages.Percentage);
		}
		public static DataTypeDescription GetCurrency(int PropertyNumber = 0)
		{
			return new DataTypeDescription(PropertyNumber, typeof(double), DataUsages.Currency);
		}
		public static DataTypeDescription GetNumberDouble(int PropertyNumber = 0)
		{
			return new DataTypeDescription(PropertyNumber, typeof(double), DataUsages.Number);
		}
		public static DataTypeDescription GetNumberInt(int PropertyNumber = 0)
        {
            return new DataTypeDescription(PropertyNumber, typeof(int), DataUsages.Number);
        }
		public static DataTypeDescription GetNumberLong(int PropertyNumber = 0)
        {
            return new DataTypeDescription(PropertyNumber, typeof(long), DataUsages.Number);
        }
		public static DataTypeDescription GetString(int PropertyNumber = 0)
		{
			return new DataTypeDescription(PropertyNumber, typeof(string), DataUsages.String);
		}

		private string formatString;
		[DataMember]
		public string FormatString
		{
			get
			{
				return formatString;
			}
			set
			{
				formatString = value;
			}
		}

		public void SetFormatString(bool isLongNumberFormat)
		{
			string str = Property.GetFormatString(PropertyType, isLongNumberFormat);
			if (IsNumber())
			{
				if (IsCurrency())
					str = "$" + str;

				if (IsPercentage())
					str += "%";
			}

			formatString = str;
		}
		public void SetFormatString(int offset)
		{
			SetFormatString(offset, false);
		}
		public void SetFormatString(int offset, bool isLongNumberFormat)
		{
			string str = offset.ToString();
			if (IsNumber())
			{
				SetFormatString(isLongNumberFormat);
				str += ":" + formatString;
			}

			formatString = str.WrapInBraces();
		}

		private static DataUsages GetBestDataUsageGuess(System.Type t)
		{
			if (Property.IsDateTimeType(t))
				return DataUsages.Date;
			else if (Property.IsDoubleType(t))
				return DataUsages.Number;
			else if (Property.IsIntType(t))
				return DataUsages.Number;

			return DataUsages.String;
		}
	}
}
