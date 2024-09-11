using IdentityServer.Domain.Entities.Abstractions;

namespace IdentityServer.Domain.Entities;

public sealed class Role : Entity<long>, IAggregateRoot
{
	public string Name { get; set; }

	public string NormalizedName { get; set; }
	
	public string ConcurrencyStamp { get; set; }

	public IList<RoleClaim> Claims { get; set; }

	public IList<UserRole> UserRoles { get; set; }
}
