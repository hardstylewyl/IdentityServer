using IdentityServer.Domain.Identity;
using IdentityServer.Domain.ValueObjects;
using IP2Region.Net.XDB;

namespace IdentityServer.Infrastructure.Identity;

public sealed class IP2RegionIpInfoService : IIpInfoService
{
	private readonly Searcher Searcher;

	internal IP2RegionIpInfoService(string libPath)
	{
		Searcher = new(CachePolicy.File, libPath);
	}

	public Task<IpAddressDetails> GetIpInfoAsync(string ipAddress, CancellationToken cancellationToken = default)
	{
		var info = Searcher.Search(ipAddress);
		if (string.IsNullOrEmpty(info))
		{
			return Task.FromResult(IpAddressDetails.None);
		}

		var parts = info.Split('|');

		var country = parts[0];
		var state = parts[2];
		var city = parts[3];
		var carrier = parts[4];
		return Task.FromResult(new IpAddressDetails(ipAddress, city, state, country, carrier, "", ""));
	}
}
