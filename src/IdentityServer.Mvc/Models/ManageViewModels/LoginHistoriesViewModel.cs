using IdentityServer.Domain.Entites;

namespace IdentityServer.Mvc.Models.ManageViewModels;

public sealed class LoginHistoriesViewModel
{
	public IList<UserLoginHistory> LoginHistories { get; set; } = [];
}
