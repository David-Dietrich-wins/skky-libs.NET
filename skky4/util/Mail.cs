using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Reflection;

namespace skky.util
{
	public static class Mail
	{
		public static void Send(string to, string cc, string subject, string body, List<string> attachmentFileNames = null)
		{
			Send(null, to, cc, subject, body, attachmentFileNames);
		}
		public static void Send(string from, string to, string cc, string subject, string body, List<string> attachmentFileNames = null)
		{
			string[] toArray = new string[1];
			toArray[0] = to;

			string[] ccArray = new string[string.IsNullOrEmpty(cc) ? 0 : 1];
			if (!string.IsNullOrEmpty(cc))
				ccArray[0] = cc;

			Send(from, toArray, ccArray, subject, body, attachmentFileNames);
		}

		public static void Send(string from, string[] to, string[] cc, string subject, string body)
		{
			List<string> attachmentFileNames = null;

			Send(from, to, cc, subject, body, attachmentFileNames);
		}

		public static void Send(string from, string[] to, string[] cc, string subject, string body, string attachmentFileName)
		{
			List<string> attachmentFileNames = null;
			if (!string.IsNullOrWhiteSpace(attachmentFileName))
			{
				attachmentFileNames = new List<string>();
				attachmentFileNames.Add(attachmentFileName);
			}

			Send(from, to, cc, subject, body, attachmentFileNames);
		}

		public static void Send(string from, string[] to, string[] cc, string subject, string body, List<string> attachmentFileNames)
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

				objMail.Subject = subject;

				objMail.IsBodyHtml = true;
				objMail.Body = body;

				if (null != attachmentFileNames && attachmentFileNames.Count() > 0)
				{
					foreach (var attachmentFileName in attachmentFileNames)
					{
						Attachment attachment = new Attachment(attachmentFileName);
						objMail.Attachments.Add(attachment);
					}
				}

				//var smtp = new System.Net.Mail.SmtpClient(emailHost);
				var smtp = new SmtpClient();
				smtp.Send(objMail);
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				Trace.MethodException(methodName, ex);

				throw;
			}
		}
	}
}
