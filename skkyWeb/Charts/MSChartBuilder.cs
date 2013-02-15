using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using skky.Types;
using skky.util;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;

namespace skkyWeb.Charts
{
	public class MSChartBuilder : Chart
	{
		public const string Const_ChartIdPrefix = "chart";
		public const string Const_DefaultChartAreaName = "A";

		public static readonly Color colorGreenLight = Color.FromArgb(0xc4, 0xf0, 0xcb);
		public static readonly Color colorGreenDark = Color.FromArgb(0x22, 0x88, 0x33);
		public static readonly Color Const_DefaultBackgroundColorBegin = colorGreenDark;
		public static readonly Color Const_DefaultBackgroundColorEnd = colorGreenLight;
		//public static Color Const_DefaultBackgroundColorBegin = Color.FromArgb(0x33, 0x44, 0xaa);
		//public static Color Const_DefaultBackgroundColorEnd = Color.FromArgb(0xc6, 0xcc, 0xee);

		public const float Const_MinimumFontSize = 6;
		public const float Const_MinimumTitleFontSize = 7;

		private static int chartCounter = 0;
		private static int seriesCounter = 0;

		protected bool setDataPointColors;

		protected ChartSettings chartSettings;

		private MSChartBuilder()
		{ }

		public MSChartBuilder(ChartSettings cs)
		{
			chartSettings = cs;
		}

		public string getID()
		{
			return ID;
		}

		public void init(SeriesData seriesData)
		{
			List<SeriesData> sd = new List<SeriesData>();
			if (seriesData != null)
				sd.Add(seriesData);

			init(sd);
		}

		public void init(IEnumerable<SeriesData> seriesData)
		{
			int myChartId = ++chartCounter;
			this.ID = Const_ChartIdPrefix + myChartId.ToString();

			int width = chartSettings.Width;
			int height = chartSettings.Height;
			if (width < 1)
				width = 500;
			if (height < 1)
				height = 300;

			setChartBackgroundBlank();
			//ImageUrl = GetChartImageURL();

			Height = GetPixelUnit(height);
			Width = GetPixelUnit(width);

			SetTitle(chartSettings.Title);

			ChartArea ca = CreateChartArea(seriesData);

			string legendName = string.Empty;
			Legends.Clear();
			if (chartSettings.ShowLegend)
			{
				legendName = "Legend1";
				Legends.Add(new Legend()
				{
					Enabled = true,
					IsTextAutoFit = true,
					BackColor = Color.Transparent,
					Name = legendName,
					IsDockedInsideChartArea = true,
					DockedToChartArea = ca.Name,
					Docking = System.Web.UI.DataVisualization.Charting.Docking.Right,
					Alignment = StringAlignment.Far,

				});
			}
			int seriesNumber = 0;
			foreach (var item in seriesData)
			{
				CreateSeries(item, seriesNumber, seriesData.Count(), legendName);
				++seriesNumber;
			}
		}

		public Series CreateSeries(SeriesData sd, int currentSeriesNumber, int totalNumberOfSeries, string legendName)
		{
			if (sd == null)
				throw new Exception("To create a Series, you must have SeriesData.");

			string name = sd.GetName();
			if (string.IsNullOrEmpty(name))
				name = "Series"; // +DateTime.Now.Ticks.ToString();

			foreach (Series ser in Series)
			{
				if (ser.Name == name)
				{
					++seriesCounter;
					name += seriesCounter.ToString();
					break;
				}
			}

			Series series = new Series()
			{
				Name = name,
				//Type = SeriesGetChartType().Bar,
				ChartType = GetSeriesChartType(sd.GetChartType()),
				//ChartType = chartType,
				//XValueType = GetChartValueType(sd.GetXAxisPropertyType()),
				//YValueType = GetChartValueType(sd.GetYAxisPropertyType()),
				//ChartArea = chartArea.Name,
				//ToolTip = "#VALY",
			};

			if (!string.IsNullOrEmpty(legendName))
			{
				series.Legend = "Legend1";
				series.LegendText = sd.GetYAxisTitle();
			}

			Series.Add(series);

			if (sd.MainColor != Color.Empty)
				series.Color = sd.MainColor;
			if (sd.BorderColor != Color.Empty)
				series.BorderColor = sd.BorderColor;
			if (sd.GetBorderWidth() >= 0)
				series.BorderWidth = sd.GetBorderWidth();
			series.BorderDashStyle = ChartDashStyle.Solid;

			if (!string.IsNullOrEmpty(sd.GetPieLabelStyle()))
				series.SetCustomProperty("PieLabelStyle", sd.GetPieLabelStyle());
			//series.SetAttribute("LabelsRadialLineSize", "2");

			//series.SmartLabels.Enabled = true;
			//series.SmartLabels.CalloutStyle = LabelCalloutStyle.Box;
			//series.SmartLabels.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Yes;
			//series.LabelBorderWidth = 50;
			//series.SmartLabels.HideOverlapped = true;

			populateSeries(series, sd, chartSettings.EnableHref, chartSettings.EnableToolTips);

			string yaxisFormatString = sd.GetYAxisFormatString();
			if (!string.IsNullOrEmpty(yaxisFormatString))
				series.LabelFormat = yaxisFormatString;

			if (sd.GetMarkerSize() > 0)
			{
				series.MarkerSize = sd.GetMarkerSize();
				series.MarkerColor = sd.MarkerColor;
				series.MarkerBorderColor = sd.MarkerBorderColor;
				series.MarkerStyle = MarkerStyle.Diamond;
			}

			ChartArea chartArea = ChartAreas[0];

			int seriesCount = Series.Count;
			if (seriesCount == 1)
			{
				chartArea.AxisX.LabelStyle.IsEndLabelVisible = true;

				//chartArea.AxisY.Title = sd.GetYAxisTitle();
				//chartArea.AxisY.TitleColor = Color.Black;
				if (!string.IsNullOrEmpty(yaxisFormatString))
					chartArea.AxisY.LabelStyle.Format = yaxisFormatString;

				chartArea.AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;

				chartArea.AxisY.LineColor = Color.Black;
				//chartArea.AxisY.TitleFont = new Font("Trebuchet MS", 10, FontStyle.Bold);
				chartArea.AxisY.LabelStyle.Font = new Font("Trebuchet MS", 8, FontStyle.Bold);
				if (totalNumberOfSeries == 1)
					chartArea.AxisY.LabelStyle.ForeColor = Color.Black;
				else
					chartArea.AxisY.LabelStyle.ForeColor = sd.MainColor;

				//chartArea.BorderColor = Color.Black;
				//chartArea.BorderStyle = ChartDashStyle.Solid;
				//chartArea.BorderWidth = 1;
			}
			else
			{
				//chartArea.AxisY.LineColor = sd.MainColor;
				//chartArea.AxisY.LabelStyle.FontColor = sd.MainColor;
				//if(currentSeriesNumber == 1)
				//    CreateY2Axis(chartArea, series, sd.MainColor, sd.GetYAxisTitle(), sd.Settings.YAxis.AxisOffset, sd.Settings.YAxis.LabelSize);
				//else
					CreateYAxis(chartArea, series, sd.MainColor, sd.GetYAxisTitle(), sd.Settings.YAxis.AxisOffset, sd.Settings.YAxis.LabelSize);
			}

			SetStripLines(chartArea.AxisX, sd.Settings.XStripLineSettingsList);
			SetStripLines(chartArea.AxisY, sd.Settings.YStripLineSettingsList);

			return series;
		}

