namespace IdentityServer.Mvc.Models;

public sealed class AuthorizeViewModel
{
	public string? ApplicationName { get; internal set; }
	public string? Scope { get; internal set; }
}
