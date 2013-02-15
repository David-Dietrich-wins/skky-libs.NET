using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
    public partial class HtlSegment
    {
		public static List<HtlSegment> GetAll()
		{
			using (var db = new ObjectsDataContext())
			{
				var list = from segs in db.HtlSegments
						   select segs;

				return list.ToList();
			}
		}

		public static int AddHtlSegment(int segmentID, string PropertyCode, string PassengerAssoc, string VendorName, string SpecificRate,
                                         string CityCode, string StreetAddress, string City, string State, string ZipCode,
                                         string Country, string PhoneNumber, string FaxNumber, string RoomType,
                                         int NumAdults, int NumRooms)
        {
            using (var db = new ObjectsDataContext())
            {
                HtlSegment htl = new HtlSegment();

				htl.SegmentID = segmentID;
                htl.PropertyCode = PropertyCode ?? string.Empty;
                htl.PassengerAssoc = PassengerAssoc ?? string.Empty;
                htl.VendorName = VendorName ?? string.Empty;
                htl.SpecificRate = SpecificRate ?? string.Empty;
                htl.CityCode = CityCode ?? string.Empty;
                htl.Street1 = StreetAddress ?? string.Empty;
                htl.City = City ?? string.Empty;
                htl.State = State ?? string.Empty;
                htl.ZipCode = ZipCode ?? string.Empty;
                htl.Country = Country ?? string.Empty;
                htl.PhoneNumber = PhoneNumber ?? string.Empty;
                htl.FaxNumber = FaxNumber ?? string.Empty;
                htl.RoomType = RoomType ?? string.Empty;
                htl.NumAdults = NumAdults;
                htl.NumRooms = NumRooms;

                db.HtlSegments.InsertOnSubmit(htl);
                db.SubmitChanges();

                return htl.id;
            }
        }
    }
}
