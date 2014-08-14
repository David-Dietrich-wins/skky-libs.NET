using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using skky.jqGrid;
using skky.util;

namespace skky.Types
{
	public class SkkyRepository
	{
		public const string CONST_Active = "Active";
		public const string CONST_Code = "Code";
		public const string CONST_Country = "Country";
		public const string CONST_Created = "Created";
		public const string CONST_CreationDateTime = "CreationDateTime";
		public const string CONST_DateKey = "DateKey";
		public const string CONST_Description = "Description";
		public const string CONST_Email = "Email";
		public const string CONST_Enabled = "Enabled";
		public const string CONST_Exception = "Exception";
		public const string CONST_Filename = "Filename";
		public const string CONST_FirstName = "FirstName";
		public const string CONST_Gender = "Gender";
		public const string CONST_Id = "Id";
		public const string CONST_ImportType = "ImportType";
		public const string CONST_LastName = "LastName";
		public const string CONST_LastUpdatedDateTime = "LastUpdatedDateTime";
		public const string CONST_MiddleName = "MiddleName";
		public const string CONST_Name = "Name";
		public const string CONST_NickName = "NickName";
		public const string CONST_Price = "Price";
		public const string CONST_PropertyName = "PropertyName";
		public const string CONST_Success = "Success";
		public const string CONST_Suffix = "Suffix";
		public const string CONST_Title = "Title";
		public const string CONST_Updated = "Updated";
		public const string CONST_UserName = "UserName";
﻿
		// Methods for returning common strings.
		public static string GetDecimalText(Decimal? d)
		{
			if (!d.HasValue)
				return string.Empty;

			return d.Value.ToString("N0");
		}
		public static string GetDoubleText(double? d)
		{
			if (!d.HasValue)
				return string.Empty;

			return d.Value.ToString("N2");
		}
		public static string GetBooleanText(bool? b)
		{
			if (!b.HasValue)
				return string.Empty;

			return b.Value.ToString();
		}
		public static string GetYesNoText(bool? b)
		{
			if (!b.HasValue)
				return string.Empty;

			return b.Value ? "Yes" : "No";
		}

		/// <summary>
		/// Generally used for logging the most recent updates from a web form using the FillObject() method in skky.util.Extensions.
		/// </summary>
		protected List<string> _MostRecentlySavedFormUpdates = null;
		protected List<string> MostRecentlySavedFormUpdates
		{
			get
			{
				if (null == _MostRecentlySavedFormUpdates)
					_MostRecentlySavedFormUpdates = new List<string>();

				return _MostRecentlySavedFormUpdates;
			}
		}

		/// <summary>
		/// Returns a string describing the add/update method with the fields that were changed
		/// as the result of editing from a web form.
		/// </summary>
		/// <param name="wasAdded">Whether the object was added or updated.</param>
		/// <param name="MaxStringLength">The max string length in case a lot of fields were edited. 0 indicates no max length.</param>
		/// <returns>A comma delimited string describing all of the fields that were edited with the most recent population from a web form.</returns>
		public string GetMostRecentUpdateMessage(bool wasAdded, int MaxStringLength = 100)
		{
			string op = wasAdded ? ActionParams.CONST_ActionAdd : ActionParams.CONST_ActionUpdate;
			op += "-" + string.Join(", ", MostRecentlySavedFormUpdates);

			return MaxStringLength > 0 ? op.Left(MaxStringLength) : op;
		}
	}
}
