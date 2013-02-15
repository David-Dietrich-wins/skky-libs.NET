using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Drawing;
using skkyWeb.Security;
using skky.db;
using skky.Types;
using skkyWeb.Charts;
using skky.util;

namespace skkyWeb.util
{
	public class MasterPage : System.Web.UI.MasterPage
	{
		protected PortalUser GetUser()
		{
			return GetUserStatic();
		}
		protected static PortalUser GetUserStatic()
		{
			return skkyWeb.util.Page.GetUserStatic();
		}
		public UserWebSettings GetUserSettings()
		{
			return GetUser().Settings;
		}
		public Customer GetCustomer()
		{
			return GetUser().Customer;
		}

		public Client GetClient()
		{
			return GetUser().Client;
		}

		public int GetCustomerID()
		{
			return GetCustomer().id;
		}
		public string GetCustomerName()
		{
			return GetCustomer().Name;
		}

		public string GetClientHREFLogo()
		{
			return Html.GetHREFImage(GetClient().Name, GetClient().url, GetClient().LogoPath, GetClient().LogoWidth, GetClient().LogoHeight);
		}
		public string GetCustomerHREFLogo()
		{
			return Html.GetHREFImage(GetCustomerName(), GetCustomerURL(), GetCustomerLogoPath(), GetCustomer().LogoWidth, GetCustomer().LogoHeight);
		}
		public string GetCustomerLogoPath()
		{
			return GetCustomer().LogoPath ?? string.Empty;
		}
		public string GetCustomerURL()
		{
			return GetCustomer().url ?? string.Empty;
		}
	}
}
