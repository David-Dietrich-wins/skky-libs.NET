using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.util;

namespace skky.Types
{
	[DataContract]
	public class PropertyInt : Property
	{
		private int? myProperty;

		public PropertyInt()
		{ }
		public PropertyInt(int? i)
		{
			myProperty = i;
		}

		public override object GetObject()
		{
			return myProperty;
		}
        public override System.Type GetObjectType()
        {
            return typeof(int);
        }

		protected override void SetString(string s)
		{
			myProperty = null;
			if (s != null)
				myProperty = s.ToInteger();
		}
        protected override long? GetLong()
        {
            return myProperty;
        }
        protected override void SetLong(long? l)
        {
            myProperty = (int?)l;
        }
		protected override int? GetInt()
		{
			return myProperty;
		}
		protected override void SetInt(int? i)
		{
			myProperty = i;
		}
		protected override double? GetDouble()
		{
			return myProperty;
		}
		protected override void SetDouble(double? s)
		{
			myProperty = (int?)s;
		}
		protected override DateTime? GetDateTime()
		{
			long? l = GetLong();
			if (l.HasValue)
				return new DateTime((long)l);

			return null;
		}
		protected override void SetDateTime(DateTime? dt)
		{
			myProperty = null;
			if(dt.HasValue)
				myProperty = (int)dt.Value.Ticks;
		}

		public override bool IsValueGreaterThan(Property p)
		{
			return myProperty > p.intValue;
		}
		public override bool IsValueLessThan(Property p)
		{
			return myProperty < p.intValue;
		}
		public override bool IsValueEqualTo(Property p)
		{
			return myProperty == p.intValue;
		}
	}
}
