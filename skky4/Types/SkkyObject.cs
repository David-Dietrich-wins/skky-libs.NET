using System;
using System.IO;
using System.Security;
using System.Text;
using System.Diagnostics; 
using System.Collections;
using skky.util;
using skky.web;


namespace skky
{
	/// <summary>
	/// Summary description for SkkyObject.
	/// </summary>
	public class SkkyObject : ICloneable
	{
		public const string sClassName = "SkkyObject";
		public const int iHashCode = 1;

		public const int RC_Success				= 0;

		public const int RC_UNKNOWN_FAILURE		= -32767;
		public const int RC_Error				= RC_UNKNOWN_FAILURE;

		public const int RC_BAD_DATE			= -1;
		public const int RC_BAD_START_CITY		= -2;
		public const int RC_BAD_END_CITY		= -3;
		public const int RC_BAD_FLIGHT			= -4;
		public const int RC_BAD_CLASS			= -5;
		public const int RC_BAD_SEGNUM			= -6;
		public const int RC_NOT_ON_QUEUE		= -7;
		public const int RC_NOT_ALLOWED_EMULATE	= -8;
		public const int RC_BAD_LENGTH			= -9;

		public const int RC_SINGLE_ITEM_FIELD		= -10;
		public const int RC_INVALID_WORKSPACE		= -11;
		public const int RC_SIGNED_OUT				= -12;
		public const int RC_OFF_QUEUE_NOT_ALLOWED	= -13;
		public const int RC_NO_ITIN					= -14;
		public const int RC_NO_STAR					= -15;
		public const int RC_NO_COMPANY_PROFILE		= -16;
		public const int RC_NO_TRAVELER_PROFILE		= -17;
		public const int RC_NO_EXACT_MATCH			= -18;
		public const int RC_NEED_NAMES				= -19;

				// Copied error returns
		public const int RC_BAD_AVAIL_LINE			= -21;
		public const int RC_BAD_STATUS_CODE			= -22;


		public const int RC_NO_SEATS              = -20;
		public const int RC_BAD_ID                = -21;
		public const int RC_BAD_CD                = -22;
		public const int RC_NO_AVAIL              = -23;
		public const int RC_CAR_COMPANY_NOT_AVAIL = -24;
		public const int RC_UC_SEGMENT            = -25;
		public const int RC_NEED_CD               = -26;
		public const int RC_NEED_CC               = -27;
		public const int RC_BAD_CC_NUMBER         = -28;
		public const int RC_BAD_CC_EXP_DATE       = -29;

		public const int RC_BAD_GTY          = -30;
		public const int RC_BAD_SEAT_NUMBER  = -31;
		public const int RC_END_FAILURE      = -32;
		public const int RC_SIMULT_CHANGE    = -33;
		public const int RC_BAD_LOCATOR      = -34;
		public const int RC_FINISH_OR_IGNORE = -35;
		public const int RC_ON_QUEUE         = -36;
		public const int RC_BAD_PCC          = -37;
		public const int RC_BAD_QUEUE        = -38;
		public const int RC_BAD_LINE_NUMBER  = -39;

		public const int RC_NAMELIST			= -40;
		public const int RC_NOT_AUTHORIZED		= -41;
		public const int RC_INCORRECT_PASSWORD	= -42;
		public const int RC_RESTRICTED			= -43;
		public const int RC_PRINTER_ERROR		= -44;
		public const int RC_NOT_IMPLEMENTED		= -45;
		public const int RC_FLIGHT_NOOP			= -46; // Flight not operating this day.
		public const int RC_BAD_NUMBER_SEATS	= -47;
		public const int RC_BAD_FORMAT			= -48;
		public const int RC_BAD_CITY			= -49; // Incorrect Pseudo City

		public const int RC_NEED_DISPLAY_MODE  = -50; // Must be in Profile Display Mode
		public const int RC_LINK_NOT_AVAILABLE = -51;
		public const int RC_HOST_ERROR_STATE   = -52;
		public const int RC_NO_FIRST_AVAILABLE = -53;
		public const int RC_NO_FLIGHTS         = -54;
		public const int RC_NON_PARTICIPANT    = -55;
		public const int RC_INVALID_CARRIER    = -56;
		public const int RC_INVALID_SERVICE    = -57;
		public const int RC_NO_FARES           = -58;
		public const int RC_INVALID_HANDLE     = -59;

		public const int RC_INVALID_NUMBER_IN_PARTY = -60;
		public const int RC_INVALID_SSR             = -61;
		public const int RC_DUPLICATE_SSR           = -62;
		public const int RC_INVALID_MEALCODE        = -63;
		public const int RC_NO_PNR                  = -64;
		public const int RC_NO_MORE_ENTRIES         = -65;
		public const int RC_INVALID_RATE_VALUE      = -66;
		public const int RC_INVALID_RATE_TYPE       = -67;
		public const int RC_INVALID_CURRENCY_CODE   = -68;
		public const int RC_UPDATE_IN_PROGRESS      = -69;

		public const int RC_INVALID_NEG_RATE_CODE   = -70;
		public const int RC_INVALID_NUM_ADULTS      = -71;
		public const int RC_TOO_MANY_RATES		   	= -72;
		public const int RC_INVALID_PROPERTY_NUM	= -73;
		public const int RC_INVALID_VENDOR		   	= -74;
		public const int RC_INVALID_ROOM_TYPE		= -75;
		public const int RC_NEED_ADDRESS			= -76;
		public const int RC_ROOM_NOT_AVAIL		   	= -77;
		public const int RC_RATES_NOT_AVAIL		   	= -78;
		public const int RC_INVALID_NAME_OR_ADDR	= -79;

