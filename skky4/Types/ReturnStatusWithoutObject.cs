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
		public virtual long code { get; set; }
		public virtual long subcode { get; set; }

		private List<string> _messageList = null;
		public virtual List<string> msg
		{
			get
			{
				if (null == _messageList)
					_messageList = new List<string>();

				return _messageList;
			}
			set
			{
				_messageList = value;
			}
		}

		private List<string> _errorList = null;
		public virtual List<string> err
		{
			get
			{
				if (null == _errorList)
					_errorList = new List<string>();

				return _errorList;
			}
			set
			{
				_errorList = value;
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

		public virtual void Clear()
		{
			code = 0;
			_errorList = null;
			_messageList = null;
		}

		public virtual bool HasErrors(int minimumNumberOfItems = 0)
		{
			return (code < 0 || (null != _errorList && _errorList.Count() > minimumNumberOfItems));
		}
		public virtual bool ErrorFree()
		{
			return !HasErrors();
		}
		public virtual bool HasMessages(int minimumNumberOfItems = 0)
		{
			return (null != _messageList && _messageList.Count() > minimumNumberOfItems);
		}
		public bool HasAnyMessages()
		{
			return HasErrors() || HasMessages();
		}
		public virtual bool HasAnyData()
		{
			return HasAnyMessages()
				|| HasErrors()
				|| code != 0;
		}

		public virtual int AppendMessages(ReturnStatusWithoutObject rs)
		{
			int messagesAdded = 0;

			if (null != rs)
			{
				if (null != rs._errorList)
				{
					foreach (var rsErr in rs._errorList)
					{
						err.Add(rsErr);
						++messagesAdded;
					}
				}

				if (null != rs._messageList)
				{
					foreach (var rsMsg in rs._messageList)
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

		public virtual void AddException(Exception ex)
		{
			AddError(ex.GetExceptionMessage());
		}
		public virtual void AddError(Exception ex)
		{
			AddError(ex.GetExceptionMessage());
		}
		public virtual void AddError(string errorString)
		{
			err.Add(errorString);
		}

		public virtual void AddMessage(string message)
		{
			msg.Add(message);
		}
	}
}
