using System;

namespace skky.Types
{
	public interface IEntityCreatedOn
	{
		DateTime createdOn { get; set; }
	}
	public interface IEntityCreatedBy : IEntityCreatedOn
	{
		string createdBy { get; set; }
	}

	public interface IEntityUpdatedOn : IEntityCreatedOn
	{
		DateTime updatedOn { get; set; }
	}
	public interface IEntityUpdatedBy : IEntityUpdatedOn, IEntityCreatedBy
	{
		string updatedBy { get; set; }
	}
}
