using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using skky.Types;
using skky.util;
using System.Net;
using System.Configuration;
using System.Reflection;
using skky.jqGrid;
using System.Text;
using System.Web.UI;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web.Helpers;

namespace skkyMVC.Controllers
{
	public class skkyController : Controller
	{
		public const string Const_FileURLPath = "/App_Data";

		public const string CONST_Date4DigitYear = "MMM d, yyyy";
		public const string CONST_DateTimeLong = "MMMM dd, yyyy hh:mm:ss.ff tt";

		protected ReturnStatus rs = new ReturnStatus();
		protected DateTime dtNow = DateTime.Now;

		public FileResult ExcelFile(byte[] bytes, string fileName = null)
		{
			if(!string.IsNullOrWhiteSpace(fileName))
				return base.File(bytes, "application/vnd.ms-excel", fileName);

			return base.File(bytes, "application/vnd.ms-excel");
		}

		public static bool getSortOrder(string sord)
		{
			return !("desc" == sord);
		}

		protected string printMethodAndClassName(string methodName)
		{
			string msg = this.GetType().Name;
			msg += ".";
			msg += methodName ?? string.Empty;

			return msg;
		}

		protected void TraceInformation(string methodName, string msg)
		{
			string className = this.GetType().Name;

			skky.util.Trace.MethodInformation(className, methodName, msg);
		}
		protected string TraceException(string methodName, Exception ex, string msg = "")
		{
			string className = this.GetType().Name;

			string s = skky.util.Trace.MethodException(className, methodName, ex, msg);

			rs.AddError(s);

			return s;
		}

		public string AddGridExceptionToReturnStatus(string action, Exception ex, string actionType = "", [CallerMemberName]string callerMethodName = "")
		{
			string str = "Exception while ";
			switch (action)
			{
				case ActionParams.CONST_ActionAdd:
					str += "adding";
					break;
				case ActionParams.CONST_ActionDelete:
					str += "deleting";
					break;
				default:
				//case ActionParams.CONST_ActionEdit:
					str += "editing";
					break;
			}

			str += " ";
			str += (string.IsNullOrWhiteSpace(actionType) ? "item" : actionType);
			//str += ".";
			//rs.AddError(str);

			TraceException(callerMethodName, ex, str);

			//rs.AddExceptionErrorMessage(ex);

			return str;
		}

		#region HTTP Status returns
		// Return Status errors and exceptions
		protected JsonResult ReturnStatusConflictIfError(ReturnStatus rs, int rc = 0)
		{
			if(null == rs)
				rs = new ReturnStatus(rc);

			if (!rs.HasErrors())
				return Json(rs);

			return ReturnStatusConflict(rs);
		}
		protected ActionResult ContentWithNoDataCheck(string str)
		{
			if (string.IsNullOrEmpty(str))
				return NoDataStatus();

			return Content(str);
		}
		protected JsonResult NotAcceptableStatus(Exception ex, string methodName = null, [CallerMemberName]string callerMethodName = "")
		{
			TraceException(string.IsNullOrEmpty(methodName) ? callerMethodName : methodName, ex);

			Response.TrySkipIisCustomErrors = true;

			// There was some kind of error or exception.
			Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
			return Json(ReturnStatus.AddExceptionErrorMessage(rs, ex));
		}
		protected EmptyResult NoDataStatus()
		{
			Response.TrySkipIisCustomErrors = true;

			// There was some kind of error.
			Response.StatusCode = (int)HttpStatusCode.NoContent;
			return new EmptyResult();
		}
		protected JsonResult ReturnStatusConflict(string errorMessage)
		{
			rs.AddError(errorMessage);

			return ReturnStatusConflict(rs);
		}
		protected JsonResult ReturnStatusConflict(ReturnStatus rs)
		{
			Response.TrySkipIisCustomErrors = true;

			// There was some kind of error.
			Response.StatusCode = (int)HttpStatusCode.Conflict;
			return Json(rs);
		}
		protected JsonResult ReturnStatusNotFound(ReturnStatus rs)
		{
			Response.TrySkipIisCustomErrors = true;

			// There was some kind of error.
			Response.StatusCode = (int)HttpStatusCode.NotFound;
			return Json(rs);
		}
		protected JsonResult ReturnStatusNotFound(string errorMessage)
		{
			rs.AddError(errorMessage);
			return ReturnStatusNotFound(rs);
		}

