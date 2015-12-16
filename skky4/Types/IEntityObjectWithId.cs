using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skky.Types
{
	public interface IEntityWithId<TEntityId>
	{
		TEntityId id { get; }
	}

	public interface IEntityWithIntId : IEntityWithId<int>
	{ }

	public interface IEntityWithLongId : IEntityWithId<long>
	{ }

	public interface IEntityWithGuidId : IEntityWithId<Guid>
	{ }

	public interface IEntityWithStringId : IEntityWithId<string>
	{ }
}
