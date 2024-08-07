using IdentityServer.Domain.Entites.Abstractions;

namespace IdentityServer.Domain.Entites;

public class ConfigurationEntry : Entity<long>, IAggregateRoot
{
	public string Key { get; set; }

	public string Value { get; set; }

	public string Description { get; set; }

	public bool IsSensitive { get; set; }
}
