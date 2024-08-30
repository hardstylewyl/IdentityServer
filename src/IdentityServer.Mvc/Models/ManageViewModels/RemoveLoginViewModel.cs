namespace IdentityServer.Mvc.Models.ManageViewModels;

public sealed class RemoveLoginViewModel
{
	public string LoginProvider { get; set; }
	public string ProviderKey { get; set; }
}
