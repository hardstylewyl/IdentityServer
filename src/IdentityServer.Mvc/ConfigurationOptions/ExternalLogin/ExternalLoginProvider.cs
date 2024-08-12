using System.Collections.Immutable;

namespace IdentityServer.Mvc.ConfigurationOptions.ExternalLogin;

public static class ExternalLoginProvider
{
	public const string GitHub = nameof(GitHub);
	public const string Gitee = nameof(Gitee);

	private static readonly ImmutableHashSet<string> SupportedProviders =
		ImmutableHashSet.Create(StringComparer.Ordinal, [
			Gitee,
			GitHub
		]);

	public static bool IsSupported(string provider) =>
		SupportedProviders.Contains(provider);
}
