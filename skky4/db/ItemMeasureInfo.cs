using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
	public partial class ItemMeasureInfo
	{
		public static List<ItemMeasureInfo> GetAll(int itemId)
		{
			if (itemId < 1)
				throw new Exception("Invalid Item trying to access ItemMeasureInfos.");

			List<ItemMeasureInfo> miList = null;
			using (var db = new ObjectsDataContext())
			{
				db.DeferredLoadingEnabled = false;
				var list = from mi in db.ItemMeasureInfos
						   where mi.idItem == itemId
						   select mi;

				miList = list.ToList();
			}

			if (miList == null)
				miList = new List<ItemMeasureInfo>();

			return miList;
		}
		public static List<MeasureInfo> GetAllForItemId(int itemId)
		{
			if (itemId < 1)
				throw new Exception("Invalid Item trying to access ItemMeasureInfos.");

			List<MeasureInfo> miList = null;
			using (var db = new ObjectsDataContext())
			{
				db.DeferredLoadingEnabled = false;
				var list = from mi in db.ItemMeasureInfos
						   where mi.idItem == itemId
						   select mi.MeasureInfo;

				miList = list.ToList();
			}

			if (miList == null)
				miList = new List<MeasureInfo>();

			return miList;
		}
		public static List<ItemMeasureInfo> GetAllForCustomer(int customerID)
		{
			if (customerID < 1)
				throw new Exception("Invalid Customer trying to access ItemMeasureInfos.");

			List<ItemMeasureInfo> miList = null;
			using (var db = new ObjectsDataContext())
			{
				db.DeferredLoadingEnabled = false;
				var list = from mi in db.ItemMeasureInfos
						   where mi.Item.ItemType.idCustomer==customerID
						   select mi;

				miList = list.ToList();
			}

			if (miList == null)
				miList = new List<ItemMeasureInfo>();

			return miList;
		}
		public static IEnumerable<ItemMeasureInfo> GetFromCustomer(int customerID)
		{
			if (customerID < 1)
				throw new Exception("Invalid Customer trying to access ItemMeasureInfos.");

			using (var db = new ObjectsDataContext())
			{
				//db.DeferredLoadingEnabled = false;
				return from mi in db.ItemMeasureInfos
					   where mi.Item.ItemType.idCustomer == customerID
					   select mi;

			}
		}
		public static IEnumerable<ItemMeasureInfo> GetFromItemId(ObjectsDataContext db, int itemId)
		{
			if (itemId < 1)
				throw new Exception("Invalid Item trying to access ItemMeasureInfos.");

				//db.DeferredLoadingEnabled = false;
			return from mi in db.ItemMeasureInfos
				   where mi.Item.id == itemId
				   select mi;
		}

		public void SaveFromWeb()
		{
			using (var db = new ObjectsDataContext())
			{
				if (this.id > 0)
				{
					var itemMeasureInfo = (from im in db.ItemMeasureInfos
										   where im.id == this.id
										   select im).Single();
					if (itemMeasureInfo != null)
					{
						if (itemMeasureInfo.MeasureInfo == null || this.MeasureInfo == null)
						{
							itemMeasureInfo.MeasureInfo = this.MeasureInfo;
						}
						else
						{
							// Do not overwrite the itemMeasureInfo.MeasureInfo.id field. Everything else though.
							itemMeasureInfo.MeasureInfo.idNamerItemVolume = this.MeasureInfo.idNamerItemVolume;
							itemMeasureInfo.MeasureInfo.idNamerItemTime = this.MeasureInfo.idNamerItemTime;
							itemMeasureInfo.MeasureInfo.idNamerItemRate = this.MeasureInfo.idNamerItemRate;
						}

						itemMeasureInfo.Notes = this.Notes;
					}
				}
				else
				{
					var itemMeasureInfo = from im in db.ItemMeasureInfos
										  where im.idItem == this.idItem
												&& im.idMeasureInfo == this.idMeasureInfo
										  select im;
					if(itemMeasureInfo.Count() < 1)
						db.ItemMeasureInfos.InsertOnSubmit(this);
				}

				db.SubmitChanges();
			}
		}

		public static MeasureInfo findMeasureInfo(int itemMeasureInfoID, int? namerItemRateID, int? namerItemVolumeID, int? namerItemTimeID)
		{
			using (var db = new ObjectsDataContext())
			{
				var measureInfo = (from mi in db.MeasureInfos
								   join im in db.ItemMeasureInfos on mi.id equals im.idMeasureInfo
								   where mi.idNamerItemRate == namerItemRateID
									&& mi.idNamerItemTime == namerItemTimeID
									&& mi.idNamerItemVolume == namerItemVolumeID
									&& im.id == itemMeasureInfoID
								   select mi);
				if(measureInfo.Count() > 0)
					return measureInfo.First();
			}

			return null;
		}
	}
}
