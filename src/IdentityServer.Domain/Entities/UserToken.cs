using IdentityServer.Domain.Entities.Abstractions;

namespace IdentityServer.Domain.Entities;

public class UserToken : Entity<long>
{
	public long UserId { get; set; }

	public string LoginProvider { get; set; }

	public string TokenName { get; set; }

	public string TokenValue { get; set; }
}
