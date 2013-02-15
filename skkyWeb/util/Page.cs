using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Web.SessionState;
using skky.Conversions;
using skky.db;
using skky.Types;
using skky.util;
using skky.web;
using skkyWeb.Security;
using skkyWeb.Charts;

namespace skkyWeb.util
{
	public class Page : System.Web.UI.Page
	{
		public const string SESSION_CallParams = "CallParams";
		public const string SESSION_LastPage = "LastPage";

		protected static readonly Color Const_ColorMessageError = Color.FromArgb(0xcc, 0x33, 0x33);
		protected static readonly Color Const_ColorMessageSuccess = Color.FromArgb(0x33, 0xCC, 0x33);
		protected string MessageError;
		protected string MessageSuccess;

		protected string HttpHost
		{
			// This is the Requests ApplicationURI
			get { return Http.GetHTTPHost(Request).TrimEnd('/') + "/"; }
			set { }
		}

		protected bool TraceRequests { get; set; }

		protected void TraceSession(string action)
		{
			string msg = string.Empty;
			if (Session != null)
			{
				foreach (var x in Session)
				{
					if (x != null)
						msg += "\r\n" + x.ToString() + ": " + Session[x.ToString()];
				}
			}

			if (string.IsNullOrEmpty(msg))
				msg = "None";
			skky.util.Trace.Information((action ?? "Unknown Page") + " Session Variables: " + msg);
		}
		protected void TraceRequest(string action)
		{
			string msg = string.Empty;
			if (Request != null && Request.Params != null)
			{
				foreach (var x in Request.Params)
				{
					if (x != null)
						msg += "\r\n" + x.ToString() + ": " + Request.Params[x.ToString()];
				}
			}

			if (string.IsNullOrEmpty(msg))
				msg = "None";
			skky.util.Trace.Information((action ?? "Unknown Page") + " Params: " + msg);
		}
		protected void TraceRequestForm(string action)
		{
			string msg = string.Empty;
			if (Request != null && Request.Form != null)
			{
				foreach (var x in Request.Form)
				{
					if (x != null)
						msg += "\r\n" + x.ToString() + ": " + Request.Form[x.ToString()];
				}
			}

			if (string.IsNullOrEmpty(msg))
				msg = "None";
			skky.util.Trace.Information((action ?? "Unknown Page") + " Form Variables: " + msg);
		}

		protected string GetPageName()
		{
			return System.IO.Path.GetFileName(Page.Request.FilePath);
		}

		private void UpdatePageState()
		{
			Object obj = Session[SESSION_LastPage];
			string origPageName = (obj == null ? string.Empty : obj.ToString());

			string pageName = GetPageName();
			if (pageName.ToLower() != "ajax.aspx" && pageName.ToLower() != "json.aspx")
			{
				Session[SESSION_LastPage] = pageName;

				if (pageName != origPageName)
					Session.Remove(SESSION_CallParams);
			}
		}
		private void UpdatePageState(CallParams sp)
		{
			if (sp != null)
			{
				List<CallParams> scp = (List<CallParams>)Session[SESSION_CallParams];
				if (scp == null)
					scp = new List<CallParams>();

				scp.Add(sp);
				Session[SESSION_CallParams] = scp;
			}
		}
		protected CallParams GetLastCallParams()
		{
			List<CallParams> scp = (List<CallParams>)Session[SESSION_CallParams];
			if (scp != null)
				return scp.Last();

			return null;
		}
		protected ChartSettings GetLastChartSettings()
		{
			CallParams cp = GetLastCallParams();

			return cp as ChartSettings;
		}

