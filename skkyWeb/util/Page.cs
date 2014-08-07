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
				/*
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
				*/
			}
			catch (Exception ex)
			{
				skky.util.Trace.Critical("LogActivity: " + action ?? ".");
				skky.util.Trace.Critical(ex);
			}
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
			Response.Write(XMLHelper.Label(msg, color));
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

		public List<StringIntDoubleDateTime> PerformMetricConversions(IEnumerable<StringIntDoubleDateTime> siddCollection, bool useMetric = false)
		{
			List<StringIntDoubleDateTime> siList = siddCollection.ToList();
			// The field is metric=true, because the db is all in metric.
			IConversion converter = new KilometersToMiles();
			foreach (StringInt si in siList)
				Converters.ConvertIntUnits(si, converter, false, useMetric);

			converter = new KilogramsToPounds();
			foreach (StringIntDouble sid in siList)
				Converters.ConvertDoubleUnits(sid, converter, false, useMetric);
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

		protected virtual ChartSettings GetChartSettings() {
			return new ChartSettings();
		}
	}
}
