using IdentityServer.Domain.Entities.Abstractions;

namespace IdentityServer.Domain.Entities;

public class AuditLogEntry : Entity<long>, IAggregateRoot
{
	public long UserId { get; set; }

	public string Action { get; set; }

	public string ObjectId { get; set; }

	public string Log { get; set; }
}
