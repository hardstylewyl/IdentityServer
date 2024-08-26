using IdentityServer.CrossCuttingConcerns.DateTimes;
using IdentityServer.Domain.Entities;
using IdentityServer.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Persistence.Repositories;

public class RoleRepository : Repository<Role, long>, IRoleRepository
{
	public RoleRepository(IdentityServerDbContext dbContext, IDateTimeProvider dateTimeProvider)
		: base(dbContext, dateTimeProvider)
	{
	}

	public IQueryable<Role> Get(RoleQueryOptions queryOptions)
	{
		var query = GetQueryableSet();

		if (queryOptions.IncludeClaims)
		{
			query = query.Include(x => x.Claims);
		}

		if (queryOptions.IncludeUserRoles)
		{
			query = query.Include(x => x.UserRoles);
		}

		if (queryOptions.IncludeUsers)
		{
			query = query.Include("UserRoles.User");
		}

		if (queryOptions.AsNoTracking)
		{
			query = query = query.AsNoTracking();
		}

		return query;
	}
}
