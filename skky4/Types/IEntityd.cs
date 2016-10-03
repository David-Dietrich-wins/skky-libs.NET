using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skky.Types
{
	public interface IEntityid<TEntityId>
	{
		TEntityId id { get; }

		object[] GetObjectArray();
	}

	public interface IEntityIntid : IEntityid<int>
	{ }

	public interface IEntityLongid : IEntityid<long>
	{ }

	public interface IEntityGuidid : IEntityid<Guid>
	{ }

	public interface IEntityStringid : IEntityid<string>
	{ }
}
