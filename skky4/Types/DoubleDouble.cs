using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
    [Serializable]
    [DataContract]
    public class DoubleDouble
    {
		public DoubleDouble()
		{ }
		public DoubleDouble(double dFirst, double dSecond)
		{
			d1 = dFirst;
			d2 = dSecond;
		}

        [DataMember]
        public double d1 { get; set; }
        [DataMember]
        public double d2 { get; set; }
    }
}
