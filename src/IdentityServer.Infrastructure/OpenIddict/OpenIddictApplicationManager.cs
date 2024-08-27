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
	public IAsyncEnumerable<OpenIddictEntityFrameworkCoreApplication> ListAsync(IEnumerable<string> ids,
		CancellationToken cancellationToken = default)
	{
		return this.ListAsync(x => x.Where(y => ids.Contains(y.Id)), cancellationToken);
	}
}
