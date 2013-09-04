using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Westwind.Tools;

namespace skky.web
{
	// Every object must be preinitialized.
	public class config : wwAppConfiguration
	{
		public string CompanyWebsiteName = "";
		public string CompanyWebsiteURL = "";
		public string CompanyWebsiteLogoURL = "";
		public string CompanyImageBaseUri = "";
		public string ApplicationUri = "";
		public string ChartsSvcUri = "";
		public string UserRoutesUri = "";
		public string DemoUserName = "demo@skky.net";
		public string DemoPassword = "skkyWins";

		//public string MembershipConnectionString = "";

			// Facebook assigned keys.
		public string APIKey = "";
		public string Secret = "";

		public string FacebookApplicationName = "";
		public string FacebookUserRouteUri = "";
		public string FacebookImageUri = "";

		public string FacebookApplicationBaseUri = "";
		public string FacebookTitleLogo = "";
		public string FacebookTitleLogoHeight = "";
		public string FacebookTitleLogoWidth = "";
		public string FacebookTitleLogoURL = "";

		public bool FacebookLookupFriendsOnInvite = true;

		public string PartNomenclature = "Part";

		public string GoogleMapKey = "";
	}
}