		public ChartArea CreateYAxis(ChartArea area, Series series, Color axisColor
			, string axisName, float axisOffset, float labelsSize)
		{
			return CreateYAxis(area, series, axisColor, axisColor, axisName, axisOffset, labelsSize);
		}
		public ChartArea CreateYAxis(ChartArea area, Series series, Color axisColor, Color axisLineColor, string axisName, float axisOffset, float labelsSize)
		{
			if (axisLineColor == null)
				axisLineColor = axisColor;

			// Create new chart area for original series
			ChartArea areaSeries = ChartAreas.Add("ChartArea_" + series.Name);
			areaSeries.BackColor = Color.Transparent;
			areaSeries.BorderColor = Color.Transparent;

			RectangleF posRectangleF = area.Position.ToRectangleF();
			areaSeries.Position.FromRectangleF(posRectangleF);
			posRectangleF = area.InnerPlotPosition.ToRectangleF();
			areaSeries.InnerPlotPosition.FromRectangleF(posRectangleF);
			areaSeries.AxisX.MajorGrid.Enabled = false;
			areaSeries.AxisX.MajorTickMark.Enabled = false;
			areaSeries.AxisX.LabelStyle.Enabled = false;

			areaSeries.AxisY.MajorGrid.Enabled = false;
			areaSeries.AxisY.MajorTickMark.Enabled = false;
			areaSeries.AxisY.LabelStyle.Enabled = false;
			areaSeries.AxisY.IsStartedFromZero = area.AxisY.IsStartedFromZero;

			series.ChartArea = areaSeries.Name;

			//areaSeries.AxisY.Arrows = ArrowsType.Triangle;

			// Create new chart area for axis
			ChartArea areaAxis = ChartAreas.Add("AxisY_" + series.ChartArea);

			if (axisColor != null)
			{
				Axis axis = areaAxis.AxisY;
				axis.LineColor = axisLineColor;
				axis.LineWidth = 3;
				axis.LabelStyle.ForeColor = axisColor;
				//axis.TitleFont = new Font("Trebuchet MS", 10, FontStyle.Bold);
				axis.LabelStyle.Font = new Font("Trebuchet MS", 8, FontStyle.Bold);
				//axis.TitleColor = axisColor;
				//axis.TitleColor = Color.Black;
				//axis.Title = axisName;
			}

			areaAxis.BackColor = Color.Transparent;
			areaAxis.BorderColor = Color.Transparent;

			posRectangleF = ChartAreas[series.ChartArea].Position.ToRectangleF();
			areaAxis.Position.FromRectangleF(posRectangleF);
			posRectangleF = ChartAreas[series.ChartArea].InnerPlotPosition.ToRectangleF();
			areaAxis.InnerPlotPosition.FromRectangleF(posRectangleF);
			/*
			Axis axis = areaAxis.AxisY;
			axis.Arrows = ArrowsType.Triangle;
			axis.IntervalAutoMode = IntervalAutoMode.FixedCount;
			AxisDataView adv = new AxisDataView();
			adv.Size = 10;
			axis.View = adv;
			*/
			//addCustomLabels(areaAxis.AxisY);

			// Create a copy of specified series
			Series seriesCopy = Series.Add(series.Name + "_Copy");
			seriesCopy.ChartType = series.ChartType;
			foreach (DataPoint point in series.Points)
			{
				seriesCopy.Points.AddXY(point.XValue, point.YValues[0]);
			}

			// Hide copied series
			seriesCopy.IsVisibleInLegend = false;
			seriesCopy.Color = Color.Transparent;
			seriesCopy.BorderColor = Color.Transparent;
			seriesCopy.ChartArea = areaAxis.Name;

			// Disable grid lines & tickmarks
			areaAxis.AxisX.LineWidth = 0;
			areaAxis.AxisX.MajorGrid.Enabled = false;
			areaAxis.AxisX.MajorTickMark.Enabled = false;
			areaAxis.AxisX.LabelStyle.Enabled = false;

			areaAxis.AxisY.MajorGrid.Enabled = false;
			areaAxis.AxisY.IsStartedFromZero = area.AxisY.IsStartedFromZero;
			areaAxis.AxisY.LabelStyle.Format = series.LabelFormat;

			areaAxis.Position.Auto = false;
			// Adjust area position
			// Added checks so that Dundas does not exception on negative Positions.
			if (axisOffset > areaAxis.Position.X)
				areaAxis.Position.X = 0;
			else
			    areaAxis.Position.X -= axisOffset;

			areaAxis.InnerPlotPosition.Auto = false;
			if (labelsSize > areaAxis.InnerPlotPosition.X)
			    areaAxis.InnerPlotPosition.X = 0;
			else
				areaAxis.InnerPlotPosition.X += labelsSize;
			//areaAxis.InnerPlotPosition.X = labelsSize;

			return areaAxis;
		}
		public ChartArea CreateY2Axis(ChartArea area, Series series, Color axisColor
			, string axisName, float axisOffset, float labelsSize)
		{
			return CreateY2Axis(area, series, axisColor, axisColor, axisName, axisOffset, labelsSize);
		}
		public ChartArea CreateY2Axis(ChartArea area, Series series, Color axisColor, Color axisLineColor, string axisName, float axisOffset, float labelsSize)
		{
			if (axisLineColor == null)
				axisLineColor = axisColor;

			RectangleF posRectangleF = area.Position.ToRectangleF();

			// Create new chart area for original series
			ChartArea areaSeries = ChartAreas.Add("ChartArea_" + series.Name);
			areaSeries.BackColor = Color.Transparent;
			areaSeries.BorderColor = Color.Transparent;

			areaSeries.Position.FromRectangleF(posRectangleF);
			posRectangleF = area.InnerPlotPosition.ToRectangleF();
			areaSeries.InnerPlotPosition.FromRectangleF(posRectangleF);
			areaSeries.AxisX.MajorGrid.Enabled = false;
			areaSeries.AxisX.MajorTickMark.Enabled = false;
			areaSeries.AxisX.LabelStyle.Enabled = false;

			areaSeries.AxisY2.Enabled = AxisEnabled.True;
			areaSeries.AxisY2.MajorGrid.Enabled = false;
			areaSeries.AxisY2.MajorTickMark.Enabled = false;
			areaSeries.AxisY2.LabelStyle.Enabled = false;
			areaSeries.AxisY2.IsStartedFromZero = area.AxisY2.IsStartedFromZero;

			series.ChartArea = areaSeries.Name;

			//areaSeries.AxisY.Arrows = ArrowsType.Triangle;

			// Create new chart area for axis
			ChartArea areaAxis = ChartAreas.Add("AxisY2_" + series.ChartArea);

			if (axisColor != null)
			{
				Axis axis = areaAxis.AxisY2;
				axis.LineColor = axisLineColor;
				axis.LineWidth = 3;
				axis.LabelStyle.ForeColor = axisColor;
				axis.TitleFont = new Font("Trebuchet MS", 10, FontStyle.Bold);
				axis.LabelStyle.Font = new Font("Trebuchet MS", 8, FontStyle.Bold);
				//axis.TitleColor = axisColor;
				axis.TitleForeColor = Color.Black;
				axis.Title = axisName;
			}

			areaAxis.BackColor = Color.Transparent;
			areaAxis.BorderColor = Color.Transparent;

			posRectangleF = ChartAreas[series.ChartArea].Position.ToRectangleF();
			areaAxis.Position.FromRectangleF(posRectangleF);
			posRectangleF = ChartAreas[series.ChartArea].InnerPlotPosition.ToRectangleF();
			areaAxis.InnerPlotPosition.FromRectangleF(posRectangleF);
			/*
			Axis axis = areaAxis.AxisY;
			axis.Arrows = ArrowsType.Triangle;
			axis.IntervalAutoMode = IntervalAutoMode.FixedCount;
			AxisDataView adv = new AxisDataView();
			adv.Size = 10;
			axis.View = adv;
			*/
			//addCustomLabels(areaAxis.AxisY);

			// Create a copy of specified series
			Series seriesCopy = Series.Add(series.Name + "_Copy");
			seriesCopy.ChartType = series.ChartType;
			foreach (DataPoint point in series.Points)
			{
				seriesCopy.Points.AddXY(point.XValue, point.YValues[0]);
			}

			// Hide copied series
			seriesCopy.IsVisibleInLegend = false;
			seriesCopy.Color = Color.Transparent;
			seriesCopy.BorderColor = Color.Transparent;
			seriesCopy.ChartArea = areaAxis.Name;

			// Disable grid lines & tickmarks
			areaAxis.AxisX.LineWidth = 0;
			areaAxis.AxisX.MajorGrid.Enabled = false;
			areaAxis.AxisX.MajorTickMark.Enabled = false;
			areaAxis.AxisX.LabelStyle.Enabled = false;

			areaAxis.AxisY2.MajorGrid.Enabled = false;
			areaAxis.AxisY2.IsStartedFromZero = area.AxisY2.IsStartedFromZero;
			areaAxis.AxisY2.LabelStyle.Format = series.LabelFormat;

			//areaAxis.Position.Auto = false;
			// Adjust area position
			// Added checks so that Dundas does not exception on negative Positions.
			//			if (axisOffset > areaAxis.Position.X)
			//areaAxis.Position.X = 0;
			//else
			areaAxis.Position.X = axisOffset;

			//areaAxis.InnerPlotPosition.Auto = false;
			//if (labelsSize > areaAxis.InnerPlotPosition.X)
			//    areaAxis.InnerPlotPosition.X = 0;
			//else
			//	areaAxis.InnerPlotPosition.X += labelsSize;
			//areaAxis.InnerPlotPosition.X = labelsSize;

			return areaAxis;
		}


