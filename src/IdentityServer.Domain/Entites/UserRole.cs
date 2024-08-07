using IdentityServer.Domain.Entites.Abstractions;

namespace IdentityServer.Domain.Entites;

public class UserRole : Entity<long>
{
	public long UserId { get; set; }

	public long RoleId { get; set; }

	public User User { get; set; }

	public Role Role { get; set; }
}
