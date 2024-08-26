namespace IdentityServer.Domain.Entites.Abstractions;

public interface ITrackable
{
	//byte[] RowVersion { get; set; }

	//https://www.npgsql.org/efcore/modeling/concurrency.html?q=rowversion&tabs=data-annotations
	uint RowVersion { get; set; }

	DateTimeOffset CreatedDateTime { get; set; }

	DateTimeOffset? UpdatedDateTime { get; set; }
}
