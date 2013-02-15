using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using skky.util;
using skky.web;

namespace skkyWeb.util
{
	public class HttpRequestor
	{
		/// <summary>
		/// Struct for file downloads that contains and describes
		/// the file that was downloaded.
		/// </summary>
		public struct FileDownloadSpec
		{
			public string ContentType;
			public string Extension;
			public MemoryStream File;
		}

		/// <summary>Browser type constants.</summary>
		private const string Browser = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.0.04506;)";

		/// <summary>The number of time to keep trying the request, before failing.</summary>
		private const int RequestRetryAttempts = 3;

		/// <summary>Number of milliseconds between each failed call.</summary>
		private const int TimeBetweenRetries = 250;

		public HttpRequestOptions HttpOptions { get; set; }
		public HttpAuthenticationOptions AuthHelper { get; set; }

		public HttpRequestor(HttpRequestOptions opts)
			: this(opts, null)
		{ }
		public HttpRequestor(HttpRequestOptions opts, HttpAuthenticationOptions auth)
		{
			HttpOptions = opts;
			AuthHelper = auth;
		}
		public HttpRequestor(string url)
			: this(new HttpRequestOptions()
			{
				Url = url,
				ContentType = HttpRequestOptions.Const_ContentTypeFormUrlEncoded,
				Method = HttpRequestOptions.Const_Get,
			})
		{ }
		public HttpRequestor(string url, string postData)
			: this(new HttpRequestOptions()
			{
				Url = url,
				ContentType = HttpRequestOptions.Const_ContentTypeFormUrlEncoded,
				Method = HttpRequestOptions.Const_Post,
				PostData = postData,
			})
		{ }

		static public Uri GetURI(string uri)
		{
			return new Uri(uri);
		}

		private HttpWebResponse GetAuthenticationResponse()
		{
			if (AuthHelper == null)
				AuthHelper = HttpAuthenticationOptions.GetDefault();

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(HttpOptions.Uri);
			request.Method = HttpOptions.Method;
			request.KeepAlive = true;
			request.Accept = @"*/*";

			request.Credentials = AuthHelper.GetCredentialCache();

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			return response;
		}

		#region GetWebRequest
		/// <summary>
		/// Initializes a new HttpWebRequest based on the passed in arguments.
		/// </summary>
		/// <param name="uri">The uri that identifies the internet resource.</param>
		/// <param name="method">The http method for the request.</param>
		/// <param name="connectionGroup">The connection group name for the request.</param>
		/// <param name="authenticationType">The type of authentication for the request..</param>
		/// <param name="userName">The username associated with the credentials.</param>
		/// <param name="password">The password of the username associated with the credentials.</param>
		/// <param name="cookies">The cookie container for the request.</param>
		/// <returns>An initialized HttpWebRequest.</returns>
		private HttpWebRequest MakeWebRequest()
		{
			// Initiate a new WebRequest to the given URI.
			HttpWebRequest webRequest = WebRequest.Create(HttpOptions.Uri) as HttpWebRequest;
			if (webRequest == null)
				return null;

			if (AuthHelper != null)
				webRequest.Credentials = AuthHelper.GetCredentials();

			if (HttpOptions.Cookies != null)
				webRequest.CookieContainer = HttpOptions.Cookies;

			webRequest.Method = HttpOptions.Method;
			webRequest.UnsafeAuthenticatedConnectionSharing = true;
			//webRequest.PreAuthenticate = true;
			webRequest.KeepAlive = true;

			//webRequest.Pipelined = true;
			webRequest.UserAgent = Browser;
			webRequest.ProtocolVersion = HttpVersion.Version11;
			webRequest.ContentType = HttpOptions.ContentType;

			if (HttpOptions.Method == HttpRequestOptions.Const_Post)
				StreamHelper.WriteStringEncoded(webRequest.GetRequestStream(), HttpOptions.PostData, Encoding.ASCII.CodePage, true);

			return webRequest;
		}

