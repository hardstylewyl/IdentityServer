using IdentityServer.Domain.Entites.Abstractions;

namespace IdentityServer.Domain.Entites;

public class UserToken : Entity<long>
{
	public long UserId { get; set; }

	public string LoginProvider { get; set; }

	public string TokenName { get; set; }

	public string TokenValue { get; set; }
}
