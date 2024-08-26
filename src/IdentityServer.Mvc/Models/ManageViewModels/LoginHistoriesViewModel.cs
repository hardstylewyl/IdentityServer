using IdentityServer.Domain.Entities;

namespace IdentityServer.Mvc.Models.ManageViewModels;

public sealed class LoginHistoriesViewModel
{
	public IList<UserLoginHistory> LoginHistories { get; set; } = [];
}
