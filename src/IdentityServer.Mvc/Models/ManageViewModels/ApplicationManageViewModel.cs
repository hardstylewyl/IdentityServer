using OpenIddict.EntityFrameworkCore.Models;

namespace IdentityServer.Mvc.Models.ManageViewModels;

public sealed class ApplicationManageViewModel
{
	public List<OpenIddictEntityFrameworkCoreApplication> Applications { get; set; }
}
