using IdentityServer.CrossCuttingConcerns.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace IdentityServer.Infrastructure.OpenIddict;

public sealed class OpenIddictApplicationManager(
	IOpenIddictApplicationCache<OpenIddictEntityFrameworkCoreApplication> cache,
	ILogger<OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication>> logger,
	IOptionsMonitor<OpenIddictCoreOptions> options,
	IOpenIddictApplicationStoreResolver resolver)
	: OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication>(cache, logger, options, resolver)
{
	public Task<List<OpenIddictEntityFrameworkCoreApplication>> ListByClientIdAsync(IEnumerable<string> clientIds,
		CancellationToken cancellationToken = default)
	{
		return this.ListAsync(x =>
				x.Where(y => clientIds.Contains(y.ClientId)), cancellationToken)
			.ToListAsync();
	}

	public async Task<OpenIddictEntityFrameworkCoreApplication> ResourceCreateAsync(
		string clientId,
		string clientSecret,
		string displayName,
		CancellationToken cancellationToken = default)
	{
		var descriptor = new OpenIddictApplicationDescriptor
		{
			ClientId = clientId,
			ClientSecret = clientSecret,
			DisplayName = displayName,
			ClientType = OpenIddictConstants.ClientTypes.Confidential,
			Permissions =
			{
				OpenIddictConstants.Permissions.Endpoints.Introspection
			}
		};
		
		return await CreateAsync(descriptor, cancellationToken);
	}
}