		protected void AddEditItemException(Exception ex, string objectName)
		{
			rs.AddError("Error editing item.");

			if (ex.GetExceptionMessage().Contains("duplicate"))
			{
				rs.AddMessage("Items are unique.");
				rs.AddMessage("You attempted to add a " + objectName + " that already exists.");
			}
			else
			{
				rs.AddMessage(ex.GetExceptionMessage());
			}

			rs.AddMessage("Please try again.");
		}
		protected void AddEmptyFieldError(string operation, string objectName, string fieldName)
		{
			if (string.IsNullOrWhiteSpace(operation))
				operation = "adding";

			rs.AddError("Error " + operation.ToLower() + " " + objectName + ".");

			rs.AddMessage(fieldName + " cannot be empty.");
			rs.AddMessage("Please try again.");
		}
		protected void AddNumberError(string operation, string field, string rate, string objectName)
		{
			if (string.IsNullOrWhiteSpace(operation))
				operation = "adding";

			rs.AddError("Error " + operation.ToLower() + " " + objectName + ".");

			string s = field ?? "EMPTY";
			s += " field: ";
			s += rate ?? "Rate";
			s += " is not a valid number.";

			rs.AddMessage(s);
			rs.AddMessage("Please try again.");
		}
		#endregion

		protected string GetPageName()
		{
			return System.IO.Path.GetFileName(Request.FilePath);
		}
﻿
		public static string ReverseMapPath(HttpServerUtilityBase server, string path)
		{
			string appPath = server.MapPath("~");
			//string res = string.Format("/{0}", path.Replace(appPath, "").Replace("\\", "/"));
			string res = string.Format("/{0}", path.Replace(appPath, "").Replace("\\", "/"));
			return res;
		}

		public static string getFileDateStyle()
		{
			string fileDateStyle = string.Empty;

			DateTime dt = DateTime.Now;
			int year = (dt.Year % 1000);
			int month = dt.Month;
			int day = dt.Day;

			fileDateStyle += year.ToString().PadLeft(2, '0');
			fileDateStyle += month.ToString().PadLeft(2, '0');
			fileDateStyle += day.ToString().PadLeft(2, '0');

			return fileDateStyle;
		}
		public static string getFileDateTimeStyle()
		{
			string fileDateStyle = string.Empty;

			DateTime dt = DateTime.Now;
			int year = (dt.Year % 1000);
			int month = dt.Month;
			int day = dt.Day;
			int hour = dt.Hour;
			int min = dt.Minute;
			int sec = dt.Second;

			fileDateStyle += year.ToString().PadLeft(2, '0');
			fileDateStyle += month.ToString().PadLeft(2, '0');
			fileDateStyle += day.ToString().PadLeft(2, '0');
			fileDateStyle += hour.ToString().PadLeft(2, '0');
			fileDateStyle += min.ToString().PadLeft(2, '0');
			fileDateStyle += sec.ToString().PadLeft(2, '0');

			return fileDateStyle;
		}

		public static string getTempDirectory(HttpServerUtilityBase Server)
		{
			string savePath = getFileDateStyle();

			string dirToSaveIn = Path.Combine(Server.MapPath(Const_FileURLPath), savePath);
			return dirToSaveIn;
		}

		protected string getTempDirectory()
		{
			string savePath = FileHelper.getFileDateStyle();
			string serverMapPath = Server.MapPath(Const_FileURLPath);

			string dirToSaveIn = System.IO.Path.Combine(serverMapPath, savePath);
			return dirToSaveIn;
		}
﻿
		public string ReverseMapPath(string path)
		{
			string appPath = HttpContext.Server.MapPath("~");
			//string res = string.Format("/{0}", path.Replace(appPath, "").Replace("\\", "/"));
			string res = string.Format("/{0}", path.Replace(appPath, "").Replace("\\", "/"));
			return res;
		}
		
