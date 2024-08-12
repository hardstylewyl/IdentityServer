using IdentityServer.CrossCuttingConcerns.DateTimes;
using IdentityServer.Domain.Entites;
using IdentityServer.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Persistence.Repositories;

public class AuditLogEntryRepository : Repository<AuditLogEntry, long>, IAuditLogEntryRepository
{
	public AuditLogEntryRepository(IdentityServerDbContext dbContext, IDateTimeProvider dateTimeProvider)
		: base(dbContext, dateTimeProvider)
	{
	}

	public IQueryable<AuditLogEntry> Get(AuditLogEntryQueryOptions queryOptions)
	{
		var query = GetQueryableSet();

		if (queryOptions.UserId != 0)
		{
			query = query.Where(x => x.UserId == queryOptions.UserId);
		}

		if (!string.IsNullOrEmpty(queryOptions.ObjectId))
		{
			query = query.Where(x => x.ObjectId == queryOptions.ObjectId);
		}

		if (queryOptions.AsNoTracking)
		{
			query = query.AsNoTracking();
		}

		return query;
	}
}
