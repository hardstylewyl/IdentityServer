namespace IdentityServer.Mvc.ConfigurationOptions.ExternalLogin;

public class GiteeOptions
{
	public bool IsEnabled { get; set; }

	public string ClientId { get; set; }

	public string ClientSecret { get; set; }
}
