using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using skky.Conversions;
using skky.util;
using skky.web;
using skkyWeb.util;

namespace skkyWeb.Charts
{
	public static class ChartToHtml
	{
		private static int uniqueVariableName;

		private static string GetNextVariableName()
		{
			++uniqueVariableName;
			return "popmenu" + uniqueVariableName.ToString();
		}
		private static string GetPopupMenuVariableName(string varName, bool isJSON)
		{
			if (!isJSON || string.IsNullOrEmpty(varName))
				return varName;

			// Must be in sync with dhtmlwindow.js. There must be a json variable that must
			//  get assigned the returned response text from this AJAX call.
			return "skkyjsonGetPopupMenuHTML(dhtmlwindow.json.popupMenus, " + varName.WrapInSingleQuotes() + ")";
		}
		public static skkyChartBase GetChartBuilder(ChartManager cm, HttpServerUtility httpServer
			, skkyjson sj, object eventSource, System.Web.UI.Page page)
		{
			if (cm == null)
				return null;

			try
			{
				var cb = cm.getChart();
				if (cb != null)
				{
					if (sj != null)
						sj.nameValues.Add("chartName", cb.getID());

					HtmlTable htChartInfo = null;
					if (cm.ChartSettings.EnableHelp && cm.ChartSettings.ShowHelpIcon)
					{
						string helpIconPhysicalLocation = cm.ChartSettings.HelpIconRelativeURL;
						if (string.IsNullOrEmpty(helpIconPhysicalLocation))
							helpIconPhysicalLocation = ChartManager.Const_DefaultHelpIconLocation;

						//helpIconPhysicalLocation = Server.MapPath(helpIconPhysicalLocation);
						if (!string.IsNullOrEmpty(helpIconPhysicalLocation))
						{
							helpIconPhysicalLocation = httpServer.MapPath(helpIconPhysicalLocation);

							string href = cm.ChartSettings.HelpIconHREF;
							if (!string.IsNullOrEmpty(href))
							{
								string varName = GetNextVariableName();

								if (href.Contains("{") && href.Contains("}"))
								{
									href = string.Format(cm.ChartSettings.HelpIconHREF, GetPopupMenuVariableName(varName, (sj != null)));
									cm.ChartSettings.HelpIconHREF = href;
								}

								htChartInfo = GetChartInformationTable(cm);
								href = Html.GetRawHTML(htChartInfo);

								href = href.Replace("\n", "");
								href = href.Replace("\r", "");
								if (sj != null)
								{
									//sj.popupMenus.Add(varName, new PopupMenu() { variableName = GetPopupMenuVariableName(varName, true), innerHTML = href });
									sj.popupMenus.Add(varName, new PopupMenu() { variableName = GetPopupMenuVariableName(varName, true), innerHTML = href });
								}
								else
								{
									href = "var " + varName + " = " + href.WrapInSingleQuotes() + ";";
									if(page != null)
										page.ClientScript.RegisterStartupScript(typeof(System.Web.UI.Page), varName, href, true);
								}
							}

							cm.ChartSettings.HelpIconRelativeURL = helpIconPhysicalLocation;
							var chart = cm.getChart();
							chart.AddHelpImagePaint();
						}
					}
				}

				return cb;
			}
			catch (Exception ex)
			{
				ChartErrorEvent e = new ChartErrorEvent("GetUser().DisplayName", "GetCustomerName()", cm.Name, "ChartFactory.GetChart", eventSource, ex);
				e.Raise();
			}

			return null;
		}

		public static Control GetChart(ChartManager cm, HttpServerUtility httpServer, skkyjson sj, object eventSource, System.Web.UI.Page page)
		{
			var cb = GetChartBuilder(cm, httpServer, sj, eventSource, page);
			if(cb == null)
				return null;

			// Return the chart if we are not going to build any chart information.
			// Unless we were not able to build the chart, then we need to wrap the error message in a table.
			if (!cm.ChartSettings.ShowChartInformation)
			{
				//string url = cb.ImageUrl.TrimStart('~');
				//url = url.TrimStart('/');
				////url = "http://wiretoss.com/" + url;
				//int index = url.IndexOf('?');
				//if (index > 0)
				//    url = url.Substring(0, index - 1);
				//cb.ImageUrl = url;
				return cb as Control;
			}

			var tbl = new HtmlTable();
			HtmlTableRow tr = new HtmlTableRow();
			tr.VAlign = "top";
			tbl.Rows.Add(tr);

			if (cb == null)
			{
				tr.AddChild(Html.GetLiteralControl("No Data for the Chart."));
			}
			else
			{
				tr.AddChild(cb as Control);
				if (cm.ChartSettings.ShowChartInformation)
				{
					tr.AddChild(GetChartInformationTable(cm));
				}
			}

			return tbl;
		}

