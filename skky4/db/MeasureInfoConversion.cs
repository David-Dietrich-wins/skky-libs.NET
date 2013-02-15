using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using skky.Conversions;
using skky.util;

namespace skky.db
{
	public partial class MeasureInfoConversion
	{
		public static readonly List<MeasureInfoConversion> All = privateGetAll();

		public static readonly string CO2InPoundsToTrees = "CO2InPoundsToTrees";
		public static readonly string CO2InKilogramsToTrees = "CO2InKilogramsToTrees";

		private static List<MeasureInfoConversion> privateGetAll()
		{
			using (var db = new ObjectsDataContext())
			{
//				db.DeferredLoadingEnabled = false;	// Kills the web service dispose() if not disabled.
				var list = from conv in db.MeasureInfoConversions
						   orderby conv.Name
						   select conv;

				return list.ToList();
			}
		}

		public static double Convert(string conversionName, double din)
		{
			var list = All.Where(x => x.Name == conversionName);
			if (list == null || list.Count() > 1)
				return -1;

			var first = list.First();

			string expr = string.Format(first.expression, din);
			return skky.util.Eval.Calculate(expr);
		}

		public static double getCO2InTrees(double din, bool isMetric)
		{
			return Convert(isMetric ? CO2InKilogramsToTrees : CO2InPoundsToTrees, din);
		}
	}
}
