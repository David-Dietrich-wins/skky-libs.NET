using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
	public partial class EquipmentType
	{
		protected class AllEquipmentType
		{
			internal static readonly List<EquipmentType> EquipmentType = All();

			private static List<EquipmentType> All()
			{
				List<EquipmentType> equip = null;
				using (var db = new ObjectsDataContext())
				{
					var list = from eq in db.EquipmentTypes
							   orderby eq.Name
							   select eq;

					equip = list.ToList();

					foreach (var item in list)
						item.Name = item.Name.Trim();
				}

				return equip;
			}
		}

		public static List<EquipmentType> All()
		{
			return AllEquipmentType.EquipmentType;
		}

		public static EquipmentType GetEquipmentTypeFromCode(string code)
		{
			if (!string.IsNullOrEmpty(code))
			{
				var list = from eq in All()
						   where eq.Code == code
						   select eq;

				if (list.Any())
					return list.First();
			}

			return null;
		}
		public static EquipmentType GetEquipmentTypeFromName(string name)
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

		public static EquipmentType GetEquipmentType(string code, string name)
		{
			EquipmentType et = GetEquipmentTypeFromCode(code);
			if (et != null)
				return et;

			return GetEquipmentTypeFromName(name);
		}

		public static string GetEquipmentCode(string name)
		{
			EquipmentType et = GetEquipmentTypeFromName(name);
			if (et != null)
				return et.Code;

			et = GetEquipmentTypeFromCode(name);
			if (et != null)
				return et.Code;

			return string.Empty;
		}
		public static string GetEquipmentName(string code)
		{
			EquipmentType et = GetEquipmentTypeFromCode(code);
			if (et != null)
				return et.Code;

			et = GetEquipmentTypeFromName(code);
			if (et != null)
				return et.Code;

			return string.Empty;
		}
	}
}
