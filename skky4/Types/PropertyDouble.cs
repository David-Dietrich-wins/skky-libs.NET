using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.util;

namespace skky.Types
{
	[DataContract]
	public class PropertyDouble : Property
	{
		private double? myProperty;

		public PropertyDouble()
		{ }
		public PropertyDouble(double? d)
		{
			myProperty = d;
		}

		public override object GetObject()
		{
			return myProperty;
		}
        public override System.Type GetObjectType()
        {
            return typeof(double);
        }

		protected override void SetString(string s)
		{
			myProperty = null;
			if(s != null)
				myProperty = s.ToDouble();
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
			myProperty = d;
		}
		protected override DateTime? GetDateTime()
		{
			if (!myProperty.HasValue)
				return null;

			return new DateTime((long)myProperty);
		}
		protected override void SetDateTime(DateTime? dt)
		{
			myProperty = null;
			if (dt.HasValue)
				myProperty = ((DateTime)dt).Ticks;
		}

		public override bool IsValueGreaterThan(Property p)
		{
			return myProperty > p.doubleValue;
		}
		public override bool IsValueLessThan(Property p)
		{
			return myProperty < p.doubleValue;
		}
		public override bool IsValueEqualTo(Property p)
		{
			return myProperty == p.doubleValue;
		}
	}
}
