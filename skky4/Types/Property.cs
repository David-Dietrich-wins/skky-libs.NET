using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.util;

namespace skky.Types
{
	[DataContract]
	public abstract class Property
	{
		protected Property()
		{ }

		public virtual System.Type PropertyType
		{
			get
			{
                return GetObjectType();
			}

			private set
			{
				throw new Exception("Cannot set Property Type.");
			}
		}

		public virtual object GetObject()
		{
			throw new Exception("Property cannot return an object.");
		}
        public virtual System.Type GetObjectType()
        {
            return typeof(object);
        }

        protected virtual string GetString()
		{
			object o = GetObject();
			if(o == null)
				return null;

			return o.ToString();
		}
		protected virtual void SetString(string s)
		{
			throw new Exception("Property cannot set a string value.");
		}
		protected virtual int? GetInt()
		{
			string s = GetString();
			if(s == null)
				return null;

			return s.ToInteger();
		}
		protected virtual void SetInt(int? i)
		{
			throw new Exception("Property cannot set an int value.");
		}
		protected virtual long? GetLong()
		{
			string s = GetString();
			if (s == null)
				return null;

			return s.ToLong();
		}
		protected virtual void SetLong(long? l)
		{
			throw new Exception("Property cannot set a long value.");
		}
		protected virtual double? GetDouble()
		{
			string s = GetString();
			if (s == null)
				return null;

			return s.ToDouble();
		}
		protected virtual void SetDouble(double? s)
		{
			throw new Exception("Property cannot set a double value.");
		}
		protected virtual DateTime? GetDateTime()
		{
			string s = GetString();
			if (s == null)
				return null;

			return s.ToDateTime();
		}
		protected virtual void SetDateTime(DateTime? s)
		{
			throw new Exception("Property cannot set a DateTime value.");
		}
		protected virtual Guid? GetGuid()
		{
			string s = GetString();
			if (s == null)
				return null;

			return new Guid(s);
		}
		protected virtual void SetGuid(Guid? s)
		{
			throw new Exception("Property cannot set a Guid value.");
		}

		public virtual bool IsValueGreaterThan(Property p)
		{
			throw new Exception("Invalid Property call for IsValueGreaterThan().");
		}
		public virtual bool IsValueLessThan(Property p)
		{
			throw new Exception("Invalid Property call for IsValueLessThan().");
		}
		public virtual bool IsValueEqualTo(Property p)
		{
			throw new Exception("Invalid Property call for IsValueEqualTo().");
		}

		public static string GetFormatString(System.Type p, bool isLongFormat)
		{
			if (IsDoubleType(p) && isLongFormat)
				return "#,###.00";
			else if (IsNumberType(p))
				return "#,#";
			else if (IsDateTimeType(p))
				return "d";

			return string.Empty;
		}
		public string GetFormatString(bool isLongFormat)
		{
			return GetFormatString(PropertyType, isLongFormat);
		}
		public static bool IsNumberType(System.Type t)
		{
			return IsIntType(t) || IsDoubleType(t) || IsLongType(t);
		}
		public static bool IsDateTimeType(System.Type t)
		{
			return t == typeof(DateTime);
		}
		public static bool IsDoubleType(System.Type t)
		{
			return t == typeof(Double);
		}
		public static bool IsIntType(System.Type t)
		{
			return t == typeof(int);
		}
        public static bool IsLongType(System.Type t)
        {
            return t == typeof(long);
        }
		public static bool IsGuidType(System.Type t)
		{
			return t == typeof(Guid);
		}
		public static bool IsStringType(System.Type t)
		{
			return t == typeof(string);
		}

		public bool IsNumberType()
		{
			return IsNumberType(PropertyType);
		}
		public bool IsDateTimeType()
		{
			return IsDateTimeType(PropertyType);
		}
		public bool IsDoubleType()
		{
			return IsDoubleType(PropertyType);
		}
		public bool IsIntType()
		{
			return IsIntType(PropertyType);
		}
		public bool IsGuidType()
		{
			return IsGuidType(PropertyType);
		}
		public bool IsStringType()
		{
			return IsStringType(PropertyType);
		}

		public string stringValue
		{
			get
			{
				return GetString();
			}
			set
			{
				SetString(value);
			}
		}
		public int? intValue
		{
			get
			{
				return GetInt();
			}
			set
			{
				SetInt(value);
			}
		}
		public int intValueOrDefault
		{
			get
			{
				return GetInt() ?? 0;
			}
			set
			{
				SetInt(value);
			}
		}
        public long? longValue
        {
            get
            {
                return GetLong();
            }
            set
            {
                SetLong(value);
            }
        }
		public long longValueOrDefault
		{
			get
			{
				return GetLong() ?? 0;
			}
			set
			{
				SetLong(value);
			}
		}
		public double? doubleValue
		{
			get
			{
				return GetDouble();
			}

			set
			{
				SetDouble(value);
			}
		}
		public double doubleValueOrDefault
		{
			get
			{
				return GetDouble() ?? 0;
			}
			set
			{
				SetDouble(value);
			}
		}
		public DateTime? dateTimeValue
		{
			get
			{
				return GetDateTime();
			}

			set
			{
				SetDateTime(value);
			}
		}
		public DateTime dateTimeValueOrDefault
		{
			get
			{
				DateTime? dt = GetDateTime();
				if (!dt.HasValue)
					return DateTime.MinValue;

				return dt.Value;
			}
			set
			{
				SetDateTime(value);
			}
		}
		public Guid? guidValue
		{
			get
			{
				return GetGuid();
			}

			set
			{
				SetGuid(value);
			}
		}
		public Guid guidValueOrDefault
		{
			get
			{
				Guid? g = GetGuid();
				if (!g.HasValue)
					return Guid.Empty;

				return g.Value;
			}
			set
			{
				SetGuid(value);
			}
		}

		public static Property GetNewProperty(System.Type t)
		{
            if (IsDateTimeType(t))
                return new PropertyDateTime();
//            else if (IsDateTimeType(t))
//                return new PropertyDateTime();
            else if (IsDoubleType(t))
                return new PropertyDouble();
			else if (IsLongType(t))
				return new PropertyLong();
			else if (IsIntType(t))
                return new PropertyInt();
            else if (IsGuidType(t))
                return new PropertyGuid();
            else if (IsStringType(t))
                return new PropertyString();

			throw new Exception("Trying to create unknown Property type: " + t.ToString());
		}

		public static PropertyString GetNew(string s)
		{
			return new PropertyString(s);
		}
		public static PropertyInt GetNew(int? s)
		{
			return new PropertyInt(s);
		}
        public static PropertyLong GetNew(long? l)
        {
            return new PropertyLong(l);
        }
		public static PropertyDouble GetNew(double? s)
		{
			return new PropertyDouble(s);
		}
		public static PropertyDateTime GetNew(DateTime? s)
		{
			return new PropertyDateTime(s);
		}
		public static PropertyGuid GetNew(Guid? s)
		{
			return new PropertyGuid(s);
		}
	}
}
