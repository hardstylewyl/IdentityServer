using IdentityServer.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Mvc.Controllers;

public sealed class UserinfoController(UserManager userManager)
	: Controller
{
	[Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
	[HttpGet("~/connect/userinfo"), HttpPost("~/connect/userinfo"), Produces("application/json")]
	public async Task<IActionResult> Userinfo()
	{
		var user = await userManager.FindByIdAsync(User.GetClaim(Claims.Subject)!);
		if (user == null)
		{
			return Challenge(
				authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
				properties: new AuthenticationProperties(new Dictionary<string, string?>
				{
					[OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
					[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
						"指定的访问令牌绑定到不再存在的帐户"
				}));
		}

		var claims = new Dictionary<string, object?>(StringComparer.Ordinal)
		{
			// 注意：“sub”声明是强制声明，必须包含在 JSON 响应中。
			[Claims.Subject] = await userManager.GetUserIdAsync(user)
		};

		if (User.HasScope(Scopes.Email))
		{
			claims[Claims.Email] = await userManager.GetEmailAsync(user);
			claims[Claims.EmailVerified] = await userManager.IsEmailConfirmedAsync(user);
		}

		if (User.HasScope(Scopes.Phone))
		{
			claims[Claims.PhoneNumber] = await userManager.GetPhoneNumberAsync(user);
			claims[Claims.PhoneNumberVerified] = await userManager.IsPhoneNumberConfirmedAsync(user);
		}

		if (User.HasScope(Scopes.Roles))
		{
			claims[Claims.Role] = await userManager.GetRolesAsync(user);
		}

		//用户声明中持有profile的scope
		if (User.HasScope(Scopes.Profile))
		{
			claims[Claims.Picture] = "";
			claims[Claims.Nickname] = "";
			claims[Claims.Birthdate] = "";
			claims[Claims.Gender] = "";
		}

		// 注意：OpenID Connect 规范支持的标准声明的完整列表
		// 可以在这里找到：http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
		return Ok(claims);
	}
}
