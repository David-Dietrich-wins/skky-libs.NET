using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
	public class MeasureItemEntity : MeasureObject
	{
		public static List<MeasureItemEntity> MeasureItemList = new List<MeasureItemEntity>();

		static MeasureItemEntity()
		{
			MeasureItemList.Add(new MeasureItemEntity() { id = 1, Name = "Cubic", ShortName = "<sup>3</sup>" });
			MeasureItemList.Add(new MeasureItemEntity() { id = 2, Name = "Parts per", ShortName = "pp" });
			MeasureItemList.Add(new MeasureItemEntity() { id = 3, Name = "Square", ShortName = "<sup>2</sup>" });
		}

		//public MeasureItem ToDbType()
		//{
		//    MeasureItem mi = new MeasureItem()
		//    {
		//        id = this.id,
		//        Name = this.Name,
		//        ShortName = this.ShortName,
		//    };

		//    return mi;
		//}
	}
}
