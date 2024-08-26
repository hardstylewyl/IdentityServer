using IdentityServer.Domain.Entities.Abstractions;

namespace IdentityServer.Domain.Entities;

public class CustomMigrationHistory : Entity<long>
{
	public string MigrationName { get; set; }
}
