using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace skky.web
{
	public static class JsonRequestor
	{
		public static string RequestOrig(string url)
		{
			var request = WebRequest.Create(url);
			//request.ContentType = "application/json; charset=utf-8";
			request.Method = WebRequestMethods.Http.Post;
			//request.Accept = "application/json";

			string text;
			var response = (HttpWebResponse)request.GetResponse();

			using (var sr = new StreamReader(response.GetResponseStream()))
			{
				text = sr.ReadToEnd();
			}

			return text;
		}
		public static string Request(string url, string json)
		{
			string result = string.Empty;
			var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.ContentType = "text/json";
			httpWebRequest.Method = "POST";

			using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				//string json = "{\"user\":\"test\"," +
				//			  "\"password\":\"bla\"}";

				streamWriter.Write(json);
				streamWriter.Flush();
				streamWriter.Close();

				var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
				{
					result = streamReader.ReadToEnd();
				}
			}

			return result;
		}
	}
}
