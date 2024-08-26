using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Domain.Entities.Abstractions;

public abstract class Entity<TKey> : IHasKey<TKey>, ITrackable
{
	public TKey Id { get; set; }

	[Timestamp]
	public uint RowVersion { get; set; }

	public DateTimeOffset CreatedDateTime { get; set; } = DateTimeOffset.UtcNow;

	public DateTimeOffset? UpdatedDateTime { get; set; }
}
