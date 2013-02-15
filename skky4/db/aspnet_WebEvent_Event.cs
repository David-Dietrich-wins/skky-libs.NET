using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
	public partial class aspnet_WebEvent_Event
	{
		public static aspnet_WebEvent_Event GetEvent(string eventID)
		{
			using (var db = new ASPNetDbDataContext())
			{
				var item = from we in db.aspnet_WebEvent_Events
						   where we.EventId == eventID
						   select we;

				return item.SingleOrDefault();
			}
		}

		public static List<aspnet_WebEvent_Event> GetWebEvents()
		{
			using (var db = new ASPNetDbDataContext())
			{
				var list = from we in db.aspnet_WebEvent_Events
						   orderby we.EventTime descending
							  select we;

				return list.ToList();
			}
		}
		public static List<aspnet_WebEvent_Event> GetWebEvents(string eventType, string keyword, int count)
		{
			using (var db = new ASPNetDbDataContext())
			{
				var results = from we in db.aspnet_WebEvent_Events
							  let et = eventType
							  where et == "All" ||
								  (et == "Exceptions" && we.EventCode < 100000) ||
								  (et == "Login" && we.EventCode == 100001)
							  let kw = keyword
							  where (kw == null || kw == "" || we.Message.Contains(keyword) || we.Details.Contains(keyword))
							  orderby we.EventTime descending
							  select we;


				return results.Take(count).ToList();
			}
		}
	}
}
