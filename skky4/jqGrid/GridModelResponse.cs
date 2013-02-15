using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.jqGrid
{
	public class GridModelResponse<T>
	{
		public int page { get; set; }

		public int total { get; set; }

		public int records { get; set; }

		public string sidx { get; set; }

		public string sord { get; set; }

		public IEnumerable<T> rows { get; set; }

		public Dictionary<string, string> userdata { get; set; }
	}
}