		protected string GetChartImageURL()
		{
			return "~/TempImages/ChartPic_#SEQ(300,3)";
		}
		static protected string GetDefaultFontName()
		{
			return "Trebuchet MS";
		}

		public void SetStripLines(Axis axis, List<StripLineSettings> slsList)
		{
			if (slsList != null && slsList.Count() > 0)
			{
				foreach (StripLineSettings sls in slsList)
				{
					//using Dundas.Charting.WinControl;

					// Add a new instance of a StripLine along the y-axis.
					axis.StripLines.Add(new StripLine());

					int offset = axis.StripLines.Count() - 1;

					Color color = Color.FromArgb(80, 252, 180, 65);
					// Set the strip line back color. 
					if (!string.IsNullOrEmpty(sls.MainColor))
						color = sls.MainColor.ToColor();
					axis.StripLines[offset].BackColor = color;

					// Set the strip line width.
					if (sls.StripWidth == 0)
						axis.StripLines[offset].StripWidth = 1; // (sls.UpperValue - sls.LowerValue); // 40
					else
						axis.StripLines[offset].StripWidth = sls.StripWidth; // (sls.UpperValue - sls.LowerValue); // 40

					// Set the strip line interval.
					// In this case, we only want to add one strip so we define an interval off the chart.
					axis.StripLines[offset].Interval = sls.Interval;

					//axis.StripLines[offset].StripWidthType = DateTimeIntervalType.Days;
					//axis.StripLines[offset].IntervalType = DateTimeIntervalType.Days;

					// Set the strip line interval offset.
					//axis.StripLines[offset].IntervalOffset = sls.LowerValue; // 20;
					//if (sls.IntervalOffset == 0)
					//    axis.StripLines[offset].IntervalOffset = 2; // 20;
					//else
					axis.StripLines[offset].IntervalOffset = sls.IntervalOffset;

					if (!string.IsNullOrEmpty(sls.ToolTip))
						axis.StripLines[offset].ToolTip = sls.ToolTip;

					if (!string.IsNullOrEmpty(sls.HREF))
						axis.StripLines[offset].Url = sls.HREF;
				}
			}
		}

		public void SetPaletteCustomColors(Color[] colors)
		{
			PaletteCustomColors = colors;
		}

