using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Conversions;
using System.Drawing;
using skky.Types;
using skky.util;

namespace skkyWeb.Charts
{
	public class SeriesDataRowManager : SeriesData
	{
        public delegate string HREFDelegate(int offset, PropertyManager pm);
		public SeriesDataRowManager(DataRowManager dm, SeriesSettings ds)
			: base(ds)
		{
			RowManager = dm;
			Settings = ds;
		}

        public HREFDelegate myDelegate = null;

		public override StringObject getPoint(int offset)
		{
			StringObject soa = new StringObject();
			PropertyManager dr = GetDataRow(offset);
			int yAxisPropertyNumber = GetYAxisPropertyNumber();
			Property prop = dr.GetProperty(GetXAxisPropertyNumber());
			string product = prop.stringValue;

			product = product.Left(GetMaxDataDisplayLength());
			if (string.IsNullOrEmpty(product))
				product = "N/A";

			soa.stringValue = product;

			if (Property.IsDoubleType(GetYAxisFieldDesc().PropertyType))
			{
				if (!dr.GetProperty(yAxisPropertyNumber).doubleValue.HasValue)
					return null;
				soa.objValue = dr.GetProperty(yAxisPropertyNumber).doubleValue;
			}
			else if (Property.IsLongType(GetYAxisFieldDesc().PropertyType))
			{
				if (!dr.GetProperty(yAxisPropertyNumber).longValue.HasValue)
					return null;
				soa.objValue = dr.GetProperty(yAxisPropertyNumber).longValue;
			}
			else if (GetYAxisFieldDesc().IsNumber())
			{
				if (!dr.GetProperty(yAxisPropertyNumber).intValue.HasValue)
					return null;
				soa.objValue = dr.GetProperty(yAxisPropertyNumber).intValue;
			}
			else if (Property.IsStringType(GetYAxisFieldDesc().PropertyType))
			{
				soa.objValue = dr.GetProperty(yAxisPropertyNumber).stringValue;
			}
			else if (Property.IsDateTimeType(GetYAxisFieldDesc().PropertyType))
			{
				if (!dr.GetProperty(yAxisPropertyNumber).dateTimeValue.HasValue)
					return null;
				soa.objValue = dr.GetProperty(yAxisPropertyNumber).dateTimeValueOrDefault.ToShortDateString();
			}

			return soa;
		}

		protected override Property GetTotalOnce()
		{
			int propertyNum = GetNumberPropertyNumber();
			Property total = GetNewPropertyFromNumberAxis();

			if (total.IsDoubleType())
			{
				double dtotal = 0d;
				for (int i = 0; i < RowCount(); ++i)
				{
					Property p = GetProperty(i, propertyNum);
					dtotal += p.doubleValueOrDefault;
				}

				total.doubleValue = dtotal;
			}
			else
			{
				int itotal = 0;
				for (int i = 0; i < RowCount(); ++i)
				{
					Property p = GetProperty(i, propertyNum);
					itotal += p.intValueOrDefault;
				}

				total.intValue = itotal;
			}

			return total;
		}

		private int GetNumberPropertyNumber()
		{
			int propertyNum = GetYAxisPropertyNumber();
			if (!GetYAxisFieldDesc().IsNumber())
				propertyNum = GetXAxisPropertyNumber();

			return propertyNum;
		}
		private int GetStringPropertyNumber()
		{
			int propertyNum = GetXAxisPropertyNumber();
			if (!GetYAxisFieldDesc().IsNumber())
				propertyNum = GetYAxisPropertyNumber();

			return propertyNum;
		}
		private Property GetNewPropertyFromNumberAxis()
		{
			if (GetYAxisFieldDesc().IsNumber())
				return Property.GetNewProperty(GetYAxisPropertyType());

			return Property.GetNewProperty(GetXAxisPropertyType());
		}

		public string GetStringProperty(int offset)
		{
			return rowManager.GetProperty(offset, GetStringPropertyNumber()).stringValue;
		}
		public void SetStringProperty(int offset, string str)
		{
			Property p = rowManager.GetProperty(offset, GetStringPropertyNumber());
			p.stringValue = str;
		}
		public double GetNumberProperty(int offset)
		{
			return rowManager.GetProperty(offset, GetNumberPropertyNumber()).doubleValueOrDefault;
		}

		public double PropertyIsWhatPercentageOfTheWhole(int offset)
		{
			Property p = GetProperty(offset, GetNumberPropertyNumber());
			if (p.IsIntType() && p.intValue.HasValue && Total.intValue.HasValue && Total.intValueOrDefault != 0)
				return p.intValueOrDefault / Total.intValueOrDefault;

			if(!Total.doubleValue.HasValue || Total.doubleValueOrDefault == 0)
				return 0;

			return p.doubleValueOrDefault / Total.doubleValueOrDefault;
		}

		private DataRowManager rowManager;
		private DataRowManager RowManager
		{
			get
			{
				return rowManager;
			}

			set
			{
				rowManager = value;
				if (rowManager == null)
					throw new Exception("SeriesData must reference a valid DataRowManager.");
			}
		}

		public override int RowCount()
		{
			return RowManager.RowCount();
		}
		public PropertyManager GetDataRow(int offset)
		{
			return RowManager.GetDataRow(offset);
		}
		public Property GetProperty(int offset, int propertyNumber)
		{
			return RowManager.GetProperty(offset, propertyNumber);
		}

