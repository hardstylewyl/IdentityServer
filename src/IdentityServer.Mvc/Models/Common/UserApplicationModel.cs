using IdentityServer.Domain.Entities;
using OpenIddict.EntityFrameworkCore.Models;

namespace IdentityServer.Mvc.Models.Common;

public sealed class UserApplicationModel
{
	public UserApplication UserApplication { get; set; }
	public OpenIddictEntityFrameworkCoreApplication OpenIdApplication { get; set; }
}
