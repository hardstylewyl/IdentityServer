using System.Security.Claims;
using IdentityServer.CrossCuttingConcerns.Extensions;
using IdentityServer.Domain.Entites;
using IdentityServer.Infrastructure.Identity;
using IdentityServer.Mvc.ConfigurationOptions.ExternalLogin;
using IdentityServer.Mvc.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Mvc.Controllers;

public sealed class AuthorizationController(
	IOpenIddictApplicationManager applicationManager,
	IOpenIddictAuthorizationManager authorizationManager,
	IOpenIddictScopeManager scopeManager,
	SignInManager signInManager,
	UserManager userManager)
	: Controller
{
	[HttpGet("~/connect/authorize")]
	[HttpPost("~/connect/authorize")]
	[IgnoreAntiforgeryToken]
	public async Task<IActionResult> Authorize()
	{
		var request = HttpContext.GetOpenIddictServerRequest() ??
			throw new InvalidOperationException("无法检索OpenID Connect请求");

		//尝试检索存储在身份验证cookie中的用户主体并重定向到登录
		//在以下情况下，用户代理到登录页面（或到外部提供商）：
		//-如果无法提取用户主体或cookie太旧。
		//-如果客户端应用程序指定了prompt=login。
		//-如果提供了max_age参数，并且身份验证cookie被认为不够“新鲜”。
		var result = await HttpContext.AuthenticateAsync();
		if (!result.Succeeded || request.HasPrompt(Prompts.Login) ||
		   request.MaxAge != null && result.Properties?.IssuedUtc != null &&
			DateTimeOffset.UtcNow - result.Properties.IssuedUtc > TimeSpan.FromSeconds(request.MaxAge.Value))
		{
			//如果客户端应用程序请求无提示认证，
			//返回一个错误，指示用户未登录。
			if (request.HasPrompt(Prompts.None))
			{
				return BuildForbidResult(Errors.LoginRequired, "用户未登录");
			}

			//为了避免无休止的登录->授权重定向，prompt=login标志
			//在重定向用户之前从授权请求有效载荷中删除。
			var prompt = string.Join(" ", request.GetPrompts().Remove(Prompts.Login));

			//提取请求的参数，过滤prompt参数
			var parameters = Request.HasFormContentType ?
				Request.Form.Where(parameter => parameter.Key != Parameters.Prompt).ToList() :
				Request.Query.Where(parameter => parameter.Key != Parameters.Prompt).ToList();

			parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt)));

			//外部身份提供商
			var externalProvider = request.IdentityProvider;

			//未提供内部idp登录
			if (string.IsNullOrWhiteSpace(externalProvider))
			{
				return Challenge(authenticationSchemes: IdentityConstants.ApplicationScheme,
					 properties: new AuthenticationProperties
					 {
						 RedirectUri = Request.PathBase + Request.Path + QueryString.Create(parameters)
					 });
			}

			//不支持的三方Provider 403
			if (!ExternalLoginProvider.IsSupported(externalProvider))
			{
				return BuildForbidResult(Errors.InvalidRequest, "指定的标识的提供程序无效");
			}

			//构建外部登录的props
			var properties = signInManager.ConfigureExternalAuthenticationProperties(
				provider: request.IdentityProvider,
				redirectUrl: Url.Action("ExternalLoginCallback", "Account", new
				{
					ReturnUrl = Request.PathBase + Request.Path + QueryString.Create(parameters)
				}));

			//注意：当在客户端选项中仅注册了一个客户端时，
			//不需要指定颁发者URI或提供程序名称。
			properties.SetString(OpenIddictClientAspNetCoreConstants.Properties.ProviderName, externalProvider);

			//要求openiddict客户端中间件将用户代理重定向到身份提供者。
			return Challenge(properties, OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
		}

		// 检索已登录用户的配置文件
		var user = await userManager.GetUserAsync(result.Principal) ??
			throw new InvalidOperationException("无法检索用户详细信息");

		// 从数据库中检索应用程序详细信息。
		var application = await applicationManager.FindByClientIdAsync(request.ClientId!) ??
			throw new InvalidOperationException("找不到有关调用客户端应用程序的详细信息");

		//检索与用户和调用客户端应用程序相关联的永久授权。
		var authorizations = await authorizationManager.FindAsync(
			subject: await userManager.GetUserIdAsync(user),
			client: (await applicationManager.GetIdAsync(application))!,
			status: Statuses.Valid,
			type: AuthorizationTypes.Permanent,
			scopes: request.GetScopes()).ToListAsync();

		//根据客户端的同意类型（ConsentTypes）执行不同的策略
		switch (await applicationManager.GetConsentTypeAsync(application))
		{
			//如果同意是外部的（例如，当系统管理员授予授权时）
			//如果在数据库中找不到授权，则立即返回错误。
			case ConsentTypes.External when authorizations.Count == 0:
				{
					return BuildForbidResult(Errors.ConsentRequired, "不允许登录的用户访问此客户端应用程序");
				}

			//如果同意类型是隐含的或者如果发现授权，
			//返回授权响应而不显示同意书。
			case ConsentTypes.Implicit:
			case ConsentTypes.External when authorizations.Count != 0:
			case ConsentTypes.Explicit when authorizations.Count != 0 && !request.HasPrompt(Prompts.Consent):
				{
					return await BuildSuccessSigInResult(request, user, authorizations, application);
				}

			// 此时数据库中没有发现授权，必须返回错误
			// 如果客户端应用程序在授权请求中指定prompt=none。
			case ConsentTypes.Explicit when request.HasPrompt(Prompts.None):
			case ConsentTypes.Systematic when request.HasPrompt(Prompts.None):
				{
					return BuildForbidResult(Errors.ConsentRequired, "需要交互用户同意");
				}

			// 在所有其他情况下，呈现同意书。
			default:
				return View(new AuthorizeViewModel
				{
					ApplicationName = await applicationManager.GetLocalizedDisplayNameAsync(application),
					Scope = request.Scope
				});
		}
	}

	[Authorize, FormValueRequired("submit.Accept")]
	[HttpPost("~/connect/authorize"), ValidateAntiForgeryToken]
	public async Task<IActionResult> Accept()
	{
		var request = HttpContext.GetOpenIddictServerRequest() ??
			throw new InvalidOperationException("无法检索 OpenID Connect 请求");

		// 检索已登录用户的个人资料。
		var user = await userManager.GetUserAsync(User) ??
			throw new InvalidOperationException("无法检索用户详细信息");

		// 从数据库中检索应用程序详细信息
		var application = await applicationManager.FindByClientIdAsync(request.ClientId!) ??
			throw new InvalidOperationException("找不到有关调用客户端应用程序的详细信息");

		// 检索与用户和调用客户端应用程序关联的永久授权。
		var authorizations = await authorizationManager.FindAsync(
			subject: await userManager.GetUserIdAsync(user),
			client: (await applicationManager.GetIdAsync(application))!,
			status: Statuses.Valid,
			type: AuthorizationTypes.Permanent,
			scopes: request.GetScopes()).ToListAsync();

		// 注意：在其他操作中已经进行了相同的检查，但会重复进行
		// 这里确保恶意用户不能滥用这个仅 POST 端点
		// 强制它在没有外部授权的情况下返回有效的响应。
		if (authorizations.Count == 0 && await applicationManager.HasConsentTypeAsync(application, ConsentTypes.External))
		{
			return BuildForbidResult(Errors.ConsentRequired, "不允许登录的用户访问此客户端应用程序");
		}

		return await BuildSuccessSigInResult(request, user, authorizations, application);
	}

	// 通知 OpenIddict 授权已被资源所有者拒绝
	// 使用适当的response_mode 将用户代理重定向到客户端应用程序。
	[Authorize, FormValueRequired("submit.Deny")]
	[HttpPost("~/connect/authorize"), ValidateAntiForgeryToken]
	public IActionResult Deny() => Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

	[HttpGet("~/connect/logout")]
	public IActionResult Logout() => View();

	[ActionName(nameof(Logout)), HttpPost("~/connect/logout"), ValidateAntiForgeryToken]
	public async Task<IActionResult> LogoutPost()
	{
		// 要求 ASP.NET Core Identity 删除创建的本地和外部 cookie
		// 当用户代理从外部身份提供者重定向时
		// 成功验证流程后（例如 Google 或 Facebook）。
		await signInManager.SignOutAsync();

		// 返回 SignOutResult 将要求 OpenIddict 重定向用户代理
		// 到客户端应用程序指定的 post_logout_redirect_uri 或
		// 如果未设置，则在身份验证属性中指定 RedirectUri。
		return SignOut(
			authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
			properties: new AuthenticationProperties
			{
				RedirectUri = "/"
			});
	}

	//支持授权码和刷新Token模式获取token
	[HttpPost("~/connect/token"), IgnoreAntiforgeryToken, Produces("application/json")]
	public async Task<IActionResult> Exchange()
	{
		var request = HttpContext.GetOpenIddictServerRequest() ??
			throw new InvalidOperationException("无法检索 OpenID Connect 请求");

		if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
		{
			// 检索存储在授权代码/刷新令牌中的声明主体
			var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

			// 检索与授权码/刷新令牌对应的用户配置文件
			var user = await userManager.FindByIdAsync(result.Principal!.GetClaim(Claims.Subject)!);
			if (user is null)
			{
				return BuildForbidResult(Errors.InvalidGrant, "令牌不再有效");
			}

			// 确保用户仍然可以登录。
			if (!await signInManager.CanSignInAsync(user))
			{
				return BuildForbidResult(Errors.InvalidGrant, "不再允许用户登录");
			}

			var identity = new ClaimsIdentity(result.Principal!.Claims,
				authenticationType: TokenValidationParameters.DefaultAuthenticationType,
				nameType: Claims.Name,
				roleType: Claims.Role);

			// 覆盖主体中存在的用户声明，以防万一
			// 自颁发授权代码/刷新令牌以来已更改。
			identity.SetClaim(Claims.Subject, await userManager.GetUserIdAsync(user))
					.SetClaim(Claims.Email, await userManager.GetEmailAsync(user))
					.SetClaim(Claims.Name, await userManager.GetUserNameAsync(user))
					.SetClaim(Claims.PreferredUsername, await userManager.GetUserNameAsync(user))
					.SetClaims(Claims.Role, [.. (await userManager.GetRolesAsync(user))]);

			identity.SetDestinations(GetDestinations);

			// 返回 SignInResult 将要求 OpenIddict 颁发适当的访问/身份令牌。
			return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
		}

		throw new InvalidOperationException("不支持指定的授权类型");
	}

	private async Task<Microsoft.AspNetCore.Mvc.SignInResult> BuildSuccessSigInResult(OpenIddictRequest request, User user, List<object> authorizations, object application)
	{
		// 创建基于声明的标识，OpenIddict将使用该标识生成令牌。
		var identity = new ClaimsIdentity(
			authenticationType: TokenValidationParameters.DefaultAuthenticationType,
			nameType: Claims.Name,
			roleType: Claims.Role);

		// 添加将保留在令牌中的声明。
		identity.SetClaim(Claims.Subject, await userManager.GetUserIdAsync(user))
				.SetClaim(Claims.Email, await userManager.GetEmailAsync(user))
				.SetClaim(Claims.Name, await userManager.GetUserNameAsync(user))
				.SetClaim(Claims.PreferredUsername, await userManager.GetUserNameAsync(user))
				.SetClaims(Claims.Role, [.. (await userManager.GetRolesAsync(user))]);

		// 注意：在此示例中，授予的范围与请求的范围匹配
		// 但您可能希望允许用户取消选中特定范围。
		// 为此，只需在调用 SetScopes 之前限制范围列表即可。
		identity.SetScopes(request.GetScopes());
		identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

		// 自动（创建永久授权）以避免需要明确同意
		// 用于将来的授权或包含相同范围的令牌请求。
		var authorization = authorizations.LastOrDefault();
		authorization ??= await authorizationManager.CreateAsync(
			identity: identity,
			subject: await userManager.GetUserIdAsync(user),
			client: (await applicationManager.GetIdAsync(application))!,
			type: AuthorizationTypes.Permanent,
			scopes: identity.GetScopes());

		identity.SetAuthorizationId(await authorizationManager.GetIdAsync(authorization));
		identity.SetDestinations(GetDestinations);

		return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
	}

	private ForbidResult BuildForbidResult(string error, string errorDescription)
	{
		return Forbid(
				authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
				properties: new AuthenticationProperties(new Dictionary<string, string?>
				{
					[OpenIddictServerAspNetCoreConstants.Properties.Error] = error,
					[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = errorDescription
				}));
	}

	private static IEnumerable<string> GetDestinations(Claim claim)
	{
		// 注意：默认情况下，声明不会自动包含在访问令牌和身份令牌中。
		// 要允许 OpenIddict 序列化它们，您必须为它们附加一个目的地，该目的地指定
		// 它们是否应该包含在访问令牌、身份令牌或两者中。
		switch (claim.Type)
		{
			case Claims.Name or Claims.PreferredUsername:
				yield return Destinations.AccessToken;

				if (claim.Subject?.HasScope(Scopes.Profile) ?? false)
					yield return Destinations.IdentityToken;

				yield break;

			case Claims.Email:
				yield return Destinations.AccessToken;

				if (claim.Subject?.HasScope(Scopes.Email) ?? false)
					yield return Destinations.IdentityToken;

				yield break;

			case Claims.Role:
				yield return Destinations.AccessToken;

				if (claim.Subject?.HasScope(Scopes.Roles) ?? false)
					yield return Destinations.IdentityToken;

				yield break;

			// 切勿在访问令牌和身份令牌中包含安全标记，因为它是一个秘密值。
			case "AspNet.Identity.SecurityStamp":
				yield break;

			default:
				yield return Destinations.AccessToken;
				yield break;
		}
	}
}
