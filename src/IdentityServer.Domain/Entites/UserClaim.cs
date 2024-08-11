using IdentityServer.Domain.Entites.Abstractions;

namespace IdentityServer.Domain.Entites;

public class UserClaim : Entity<long>
{
	public string Type { get; set; }
	public string Value { get; set; }

	public User User { get; set; }
}
