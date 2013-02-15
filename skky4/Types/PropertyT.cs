using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class PropertyT<T> : Property
	{
		protected PropertyT()
		{ }

		protected PropertyT(T propInitialValue)
		{
			prop = propInitialValue;
		}

		[DataMember]
		public T prop { get; set; }

		public override object GetObject()
		{
			return prop;
		}
		public override System.Type PropertyType
		{
			get
			{
				return typeof(T);
			}
		}

		protected override string GetString()
		{
			return (prop == null) ? string.Empty : prop.ToString();

			//object o = GetObject();
			//if (o == null)
			//    return string.Empty;

			//return o.ToString();
		}
	}
}
