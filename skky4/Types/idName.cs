namespace skky.Types
{
	public class idName
	{
		public int id { get; set; }

		public string name { get; set; }

		public idName()
		{ }

		public idName(int theid, string theName)
		{
			id = theid;
			name = theName;
		}
	}

	public class idNameTypeDesc : idName
	{
		public int type { get; set; }
		public string desc { get; set; }

		public idNameTypeDesc()
		{ }

		public idNameTypeDesc(int theid, string theName, int theType, string theDescription)
			: base(theid, theName)
		{
			type = theType;
			desc = theDescription;
		}
	}
}
