using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Types;

namespace skky.db
{
    public partial class Segment
    {
		public static List<Segment> GetAll()
		{
			using (var db = new ObjectsDataContext())
			{
				var segs = from foundSegs in db.Segments
						   select foundSegs;

				return segs.ToList();
			}
		}

        public static bool DeleteOldSegments(int pnrId)
        {
            using (var db = new ObjectsDataContext())
            {
                var segs = from foundSegs in db.Segments
                           where foundSegs.PnrId == pnrId
                           select foundSegs;
                foreach (Segment seg in segs)
                {
                    if (seg.SegmentEmissions != null)
                    {
                        foreach(var segEmission in seg.SegmentEmissions)
                            db.SegmentEmissions.DeleteOnSubmit(segEmission);
                    }

                    if (seg.HtlSegments != null)
                    {
                        foreach (var htlSegment in seg.HtlSegments)
                            db.HtlSegments.DeleteOnSubmit(htlSegment);
                    }

                    if (seg.FltSegments != null)
                    {
                        foreach (var fltSegment in seg.FltSegments)
                            db.FltSegments.DeleteOnSubmit(fltSegment);
                    }

					db.Segments.DeleteOnSubmit( seg );
                }

                if (segs.Count() > 0)
                    db.SubmitChanges();
            }
            return true;
        }

        public static int AddFltSegment(int pnrId, SkkySegment seg)
        {
            if (0 == seg.Miles)
            {
                seg.Miles = Convert.ToInt32(City.DistanceInMi(seg.StartCity, seg.EndCity));
            }

            DateTime startDateTime = DateTime.Parse(FormatDate(seg.StartDate));
            startDateTime = startDateTime.Add(TimeSpan.Parse(FormatTime(seg.StartTime)));

            DateTime endDateTime = DateTime.Parse(FormatDate(seg.EndDate));
            endDateTime = endDateTime.Add(TimeSpan.Parse(FormatTime(seg.EndTime)));
            
            int fltId = 0;

			int segmentID = AddSegment(pnrId, seg.SegmentType, seg.Vendor, seg.VendorTrackingNumber, seg.SegmentNumber,
                    seg.Status, startDateTime, endDateTime, seg.RateCurrency, seg.Rate, seg.ConfirmationNumber, Convert.ToInt32(seg.Manual));
			if (segmentID > 0)
			{
                fltId = FltSegment.AddFltSegment(segmentID, seg.NumPassengers, seg.ClassOfService, seg.EquipmentType, seg.Miles, seg.StartCity, seg.EndCity);
                int emissionId = SegmentEmission.AddFltSegmentEmission(segmentID, seg.EquipmentType, seg.Miles);
			}

			return fltId;
        }


        public static int AddHtlSegment(int pnrId, SkkySegment seg)
        {
            int hotelId = 0;

            DateTime startDateTime = DateTime.Parse(FormatDate(seg.StartDate));
            startDateTime = startDateTime.Add(TimeSpan.Parse(FormatTime(seg.StartTime)));

            DateTime endDateTime = DateTime.Parse(FormatDate(seg.EndDate));
            endDateTime = endDateTime.Add(TimeSpan.Parse(FormatTime(seg.EndTime)));

            int segmentID = AddSegment(pnrId, seg.SegmentType, seg.Vendor, seg.VendorTrackingNumber, seg.SegmentNumber, seg.Status,
                    startDateTime, endDateTime, seg.RateCurrency, seg.Rate, seg.ConfirmationNumber, Convert.ToInt32(seg.Manual));
            if (segmentID > 0)
            {
                hotelId = HtlSegment.AddHtlSegment(segmentID, seg.VendorTrackingNumber, seg.PassengerAssoc.ToString(), seg.VendorName, seg.Rate, seg.StartCity,
                        seg.AddressStreet, seg.AddressCity, seg.AddressState, seg.AddressZipCode, seg.AddressCountry, seg.PhoneNumber,
                        seg.FaxNumber, seg.RoomType, seg.NumAdults, seg.NumRooms);

                int emissionId = SegmentEmission.AddHtlSegmentEmissions(segmentID, seg.VendorTrackingNumber, endDateTime.Subtract(startDateTime.Date).Days);
            }

            return hotelId;
        }

		public static int AddSegment(int pnrId, int Type, string Vendor, string VendorTrackingNumber,
								 int SegmentNumber, string Status, DateTime StartDateTime, DateTime EndDateTime,
								 string RateCurrency, string Rate, string ConfirmationNumber, int Manual)
		{
            using (var db = new ObjectsDataContext())
            {
                Segment seg = new Segment();
                seg.PnrId = pnrId;
                seg.Type = Type;
                seg.Vendor = Vendor;
                seg.VendorTrackingNumber = VendorTrackingNumber;
                seg.SegmentNumber = SegmentNumber;
                seg.Status = Status;
                seg.StartDateTime = StartDateTime;
                seg.EndDateTime = EndDateTime;
                seg.RateCurrency = RateCurrency;
                seg.Rate = Rate;
                seg.ConfirmationNumber = ConfirmationNumber;
                seg.Manual = Manual;
				//if (true == IsFlight)
				//    seg.FltSegmentId = FltOrHotelId;
				//else
				//    seg.HtlSegmentId = FltOrHotelId;
				//seg.EmissionId = EmissionId;
                db.Segments.InsertOnSubmit(seg);
                db.SubmitChanges();
                return seg.id;
            }
        }

        private static string FormatDate(string theDate)
        {
            if (string.IsNullOrEmpty(theDate))
                return "09/09/99";

			return theDate.Substring(2, 2) + "/" + theDate.Substring(0, 2) + "/" + theDate.Substring(4, 2);
        }

        private static string FormatTime(string theTime)
        {
            if (string.IsNullOrEmpty(theTime))
                return "00:00";

			return theTime.Substring(0, 2) + ":" + theTime.Substring(2, 2);
        }

    }
}
