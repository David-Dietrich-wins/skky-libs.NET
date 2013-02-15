using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
	public partial class Equipment
	{
		protected class AllEquipment
		{
			public static readonly List<Equipment> Equipment = All();

			private static List<Equipment> All()
			{
				List<Equipment> equip = null;
				using (var db = new ObjectsDataContext())
				{
					var list = from eq in db.Equipments
							   orderby eq.Name
							   select eq;

					equip = list.ToList();
				}

				return equip;
			}
		}

		public static List<Equipment> All()
		{
			return AllEquipment.Equipment;
		}

		public static Equipment GetEquipmentFromName(string name)
		{
			if(!string.IsNullOrEmpty(name))
			{
				var list = from eq in All()
						   where eq.Name == name
						   select eq;

				if (list.Any())
					return list.First();
			}

			return null;
		}
	}
}
