using IdentityServer.Domain.Entities.Abstractions;

namespace IdentityServer.Domain.Entities;

public sealed class LocalizationEntry : Entity<long>, IAggregateRoot
{
	public string Name { get; set; }

	public string Value { get; set; }

	public string Culture { get; set; }

	public string Description { get; set; }
}
