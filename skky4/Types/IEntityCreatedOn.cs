using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skky.Types
{
	public interface IEntityCreatedOn
	{
		DateTime createdOn { get; set; }
	}
	public interface IEntityUpdatedOn : IEntityCreatedOn
	{
		DateTime updatedOn { get; set; }
	}
}