		public static string TimeDifferenceMessage(DateTime dtStart, DateTime dtEnd)
		{
			return DateTimeHelper.TimeDifferenceMessage(dtStart, dtEnd);
		}
﻿
		// Methods for returning common strings.
		protected string getDecimalText(Decimal? d)
		{
			if (!d.HasValue)
				return string.Empty;

			return d.Value.ToString("N0");
		}
		protected string getDoubleText(double? d)
		{
			if (!d.HasValue)
				return string.Empty;

			return d.Value.ToString("N2");
		}
		protected string getBooleanText(bool? b)
		{
			if (!b.HasValue)
				return string.Empty;

			return b.Value.ToString();
		}
		protected string getYesNoText(bool? b)
		{
			if (!b.HasValue)
				return string.Empty;

			return b.Value ? "Yes" : "No";
		}

		#region Razor Render to string
		public string RazorRender(string DefaultAction)
		{
			string Cache = string.Empty;

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.IO.TextWriter tw = new System.IO.StringWriter(sb);

			RazorView view_ = new RazorView(ControllerContext, DefaultAction, null, false, null);
			view_.Render(new ViewContext(ControllerContext, view_, new ViewDataDictionary(), new TempDataDictionary(), tw), tw);

			Cache = sb.ToString();

			return Cache;
		}

		public string RenderRazorViewToString(string viewName, object model)
		{
			ViewData.Model = model;
			using (var sw = new StringWriter())
			{
				var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
				var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
				viewResult.View.Render(viewContext, sw);
				return sw.GetStringBuilder().ToString();
			}
		}

		public static string RenderPartialToString(ControllerContext context, string partialViewName, ViewDataDictionary viewData, TempDataDictionary tempData)
		{
			ViewEngineResult result = ViewEngines.Engines.FindPartialView(context, partialViewName);

			if (result.View != null)
			{
				StringBuilder sb = new StringBuilder();

				StringWriter sw = new StringWriter(sb);
				using (HtmlTextWriter output = new HtmlTextWriter(sw))
				{
					ViewContext viewContext = new ViewContext(context, result.View, viewData, tempData, output);
					result.View.Render(viewContext, output);
				}

				return sb.ToString();
			}

			return string.Empty;
		}
		#endregion

		public FileResult GetFileResult(string filename, string mimeType)
		{
			return File(filename, mimeType);
		}
		public FileResult GetPdf(string filename)
		{
			if(!System.IO.File.Exists(filename))
				throw new Exception("Unable to retrieve PDF: " + filename + ". The file could not be found.");

			return GetFileResult(filename, System.Net.Mime.MediaTypeNames.Application.Pdf);
		}

		public List<int> GetCheckBoxInts(string ids)
		{
			List<string> chkboxStrings = GetCheckBoxStrings(ids);

			List<int> chkboxInts = new List<int>();
			foreach (var str in chkboxStrings)
				chkboxInts.Add(str.ToInteger());

			return chkboxInts;
		}
		public List<string> GetCheckBoxStrings(string ids)
		{
			ids = (ids ?? string.Empty).Trim().TrimStart('[').TrimEnd(']').Trim();
			List<string> chkboxids = ids.ToStringList("\"");

			return chkboxids;
		}

