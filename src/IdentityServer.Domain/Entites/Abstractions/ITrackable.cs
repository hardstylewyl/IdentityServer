namespace IdentityServer.Domain.Entites.Abstractions;

public interface ITrackable
{
	byte[] RowVersion { get; set; }

	DateTimeOffset CreatedDateTime { get; set; }

	DateTimeOffset? UpdatedDateTime { get; set; }
}
