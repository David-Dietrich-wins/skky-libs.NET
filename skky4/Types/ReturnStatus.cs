using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.util;

namespace skky.Types
{
	[DataContract]
	public class ReturnStatus
	{
		public ReturnStatus()
		{
			Message = new List<string>();
			ErrorMessage = new List<string>();
		}
		public ReturnStatus(int rc)
			: this()
		{
			ReturnCode = rc;
		}
		public ReturnStatus(int rc, string errorMessage)
			: this(rc)
		{
			ErrorMessage.Add(errorMessage);
		}
		public ReturnStatus(string errorMessage)
			: this()
		{
			ErrorMessage.Add(errorMessage);
		}

		public bool HasErrors()
		{
			return ReturnCode < 0 || ErrorMessage.Count() > 0 ? true : false;
		}
		public bool ErrorFree()
		{
			return !HasErrors();
		}
		public bool HasMessages()
		{
			return Message.Count() > 0 ? true : false;
		}

		public int AddMessages(ReturnStatus rs)
		{
			int messagesAdded = 0;
			if (null != rs)
			{
				foreach (var rsErr in rs.ErrorMessage)
				{
					ErrorMessage.Add(rsErr);
					++messagesAdded;
				}

				foreach (var rsMsg in rs.Message)
				{
					Message.Add(rsMsg);
					++messagesAdded;
				}
			}

			return messagesAdded;
		}

		[DataMember]
		public int ReturnCode { get; set; }

		[DataMember]
		public string url { get; set; }

		[DataMember]
		public object obj { get; set; }

		[DataMember]
		public List<string> Message { get; set; }

		[DataMember]
		public List<string> ErrorMessage { get; set; }

		public string GetErrorMessage()
		{
			if (HasErrors())
				return string.Join(", ", ErrorMessage);

			return string.Empty;
		}

		public static ReturnStatus AddExceptionErrorMessage(ReturnStatus rs, Exception ex)
		{
			string exceptionMessage = skky.util.Trace.GetExceptionMessageString(ex);
			if (null == rs)
				rs = new ReturnStatus(-1, exceptionMessage);
			else
				rs.ErrorMessage.Add(exceptionMessage);

			return rs;
		}

		public ReturnStatus AddExceptionErrorMessage(Exception ex)
		{
			return ReturnStatus.AddExceptionErrorMessage(this, ex);
		}

		public static ReturnStatus AddError(ReturnStatus rs, string errorString)
		{
			if (null == rs)
				rs = new ReturnStatus();

			rs.AddError(errorString);

			return rs;
		}
		public void AddError(string errorString)
		{
			ErrorMessage.Add(errorString);
		}

		public static ReturnStatus AddMessage(ReturnStatus rs, string msg)
		{
			if (null == rs)
				rs = new ReturnStatus();

			rs.AddError(msg);

			return rs;
		}
		public void AddMessage(string msg)
		{
			Message.Add(msg);
		}
	}
}
