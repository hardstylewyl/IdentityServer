using IdentityServer.Domain.Entites.Abstractions;

namespace IdentityServer.Domain.Entites;

public class User : Entity<long>, IAggregateRoot
{
	public string UserName { get; set; } = null!;

	public string NormalizedUserName { get; set; } = null!;

	public string? Email { get; set; }

	public string? NormalizedEmail { get; set; }

	public bool EmailConfirmed { get; set; }

	public string? PasswordHash { get; set; }

	public string? PhoneNumber { get; set; }

	public bool PhoneNumberConfirmed { get; set; }

	public bool TwoFactorEnabled { get; set; }

	public string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

	public string? SecurityStamp { get; set; }

	public bool LockoutEnabled { get; set; }

	public DateTimeOffset? LockoutEnd { get; set; }

	public int AccessFailedCount { get; set; }

	public IList<UserToken> Tokens { get; set; } = [];

	public IList<UserClaim> Claims { get; set; } = [];

	public IList<UserRole> UserRoles { get; set; } = [];

	public IList<UserLink> UserLinks { get; set; } = [];

	public IList<UserLoginHistory> UserLoginHistories { get; set; } = [];

	public IList<PasswordHistory> PasswordHistories { get; set; } = [];
}
