using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using skky.util;

namespace skkyMVC.Models
{
	public class SiteHelper
	{
		public const string CONST_All = ﻿"All";

		public static SelectListItem GetSelectListItem(string text, string value = null, bool isSelected = false)
		{
			SelectListItem sli = new SelectListItem
			{
				Text = text,
			};

			if (null == value)
				sli.Value = text;
			else
				sli.Value = value;

			return sli;
		}

		public static List<SelectListItem> GetSelectListItems(IEnumerable<string> items, string selectedItem, string defaultItem)
		{
			if (string.IsNullOrWhiteSpace(selectedItem))
				selectedItem = defaultItem ?? string.Empty;

			List<SelectListItem> slis = new List<SelectListItem>();
			foreach (var item in items)
			{
				//string itemEnglish = Resources.res Languages.Language..skkyDate_SelectDateRange;
				SelectListItem sli = GetSelectListItem(item, item, selectedItem == item);

				slis.Add(sli);
			}

			return slis;
		}

		public static List<SelectListItem> GetMonthsSelectListItems(bool showAll = true, string selectedItem = "")
		{
			SelectListItem sli = null;
			var sliMonths = new List<SelectListItem>();
			if (showAll)
			{
				sli = GetSelectListItem(CONST_All, string.Empty);
				sliMonths.Add(sli);
			}

			for (int i = 0; i < DateTimeHelper.CONST_MonthNames.Length; ++i)
			{
				sli = GetSelectListItem(DateTimeHelper.CONST_MonthNames[i], (i + 1).ToString());
				if (DateTimeHelper.CONST_MonthNames[i] == selectedItem)
					sli.Selected = true;

				sliMonths.Add(sli);
			}

			return sliMonths;
		}

		public static List<SelectListItem> GetYearSelectListItems(int selectedItem = 0)
		{
			int beginYear = 2010;
			int endYear = DateTime.Now.Year;

			if (selectedItem < beginYear || selectedItem > endYear)
				selectedItem = endYear;

			List<SelectListItem> slis = new List<SelectListItem>();
			for (int i = beginYear; i <= endYear; ++i)
			{
				//string itemEnglish = Resources.res Languages.Language..skkyDate_SelectDateRange;
				SelectListItem sli = GetSelectListItem(i.ToString(), i.ToString(), selectedItem == i);

				slis.Add(sli);
			}

			return slis;
		}

		#region JqGrid support methods
		public static string GetJqgridSafeString(string jqgridString)
		{
			string str = (jqgridString ?? string.Empty).Replace("'", "\\'");

			return str;
		}
		public static string GetJqgridSelectItemString(int selectId, string selectValue, bool prependSemicolon = true)
		{
			string str = (prependSemicolon ? ";" : string.Empty);

			str += selectId + ":" + GetJqgridSafeString(selectValue);

			return str;
		}
		#endregion
	}
}