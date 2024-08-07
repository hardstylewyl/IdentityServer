using IdentityServer.Domain.Entites.Abstractions;

namespace IdentityServer.Domain.Entites;

public class LocalizationEntry : Entity<long>, IAggregateRoot
{
	public string Name { get; set; }

	public string Value { get; set; }

	public string Culture { get; set; }

	public string Description { get; set; }
}
