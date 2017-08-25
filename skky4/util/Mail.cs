using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;

namespace skky.util
{
	public static class Mail
	{
		private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);
		public static void Send(string toEmails, string ccEmails, string subject, string body, IEnumerable<string> attachmentFileNames = null, string from = "")
		{
			var toArray = Parser.SplitAndTrimString(toEmails);
			var ccArray = Parser.SplitAndTrimString(ccEmails);

			Send(toArray, ccArray, null, subject, body, attachmentFileNames, from);
		}

		public static void Send(IEnumerable<string> to, IEnumerable<string> cc, string subject, string body, string from = "")
		{
			List<string> attachmentFileNames = null;

			Send(to, cc, null, subject, body, attachmentFileNames, from);
		}

		public static void SendWithAttachment(IEnumerable<string> to, string subject, string body, string attachmentFileName, string from = "")
		{
			List<string> attachmentFileNames = null;
			if (!string.IsNullOrWhiteSpace(attachmentFileName))
			{
				attachmentFileNames = new List<string>
				{
					attachmentFileName
				};
			}

			Send(to, null, null, subject, body, attachmentFileNames, from);
		}
		public static void Send(IEnumerable<string> to, IEnumerable<string> cc, string subject, string body, string attachmentFileName, string from = "")
		{
			List<string> attachmentFileNames = null;
			if (!string.IsNullOrWhiteSpace(attachmentFileName))
			{
				attachmentFileNames = new List<string>
				{
					attachmentFileName
				};
			}

			Send(to, cc, null, subject, body, attachmentFileNames, from);
		}

		public static void Send(IEnumerable<string> to, IEnumerable<string> cc, IEnumerable<string> bcc, string subject, string body, IEnumerable<string> attachmentFilenames = null, string from = "")
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

				if (null != cc)
				{
					foreach (var ccAddress in cc)
					{
						mm.CC.Add(new MailAddress(ccAddress.Trim()));
					}
				}

				if (null != bcc)
				{
					foreach (var address in bcc)
					{
						mm.Bcc.Add(new MailAddress(address.Trim()));
					}
				}

				mm.Subject = subject;

				mm.IsBodyHtml = true;
				mm.Body = body;

				if (null != attachmentFilenames && attachmentFilenames.Any(x => null != x && x != string.Empty))
				{
					foreach (var attachmentFilename in attachmentFilenames.Where(x => null != x && x != string.Empty))
					{
						if (File.Exists(attachmentFilename))
						{
							Attachment attachment = new Attachment(attachmentFilename);
							mm.Attachments.Add(attachment);
						}
						else
						{
							logger.Error("Attachment missing: " + attachmentFilename + ".");
						}
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