		protected ChartArea CreateChartArea(IEnumerable<SeriesData> seriesData)
		{
			ChartArea ca = CreateChartArea(Const_DefaultChartAreaName);
			if (ca != null)
			{
				if (seriesData.Count() > 1)
				{
					if (chartSettings != null
						&& chartSettings.ChartAreaHeight != 0
						&& chartSettings.ChartAreaWidth != 0)
						ca.Position = new ElementPosition(chartSettings.ChartAreaX, chartSettings.ChartAreaY, chartSettings.ChartAreaWidth, chartSettings.ChartAreaHeight);
					else
						ca.Position = new ElementPosition(25, 15, 70, 80);

					if (chartSettings != null
						&& chartSettings.InnerHeight != 0
						&& chartSettings.InnerWidth != 0)
						ca.InnerPlotPosition = new ElementPosition(chartSettings.InnerX, chartSettings.InnerY, chartSettings.InnerWidth, chartSettings.InnerHeight);
					else
						ca.InnerPlotPosition = new ElementPosition(10, 0, 90, 90);
				}

				if (seriesData.Count() > 0)
				{
					SeriesData sd = seriesData.ElementAt(0);
					Color backBegin = chartSettings.BackgroundColorBegin.ToColor(Const_DefaultBackgroundColorBegin);
					Color backEnd = chartSettings.BackgroundColorEnd.ToColor(Const_DefaultBackgroundColorEnd);
					if (sd.GetChartType() == SeriesSettings.Const_BarChart || sd.GetChartType() == SeriesSettings.Const_ColumnChart)
						UpdateChartAreaForBar(ca
							, sd.GetXAxisTitle()
							, sd.GetYAxisTitle()
							, backBegin
							, backEnd);
					else if (sd.GetChartType() == SeriesSettings.Const_SplineChart)
						setChartBackgroundEmbossed();

					if (Property.IsDoubleType(sd.GetYAxisFieldDesc().PropertyType))
						ChartAreas[0].AxisY.LabelStyle.Format = sd.GetFormatString();
				}
			}

			return ca;
		}

		protected ChartArea CreateChartArea(string name)
		{
			if (string.IsNullOrEmpty(name))
				name = Const_DefaultChartAreaName;

			var chartArea = new ChartArea()
			{
				Name = name,
				//BackColor = Color.OldLace,
				BackGradientStyle = GradientStyle.TopBottom,
				BackSecondaryColor = Color.White,
				//BackGradientEndColor = Color.White,
				//BackGradientType = GradientType.TopBottom,
				BorderColor = Color.FromArgb(64, 64, 64),
				ShadowColor = Color.Transparent
			};
/*
			chartArea.Area3DStyle.Clustered = true;
			chartArea.Area3DStyle.Perspective = 10;
			chartArea.Area3DStyle.RightAngleAxes = false;
			chartArea.Area3DStyle.WallWidth = 0;
			chartArea.Area3DStyle.XAngle = 15;
			chartArea.Area3DStyle.YAngle = 10;
	*/		
			//chartArea.Position = new ElementPosition(25, 15, 70, 80);
			//chartArea.InnerPlotPosition = new ElementPosition(10, 0, 90, 90);
			ChartAreas.Add(chartArea);

			return chartArea;
		}
		protected ChartArea UpdateChartAreaForBar(ChartArea chartArea, string xAxisTitle, string yAxisTitle, Color chartBackgroundColorBegin, Color chartBackgrouundColorEnd)
		{
			chartArea.AlignmentOrientation = AreaAlignmentOrientations.Horizontal;

			//chartArea.AxisY.Interval = 100;
			//chartArea.AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
			Axis axis = chartArea.AxisX;
			axis.Title = xAxisTitle;
			axis.TitleFont = GetXAxisTitleFont(axis.TitleFont, Width.Value, Height.Value);
			axis.Interval = 1;
			axis.IntervalAutoMode = IntervalAutoMode.VariableCount;

			axis.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;
			//axis.MajorGrid.Enabled = true;
			//axis.MajorTickMark.Enabled = true;
			axis.MajorGrid.Enabled = false;
			axis.MajorTickMark.Enabled = false;
			axis.LabelStyle.Enabled = true;
			axis.LineColor = Color.Purple;
			LabelStyle label = new LabelStyle();
			//if (string.IsNullOrEmpty(xAxisNumberFormat))
			//    xAxisNumberFormat = "#,###";

			//label.Format = xAxisNumberFormat;
			//axis.LabelStyle = label;

			axis = chartArea.AxisY;
			axis.Title = yAxisTitle;
			axis.TitleFont = GetYAxisTitleFont(axis.TitleFont, Width.Value, Height.Value);
			axis.MajorGrid.Enabled = true;
			axis.MajorTickMark.Enabled = true;
			axis.LabelStyle.Enabled = true;
			axis.LineColor = Color.Green;
			axis.LabelStyle = label;

			chartArea.BackColor = chartBackgroundColorBegin;
			if (chartBackgrouundColorEnd != chartBackgroundColorBegin)
			{
				BackSecondaryColor = chartBackgrouundColorEnd;
				chartArea.BackGradientStyle = GradientStyle.TopBottom;
			}

			SetScaleBreak(chartArea.AxisY);
			//chartArea.AxisY.ScaleBreakStyle.Enabled = true;
			//ChartAreas.Add(chartArea);

			return chartArea;
		}

