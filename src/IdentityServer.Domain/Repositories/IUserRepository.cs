using IdentityServer.Domain.Entites;

namespace IdentityServer.Domain.Repositories;

public class UserQueryOptions
{
	public bool IncludePasswordHistories { get; set; }
	public bool IncludeClaims { get; set; }
	public bool IncludeUserLinks { get; set; }
	public bool IncludeUserLoginHistories { get; set; }
	public bool IncludeUserRoles { get; set; }
	public bool IncludeRoles { get; set; }
	public bool IncludeTokens { get; set; }
	public bool AsNoTracking { get; set; }
}

public interface IUserRepository : IRepository<User, long>
{
	IQueryable<User> Get(UserQueryOptions queryOptions);
}
