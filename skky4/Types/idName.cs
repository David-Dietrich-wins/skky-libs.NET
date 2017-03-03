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
}