		protected void LogActivity(string action)
		{
			try
			{
				UpdatePageState();

				if (TraceRequests)
				{
					TraceRequest(action);
					TraceSession(action);
					TraceRequestForm(action);
				}

				//Only proceed if the user is authenticated
				if (Request.IsAuthenticated)
				{
					//Get information about the currently logged on user
					MembershipUser usr = Membership.GetUser();
					//Whoops, we don't know who this user is!
					if (usr != null)
					{
						//Read in the user's UserId value
						Guid UserId = (Guid)usr.ProviderUserKey;

						//Call the sproc_UpdateUsersCurrentActivity sproc
						string sqlServer = skky.app.WebConfig.MembershipConnectionString;
						if (!string.IsNullOrEmpty(sqlServer))
						{
							using (SqlConnection myConnection = new SqlConnection(sqlServer))
							{
								SqlCommand myCommand = new SqlCommand("UpdateUserActivity", myConnection);
								myCommand.CommandType = CommandType.StoredProcedure;

								myCommand.Parameters.AddWithValue("@UserId", UserId);
								myCommand.Parameters.AddWithValue("@Action", action);

								//Execute the sproc
								myConnection.Open();
								myCommand.ExecuteNonQuery();
								myConnection.Close();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				skky.util.Trace.Critical("LogActivity: " + action ?? ".");
				skky.util.Trace.Critical(ex);
			}
		}

		public virtual PortalUser GetUser()
		{
			return GetUserStatic();
		}
		public virtual UserWebSettings GetUserSettings()
		{
			return GetUser().Settings;
		}

		public void writeImageTag(string file, string alt)
		{
			Response.Write(Html.BuildImg(skky.app.WebConfig.CompanyImageBaseUri + file, 0, 0, alt));
		}

		public virtual Client GetClient()
		{
			return GetUser().Client;
		}
		public virtual int GetClientID()
		{
			return GetClient().id;
		}
		public virtual string GetClientName()
		{
			return GetClient().Name;
		}
		public virtual string GetClientLogoPath()
		{
			return GetClient().LogoPath ?? string.Empty;
		}
		public virtual string GetClientURL()
		{
			return GetClient().url ?? string.Empty;
		}
		public virtual string GetClientHREFLogo()
		{
			return Html.GetHREFImage(GetClient().Name, GetClient().url, GetClient().LogoPath, GetClient().LogoWidth, GetClient().LogoHeight);
		}

		public virtual Customer GetCustomer()
		{
			return GetUser().Customer;
		}
		public virtual int GetCustomerID()
		{
			return GetCustomer().id;
		}
		public virtual string GetCustomerName()
		{
			return GetCustomer().Name;
		}
		public virtual string GetCustomerLogoPath()
		{
			return GetCustomer().LogoPath ?? string.Empty;
		}
		public virtual string GetCustomerURL()
		{
			return GetCustomer().url ?? string.Empty;
		}
		public virtual string GetCustomerHREFLogo()
		{
			return Html.GetHREFImage(GetCustomerName(), GetCustomerURL(), GetCustomerLogoPath(), GetCustomer().LogoWidth, GetCustomer().LogoHeight);
		}

		public static PortalUser GetUserStatic()
		{
			return UserController.CurrentPortalUser;
		}
		public static UserWebSettings GetUserSettingsStatic()
		{
			return GetUserStatic().Settings;
		}

		public static Client GetClientStatic()
		{
			return GetUserStatic().Client;
		}
		public static int GetClientIDStatic()
		{
			return GetClientStatic().id;
		}
		public static string GetClientNameStatic()
		{
			return GetClientStatic().Name;
		}
		public static string GetClientLogoPathStatic()
		{
			return GetClientStatic().LogoPath ?? string.Empty;
		}
		public static string GetClientURLStatic()
		{
			return GetClientStatic().url ?? string.Empty;
		}

		public static Customer GetCustomerStatic()
		{
			return GetUserStatic().Customer;
		}
		public static int GetCustomerIDStatic()
		{
			return GetCustomerStatic().id;
		}
		public static string GetCustomerNameStatic()
		{
			return GetCustomerStatic().Name;
		}
		public static string GetCustomerLogoPathStatic()
		{
			return GetCustomerStatic().LogoPath ?? string.Empty;
		}
		public static string GetCustomerURLStatic()
		{
			return GetCustomerStatic().url ?? string.Empty;
		}

		protected virtual Namer.Type GetEmissionTypeEnum()
		{
			return GetUserSettings().GetEmissionType();
		}
		protected virtual string GetEmissionType()
		{
			return GetUserSettings().EmissionType;
		}
		protected virtual DateSettings GetDateRange()
		{
			return GetUserSettings().DateRange;
		}
		protected virtual List<string> GetSelectedSources()
		{
			return GetUserSettings().GetSelectedSources();
		}
		protected virtual string GetAccountName()
		{
			return GetUserSettings().AccountName;
		}
		protected virtual string GetAccountNumber()
		{
			return GetUserSettings().AccountNumber;
		}
		protected virtual string GetDepartmentName()
		{
			return GetUserSettings().DepartmentName;
		}
		protected virtual int GetDepartmentId()
		{
			return GetUserSettings().DepartmentId;
		}

		protected virtual bool UserIsMetric()
		{
			return GetUserSettings().UseMetric;
		}

		public string GetRequestString(string name)
		{
			return Http.GetRequestString(Request, name);
		}
		public string GetRequestParamString(string name)
		{
			return Http.GetRequestParamString(Request, name);
		}
        public Guid GetRequestParamGuid(string name)
        {
            return Http.GetRequestParamGuid(Request, name);
        }
        public int GetRequestParamInt(string name)
        {
            return Http.GetRequestParamInt(Request, name);
        }
        public long GetRequestParamLong(string name)
		{
			return Http.GetRequestParamLong(Request, name);
		}

		protected string GetSessionString(string name)
		{
			return Http.GetSessionString(Session, name);
		}
		protected int GetSessionInt(string name)
		{
			return Http.GetSessionInt(Session, name);
		}

		public virtual bool IsMetric
		{
			get
			{
				return GetUserSettings().UseMetric;
			}
			set
			{
				;
			}
		}

		protected void WriteIntegerNoZero(int i)
		{
			Response.Write(i.ToString(NumberDisplay.NoDecimalsNoZeroWithoutComma));
		}
		protected void WriteDoubleNoZero(double d)
		{
			Response.Write(d.ToString(NumberDisplay.TwoDecimalsOptionalNoZeroWithoutComma));
		}
		protected void WriteDoubleTwoZeros(double d)
		{
			Response.Write(d.ToString(NumberDisplay.TwoDecimalsNoZeroWithoutComma));
		}
		public void WriteChecked(int currentItem, int targetItemToCheck)
		{
			if(currentItem == targetItemToCheck)
				Response.Write("checked=\"checked\"");
		}

		private void WriteMessageLabel(string msg, Color color)
		{
			Response.Write(Html.Label(msg, color));
		}
		protected void WriteResponseMessages()
		{
			if (!string.IsNullOrEmpty(MessageError))
				WriteMessageLabel(MessageError, Const_ColorMessageError);

			if (!string.IsNullOrEmpty(MessageSuccess))
				WriteMessageLabel(MessageSuccess, Const_ColorMessageSuccess);

			MessageError = string.Empty;
			MessageSuccess = string.Empty;
		}

		protected void WriteResponseFormVariables()
		{
			Http.PrintRequestFormVariables(Request, Response, IsPostBack);
		}

		protected void addLabelToPage(string str)
		{
			if (!string.IsNullOrEmpty(str))
				Html.AddLabelToControl(Page.Controls, str);
		}

		public string KilometersMilesLongName()
		{
			return KilometersToMiles.GetLongName(IsMetric);
		}
		public string KilometersMilesShortName()
		{
			return KilometersToMiles.GetShortName(IsMetric);
		}
		public string KilogramsPoundsLongName()
		{
			return KilogramsToPounds.GetLongName(IsMetric);
		}
		public string KilogramsPoundsShortName()
		{
			return KilogramsToPounds.GetShortName(IsMetric);
		}
		public string LitresGallonsLongName()
		{
			return LitersToGallons.GetLongName(IsMetric);
		}
		public string LitresGallonsShortName()
		{
			return LitersToGallons.GetShortName(IsMetric);
		}

		public List<StringIntDoubleDateTime> PerformMetricConversions(IEnumerable<StringIntDoubleDateTime> siddCollection)
		{
			List<StringIntDoubleDateTime> siList = siddCollection.ToList();
			// The field is metric=true, because the db is all in metric.
			IConversion converter = new KilometersToMiles();
			foreach (StringInt si in siList)
				Converters.ConvertIntUnits(si, converter, false, GetUserSettings().UseMetric);

			converter = new KilogramsToPounds();
			foreach (StringIntDouble sid in siList)
				Converters.ConvertDoubleUnits(sid, converter, false, GetUserSettings().UseMetric);
			/*
			for (int i = 0; i < siList.Count(); ++i)
			{
				StringIntDoubleDateTime sid = siList[i];
				if (sid.doubleValue == 0.0)
				{
					siList.RemoveAt(i);
					--i;
				}
			}
			*/
			return siList;
		}

		protected void AddJavaScriptToHead(string str)
		{
			HtmlGenericControl script = new HtmlGenericControl("script");
			script.SetAttribute("type", "text/javascript");
			//script.InnerHtml = "alert('JavaScript in Page Header');";
			script.InnerHtml = str;
			//script.InnerText = str;
			Header.AddChild(script);
		}

		protected string GoogleMapJavascriptLocation
		{
			get
			{
				//return "http://www.google.com/jsapi?key=" + skky.app.WebConfig.GoogleMapKey;	// Version 2
				return "http://maps.google.com/maps/api/js?sensor=false";	// Version 3
			}
			private set { }
		}

		protected static Parameter AddOrUpdateReportQueryParameter(ParameterCollection selectParameters, string selectName, string selectValue)
		{
			Parameter paramFound = null;
			for (int i = 0; i < selectParameters.Count; ++i)
			{
				Parameter param = selectParameters[i];
				if (param.Name == selectName)
				{
					paramFound = param;
					break;
				}
			}

			if (paramFound == null)
			{
				int indexOfAddedItem = selectParameters.Add(selectName, selectValue);
				paramFound = selectParameters[indexOfAddedItem];
			}
			else
			{
				paramFound.DefaultValue = selectValue;
			}

			return paramFound;
		}

		protected Control GetChart(ChartManager cm, skkyjson sj)
		{
			if (cm == null)
				return null;

			try
			{
				if (string.IsNullOrEmpty(cm.AccountName))
					cm.AccountName = GetAccountName();
				if (string.IsNullOrEmpty(cm.AccountNumber))
					cm.AccountNumber = GetAccountNumber();
				if (cm.DepartmentId == 0)
					cm.DepartmentId = GetDepartmentId();
				if (string.IsNullOrEmpty(cm.DepartmentName))
					cm.DepartmentName = GetDepartmentName();
				if (string.IsNullOrEmpty(cm.EmissionType))
					cm.EmissionType = GetEmissionType();

				cm.ChartSettings.DateRange = GetDateRange();

				return ChartToHtml.GetChart(cm, Server, sj, this, this);
			}
			catch (Exception ex)
			{
				ChartErrorEvent e = new ChartErrorEvent(GetUser().DisplayName, GetCustomerName(), cm.Name, "ChartFactory.GetChart", this, ex);
				e.Raise();
			}

			return null;
		}

		protected Control AddChartToControl(Control ctl, ChartManager cm)
		{
			return AddChartToControl(ctl, cm, null);
		}
		protected Control SetChartToControl(Control ctl, ChartManager cm)
		{
			if (ctl != null)
				ctl.Controls.Clear();

			return AddChartToControl(ctl, cm, null);
		}

		protected Control AddChartToControl(Control ctl, ChartManager cm, skkyjson js)
		{
			Control chart = GetChart(cm, js);
			ctl.AddChild(chart);

			return chart;
		}

		protected virtual ChartSettings GetChartSettings() {
			return new ChartSettings();
		}
		protected virtual ChartManager GetChartManager(ChartSettings cs)
		{
			return new ChartManager(cs, IsMetric);
		}
		protected skkyjson BuildChart(bool isJSON)
		{
			string callType = isJSON ? "JSON" : "AJAX";

			skkyjson sjson = null;
			ChartSettings cs = new ChartSettings();
			try
			{
				Request.LoadSkkyCallParams(cs);
				UpdatePageState(cs);

				string str = string.Empty;

				if (cs.act == "chart")
				{
					LogActivity(callType + " retrieve of chart named: " + cs.Name);

					ChartManager cm = GetChartManager(cs);
					if (isJSON)
					{
						sjson = new skkyjson();
						Control ctl = AddChartToControl(this.Page, cm, sjson);
						str = Html.GetRawHTML(ctl);
						sjson.innerHTML = str;
					}
					else
					{
						AddChartToControl(this.Page, cm);
					}
				}
				else
				{
					str = "The action selected is: " + cs.act;
				}
			}
			catch (Exception ex)
			{
				WebError we = new WebError(GetCustomerName(), GetUser().DisplayName, callType + " call failed", this, 1, ex);
				we.Raise();
			}

			return sjson;
		}
	}
}
