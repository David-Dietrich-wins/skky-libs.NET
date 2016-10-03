using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace skky.web
{
	public class HttpRequestOptions
	{
		public const string CONST_Get = "GET";
		public const string CONST_Post = "POST";
		public const string CONST_Put = "PUT";
		public const string CONST_Delete = "DELETE";

		public const string CONST_ContentTypeXml = "text/xml";
		public const string CONST_ContentTypeJson = "application/json";
		public const string CONST_ContentTypeJsonUtf8 = CONST_ContentTypeJson + "; charset=utf-8";
		public const string CONST_ContentTypeFormUrlEncoded = "application/x-www-form-urlencoded";

		public HttpRequestOptions()
		{
			Method = CONST_Get;
			ContentType = CONST_ContentTypeXml;
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
