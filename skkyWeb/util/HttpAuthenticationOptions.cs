using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace skkyWeb.util
{

	/// <summary>
	/// The types of authenticatio to use for web requests.
	/// </summary>
	/// 
	public enum AuthenticationType
	{
		None,
		Basic,
		Digest,
		Forms,
		Kerberos,
		NTLM,
	}

	public class HttpAuthenticationOptions
	{
		public const string Const_DefaultLoginURI = "http://localhost:3800/Login.aspx";
		public static readonly AuthenticationType Const_DefaultAuthenticationType = AuthenticationType.Basic;

		private AuthenticationType authType = AuthenticationType.None;
		public AuthenticationType AuthType
		{
			get
			{
				return authType;
			}
			set
			{
				authType = value;
			}
		}

		public string URI { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Domain { get; set; }

		//public string LogOnUrl  { get; set; }
		public string UserNameTextbox  { get; set; }
		public string PasswordTextBox  { get; set; }
		public string LogOnButtonName  { get; set; }
		public string LogOnButtonValue { get; set; }

		public HttpAuthenticationOptions()
		{ }
		public HttpAuthenticationOptions(string userName, string password)
		{
			UserName = userName;
			Password = password;
		}
		public HttpAuthenticationOptions(string uri, string userName, string password)
			: this(userName, password)
		{
			URI = uri;
			AuthType = AuthenticationType.Basic;
		}
		public HttpAuthenticationOptions(string uri, string userName, string password, string domain)
			: this(uri, userName, password)
		{
			Domain = domain;
			AuthType = AuthenticationType.Digest;
		}
		public HttpAuthenticationOptions(string uri, string userName, string password, string userNameTextbox, string passwordTextBox, string logOnButtonName, string logOnButtonValue)
			: this(uri, userName, password)
		{
			UserNameTextbox = UserNameTextbox;
			PasswordTextBox = passwordTextBox;
			LogOnButtonName = logOnButtonName;
			LogOnButtonValue = logOnButtonValue;
			AuthType = AuthenticationType.Forms;
		}

		public string GetAuthenticationString()
		{
			return GetAuthenticationString(AuthType);
		}
		public static string GetAuthenticationString(AuthenticationType at)
		{
			switch (at)
			{
				//case AuthenticationType.Basic: return "Basic";
				case AuthenticationType.Digest: return "Digest";
				case AuthenticationType.NTLM: return "NTLM";
				case AuthenticationType.Kerberos: return "Kerberos";
			}

			return "Basic";
		}

		static public NetworkCredential GetNetworkCredential(string userName, string password, string domain)
		{
			if (string.IsNullOrEmpty(userName))
				throw new Exception("Attempting to authenticate with no user name.");

			return new NetworkCredential()
			{
				UserName = userName,
				Password = password,
				Domain = domain,
			};
		}
		public NetworkCredential GetNetworkCredential()
		{
			return GetNetworkCredential(UserName, Password, Domain);
		}

		public CredentialCache GetCredentialCache()
		{
			CredentialCache cc = new CredentialCache();
			AddToCredentialCache(cc, URI, AuthType, UserName, Password, Domain);

			return cc;
		}
		static public void AddToCredentialCache(CredentialCache cc, string uri, AuthenticationType type, string userName, string password, string domain)
		{
			if (cc == null)
				throw new Exception("NULL Credential Cache");

			NetworkCredential nc = GetNetworkCredential(userName, password, domain);
			AddToCredentialCache(cc, uri, type, nc);
		}
		static public void AddToCredentialCache(CredentialCache cc, string uri, AuthenticationType type, NetworkCredential nc)
		{
			if (cc == null)
				throw new Exception("NULL Credential Cache");

			cc.Add(HttpRequestor.GetURI(uri), GetAuthenticationString(type), nc);
		}

		public ICredentials GetCredentials()
		{
			if (AuthType == AuthenticationType.None || AuthType == AuthenticationType.Forms)
				return CredentialCache.DefaultCredentials;

			NetworkCredential nc = GetNetworkCredential();
			if (AuthType == AuthenticationType.NTLM)
				return nc;

			CredentialCache cc = new CredentialCache();

			AddToCredentialCache(cc, URI, AuthType, nc);

			return cc;
		}

		static public HttpAuthenticationOptions GetDefault()
		{
			return new HttpAuthenticationOptions(Const_DefaultLoginURI, skky.app.WebConfig.DemoUserName, skky.app.WebConfig.DemoPassword);
		}
	}
}
