using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.Types
{
	public static class EmissionsHelper
	{
		public static readonly Namer CO2 = Namer.getNamer(Namer.Type.CarbonDioxide);
		public static readonly Namer CH4 = Namer.getNamer(Namer.Type.Methane);
		public static readonly Namer H2O = Namer.getNamer(Namer.Type.WaterVapor);
		public static readonly Namer NOx = Namer.getNamer(Namer.Type.Nitrogen);
		public static readonly Namer SOx = Namer.getNamer(Namer.Type.Sulfur);

		private static List<Namer> privateAll = Namer.getEmissionTypes();

		public static Namer GetFromType(string emissionType)
		{
			if (!string.IsNullOrEmpty(emissionType))
			{
				var list = privateAll.Where(x => emissionType.StartsWith(x.Name) || emissionType.StartsWith(x.ShortName));
				if (list != null && list.Count() > 0)
					return list.First();
			}

			return CO2;
		}
		public static Namer.Type GetType(string emissionType)
		{
			return GetFromType(emissionType).NamerType;
		}

		public static IEnumerable<string> GetEmissionTypes()
		{
			return (from nl in privateAll
					   select nl.Name).ToList();
		}

		public static string GetEmissionTypeShortName(string emissionType)
		{
			return GetFromType(emissionType).ShortName;
		}
		public static string GetEmissionTypeLongName(string emissionType)
		{
			return GetFromType(emissionType).Name;
		}
		public static string GetEmissionTypeHTMLName(string emissionType)
		{
			return GetFromType(emissionType).ShortNameHTML;
		}
	}
}
