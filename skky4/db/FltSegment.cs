using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
    public partial class FltSegment
    {
		public static List<FltSegment> GetAll()
		{
			using (var db = new ObjectsDataContext())
			{
				var list = from segs in db.FltSegments
						   select segs;

				return list.ToList();
			}
		}

		public static void UpdateMiles(int fltSegmentID, int miles)
		{
			using (var db = new ObjectsDataContext())
			{
				var seg = (from s in db.FltSegments
						   where s.id==fltSegmentID
							   select s).Single();

				if(seg != null)
				{
					seg.Miles = miles;
					db.SubmitChanges();
				}
			}
		}

		public static int AddFltSegment(int segmentID, int NumPassengers, string ClassOfService, string EquipmentType,
                                         int Miles, string StartCity, string EndCity)
        {
            using (var db = new ObjectsDataContext())
            {
                FltSegment flt = new FltSegment();

				flt.SegmentID = segmentID;
                flt.NumPassengers = NumPassengers;
                flt.ClassOfService = ClassOfService;
                flt.EquipmentType = EquipmentType;
                flt.Miles = Miles;
                flt.StartCity = StartCity;
                flt.EndCity = EndCity;

                db.FltSegments.InsertOnSubmit(flt);
                db.SubmitChanges();

                return flt.id;
            }
        }
    }
}
