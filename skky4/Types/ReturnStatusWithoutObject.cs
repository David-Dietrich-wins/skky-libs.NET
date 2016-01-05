using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.util;

namespace skky.Types
{
	public class ReturnStatusWithoutObject : idts
	{
		public int code { get; set; }

		public string url { get; set; }

		public List<string> msg { get; set; }

		public List<string> err { get; set; }

		public ReturnStatusWithoutObject()
		{
			msg = new List<string>();
			err = new List<string>();
		}
		public ReturnStatusWithoutObject(int rc)
			: this()
		{
			code = rc;
		}
		public ReturnStatusWithoutObject(int rc, string errorMessage)
			: this(rc)
		{
			err.Add(errorMessage);
		}
		public ReturnStatusWithoutObject(string errorMessage)
			: this()
		{
			err.Add(errorMessage);
		}

		public bool HasErrors()
		{
			return (code < 0 || err.Any());
		}
		public bool ErrorFree()
		{
			return !HasErrors();
		}
		public bool HasMessages()
		{
			return msg.Any();
		}
		public bool HasAnyMessages()
		{
			return HasErrors() || HasMessages();
		}

		public int AddMessages(ReturnStatusWithoutObject rs)
		{
			int messagesAdded = 0;
			if (null != rs)
			{
				foreach (var rsErr in rs.err)
				{
					err.Add(rsErr);
					++messagesAdded;
				}

				foreach (var rsMsg in rs.msg)
				{
					msg.Add(rsMsg);
					++messagesAdded;
				}
			}

			return messagesAdded;
		}

		public string GetErrorMessage()
		{
			if (HasErrors())
				return string.Join(", ", err);

			return string.Empty;
		}

		public void AddError(string errorString)
		{
			err.Add(errorString);
		}

		public void AddMessage(string message)
		{
			msg.Add(message);
		}
	}
}
