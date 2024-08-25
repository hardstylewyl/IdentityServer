using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using IdentityServer.Mvc.Models.Common;
using OpenIddict.Abstractions;

namespace IdentityServer.Mvc.Models.ManageViewModels;

public sealed class ManageProfileViewModel : UserProfileModel
{
	[Required(ErrorMessage = "姓名是必须的")] 
	public override string Name { get; set; }

	[Required(ErrorMessage = "昵称是必须的")] 
	public override string Nickname { get; set; }

	public override string? Picture { get; set; }
	public override string? Birthdate { get; set; }
	public override string? Gender { get; set; }
	public override string? Address { get; set; }
	
	public static ManageProfileViewModel CreateForUserClaims(IList<Claim> claims) =>
		new()
		{
			Name = claims.First(x => x.Type == OpenIddictConstants.Claims.Name).Value,
			Nickname = claims.First(x => x.Type == OpenIddictConstants.Claims.Nickname).Value,
			Gender = claims.FirstOrDefault(x => x.Type == OpenIddictConstants.Claims.Gender)?.Value,
			Address = claims.FirstOrDefault(x => x.Type == OpenIddictConstants.Claims.Address)?.Value,
			Birthdate = claims.FirstOrDefault(x => x.Type == OpenIddictConstants.Claims.Birthdate)?.Value
		};
}