		private string GetResponse(HttpWebRequest request)
		{
			return GetEncodedResponse(request, 0);	// Use default encoding.
		}
		private string GetResponse(HttpWebResponse wr)
		{
			return GetEncodedResponse(wr, 0);	// Use default encoding.
		}
		private string GetEncodedResponse(HttpWebRequest request, int encoding)
		{
			if (request == null)
				return string.Empty;

			HttpWebResponse wr = (HttpWebResponse)request.GetResponse();
			return GetEncodedResponse(wr, encoding);
		}
		private static string GetEncodedResponse(WebResponse wr, int encoding)
		{
			if (wr == null)
				return string.Empty;

			return StreamHelper.GetString(wr.GetResponseStream(), encoding, false);
		}
		public string RetrieveString()
		{
			HttpWebRequest wr = MakeWebRequest();
			HttpWebResponse response = wr.GetResponse() as HttpWebResponse;

			return GetEncodedResponse(response, 0);
		}

		#endregion

		#region AddCertToRequest
		/// <summary>
		/// Load the given client certificate file and adds the certificate to the current http web request.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="clientCertFile">The client certificate file to use for the request.</param>
		/// <returns></returns>
		private static void AddCertToRequest(HttpWebRequest request, string clientCertFile)
		{
			if (!string.IsNullOrEmpty(clientCertFile))
			{
				X509Certificate cert = X509Certificate.CreateFromCertFile(clientCertFile);
				X509CertificateCollection x509 = request.ClientCertificates;
				x509.Add(cert);
				request.ClientCertificates.Add(cert);
			}
		}
		#endregion

		#region GetRequestDataArray
		/// <summary>
		/// Sends a get request to the specified url, and adds each line of the response into
		/// an ArrayList.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="clientCert"></param>
		/// <returns></returns>
		public static ArrayList GetRequestDataArray(string url, string clientCert)
		{
			ArrayList data = new ArrayList();

			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			//request.Credentials = CredentialCache.DefaultCredentials;
			request.Method = HttpRequestOptions.Const_Get;
			request.KeepAlive = false;

			// Add the client certificate
			if (!string.IsNullOrEmpty(clientCert))
				AddCertToRequest(request, clientCert);

			// Get the response from the server.
			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			{
				StreamReader sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.ASCII);
				string line = string.Empty;
				// Put each line of the response stream into an arraylist.
				while ((line = sr.ReadLine()) != null)
				{
					data.Add(line);
				}
				sr.Close();
			}

