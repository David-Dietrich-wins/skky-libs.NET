using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace skkyWeb.Charts
{
	[DataContract]
	public class DataPointSettings
	{
		public DataPointSettings()
		{ }

		public DataPointSettings(DataPointSettings ds)
		{
			DataPointSettings dps = ds;
			if (dps == null)
				dps = new DataPointSettings();

			MainColor = dps.MainColor;
			BorderColor = dps.BorderColor;
			BorderWidth = dps.BorderWidth;
			MarkerColor = dps.MarkerColor;
			MarkerBorderColor = dps.MarkerBorderColor;
			LabelBackColor = dps.LabelBackColor;
			LabelBorderWidth = dps.LabelBorderWidth;
			BackGradientEndColor = dps.BackGradientEndColor;
			BackGradientType = dps.BackGradientType;
		}

		[DataMember]
		public string MainColor { get; set; }
		[DataMember]
		public bool SetMainColor { get; set; }

		[DataMember]
		public string BorderColor { get; set; }
		[DataMember]
		public bool SetBorderColor { get; set; }

		[DataMember]
		public int BorderWidth { get; set; }
		[DataMember]
		public bool SetBorderWidth { get; set; }

		[DataMember]
		public string MarkerColor { get; set; }
		[DataMember]
		public bool SetMarkerColor { get; set; }

		[DataMember]
		public string MarkerBorderColor { get; set; }
		[DataMember]
		public bool SetMarkerBorderColor { get; set; }

		[DataMember]
		public string LabelBackColor { get; set; }
		[DataMember]
		public bool SetLabelBackColor { get; set; }

		[DataMember]
		public int LabelBorderWidth { get; set; }
		[DataMember]
		public bool SetLabelBorderWidth { get; set; }

		[DataMember]
		public string BackGradientEndColor { get; set; }
		[DataMember]
		public bool SetBackGradientEndColor { get; set; }

		[DataMember]
		public string BackGradientType { get; set; }
		[DataMember]
		public bool SetBackGradientType { get; set; }

		[DataMember]
		public bool SetExploded { get; set; }
	}
}