		public static HtmlTable GetChartInformationTable(ChartManager cm)
		{
			string str = "Chart Information";

			HtmlTable tbl = new HtmlTable();
			tbl.SetClass("chartInfo");
			//tbl.Style["background-color"] = "#999;";

			HtmlTableRow tr = new HtmlTableRow();
			HtmlTableCell td = tr.AddLiteralControl(str);
			td.ColSpan = 2;
			td.SetClass("header");
			tbl.Rows.Add(tr);

			if (cm != null)
			{
				//double totalKgLb = 0.0;
				//int totalKmMi = 0;

				int seriesListCount = (cm.SeriesDataList == null ? 0 : cm.SeriesDataList.Count());
				//if (seriesListCount > 0)
				//{
				//    foreach (var item in cm.SeriesDataList)
				//    {
				//        if (item.Settings.Conversion == ConversionBase.ConversionIdentifiers.KilogramsToPounds)
				//            totalKgLb = item.Total.doubleValue;
				//        else if (item.Settings.Conversion == ConversionBase.ConversionIdentifiers.KilometersToMiles)
				//            totalKmMi = item.Total.intValue;
				//    }
				//}

				//AddTableRow(tbl, "Name", cm.Name);
				Html.AddTableRow(tbl, "Title", cm.Title);
				Html.AddTableRowIfValue(tbl, "Type", cm.Type);
				//Html.AddTableRow(tbl, "Account", string.IsNullOrEmpty(cm.AccountNumber) ? "All Accounts" : cm.AccountName ?? cm.AccountNumber);
				//Html.AddTableRow(tbl, "Department", cm.DepartmentId == 0 ? "All Departments" : cm.DepartmentName ?? cm.DepartmentId.ToString());

				str = "All";
				if (cm.DateRange != null
					&& cm.DateRange.ShouldShowAllDates() == false)
				{
					str = cm.DateRange.GetStartDate().ToLongDateString();
					str += " - ";
					str += cm.DateRange.GetEndDate().ToLongDateString();
				}
				Html.AddTableRow(tbl, "Dates", str);

				//str = string.Empty;
				//if(totalKgLb > 0.0)
				//    str += totalKgLb.ToString("#,#") + " " + cm.GetKgPounds() + " of ";
				//str += cm.GetEmissionTypeHTMLName();

				//if (totalKmMi > 0)
				//    Html.AddTableRow(tbl, "Distance", totalKmMi.ToString(NumberDisplay.NoDecimalsWithZeroWithComma) + " " + cm.GetKilometersMilesShort());
				//if (totalKgLb > 0 && totalKmMi > 0)
				//{
				//    str = (totalKgLb / totalKmMi).ToString(NumberDisplay.TwoDecimalsOptionalNoZeroWithComma);
				//    str += " ";
				//    str += cm.GetKgPoundsShort() + "/" + cm.GetKilometersMilesShort();
				//    Html.AddTableRow(tbl, "Average", str);
				//}

				if (seriesListCount > 0)
				{
					foreach (var item in cm.SeriesDataList)
					{
						tr = new HtmlTableRow();

						td = tr.AddLiteralControl(string.Empty);
						td.BgColor = item.MainColor.ToHtmlString();

						tr.AddLiteralControl(item.GetYAxisTitle());

						tbl.Rows.Add(tr);
					}

					Control ctlDept = cm.BuildDepartmentalTable();
					if (ctlDept != null)
					{
						tr = new HtmlTableRow();
						td = tr.AddLiteralControl(string.Empty);
						td.ColSpan = 2;
						tbl.Rows.Add(tr);

						tr = new HtmlTableRow();
						td = tr.AddChild(ctlDept);
						td.ColSpan = 2;

						tbl.Rows.Add(tr);
					}
				}
			}

			return tbl;
		}
	}
}
