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
	public class SeriesData
	{
		public SeriesData(SeriesSettings ds)
		{
			Settings = ds;
		}

		public virtual int RowCount()
		{
			return 0;
		}

		public virtual StringObject getPoint(int offset)
		{
			return null;
		}

		protected Property propTotal;
		public Property Total
		{
			get
			{
				if (propTotal == null)
				{
					propTotal = GetTotalOnce();
				}

				return propTotal;
			}
			set
			{
				propTotal = value;
			}
		}

		protected virtual Property GetTotalOnce()
		{
			return new PropertyInt();
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

		private SeriesSettings seriesSettings;
		public SeriesSettings Settings
		{
			get
			{
				return seriesSettings;
			}

			set
			{
				seriesSettings = value;
				if (seriesSettings == null)
				{
					throw new Exception("SeriesData must have valid SeriesSettings.");
				}
				else
				{
					MainColor = seriesSettings.MainColor.ToColor(Color.Blue);
					BorderColor = seriesSettings.BorderColor.ToColor(Color.Gray);
					MarkerColor = seriesSettings.MarkerColor.ToColor(Color.Orange);
					MarkerBorderColor = seriesSettings.MarkerBorderColor.ToColor(Color.Green);
				}
			}
		}

		public int GetMaxDataDisplayLength()
		{
			return Settings.MaxDataDisplayLength;
		}

		public Color[] PaletteCustomColors;

		protected List<DataPointSettingsWithObjects> dataPointSettingsList;
		public List<DataPointSettingsWithObjects> DataPointSettingsList
		{
			get
			{
				if (dataPointSettingsList == null)
				{
					if (Settings.DataPointSettingsList != null && Settings.DataPointSettingsList.Count() > 0)
					{
						dataPointSettingsList = new List<DataPointSettingsWithObjects>();
						foreach (var dps in Settings.DataPointSettingsList)
							dataPointSettingsList.Add(new DataPointSettingsWithObjects(dps));
					}
				}

				return dataPointSettingsList;
			}

			set
			{
				dataPointSettingsList = value;
			}
		}

		public DataPointSettingsWithObjects GetDataPointSettings(int offset)
		{
			if (DataPointSettingsList != null && DataPointSettingsList.Count() > offset)
				return DataPointSettingsList.ElementAt(offset);

			return null;
		}

		public int GetXAxisPropertyNumber()
		{
			return Settings.XAxis.FieldDescription.PropertyNumber;
		}
		public int GetYAxisPropertyNumber()
		{
			return Settings.YAxis.FieldDescription.PropertyNumber;
		}
		public System.Type GetXAxisPropertyType()
		{
			return Settings.XAxis.FieldDescription.PropertyType;
		}
		public System.Type GetYAxisPropertyType()
		{
			return Settings.YAxis.FieldDescription.PropertyType;
		}
		public DataTypeDescription GetXAxisFieldDesc()
		{
			return Settings.XAxis.FieldDescription;
		}
		public DataTypeDescription GetYAxisFieldDesc()
		{
			return Settings.YAxis.FieldDescription;
		}

		public virtual string GetYAxisFormatString()
		{
			return GetYAxisFormatString(false);
		}
		public string GetYAxisFormatString(bool isLongNumber)
		{
			if(null != Settings.YAxis
			 && null != Settings.YAxis.FieldDescription
			 && string.IsNullOrEmpty(Settings.YAxis.FieldDescription.FormatString))
				Settings.YAxis.SetFormatString(isLongNumber);
			return Settings.YAxis.FieldDescription.FormatString;
		}
		public string GetXAxisTitle()
		{
			return Settings.XAxis.Title;
		}
		public string GetYAxisTitle()
		{
			return Settings.YAxis.Title;
		}

		public int GetBorderWidth()
		{
			return Settings.BorderWidth;
		}
		public int GetMaxPointsToShow()
		{
			return Settings.MaxPointsToShow;
		}
		public int GetMarkerSize()
		{
			return Settings.MarkerSize;
		}
		public string GetChartType()
		{
			return Settings.ChartType;
		}
		public string GetPieLabelStyle()
		{
			return Settings.PieLabelStyle;
		}
		public string GetName()
		{
			return Settings.Name;
		}

		public Color MainColor { get; private set; }
		public Color BorderColor { get; private set; }
		public Color MarkerColor { get; private set; }
		public Color MarkerBorderColor { get; private set; }

		public virtual string GetHREF(int offset)
		{
			return string.Empty;
		}

		public virtual string GetToolTip(int offset)
		{
			return string.Empty;
		}

		public virtual string GetFormatString()
		{
			return "N0";
		}
	}
}
