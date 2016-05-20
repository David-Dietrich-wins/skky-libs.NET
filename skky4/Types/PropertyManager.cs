using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.util;

namespace skky.Types
{
	[DataContract]
	public class PropertyManager : List<Property>
	{
		public PropertyManager()
		{ }

		public KeyValuePairCollection<int, System.Type> GetDescription()
		{
			KeyValuePairCollection<int, System.Type> k = new KeyValuePairCollection<int, System.Type>();
			int i = 0;
			foreach(var p in this)
			{
				SerializableKeyValuePair<int, System.Type> kvp = new SerializableKeyValuePair<int, System.Type>(i, p.PropertyType);
				k.Add(kvp);

				++i;
			}

			return k;
		}

		public System.Type GetType(int offset)
		{
			return GetProperty(offset).PropertyType;
		}

		public Property GetProperty(int PropertyNum)
		{
			if (PropertyNum < 0 || PropertyNum >= base.Count)
				throw new Exception(string.Format("PropertyManager attempted to access an invalid Property number of {0}.", PropertyNum));

			return this[PropertyNum];
		}

		public string GetString(int propertyNum)
		{
			Property p = GetProperty(propertyNum);
			return p.stringValue;
		}
		public int? GetInt(int propertyNum)
		{
			Property p = GetProperty(propertyNum);
			return p.intValue;
		}
		public double? GetDouble(int propertyNum)
		{
			Property p = GetProperty(propertyNum);
			return p.doubleValue;
		}
		public long? GetLong(int propertyNum)
		{
			Property p = GetProperty(propertyNum);
			return p.longValue;
		}
		public DateTime? GetDateTime(int propertyNum)
		{
			Property p = GetProperty(propertyNum);
			return p.dateTimeValue;
		}
		public int GetIntOrDefault(int propertyNum)
		{
			Property p = GetProperty(propertyNum);
			return p.intValueOrDefault;
		}
		public double GetDoubleOrDefault(int propertyNum)
		{
			Property p = GetProperty(propertyNum);
			return p.doubleValueOrDefault;
		}
		public long GetLongOrDefault(int propertyNum)
		{
			Property p = GetProperty(propertyNum);
			return p.longValueOrDefault;
		}
		public DateTime GetDateTimeOrDefault(int propertyNum)
		{
			Property p = GetProperty(propertyNum);
			return p.dateTimeValueOrDefault;
		}

		private Property GetFirst(System.Type pt)
		{
			foreach (var prop in this)
			{
				if (prop.PropertyType == pt)
					return prop;
			}

			return null;
		}

		public List<Property> GetProperties(IEnumerable<int> PropertyNumbers)
		{
			List<Property> props = new List<Property>();
			if (PropertyNumbers != null)
			{
				foreach (int i in PropertyNumbers)
					props.Add(GetProperty(i));
			}

			return props;
		}
		public object[] GetObjects(IEnumerable<int> PropertyNumbers)
		{
			List<Property> props = GetProperties(PropertyNumbers);
			object[] o = new object[props.Count()];
			int offset = 0;
			foreach(var prop in props)
			{
				o[offset] = prop.GetObject();

				++offset;
			}

			return o;
		}

		public string stringValue
		{
			get
			{
				Property p = GetFirst(typeof(string));
				if (p == null)
					return null;

				return p.stringValue;
			}

			set
			{
				PropertyString p = Property.GetNew(value);
				this.Add(p);
			}
		}
		public int? intValue
		{
			get
			{
				Property p = GetFirst(typeof(int));
				return (p == null ? null : p.intValue);
			}

			set
			{
				PropertyInt p = Property.GetNew(value);
				this.Add(p);
			}
		}
		public int intValueOrDefault
		{
			get
			{
				int? i = intValue;
				if (!i.HasValue)
					i = 0;

				return i.Value;
			}
			set
			{
				intValue = value;
			}
		}
        public long? longValue
        {
            get
            {
                Property p = GetFirst(typeof(long));
                return (p == null ? null : p.longValue);
            }
            set
            {
                PropertyLong p = Property.GetNew(value);
                this.Add(p);
            }
        }
		public long longValueOrDefault
		{
			get
			{
				long? l = longValue;
				if (!l.HasValue)
					l = 0;

				return l.Value;
			}
			set
			{
				longValue = value;
			}
		}
		public double? doubleValue
		{
			get
			{
				Property p = GetFirst(typeof(double));
				return (p == null ? null : p.doubleValue);
			}

			set
			{
				PropertyDouble p = Property.GetNew(value);
				this.Add(p);
			}
		}
		public double doubleValueOrDefault
		{
			get
			{
				double? d = doubleValue;
				if (!d.HasValue)
					d = 0;

				return d.Value;
			}
			set
			{
				doubleValue = value;
			}
		}
		public DateTime? dateTimeValue
		{
			get
			{
				Property p = GetFirst(typeof(DateTime));
				return (p == null ? null : p.dateTimeValue);
//				return (p == null ? DateTime.MinValue : p.dateTimeValue);
			}

			set
			{
				PropertyDateTime p = Property.GetNew(value);
				this.Add(p);
			}
		}
		public DateTime dateTimeValueOrDefault
		{
			get
			{
				DateTime? dt = dateTimeValue;
				if (!dt.HasValue)
					dt = DateTime.MinValue;

				return dt.Value;
			}
			set
			{
				dateTimeValue = value;
			}
		}
		public Guid? guidValue
		{
			get
			{
				Property p = GetFirst(typeof(Guid));
				return (p == null ? null : p.guidValue);
//				return (p == null ? Guid.Empty : p.guidValue);
			}

			set
			{
				PropertyGuid p = Property.GetNew(value);
				this.Add(p);
			}
		}
		public Guid guidValueOrDefault
		{
			get
			{
				Guid? g = guidValue;
				if (!g.HasValue)
					g = Guid.Empty;

				return g.Value;
			}
			set
			{
				guidValue = value;
			}
		}

