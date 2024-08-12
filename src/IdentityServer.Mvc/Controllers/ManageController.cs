using IdentityServer.Domain.Entites;
using IdentityServer.Domain.Notification;
using IdentityServer.Infrastructure.Identity;
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
	//
	// GET: /Manage/Index
	[HttpGet]
	public async Task<IActionResult> Index(ManageMessageId? message = null)
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
		var model = new IndexViewModel
		{
			HasPassword = await userManager.HasPasswordAsync(user),
			PhoneNumber = await userManager.GetPhoneNumberAsync(user),
			TwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user),
			UserLoginInfos = await userManager.GetLoginsAsync(user),
			BrowserRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user),
			AuthenticatorKey = await userManager.GetAuthenticatorKeyAsync(user)
		};
		return View(model);
	}

	//
	// POST: /Manage/ResetAuthenticatorKey
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ResetAuthenticatorKey()
	{
		var user = await GetCurrentUserAsync();
		if (user != null)
		{
			await userManager.ResetAuthenticatorKeyAsync(user);
		}
		return RedirectToAction(nameof(Index), "Manage");
	}

	//
	// POST: /Manage/GenerateRecoveryCode
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> GenerateRecoveryCode()
	{
		var user = await GetCurrentUserAsync();
		if (user != null)
		{
			var codes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 5);
			return View("DisplayRecoveryCodes", new DisplayRecoveryCodesViewModel { Codes = codes });
		}
		return View("Error");
	}

	#region 移除/关联 三方账户

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
		if (user == null)
		{
			return View("Error");
		}

		var userLogins = await userManager.GetLoginsAsync(user);
		var schemes = await signInManager.GetExternalAuthenticationSchemesAsync();
		var otherLogins = schemes.Where(auth => userLogins.All(ul => auth.Name != ul.LoginProvider)).ToList();
		//当密码设置或者第三方关联绑定>1才允许解除一个三方身份
		ViewData["ShowRemoveButton"] = user.PasswordHash != null || userLogins.Count > 1;

		return View(new ManageLoginsViewModel
		{
			CurrentLogins = userLogins,
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
		if (user == null)
		{
			return RedirectToAction(nameof(ManageLogins), new { Message = message });
		}

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
			//TODO:记录成功登录日志
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
		var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userManager.GetUserId(User));
		return Challenge(properties, provider);
	}

	//
	// GET: /Manage/LinkLoginCallback
	[HttpGet]
	public async Task<ActionResult> LinkLoginCallback()
	{
		var user = await GetCurrentUserAsync();
		if (user == null)
		{
			return View("Error");
		}

		var info = await signInManager.GetExternalLoginInfoAsync(await userManager.GetUserIdAsync(user));
		if (info == null)
		{
			return RedirectToAction(nameof(ManageLogins), new { Message = ManageMessageId.Error });
		}

		var result = await userManager.AddLoginAsync(user, info);
		var message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;
		return RedirectToAction(nameof(ManageLogins), new { Message = message });
	}

	#endregion 移除/关联 三方账户

	#region 双因素（2FA）启停

	//
	// POST: /Manage/EnableTwoFactorAuthentication
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> EnableTwoFactorAuthentication()
	{
		var user = await GetCurrentUserAsync();
		if (user != null)
		{
			await userManager.SetTwoFactorEnabledAsync(user, true);
			await signInManager.SignInAsync(user, isPersistent: false);
		}
		return RedirectToAction(nameof(Index), "Manage");
	}

	//
	// POST: /Manage/DisableTwoFactorAuthentication
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DisableTwoFactorAuthentication()
	{
		var user = await GetCurrentUserAsync();
		if (user != null)
		{
			await userManager.SetTwoFactorEnabledAsync(user, false);
			await signInManager.SignInAsync(user, isPersistent: false);
		}
		return RedirectToAction(nameof(Index), "Manage");
	}

	#endregion 双因素（2FA）启停

	#region 添加/更换/移除 手机号

	//
	// GET: /Manage/AddPhoneNumber
	public IActionResult AddPhoneNumber()
	{
		return View();
	}

	//
	// POST: /Manage/AddPhoneNumber
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		// 生成token并发送
		var user = await GetCurrentUserAsync();
		var code = await userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
		//发送code到指定手机号
		//await smsSender.SendAsync(model.PhoneNumber, "Your security code is: " + code);
		return RedirectToAction(nameof(VerifyPhoneNumber), new { model.PhoneNumber });
	}

	//
	// GET: /Manage/VerifyPhoneNumber
	[HttpGet]
	public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
	{
		var code = await userManager.GenerateChangePhoneNumberTokenAsync(await GetCurrentUserAsync(), phoneNumber);
		//TODO:发送短信验证电话号码
		//将验证码发送到新的手机号上
		//smsSender.SendAsync()
		return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
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
		if (user != null)
		{
			var result = await userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
			if (result.Succeeded)
			{
				await signInManager.SignInAsync(user, isPersistent: false);
				return RedirectToAction(nameof(Index), new { Message = ManageMessageId.AddPhoneSuccess });
			}
		}

		ModelState.AddModelError(string.Empty, "电话号码验证失败");
		return View(model);
	}

	//
	// GET: /Manage/RemovePhoneNumber
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> RemovePhoneNumber()
	{
		var user = await GetCurrentUserAsync();
		if (user != null)
		{
			var result = await userManager.SetPhoneNumberAsync(user, null);
			if (result.Succeeded)
			{
				await signInManager.SignInAsync(user, isPersistent: false);
				return RedirectToAction(nameof(Index), new { Message = ManageMessageId.RemovePhoneSuccess });
			}
		}
		return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
	}

	#endregion 添加/更换/移除 手机号

	#region 更新/添加 密码

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
		if (user == null)
		{
			return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
		}

		var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
		if (!result.Succeeded)
		{
			AddErrors(result);
			return View(model);
		}

		await signInManager.SignInAsync(user, isPersistent: false);
		return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
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
		if (user == null)
		{
			return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
		}

		var result = await userManager.AddPasswordAsync(user, model.NewPassword);
		if (!result.Succeeded)
		{
			AddErrors(result);
			return View(model);
		}

		await signInManager.SignInAsync(user, isPersistent: false);
		return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
	}

	#endregion 更新/添加 密码

	#region 个人信息

	//
	// GET: /Manage/ManageProfile
	public async Task<IActionResult> ManageProfile()
	{
		var user = await GetCurrentUserAsync();
		if (user == null)
		{
			return View("Error");
		}

		//TODO:获取用户信息
		// userManager.GetUserAsync()
		var model = new ManageProfileViewModel();

		return View(model);
	}

	//
	// POST: /Manage/ChangeProfile
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ChangeProfile(ManageProfileViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		//TODO:修改用户信息
		// userManager.ChangeProfileAsync()
		return RedirectToAction(nameof(ManageProfile));
	}

	#endregion 个人信息

	#region 登录历史

	//
	// GET: /Manage/LoginHistory
	public async Task<IActionResult> LoginHistory()
	{
		var user = await GetCurrentUserAsync();
		if (user == null)
		{
			return View("Error");
		}

		//TODO：获取登录历史
		//user.PasswordHistories
		var model = new LoginHistoriesViewModel();

		return View(model);
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
		AddLoginSuccess,
		ChangePasswordSuccess,
		SetTwoFactorSuccess,
		SetPasswordSuccess,
		RemoveLoginSuccess,
		RemovePhoneSuccess,
		Error
	}

	private Task<User> GetCurrentUserAsync()
	{
		return userManager.GetUserAsync(HttpContext.User)!;
	}

	#endregion Helpers
}
