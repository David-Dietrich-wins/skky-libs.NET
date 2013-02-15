using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
	public class MeasureTimeEntity : MeasureObject
	{
		public static List<MeasureTimeEntity> MeasureTimeList = new List<MeasureTimeEntity>();

		static MeasureTimeEntity()
		{
			MeasureTimeList.Add(new MeasureTimeEntity() { id = 1, Name = "Minute", ShortName = "mn" });
			MeasureTimeList.Add(new MeasureTimeEntity() { id = 2, Name = "Month", ShortName = "mo" });
			MeasureTimeList.Add(new MeasureTimeEntity() { id = 3, Name = "Second", ShortName = "s" });
			MeasureTimeList.Add(new MeasureTimeEntity() { id = 4, Name = "Week", ShortName = "wk" });
			MeasureTimeList.Add(new MeasureTimeEntity() { id = 5, Name = "Year", ShortName = "yr" });
		}

		//public MeasureTime ToDbType()
		//{
		//    MeasureTime mi = new MeasureTime()
		//    {
		//        id = this.id,
		//        Name = this.Name,
		//        ShortName = this.ShortName,
		//    };

		//    return mi;
		//}
	}
}
