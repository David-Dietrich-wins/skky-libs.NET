using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Drawing;

namespace skkyWeb.Charts
{
	[DataContract]
	public class StripLineSettings
	{
		public StripLineSettings()
		{ }

		public StripLineSettings(double lower, double upper, Color color)
		{
			LowerValue = lower;
			UpperValue = upper;
			MainColor = color.ToArgb().ToString();
		}
		public StripLineSettings(StripLineSettings ds)
		{
			StripLineSettings dps = ds;
			if (dps == null)
				dps = new StripLineSettings();

			MainColor = dps.MainColor;
			BorderColor = dps.BorderColor;
			BorderWidth = dps.BorderWidth;
			LabelBackColor = dps.LabelBackColor;
			LowerValue = dps.LowerValue;
			UpperValue = dps.UpperValue;
		}

		[DataMember]
		public string MainColor { get; set; }

		[DataMember]
		public string BorderColor { get; set; }
		[DataMember]
		public bool SetBorderColor { get; set; }

		[DataMember]
		public int BorderWidth { get; set; }
		[DataMember]
		public bool SetBorderWidth { get; set; }

		[DataMember]
		public string LabelBackColor { get; set; }
		[DataMember]
		public bool SetLabelBackColor { get; set; }

		[DataMember]
		public double LowerValue { get; set; }
		[DataMember]
		public double UpperValue { get; set; }

		[DataMember]
		public double Interval { get; set; }
		[DataMember]
		public double IntervalOffset { get; set; }

		[DataMember]
		public int StripWidth { get; set; }

		[DataMember]
		public string HREF { get; set; }

		[DataMember]
		public string ToolTip { get; set; }
	}
}
