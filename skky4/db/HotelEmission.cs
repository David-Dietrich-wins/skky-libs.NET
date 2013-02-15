using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Types;

namespace skky.db
{
    public partial class HotelEmission
    {
        private static readonly HotelEmission defaultEmissions = GetEmissions("default");

        public static HotelEmission GetEmissions(string hotelPropertyCode)
        {
            HotelEmission queryResult = null;
            using (var db = new ObjectsDataContext())
            {
                var result = from emissions in db.HotelEmissions
                             where emissions.PropertyCode == hotelPropertyCode.ToLower()
                             select emissions;
                if (result.Count() > 0)
                    queryResult = result.First();
            }
            return queryResult;
        }

		public static HotelEmission DefaultEmissions
		{
			get
			{
				return defaultEmissions;
			}
		}

		public static List<PropertyManager> GetCO2ReportByVendor(string vendor)
		{
			using (var db = new ObjectsDataContext())
			{
				var list = (from emissions in db.TotalHotelEmissions
							where emissions.TotalCO2 > 0
							where emissions.VendorName == vendor
							select new PropertyManager
							{
								stringValue = vendor,
								intValue = emissions.NumRooms,
								doubleValue = emissions.TotalCO2 ?? 0.0
							});

				return list.ToList();
			}
		}
	}
}