		protected void WriteRequestAndReferrerInfo()
		{
			string msg = string.Empty;
			int messageNum = 0;

			if (null != Request.UserLanguages && Request.UserLanguages.Count() > 0)
			{
				msg = string.Empty;
				messageNum = 0;

				foreach (var seg in Request.UserLanguages)
				{
					if (!string.IsNullOrEmpty(msg))
						msg += ", ";

					msg += string.Format("{0}: {1}", messageNum, seg[messageNum]);
					++messageNum;
				}
			}

			TraceInformation("Default", "UserHostAddress: " + (Request.UserHostAddress ?? string.Empty)
				+ ", UserHostName: " + (Request.UserHostName ?? string.Empty)
				+ ", UserLanguages: " + (msg ?? string.Empty)
				+ ", UserAgent: " + (Request.UserAgent ?? string.Empty)
			);

			if (null != Request.UrlReferrer)
			{
				if (null != Request.UrlReferrer.Segments && Request.UrlReferrer.Segments.Count() > 0)
				{
					msg = string.Empty;
					messageNum = 0;

					foreach (var seg in Request.UrlReferrer.Segments)
					{
						if (!string.IsNullOrEmpty(msg))
							msg += ", ";

						msg += string.Format("{0}: {1}", messageNum, seg[messageNum]);
						++messageNum;
					}
				}

				TraceInformation("Default", "URL Referrer: "
					+ ", AbsolutePath: " + (Request.UrlReferrer.AbsolutePath ?? string.Empty)
					+ ", AbsoluteUri: " + (Request.UrlReferrer.AbsoluteUri ?? string.Empty)
					+ ", Authority: " + (Request.UrlReferrer.Authority ?? string.Empty)
					+ ", DnsSafeHost: " + (Request.UrlReferrer.DnsSafeHost ?? string.Empty)
					+ ", Fragment: " + (Request.UrlReferrer.Fragment ?? string.Empty)
					+ ", Host: " + (Request.UrlReferrer.Host ?? string.Empty)
					+ ", HostNameType: " + (Request.UrlReferrer.HostNameType.ToString() ?? string.Empty)
					+ ", LocalPath: " + (Request.UrlReferrer.LocalPath ?? string.Empty)
					+ ", OriginalString: " + (Request.UrlReferrer.OriginalString ?? string.Empty)
					+ ", PathAndQuery: " + (Request.UrlReferrer.PathAndQuery ?? string.Empty)
					+ ", Port: " + (Request.UrlReferrer.Port.ToString() ?? string.Empty)
					+ ", Query: " + (Request.UrlReferrer.Query ?? string.Empty)
					+ ", Scheme: " + (Request.UrlReferrer.Scheme ?? string.Empty)
					+ ", Segments: " + msg
					+ ", UserInfo: " + (Request.UrlReferrer.UserInfo ?? string.Empty)
					);
			}

			if (null != Request.Url)
			{
				if (null != Request.Url.Segments && Request.Url.Segments.Count() > 0)
				{
					msg = string.Empty;
					messageNum = 0;

					foreach (var seg in Request.Url.Segments)
					{
						if (!string.IsNullOrEmpty(msg))
							msg += ", ";

						msg += string.Format("{0}: {1}", messageNum, seg[messageNum]);
						++messageNum;
					}
				}

				TraceInformation("Default", "Raw URL: " + Request.RawUrl
					+ ", AbsolutePath: " + (Request.Url.AbsolutePath ?? string.Empty)
					+ ", AbsoluteUri: " + (Request.Url.AbsoluteUri ?? string.Empty)
					+ ", Authority: " + (Request.Url.Authority ?? string.Empty)
					+ ", DnsSafeHost: " + (Request.Url.DnsSafeHost ?? string.Empty)
					+ ", Fragment: " + (Request.Url.Fragment ?? string.Empty)
					+ ", Host: " + (Request.Url.Host ?? string.Empty)
					+ ", HostNameType: " + (Request.Url.HostNameType.ToString() ?? string.Empty)
					+ ", LocalPath: " + (Request.Url.LocalPath ?? string.Empty)
					+ ", OriginalString: " + (Request.Url.OriginalString ?? string.Empty)
					+ ", PathAndQuery: " + (Request.Url.PathAndQuery ?? string.Empty)
					+ ", Port: " + (Request.Url.Port.ToString() ?? string.Empty)
					+ ", Query: " + (Request.Url.Query ?? string.Empty)
					+ ", Scheme: " + (Request.Url.Scheme ?? string.Empty)
					+ ", Segments: " + msg
					+ ", UserInfo: " + (Request.Url.UserInfo ?? string.Empty)
					);
			}
		}

