namespace IdentityServer.Domain.Notification;

public interface IWebNotification<T>
{
	Task SendAsync(T message, CancellationToken cancellationToken = default);
}
