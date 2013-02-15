using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.Types;
using System.Drawing;
using skky.Conversions;

namespace skkyWeb.Charts
{
	[DataContract]
	public class SeriesSettings
	{
		// Chart types
		public const string Const_BarChart = "Bar";
		public const string Const_ColumnChart = "Column";
		public const string Const_PieChart = "Pie";
		public const string Const_SplineChart = "Spline";
		public const string Const_StepLineChart = "StepLine";

		public static readonly string[] Const_PieLabelStyles =
		{
			"Disabled",
			"Inside",
			"Outside",
			"Ellipse",
		};

		public SeriesSettings()
		{
			SetDefaults();
		}
		public SeriesSettings(string xAxisTitle, int xAxisPropertyNumber, string yAxisTitle, int yAxisPropertyNumber, ConversionBase.ConversionIdentifiers ci)
			: this()
		{
			XAxis.Title = xAxisTitle;
			XAxis.FieldDescription.PropertyNumber = xAxisPropertyNumber;
			YAxis.Title = yAxisTitle;
			YAxis.FieldDescription.PropertyNumber = yAxisPropertyNumber;
			Conversion = ci;
		}

		public void SetDefaults()
		{
			BorderWidth = 1;
			ChartType = Const_BarChart;
			MaxDataDisplayLength = 12;
			Conversion = ConversionBase.ConversionIdentifiers.None;

			XAxis = new AxisSettings();
			YAxis = new AxisSettings(AxisType.YAxis);
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string ChartType { get; set; }

		[DataMember]
		public string HREF { get; set; }
		[DataMember]
		public IEnumerable<int> HREFPropertyNumbers { get; set; }
		[DataMember]
		public IEnumerable<string> HREFPropertyNames { get; set; }
		[DataMember]
		public bool HREFIsJavascript { get; set; }

		[DataMember]
		public int MarkerSize { get; set; }

		[DataMember]
		public string MarkerColor { get; set; }
		[DataMember]
		public string MarkerBorderColor { get; set; }

		[DataMember]
		public string MainColor { get; set; }

		[DataMember]
		public string BorderColor { get; set; }

		[DataMember]
		public int BorderHeight { get; set; }

		[DataMember]
		public int BorderWidth { get; set; }

		[DataMember]
		public string Category { get; set; }

		[DataMember]
		public string Classification { get; set; }

		[DataMember]
		public string TimeFrame { get; set; }

		[DataMember]
		public string PieLabelStyle { get; set; }

		[DataMember]
		public string ToolTipFormat { get; set; }
		[DataMember]
		public IEnumerable<int> ToolTipPropertyNumbers { get; set; }
		[DataMember]
		public IEnumerable<string> ToolTipPropertyNames { get; set; }

		private AxisSettings xAxis;
		[DataMember]
		public AxisSettings XAxis
		{
			get
			{
				if (null == xAxis)
					xAxis = new AxisSettings();

				return xAxis;
			}
			set
			{
				xAxis = value;
			}
		}

		private AxisSettings yAxis;
		[DataMember]
		public AxisSettings YAxis
		{
			get
			{
				if (null == yAxis)
					yAxis = new AxisSettings();

				return yAxis;
			}
			set
			{
				yAxis = value;
			}
		}

		[DataMember]
		public List<DataPointSettings> DataPointSettingsList { get; set; }

		private List<StripLineSettings> xstripLineSettingsList = null;
		[DataMember]
		public List<StripLineSettings> XStripLineSettingsList {
			get
			{
				if (xstripLineSettingsList == null)
					xstripLineSettingsList = new List<StripLineSettings>();

				return xstripLineSettingsList;
			}
			set
			{
				xstripLineSettingsList = value;
			}
		}

		private List<StripLineSettings> ystripLineSettingsList = null;
		[DataMember]
		public List<StripLineSettings> YStripLineSettingsList {
			get
			{
				if (ystripLineSettingsList == null)
					ystripLineSettingsList = new List<StripLineSettings>();

				return ystripLineSettingsList;
			}
			set
			{
				ystripLineSettingsList = value;
			}
		}

		[DataMember]
		public int MaxPointsToShow { get; set; }

		[DataMember]
		public ConversionBase.ConversionIdentifiers Conversion { get; set; }

		[DataMember]
		public int MaxDataDisplayLength { get; set; }

		public StripLineSettings AddXStripLine(double lower, double upper, Color color)
		{
			StripLineSettings sls = new StripLineSettings(lower, upper, color);
			XStripLineSettingsList.Add(sls);

			return sls;
		}
		public StripLineSettings AddYStripLine(double lower, double upper, Color color)
		{
			StripLineSettings sls = new StripLineSettings(lower, upper, color);
			YStripLineSettingsList.Add(sls);

			return sls;
		}
		public void SetHREFPropertyNumbersToAxis()
		{
			List<int> list = new List<int>();
			list.Add(XAxis.FieldDescription.PropertyNumber);
			list.Add(YAxis.FieldDescription.PropertyNumber);

			HREFPropertyNumbers = list;
		}
		public void SetToolTipPropertyNumbersToAxis()
		{
			List<int> list = new List<int>();
			list.Add(XAxis.FieldDescription.PropertyNumber);
			list.Add(YAxis.FieldDescription.PropertyNumber);

			ToolTipPropertyNumbers = list;
		}

		public void SetToolTips(string format, string field1, string field2)
		{
			ToolTipFormat = format;

			string[] iarr = { field1, field2 };
			ToolTipPropertyNames = iarr.ToList();
		}
		public void SetToolTips(string format, string field1, string field2, string field3)
		{
			ToolTipFormat = format;

			string[] iarr = { field1, field2, field3 };
			ToolTipPropertyNames = iarr.ToList();
		}
		public void SetToolTips(string format, int offset1, int offset2)
		{
			ToolTipFormat = format;

			int[] iarr = { offset1, offset2 };
			ToolTipPropertyNumbers = iarr.ToArray();
		}
		public void SetToolTips(string format, int offset1, int offset2, int offset3)
		{
			ToolTipFormat = format;

			int[] iarr = { offset1, offset2, offset3 };
			ToolTipPropertyNumbers = iarr.ToArray();
		}
		public void SetToolTips(string format, int offset1, int offset2, int offset3, int offset4)
		{
			ToolTipFormat = format;

			int[] iarr = { offset1, offset2, offset3, offset4 };
			ToolTipPropertyNumbers = iarr.ToArray();
		}

		public void SetJavascriptMethod(string str)
		{
			HREF = str;
			HREFIsJavascript = true;

			//SetHREFPropertyNumbersToAxis();
		}
		public void SetJavascriptMethod(string str, int offset1, int offset2)
		{
			HREF = str;
			HREFIsJavascript = true;

			SetHREFs(offset1, offset2);
		}
		public void SetJavascriptMethod(string str, int offset1, int offset2, int offset3)
		{
			HREF = str;
			HREFIsJavascript = true;

			SetHREFs(offset1, offset2, offset3);
		}

		public static string[] GetChartTypes()
		{
			string[] charts = new string[4];
			charts[0] = Const_BarChart;
			charts[1] = Const_ColumnChart;
			charts[2] = Const_PieChart;
			charts[3] = Const_SplineChart;

			return charts;
		}
		public static string GetChartTypeNormalized(string type)
		{
			if (string.IsNullOrEmpty(type))
				return Const_BarChart;

			foreach (var str in GetChartTypes())
			{
				if (str.ToLower() == type.ToLower())
					return str;
			}

			return Const_BarChart;
		}

		public void SetHREFs(int offset1, int offset2)
		{
			int[] iarr = { offset1, offset2 };
			HREFPropertyNumbers = iarr.ToArray();
		}
		public void SetHREFs(int offset1, int offset2, int offset3)
		{
			int[] iarr = { offset1, offset2, offset3 };
			HREFPropertyNumbers = iarr.ToArray();
		}

		public bool IsBarChart()
		{
			return ChartType == Const_BarChart;
		}
		public bool IsColumnChart()
		{
			return ChartType == Const_ColumnChart;
		}
		public bool IsPieChart()
		{
			return ChartType == Const_PieChart;
		}
		public bool IsSplineChart()
		{
			return ChartType == Const_SplineChart;
		}
		public bool IsStepLineChart()
		{
			return ChartType == Const_StepLineChart;
		}
	}
}
