using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using skky.jqGrid;
using skky.util;
using System.Security.Cryptography;

namespace skky.Types
{
	public class SkkyRepository
	{
		public const float CONST_MetersInMile = 1609.344f;

		public const string CONST_actionedBy = "actionedBy";
		public const string CONST_actionedOn = "actionedOn";
		public const string CONST_Active = "Active";
		public const string CONST_Address = "Address";
		public const string CONST_Address2 = "Address2";
		public const string CONST_City = "City";
		public const string CONST_Code = "Code";
		public const string CONST_Company = "Company";
		public const string CONST_Country = "Country";
		public const string CONST_County = "County";
		public const string CONST_Created = "Created";
		public const string CONST_createdBy = "createdBy";
		public const string CONST_createdOn = "createdOn";
		public const string CONST_CreationDateTime = "CreationDateTime";
		public const string CONST_Customer = "Customer";
		public const string CONST_DateKey = "DateKey";
		public const string CONST_Description = "Description";
		public const string CONST_Email = "Email";
		public const string CONST_EmailAddress = "EmailAddress";
		public const string CONST_Enabled = "Enabled";
		public const string CONST_Exception = "Exception";
		public const string CONST_Filename = "Filename";
		public const string CONST_First = "First";
		public const string CONST_FirstName = "FirstName";
		public const string CONST_FullName = "FullName";
		public const string CONST_Gender = "Gender";
		public const string CONST_id = "id";
		public const string CONST_Id = "Id";
		public const string CONST_ImportType = "ImportType";
		public const string CONST_Last = "Last";
		public const string CONST_lastName = "lastName";
		public const string CONST_LastName = "LastName";
		public const string CONST_LastUpdatedDateTime = "LastUpdatedDateTime";
		public const string CONST_Latitude = "Latitude";
		public const string CONST_Longitude = "Longitude";
		public const string CONST_MiddleName = "MiddleName";
		public const string CONST_name = "name";
		public const string CONST_Name = "Name";
		public const string CONST_NickName = "NickName";
		public const string CONST_Notes = "Notes";
		public const string CONST_Password = "Password";
		public const string CONST_Period = "Period";
		public const string CONST_Phone = "Phone";
		public const string CONST_PhoneNumber = "PhoneNumber";
		public const string CONST_PostalCode = "Postalcode";
		public const string CONST_Price = "Price";
		public const string CONST_PropertyName = "PropertyName";
		public const string CONST_Role = "Role";
		public const string CONST_RoleArray = CONST_Role + "[]";
		public const string CONST_Roles = CONST_Role + "s";
		public const string CONST_RolesArray = CONST_Roles + "[]";
		public const string CONST_Sex = "Sex";
		public const string CONST_State = "State";
		public const string CONST_StateProvince = "StateProvince";
		public const string CONST_Status = "Status";
		public const string CONST_StreetAddress = "StreetAddress";
		public const string CONST_StreetAddress2 = "StreetAddress2";
		public const string CONST_Success = "Success";
		public const string CONST_Suffix = "Suffix";
		public const string CONST_Title = "Title";
		public const string CONST_Total = "Total";
		public const string CONST_Type = "Type";
		public const string CONST_Updated = "Updated";
		public const string CONST_updatedBy = "updatedBy";
		public const string CONST_updatedOn = "updatedOn";
		public const string CONST_Url = "Url";
		public const string CONST_UserName = "UserName";
		public const string CONST_Value = "Value";
		public const string CONST_Website = "Website";
		public const string CONST_Zip = "Zip";
		public const string CONST_ZipCode = "ZipCode";

		public const int CONST_DefaultPageSize = 20;

		public virtual string DefaultSortField()
		{
			return CONST_createdOn;
		}
		public virtual string DefaultSortOrder()
		{
			return ActionParams.CONST_sordDesc;
		}
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
		public List<string> MostRecentlySavedFormUpdates
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

		/// <summary>
		/// Userd for creating a hash.
		/// </summary>
		/// <param name="value">The string to be encrypted.</param>
		/// <returns>The encrypted string value.</returns>
		public static string CreateMD5(string value)
		{
			var data = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(value));
			return data.Aggregate("", (s, b) => s + b.ToString("x2"));
		}
	}
}
