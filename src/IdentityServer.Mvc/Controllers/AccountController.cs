using System.Security.Claims;
using IdentityServer.Domain.Entites;
using IdentityServer.Domain.Notification;
using IdentityServer.Infrastructure.Identity;
using IdentityServer.Mvc.ConfigurationOptions;
using IdentityServer.Mvc.ConfigurationOptions.ExternalLogin;
using IdentityServer.Mvc.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Mvc.Controllers;

[Authorize]
public sealed class AccountController(
	UserManager userManager,
	SignInManager signInManager,
	IEmailNotification emailSender,
	ISmsNotification smsSender,
	ILogger<AccountController> logger)
	: Controller
{
	#region 账号密码登录

	//
	// GET: /Account/Login
	[HttpGet]
	[AllowAnonymous]
	public IActionResult Login(string returnUrl = "/")
	{
		ViewData["ReturnUrl"] = returnUrl;
		return View();
	}

	//
	// POST: /Account/Login
	[HttpPost]
	[AllowAnonymous]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "/")
	{
		ViewData["ReturnUrl"] = returnUrl;
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var user = await userManager.FindByNameAsync(model.Username);

		// 这不将登录失败计入帐户锁定
		// 要使密码失败触发帐户锁定，请设置 lockoutOnFailure: true
		var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe,
			lockoutOnFailure: true);
		if (result.Succeeded)
		{
			//记录登录成功 清除失败次数
			await signInManager.AddLoginHistoryAsync(user!, LoginMethods.UsernamePassword);
			await userManager.ResetAccessFailedCountAsync(user!);
			return RedirectToLocal(returnUrl);
		}

		if (result.RequiresTwoFactor)
		{
			return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, model.RememberMe });
		}

		if (result.IsLockedOut)
		{
			await signInManager.AddLoginHistoryAsync(user!, LoginMethods.UsernamePassword, loginSuccess: false);
			return View("Lockout");
		}

		//当用户存在并且登录失败记录失败日志
		if (user != null)
		{
			await signInManager.AddLoginHistoryAsync(user!, LoginMethods.UsernamePassword, loginSuccess: false);
		}

		ModelState.AddModelError(string.Empty, "登录失败，账户或者密码错误");
		return View(model);
	}

	#endregion 登录

	#region 外部登录 （External Login）

	//
	// POST: /Account/ExternalLogin
	[HttpPost]
	[AllowAnonymous]
	[ValidateAntiForgeryToken]
	public IActionResult ExternalLogin(string provider, string returnUrl = "/")
	{
		// 请求重定向到外部登录提供程序,根据provider
		var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
		var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
		return Challenge(properties, provider);
	}

	//
	// GET: /Account/ExternalLoginCallback
	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/", string? remoteError = null)
	{
		if (remoteError != null)
		{
			ModelState.AddModelError(string.Empty, $"来自外部提供商的错误: {remoteError}");
			return View("Login");
		}

		var info = await signInManager.GetExternalLoginInfoAsync();
		if (info == null)
		{
			return RedirectToAction("Login");
		}

		var user = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

		// 如果用户已经登录，则使用此外部登录提供程序登录用户。
		var result = await signInManager
			.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
		if (result.Succeeded)
		{
			// 如果登录成功则更新所有身份验证令牌
			await signInManager.UpdateExternalAuthenticationTokensAsync(info);

			//记录登录日志 清除失败次数
			await signInManager.AddLoginHistoryAsync(user!, LoginMethods.ExternalLogin, info.LoginProvider);
			await userManager.ResetAccessFailedCountAsync(user!);
			return RedirectToLocal(returnUrl);
		}

		if (result.RequiresTwoFactor)
		{
			return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
		}

		if (result.IsLockedOut)
		{
			//记录登录失败
			await signInManager.AddLoginHistoryAsync(user!, LoginMethods.ExternalLogin, info.LoginProvider,
				loginSuccess: false);
			return View("Lockout");
		}

		// 如果用户没有帐户，则要求用户创建一个帐户。
		ViewData["ReturnUrl"] = returnUrl;
		ViewData["ProviderDisplayName"] = info.ProviderDisplayName;
		var email = info.Principal.FindFirstValue(ClaimTypes.Email);
		return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email! });
	}

	//
	// POST: /Account/ExternalLoginConfirmation
	[HttpPost]
	[AllowAnonymous]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model,
		string returnUrl = "/")
	{
		ViewData["ReturnUrl"] = returnUrl;
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		// 从外部登录提供者获取有关用户的信息
		var info = await signInManager.GetExternalLoginInfoAsync();
		if (info == null)
		{
			return View("ExternalLoginFailure");
		}

		//如果邮箱被注册了提示
		var user = await userManager.FindByEmailAsync(model.Email);
		if (user != null)
		{
			ModelState.AddModelError(string.Empty, "邮箱已经被注册了，换个试试吧！");
			return View(model);
		}

		//创建账户，使用邮箱作为用户名
		user = new User { UserName = model.Email, Email = model.Email };
		var result = await userManager.CreateAsync(user);
		if (!result.Succeeded)
		{
			AddErrors(result);
			return View(model);
		}

		//TODO：添加声明信息
		//result = await userManager.AddClaimsAsync(user, []);
		//if (!result.Succeeded)
		//{
		//	//添加声明信息失败，删除用户
		//	await userManager.DeleteAsync(user);
		//	AddErrors(result);
		//	return View(model);
		//}

		//保存登录信息
		result = await userManager.AddLoginAsync(user, info);
		if (!result.Succeeded)
		{
			AddErrors(result);
			return View(model);
		}

		await signInManager.SignInAsync(user, isPersistent: false);
		await signInManager.AddLoginHistoryAsync(user, LoginMethods.ExternalLogin, info.LoginProvider);
		//同时更新所有身份验证令牌
		await signInManager.UpdateExternalAuthenticationTokensAsync(info);

		return RedirectToLocal(returnUrl);
	}

	#endregion 外部登录 （External Login）

	#region 双因素登录（2FA）相关

	//
	// GET: /Account/SendCode
	[HttpGet]
	[AllowAnonymous]
	public async Task<ActionResult> SendCode(string returnUrl = "/", bool rememberMe = false)
	{
		var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
		if (user == null)
		{
			return View("Error");
		}

		//检索用户支持的双因素提供商
		var userFactors = await userManager.GetValidTwoFactorProvidersAsync(user);
		var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose })
			.ToList();

		return View(new SendCodeViewModel
			{ Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
	}

	//
	// POST: /Account/SendCode
	[HttpPost]
	[AllowAnonymous]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> SendCode(SendCodeViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View();
		}

		var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
		if (user == null)
		{
			return View("Error");
		}

		//当选择为身份验证器时，转到验证器界面
		if (model.SelectedProvider == "Authenticator")
		{
			return RedirectToAction(nameof(VerifyAuthenticatorCode), new { model.ReturnUrl, model.RememberMe });
		}

		//生成code并发送
		var code = await userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
		if (string.IsNullOrWhiteSpace(code))
		{
			return View("Error");
		}

		//TODO：根据选择的类型去发送code
		var message = "Your security code is: " + code;
		if (model.SelectedProvider == "Email")
		{
			Console.WriteLine("------------------\n" + message + "\n------------------");
			//await emailSender.SendEmailAsync(await userManager.GetEmailAsync(user), "Security Code", message);
		}
		else if (model.SelectedProvider == "Phone")
		{
			Console.WriteLine("------------------\n" + message + "\n------------------");
			//await smsSender.SendSmsAsync(await userManager.GetPhoneNumberAsync(user), message);
		}

		return RedirectToAction(nameof(VerifyCode),
			new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
	}

	//
	// GET: /Account/VerifyCode
	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = "/")
	{
		// 要求用户已经通过用户名/密码或外部登录登录
		var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
		if (user == null)
		{
			return View("Error");
		}

		return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
	}

	//
	// POST: /Account/VerifyCode
	[HttpPost]
	[AllowAnonymous]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		// 以下代码可防止针对双因素(2FA)代码的暴力攻击。
		// 如果用户在指定时间内输入错误代码，则用户帐户将被删除
		// 将被锁定指定的时间。
		var result = await signInManager
			.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
		var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
		if (result.Succeeded)
		{
			await signInManager.AddLoginHistoryAsync(user!, LoginMethods.TwoFactor);
			await userManager.ResetAccessFailedCountAsync(user!);
			return RedirectToLocal(model.ReturnUrl);
		}

		if (result.IsLockedOut)
		{
			await signInManager.AddLoginHistoryAsync(user!, LoginMethods.TwoFactor, loginSuccess: false);
			return View("Lockout");
		}

		await signInManager.AddLoginHistoryAsync(user!, LoginMethods.TwoFactor, loginSuccess: false);
		ModelState.AddModelError(string.Empty, "验证码错误");
		return View(model);
	}

	#endregion 双因素登录（2FA）相关

	#region 注册

	//
	// GET: /Account/Register
	[HttpGet]
	[AllowAnonymous]
	public IActionResult Register(string returnUrl = "/")
	{
		ViewData["ReturnUrl"] = returnUrl;
		return View();
	}

	//
	// POST: /Account/Register
	[HttpPost]
	[AllowAnonymous]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = "/")
	{
		ViewData["ReturnUrl"] = returnUrl;
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var user = new User { UserName = model.Username, Email = model.Email };
		var result = await userManager.CreateAsync(user, model.Password);

		//注册失败
		if (!result.Succeeded)
		{
			AddErrors(result);
			return View(model);
		}

		//添加一些默认的声明信息
		result = await userManager.AddClaimsAsync(user, [
			new(Claims.Gender, ""),
			new(Claims.Address, ""),
			new(Claims.Birthdate, ""),
			new Claim(Claims.Nickname, model.Nickname),
			new Claim(Claims.Name, model.Nickname),
			//使用生成的头像
			new Claim(Claims.Picture, $"https://ui-avatars.com/api/?name={model.Nickname}&background=0D8ABC")
		]);

		//声明信息添加失败，删除用户
		if (!result.Succeeded)
		{
			await userManager.DeleteAsync(user);
			AddErrors(result);
			return View(model);
		}

		//有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=532713
		//发送包含此链接的电子邮件
		//var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		//var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
		//await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
		//    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");

		await signInManager.SignInAsync(user, isPersistent: false);
		await signInManager.AddLoginHistoryAsync(user, LoginMethods.UsernamePassword);
		return RedirectToLocal(returnUrl);
	}

	#endregion 注册

	#region 登出

	//
	// POST: /Account/LogOff
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> LogOff()
	{
		await signInManager.SignOutAsync();
		return RedirectToAction(nameof(HomeController.Index), "Home");
	}

	#endregion 登出

	#region 邮箱确认

	// GET: /Account/ConfirmEmail
	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> ConfirmEmail(string? userId, string? code)
	{
		if (userId == null || code == null)
		{
			return View("Error");
		}

		var user = await userManager.FindByIdAsync(userId);
		if (user == null)
		{
			return View("Error");
		}

		var result = await userManager.ConfirmEmailAsync(user, code);
		return View(result.Succeeded ? "ConfirmEmail" : "Error");
	}

	#endregion 邮箱确认

	#region 找回密码

	//
	// GET: /Account/ForgotPassword
	[HttpGet]
	[AllowAnonymous]
	public IActionResult ForgotPassword()
	{
		return View();
	}

	//
	// POST: /Account/ForgotPassword
	[HttpPost]
	[AllowAnonymous]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var user = await userManager.FindByEmailAsync(model.Email);
		if (user == null || !await userManager.IsEmailConfirmedAsync(user))
		{
			// 不要透露用户不存在或未确认
			return View("ForgotPasswordConfirmation");
		}

		// 有关如何启用帐户确认和密码重置的更多信息，请访问 http://go.microsoft.com/fwlink/?LinkID=532713
		// 发送包含此链接的电子邮件
		//TODO：发生重置密码邮件
		//var code = await userManager.GeneratePasswordResetTokenAsync(user);
		//var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
		//await emailSender.SendAsync(model.Email, "Reset Password",
		//   "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");

		return View("ForgotPasswordConfirmation");
	}

	//
	// GET: /Account/ForgotPasswordConfirmation
	[HttpGet]
	[AllowAnonymous]
	public IActionResult ForgotPasswordConfirmation()
	{
		return View();
	}

	#endregion 找回密码

	#region 重置密码

	//
	// GET: /Account/ResetPassword
	[HttpGet]
	[AllowAnonymous]
	public IActionResult ResetPassword(string? code = null)
	{
		return code == null ? View("Error") : View();
	}

	//
	// POST: /Account/ResetPassword
	[HttpPost]
	[AllowAnonymous]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var user = await userManager.FindByEmailAsync(model.Email);
		if (user == null)
		{
			//不要透露用户不存在
			return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
		}

		//通过code重置密码
		var result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);
		if (result.Succeeded)
		{
			return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
		}

		AddErrors(result);
		return View();
	}

	//
	// GET: /Account/ResetPasswordConfirmation
	[HttpGet]
	[AllowAnonymous]
	public IActionResult ResetPasswordConfirmation()
	{
		return View();
	}

	#endregion 重置密码

	#region 验证器登录方案

	//
	// GET: /Account/VerifyAuthenticatorCode
	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> VerifyAuthenticatorCode(bool rememberMe, string returnUrl = "/")
	{
		//要求用户已经通过用户名/密码或外部登录登录
		var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
		if (user == null)
		{
			return View("Error");
		}

		return View(new VerifyAuthenticatorCodeViewModel { ReturnUrl = returnUrl, RememberMe = rememberMe });
	}

	//
	// POST: /Account/VerifyAuthenticatorCode
	[HttpPost]
	[AllowAnonymous]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> VerifyAuthenticatorCode(VerifyAuthenticatorCodeViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		// 以下代码可防止针对两个因素代码的暴力攻击。
		// 如果用户在指定时间内输入错误代码，则用户帐户将被删除
		// 将被锁定指定的时间。
		var result =
			await signInManager.TwoFactorAuthenticatorSignInAsync(model.Code, model.RememberMe, model.RememberBrowser);
		if (result.Succeeded)
		{
			return RedirectToLocal(model.ReturnUrl);
		}

		if (result.IsLockedOut)
		{
			return View("Lockout");
		}

		ModelState.AddModelError(string.Empty, "验证码错误");
		return View(model);
	}

	//
	// GET: /Account/UseRecoveryCode
	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> UseRecoveryCode(string returnUrl = "/")
	{
		// Require that the user has already logged in via username/password or external login
		var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
		if (user == null)
		{
			return View("Error");
		}

		return View(new UseRecoveryCodeViewModel { ReturnUrl = returnUrl });
	}

	//
	// POST: /Account/UseRecoveryCode
	[HttpPost]
	[AllowAnonymous]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> UseRecoveryCode(UseRecoveryCodeViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(model.Code);
		if (result.Succeeded)
		{
			return RedirectToLocal(model.ReturnUrl);
		}
		else
		{
			ModelState.AddModelError(string.Empty, "验证码错误");
			return View(model);
		}
	}

	#endregion 验证器登录方案

	#region Helpers

	private void AddErrors(IdentityResult result)
	{
		foreach (var error in result.Errors)
		{
			ModelState.AddModelError(string.Empty, error.Description);
		}
	}

	private Task<User?> GetCurrentUserAsync()
	{
		return userManager.GetUserAsync(HttpContext.User);
	}

	private IActionResult RedirectToLocal(string returnUrl)
	{
		if (Url.IsLocalUrl(returnUrl))
		{
			return Redirect(returnUrl);
		}
		else
		{
			return RedirectToAction(nameof(HomeController.Index), "Home");
		}
	}

	#endregion Helpers
}