		protected void setChartBackgroundEmbossed()
		{
			BackColor = Color.FromArgb(243, 223, 193);
			BackGradientStyle = GradientStyle.TopBottom;
			BorderlineColor = Color.FromArgb(181, 64, 1);
			BorderlineDashStyle = ChartDashStyle.Solid;
			BorderlineWidth = 2;

			BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
		}
		protected void setChartBackgroundBlank()
		{
			BackColor = Color.White;
			BackGradientStyle = GradientStyle.None;
			BorderlineColor = Color.White;
			BorderlineDashStyle = ChartDashStyle.NotSet;
			BorderlineWidth = 0;

			BorderSkin.SkinStyle = BorderSkinStyle.None;
		}
		protected void setChartBackgroundTransparent()
		{
			BackColor = Color.Transparent;
			BackGradientStyle = GradientStyle.None;
			BorderlineColor = Color.Transparent;
			BorderlineDashStyle = ChartDashStyle.NotSet;
			BorderlineWidth = 0;

			BorderSkin.SkinStyle = BorderSkinStyle.None;
		}
		/*
				protected void FormatPrimaryChartArea(ChartArea area, Series series, string yAxisTitle, string yAxisFormat)
				{
					//Data Chart
					area.AxisX.LabelStyle.ShowEndLabels = true;

					series.XValueType = ChartValueTypes.String;
					Color axisColor = series.Color;
					//axisColor = Color.Green;
					//series.Color = axisColor;
					area.AxisY.Title = yAxisTitle;
					area.AxisY.TitleColor = axisColor;
					if (string.IsNullOrEmpty(yAxisFormat))
						yAxisFormat = "#,#";
					area.AxisY.LabelStyle.Format = yAxisFormat;

					area.AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;

					area.AxisY.LineColor = axisColor;
					area.AxisY.LabelStyle.FontColor = axisColor;
					area.AxisY.TitleFont = new Font("Trebuchet MS", 10, FontStyle.Bold);
					area.AxisY.LabelStyle.Font = new Font("Trebuchet MS", 8, FontStyle.Bold);

					area.BorderColor = Color.Black;
					area.BorderStyle = ChartDashStyle.Solid;
					area.BorderWidth = 1;
				}
		*/
		static protected float GetProportionalTitleFontSize(float originalFontSize, int width, int height)
		{
			float fontSize = originalFontSize;
			if (fontSize < Const_MinimumTitleFontSize)
				fontSize = Const_MinimumTitleFontSize;

			if (width < 200)
				fontSize = Const_MinimumTitleFontSize;

			if (width > 600)
				fontSize = 12;

			return fontSize;
		}
		static protected float GetTitleFontSize(float originalFontSize, int titleLength, double width, double height)
		{
			float fontSize = GetProportionalTitleFontSize(originalFontSize, (int)width, (int)height);

			if (titleLength > 50)
				fontSize -= 3;
			else if (titleLength > 30)
				fontSize -= 2;
			else if (titleLength > 20)
				fontSize -= 1;

			return fontSize;
		}

		protected static Font GetXAxisTitleFont(Font existingFont, double width, double height)
		{
			float axisFontSize = GetProportionalXAxisFontSize(existingFont.Size + 2, (int)width, (int)height);
			Font font = new Font(existingFont.FontFamily, axisFontSize, FontStyle.Bold);

			return font;
		}
		protected static Font GetYAxisTitleFont(Font existingFont, double width, double height)
		{
			float axisFontSize = GetProportionalYAxisFontSize(existingFont.Size + 2, (int)width, (int)height);
			Font font = new Font(existingFont.FontFamily, axisFontSize, FontStyle.Bold);

			return font;
		}
		static protected float GetProportionalXAxisFontSize(float originalFontSize, int width, int height)
		{
			float fontSize = originalFontSize;
			if (fontSize < Const_MinimumFontSize)
				fontSize = Const_MinimumFontSize;

			if (width < 200)
				fontSize = Const_MinimumFontSize;

			return fontSize;
		}
		static protected float GetProportionalYAxisFontSize(float originalFontSize, int width, int height)
		{
			float fontSize = originalFontSize;
			if (fontSize < Const_MinimumFontSize)
				fontSize = Const_MinimumFontSize;

			if (width < 200)
				fontSize = Const_MinimumFontSize;

			return fontSize;
		}

		static protected Unit GetPixelUnit(int pixels)
		{
			if (pixels < 1)
				pixels = 300;

			return new Unit(pixels.ToString() + "px");
		}

		protected Title SetTitle(string chartTitle)
		{
			if (Titles.Count == 0)
				Titles.Add(chartTitle);

			Title title = Titles[0];

			//float titleFontSize = GetProportionalTitleFontSize(title.Font.Size + 4, chartTitle.Length, width, height);
			float titleFontSize = GetTitleFontSize(title.Font.Size + 4, chartTitle.Length, Width.Value, Height.Value);
			//~~title.Font = new Font(title.Font.FontFamily, titleFontSize, FontStyle.Regular);
			title.Font = new Font(title.Font.FontFamily, titleFontSize, FontStyle.Bold);

			return title;
		}

		protected Series GetChartSeries(string name)
		{
			if (string.IsNullOrEmpty(name))
				return null;

			try
			{
				return Series[name];
			}
			catch (Exception ex)
			{
				Trace.Warning("Cannot find series: " + name, ex);
				throw;
			}
		}
		/*
		private static ChartValueTypes GetChartValueType(System.Type t)
        {
            if (Property.IsIntType(t))
                return ChartValueTypes.Int;
            else if (Property.IsStringType(t))
                return ChartValueTypes.String;
            else if (Property.IsGuidType(t))
                return ChartValueTypes.String;
            else if (Property.IsLongType(t))
                return ChartValueTypes.Long;
            else if (Property.IsDoubleType(t))
                return ChartValueTypes.Double;
            else if (Property.IsDateTimeType(t))
                return ChartValueTypes.DateTime;
			return ChartValueTypes.Auto;
		}
		*/
		private static SeriesChartType GetSeriesChartType(string str)
		{
			if (string.IsNullOrEmpty(str))
				return SeriesChartType.Bar;

			if (str == SeriesSettings.Const_ColumnChart)
				return SeriesChartType.Column;
			else if (str == SeriesSettings.Const_PieChart)
				return SeriesChartType.Pie;
			else if (str == SeriesSettings.Const_SplineChart)
				return SeriesChartType.Spline;
			else if (str == SeriesSettings.Const_StepLineChart)
				return SeriesChartType.StepLine;

			return SeriesChartType.Bar;
		}

		static protected void SetScaleBreak(Axis axis)
		{
			// Enable scale breaks
			axis.ScaleBreakStyle.Enabled = true;

			// Set the scale break type
			axis.ScaleBreakStyle.BreakLineStyle = BreakLineStyle.Wave;

			// Set the spacing gap between the lines of the scale break (as a percentage of y-axis)
			axis.ScaleBreakStyle.Spacing = 2;

			// Set the line width of the scale break
			axis.ScaleBreakStyle.LineWidth = 1;

			// Set the color of the scale break
			axis.ScaleBreakStyle.LineColor = Color.Red;

			// Show scale break if more than 25% of the chart is empty space
			axis.ScaleBreakStyle.CollapsibleSpaceThreshold = 50;

			// If all data points are significantly far from zero, 
			// the Chart will calculate the scale minimum value
			axis.ScaleBreakStyle.StartFromZero = StartFromZero.Yes;// AutoBool.Auto;
		}

