namespace IdentityServer.Domain.Entites.Abstractions;

public interface IHasKey<T>
{
	T Id { get; set; }
}
