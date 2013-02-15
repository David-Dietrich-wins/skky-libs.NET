using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace skky.util
{
	public static class EnvironmentHelper
	{
		public static bool TryExpandEnvironmentVariables(string input, out string output)
		{
			bool result = true;
			output = input;
			MatchCollection matches = Regex.Matches(input, "\\%[^%]*\\%");
			if (matches.Count > 0)
			{
				StringBuilder builder = new StringBuilder();
				if (matches[0].Index > 0)
					builder.Append(input.Substring(0, matches[0].Index));
				foreach (Match match in matches)
				{
					string token = match.Value;
					string value = GetEnvironmentVariable(token);
					builder.Append(value);
					if (value.Length == 0)
					{
						builder.Append(token);
						result = false;
					}
					// check for stuff between the matches and append to the output
					int lastIndex = match.Index + match.Length;
					if (match.NextMatch().Index - 1 > lastIndex)
						builder.Append(input.Substring(lastIndex, match.NextMatch().Index - lastIndex));
				}
				// check for stuff after the last match and add it to the output
				Match lastMatch = matches[matches.Count - 1];
				int lastMatchTail = lastMatch.Index + lastMatch.Length;
				if (lastMatchTail < input.Length)
					builder.Append(input.Substring(lastMatchTail, input.Length - lastMatchTail));
				output = builder.ToString();
			}
			return result;
		}
		private static string GetEnvironmentVariable(string name)
		{
			name = name.Replace("%", "");
			string output = string.Empty;
			if (name.Length > 0)
			{
				output = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
			}
			return output;
		}
	}
}
