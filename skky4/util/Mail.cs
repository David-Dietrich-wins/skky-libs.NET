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
		public static void Send(string to, string cc, string subject, string body, IEnumerable<string> attachmentFileNames = null, string from = "")
		{
			List<string> toArray = new List<string>();
			toArray.Add(to);

			List<string> ccArray = new List<string>();
			if (!string.IsNullOrEmpty(cc))
				ccArray.Add(cc);

			Send(toArray, ccArray, subject, body, attachmentFileNames, from);
		}

		public static void Send(IEnumerable<string> to, IEnumerable<string> cc, string subject, string body, string from = "")
		{
			List<string> attachmentFileNames = null;

			Send(to, cc, subject, body, attachmentFileNames, from);
		}

		public static void Send(IEnumerable<string> to, IEnumerable<string> cc, string subject, string body, string attachmentFileName, string from = "")
		{
			List<string> attachmentFileNames = null;
			if (!string.IsNullOrWhiteSpace(attachmentFileName))
			{
				attachmentFileNames = new List<string>();
				attachmentFileNames.Add(attachmentFileName);
			}

			Send(to, cc, subject, body, attachmentFileNames, from);
		}

		public static void Send(IEnumerable<string> to, IEnumerable<string> cc, string subject, string body, IEnumerable<string> attachmentFileNames, string from = "")
		{
			try
			{
				var mm = new MailMessage();

				if (!string.IsNullOrWhiteSpace(from))
					mm.From = new MailAddress(from);

				if (null != to)
				{
					foreach (var toAddress in to)
					{
						mm.To.Add(new MailAddress(toAddress.Trim()));
					}
				}

				if(null != cc)
				{
					foreach (var ccAddress in cc)
					{
						mm.CC.Add(new MailAddress(ccAddress.Trim()));
					}
				}

				mm.Subject = subject;

				mm.IsBodyHtml = true;
				mm.Body = body;

				if (null != attachmentFileNames && attachmentFileNames.Count() > 0)
				{
					foreach (var attachmentFileName in attachmentFileNames)
					{
						Attachment attachment = new Attachment(attachmentFileName);
						mm.Attachments.Add(attachment);
					}
				}

				//var smtp = new System.Net.Mail.SmtpClient(emailHost);
				var smtp = new SmtpClient();
				smtp.Send(mm);
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