		public const int RC_NEED_REFERENCE				= -80;
		public const int RC_NO_PROPERTIES				= -81;
		public const int RC_NO_TAX_INFO					= -82;
		public const int RC_NO_INFO						= -83;
		public const int RC_DESC_CODE_NOT_FOUND			= -84;
		public const int RC_INVALID_DISTANCE			= -85;
		public const int RC_ZIP_AREA_CODE_NOT_ALLOWED	= -86;
		public const int RC_HOST_NOT_RESPONDING         = -87;

		public const int RC_Exception				= -200;
		public const int RC_InvalidParameter		= -201;
		public const int RC_Null					= -202;
		public const int RC_NotFound				= -203;

		public const int RC_FileNotFound			= -300;

		public const string Const_Description	= "Description";
		public const string Const_Login			= "Login";
		public const string Const_Name          = "Name";
		public const string Const_Notes         = "Notes";
		public const string Const_Password		= "Password";
		public const string Const_StartDate		= "StartDate";
		public const string Const_Status		= "Status";
		public const string Const_StopDate		= "StopDate";
		public const string Const_Type          = "Type";
		public const string Const_Value         = "Value";

		public SkkyObject()
		{}
		public virtual string GetClassName() { return sClassName; }

		public override bool Equals(Object obj) { return true; }
		public override int GetHashCode()
		{
			return iHashCode;
		}

		public static bool AllGood(int rc) { return (rc >= 0); }

		public virtual void Clear() {}
		public virtual void Copy(SkkyObject rhs) {}
		public virtual object Clone()
		{
			return new SkkyObject(); // No data as of yet.
		}
		public static bool HasData(ArrayList arrayList) 
		{
			if (arrayList == null)
				return false;
			if (arrayList.Count > 0)
				return true;

			return false;
		}
		public static ArrayList NonNull(ArrayList arrayList)
		{
			if (arrayList != null)
				return arrayList;

			return new ArrayList();
		}

		public static string GetGlobalDBString()
		{
			return "Server=(local);Database=Dave;User Id=sa;Password=;";
		}

		public static string StartTag(string tagName)
		{
			return "<" + tagName.Trim() + ">";
		}
		public static string EndTag(string tagName)
		{
			return "</" + tagName.Trim() + ">";
		}
		public static string EmptyTag(string tagName)
		{
			return "<" + tagName.Trim() + " />";
		}
		public static string AddTag(string tagName, string value)
		{
			if(!string.IsNullOrEmpty(value))
				return StartTag(tagName) + XMLHelper.MakeEntitySafe(value) + EndTag(tagName);

			return EmptyTag(tagName);
		}
		public static string AddHtmlTag(string tagName, string data) 
		{
			if(!string.IsNullOrEmpty(tagName)) 
			{
				if(!string.IsNullOrEmpty(data))
					return StartTag(tagName) + data + EndTag(tagName);
            
				return EmptyTag(tagName);
			}
        
			return string.Empty;
		}
		public static String AddTag(string tagName, int intValue) 
		{
			return AddTag(tagName, intValue.ToString());
		}
		public static String AddTag(string tagName, long longValue)
		{
			return AddTag(tagName, longValue.ToString());
		}
		public static String AddTag(string tagName, bool boolValue)
		{
			return AddTag(tagName, boolValue.ToString());
		}

		public override string ToString() 
		{
			return ToXML();
		}
		public virtual string ToXML()
		{
			return StartTag(GetClassName()) + ToXMLAttribs() + EndTag(GetClassName());
		}
		public virtual string ToXMLAttribs() { return string.Empty; }

		public static int log(string sClassName, string sMethodName, string s) 
		{
			string str = PrintToSystemStatic(sClassName, sMethodName, s);
			return RC_Success;
		}
		public static int log(string sClassName, string s)
		{
			return log(sClassName, null, s);
		}
		public int log(string s) 
		{
			return log(GetClassName(), s);
		}
		public static int log(string sClassName, string sMethodName, Exception e) 
		{
			return log(sClassName, sMethodName, e.ToString());
		}
		public static int log(string sClassName, Exception e) 
		{
			return log(sClassName, null, e);
		}
		public int log(Exception e) 
		{
			return log(null, e);
		}

		public static string PrintToSystemStatic(string sClassName, string sMethodName, string s) 
		{
			string str = string.Empty;
			if (!string.IsNullOrEmpty(sClassName))
			{
				str += sClassName;
				if (!string.IsNullOrEmpty(sMethodName))
					str += "." + sMethodName;
				str += ": ";
			}

			str += s ?? string.Empty;
			Debug.WriteLine(str); 
			//System.Console.WriteLine(str);

			return str;
		}
		public static string PrintToSystemStatic(string sClassName, string s) 
		{
			return PrintToSystemStatic(sClassName, null, s);
		}
		public virtual string PrintToSystemMethod(string sMethodName, string s)
		{
			return PrintToSystemStatic(GetClassName(), sMethodName, s);
		}
		public virtual string PrintToSystem(string s) 
		{
			return PrintToSystemMethod(null, s);
		}
		public virtual string PrintToSystem(Exception e)
		{
			return PrintToSystem(e.ToString());
		}
		public static string PrintToSystem(string sClassName, string sMethodName, Exception e)
		{
			return PrintToSystemStatic(sClassName, sMethodName, e.ToString());
		}
		public static string PrintToSystemStatic(string sClassName, Exception e)
		{
			return PrintToSystem(sClassName, null, e);
		}
		public virtual string PrintToSystem(string sMethodName, Exception e)
		{
			return PrintToSystemMethod(sMethodName, e.ToString());
		}
	}
}
