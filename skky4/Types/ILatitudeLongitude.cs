using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Types
{
	public interface ILatitudeLongitude
	{
		LatitudeLongitude GetLatitudeLongitude();

		double GetLatitude();
		double GetLongitude();
	}
}
