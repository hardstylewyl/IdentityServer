using IdentityServer.Domain.Entites.Abstractions;

namespace IdentityServer.Domain.Entites;

public class PasswordHistory : Entity<long>
{
	public long UserId { get; set; }

	public string PasswordHash { get; set; }

	public virtual User User { get; set; }
}
