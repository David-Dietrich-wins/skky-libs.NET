using System;
using System.Web;
using System.Web.Security;
using System.Configuration;

namespace skkyWeb.Security
{
	public static class AuthenticationController
	{
		private const string Const_RootDomain = "skky.net";
		public const string Const_DefaultCookieName = "skky";

		public static void AuthenticationLogOff()
		{
			try
			{
				//PortalUser user = UserController.GetCurrentPortalUser();
				//LogoutAuditEvent loginSuccess = new LogoutAuditEvent("Logout Succeeded: " + user.UserName + ":" + user.CustomerName, null, 1);
				//loginSuccess.Raise();

				FormsAuthentication.SignOut();

				//** delete any skky cookie that exists
				var cookie = HttpContext.Current.Request.Cookies[Const_DefaultCookieName];
				if (cookie != null)
					HttpContext.Current.Response.Cookies[Const_DefaultCookieName].Expires = DateTime.Now.AddYears(-30);
			}
			catch
			{ }
		}

		public static bool AuthenticationLogOn(string userName, string password)
		{
			if (Membership.ValidateUser(userName, password))
			{
				//** set the authentication cookie
				AuthenticationController.SetSessionCookie(userName);

				//** delete any skky cookie that may exist
				var cookie = HttpContext.Current.Request.Cookies[Const_DefaultCookieName];
				if (cookie != null)
					HttpContext.Current.Response.Cookies[Const_DefaultCookieName].Expires = DateTime.Now.AddYears(-30);										

				//PortalUser user = UserController.FindEnabledUser(userName);

				//LoginAuditEvent loginSuccess = new LoginAuditEvent("Login Succeeded: " + user.UserName + ":" + user.CustomerName + ":" + user.IsSystemAdmin.ToString(), null, 1);
				//loginSuccess.Raise();
				return true;
			}
			//LoginAuditEvent loginFailure = new LoginAuditEvent("Login Failed: " + userName, null, 1);
			//loginFailure.Raise();
			return false;
		}

		public static HttpCookie SetSessionCookie(string userName)
		{
			FormsAuthentication.SetAuthCookie(userName, false);

			//** get the application root domain
			var root = Convert.ToString(ConfigurationManager.AppSettings["rootDomain"]);

			//** since were authenticating the user, and we need to support multiple domains, get the cookie...
			var cookie = FormsAuthentication.GetAuthCookie(userName, false);

			//** if there was a root domain given, set it as the top level
			if (!string.IsNullOrEmpty(root))
			{
				//** set its top level domain
				cookie.Domain = root;

				//** remove the cookie and add it back
				System.Web.HttpContext.Current.Response.Cookies.Remove(cookie.Name);
				System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
			}

			return cookie;
		}
	}
}
