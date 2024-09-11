using IdentityServer.Domain.Entities.Abstractions;

namespace IdentityServer.Domain.Entities;

public sealed class CustomMigrationHistory : Entity<long>
{
	public string MigrationName { get; set; }
}
