using IdentityServer.Domain.Entites.Abstractions;

namespace IdentityServer.Domain.Entites;

public class OutboxEvent : Entity<long>, IAggregateRoot
{
	public string EventType { get; set; }

	public long TriggeredById { get; set; }

	public string ObjectId { get; set; }

	public string Message { get; set; }

	public bool Published { get; set; }
}
