using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.util;

namespace skky.Types
{
	[DataContract]
	public class PropertyLong : Property
	{
		private long? myProperty;

		public PropertyLong()
		{ }
		public PropertyLong(long? l)
		{
			myProperty = l;
		}

		public override object GetObject()
		{
			return myProperty;
		}
        public override System.Type GetObjectType()
        {
            return typeof(long);
        }

		protected override void SetString(string s)
		{
			myProperty = null;
			if(s != null)
				myProperty = s.ToLong();
		}
        protected override long? GetLong()
        {
            return myProperty;
        }
        protected override void SetLong(long? l)
        {
            myProperty = l;
        }
		protected override int? GetInt()
		{
			return (int?)myProperty;
		}
		protected override void SetInt(int? i)
		{
			myProperty = i;
		}
		protected override double? GetDouble()
		{
			return myProperty;
		}
		protected override void SetDouble(double? d)
		{
			myProperty = (long)d;
		}
		protected override DateTime? GetDateTime()
		{
			if(myProperty.HasValue)
				return new DateTime(myProperty.Value);

			return null;
		}
		protected override void SetDateTime(DateTime? dt)
		{
			myProperty = null;
			if(dt.HasValue)
				myProperty = dt.Value.Ticks;
		}

		public override bool IsValueGreaterThan(Property p)
		{
			return myProperty > p.longValue;
		}
		public override bool IsValueLessThan(Property p)
		{
			return myProperty < p.longValue;
		}
		public override bool IsValueEqualTo(Property p)
		{
			return myProperty == p.longValue;
		}
	}
}
