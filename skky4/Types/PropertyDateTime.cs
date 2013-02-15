using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.util;

namespace skky.Types
{
	[DataContract]
	public class PropertyDateTime : Property
	{
		private DateTime? myProperty;

		public PropertyDateTime()
		{ }
		public PropertyDateTime(DateTime? dt)
		{
			myProperty = dt;
		}

		public override object GetObject()
		{
			return myProperty;
		}
        public override System.Type GetObjectType()
        {
            return typeof(DateTime);
        }

		protected override string GetString()
		{
			if (!myProperty.HasValue)
				return null;

			return myProperty.Value.ToShortDateString();
		}
		protected override void SetString(string s)
		{
			if (string.IsNullOrEmpty(s))
				myProperty = null;

		    myProperty = s.ToDateTime();
		}
		protected override int? GetInt()
		{
			if (!myProperty.HasValue)
				return null;

			return (int?)myProperty.Value.Ticks;
		}
		protected override void SetInt(int? i)
		{
			myProperty = null;
			if(i.HasValue)
				myProperty = new DateTime(i.Value);
		}
		protected override double? GetDouble()
		{
			if (!myProperty.HasValue)
				return null;

			return myProperty.Value.Ticks;
		}
		protected override void SetDouble(double? d)
		{
			myProperty = null;
			if(d.HasValue)
				myProperty = new DateTime((long)d.Value);
		}
		protected override DateTime? GetDateTime()
		{
			return myProperty;
		}
		protected override void SetDateTime(DateTime? dt)
		{
			myProperty = dt;
		}

		public override bool IsValueGreaterThan(Property p)
		{
			return myProperty > p.dateTimeValue;
		}
		public override bool IsValueLessThan(Property p)
		{
			return myProperty < p.dateTimeValue;
		}
		public override bool IsValueEqualTo(Property p)
		{
			return myProperty == p.dateTimeValue;
		}
	}
}
