using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
    public partial class SegmentEmission
    {
        public static int AddFltSegmentEmission(int segmentID, string aircraftModel, int miles)
        {
            using (var db = new ObjectsDataContext())
            {
                SegmentEmission emission = new SegmentEmission();
				emission.SegmentID = segmentID;
                AirlineEmission airlineEmission = AirlineEmission.GetEmissions(aircraftModel);
                emission.kgCO2 = miles * airlineEmission.CO2permile;
                emission.kgCH4 = miles * airlineEmission.CH4permile;
                emission.kgNOx = miles * airlineEmission.NOxpermile;
                emission.kgH2O = miles * airlineEmission.H2Opermile;
                db.SegmentEmissions.InsertOnSubmit(emission);
                db.SubmitChanges();
                return emission.id;
            }
        }

		public static int AddHtlSegmentEmissions(int segmentID, string propertyCode, int numNights)
        {
            using (var db = new ObjectsDataContext())
            {
                SegmentEmission emission = new SegmentEmission();
				emission.SegmentID = segmentID;
				HotelEmission hotelEmission = HotelEmission.GetEmissions(propertyCode);
                emission.kgCO2 = numNights * hotelEmission.CO2pernight;
                emission.kgCH4 = numNights * hotelEmission.CH4pernight;
                emission.kgNOx = numNights * hotelEmission.NOxpernight;
                emission.kgH2O = numNights * hotelEmission.H2Opernight;
                db.SegmentEmissions.InsertOnSubmit(emission);
                db.SubmitChanges();
                return emission.id;
            }
        }
    }
}
