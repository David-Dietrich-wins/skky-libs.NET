using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.db;

namespace skky.Types
{
	[DataContract]
	public class EmissionMarker : City
	{
		[DataMember]
		public double CH4 { get; set; }

		[DataMember]
		public double CO2 { get; set; }

		[DataMember]
		public double H2O { get; set; }

		[DataMember]
		public double NOx { get; set; }

		[DataMember]
		public double SOx { get; set; }

		[DataMember]
		public bool IsMetric { get; set; }

		[DataMember]
		public double Distance { get; set; }

		public static EmissionMarker CopyEmissionMarker(EmissionMarker em)
		{
			EmissionMarker emnew = new EmissionMarker();
			if (em != null)
			{
				CopyCity(em, emnew);

				emnew.Distance = em.Distance;

				emnew.CO2 = em.CO2;
				emnew.CH4 = em.CH4;
				emnew.H2O = em.H2O;
				emnew.NOx = em.NOx;
				emnew.SOx = em.SOx;

				emnew.IsMetric = em.IsMetric;
			}

			return emnew;
		}
		public void Add(EmissionMarker em)
		{
			if (em != null)
			{
				Distance += em.Distance;

				CO2 += em.CO2;
				CH4 += em.CH4;
				H2O += em.H2O;
				NOx += em.NOx;
				SOx += em.SOx;
			}
		}
		public static EmissionMarker Add(EmissionMarker start, EmissionMarker end)
		{
			if (start != null && end != null)
			{
				EmissionMarker emnew = EmissionMarker.CopyEmissionMarker(start);
				emnew.Add(end);
				return emnew;
			}

			return new EmissionMarker();
		}
	}
}
