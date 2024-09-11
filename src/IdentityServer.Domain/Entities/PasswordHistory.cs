using IdentityServer.Domain.Entities.Abstractions;

namespace IdentityServer.Domain.Entities;

public sealed class PasswordHistory : Entity<long>
{
	public long UserId { get; set; }

	public string PasswordHash { get; set; }

	public User User { get; set; }
}
