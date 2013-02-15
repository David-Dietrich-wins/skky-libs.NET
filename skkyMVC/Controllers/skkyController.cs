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

namespace skkyMVC.Controllers
{
	public class skkyController : Controller
	{
		public const string CONST_Date4DigitYear = "MMM d, yyyy";
		public const string CONST_DateTimeLong = "MMMM dd, yyyy hh:mm:ss.ff tt";

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

			return skky.util.Trace.MethodException(className, methodName, ex, msg);
		}

		#region HTTP Status returns
		// Return Status errors and exceptions
		protected JsonResult ReturnStatusConflictIfError(ReturnStatus rs, int rc)
		{
			if (rs == null)
				return Json(rc);

			return ReturnStatusConflict(rs);
		}
		protected ActionResult ContentWithNoDataCheck(string str)
		{
			if (string.IsNullOrEmpty(str))
				return NoDataStatus();

			return Content(str);
		}
		protected JsonResult NotAcceptableStatus(ReturnStatus rs, Exception ex)
		{
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
			ReturnStatus rs = new ReturnStatus(errorMessage);
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
			ReturnStatus rs = new ReturnStatus(errorMessage);
			return ReturnStatusNotFound(rs);
		}
		protected ReturnStatus GetAddEditItemException(Exception ex, string objectName)
		{
			ReturnStatus rs = new ReturnStatus();
			rs.ErrorMessage.Add("Error editing item.");

			if (ex.Message.Contains("duplicate") || (null != ex.InnerException && ex.InnerException.Message.Contains("duplicate")))
			{
				rs.Message.Add("Items are unique.");
				rs.Message.Add("You attempted to add a " + objectName + " that already exists.");
			}
			else
			{
				if (null != ex.InnerException && !string.IsNullOrEmpty(ex.InnerException.Message))
					rs.Message.Add(ex.InnerException.Message);
				else
					rs.Message.Add(ex.Message);
			}
			rs.Message.Add("Please try again.");

			return rs;
		}
		protected ReturnStatus GetEmptyFieldError(string operation, string objectName, string fieldName)
		{
			ReturnStatus rs = new ReturnStatus();
			if (string.IsNullOrWhiteSpace(operation))
				operation = "adding";
			rs.ErrorMessage.Add("Error " + operation.ToLower() + " " + objectName + ".");

			rs.Message.Add(fieldName + " cannot be empty.");
			rs.Message.Add("Please try again.");

			return rs;
		}
		protected ReturnStatus GetNumberError(string operation, string field, string rate, string objectName)
		{
			ReturnStatus rs = new ReturnStatus();
			if (string.IsNullOrWhiteSpace(operation))
				operation = "adding";
			rs.ErrorMessage.Add("Error " + operation.ToLower() + " " + objectName + ".");

			string s = field ?? "EMPTY";
			s += " field: ";
			s += rate ?? "Rate";
			s += " is not a valid number.";

			rs.Message.Add(s);
			rs.Message.Add("Please try again.");

			return rs;
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

			string dirToSaveIn = System.IO.Path.Combine(Server.MapPath("~/App_Data"), savePath);
			return dirToSaveIn;
		}
		public static string TimeDifferenceMessage(DateTime dtStart, DateTime dtEnd)
		{
			string msg = string.Empty;
			TimeSpan ts = dtEnd - dtStart;
			if (ts.Minutes > 0)
				msg += ts.Minutes.ToString() + "m";
			if (ts.Seconds > 0 || ts.Minutes > 0)
			{
				if (!string.IsNullOrEmpty(msg))
					msg += " ";

				msg += ts.Seconds.ToString() + "s";
			}
			if (ts.Milliseconds > 0 || ts.Seconds > 0 || ts.Minutes > 0)
			{
				if (!string.IsNullOrEmpty(msg))
					msg += " ";

				msg += ts.Milliseconds.ToString() + "ms";
			}

			return msg;
		}
	}
}
