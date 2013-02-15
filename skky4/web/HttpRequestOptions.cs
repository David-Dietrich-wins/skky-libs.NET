using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace skky.web
{
	public class HttpRequestOptions
	{
		public const string Const_Get = "GET";
		public const string Const_Post = "POST";

		public const string Const_ContentTypeXml = "text/xml";
		public const string Const_ContentTypeJSON = "application/json";
		public const string Const_ContentTypeFormUrlEncoded = "application/x-www-form-urlencoded";

		public HttpRequestOptions()
		{
			Method = Const_Get;
			ContentType = Const_ContentTypeXml;
		}

		public string ContentType { get; set; }
		public string Url { get; set; }
		public System.Uri Uri
		{
			get
			{
				return new System.Uri(Url);
			}
		}
		public string Method { get; set; }
		public string PostData { get; set; }

		public string ConnectionGroup { get; set; }

		public bool RetrieveCookies { get; set; }
		public CookieContainer Cookies { get; set; }
		public string CookiesReturned { get; set; }
	}
}
