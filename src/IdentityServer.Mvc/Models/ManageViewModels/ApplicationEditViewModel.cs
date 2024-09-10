namespace IdentityServer.Mvc.Models.ManageViewModels;

public sealed class ApplicationEditViewModel
{
	public string ClientId { get; set; }
	
	public string ClientSecret { get; set; }
	
	public string ConsentType { get; set; }
	
	public string DisplayName { get; set; }
	
	public string ClientType { get; set; }
	
	public string[] RedirectUris { get; set; } = new string[1];

	public string[] PostLogoutRedirectUris { get; set; } = new string[1];

	// public string[] Permissions { get; set; } = [];

	public string? AllowScopes { get; set; }
	
	public bool RequirePkce { get; set; }
}
