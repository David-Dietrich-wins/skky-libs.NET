using System;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class StringStringInt
	{
		public StringStringInt()
		{ }

		public StringStringInt(string sValue, string s2Value, int iValue)
		{
			stringValue = sValue;
			string2Value = s2Value;
			intValue = iValue;
		}
		/*
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (!this.GetType().Equals(obj.GetType()))
				return false;

			StringStringInt rhs = obj as StringStringInt;
			if (rhs == null)
				return false;

			if (stringValue != rhs.stringValue)
				return false;
			if (string2Value != rhs.string2Value)
				return false;
			if (intValue != rhs.intValue)
				return false;

			return base.Equals(obj);
		}
		*/
		[DataMember]
		public string stringValue { get; set; }

		[DataMember]
		public string string2Value { get; set; }

		[DataMember]
		public int intValue { get; set; }
	}
}
