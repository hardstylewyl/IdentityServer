using IdentityServer.Domain.Entites.Abstractions;

namespace IdentityServer.Domain.Entites;

public class CustomMigrationHistory : Entity<long>
{
	public string MigrationName { get; set; }
}
