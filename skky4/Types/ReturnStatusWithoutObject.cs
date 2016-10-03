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
		public long code { get; set; }

		public string url { get; set; }

		private List<string> _msg = null;
		public List<string> msg
		{
			get
			{
				if (null == _msg)
					_msg = new List<string>();

				return _msg;
			}
			set
			{
				_msg = value;
			}
		}

		private List<string> _err = null;
		public List<string> err
		{
			get
			{
				if (null == _err)
					_err = new List<string>();

				return _err;
			}
			set
			{
				_err = value;
			}
		}

		public ReturnStatusWithoutObject()
		{ }
		public ReturnStatusWithoutObject(int rc)
		{
			code = rc;
		}
		public ReturnStatusWithoutObject(int rc, string errorMessage)
		{
			err.Add(errorMessage);
		}
		public ReturnStatusWithoutObject(string errorMessage)
		{
			err.Add(errorMessage);
		}

		public void Clear()
		{
			code = 0;
			url = string.Empty;
			_err = null;
			_msg = null;
		}

		public bool HasErrors(int minimumNumberOfItems = 0)
		{
			return (code < 0 || (null != _err && _err.Count() > minimumNumberOfItems));
		}
		public bool ErrorFree()
		{
			return !HasErrors();
		}
		public bool HasMessages(int minimumNumberOfItems = 0)
		{
			return (null != _msg && _msg.Count() > minimumNumberOfItems);
		}
		public bool HasAnyMessages()
		{
			return HasErrors() || HasMessages();
		}
		public virtual bool HasAnyData()
		{
			return HasAnyMessages()
				|| HasErrors()
				|| code != 0
				|| !string.IsNullOrEmpty(url);
		}

		public int AppendMessages(ReturnStatusWithoutObject rs)
		{
			int messagesAdded = 0;

			if (null != rs)
			{
				if (null != rs._err)
				{
					foreach (var rsErr in rs._err)
					{
						err.Add(rsErr);
						++messagesAdded;
					}
				}

				if (null != rs._msg)
				{
					foreach (var rsMsg in rs._msg)
					{
						msg.Add(rsMsg);
						++messagesAdded;
					}
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

		public void AddException(Exception ex)
		{
			AddError(ex.GetExceptionMessage());
		}
		public void AddError(Exception ex)
		{
			AddError(ex.GetExceptionMessage());
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
