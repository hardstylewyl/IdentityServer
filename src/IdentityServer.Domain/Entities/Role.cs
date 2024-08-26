using IdentityServer.Domain.Entities.Abstractions;

namespace IdentityServer.Domain.Entities;

public class Role : Entity<long>, IAggregateRoot
{
	public virtual string? Name { get; set; }

	public virtual string? NormalizedName { get; set; }

	public virtual string ConcurrencyStamp { get; set; }

	public IList<RoleClaim> Claims { get; set; }

	public IList<UserRole> UserRoles { get; set; }
}
