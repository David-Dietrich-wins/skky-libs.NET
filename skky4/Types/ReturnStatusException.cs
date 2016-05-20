using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.util;

namespace skky.Types
{
	[Serializable]
	[DataContract]
	public class ReturnStatusException : Exception
	{
		public ReturnStatus returnStatus;

		public ReturnStatusException() : base()
		{ }
		public ReturnStatusException(string message) : base(message)
		{ }
		public ReturnStatusException(string message, System.Exception inner) : base(message, inner)
		{ }

		// A constructor is needed for serialization when an
		// exception propagates from a remoting server to the client. 
		protected ReturnStatusException(System.Runtime.Serialization.SerializationInfo info,
	        System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{ }

		public ReturnStatusException(ReturnStatus rs)
		{
			returnStatus = rs;
		}

		public bool HasErrors()
		{
			return returnStatus.HasErrors();
		}
		public bool ErrorFree()
		{
			return !HasErrors();
		}
		public bool HasMessages()
		{
			return Message.Count() > 0 ? true : false;
		}
	}
}
