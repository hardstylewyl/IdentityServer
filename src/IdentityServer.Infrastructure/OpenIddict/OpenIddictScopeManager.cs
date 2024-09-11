using IdentityServer.CrossCuttingConcerns.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace IdentityServer.Infrastructure.OpenIddict;

public sealed class OpenIddictScopeManager(
	IOpenIddictScopeCache<OpenIddictEntityFrameworkCoreScope> cache,
	ILogger<OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope>> logger,
	IOptionsMonitor<OpenIddictCoreOptions> options,
	IOpenIddictScopeStoreResolver resolver)
	: OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope>(cache, logger, options, resolver)
{
	public Task<List<OpenIddictEntityFrameworkCoreScope>> ListByNameAsync(IEnumerable<string> scopeNames,
		CancellationToken cancellationToken = default)
	{
		return this.ListAsync(x =>
				x.Where(y => scopeNames.Contains(y.Name)), cancellationToken)
			.ToListAsync();
	}
}