		//public StringInt stringInt
		//{
		//    get
		//    {
		//        return new StringInt(stringValue, intValue);
		//    }
		//    set
		//    {
		//        if (value == null)
		//        {
		//            intValue = 0;
		//            stringValue = string.Empty;
		//        }
		//        else
		//        {
		//            intValue = value.intValue;
		//            stringValue = value.stringValue;
		//        }
		//    }
		//}

		//public StringDouble stringDouble
		//{
		//    get
		//    {
		//        return new StringDouble(stringValue, doubleValue);
		//    }
		//    set
		//    {
		//        if (value == null)
		//        {
		//            stringValue = string.Empty;
		//            doubleValue = 0d;
		//        }
		//        else
		//        {
		//            stringValue = value.stringValue;
		//            doubleValue = value.doubleValue;
		//        }
		//    }
		//}

		//public StringIntDouble stringIntDouble
		//{
		//    get
		//    {
		//        return new StringIntDouble(stringValue, intValue, doubleValue);
		//    }
		//    set
		//    {
		//        if (value == null)
		//        {
		//            stringValue = string.Empty;
		//            intValue = 0;
		//            doubleValue = 0d;
		//        }
		//        else
		//        {
		//            stringValue = value.stringValue;
		//            intValue = value.intValue;
		//            doubleValue = value.doubleValue;
		//        }
		//    }
		//}

		//public GuidString guidString
		//{
		//    get
		//    {
		//        return this;
		//    }
		//    set
		//    {
		//        if (value == null)
		//        {
		//            guidValue = Guid.Empty;
		//            stringValue = string.Empty;
		//        }
		//        else
		//        {
		//            guidValue = value.guidValue;
		//            stringValue = value.stringValue;
		//        }
		//    }
		//}
		//public GuidStringInt guidStringInt
		//{
		//    get
		//    {
		//        return this;
		//    }
		//    set
		//    {
		//        if (value == null)
		//        {
		//            guidValue = Guid.Empty;
		//            stringValue = string.Empty;
		//            intValue = 0;
		//        }
		//        else
		//        {
		//            guidValue = value.guidValue;
		//            stringValue = value.stringValue;
		//            intValue = value.intValue;
		//        }
		//    }
		//}
		//public GuidStringDouble guidStringDouble
		//{
		//    get
		//    {
		//        return new GuidStringDouble(guidValue, stringValue, doubleValue);
		//    }
		//    set
		//    {
		//        if (value == null)
		//        {
		//            guidValue = Guid.Empty;
		//            stringValue = string.Empty;
		//            doubleValue = 0d;
		//        }
		//        else
		//        {
		//            guidValue = value.guidValue;
		//            stringValue = value.stringValue;
		//            doubleValue = value.doubleValue;
		//        }
		//    }
		//}
		//public GuidStringIntDouble guidStringIntDouble
		//{
		//    get
		//    {
		//        return this;
		//    }
		//    set
		//    {
		//        if (value == null)
		//        {
		//            guidValue = Guid.Empty;
		//            stringValue = string.Empty;
		//            intValue = 0;
		//            doubleValue = 0d;
		//        }
		//        else
		//        {
		//            guidValue = value.guidValue;
		//            stringValue = value.stringValue;
		//            intValue = value.intValue;
		//            doubleValue = value.doubleValue;
		//        }
		//    }
		//}

		public static List<PropertyManager> ConvertDateField(List<PropertyManager> pmWithoutDate, int DateField)
		{
			List<PropertyManager> pmWithDate = new List<PropertyManager>();

			foreach (var p in pmWithoutDate)
			{
				PropertyManager pm = new PropertyManager();
				for (int i = 0; i < p.Count(); ++i)
				{
					Property py = p.GetProperty(i);
					if (i == DateField)
					{
						if (!py.intValue.HasValue)
							continue;

						PropertyDateTime pdt = new PropertyDateTime(py.intValueOrDefault.FromDateKey());
						py = pdt;
					}
					pm.Add(py);
				}

				pmWithDate.Add(pm);
			}

			return pmWithDate;
		}
	}
}
