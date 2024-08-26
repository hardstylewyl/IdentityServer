using IdentityServer.CrossCuttingConcerns.Utility;
using IdentityServer.Domain.Entites;
using IdentityServer.Domain.Notification;
using IdentityServer.Infrastructure.Identity;
using IdentityServer.Mvc.Models.Common;
using IdentityServer.Mvc.Models.ManageViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Mvc.Controllers;

[Authorize]
public sealed class ManageController(
	UserManager userManager,
	SignInManager signInManager,
	IEmailNotification emailSender,
	ISmsNotification smsSender,
	ILogger<ManageController> logger)
	: Controller
{
	#region 账户管理

	//
	// GET: /Manage/AccountManage
	[HttpGet]
	public async Task<IActionResult> AccountManage(ManageMessageId? message = null)
	{
		ViewData["StatusMessage"] =
			message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
			: message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
			: message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
			: message == ManageMessageId.Error ? "An error has occurred."
			: message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
			: message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
			: "";

		var user = await GetCurrentUserAsync();
		var userLogins = await userManager.GetLoginsAsync(user);
		var model = new AccountManageViewModel
		{
			PhoneNumber = await userManager.GetPhoneNumberAsync(user),
			PhoneNumberConfirmed = await userManager.IsPhoneNumberConfirmedAsync(user),
			Email = await userManager.GetEmailAsync(user),
			EmailConfirmed = await userManager.IsEmailConfirmedAsync(user),
			HasPassword = await userManager.HasPasswordAsync(user),
			TwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user),
			BrowserRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user),
			AuthenticatorKey = await userManager.GetAuthenticatorKeyAsync(user),
			CurrentUserLogins = userLogins,
		};
		return View(model);
	}

	#endregion 账户管理

	#region 鉴权器

	//
	// POST: /Manage/ResetAuthenticatorKey
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ResetAuthenticatorKey()
	{
		var user = await GetCurrentUserAsync();
		await userManager.ResetAuthenticatorKeyAsync(user);

		return RedirectToAction(nameof(AccountManage), "Manage");
	}

	//
	// POST: /Manage/GenerateRecoveryCode
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> GenerateRecoveryCode()
	{
		var user = await GetCurrentUserAsync();

		var codes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 5);
		if (codes == null)
		{
			return  View("Error");
		}
		return View("DisplayRecoveryCodes", new DisplayRecoveryCodesViewModel { Codes = codes });
	}

	#endregion 鉴权器

	#region 三方账户 移除/关联

	//
	//GET: /Manage/ManageLogins
	[HttpGet]
	public async Task<IActionResult> ManageLogins(ManageMessageId? message = null)
	{
		ViewData["StatusMessage"] =
			message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
			: message == ManageMessageId.AddLoginSuccess ? "The external login was added."
			: message == ManageMessageId.Error ? "An error has occurred."
			: "";

		var user = await GetCurrentUserAsync();

		var userLinks = await userManager.GetUserLinksAsync(user);
		var schemes = await signInManager.GetExternalAuthenticationSchemesAsync();
		var otherLogins = schemes.Where(auth => userLinks.All(ul => auth.Name != ul.LoginProvider)).ToList();
		//当密码设置或者第三方关联绑定>1才允许解除一个三方身份
		ViewData["ShowRemoveButton"] = user.PasswordHash != null || userLinks.Count > 1;

		return View(new ManageLoginsViewModel
		{
			CurrentUserLinks = userLinks,
			OtherLogins = otherLogins
		});
	}

	//
	// POST: /Manage/RemoveLogin
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel account)
	{
		ManageMessageId? message = ManageMessageId.Error;
		var user = await GetCurrentUserAsync();

		//当密码设置或者第三方关联绑定>1才允许解除一个三方身份
		var logins = await userManager.GetLoginsAsync(user);
		var hasPassword = await userManager.HasPasswordAsync(user);
		if (!hasPassword && logins.Count <= 1)
		{
			return RedirectToAction(nameof(ManageLogins), new { Message = message });
		}

		//解除一个第三方关联
		var result = await userManager.RemoveLoginAsync(user, account.LoginProvider, account.ProviderKey);
		if (result.Succeeded)
		{
			await signInManager.SignInAsync(user, isPersistent: false);
			message = ManageMessageId.RemoveLoginSuccess;
		}

		return RedirectToAction(nameof(ManageLogins), new { Message = message });
	}

	//
	// POST: /Manage/LinkLogin
	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult LinkLogin(string provider)
	{
		// 请求重定向到外部登录提供程序以链接当前用户的登录
		var redirectUrl = Url.Action("LinkLoginCallback", "Manage");
		var properties =
			signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userManager.GetUserId(User));
		return Challenge(properties, provider);
	}

	//
	// GET: /Manage/LinkLoginCallback
	[HttpGet]
	public async Task<ActionResult> LinkLoginCallback()
	{
		var user = await GetCurrentUserAsync();

		var info = await signInManager.GetExternalLoginInfoAsync(await userManager.GetUserIdAsync(user));
		if (info == null)
		{
			return RedirectToAction(nameof(ManageLogins), new { Message = ManageMessageId.Error });
		}

		//获取三方用户信息
		var externalProfile = ExternalProfileModel.CreateForExternalLoginInfo(info);
		if (externalProfile == null)
		{
			return RedirectToAction(nameof(ManageLogins), new { Message = ManageMessageId.Error });
		}

		//添加关联信息
		var result = await userManager.AddUserLinkAsync(user, externalProfile.CreateUserLink());
		var message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;
		return RedirectToAction(nameof(ManageLogins), new { Message = message });
	}

	#endregion 移除/关联 三方账户

	#region 双因素（2FA）启动/停止

	//
	// POST: /Manage/EnableTwoFactorAuthentication
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> EnableTwoFactorAuthentication()
	{
		var user = await GetCurrentUserAsync();

		await userManager.SetTwoFactorEnabledAsync(user, true);
		await signInManager.SignInAsync(user, isPersistent: false);

		AddAlert(AlertType.Success, "启用成功");
		return RedirectToAction(nameof(AccountManage), "Manage");
	}

	//
	// POST: /Manage/DisableTwoFactorAuthentication
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DisableTwoFactorAuthentication()
	{
		var user = await GetCurrentUserAsync();

		await userManager.SetTwoFactorEnabledAsync(user, false);
		await signInManager.SignInAsync(user, isPersistent: false);
		
		AddAlert(AlertType.Success, "关闭成功");
		return RedirectToAction(nameof(AccountManage), "Manage");
	}

	#endregion 双因素（2FA）启停

	#region 手机号 添加/更换/移除

	//
	// GET: /Manage/ChangePhoneNumber
	public IActionResult ChangePhoneNumber()
	{
		return View();
	}

	//
	// POST: /Manage/ChangePhoneNumber
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ChangePhoneNumber(ChangePhoneNumberViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var phoneNumber = model.PhoneNumber.Trim();
		//验证格式
		if (!Checker.PhoneNumber(phoneNumber))
		{
			return View("Error");
		}

		// 生成token并发送
		var user = await GetCurrentUserAsync();
		var code = await userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
		//发送code到指定手机号
		//await smsSender.SendAsync(model.PhoneNumber, "Your security code is: " + code);
		Console.WriteLine($"-------------\n验证吗为：{code}\n-----------------");
		AddAlert(AlertType.Success, "验证码发送成功");
		return RedirectToAction(nameof(VerifyPhoneNumber), new { phoneNumber });
	}

	//
	// GET: /Manage/VerifyPhoneNumber
	[HttpGet]
	public IActionResult VerifyPhoneNumber(string phoneNumber)
	{
		return View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
	}

	//
	// POST: /Manage/VerifyPhoneNumber
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var user = await GetCurrentUserAsync();
		var result = await userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
		if (result.Succeeded)
		{
			await signInManager.SignInAsync(user, isPersistent: false);
			return RedirectToAction(nameof(AccountManage), new { Message = ManageMessageId.AddPhoneSuccess });
		}

		ModelState.AddModelError(string.Empty, "验证码输入有误");
		return View(model);
	}

	//
	// GET: /Manage/RemovePhoneNumber
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> RemovePhoneNumber()
	{
		var user = await GetCurrentUserAsync();

		var result = await userManager.SetPhoneNumberAsync(user, null);
		if (result.Succeeded)
		{
			await signInManager.SignInAsync(user, isPersistent: false);
			return RedirectToAction(nameof(AccountManage), new { Message = ManageMessageId.RemovePhoneSuccess });
		}

		return RedirectToAction(nameof(AccountManage), new { Message = ManageMessageId.Error });
	}

	#endregion 添加/更换/移除 手机号

	#region 邮箱 更换/验证

	//
	// GET: /Manage/VerifyPhoneNumber
	[HttpGet]
	public IActionResult ChangeEmail()
	{
		return View();
	}

	//
	// POST: /Manage/ChangeEmail
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var email = model.Email.Trim();
		//验证格式
		if (!Checker.Email(email))
		{
			return View("Error");
		}

		var user = await GetCurrentUserAsync();
		var code = await userManager.GenerateChangeEmailTokenAsync(user, email);
		//TODO:发送code到指定邮箱
		//emailSender.SendAsync()
		Console.WriteLine($"-------------\n验证吗为：{code}\n-----------------");
		AddAlert(AlertType.Success, "验证码发送成功");
		return RedirectToAction(nameof(VerifyEmail), new { email });
	}

	//
	// GET: /Manage/ConfirmEmail
	[HttpGet]
	public async Task<IActionResult> ConfirmEmail(string email)
	{
		//验证格式
		if (string.IsNullOrWhiteSpace(email))
		{
			return View("Error");
		}

		var user = await GetCurrentUserAsync();
		var code = await userManager.GenerateChangeEmailTokenAsync(user, email);
		//TODO:发送code到指定邮箱
		//emailSender.SendAsync()
		Console.WriteLine($"-------------\n验证吗为：{code}\n-----------------");
		AddAlert(AlertType.Success, "验证码发送成功");
		return RedirectToAction(nameof(VerifyEmail), new { email });
	}

	//
	// GET: /Manage/VerifyEmail
	[HttpGet]
	public IActionResult VerifyEmail(string email)
	{
		return View(new VerifyEmailViewModel { Email = email });
	}

	//
	// POST: /Manage/VerifyEmail
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var user = await GetCurrentUserAsync();
		var result = await userManager.ChangeEmailAsync(user, model.Email, model.Code);
		if (result.Succeeded)
		{
			await signInManager.SignInAsync(user, isPersistent: false);
			return RedirectToAction(nameof(AccountManage), new { Message = ManageMessageId.AddEmailSuccess });
		}

		ModelState.AddModelError(string.Empty, "验证码输入有误");
		return View(model);
	}

	#endregion

	#region 密码 更新/添加

	//
	// GET: /Manage/ChangePassword
	[HttpGet]
	public IActionResult ChangePassword()
	{
		return View();
	}

	//
	// POST: /Manage/ChangePassword
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var user = await GetCurrentUserAsync();
		
		var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
		if (!result.Succeeded)
		{
			AddErrors(result);
			return View(model);
		}

		await signInManager.SignInAsync(user, isPersistent: false);
		return RedirectToAction(nameof(AccountManage), new { Message = ManageMessageId.ChangePasswordSuccess });
	}

	//
	// GET: /Manage/SetPassword
	[HttpGet]
	public IActionResult SetPassword()
	{
		return View();
	}

	//
	// POST: /Manage/SetPassword
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var user = await GetCurrentUserAsync();
		
		var result = await userManager.AddPasswordAsync(user, model.NewPassword);
		if (!result.Succeeded)
		{
			AddErrors(result);
			return View(model);
		}

		await signInManager.SignInAsync(user, isPersistent: false);
		return RedirectToAction(nameof(AccountManage), new { Message = ManageMessageId.SetPasswordSuccess });
	}

	#endregion 更新/添加 密码

	#region 个人资料 查看/修改

	//
	// GET: /Manage/ManageProfile
	[HttpGet]
	public async Task<IActionResult> ManageProfile()
	{
		var claims = await userManager.GetClaimsAsync(await GetCurrentUserAsync());
		var model = ManageProfileViewModel.CreateForUserClaims(claims);
		return View(model);
	}

	//
	// POST: /Manage/ManageProfile
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ManageProfile(ManageProfileViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var user =  await GetCurrentUserAsync();
		var oldClaims = await userManager.GetClaimsAsync(user);
		//修改用户信息 更新UserClaims
		var newClaims =  model.ExtractClaims();
		foreach (var claim in newClaims)
		{
			//找出需要修改的claim 类型相同value不同
			var diffClaim = oldClaims.FirstOrDefault(x => x.Type == claim.Type && x.Value != claim.Value);
			if (diffClaim != null)
			{
				await userManager.ReplaceClaimAsync(user, diffClaim, claim);
			}
		}
		
		AddAlert(AlertType.Success, "更新用户信息成功");;
		return View(model);
	}

	#endregion 个人资料

	#region 第三方应用

	//
	// GET: /Manage/Application
	[HttpGet]
	public async Task<IActionResult> Application()
	{
		return View();
	}

	#endregion 第三方应用

	#region 登录历史 查看

	//
	// GET: /Manage/LoginHistory
	public async Task<IActionResult> LoginHistory()
	{
		var user = await GetCurrentUserAsync();
		var loginHistories = await signInManager.GetLoginHistoriesAsync(user);
		return View(new LoginHistoriesViewModel(){ LoginHistories = loginHistories });
	}

	#endregion 登录历史

	#region Helpers

	private void AddErrors(IdentityResult result)
	{
		foreach (var error in result.Errors)
		{
			ModelState.AddModelError(string.Empty, error.Description);
		}
	}

	public enum ManageMessageId
	{
		AddPhoneSuccess,
		AddEmailSuccess,
		AddLoginSuccess,
		ChangePasswordSuccess,
		SetTwoFactorSuccess,
		SetPasswordSuccess,
		RemoveLoginSuccess,
		RemovePhoneSuccess,
		Error
	}

	private async Task<User> GetCurrentUserAsync()
	{
		return await userManager.GetUserAsync(HttpContext.User) ??
		       throw new InvalidOperationException("Can't get user.");
	}

	private void AddAlert(AlertType type, string message)
	{
		new AlertModel { Type = type, Message = message }
			.WriteTempData(TempData);
	}

	private void AddDelayRedirect(string url, uint delay = 1500)
	{
		if (!Url.IsLocalUrl(url))
		{
			return;
		}

		ActionModel.Redirect(url, delay)
			.WriteTempData(TempData);
	}

	#endregion Helpers
}