			return data;
		}
		#endregion

		#region SendGetRequest
		/// <summary>
		/// Sends a get request and returns the response as a string.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="clientCert"></param>
		/// <param name="cookies"></param>
		/// <returns></returns>
		public Stream SendGetRequest(string url, string clientCert, CookieContainer cookies)
		{
			return SendGetRequest(url, clientCert, cookies, int.MinValue);
		}

		/// <summary>
		/// Sends a get request and returns the response as a string.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="clientCert"></param>
		/// <param name="cookies"></param>
		/// <param name="readWriteTimeout">Number of milliseconds before a read or write operation times out.</param>
		/// <returns></returns>
		public Stream SendGetRequest(string url, string clientCert, CookieContainer cookies, int readWriteTimeout)
		{
			for (int i = 1; i < RequestRetryAttempts; i++)
			{
				try
				{
					return SendHttpGetRequest(url, clientCert, cookies);
				}
				// Loop if an exception occurred other than SoapException occurred. SoapExceptions
				// should be ignored because we do not want to make calls to the server that have
				// already run.
				catch (WebException)
				{
					// ignore
				}
				catch (Exception)
				{
					//ignore
				}

				// Wait for a quarter of a second before retrying.
				System.Threading.Thread.Sleep(TimeBetweenRetries);
			}

			return SendHttpGetRequest(url, clientCert, cookies);
		}
		#endregion

		#region SendHttpGetBinaryRequest
		/// <summary>
		/// Downloads a file from an http location.
		/// </summary>
		/// <param name="url">The url of the file to download.</param>
		/// <param name="clientCert">The client certificate if the site requires client certifcate authentication.</param>
		/// <param name="cookies">The cookie container to use for the requst.</param>
		/// <returns></returns>
		public static FileDownloadSpec SendHttpGetBinaryRequest(string url, string clientCert, CookieContainer cookies)
		{
			FileDownloadSpec fds = new FileDownloadSpec();

			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			//request.Credentials = CredentialCache.DefaultCredentials;
			request.Method = HttpRequestOptions.Const_Get;
			request.KeepAlive = false;
			request.ProtocolVersion = new System.Version("1.0");
			request.AllowAutoRedirect = true;
			request.UserAgent = Browser;

			// Send the GET method request and get the response from the server.
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			// Display the item's stream content type and length.
			Debug.WriteLine("Content type: " + response.ContentType);
			fds.ContentType = response.ContentType;
			Debug.WriteLine(fds.ContentType.Substring(fds.ContentType.IndexOf("/")));
			fds.Extension = "." + fds.ContentType.Substring(fds.ContentType.IndexOf("/") + 1);
			Debug.WriteLine(fds.Extension);
			Debug.WriteLine("Content length: " + response.ContentLength);

			// Get the datastream from the internet resource.
			fds.File = GetMemoryStream(response, true);

			return fds;
		}

		private static MemoryStream GetMemoryStream(HttpWebResponse response, bool closeStream)
		{
			return StreamHelper.GetMemoryStream(response.GetResponseStream(), closeStream);
		}

		#endregion

		#region SendHttpGetRequest
		/// <summary>
		/// Sends a get request and returns the response as a string.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="clientCert"></param>
		/// <param name="cookies"></param>
		/// <returns></returns>
		public Stream SendHttpGetRequest(string url, string clientCert, CookieContainer cookies)
		{
			return SendHttpGetRequest(url, clientCert, cookies, int.MinValue);
		}

		private static void TraceHttpWebResponse(HttpWebResponse response)
		{
			skky.util.Trace.Information("Response Code = {0}/{1}", (int)response.StatusCode, response.StatusCode);
			skky.util.Trace.Information("Result bytes = {0}", response.ContentLength);
			skky.util.Trace.Information("Cookie Count = {0}", response.Cookies.Count);
			if (skky.util.Trace.IsVerboseEnabled)
			{
				foreach (Cookie cookie in response.Cookies)
				{
					skky.util.Trace.Verbose("Response.Cookies[\"{0}\"] = \"{1}\" Path = {2}", cookie.Name, cookie.Value, cookie.Path);
				}
			}
		}

		/// <summary>
		/// Sends a get request and returns the response as a string.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="clientCert"></param>
		/// <param name="cookies"></param>
		/// <param name="readWriteTimeout">Number of milliseconds before a read or write operation times out.</param>
		/// <returns></returns>
		public Stream SendHttpGetRequest(string url, string clientCert, CookieContainer cookies, int readWriteTimeout)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

			//request.Credentials = CredentialCache.DefaultCredentials;
			request.Method = HttpRequestOptions.Const_Get;
			request.KeepAlive = false;
			request.ProtocolVersion = new System.Version("1.0");
			request.AllowAutoRedirect = true;
			request.UserAgent = Browser;

			if (readWriteTimeout > 0)
				request.ReadWriteTimeout = readWriteTimeout;

			//add the cookies
			if (cookies != null)
				request.CookieContainer = cookies;

			// Add the client certificate
			if (!string.IsNullOrEmpty(clientCert))
				AddCertToRequest(request, clientCert);

			//Trace.Information("Making {0} {1} Request to: {2}", request.RequestUri.Scheme, request.Method, request.RequestUri);

			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			{
				long len = response.ContentLength;
				return StreamHelper.GetMemoryStreamFlush(response.GetResponseStream(), (int)len);

				//TraceHttpWebResponse(response);
			}
			//catch (WebException e)
			//{
			//    // Check to see if the remote host return a response
			//    if (e.Response != null)
			//    {
			//        StreamReader sr = new StreamReader(e.Response.GetResponseStream(), System.Text.Encoding.ASCII);
			//        Debug.WriteLine(sr.ReadToEnd());
			//        e.Response.Close();
			//    }

			//    throw new Exception("Error accessing Url " + url + " Error: " + e.Message, e);
			//}
		}
		#endregion

		#region SendPostRequest
		/// <summary>
		/// Sends HTTP post request to the specified url posting the data. Returns
		/// the response from the server. Will retry up to 3 times if the request fails.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="clientCert"></param>
		/// <param name="data"></param>
		/// <param name="cookies"></param>
		/// <returns></returns>
		public static Stream SendPostRequest(string url, string clientCert, string data, CookieContainer cookies)
		{
			return SendPostRequest(url, clientCert, data, string.Empty, cookies);
		}

		/// <summary>
		/// Sends HTTP post request to the specified url posting the data. Returns
		/// the response from the server. Will retry up to 3 times if the request fails.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="clientCert"></param>
		/// <param name="data"></param>
		/// <param name="referrer"></param>
		/// <param name="cookies"></param>
		/// <returns></returns>
		public static Stream SendPostRequest(string url, string clientCert, string data, string referrer, CookieContainer cookies)
		{
			for (int i = 1; i < RequestRetryAttempts; i++)
			{
				try
				{
					return SendHttpPostRequest(url, clientCert, data, referrer, cookies);
				}
				// Loop if an exception occurred other than SoapException occurred. SoapExceptions
				// should be ignored because we do not want to make calls to the server that have
				// already run.
				catch (WebException)
				{
					// ignore
				}
				catch (Exception)
				{
					//ignore
				}

				// Wait for a quarter of a second before retrying.
				System.Threading.Thread.Sleep(TimeBetweenRetries);
			}

			return SendHttpPostRequest(url, clientCert, data, referrer, cookies);
		}
		#endregion

		#region SendHttpPostRequest
		/// <summary>
		/// Sends HTTP post request to the specified url posting the data. Returns
		/// the response from the server.
		/// </summary>
		/// <param name="url">Url to post the data.</param>
		/// <param name="clientCert"></param>
		/// <param name="data">Name value pairs of data to post.</param>
		/// <param name="referrer"></param>
		/// <param name="cookies"></param>
		/// <returns>Server response to post.</returns>
		public static Stream SendHttpPostRequest(string url, string clientCert, string contentType, string data, string referrer, CookieContainer cookies)
		{
			MemoryStream result = new MemoryStream();
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

			request.Method = "POST";
			//request.KeepAlive = true;
			request.ProtocolVersion = new System.Version("1.0");
			request.AllowAutoRedirect = true;
			request.UserAgent = Browser;

			request.ContentType = contentType;
			request.ContentLength = data.Length;
			request.Referer = referrer;

			//add the cookies
			if (cookies != null)
				request.CookieContainer = cookies;

			// Add the client certificate
			if (!string.IsNullOrEmpty(clientCert))
				AddCertToRequest(request, clientCert);

			//Trace.Information("Making {0} {1} Request to: {2}", request.RequestUri.Scheme, request.Method, request.RequestUri);

			HttpWebResponse response = null;

			try
			{
				// Get a streamwriter from the request stream to send the form data.
				StreamWriter w = new StreamWriter(request.GetRequestStream());
				w.Write(data);
				w.Close();

				// Get the response from the server.
				response = (HttpWebResponse)request.GetResponse();

				// copy the stream into memory
				w = new StreamWriter(result);
				StreamReader reader = new StreamReader(response.GetResponseStream());
				w.Write(reader.ReadToEnd());
				w.Flush();
				result.Position = 0; // rewind stream for reading

				//TraceHttpWebResponse(response);
			}
			catch (WebException e)
			{
				// Check to see if the remote host return a response
				if (e.Response != null)
					e.Response.Close();

				throw new Exception("Error accessing Url " + url + " Error: " + e.Message, e);
			}
			finally
			{
				if (response != null)
					response.Close();
			}

			return result;
		}


		/// <summary>
		/// Sends HTTP post request to the specified url posting the data. Returns
		/// the response from the server.
		/// </summary>
		/// <param name="url">Url to post the data.</param>
		/// <param name="clientCert"></param>
		/// <param name="data">Name value pairs of data to post.</param>
		/// <param name="referrer"></param>
		/// <param name="cookies"></param>
		/// <returns>Server response to post.</returns>
		public static Stream SendHttpPostRequest(string url, string clientCert, string data, string referrer, CookieContainer cookies)
		{
			return SendHttpPostRequest(url, clientCert, "application/x-www-form-urlencoded", data, referrer, cookies);
		}

		#endregion

		private static string ExtractViewState(string s)
		{
			string viewStateNameDelimiter = "__VIEWSTATE";
			string valueDelimiter = "value=\"";

			int viewStateNamePosition = s.IndexOf(viewStateNameDelimiter);
			int viewStateValuePosition = s.IndexOf(
				  valueDelimiter, viewStateNamePosition
			   );

			int viewStateStartPosition = viewStateValuePosition +
										 valueDelimiter.Length;
			int viewStateEndPosition = s.IndexOf("\"", viewStateStartPosition);

			return HttpUtility.UrlEncodeUnicode(
					 s.Substring(
						viewStateStartPosition,
						viewStateEndPosition - viewStateStartPosition
					 )
				  );
		}

		public CookieContainer FormsAuthenticate(HttpRequestOptions hro)
		{
			Uri originalUri = HttpOptions.Uri;

			System.Diagnostics.ConsoleTraceListener trace =
					new System.Diagnostics.ConsoleTraceListener();

			//
			// Request page protected by forms authentication.
			// This request will get a 302 to login page
			//
			trace.Write("Requesting : " + originalUri);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(originalUri);
			request.AllowAutoRedirect = false;

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			if (response.StatusCode == HttpStatusCode.Found)
			{
				trace.Write("Response: 302 ");
				trace.WriteLine(response.StatusCode);
			}
			else
			{
				trace.Fail("Response status is " + response.StatusCode + ". Expected was Found");
			}

			//
			// Get the url of login page from location header
			//
			string locationHeader = response.Headers[HttpResponseHeader.Location];
			string cookieReturned = response.Headers[HttpResponseHeader.SetCookie];
			//string locationHeader = response.GetResponseHeader("Location");
			trace.WriteLine("Location header is " + locationHeader);
			trace.WriteLine("");

			//
			// Request login page
			//
			string loginPageUrl = "http://localhost" + locationHeader;
			Console.WriteLine("Requesting " + loginPageUrl);
			request = (HttpWebRequest)WebRequest.Create(loginPageUrl);
			request.AllowAutoRedirect = false;

			response = (HttpWebResponse)request.GetResponse();
			if (response.StatusCode == HttpStatusCode.OK)
			{
				trace.Write("Response: 200 ");
				trace.WriteLine(response.StatusCode);
			}
			else
			{
				trace.Fail("Response status is " + response.StatusCode + ". Expected was OK");
			}

			trace.WriteLine("Parsing login page to create post message");
			trace.WriteLine("");

			string loginResponse = StreamHelper.GetString(response.GetResponseStream());

			string eventTargetVar = "__EVENTTARGET=";
			string eventTargetValue = "";

			string eventArgumentVar = "__EVENTARGUMENT=";
			string eventArgumentValue = "";

			string viewStateVar = "__VIEWSTATE=";
			string viewStateSearchString = "name=\"__VIEWSTATE\" id=\"__VIEWSTATE\" value=\"";
			int viewStateStartIndex = loginResponse.IndexOf(viewStateSearchString);
			loginResponse = loginResponse.Substring(viewStateStartIndex + viewStateSearchString.Length);
			string viewStateValue = Uri.EscapeDataString(
														   loginResponse.Substring(0, loginResponse.IndexOf("\" />"))
													   );
			loginResponse = loginResponse.Substring(loginResponse.IndexOf("\" />"));

			string eventValidationVar = "__EVENTVALIDATION=";
			string eventValSearchString =
				"name=\"__EVENTVALIDATION\" id=\"__EVENTVALIDATION\" value=\"";
			int eventValStartIndex = loginResponse.IndexOf(eventValSearchString);
			loginResponse = loginResponse.Substring(eventValStartIndex + eventValSearchString.Length);
			string eventValidationValue =
				Uri.EscapeDataString(
					loginResponse.Substring(0, loginResponse.IndexOf("\" />"))
				);

			//
			// Look for logon control id
			// Use any valid username and password
			//
			string lcSearchStr = "input name=";
			int lcSearchIndex = loginResponse.IndexOf(lcSearchStr);
			loginResponse = loginResponse.Substring(lcSearchIndex + lcSearchStr.Length + 1);
			string userNameVar = Uri.EscapeDataString(
													   loginResponse.Substring(0, loginResponse.IndexOf("\""))
												   ) + "=";
			string userNameValue = AuthHelper.UserName;

			lcSearchIndex = loginResponse.IndexOf(lcSearchStr);
			loginResponse = loginResponse.Substring(lcSearchIndex + lcSearchStr.Length + 1);
			string passwordVar = Uri.EscapeDataString(
													   loginResponse.Substring(0, loginResponse.IndexOf("\""))
													) + "=";
			string passwordValue = AuthHelper.Password;

			lcSearchStr = "type=\"submit\" name=";
			lcSearchIndex = loginResponse.IndexOf(lcSearchStr);
			loginResponse = loginResponse.Substring(lcSearchIndex + lcSearchStr.Length + 1);
			string logOnButtonVar = Uri.EscapeDataString(
														   loginResponse.Substring(0, loginResponse.IndexOf("\""))
													   ) + "=";
			//String logOnButtonValue = "Log+In";

			string postString = eventTargetVar + eventTargetValue;
			postString += "&" + eventArgumentVar + eventArgumentValue;
			postString += "&" + viewStateVar + viewStateValue;
			postString += "&" + userNameVar + userNameValue;
			postString += "&" + passwordVar + passwordValue;
			postString += "&" + logOnButtonVar + AuthHelper.LogOnButtonValue;
			postString += "&" + eventValidationVar + eventValidationValue;

			//
			// Do a POST to login.aspx now
			// This should result in 302 with Set-Cookie header
			//
			Console.WriteLine("POST request to http://localhost" + locationHeader);
			request = (HttpWebRequest)WebRequest.Create("http://localhost" + locationHeader);
			request.AllowAutoRedirect = false;
			request.Method = HttpRequestOptions.Const_Post;
			request.ContentType = HttpRequestOptions.Const_ContentTypeFormUrlEncoded;

			CookieContainer cookies = new CookieContainer();
			request.CookieContainer = cookies;

			byte[] requestData = postString.EncodeAscii();
			request.ContentLength = requestData.Length;

			Stream requestStream = request.GetRequestStream();
			requestStream.Write(requestData, 0, requestData.Length);

			response = (HttpWebResponse)request.GetResponse();
			if (response.StatusCode == HttpStatusCode.Found)
			{
				trace.Write("Response: 302 ");
				trace.WriteLine(response.StatusCode);
			}
			else
			{
				trace.Fail("Response status is " + response.StatusCode + ". Expected was Found");
			}

			locationHeader = response.Headers[HttpResponseHeader.Location];
			trace.WriteLine("Location header is " + locationHeader);
			//cookieReturned = response.Headers[HttpResponseHeader.SetCookie];
			//ap.CookieReturned = response.GetResponseHeader("Set-Cookie");
			//trace.WriteLine("Set-Cookie header is " + cookie);
			trace.WriteLine("");

			cookies.Add(response.Cookies);

			return cookies;
		}

		static public string AddFirstPostParameter(string name, string value, bool escapeName)
		{
			return (escapeName ? Uri.EscapeDataString(name) : name) + "=" + Uri.EscapeDataString(value);
		}
		static public string AddFirstPostParameter(string name, string value)
		{
			return AddFirstPostParameter(name, value, true);
		}
		static public string AddPostParameter(string name, string value, bool escapeName)
		{
			return "&" + AddFirstPostParameter(name, value, escapeName);
		}
		static public string AddPostParameter(string name, string value)
		{
			return AddPostParameter(name, value, true);
		}

		public string LogOn()
		{
			Console.WriteLine("Requesting LogOn: " + AuthHelper.URI + ".");
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AuthHelper.URI);
			request.AllowAutoRedirect = false;

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			if (response.StatusCode == HttpStatusCode.OK)
			{
				Console.WriteLine("Response: 200 ");
				Console.WriteLine(response.StatusCode);
			}
			else
			{
				Console.WriteLine("Response status is " + response.StatusCode + ". Expected was OK");
			}

			string locationHeader = response.GetResponseHeader("Location");
			string loginResponse = StreamHelper.GetString(response.GetResponseStream());

			string eventTargetVar = "__EVENTTARGET=";
			string eventTargetValue = "";

			string eventArgumentVar = "__EVENTARGUMENT=";
			string eventArgumentValue = "";

			string viewStateVar = "__VIEWSTATE=";
			string viewStateSearchString = "name=\"__VIEWSTATE\" id=\"__VIEWSTATE\" value=\"";
			int viewStateStartIndex = loginResponse.IndexOf(viewStateSearchString);
			loginResponse = loginResponse.Substring(viewStateStartIndex + viewStateSearchString.Length);
			string viewStateValue = Uri.EscapeDataString(
														   loginResponse.Substring(0, loginResponse.IndexOf("\" />"))
													   );
			loginResponse = loginResponse.Substring(loginResponse.IndexOf("\" />"));

			string eventValidationVar = "__EVENTVALIDATION=";
			string eventValSearchString =
				"name=\"__EVENTVALIDATION\" id=\"__EVENTVALIDATION\" value=\"";
			int eventValStartIndex = loginResponse.IndexOf(eventValSearchString);
			loginResponse = loginResponse.Substring(eventValStartIndex + eventValSearchString.Length);
			string eventValidationValue =
				Uri.EscapeDataString(
					loginResponse.Substring(0, loginResponse.IndexOf("\" />"))
				);

			string lcSearchStr = "input name=";
			int lcSearchIndex = 0;

			//
			// Look for logon control id
			// Use any valid username and password
			//
			lcSearchIndex = loginResponse.IndexOf(lcSearchStr);
			loginResponse = loginResponse.Substring(lcSearchIndex + lcSearchStr.Length + 1);
			string userNameVar = Uri.EscapeDataString(
													   loginResponse.Substring(0, loginResponse.IndexOf("\""))
												   ) + "=";
			string userNameValue = AuthHelper.UserName;

			lcSearchIndex = loginResponse.IndexOf(lcSearchStr);
			loginResponse = loginResponse.Substring(lcSearchIndex + lcSearchStr.Length + 1);
			string passwordVar = Uri.EscapeDataString(
													   loginResponse.Substring(0, loginResponse.IndexOf("\""))
													) + "=";
			string passwordValue = AuthHelper.Password;

			lcSearchStr = "type=\"submit\" name=";
			lcSearchIndex = loginResponse.IndexOf(lcSearchStr);
			loginResponse = loginResponse.Substring(lcSearchIndex + lcSearchStr.Length + 1);
			string logOnButtonVar = Uri.EscapeDataString(
														   loginResponse.Substring(0, loginResponse.IndexOf("\""))
													   ) + "=";
			//String logOnButtonValue = "Log+In";

			string postString = eventTargetVar + eventTargetValue;
			postString += "&" + eventArgumentVar + eventArgumentValue;
			postString += "&" + viewStateVar + viewStateValue;
			postString += "&" + userNameVar + userNameValue;
			postString += "&" + passwordVar + passwordValue;
			postString += "&" + logOnButtonVar + AuthHelper.LogOnButtonValue;
			postString += "&" + eventValidationVar + eventValidationValue;

			//
			// Do a POST to login.aspx now
			// This should result in 302 with Set-Cookie header
			//
			//Console.WriteLine("POST request to http://localhost" + locationHeader);
			//request = (HttpWebRequest)WebRequest.Create("http://localhost" + locationHeader);
			Console.WriteLine("POST request to " + AuthHelper.URI);
			request = (HttpWebRequest)WebRequest.Create(AuthHelper.URI);
			request.AllowAutoRedirect = false;
			request.Method = HttpRequestOptions.Const_Post;
			request.ContentType = HttpRequestOptions.Const_ContentTypeFormUrlEncoded;

			if (HttpOptions.RetrieveCookies)
			{
				HttpOptions.Cookies = new CookieContainer();
				request.CookieContainer = HttpOptions.Cookies;
			}

			byte[] requestData = postString.EncodeAscii();
			request.ContentLength = requestData.Length;

			Stream requestStream = request.GetRequestStream();
			requestStream.Write(requestData, 0, requestData.Length);

			response = (HttpWebResponse)request.GetResponse();
			if (response.StatusCode == HttpStatusCode.Found)
			{
				Console.Write("Response: 302 ");
				Console.WriteLine(response.StatusCode);
			}
			else
			{
				Console.WriteLine("Response status is " + response.StatusCode + ". Expected was Found");
			}

			locationHeader = response.Headers[HttpResponseHeader.Location];
			Console.WriteLine("Location header is " + locationHeader);
			//string cookieReturned = response.Headers[HttpResponseHeader.SetCookie];
			//ap.CookieReturned = response.GetResponseHeader("Set-Cookie");
			//trace.WriteLine("Set-Cookie header is " + cookie);
			Console.WriteLine("");

			//cookies.Add(response.Cookies);

			return locationHeader;
		}
	}

	public class XMLHttpRequestor : HttpRequestor
	{
		public XMLHttpRequestor(string url)
			: base(url)
		{ }

		public string xmlResponse { get; set; }

		public T HttpGetFromXML<T>()
		{
			xmlResponse = string.Empty;
			xmlResponse = RetrieveString();
			xmlResponse = xmlResponse.Replace("\n", "");

			return XMLHelper.Deserialize<T>(xmlResponse);
		}
		public static T HttpGetFromXML<T>(string url)
		{
			XMLHttpRequestor req = new XMLHttpRequestor(url);
			return req.HttpGetFromXML<T>();
		}
	}
	public class JSONHttpRequestor : HttpRequestor
	{
		public static readonly HttpRequestOptions JSONHttpRequestOptions = new HttpRequestOptions
		{
			ContentType = HttpRequestOptions.Const_ContentTypeJSON,
			Method = HttpRequestOptions.Const_Post,
		};

		public JSONHttpRequestor(string url)
			: base(GetRequestOptions(url))
		{ }

		public static HttpRequestOptions GetRequestOptions(string url)
		{
			HttpRequestOptions hro = new HttpRequestOptions
			{
				ContentType = JSONHttpRequestOptions.ContentType,
				Method = JSONHttpRequestOptions.Method,
				RetrieveCookies = JSONHttpRequestOptions.RetrieveCookies,
			};

			hro.Url = url;

			return hro;
		}

		public string xmlResponse { get; set; }

		public T HttpGetFromJSON<T>()
		{
			xmlResponse = string.Empty;
			xmlResponse = RetrieveString();
			//xmlResponse = xmlResponse.Replace("\n", "");

			return XMLHelper.Deserialize<T>(xmlResponse);
		}
		public static T HttpGetFromJSON<T>(string url)
		{
			JSONHttpRequestor req = new JSONHttpRequestor(url);
			return req.HttpGetFromJSON<T>();
		}
	}
}
