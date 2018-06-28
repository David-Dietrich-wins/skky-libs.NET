using skky.Types;
using skky.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace skky.App
{
	/// <summary>
	/// The base level class that all Programs derive from.
	/// Provides the basic Run and Process functionality that each program uses.
	/// The main method to override is the Process() method.
	/// </summary>
	public class ProgramBase
	{
		public const string CONST_Date4DigitYear = "MMM d, yyyy";
		public const string CONST_DateTimeLong = "MMMM dd, yyyy hh:mm:ss.ff tt";

		private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

		public DateTime dtStartRun = DateTime.Now;

		/// <summary>
		/// A ReturnStatus object that can be used anywhere throughout the program.
		/// </summary>
		public ReturnStatus rs = new ReturnStatus();

		/// <summary>
		/// Constructor that initializes the log4net infrastructure.
		/// </summary>
		public ProgramBase()
		{
			log4net.Config.XmlConfigurator.Configure();
		}

		/// <summary>
		/// Get the Application name.
		/// </summary>
		/// <returns>The string application name. Default to ProgramBase.</returns>
		public virtual string GetApplicationName()
		{
			return "ProgramBase";
		}

		/// <summary>
		/// Get the log4net log manager for this class.
		/// </summary>
		/// <returns>This classes log manager.</returns>
		public virtual log4net.ILog GetLogger()
		{
			return logger;
		}

		/// <summary>
		/// Convenient method to write out an error message to the log.
		/// </summary>
		/// <param name="methodName">The method name that generated the error.</param>
		/// <param name="errorMessage">The error message to write out to the log.</param>
		/// <returns>A string of the entire message written to the log file.</returns>
		protected virtual string LogError(string methodName, string errorMessage)
		{
			string s = Trace.MethodInformation(methodName, errorMessage);

			rs.AddError(s);

			GetLogger().Error(s);

			return s;
		}

		/// <summary>
		/// Convenient method to write out an informational message to the log.
		/// </summary>
		/// <param name="methodName">The method name that generated the message.</param>
		/// <param name="msg">The informational message to write out to the log.</param>
		/// <returns>A string of the entire message written to the log file.</returns>
		protected virtual string LogInfo(string methodName, string msg)
		{
			string s = Trace.MethodInformation(methodName, msg);

			GetLogger().Info(s);

			return s;
		}

		/// <summary>
		/// Convenient method to write out an exception to the log.
		/// </summary>
		/// <param name="methodName">The method name that generated the exception.</param>
		/// <param name="ex">The Exception object.</param>
		/// <param name="msg">Additional message to write out to the log.</param>
		/// <returns>A string of the entire message written to the log file.</returns>
		protected virtual string LogException(string methodName, Exception ex, string msg = "")
		{
			string s = Trace.MethodException(methodName, ex, msg);

			rs.AddError(s);

			GetLogger().Error(s);

			return s;
		}

		/// <summary>
		/// Wrapper to write out an informational message to the log and to the screen.
		/// </summary>
		/// <param name="methodName">The method name that generated the message.</param>
		/// <param name="msg">The informational message to write out to the log.</param>
		/// <returns>A string of the entire message written to the log file.</returns>
		protected string ShowInformation(string methodName, string msg)
		{
			Console.WriteLine(msg);

			return TraceInformation(methodName, msg);
		}

		/// <summary>
		/// Convenient method to write out an informational message to the log.
		/// </summary>
		/// <param name="methodName">The method name that generated the message.</param>
		/// <param name="msg">The informational message to write out to the log.</param>
		/// <returns>A string of the entire message written to the log file.</returns>
		protected virtual string TraceInformation(string methodName, string msg)
		{
			string className = this.GetType().Name;

			LogInfo(methodName, msg);

			return Trace.MethodInformation(className, methodName, msg);
		}

		/// <summary>
		/// Convenient method to write out an exception to the log.
		/// </summary>
		/// <param name="methodName">The method name that generated the exception.</param>
		/// <param name="ex">The Exception object.</param>
		/// <param name="msg">Additional message to write out to the log.</param>
		/// <returns>A string of the entire message written to the log file.</returns>
		protected virtual string TraceException(string methodName, Exception ex, string msg = "")
		{
			string className = this.GetType().Name;

			LogException(methodName, ex, msg);

			string s = Trace.MethodException(className, methodName, ex, msg);

			rs.AddError(s);

			return s;
		}

		/// <summary>
		/// Get the applications Admin domain for website administration.
		/// Subclasses should override.
		/// </summary>
		/// <returns>The Admin website for this application.</returns>
		public virtual string GetAdminDomain()
		{
			return string.Empty;
		}
		/// <summary>
		/// Get the applications Public domain for its website.
		/// Subclasses should override.
		/// </summary>
		/// <returns>The Public website for this application.</returns>
		public virtual string GetPublicDomain()
		{
			return string.Empty;
		}

		/// <summary>
		/// Returns the InstrumentationBase object used for this application.
		/// Subclasses should override.
		/// </summary>
		/// <returns>This application's InstrumentationBase object.</returns>
		public virtual InstrumentationBase GetInstrumentation()
		{
			return null;
		}

		/// <summary>
		/// Returns the Public URL based on the relative path passed in.
		/// </summary>
		/// <param name="relativePath">The relative path to append to the Public URL.</param>
		/// <returns>The full Public URL.</returns>
		public virtual string GetPublicUrl(string relativePath)
		{
			return GetPublicDomain().UrlCombine((relativePath ?? string.Empty).ToLower());
		}

		/// <summary>
		/// Gets attachment filenames to be attached to the Admin summary email.
		/// Subclasses should call this method first to get a list of already selected attachments.
		/// </summary>
		/// <param name="includePath">Set to true to include the full path of the attachments.</param>
		/// <returns>A list of attachment filenames.</returns>
		public virtual List<string> GetAttachmentFileNames(bool includePath = true)
		{
			return new List<string>();
		}

		/// <summary>
		/// Get the BCC email addresses for the Admin summary email.
		/// </summary>
		/// <returns>A list of BCC email addresses.</returns>
		public virtual List<string> GetEmailBccAddresses()
		{
			return new List<string>();
		}
		/// <summary>
		/// Get the Admin email addresses for the Admin summary email.
		/// </summary>
		/// <returns>A list of email addresses to send the Admin summary to.</returns>
		public virtual List<string> GetAdminEmailAddresses()
		{
			return new List<string>();
		}

		/// <summary>
		/// Wrapper method for sending an email with attachments to the GetAdminEmailAddresses() list of emails.
		/// </summary>
		/// <param name="subject">Subject line for the email to send.</param>
		/// <param name="body">Email body of the email.</param>
		/// <param name="attachmentFileNames">Any attachments to be added to the email.</param>
		/// <returns>True if successful send of email.</returns>
		protected virtual bool SendEmail(string subject, string body, IEnumerable<string> attachmentFileNames = null)
		{
			Mail.Send(GetAdminEmailAddresses()
				, new List<string>()
				, GetEmailBccAddresses()
				, subject
				, body
				, attachmentFileNames
				);

			return true;
		}

		/// <summary>
		/// Get the Admin summary email CSS styles.
		/// </summary>
		/// <returns>The Admin summary email CSS styles.</returns>
		public virtual string GetEmailStyle()
		{
			return @"
<style type='text/css'>
table.TableAlert
{
	border: 1px #03F solid;
	border-collapse: collapse;
}
table.TableAlert caption
{
	background-color: #fea0a0;
	text-align: center;
	text-decoration: none;
	font-size: 13pt;
	font-weight: bold;
	color: black;
	border-bottom-color: #03F;
	border-bottom-width: 3px;
	border-bottom-style: solid;
}
table.info-message
{
	border: 1px #03F solid;
	border-collapse: collapse;
}
table.info-message caption
{
	background-color: #a0fea0;
	text-align: center;
	text-decoration: none;
	font-size: 13pt;
	font-weight: bold;
	color: black;
	border-bottom-color: #03F;
	border-bottom-width: 3px;
	border-bottom-style: solid;
}
table.TableSalutation
{
	font-family: Arial, 'Arial Narrow', 'Arial MT Std';
	font-weight: normal;
	font-size: 11px;
	color: #404040;
	background-color: #fafafa;
	border: 1px #8FA135 solid;
	border-collapse: collapse;
	margin-top: 5px;
	max-width: 500px;
}
table.TableSalutation a
{
	border: none;
	text-decoration: none;
}
table.TableSalutation a:link, a:active, a:visited
{
	color: #A73A02;
}
table.TableSalutation a:hover
{
	text-decoration: underline;
	color: #D88207;
}
table.TableSalutation td.header
{
	border-bottom: 2px solid #d79900;
	background-color: #8FA135;
	color: White;
	font-family: Verdana;
	font-weight: bold;
	font-size: 14px;
	text-align: center;
}
table.TableSalutation td.subtitle
{
	color: #404040;
	font-family: Verdana;
	font-weight: bold;
	font-size: 12px;
	padding-left: 5px;
	padding-top: 10px;
}
</style>";
		}

		/// <summary>
		/// Get the salutation for the Admin summary email.
		/// </summary>
		/// <returns>The Admin summary email salutation.</returns>
		public virtual string GetEmailSalutation()
		{
			return @"
<br /><p>
<table class='TableSalutation'><tr>
	<td class='header'>Thank you for choosing <a href='http://www.GrayArrow.com/'>GrayArrow</a><br />Professional Business Solutions!</td>
</tr>
<tr>
	<td>
	If you encounter any problems, or would like a question answered, please e-mail us at <a href='mailto:support@GrayArrow.com'>support@grayarrow.com</a>.</td></tr>
<tr><td>
<br />
Your GrayArrow Team!</td></tr>
</table>
</p>";
		}

		/// <summary>
		/// Get the subject line for the Admin summary email.
		/// </summary>
		/// <returns>The Admin summary email subject line.</returns>
		public virtual string GetEmailSubject()
		{
			return GetApplicationName() + " Run Summary on " + DateTimeHelper.GetDefaultDateTimeString(dtStartRun) + ".";
		}

		/// <summary>
		/// Overridable method that is the template for creating the overall body content for the admin status email.
		/// </summary>
		/// <returns></returns>
		public virtual string GetEmailBody()
		{
			string body = "<html><head>" + GetEmailStyle() + "</head><body>";
			body += GetEmailBodyContent();
			body += GetEmailSalutation();
			body += "</body></html>";

			return body;
		}

		/// <summary>
		/// Returns the Instrumentation content for the Admin summary email.
		/// </summary>
		/// <returns>The entire body content of the email generated by GetInstrumentation().</returns>
		public virtual string GetEmailBodyContent()
		{
			return GetInstrumentation().GetSummary(GetApplicationName());
		}

		/// <summary>
		/// The main method for the applications processing.
		/// This method must be overridden by subclasses.
		/// </summary>
		/// <param name="processArguments">The arguments passed to the application.</param>
		public virtual void Process(string[] processArguments)
		{
			throw new Exception("No Process() method declared.");
		}

		/// <summary>
		/// Main run wrapper that sets and tracks overall processing time.
		/// This method does not typically need to be overridden.
		/// Override the Process() method in most cases.
		/// </summary>
		/// <param name="processArguments">The arguments passed to the application.</param>
		public virtual void Run(string[] processArguments)
		{
			string methodName = MethodBase.GetCurrentMethod().Name;

			LogInfo(methodName, string.Format("Starting run of {0} at {1}.", GetApplicationName(), DateTimeHelper.GetDefaultDateTimeString(dtStartRun)));

			string msg = string.Empty;

			try
			{
				Process(processArguments);
			}
			catch (Exception ex)
			{
				++GetInstrumentation().TotalExceptions;

				msg = LogException(methodName, ex);
				GetInstrumentation().Msg += msg + "<br />";
			}

			try
			{
				GetInstrumentation().Complete();

				ShowInformation(methodName, string.Format("Finished {0} at {1}. Run time was {2}.", GetApplicationName(), DateTimeHelper.GetDefaultDateTimeString(GetInstrumentation().ProcessEndTime), GetInstrumentation().TotalProcessingTime()));

				if (GetInstrumentation().SendAdminEmail)
				{
					string body = GetEmailBody();
					Console.WriteLine(body);
					LogInfo(methodName, body);

					var adminEmails = GetAdminEmailAddresses();
					if (null != adminEmails && adminEmails.Any())
						SendEmail(GetEmailSubject(), body, GetAttachmentFileNames());
				}
			}
			catch (Exception ex)
			{
				++GetInstrumentation().TotalExceptions;
				LogException("Run", ex);
			}

			LogInfo(methodName, string.Format("Finished {0} at {1}. Run time was {2}.", GetApplicationName(), DateTimeHelper.GetDefaultDateTimeString(DateTime.Now), DateTimeHelper.TimeDifferenceMessage(dtStartRun, DateTime.Now)));
		}
	}
}
