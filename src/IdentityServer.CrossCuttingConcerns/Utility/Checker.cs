using System.Text.RegularExpressions;

namespace IdentityServer.Domain.Core.Utility;

public static class Checker
{
	public static bool IsValidUrl(string url)
	{
		return UrlChecker.UrlRegex.IsMatch(url);
	}
}

internal static partial class UrlChecker
{
	[GeneratedRegex(@"^(https?:\/\/)[^\s]+",
		RegexOptions.IgnoreCase,
		"zh-CN")]
	private static partial Regex _urlRegex();

	internal static readonly Regex UrlRegex = _urlRegex();
}
