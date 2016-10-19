using skky.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skky.Types
{
	public class idts : Iidts
	{
		private static object m_TransactionObject = new object();
		private static long m_TransactionId = 0;

		private long _Identifier = 0;
		/// <summary>
		/// Transaction ID for this object.
		/// </summary>
		public long id
		{
			get
			{
				if (_Identifier == 0)
					_Identifier = GetNextId();

				return _Identifier;
			}
			set
			{
				_Identifier = value;
			}
		}

		private long _TimeStamp = 0;
		/// <summary>
		/// Unix Timestamp in milliseconds since Unix Epoch 1/1/1970.
		/// </summary>
		public long ts
		{
			get
			{
				if (_TimeStamp == 0)
					_TimeStamp = DateTimeHelper.GetUnixTimestampMillis();

				return _TimeStamp;
			}
			set
			{
				_TimeStamp = value;
			}
		}

		protected long GetNextId()
		{
			lock(m_TransactionObject)
			{
				return ++m_TransactionId;
			}
		}
	}

	public interface Iidts
	{
		long id { get; set; }
		long ts { get; set; }
	}
}
