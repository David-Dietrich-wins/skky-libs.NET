using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.util;

namespace skky.Types
{
	[DataContract]
	public class PropertyString : Property
	{
		private string myProperty;

		public PropertyString()
		{ }
		public PropertyString(string s)
		{
			myProperty = s;
		}

		public override object GetObject()
		{
			return myProperty;
		}
        public override System.Type GetObjectType()
        {
            return typeof(string);
        }

		protected override string GetString()
		{
			return myProperty;
		}
		protected override void SetString(string s)
		{
			myProperty = s;
		}
		protected override int? GetInt()
		{
			if(myProperty == null)
				return null;

			return myProperty.ToInteger();
		}
		protected override void SetInt(int? i)
		{
			myProperty = null;
			if (i.HasValue)
				myProperty = i.Value.ToString();
		}
		protected override double? GetDouble()
		{
			if (myProperty == null)
				return null;

			return myProperty.ToDouble();
		}
		protected override void SetDouble(double? d)
		{
			myProperty = null;
			if (d.HasValue)
				myProperty = d.Value.ToString();
		}
		protected override DateTime? GetDateTime()
		{
			if (myProperty == null)
				return null;

			return myProperty.ToDateTime();
		}
		protected override void SetDateTime(DateTime? dt)
		{
			myProperty = null;
			if (dt.HasValue)
				myProperty = dt.Value.ToShortDateString();
		}
		protected override Guid? GetGuid()
		{
			if (myProperty == null)
				return null;

			return new Guid(myProperty);
		}
		protected override void SetGuid(Guid? g)
		{
			myProperty = null;
			if(g.HasValue)
				myProperty = g.Value.ToString();
		}

		public override bool IsValueGreaterThan(Property p)
		{
			int i = myProperty.CompareTo(p.stringValue);
			return i > 0;
		}
		public override bool IsValueLessThan(Property p)
		{
			int i = myProperty.CompareTo(p.stringValue);
			return i < 0;
		}
		public override bool IsValueEqualTo(Property p)
		{
			return myProperty.Equals(p.stringValue);
		}
	}
}
