using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class PropertyGuid : Property
	{
		private Guid? myProperty;

		public PropertyGuid()
		{ }
		public PropertyGuid(Guid? g)
		{
			myProperty = g;
		}

		public override object GetObject()
		{
			return myProperty;
		}
        public override System.Type GetObjectType()
        {
            return typeof(Guid);
        }

		protected override void SetString(string s)
		{
			myProperty = null;
			if(s != null)
				myProperty = new Guid(s);
		}
		protected override Guid? GetGuid()
		{
			return myProperty;
		}
		protected override void SetGuid(Guid? s)
		{
			myProperty = s;
		}

		public override bool IsValueEqualTo(Property p)
		{
			return myProperty.Equals(p.guidValue);
		}
	}
}
