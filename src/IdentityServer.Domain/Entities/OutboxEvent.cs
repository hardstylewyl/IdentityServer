using IdentityServer.Domain.Entities.Abstractions;

namespace IdentityServer.Domain.Entities;

public class OutboxEvent : Entity<long>, IAggregateRoot
{
	public string EventType { get; set; }

	public long TriggeredById { get; set; }

	public string ObjectId { get; set; }

	public string Message { get; set; }

	public bool Published { get; set; }
}
