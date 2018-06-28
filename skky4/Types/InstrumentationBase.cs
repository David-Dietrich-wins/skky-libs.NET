using skky.util;
using System;

namespace skky.Types
{
	public class InstrumentationBase
	{
		public int Id { get; set; }
		public string FileName { get; set; }
		public int TotalErrors { get; set; }
		public int TotalExceptions { get; set; }
		public int TotalSuccesses { get; set; }
		public int TotalProcessed { get; set; }
		public int Added { get; set; }
		public int AddErrors { get; set; }
		public int AddExceptions { get; set; }
		public int AddedFound { get; set; }
		public int Updated { get; set; }
		public int UpdateErrors { get; set; }
		public int UpdateExceptions { get; set; }
		public int UpdatedFound { get; set; }
		public int Deleted { get; set; }
		public int DeleteErrors { get; set; }
		public int DeleteExceptions { get; set; }
		public int DeletedFound { get; set; }
		public int NumFileLinesRead { get; set; }
		public int NumFileLinesParsed { get; set; }
		public int SkippedRows { get; set; }
		public string ErrorMsg { get; set; }
		public string Msg { get; set; }

		public bool SendAdminEmail { get; set; }
		public bool WaitForKey { get; set; }

		public DateTime ProcessStartTime { get; set; }
        public DateTime? ProcessEndTime { get; set; }

		public InstrumentationBase()
		{
			ProcessStartTime = DateTime.Now;

			SendAdminEmail = true;
		}

		public virtual void Complete()
		{
			ProcessEndTime = DateTime.Now;
		}
		public DateTime GetProcessStartTime()
		{
			return ProcessStartTime;
		}

		public void SetProcessStartTime(DateTime value)
		{
			ProcessStartTime = value;
		}

		public string TotalProcessingTime()
		{
			return DateTimeHelper.TimeDifferenceMessage(ProcessStartTime, ProcessEndTime ?? DateTime.Now);
		}

		public virtual string GetSummary(string applicationName)
		{
			string s = string.Format("{0} ran in {1}.<br><br>", applicationName ?? string.Empty, TotalProcessingTime());

			s += "<table><caption>Comparison Statistics</caption>";
			s += "<tr><td>Function</td><td># Found</td><td># Processed</td><td># Errors</td><td># Exceptions</td></tr>\n";
			s += string.Format("\n<tr><td>{0}</td><td>{1:n0}</td><td>{2:n0}</td><td>{3:n0}</td><td>{4:n0}</td></tr>", "Added", AddedFound, Added, AddErrors, AddExceptions);
			s += "<tr><td>&nbsp;</td><td>&nbsp;</td></tr>\n";
			s += string.Format("\n<tr><td>{0}</td><td>{1:n0}</td><td>{2:n0}</td><td>{3:n0}</td><td>{4:n0}</td></tr>", "Updated", UpdatedFound, Updated, UpdateErrors, UpdateExceptions);
			s += "<tr><td>&nbsp;</td><td>&nbsp;</td></tr>\n";
			s += string.Format("\n<tr><td>{0}</td><td>{1:n0}</td><td>{2:n0}</td><td>{3:n0}</td><td>{4:n0}</td></tr>", "Deleted", DeletedFound, Deleted, DeleteErrors, DeleteExceptions);
			s += "</table>\n";
			s += "<br /><br />";
			s += "<table>";
			s += string.Format("\n<tr><td>{0}</td><td>{1:n0}</td></tr>", "Total # of Runtime Exceptions", TotalExceptions);
			s += "<tr><td>&nbsp;</td><td>&nbsp;</td></tr>\n";
			s += string.Format("<tr><td>{0}</td><td>{1:n0}</td></tr>", "Runtime Failures", TotalErrors);
			s += "<tr><td>&nbsp;</td><td>&nbsp;</td></tr>\n";
			s += string.Format("<tr><td>{0}</td><td>{1:n0}</td></tr>", "Successes", TotalSuccesses);
			s += "<tr><td>&nbsp;</td><td>&nbsp;</td></tr>\n";
			s += string.Format("<tr><td>{0}</td><td>{1:n0}</td></tr>", "Total Processed", TotalProcessed);
			s += "</table>\n";

			if (!string.IsNullOrWhiteSpace(ErrorMsg))
			{
				s += string.Format("<br /><table class=\"TableAlert\"><caption>Error Message(s)</caption><tr><td><b>{0}</b></td></tr></table>", ErrorMsg);
			}

			if (!string.IsNullOrWhiteSpace(Msg))
			{
				s += string.Format("<br /><table class=\"info-message\"><caption>Message(s)</caption><tr><td><b>{0}</b></td></tr></table>", Msg);
			}

			return s;
		}
	}
}
