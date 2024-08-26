using IdentityServer.Domain.Entities.Abstractions;

namespace IdentityServer.Domain.Entities;

public class UserClaim : Entity<long>
{
	public string Type { get; set; }
	public string Value { get; set; }

	public User User { get; set; }
}