		static private void addCustomLabels(Axis axis)
		{
			// Set X axis custom labels
			CustomLabel clElement = axis.CustomLabels.Add(0, 30, "Low");
			clElement = axis.CustomLabels.Add(30, 70, "Medium");
			clElement = axis.CustomLabels.Add(70, 100, "High");

			StripLine stripLow = new StripLine();
			stripLow.IntervalOffset = 0;
			stripLow.StripWidth = 30;
			stripLow.BackColor = Color.FromArgb(128, Color.Green);

			StripLine stripMed = new StripLine();
			stripMed.IntervalOffset = 30;
			stripMed.StripWidth = 40;
			stripMed.BackColor = Color.FromArgb(128, Color.Orange);

			StripLine stripHigh = new StripLine();
			stripHigh.IntervalOffset = 70;
			stripHigh.StripWidth = 30;
			stripHigh.BackColor = Color.FromArgb(128, Color.Red);

			axis.StripLines.Add(stripLow);
			axis.StripLines.Add(stripMed);
			axis.StripLines.Add(stripHigh);

			// Set Y axis custom labels
			/*
			element = Chart1.ChartAreas["Default"].AxisX.CustomLabels.Add(0.5, 1.5, "A");
			Chart1.ChartAreas["Default"].AxisX.CustomLabels[element].GridTick = GridTick.All;

			element = Chart1.ChartAreas["Default"].AxisX.CustomLabels.Add(1.5, 2.5, "B");
			Chart1.ChartAreas["Default"].AxisX.CustomLabels[element].GridTick = GridTick.TickMark;

			element = Chart1.ChartAreas["Default"].AxisX.CustomLabels.Add(2.5, 3.5, "C");
			Chart1.ChartAreas["Default"].AxisX.CustomLabels[element].GridTick = GridTick.All;

			element = Chart1.ChartAreas["Default"].AxisX.CustomLabels.Add(3.5, 4.5, "D");
			Chart1.ChartAreas["Default"].AxisX.CustomLabels[element].GridTick = GridTick.TickMark;

			element = Chart1.ChartAreas["Default"].AxisX.CustomLabels.Add(4.5, 5.5, "E");
			Chart1.ChartAreas["Default"].AxisX.CustomLabels[element].GridTick = GridTick.All;

			// set second row of labels
			Chart1.ChartAreas["Default"].AxisX.CustomLabels.Add(0.5, 3.5, "Group 1", 1, LabelMark.LineSideMark);
			Chart1.ChartAreas["Default"].AxisX.CustomLabels.Add(3.5, 5.5, "Group 2", 1, LabelMark.LineSideMark);

			// One more row of labels
			Chart1.ChartAreas["Default"].AxisX.CustomLabels.Add(0.5, 5.5, "All Groups", 2, LabelMark.LineSideMark);
			 */
		}

		public Annotation CreateAnnotation(DateTime date, string content, Guid partId, string partName)
		{
			PolygonAnnotation annotation = new PolygonAnnotation();

			// Explicitly set the relative height and width
			annotation.Height = 5;
			annotation.Width = 5;

			annotation.BackColor = Color.FromArgb(128, Color.Red);
			annotation.LineColor = Color.Black;
			annotation.LineDashStyle = ChartDashStyle.Solid;

			annotation.ToolTip = content;

			// Define relative value points for a polygon
			PointF[] points = new PointF[3];
			points[0].X = 0;
			points[0].Y = 0;

			points[1].X = -25;
			points[1].Y = -100;

			points[2].X = 25;
			points[2].Y = -100;

			annotation.GraphicsPath.AddPolygon(points);
			annotation.AnchorOffsetY = -8;
			//annotation.Href = string.Format(newsArticleUrl, partName, date.ToShortDateString(), Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(partId + ":" + date.ToShortDateString() + ":" + partName)));

			return annotation;
		}

		static public int populateSeries(Series series, SeriesData ds, bool enableHref, bool enableToolTips)
		{
			int count = 0;
			if (series != null && ds != null)
			{
				int yAxisPropertyNumber = ds.GetYAxisPropertyNumber();
				for (int i = 0; i < ds.RowCount(); ++i)
				{
					PropertyManager dr = ds.GetDataRow(i);
					Property prop = dr.GetProperty(ds.GetXAxisPropertyNumber());
					string product = prop.stringValue;

					product = product.Left(ds.GetMaxDataDisplayLength());
					if (string.IsNullOrEmpty(product))
						product = "N/A";

					int offset = 0;
					if (Property.IsDoubleType(ds.GetYAxisFieldDesc().PropertyType))
					{
						if (!dr.GetProperty(yAxisPropertyNumber).doubleValue.HasValue)
							continue;
						offset = series.Points.AddXY(product, dr.GetProperty(yAxisPropertyNumber).doubleValue);
					}
					else if (Property.IsLongType(ds.GetYAxisFieldDesc().PropertyType))
					{
						if (!dr.GetProperty(yAxisPropertyNumber).longValue.HasValue)
							continue;
						offset = series.Points.AddXY(product, dr.GetProperty(yAxisPropertyNumber).longValue);
					}
					else if (ds.GetYAxisFieldDesc().IsNumber())
					{
						if (!dr.GetProperty(yAxisPropertyNumber).intValue.HasValue)
							continue;
						offset = series.Points.AddXY(product, dr.GetProperty(yAxisPropertyNumber).intValue);
					}
					else if (Property.IsStringType(ds.GetYAxisFieldDesc().PropertyType))
					{
						offset = series.Points.AddXY(product, dr.GetProperty(yAxisPropertyNumber).stringValue);
					}
					else if (Property.IsDateTimeType(ds.GetYAxisFieldDesc().PropertyType))
					{
						if (!dr.GetProperty(yAxisPropertyNumber).dateTimeValue.HasValue)
							continue;
						offset = series.Points.AddXY(product, dr.GetProperty(yAxisPropertyNumber).dateTimeValueOrDefault.ToShortDateString());
					}

					DataPoint p = series.Points[offset];

					DataPointSettingsWithObjects dps = ds.GetDataPointSettings(i);
					if (dps != null)
					{
						if (dps.SetBorderColor)
							p.BorderColor = dps.BorderColorColor;
						if (dps.SetBorderWidth)
							p.BorderWidth = dps.BorderWidth;
						if (dps.SetMarkerColor)
							p.MarkerColor = dps.MarkerColorColor;
						if (dps.SetMarkerBorderColor)
							p.MarkerBorderColor = dps.MarkerBorderColorColor;
						if (dps.SetLabelBorderWidth)
							p.LabelBorderWidth = dps.LabelBorderWidth;
						if (dps.SetLabelBackColor)
							p.LabelBackColor = dps.LabelBackColorColor;
						//if (dps.SetBackGradientEndColor)
						//    p.BackGradientEndColor = dps.BackGradientEndColorColor;
						//if (dps.SetBackGradientType)
						//    p.BackGradientType = dps.BackGradientTypeGradientType;
						if (dps.SetBorderColor)
							p.BorderColor = dps.BorderColorColor;
						if (dps.SetMainColor)
							p.Color = dps.MainColorColor;
						if (dps.SetExploded)
							p.SetCustomProperty("Exploded", "true");

						//p.MarkerColor = Color.AliceBlue;
						//p.MarkerBorderColor = Color.AliceBlue;
						//p.LabelBorderWidth = 10;
						//p.LabelBackColor = Color.AliceBlue;
						//p.BackGradientEndColor = Color.Green;
						//p.BackGradientType = GradientType.DiagonalRight;
						//p.BorderColor = Color.Red;
						//p.Color = Color.SaddleBrown;

						//series.SetAttribute("PieLabelStyle", "Ellipse");
						//p.ShowLabelAsValue = true;
						//p.SetAttribute("PieLabelOffset", "100:150");
					}

					if(enableToolTips)
						p.ToolTip = ds.GetToolTip(i);
					if (enableHref)
					{
						if (ds.Settings.HREFIsJavascript)
						{
							p.Url = "javascript:void(0);";
							p.MapAreaAttributes = "onclick=\"" + ds.GetHREF(i) + "\"";
						}
						else
						{
							p.Url = ds.GetHREF(i);
						}
					}

					++count;
					if (ds.GetMaxPointsToShow() > 1 && count >= ds.GetMaxPointsToShow())
						break;
				}
			}

			return count;
		}

