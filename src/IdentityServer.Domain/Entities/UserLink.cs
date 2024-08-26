using IdentityServer.Domain.Entites.Abstractions;

namespace IdentityServer.Domain.Entites;

public class UserLink : Entity<long>
{
	public string LoginProvider { get; set; }

	public string ProviderKey { get; set; }

	public string? ProviderDisplayName { get; set; }

	public long UserId { get; set; }

	// 建立关联的时间
	public DateTimeOffset LinkTimeOnUtc { get; set; } = DateTimeOffset.UtcNow;

	// 登录名
	public string LoginName { get; set; }
}
