using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Conversions
{
	public interface IConversion
	{
		double ConvertToMetric(double units);
		double ConvertToStandard(double units);

		double ConvertToMetricOneUnit();
		double ConvertToStandardOneUnit();
	}
}