		protected void EndUserSession(string redirectUrl = null, bool addReturnUrl = true, bool expireDotNetCookies = false, bool endResponse = false)
		{
			// Expire all Session Keys
			if(null != Session)
				Session.Abandon();

			if(null != HttpContext && null != HttpContext.User)
				HttpContext.User = null;

			// Expire all Cookies
			if (expireDotNetCookies)
			{
				int cookieCount = 0;
				if (null != Request && null != Request.Cookies)
					cookieCount = Request.Cookies.Count; //Get the number of cookies and use that as the limit

				//Loop through the cookies
				for (int i = 0; i < cookieCount; i++)
				{
					string cookieName = Request.Cookies[i].Name;    //get the name of the current cookie
					if ("ASP.NET_SessionId" == cookieName
						//|| AntiForgeryConfig.CookieName == cookieName
						|| cookieName.Contains("AspNet")
						|| cookieName.Contains("Asp.Net"))
					{
						HttpCookie aCookie = new HttpCookie(cookieName);    //create a new cookie with the same name as the one you're deleting
						aCookie.Value = "";    //set a blank value to the cookie
						aCookie.Expires = DateTime.Now.AddDays(-1);    //Setting the expiration date in the past deletes the cookie

						Response.Cookies.Add(aCookie);    //Set the cookie
					}
				}
			}

			if(!string.IsNullOrEmpty(redirectUrl))
			{
				if(addReturnUrl && !Request.Url.AbsolutePath.Contains("/Login"))
					redirectUrl += "?returnUrl=" + System.Web.HttpUtility.HtmlEncode(Request.Url.AbsolutePath);

				Response.Redirect(redirectUrl, endResponse);
			}
		}

		protected string GetCookie(string cookieName, string key)
		{
			var cookie = Request.Cookies[cookieName];
			return cookie != null ? HttpUtility.UrlDecode(cookie[key]) : "";
		}
		protected void SetCookie(string cookieName, List<StringString> strs)
		{
			var cookie = new HttpCookie(cookieName);
			foreach (var item in strs)
				cookie[item.stringValue] = HttpUtility.UrlEncode(item.string2Value);

			Response.Cookies.Add(cookie);
		}
		protected ActionParams GetActionParamsFromGridCookie()
		{
			string apindex = GetCookie("gridCookie", "sidx");
			string aporder = GetCookie("gridCookie", "sord");
			int aprows = GetCookie("gridCookie", "rows").ToInteger();

			return new ActionParams(apindex, aporder, aprows);
		}

		protected string GetRequestString()
		{
			Stream req = Request.InputStream;
			req.Seek(0, System.IO.SeekOrigin.Begin);

			return new StreamReader(req).ReadToEnd();
		}
	}

	public class RequiresHTTP : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			HttpRequestBase req = filterContext.HttpContext.Request;
			HttpResponseBase res = filterContext.HttpContext.Response;

			//Check if we're secure or not and if we're on the local box
			if (req.IsSecureConnection && !req.IsLocal)
			{
				var builder = new UriBuilder(req.Url)
				{
					Scheme = Uri.UriSchemeHttp,
					Port = 80
				};

				res.Redirect(builder.Uri.ToString());
			}

			base.OnActionExecuting(filterContext);
		}
	}

	public class RequiresSSL : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			HttpRequestBase req = filterContext.HttpContext.Request;
			HttpResponseBase res = filterContext.HttpContext.Response;

			//Check if we're secure or not and if we're on the local box
			if (!req.IsSecureConnection && !req.IsLocal)
			{
				var builder = new UriBuilder(req.Url)
				{
					Scheme = Uri.UriSchemeHttps,
					Port = 443
				};

				res.RedirectPermanent(builder.Uri.ToString());
			}

			base.OnActionExecuting(filterContext);
		}
	}

	/// <summary>
	/// Used to send the Anti-forgery token in the HTTP Header.
	/// This allows us to send application/json requests without interference.
	/// Be sure to add it to your $.ajax call in -- headers: site.addAntiForgeryToken(this.jfrm)
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class ValidateHeaderAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
	{
		public void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext == null)
			{
				throw new ArgumentNullException("filterContext");
			}

			var httpContext = filterContext.HttpContext;
			var cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
			AntiForgery.Validate(cookie != null ? cookie.Value : null, httpContext.Request.Headers["__RequestVerificationToken"]);
		}
	}
}
