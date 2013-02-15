using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace skkyWeb.util
{
	public static class UnitedStates
	{
		private static readonly string[] stateLongNames =
		{
			"Alaska",
			"Alabama",
			"Arkansas",
			"Arizona",
			"California",
			"Colorado",
			"Connecticut",
			"District of Columbia",
			"Delaware",
			"Florida",
			"Georgia",
			"Hawaii",
			"Iowa",
			"Idaho",
			"Illinois",
			"Indiana",
			"Kansas",
			"Kentucky",
			"Louisiana",
			"Massachusetts",
			"Maryland",
			"Maine",
			"Michigan",
			"Minnesota",
			"Missouri",
			"Mississippi",
			"Montana",
			"North Carolina",
			"North Dakota",
			"Nebraska",
			"New Hampshire",
			"New Jersey",
			"New Mexico",
			"Nevada",
			"New York",
			"Ohio",
			"Oklahoma",
			"Oregon",
			"Pennsylvania",
			"Rhode Island",
			"South Carolina",
			"South Dakota",
			"Tennessee",
			"Texas",
			"Utah",
			"Virginia",
			"Vermont",
			"Washington",
			"Wisconsin",
			"West Virginia",
			"Wyoming"
		};

		private static readonly string[] stateAbbreviations =
		{
			"AK",
			"AL",
			"AR",
			"AZ",
			"CA",
			"CO",
			"CT",
			"DC",
			"DE",
			"FL",
			"GA",
			"HI",
			"IA",
			"ID",
			"IL",
			"IN",
			"KS",
			"KY",
			"LA",
			"MA",
			"MD",
			"ME",
			"MI",
			"MN",
			"MO",
			"MS",
			"MT",
			"NC",
			"ND",
			"NE",
			"NH",
			"NJ",
			"NM",
			"NV",
			"NY",
			"OH",
			"OK",
			"OR",
			"PA",
			"RI",
			"SC",
			"SD",
			"TN",
			"TX",
			"UT",
			"VA",
			"VT",
			"WA",
			"WI",
			"WV",
			"WY"
		};

		public static string[] Names
		{
			get { return stateLongNames; }
		}

		public static string[] Abbreviations
		{
			get { return stateAbbreviations; }
		}

		/// <summary>
		/// Returns the state's name, when passed its abbreviation
		/// </summary>
		/// <param name="strAbbr"></param>
		/// <returns></returns>
		public static string GetStateName(string strAbbr)
		{
			for (int i = 0; i < stateAbbreviations.Length; i++)
			{
				if (stateAbbreviations[i].ToLower() == strAbbr.ToLower())
					return stateLongNames[i];
			}

			return string.Empty;
		}

		/// <summary>
		/// Returns the state's abbreviation, when passed its name
		/// </summary>
		/// <param name="strName"></param>
		/// <returns></returns>
		public static string GetStateAbbr(string strName)
		{
			for (int i = 0; i < stateLongNames.Length; i++)
			{
				if (stateLongNames[i].ToLower() == strName.ToLower())
					return stateAbbreviations[i];
			}

			return string.Empty;
		}

		public static DropDownList GetDropDown(DropDownList objDdl)
		{
			return GetDropDown(objDdl, true, String.Empty, false);
		}

		public static DropDownList GetDropDown(DropDownList objDdl, bool bShortNames)
		{
			return GetDropDown(objDdl, true, String.Empty, bShortNames);
		}

		public static DropDownList GetDropDown(DropDownList objDdl, string strValue)
		{
			return GetDropDown(objDdl, true, strValue, false);
		}

		public static DropDownList GetDropDown(DropDownList objDdl, bool bAddEmptyItem, string strValue)
		{
			return GetDropDown(objDdl, true, strValue, false);
		}

		public static DropDownList GetDropDown(DropDownList objDdl, bool bAddEmptyItem, string strValue, bool bShortNames)
		{
			ListItem objItem;

			//** first create a blank listitem
			if (bAddEmptyItem)
			{
				objItem = new ListItem();
				objItem.Value = "";
				objItem.Text = "--";
				objDdl.Items.Add(objItem);
			}

			//** populdate the dropdown with the states
			for (int i = 0; i < stateLongNames.Length; i++)
			{
				objItem = new ListItem();

				if (bShortNames)
				{
					objItem.Text = stateAbbreviations[i];
					objItem.Value = stateAbbreviations[i];
				}
				else
				{
					objItem.Text = stateLongNames[i];
					objItem.Value = stateAbbreviations[i];
				}

				if (objItem.Value.ToLower() == strValue.ToLower())
					objItem.Selected = true;

				//** add the item to the list
				objDdl.Items.Add(objItem);
			}

			return objDdl;
		}
	}
}
