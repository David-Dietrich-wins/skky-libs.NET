using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using skky.Types;

namespace skky.util
{
    /// <summary>
    /// Class to wrap common parsing needs.
    /// </summary>
	public static class Parser
	{
		public const string sClassName = "Parser";

        /// <summary>
        /// Splits a string into an array of strings.
        /// </summary>
        /// <param name="str">The string to split and trim.</param>
        /// <param name="splitChar">The char to use for splitting. Defaults to a comma.</param>
        /// <param name="lowerTheString">True if you want the resulting strings to all be lowercase.</param>
        /// <returns>A string array of split, and optionally trimmed, strings. The return will never be null.</returns>
		public static string[] SplitAndTrimString(string str, char splitChar = ',', bool lowerTheString = false)
		{
			string s;
			string[] ret = null;
			if (!string.IsNullOrEmpty(str))
			{
				ret = str.Split(splitChar);
				if (null != ret)
				{
					for (int i = 0; i < ret.Length; ++i)
					{
						s = ret[i].Trim();

						if (lowerTheString)
							ret[i] = s.ToLower();
						else
							ret[i] = s;
					}
				}
			}

			if (null == ret)
				ret = new string[0];

			return ret;
		}

		public static StringString ParseName(string sName)
		{
			string prefix;
			string first;
			string middle;
			string last;
			string suffix;

			ParseName(sName, out prefix, out first, out middle, out last, out suffix);

			string sFirst = string.Empty;
			sFirst += (prefix ?? string.Empty);
			if (!string.IsNullOrEmpty(first))
			{
				if (!string.IsNullOrWhiteSpace(sFirst))
					sFirst += " ";

				sFirst += first;
			}
			if (!string.IsNullOrEmpty(middle))
			{
				if (!string.IsNullOrWhiteSpace(sFirst))
					sFirst += " ";

				sFirst += middle;
			}

			string sLast = (last ?? string.Empty);
			if (!string.IsNullOrWhiteSpace(suffix))
			{
				if (!string.IsNullOrWhiteSpace(sLast))
					sLast += " ";

				sLast += last;
			}

			return new StringString(sFirst, sLast);
		}

		public static void ParseName(string sName, out string prefix, out string first, out string middle, out string last, out string suffix)
		{
			prefix = string.Empty;
			first = string.Empty;
			middle = string.Empty;
			last = string.Empty;
			suffix = string.Empty;

			// Split on period, commas or spaces, but don't remove from results.
			List<string> parts = Regex.Split(sName, @"(?<=[., ])").ToList();

			// Remove any empty parts
			for (int x = parts.Count - 1; x >= 0; x--)
				if (parts[x].Trim() == "")
					parts.RemoveAt(x);

			if (parts.Count > 0)
			{
				// Might want to add more to this list
				string[] prefixes = { "mr", "mrs", "ms", "dr", "miss", "sir", "madam", "mayor", "president" };

				// If first part is a prefix, set prefix and remove part
				string normalizedPart = parts.First().Replace(".", "").Replace(",", "").Trim().ToLower();
				if (prefixes.Contains(normalizedPart))
				{
					prefix = parts[0].Trim();
					parts.RemoveAt(0);
				}
			}

			if (parts.Count > 0)
			{
				// Might want to add more to this list, or use code/regex for roman-numeral detection
				string[] suffixes = { "jr", "sr", "i", "ii", "iii", "iv", "v", "vi", "vii", "viii", "ix", "x", "xi", "xii", "xiii", "xiv", "xv" };

				// If last part is a suffix, set suffix and remove part
				string normalizedPart = parts.Last().Replace(".", "").Replace(",", "").Trim().ToLower();
				if (suffixes.Contains(normalizedPart))
				{
					suffix = parts.Last().Replace(",", "").Trim();
					parts.RemoveAt(parts.Count - 1);
				}
			}

			// Done, if no more parts
			if (parts.Count == 0)
				return;

			// If only one part left...
			if (parts.Count == 1)
			{
				// If no prefix, assume first name, otherwise last
				// i.e.- "Dr Jones", "Ms Jones" -- likely to be last
				if (prefix == "")
					first = parts.First().Replace(",", "").Trim();
				else
					last = parts.First().Replace(",", "").Trim();
			}

			// If first part ends with a comma, assume format:
			//   Last, First [...First...]
			else if (parts.First().EndsWith(","))
			{
				last = parts.First().Replace(",", "").Trim();
				for (int x = 1; x < parts.Count; x++)
					first += parts[x].Replace(",", "").Trim() + " ";
				first = first.Trim();
			}

			// Otherwise assume format:
			// First [...Middle...] Last

			else
			{
				first = parts.First().Replace(",", "").Trim();
				last = parts.Last().Replace(",", "").Trim();
				for (int x = 1; x < parts.Count - 1; x++)
					middle += parts[x].Replace(",", "").Trim() + " ";
				middle = middle.Trim();
			}
		}
	}
}
