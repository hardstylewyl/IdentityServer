using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.AccountViewModels;

public sealed class VerifyCodeViewModel
{
	[Required]
	public string Provider { get; set; }

	[Required]
	public string Code { get; set; }

	public string ReturnUrl { get; set; } = "/";

	[Display(Name = "记住当前浏览器?")]
	public bool RememberBrowser { get; set; }

	[Display(Name = "记住我")]
	public bool RememberMe { get; set; }
}
