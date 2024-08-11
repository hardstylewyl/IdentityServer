using IdentityServer.Domain.Entites;

namespace IdentityServer.Domain.Repositories;

public class AuditLogEntryQueryOptions
{
	public long UserId { get; set; }

	public string ObjectId { get; set; }

	public bool AsNoTracking { get; set; }
}

public interface IAuditLogEntryRepository : IRepository<AuditLogEntry, long>
{
	IQueryable<AuditLogEntry> Get(AuditLogEntryQueryOptions queryOptions);
}
