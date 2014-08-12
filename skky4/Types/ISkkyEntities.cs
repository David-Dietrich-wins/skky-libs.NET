using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using skky.jqGrid;
using skky.util;

namespace skky.Types
{
	public interface ISkkyEntities
	{
		DateTime SyncTime { get; }
		string UserName { get; }
	}
}
