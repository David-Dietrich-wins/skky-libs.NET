using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Types
{
    public class SkkySegment
    {
        public int SegmentType { get; set; }

		public string Vendor { get; set; }

		public string VendorTrackingNumber { get; set; }

		public string StartCity { get; set; }

		public string EndCity { get; set; }

		public int SegmentNumber { get; set; }

		public int NumPassengers { get; set; }

		public int PassengerAssoc { get; set; }

		public string Status { get; set; }

		public string StartDate { get; set; }

		public string EndDate { get; set; }

		public string StartTime { get; set; }

		public string EndTime { get; set; }

		public string ClassOfService { get; set; }

		public string EquipmentType { get; set; }

		public string Duration { get; set; }

		public int NumberOfStops { get; set; }

		public int Miles { get; set; }

		public string VendorName { get; set; }

		public bool Manual { get; set; }

		public string ConfirmationNumber { get; set; }

		public string RateCurrency { get; set; }

		public string Rate { get; set; }

		public string AddressStreet { get; set; }

		public string AddressCity { get; set; }

		public string AddressState { get; set; }

		public string AddressZipCode { get; set; }

		public string AddressCountry { get; set; }

		public string PhoneNumber { get; set; }

		public string FaxNumber { get; set; }

		public string RoomType { get; set; }

		public int NumAdults { get; set; }

		public int NumRooms { get; set; }
    }
}