		public static void ChartHorizontalXAxis_PrePaint(object sender, ChartPaintEventArgs e)
		{
			Chart chart = e.Chart;
			ChartArea chartArea = e.Chart.ChartAreas[0];
			Axis xaxis = chartArea.AxisX;
			Axis yaxis = chartArea.AxisY;

			// Enumerate through all axis labels
			Series series = chart.Series[0];
			for (int index = 0; index < series.Points.Count; index++)
			{
				DataPoint dp = series.Points[index];

				// Retrieve size and position of individual labels
				RectangleF rect = new RectangleF(
					(float)xaxis.GetPosition(index + 0.5),
					(float)chartArea.InnerPlotPosition.Bottom,
					(float)xaxis.GetPosition(index + 1.5) - (float)xaxis.GetPosition(index + 0.5),
					(float)(chartArea.Position.Bottom)
					);

				float[] coords = {
					(float)xaxis.GetPosition(index + 0.5),
					(float)chartArea.InnerPlotPosition.Bottom,
					(float)xaxis.GetPosition(index + 1.5) - (float)xaxis.GetPosition(index + 0.5),
					(float)(chartArea.Position.Bottom) };

				// Use label's size and position information to define "hot" areas
				MapArea ma = new MapArea() {
					ToolTip = dp.AxisLabel,
					Url = dp.Url,
					Coordinates = coords,
				};
				chart.MapAreas.Add(ma);
				/*
					(
					dp.AxisLabel,// Tool tip text
					dp.Url,
					"",		// link attribute (i.e.: Target="_Blank"
					rect	// the "hot" region for the link
					);
				 */
			}
		}
		public static void ChartVerticalXAxis_PrePaint(object sender, ChartPaintEventArgs e)
		{
			if (sender is ChartArea)
			{
				// Enumerate through all axis labels
				Chart chart = e.Chart;
				ChartArea chartArea = chart.ChartAreas[0];
				Axis xaxis = chartArea.AxisX;
				Axis yaxis = chartArea.AxisY;
				Series series = chart.Series[0];
				for (int index = 0; index < series.Points.Count; index++)
				{
					DataPoint dp = series.Points[index];
					string axisLabel = dp.AxisLabel;

					// Size of Label in pixels
					SizeF labelSize = e.ChartGraphics.Graphics.MeasureString(axisLabel,
					   xaxis.LabelStyle.Font);

					// Size of Label in pixels to Relative Size
					labelSize = e.ChartGraphics.GetRelativeSize(labelSize);

					// Retrieve size and position of individual labels
					RectangleF rect = new RectangleF(
						(float)yaxis.GetPosition(yaxis.Minimum) - labelSize.Width,
						(float)xaxis.GetPosition(index + 1) - labelSize.Height / 2,
						(float)yaxis.GetPosition(yaxis.Minimum),
						labelSize.Height
						);

					float[] coords = {
						(float)yaxis.GetPosition(yaxis.Minimum) - labelSize.Width,
						(float)xaxis.GetPosition(index + 1) - labelSize.Height / 2,
						(float)yaxis.GetPosition(yaxis.Minimum),
						labelSize.Height
					};

					// Use label's size and position information to define "hot" areas
					MapArea ma = new MapArea()
					{
						ToolTip = dp.AxisLabel,
						Url = dp.Url,
						Coordinates = coords,
					};
				}
			}
		}
		/// <summary>
		/// Creates the water mark and applies alpha blending
		/// </summary>
		/// <param name="img">A refrence to the image file to be placed in the chart (System.Drawing.Image not the ASP.NET control)</param>
		/// <param name="alpha">A value between 0-255 that determines how faded the image will appear on the chart (255 is not faded at all)</param>
		/// <param name="color">The Color that will be made completely transparent, if you don't want to do this just pass System.Drawing.Color.Transparent</param>
		/// <param name="width">The width of the image to be drawn on the chart</param>
		/// <param name="height">The height of the image to be drawn on the chart</param>
		/// <param name="e">Dundas.Charting.WebControl.ChartPaintEventArgs e, this needs to be passed to the function so the drawing can be done  <see cref="T:Dundas.Charting.WebControl.ChartPaintEventArgs"/> instance containing the event data.</param>
		public void CreateWaterMark(Chart Chart1, System.Drawing.Image img, Single alpha, System.Drawing.Color color, int width, int height, ChartPaintEventArgs e)
		{
			//The object is used to apply changes to the image
			System.Drawing.Imaging.ImageAttributes attrib = new System.Drawing.Imaging.ImageAttributes();

			//Apply a transparency color
			if (color != null)
			{
				System.Drawing.Imaging.ColorMap[] map = new System.Drawing.Imaging.ColorMap[1];
				map[0] = new System.Drawing.Imaging.ColorMap();
				map[0].OldColor = color;
				map[0].NewColor = System.Drawing.Color.Transparent;
				attrib.SetRemapTable(map);
			}

			//Apply the alpha blend
			System.Drawing.Imaging.ColorMatrix matrix = new System.Drawing.Imaging.ColorMatrix();
			matrix[3, 3] = 256 - alpha;
			attrib.SetColorMatrix(matrix);

			//Get the coordinates for the chart area
			//Calculate the rectangle that is the chartarea
			double xmint = Chart1.ChartAreas[0].AxisX.Minimum;
			double xmaxt = Chart1.ChartAreas[0].AxisX.Maximum;
			double ymint = Chart1.ChartAreas[0].AxisY.Minimum;
			double ymaxt = Chart1.ChartAreas[0].AxisY.Maximum;

			float xmin = (float)e.ChartGraphics.GetPositionFromAxis(Const_DefaultChartAreaName, AxisName.X, xmint);
			float xmax = (float)e.ChartGraphics.GetPositionFromAxis(Const_DefaultChartAreaName, AxisName.X, xmaxt);
			float ymin = (float)e.ChartGraphics.GetPositionFromAxis(Const_DefaultChartAreaName, AxisName.Y, ymint);
			float ymax = (float)e.ChartGraphics.GetPositionFromAxis(Const_DefaultChartAreaName, AxisName.Y, ymaxt);

			RectangleF rect = new RectangleF(xmin, ymax, xmax - xmin, ymin - ymax);

			//Convert the rectangle to absolute coordinates
			rect = e.ChartGraphics.GetAbsoluteRectangle(rect);

			//This rectangle is the rectangle representing the chartarea
			Rectangle areaRectangle = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);

