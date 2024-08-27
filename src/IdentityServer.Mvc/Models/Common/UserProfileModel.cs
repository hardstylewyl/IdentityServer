using System.Security.Claims;
using OpenIddict.Abstractions;

namespace IdentityServer.Mvc.Models.Common;

public class UserProfileModel
{
	public virtual string Name { get; set; }

	public virtual string Nickname { get; set; }

	public virtual string? Picture { get; set; }
	public virtual string? Birthdate { get; set; }
	public virtual string? Gender { get; set; }
	public virtual string? Address { get; set; }


	public IList<Claim> ExtractClaims() =>
	[
		new Claim(OpenIddictConstants.Claims.Name, Name),
		new Claim(OpenIddictConstants.Claims.Nickname, Nickname),
		new Claim(OpenIddictConstants.Claims.Gender, Gender ?? ""),
		new Claim(OpenIddictConstants.Claims.Address, Address ?? ""),
		new Claim(OpenIddictConstants.Claims.Birthdate, Birthdate ?? ""),
	];


}
