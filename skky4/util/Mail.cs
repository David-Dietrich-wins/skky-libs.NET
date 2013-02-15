using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace skky.util
{
	public static class Mail
	{
		public static void Send(string to, string cc, string subject, string body)
		{
			Send(null, to, cc, subject, body);
		}
		public static void Send(string from, string to, string cc, string subject, string body)
		{
			string[] toArray = new string[1];
			toArray[0] = to;

			string[] ccArray = new string[string.IsNullOrEmpty(cc) ? 0 : 1];
			if (!string.IsNullOrEmpty(cc))
				ccArray[0] = cc;

			Send(from, toArray, ccArray, subject, body);
		}

		public static void Send(string from, string[] to, string[] cc, string subject, string body)
		{
			if(string.IsNullOrEmpty(from))
				from = "skkyHost@skky.net";
			//string emailHost = "mail.skky.net";

			try
			{
				var objMail = new MailMessage();
				objMail.From = new MailAddress(from.Trim());

				if (null != to)
				{
					foreach (var toAddress in to)
					{
						objMail.To.Add(new MailAddress(toAddress.Trim()));
					}
				}

				if(null != cc)
				{
					foreach (var ccAddress in cc)
					{
						objMail.CC.Add(new MailAddress(ccAddress.Trim()));
					}
				}
				//var a = new MailAttachment("C:\a.jpg");
				//objMail.Attachments.Add(a);
				objMail.IsBodyHtml = true;
				objMail.Body = body;
				objMail.Subject = subject;
				objMail.Body = body;

				//var smtp = new System.Net.Mail.SmtpClient(emailHost);
				var smtp = new SmtpClient();
				smtp.Send(objMail);
			}
			catch (Exception ex)
			{
				System.Console.WriteLine(ex.ToString());
				throw;
			}
		}
	}
}