			//Find the center point of the chartarea
			double midX = areaRectangle.X + (0.5 * areaRectangle.Width);
			double midY = areaRectangle.Y + (0.5 * areaRectangle.Height);

			//Calculate the actual rectangle used to draw the image
			Rectangle innerRectangle = new Rectangle((int)(midX - (0.5 * width)), (int)(midY - (0.5 * height)), width, height);

			//Draw the Image
			e.ChartGraphics.Graphics.DrawImage(img, innerRectangle, 0, 0, img.Width, img.Height, System.Drawing.GraphicsUnit.Pixel, attrib);
		}

		public static void AddHelpImage(object sender, ChartPaintEventArgs e)
		{
			MSChartBuilder chart = e.Chart as MSChartBuilder;
			if (chart == null)
				return;

			string helpURL = chart.chartSettings.HelpIconRelativeURL;
			if (string.IsNullOrEmpty(helpURL))
				return;

			string helpText = chart.chartSettings.HelpIconText ?? string.Empty;
			string helpHREF = chart.chartSettings.HelpIconHREF ?? string.Empty;

			ChartArea chartArea = chart.ChartAreas[0];
			Axis xaxis = chartArea.AxisX;
			Axis yaxis = chartArea.AxisY;

			double xmint = chartArea.AxisX.Minimum;
			double xmaxt = chartArea.AxisX.Maximum;
			double ymint = chartArea.AxisY.Minimum;
			double ymaxt = chartArea.AxisY.Maximum;

			float xmin = (float)e.ChartGraphics.GetPositionFromAxis(Const_DefaultChartAreaName, AxisName.X, xmint);
			float xmax = (float)e.ChartGraphics.GetPositionFromAxis(Const_DefaultChartAreaName, AxisName.X, xmaxt);
			float ymin = (float)e.ChartGraphics.GetPositionFromAxis(Const_DefaultChartAreaName, AxisName.Y, ymint);
			float ymax = (float)e.ChartGraphics.GetPositionFromAxis(Const_DefaultChartAreaName, AxisName.Y, ymaxt);

			RectangleF rect = new RectangleF(xmin, ymax, xmax - xmin, ymin - ymax);

			//Convert the rectangle to absolute coordinates
			rect = e.ChartGraphics.GetAbsoluteRectangle(rect);

			//This rectangle is the rectangle representing the chartarea
			Rectangle areaRectangle = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);

			//Find the center point of the chartarea
			//double midX = areaRectangle.X + (0.5 * areaRectangle.Width);
			//double midY = areaRectangle.Y + (0.5 * areaRectangle.Height);

			//RectangleF rect = new RectangleF(
			//    (float)xaxis.GetPosition(10.0)
			//    , (float)chartArea.InnerPlotPosition.Right()
			//    , (float)xaxis.GetPosition(20.0)
			//    , (float)(chartArea.Position.Right()));

			System.Drawing.Image img = System.Drawing.Image.FromFile(chart.chartSettings.HelpIconRelativeURL);
			if (img != null)
			{
				//Rectangle innerRectangle = new Rectangle((int)(areaRectangle.Right - img.Width), (int)(areaRectangle.Top), img.Width, img.Height);
				Rectangle innerRectangle = new Rectangle((int)480, (int)0, img.Width, img.Height);
				//The object is used to apply changes to the image
				System.Drawing.Imaging.ImageAttributes attrib = new System.Drawing.Imaging.ImageAttributes();

				Color color = Color.Transparent;
				//Apply a transparency color
				if (color != null)
				{
					System.Drawing.Imaging.ColorMap[] map = new System.Drawing.Imaging.ColorMap[1];
					map[0] = new System.Drawing.Imaging.ColorMap();
					map[0].OldColor = color;
					map[0].NewColor = System.Drawing.Color.Transparent;
					attrib.SetRemapTable(map);
				}

				//Apply the alpha blend
				//Single alpha = 128;
				//System.Drawing.Imaging.ColorMatrix matrix = new System.Drawing.Imaging.ColorMatrix();
				//matrix[3, 3] = 256 - alpha;
				//attrib.SetColorMatrix(matrix);
				e.ChartGraphics.Graphics.DrawImage(img, innerRectangle, 0, 0, img.Width, img.Height, System.Drawing.GraphicsUnit.Pixel, attrib);
				// Use label's size and position information to define "hot" areas

				if (!string.IsNullOrEmpty(helpText) || !string.IsNullOrEmpty(helpHREF))
				{
					// The MapAreas are from 0,0 (upper left) to 100,100 in terms of percentages of the chart.
					Rectangle rectMapArea = new Rectangle(innerRectangle.X, innerRectangle.Y, innerRectangle.Width, innerRectangle.Height);
					rectMapArea.X = 95;	// This puts it on the right.
					float[] coords = {
										 //innerRectangle.X,
										 95,
										 innerRectangle.Y,
										 innerRectangle.Width,
										 innerRectangle.Height
									 };
					MapArea ma = new MapArea()
					{
						ToolTip = helpText,
						Url = helpHREF,
						Coordinates = coords,
					};
					chart.MapAreas.Add(ma);
					/*
					chart.MapAreas.Add(
						helpText,	// Tool tip text
						helpHREF,	// HREF
						"",			// link attribute (i.e.: Target="_Blank"
						rectMapArea	// the "hot" region for the link
						);
					 */
				}
			}
		}

		public void AddHelpImagePaint()
		{
			PrePaint += AddHelpImage;
		}
	}
}
