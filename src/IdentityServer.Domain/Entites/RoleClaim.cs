using IdentityServer.Domain.Entites.Abstractions;

namespace IdentityServer.Domain.Entites;

public class RoleClaim : Entity<long>
{
	public string Type { get; set; }
	public string Value { get; set; }

	public Role Role { get; set; }
}
