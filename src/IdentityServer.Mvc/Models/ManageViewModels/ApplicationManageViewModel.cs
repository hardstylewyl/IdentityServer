using IdentityServer.Mvc.Models.Common;

namespace IdentityServer.Mvc.Models.ManageViewModels;

public sealed class ApplicationManageViewModel
{
	public IEnumerable<UserApplicationModel> Applications { get; set; }
}
