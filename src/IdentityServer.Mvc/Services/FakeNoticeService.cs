using IdentityServer.Domain.Notification;

namespace IdentityServer.Mvc.Services;

public class FakeNoticeService : ISmsNotification, IEmailNotification
{
	public Task SendAsync(ISmsMessage smsMessage, CancellationToken cancellationToken = default)
	{
		Console.WriteLine(smsMessage.Message);
		return Task.CompletedTask;
	}

	public Task SendAsync(IEmailMessage emailMessage, CancellationToken cancellationToken = default)
	{
		Console.WriteLine(emailMessage.Body);
		return Task.CompletedTask;
	}
}
