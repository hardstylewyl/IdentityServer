using IdentityServer.Domain.Entities.Abstractions;

namespace IdentityServer.Domain.Entities;

public class PasswordHistory : Entity<long>
{
	public long UserId { get; set; }

	public string PasswordHash { get; set; }

	public virtual User User { get; set; }
}
