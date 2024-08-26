namespace IdentityServer.Domain.Entities.Abstractions;

public interface IHasKey<T>
{
	T Id { get; set; }
}
