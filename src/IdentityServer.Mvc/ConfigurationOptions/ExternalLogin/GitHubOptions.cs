namespace IdentityServer.Mvc.ConfigurationOptions.ExternalLogin;

public sealed class GitHubOptions
{
	public bool IsEnabled { get; set; }

	public string ClientId { get; set; }

	public string ClientSecret { get; set; }
}
