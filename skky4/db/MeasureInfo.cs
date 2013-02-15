using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Conversions;

namespace skky.db
{
	public partial class MeasureInfo
	{
		/*
		public MeasureItemEntity GetItem()
		{
			return MeasureItemEntity.MeasureItemList.SingleOrDefault(p => p.id == idMeasureItem);
		}
		public MeasureTimeEntity GetTime()
		{
			return MeasureTimeEntity.MeasureTimeList.SingleOrDefault(p => p.id == idMeasureTime);
		}
		public MeasureUnit GetUnit()
		{
			return MeasureUnit.All.SingleOrDefault(p => p.id == idMeasureUnit);
		}
		public string GetItemName()
		{
			MeasureItemEntity m = GetItem();
			if (m != null)
				return m.Name;

			return string.Empty;
		}
		public string GetTimeName()
		{
			MeasureTimeEntity m = GetTime();
			if (m != null)
				return m.Name;

			return string.Empty;
		}
		public string GetUnitName()
		{
			MeasureUnit m = GetUnit();
			if (m != null)
				return m.Name;

			return string.Empty;
		}

		public string GetItemShortName()
		{
			MeasureItemEntity m = GetItem();
			if (m != null)
				return m.ShortName;

			return string.Empty;
		}
		public string GetTimeShortName()
		{
			MeasureTimeEntity m = GetTime();
			if (m != null)
				return m.ShortName;

			return string.Empty;
		}
		public string GetUnitShortName()
		{
			MeasureUnit m = GetUnit();
			if (m != null)
				return m.ShortName;

			return string.Empty;
		}
		public string GetLongName()
		{
			string str = string.Empty;

			if (GetItem() != null)
				str += GetItemName();

			if (GetUnit() != null)
				str += " " + GetUnitName();

			if (GetTime() != null)
				str += " per " + GetTimeName();

			return str;
		}
		
		public string GetShortName()
		{
			string str = string.Empty;

			if (GetItem() != null && GetItemShortName() == "pp")
				str += "pp";

			if(GetUnit() != null)
				str += GetUnitShortName();

			if (GetItem() != null && GetItemShortName() != "pp")
				str += GetItemShortName();

			if (GetTime() != null)
			{
				str += '/';
				str += GetTimeShortName();
			}

			return str;
		}
		 */

		public static List<MeasureInfo> GetMeasureInfos(int measureInfoID)
		{
		//	if (itemId < 1)
		//		throw new Exception("Invalid Item trying to access ItemMeasureInfos.");

			List<MeasureInfo> miList = null;
			using (var db = new ObjectsDataContext())
			{
				var list = from mi in db.ItemMeasureInfos
						   where mi.idItem == measureInfoID
						   select mi.MeasureInfo;

				miList = list.ToList();
			}

			if (miList == null)
				miList = new List<MeasureInfo>();

			return miList;
		}

		public void SaveFromWeb()
		{
			using (var db = new ObjectsDataContext())
			{
				if (this.id > 0)
				{
					var measureInfo = (from im in db.MeasureInfos
									   where im.id == this.id
									   select im).Single();
					if (measureInfo != null)
					{
						// Do not overwrite the itemMeasureInfo.MeasureInfo.id field. Everything else though.
						measureInfo.idNamerItemRate = this.idNamerItemRate;
						measureInfo.idNamerItemTime = this.idNamerItemTime;
						measureInfo.idNamerItemVolume = this.idNamerItemVolume;
					}
				}
				else
				{
					db.MeasureInfos.InsertOnSubmit(this);
				}

				db.SubmitChanges();
			}
		}
	}
}
