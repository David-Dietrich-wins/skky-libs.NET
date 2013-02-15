using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.Types;
using System.Drawing;
using skky.util;

namespace skkyWeb.Charts
{
	[DataContract]
	public class ChartSettings : SkkyCallParams
	{
		public ChartSettings()
		{
			MaxRowsToDisplay = -1;
			EnableHref = true;
			EnableToolTips = true;
			EnableHelp = true;
			ShowChartInformation = true;
			ShowLegend = false;
			//ShowHelpIcon = true;
			//HelpIconText = "Help on this Chart";
			//HelpIconHREF = "#\" onMouseover=\"javascript:showmenu(event,linkset[2]);\" onMouseout=\"javascript:delayhidemenu();";
			//HelpIconHREF = "http://www.skky.net/";
			//HelpIconHREF = "<a href=\"#\" onMouseover=\"showmenu(event,linkset[0])\" onMouseout=\"delayhidemenu()\">Chart Help</a><br>";
		}

		[DataMember]
		public bool ShowHelpIcon { get; set; }

		[DataMember]
		public bool EnableHref { get; set; }

		[DataMember]
		public bool EnableToolTips { get; set; }

		[DataMember]
		public bool EnableHelp { get; set; }

		[DataMember]
		public string HelpIconRelativeURL { get; set; }

		[DataMember]
		public string HelpIconHREF { get; set; }

		[DataMember]
		public string HelpIconText { get; set; }

		[DataMember]
		public int Height { get; set; }

		[DataMember]
		public int Width { get; set; }

		[DataMember]
		public bool IncludeDistance { get; set; }
		[DataMember]
		public bool IncludeRoomNights { get; set; }
		[DataMember]
		public bool IncludeSegmentCounts { get; set; }

		[DataMember]
		public int MaxRowsToDisplay { get; set; }

		[DataMember]
		public string BackgroundColorBegin { get; set; }

		[DataMember]
		public string BackgroundColorEnd { get; set; }

		[DataMember]
		public bool ShowChartInformation { get; set; }

		[DataMember]
		public bool ShowLegend { get; set; }

		[DataMember]
		public int InnerX { get; set; }
		[DataMember]
		public int InnerY { get; set; }
		[DataMember]
		public int InnerHeight { get; set; }
		[DataMember]
		public int InnerWidth { get; set; }

		[DataMember]
		public int ChartAreaX { get; set; }
		[DataMember]
		public int ChartAreaY { get; set; }
		[DataMember]
		public int ChartAreaHeight { get; set; }
		[DataMember]
		public int ChartAreaWidth { get; set; }

		public static List<ChartSettings> GetDefaultCharts()
		{
			List<ChartSettings> lcs = new List<ChartSettings>();

			int order = 1;
			ChartSettings cs = new ChartSettings
			{
				Name = "Default Chart",
				Order = order++,
				//Type = Const_BarChart,
				Width = 600,
			};
			lcs.Add(cs);

			return lcs;
		}

		public void EnableStandardHelp()
		{
			if (EnableHelp)
			{
				ShowHelpIcon = true;
				//cs.ChartSettings.HelpIconText = "Help on this Chart";
				HelpIconText = string.Empty;	// Interferes with the menu if there is a tooltip.
				HelpIconHREF = "#\" onmouseover=\"javascript:LocalMenu.showmenuWithSkkyHeader(event,{0},250);\" onmouseout=\"javascript:LocalMenu.delayhidemenu();";
			}
		}
		public void SetBackgroundColors(Color backgroundBegin, Color backgroundEnd)
		{
			BackgroundColorBegin = backgroundBegin.ToArgb().ToString();
			BackgroundColorEnd = backgroundEnd.ToArgb().ToString();
		}
	}
}
