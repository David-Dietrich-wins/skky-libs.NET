using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using skky.util;
using System.Web.UI.DataVisualization.Charting;

namespace skkyWeb.Charts
{
	public class DataPointSettingsWithObjects : DataPointSettings
	{
		public DataPointSettingsWithObjects()
		{
			SetFromDataPointSettings();
		}
		public DataPointSettingsWithObjects(DataPointSettings ds)
			: base(ds)
		{
			SetFromDataPointSettings();
		}

		public Color MainColorColor { get; set; }
		public Color BorderColorColor { get; set; }
		public Color MarkerColorColor { get; set; }
		public Color MarkerBorderColorColor { get; set; }
		public Color LabelBackColorColor { get; set; }
		public Color BackGradientEndColorColor { get; set; }
		public GradientStyle BackGradientTypeGradientType { get; set; }

		protected void SetFromDataPointSettings()
		{
			MainColorColor = MainColor.ToColor();
			BorderColorColor = BorderColor.ToColor();
			MarkerColorColor = MarkerColor.ToColor();
			MarkerBorderColorColor = MarkerBorderColor.ToColor();
			LabelBackColorColor = LabelBackColor.ToColor();
			BackGradientEndColorColor = BackGradientEndColor.ToColor();

			string gradientType = BackGradientType ?? string.Empty;
			gradientType = gradientType.ToLower();
			GradientStyle gt = GradientStyle.None;
			if (gradientType == "center")
				gt = GradientStyle.Center;
			else if (gradientType == "diagonalleft")
				gt = GradientStyle.DiagonalLeft;
			else if (gradientType == "diagonalright")
				gt = GradientStyle.DiagonalRight;
			else if (gradientType == "horizontalcenter")
				gt = GradientStyle.HorizontalCenter;
			else if (gradientType == "leftright")
				gt = GradientStyle.LeftRight;
			else if (gradientType == "topbottom")
				gt = GradientStyle.TopBottom;
			else if (gradientType == "verticalcenter")
				gt = GradientStyle.VerticalCenter;

			BackGradientTypeGradientType = gt;
		}
	}
}
