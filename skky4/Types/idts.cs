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

		private long m_id = 0;
		/// <summary>
		/// Transaction ID for this object.
		/// </summary>
		public long id
		{
			get
			{
				if (m_id == 0)
					m_id = GetNextId();

				return m_id;
			}
			set
			{
				m_id = value;
			}
		}

		private long m_ts = 0;
		/// <summary>
		/// Unix Timestamp in milliseconds since Unix Epoch 1/1/1970.
		/// </summary>
		public long ts
		{
			get
			{
				if (m_ts == 0)
					m_ts = DateTimeHelper.GetUnixTimestampMillis();

				return m_ts;
			}
			set
			{
				m_ts = value;
			}
		}

		protected long GetNextId()
		{
			lock(m_TransactionObject)
			{
				++m_TransactionId;

				return m_TransactionId;
			}
		}
	}

	public interface Iidts
	{
		long id { get; set; }
		long ts { get; set; }
	}
}
