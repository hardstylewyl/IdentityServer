using IdentityServer.Domain.ValueObjects;

namespace IdentityServer.Domain.Identity;

public interface IIpInfoService
{
	Task<IpAddressDetails> GetIpInfoAsync(string ipAddress, CancellationToken cancellationToken = default);
}
