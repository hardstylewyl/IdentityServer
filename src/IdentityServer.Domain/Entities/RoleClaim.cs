using IdentityServer.Domain.Entities.Abstractions;

namespace IdentityServer.Domain.Entities;

public class RoleClaim : Entity<long>
{
	public string Type { get; set; }
	public string Value { get; set; }

	public Role Role { get; set; }
}
