using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.util
{
	public static class NumberDisplay
	{
		public const string TwoDecimalsNoZeroWithComma = "#,#.00;;#.#";
		public const string TwoDecimalsNoZeroWithoutComma = "#.00";
		public const string TwoDecimalsWithoutComma = "0.00";
		public const string TwoDecimalsOptionalNoZeroWithComma = "#,#.##";
		public const string TwoDecimalsOptionalNoZeroWithoutComma = "#.##";
		public const string NoDecimalsNoZeroWithComma = "#,#";
		public const string NoDecimalsNoZeroWithoutComma = "#";
		public const string NoDecimalsWithZeroWithComma = "#,#;;0";
		public const string NoDecimalsWithZeroWithoutComma = "#;;0";
		public const string TwoDecimalsMoney = "$#,#.00";
		public const string TwoDecimalsOptionalMoney = "$#,#.##";
		public const string TwoDecimalsPercentage = "#,#.00%";
		public const string TwoDecimalsOptionalPercentage = "#,#.##%";
		public const string OneDecimalsPercentage = "#,#.0%";
		public const string OneDecimalsOptionalPercentage = "#,#.#%";

		public static string ShowTwoDecimalsPlacesIfLessThan(double d, double lessThan)
		{
			if (d == 0)
				return "0";

			if (d < lessThan)
			{
				if (d < 10)
					return d.ToString("0.00");
				else
					return d.ToString("0,0.00");
			}

			return d.ToString("0,0");
		}
	}
}
