using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.SessionState;
using System.Web;
using skky.util;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using skky.Types;

namespace skkyWeb.util
{
	public static class Http
	{
		public static CallParams GetCallParams(this HttpRequest request)
		{
			return request.LoadCallParams(null);
		}
		public static SkkyCallParams GetSkkyCallParams(this HttpRequest request)
		{
			return request.LoadSkkyCallParams(null);
		}
		public static CallParams LoadCallParams(this HttpRequest request, CallParams cp)
		{
			if (cp == null)
				cp = new CallParams();

			cp.act = request.GetRequestParamString(SkkyCallParams.CallParam_act).ToLower();	// This one is special. It's the action!

			cp.Name = request.GetRequestParamString(SkkyCallParams.CallParam_Name);
			cp.Title = request.GetRequestParamString(SkkyCallParams.CallParam_Title);
			cp.Type = request.GetRequestParamString(SkkyCallParams.CallParam_Type);
			cp.Category = request.GetRequestParamString(SkkyCallParams.CallParam_Category);
			cp.Classification = request.GetRequestParamString(SkkyCallParams.CallParam_Classification);
			//cp.DateRange = request.GetRequestParamString(SkkyCallParams.CallParam_DateRange);
			cp.DateKey = request.GetRequestParamString(SkkyCallParams.CallParam_DateKey);
			cp.Order = request.GetRequestParamInt(SkkyCallParams.CallParam_Order);
			cp.UseMetric = request.GetRequestParamBool(SkkyCallParams.CallParam_UseMetric);

			return cp;
		}
		public static SkkyCallParams LoadSkkyCallParams(this HttpRequest request, SkkyCallParams cp)
		{
			if (cp == null)
				cp = new SkkyCallParams();

			request.LoadCallParams(cp);

			cp.AccountName = request.GetRequestParamString(SkkyCallParams.CallParam_AccountName);
			cp.AccountNumber = request.GetRequestParamString(SkkyCallParams.CallParam_AccountNumber);
			cp.AccountId = request.GetRequestParamInt(SkkyCallParams.CallParam_AccountId);

			cp.DepartmentName = request.GetRequestParamString(SkkyCallParams.CallParam_DepartmentName);
			cp.DepartmentId = request.GetRequestParamInt(SkkyCallParams.CallParam_DepartmentId);

			cp.EmissionType = request.GetRequestParamString(SkkyCallParams.CallParam_EmissionType);

			cp.CityName = request.GetRequestParamString(SkkyCallParams.CallParam_CityName);
			cp.CityCode = request.GetRequestParamString(SkkyCallParams.CallParam_CityCode);
			cp.CityId = request.GetRequestParamInt(SkkyCallParams.CallParam_CityId);
			cp.CityCriteria = request.GetRequestParamString(SkkyCallParams.CallParam_CityCriteria);

			cp.Equipment = request.GetRequestParamString(SkkyCallParams.CallParam_Equipment);
			cp.Vendor = request.GetRequestParamString(SkkyCallParams.CallParam_Vendor);

			cp.ZipCode = request.GetRequestParamString(SkkyCallParams.CallParam_ZipCode);

			cp.PartName = request.GetRequestParamString(SkkyCallParams.CallParam_PartName);
			cp.PartId = request.GetRequestParamGuid(SkkyCallParams.CallParam_PartId);
			cp.TermName = request.GetRequestParamString(SkkyCallParams.CallParam_TermName);
			cp.TermId = request.GetRequestParamGuid(SkkyCallParams.CallParam_TermId);

			cp.BomItemId = request.GetRequestParamInt(SkkyCallParams.CallParam_BOMItemId);

			cp.Grouping = request.GetRequestParamString(SkkyCallParams.CallParam_Grouping);
			cp.PrimaryGrouping = request.GetRequestParamString(SkkyCallParams.CallParam_PrimaryGrouping);

			cp.TravelType = request.GetRequestParamString(SkkyCallParams.CallParam_TravelType);

			cp.TimeFrame = request.GetRequestParamString(SkkyCallParams.CallParam_TimeFrame);
			cp.TimePeriod = request.GetRequestParamString(SkkyCallParams.CallParam_TimePeriod);

			cp.PropertyName = request.GetRequestParamString(SkkyCallParams.CallParam_PropertyName);

			cp.ReportName = request.GetRequestParamString(SkkyCallParams.CallParam_ReportName);
			cp.ReportTitle = request.GetRequestParamString(SkkyCallParams.CallParam_ReportTitle);
			cp.DateTimeTitle = request.GetRequestParamString(SkkyCallParams.CallParam_DateTimeTitle);
			cp.DoubleTitle = request.GetRequestParamString(SkkyCallParams.CallParam_DoubleTitle);
			cp.IntTitle = request.GetRequestParamString(SkkyCallParams.CallParam_IntTitle);
			cp.Int2Title = request.GetRequestParamString(SkkyCallParams.CallParam_Int2Title);
			cp.StringTitle = request.GetRequestParamString(SkkyCallParams.CallParam_StringTitle);

			return cp;
		}
		public static string GetRequestString(this HttpRequest request, string name)
		{
			string str = string.Empty;
			if (request != null && !string.IsNullOrEmpty(name))
				str = Convert.ToString(request.Params[name]) ?? string.Empty;

			if (string.IsNullOrEmpty(str))
				str = request[name] ?? string.Empty;

			return str;
		}
		public static string GetRequestParamString(this HttpRequest request, string name)
		{
			if (request != null && !string.IsNullOrEmpty(name))
				return Convert.ToString(request.Params[name]) ?? string.Empty;

			return string.Empty;
		}
		public static bool GetRequestParamBool(this HttpRequest request, string name)
		{
			string str = request.GetRequestParamString(name);
			if (!string.IsNullOrEmpty(str))
			{
				switch (str.ToLower())
				{
					case "0":
					case "true":
					case "yes":
					case "y":
						return true;
				}
			}

			return false;
		}
		public static Guid GetRequestParamGuid(this HttpRequest request, string name)
        {
            try
            {
				return new Guid(GetRequestParamString(request, name));
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }
        public static int GetRequestParamInt(this HttpRequest request, string name)
		{
			return GetRequestParamString(request, name).ToInteger();
		}
		public static long GetRequestParamLong(this HttpRequest request, string name)
		{
			return GetRequestParamString(request, name).ToLong();
		}
		public static List<int> GetRequestParamListInt(this HttpRequest request, string name)
		{
			return GetRequestParamString(request, name).ToIntegerList();
		}
		public static List<string> GetRequestParamListString(this HttpRequest request, string name)
		{
			return GetRequestParamString(request, name).ToStringList();
		}

		public static string GetServerVariable(this HttpRequest req, string name)
		{
			string str = req.ServerVariables[name];

			return str;
		}
		public static string GetHTTPHost(this HttpRequest req)
		{
			return GetServerVariable(req, "http_host");
		}

		public static string GetSessionString(HttpSessionState ses, string name)
		{
			string sessionString = string.Empty;
			if (ses != null && !string.IsNullOrEmpty(name))
			{
				object o = ses[name];
				if (o != null)
					sessionString = o.ToString();
			}

			return sessionString ?? string.Empty;
		}
		public static int GetSessionInt(HttpSessionState ses, string name)
		{
			string sessionString = GetSessionString(ses, name);

			return sessionString.ToInteger();
		}

		public static void PrintRequestFormVariables(HttpRequest Request, HttpResponse Response, bool IsPostBack)
		{
			Response.Write("<table border=\"1\"><tr><td>key</td><td>Value</td></tr>");

			Response.Write("<tr><td>Postback</td><td>");
			if (IsPostBack)
				Response.Write("Yes");
			else
				Response.Write("No");
			Response.Write("</td></tr>");

			foreach (var x in Request.Form)
			{
				Response.Write("<tr><td>");
				Response.Write(x.ToString());
				Response.Write("</td><td>");
				Response.Write(Request.Form[x.ToString()]);
				Response.Write("</td></tr>");
			}
			Response.Write("</table>");
		}

		public static HttpCookie AddCookie(HttpResponse response, string name, string value, DateTime dateExpires)
		{
			HttpCookie cookie = null;
			if (response != null && !string.IsNullOrEmpty(name))
			{
				cookie = new HttpCookie(name);
				cookie.Value = value ?? string.Empty;
				cookie.Expires = dateExpires;
				//cookie.Path = "";

				response.Cookies.Add(cookie);
			}

			return cookie;
		}
		public static HttpCookie AddCookieWithPath(HttpResponse response, string name, string value, DateTime dateExpires, string path)
		{
			HttpCookie cookie = null;
			if (response != null && !string.IsNullOrEmpty(name))
			{
				cookie = new HttpCookie(name);
				cookie.Value = value ?? string.Empty;
				cookie.Expires = dateExpires;
				cookie.Path = path;
				//cookie.Path = "";

				response.Cookies.Add(cookie);
			}

			return cookie;
		}

		public static string getCookie(HttpRequest request, string cookieName)
		{
			string str = string.Empty;

			if (request != null && !string.IsNullOrEmpty(cookieName) && request.Cookies.Count > 0)
			{
				for (int i = 0; i < request.Cookies.Count; ++i)
				{
					HttpCookie cookie = request.Cookies.Get(i);

					if (cookie.Name == cookieName)
					{
						str = cookie.Value;
						break;
					}
				}
			}

			return str;
		}

		public static string PrintCookies(HttpRequest request)
		{
			string str = string.Empty;

			if (request != null && request.Cookies.Count > 0)
			{
				for (int i = 0; i < request.Cookies.Count; ++i)
				{
					HttpCookie cookie = request.Cookies.Get(i);

					if (i > 0)
						str += "\n\n";
					str += "Name: " + cookie.Name + "\n";
					str += "Domain: " + cookie.Domain + "\n";
					str += "Expires: " + cookie.Expires.ToShortDateString() + "\n";
					str += "Path: " + cookie.Path + "\n";
					str += "Secure: " + cookie.Secure.ToString() + "\n";
					str += "Value: " + DecodeCookieString(cookie.Value) + "\n";
				}
			}

			return str;
		}

		public static string EncodeCookieString(string strHtml)
		{
			strHtml = strHtml.Replace("<", "#lt#");
			strHtml = strHtml.Replace(">", "#gt#");
			strHtml = strHtml.Replace("\"", "#dqt#");
			strHtml = strHtml.Replace("'", "#sqt#");

			return strHtml;
		}

		public static string DecodeCookieString(string strHtml)
		{
			strHtml = strHtml.Replace("#lt#", "<");
			strHtml = strHtml.Replace("#gt#", ">");
			strHtml = strHtml.Replace("#dqt#", "\"");
			strHtml = strHtml.Replace("#sqt#", "'");

			return strHtml;
		}
	}
}
