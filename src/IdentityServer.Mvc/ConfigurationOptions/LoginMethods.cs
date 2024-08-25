namespace IdentityServer.Mvc.ConfigurationOptions;

public static class LoginMethods
{
	//用户名+密码
	public const string UsernamePassword = nameof(UsernamePassword);
	//使用外部身份登录
	public const string ExternalLogin = nameof(ExternalLogin);
	//双因素登录登录 2FA
	public const string TwoFactor = nameof(TwoFactor);
}
