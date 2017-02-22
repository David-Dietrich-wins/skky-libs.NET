using System;

namespace skky4.Models
{
	/// <summary>
	/// Creates a .NET enum attribute designator with optional Groups.
	/// Every enum with this annotation will be exported and available to JavaScript.
	/// Usage over an enum Type:
	/// [JavascriptEnum("Admin", "Public", "Group3")]
	/// </summary>
	public class JavascriptEnumAttribute : Attribute
	{
		public string[] Groups { get; set; }

		public JavascriptEnumAttribute(params string[] groups)
		{
			Groups = groups;
		}
	}
}