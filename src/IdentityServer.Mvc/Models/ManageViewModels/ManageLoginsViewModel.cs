using IdentityServer.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Mvc.Models.ManageViewModels;

public sealed class ManageLoginsViewModel
{
	public IList<UserLink> CurrentUserLinks { get; set; }

	public IList<AuthenticationScheme> OtherLogins { get; set; }
}
