using skky.web;
using System;
using System.Web;

namespace skkyWeb.util
{
	public class JsonpHttpModule : IHttpModule
	{
		public const string JSONP_CALLBACK = "jsonp_callback";

		#region IHttpModule Members
		public void Dispose()
		{ }

		public void Init(HttpApplication app)
		{
			app.BeginRequest += OnBeginRequest;
			app.ReleaseRequestState += OnReleaseRequestState;
		}
		#endregion

		bool _Apply(HttpRequest request)
		{
			return request.Url.Query.Contains(JSONP_CALLBACK);
			//if (!request.Url.AbsolutePath.Contains(".asmx")) return false;
			//if ("json" != request.QueryString.Get("format")) return false;
		}

		public void OnBeginRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;

			if (!_Apply(app.Context.Request))
				return;

			if (string.IsNullOrEmpty(app.Context.Request.ContentType))
			{
				app.Context.Request.ContentType = HttpRequestOptions.CONST_ContentTypeJsonUtf8;
			}
		}

		public void OnReleaseRequestState(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;

			if (!_Apply(app.Context.Request))
				return;

			app.Context.Response.Filter = new JsonpResponseFilter(app.Context.Response.Filter, app.Context);
		}
	}
}
