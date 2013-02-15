using System;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class StringObjArray
	{
		public StringObjArray()
		{ }

		public StringObjArray(string sValue, object[] objectArray)
		{
			stringValue = sValue;
			objArray = objectArray;
		}

		[DataMember]
		public string stringValue { get; set; }

		[DataMember]
		public object[] objArray { get; set; }
	}
}
