using IdentityServer.Mvc.ConfigurationOptions.ExternalLogin;

namespace IdentityServer.Mvc.ConfigurationOptions;

public sealed class AppSettings
{
	public GiteeOptions Gitee { get; set; }
	public GitHubOptions GitHub { get; set; }
}