		public override string GetYAxisFormatString()
		{
			return GetYAxisFormatString(RowManager.IsLongNumberFormat(GetYAxisPropertyNumber()));
		}
		public Property GetXAxisProperty(int offset)
		{
			return RowManager.GetProperty(offset, GetXAxisPropertyNumber());
		}
		public Property GetYAxisProperty(int offset)
		{
			return RowManager.GetProperty(offset, GetYAxisPropertyNumber());
		}

		public override string GetHREF(int offset)
		{
			string str = string.Empty;
			string format = Settings.HREF;
			PropertyManager dp = GetDataRow(offset);

			if (myDelegate != null)
			{
				str = myDelegate(offset, dp);
			}
			else if (!string.IsNullOrEmpty(format))
			{
				if (Settings.HREFIsJavascript)
				{
					if (format.Contains("{") && format.Contains("}"))
					{
						str += string.Format(format, dp.GetObjects(Settings.HREFPropertyNumbers));
					}
					else
					{
						str += format;	// Name of the JavaScript function.
						str += "(this";

						List<Property> props = dp.GetProperties(Settings.HREFPropertyNumbers);
						foreach (var prop in props)
						{
							str += ", ";

							if(prop.IsNumberType())
								str += prop.stringValue;
							else
								str += prop.stringValue.WrapInSingleQuotes();
						}

						str += ");";
					}
				}
				else
				{
					if (format.Contains("{") && format.Contains("}"))
					{
						str += string.Format(format, dp.GetObjects(Settings.HREFPropertyNumbers));
					}
					else
					{
						str = format;
					}
				}
			}

			return str;
		}

		public override string GetToolTip(int offset)
		{
			// Default to sending the ToolTipFormat.
			string format = (Settings.ToolTipFormat ?? string.Empty);
			if (!string.IsNullOrEmpty(format) && Settings.ToolTipPropertyNumbers != null && Settings.ToolTipPropertyNumbers.Count() > 0)
			{
				PropertyManager dp = GetDataRow(offset);

				// No ToolTipFormat but we have ToolTipPropertyNumbers, just build the default format string.
				if (string.IsNullOrEmpty(format))
				{
					int count = 0;
					List<Property> props = dp.GetProperties(Settings.ToolTipPropertyNumbers);
					foreach (var prop in props)
					{
						if (count > 0)
							format += " - ";

						format += count.ToString().WrapInBraces();
						++count;
					}
				}

				if (format.Contains("{") && format.Contains("}"))	// at least one format parameter
					return string.Format(format, dp.GetObjects(Settings.ToolTipPropertyNumbers));
			}

			return format;
		}

		public override string GetFormatString()
		{
			return GetFormatString(GetYAxisPropertyNumber());
		}
		public string GetFormatString(int propertyNumber)
		{
			return RowManager.GetFormatString(propertyNumber);
		}
		protected void ConvertToMetricInt(IConversion converter)
		{
			int propertyNumber = GetYAxisPropertyNumber();
			for (int i = 0; i < rowManager.RowCount(); ++i)
			{
				Property p = rowManager.GetProperty(i, propertyNumber);
				if (p != null)
				{
					double d = converter.ConvertToMetric(p.intValueOrDefault);
					p.intValue = (int)d;
				}
			}
		}
		protected void ConvertToStandardInt(IConversion converter)
		{
			int propertyNumber = GetYAxisPropertyNumber();
			for (int i = 0; i < rowManager.RowCount(); ++i)
			{
				Property p = rowManager.GetProperty(i, propertyNumber);
				if (p != null)
				{
					double d = converter.ConvertToStandard(p.intValueOrDefault);
					p.intValue = (int)d;
				}
			}
		}
		protected void ConvertToMetricDouble(IConversion converter)
		{
			int propertyNumber = GetYAxisPropertyNumber();
			for (int i = 0; i < rowManager.RowCount(); ++i)
			{
				Property p = rowManager.GetProperty(i, propertyNumber);
				if (p != null)
				{
					double d = converter.ConvertToMetric(p.doubleValueOrDefault);
					p.doubleValue = d;
				}
			}
		}
		protected void ConvertToStandardDouble(IConversion converter)
		{
			int propertyNumber = GetYAxisPropertyNumber();
			for (int i = 0; i < rowManager.RowCount(); ++i)
			{
				Property p = rowManager.GetProperty(i, propertyNumber);
				if (p != null)
				{
					double d = converter.ConvertToStandard(p.doubleValueOrDefault);
					p.doubleValue = d;
				}
			}
		}

		public void ConvertIntUnits(bool dbFieldIsMetric, bool wantMetric)
		{
			IConversion converter = ConversionBase.GetConversionObjectFromIdentifier(Settings.Conversion);
			if (!dbFieldIsMetric && wantMetric)
				ConvertToMetricInt(converter);
			else if (dbFieldIsMetric && !wantMetric)
				ConvertToStandardInt(converter);
		}
		public void ConvertDoubleUnits(bool dbFieldIsMetric, bool wantMetric)
		{
			IConversion converter = ConversionBase.GetConversionObjectFromIdentifier(Settings.Conversion);
			if (!dbFieldIsMetric && wantMetric)
				ConvertToMetricDouble(converter);
			else if (dbFieldIsMetric && !wantMetric)
				ConvertToStandardDouble(converter);
		}
	}
}
