using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace skky.Types
{
	[DataContract]
	public class Namer
	{
		public enum Type
		{
			None = 0,
			AcreFeet = 1,
			Billions = 2,
			Feet = 3,
			Gallons = 4,
			Kilometers = 5,
			Liters = 6,
			Meters = 7,
			MetricTons = 8,
			Miles = 9,
			Millions = 10,
			Tons = 11,
			SquareFeet = 12,
			CubicFeet = 13,
			SquareYards = 14,
			CubicYards = 15,
			SquareMeters = 16,
			CubicMeters = 17,
			SquareMiles = 18,
			CubicMiles = 19,
			Celsius = 20,
			Fahrenheit = 21,
			Therms = 22,
			SquareKilometers = 23,
			CubicKilometers = 24,
			Trees = 25,
			Pounds = 26,
			Kilograms = 27,
			// Time
			Months = 500,
			Weeks = 501,
			Years = 502,
			Days = 503,
			Hours = 504,
			Minutes = 505,
			Seconds = 506,
			CarbonDioxide = 1000,
			Methane = 1001,
			WaterVapor = 1002,
			Nitrogen = 1003,
			Sulfur = 1004,
		}

		private static List<Namer> namerList;
		static Namer()
		{
			using (var db = new skky.db.ObjectsDataContext())
			{
				var list = from nl in db.NamerItems
						   select new Namer()
						   {
							   NamerType = (Type)nl.id,
							   id = nl.id,
							   Name = nl.Name,
							   ShortName = nl.ShortName,
							   NameSingular = nl.NameSingular,
							   NameHTML = nl.NameHTML,
							   ShortNameHTML = nl.ShortNameHTML,
							   NameSingularHTML = nl.NameSingularHTML,
						   };

				namerList = list.ToList();
				foreach(var item in namerList)
				{
					if (string.IsNullOrEmpty(item.NameSingular))
						item.NameSingular = item.Name;
					if (string.IsNullOrEmpty(item.NameHTML))
						item.NameHTML = item.Name;
					if (string.IsNullOrEmpty(item.NameSingularHTML))
						item.NameSingularHTML = item.NameHTML;

					if (string.IsNullOrEmpty(item.ShortName))
						item.ShortName = item.Name;
					if (string.IsNullOrEmpty(item.ShortNameHTML))
						item.ShortNameHTML = item.NameHTML;
				}
			}
		}

		[DataMember]
		public Type NamerType { get; set; }

		[DataMember]
		public int id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string ShortName { get; set; }

		[DataMember]
		public string NameSingular { get; set; }

		[DataMember]
		public string NameHTML { get; set; }

		[DataMember]
		public string ShortNameHTML { get; set; }

		[DataMember]
		public string NameSingularHTML { get; set; }

		public static Namer getNamer(Type namerType)
		{
			var namer = namerList.Where(x => x.NamerType == namerType);
			if (namer != null && namer.Count() > 0)
				return namer.First();

			return new Namer();
		}
		public static List<Namer> getEmissionTypes()
		{
			return namerList.Where(x => x.id >= 1000 && x.id < 1500).ToList();
		}
	}
}
