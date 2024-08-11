using IdentityServer.CrossCuttingConcerns.DateTimes;
using IdentityServer.Domain.Entites;
using IdentityServer.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Persistence.Repositories;

public class UserRepository : Repository<User, long>, IUserRepository
{
	public UserRepository(IdentityServerDbContext dbContext, IDateTimeProvider dateTimeProvider)
		: base(dbContext, dateTimeProvider)
	{
	}

	public IQueryable<User> Get(UserQueryOptions queryOptions)
	{
		var query = GetQueryableSet();

		if (queryOptions.IncludePasswordHistories)
		{
			query = query.Include(x => x.PasswordHistories);
		}

		if (queryOptions.IncludeTokens)
		{
			query = query.Include(x => x.Tokens);
		}

		if (queryOptions.IncludeClaims)
		{
			query = query.Include(x => x.Claims);
		}

		if (queryOptions.IncludeUserLinks)
		{
			query = query.Include(x => x.UserLinks);
		}

		if (queryOptions.IncludeUserLoginHistories)
		{
			query = query.Include(x => x.UserLoginHistories);
		}

		if (queryOptions.IncludeUserRoles)
		{
			query = query.Include(x => x.UserRoles);
		}

		if (queryOptions.IncludeRoles)
		{
			query = query.Include("UserRoles.Role");
		}

		if (queryOptions.AsNoTracking)
		{
			query = query = query.AsNoTracking();
		}

		return query;
	}
}
