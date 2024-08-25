using System.Text.RegularExpressions;

namespace IdentityServer.CrossCuttingConcerns.Utility;

public static partial class Checker
{
	public static bool Url(string url) => UrlRegex().IsMatch(url);
	public static bool PhoneNumber(string phoneNumber) => PhoneNumberRegex().IsMatch(phoneNumber);
	public static bool Email(string email) => EmailRegex().IsMatch(email);
	
	[GeneratedRegex(@"^(https?:\/\/)[^\s]+",
		RegexOptions.IgnoreCase,
		"zh-CN")]
	private static partial Regex UrlRegex();
	
	[GeneratedRegex(@"^((13[0-9])|(14[0-9])|(15[0-9])|(17[0-9])|(18[0-9]))\d{8}$",
		RegexOptions.IgnoreCase,
		"zh-CN")]
	private static partial Regex PhoneNumberRegex();
	
	[GeneratedRegex(@"^[A-Za-z0-9\u4e00-\u9fa5]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$",
		RegexOptions.IgnoreCase,
		"zh-CN")]
	private static partial Regex EmailRegex();
}
