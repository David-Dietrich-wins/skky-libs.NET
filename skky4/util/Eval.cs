using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.util
{
	public static class Eval
	{
		// simulate basic evaluation of arithmetic expression

		public static bool VerifyAllowed(string e)
		{
			string allowed = "0123456789+-*/().,";
			for (int i = 0; i < e.Length; i++)
			{
				if (allowed.IndexOf("" + e[i]) == -1)
				{
					return false;
				}
			}
			return true;
		}

		public static string Evaluate(string e)
		{
			if (string.IsNullOrEmpty(e))
			{
				return "String length is zero";
			}
			if (!VerifyAllowed(e))
			{
				return "The string contains not allowed characters";
			}
			if (e[0] == '-')
			{
				e = "0" + e;
			}
			string res = "";
			try
			{
				res = Calculate(e).ToString();
			}
			catch
			{
				return "The call caused an exception";
			}
			return res;
		}

		public static double Calculate(string e)
		{
			//e = e.Replace(".", ",");
			if (e.IndexOf("(") != -1)
			{
				int a = e.LastIndexOf("(");
				int b = e.IndexOf(")", a);
				double middle = Calculate(e.Substring(a + 1, b - a - 1));
				return Calculate(e.Substring(0, a) + middle.ToString() + e.Substring(b + 1));
			}
			double result = 0;
			string[] plus = e.Split('+');
			if (plus.Length > 1)
			{
				// there were some +
				result = Calculate(plus[0]);
				for (int i = 1; i < plus.Length; i++)
				{
					result += Calculate(plus[i]);
				}
				return result;

			}
			else
			{
				// no +
				string[] minus = plus[0].Split('-');
				if (minus.Length > 1)
				{
					// there were some -
					result = Calculate(minus[0]);
					for (int i = 1; i < minus.Length; i++)
					{
						result -= Calculate(minus[i]);
					}
					return result;

				}
				else
				{
					// no -
					string[] mult = minus[0].Split('*');
					if (mult.Length > 1)
					{
						// there were some *
						result = Calculate(mult[0]);
						for (int i = 1; i < mult.Length; i++)
						{
							result *= Calculate(mult[i]);
						}
						return result;

					}
					else
					{
						// no *
						string[] div = mult[0].Split('/');
						if (div.Length > 1)
						{
							// there were some /
							result = Calculate(div[0]);
							for (int i = 1; i < div.Length; i++)
							{
								result /= Calculate(div[i]);
							}
							return result;

						}
						else
						{
							// no /
							return double.Parse(e);
						}
					}
				}
			}
		}
	}
}
