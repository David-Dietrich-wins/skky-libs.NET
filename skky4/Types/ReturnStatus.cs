using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.util;

namespace skky.Types
{
	public class ReturnStatus : ReturnStatusWithoutObject
	{
		public ReturnStatus()
		{ }
		public ReturnStatus(int rc)
			: base(rc)
		{ }
		public ReturnStatus(int rc, string errorMessage)
			: base(rc, errorMessage)
		{ }
		public ReturnStatus(string errorMessage)
			: base(errorMessage)
		{ }

		public object o { get; set; }

		public static ReturnStatus AddExceptionErrorMessage(ReturnStatus rs, Exception ex)
		{
			string exceptionMessage = ex.GetExceptionMessage();
			if (null == rs)
				rs = new ReturnStatus(-1, exceptionMessage);
			else
				rs.err.Add(exceptionMessage);

			return rs;
		}

		public ReturnStatus AddExceptionErrorMessage(Exception ex)
		{
			return ReturnStatus.AddExceptionErrorMessage(this, ex);
		}

		public static ReturnStatus AddMessage(ReturnStatus rs, string message)
		{
			if (null == rs)
				rs = new ReturnStatus();

			rs.AddError(message);

			return rs;
		}
	}
}
