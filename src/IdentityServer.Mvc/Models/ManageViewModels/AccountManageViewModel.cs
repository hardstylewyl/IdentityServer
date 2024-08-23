using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Mvc.Models.ManageViewModels;

public class AccountManageViewModel
{
	//是否有手机号
	public bool HasPhoneNumber => !string.IsNullOrWhiteSpace(PhoneNumber);

	//是否有邮箱
	public bool HasEmail => !string.IsNullOrWhiteSpace(Email);

	//手机号
	public string? PhoneNumber { get; set; }

	//手机号验证
	public bool PhoneNumberConfirmed { get; set; }

	//邮箱
	public string? Email { get; set; }

	//邮箱验证
	public bool EmailConfirmed { get; set; }

	//是否有密码
	public bool HasPassword { get; set; }

	//是否启用双因素
	public bool TwoFactorEnabled { get; set; }

	//指示客户端浏览器是否已被双因素身份验证记住
	public bool BrowserRemembered { get; set; }

	//鉴权器标识
	public string? AuthenticatorKey { get; set; }

	//当前关联的第三方身份提供商
	public IList<UserLoginInfo> UserLoginInfos { get; set; } = [];
}
