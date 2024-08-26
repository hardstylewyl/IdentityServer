using IdentityServer.Domain.Entities.Abstractions;

namespace IdentityServer.Domain.Entities;

public class ConfigurationEntry : Entity<long>, IAggregateRoot
{
	public string Key { get; set; }

	public string Value { get; set; }

	public string Description { get; set; }

	public bool IsSensitive { get; set; }
}
