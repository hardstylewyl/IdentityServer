using IdentityServer.Domain.Entities.Abstractions;

namespace IdentityServer.Domain.Entities;

public sealed class UserApplication : Entity<long>
{
	public long UserId { get; set; }

	public string ClientId { get; set; }
	
	public string ClientSecret { get; set; }
}
