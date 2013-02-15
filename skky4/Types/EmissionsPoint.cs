using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using skky.Conversions;

namespace skky.Types
{
	[DataContract]
	public class EmissionsPoint
	{
		public EmissionsPoint()
		{
			IsMetric = true;
		}

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

		public EmissionsPoint ConvertToFormat(bool useMetric)
		{
			if (useMetric == this.IsMetric)	// Then they are both either metric or standard.
				return this;

			ConversionBase conv = new KilogramsToPounds();
			EmissionsPoint ep = new EmissionsPoint();
			ep.CH4 = conv.Convert(this.IsMetric, useMetric, this.CH4);
			ep.CO2 = conv.Convert(this.IsMetric, useMetric, this.CO2);
			ep.NOx = conv.Convert(this.IsMetric, useMetric, this.NOx);
			ep.SOx = conv.Convert(this.IsMetric, useMetric, this.SOx);
			ep.H2O = conv.Convert(this.IsMetric, useMetric, this.H2O);

			if (this.Distance != 0)
			{
				conv = new KilometersToMiles();
				ep.Distance = conv.Convert(!this.IsMetric, useMetric, Distance);
			}

			ep.IsMetric = useMetric;

			return ep;
		}

		public static EmissionsPoint CopyEmissionsPoint(EmissionsPoint em)
		{
			EmissionsPoint emnew = new EmissionsPoint();
			if (em != null)
			{
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
		public void setInvalid()
		{
			Distance = -1;
			CO2 = -1;
			CH4 = -1;
			H2O = -1;
			NOx = -1;
			SOx = -1;
		}
		public void MultiplyEmissions(double multiplier)
		{
			CO2 *= multiplier;
			CH4 *= multiplier;
			H2O *= multiplier;
			NOx *= multiplier;
			SOx *= multiplier;
		}
		public void DivideEmissions(double multiplier)
		{
			CO2 /= multiplier;
			CH4 /= multiplier;
			H2O /= multiplier;
			NOx /= multiplier;
			SOx /= multiplier;
		}
		public void Add(EmissionsPoint em)
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
		public static EmissionsPoint Add(EmissionsPoint start, EmissionsPoint end)
		{
			if (start != null && end != null)
			{
				EmissionsPoint emnew = EmissionsPoint.CopyEmissionsPoint(start);
				emnew.Add(end);
				return emnew;
			}

			return new EmissionsPoint();
		}
	}
}
