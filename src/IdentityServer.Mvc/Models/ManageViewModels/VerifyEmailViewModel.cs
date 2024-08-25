using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.ManageViewModels;

public sealed class VerifyEmailViewModel
{
	[Required]
	public string Code { get; set; }

	[Required]
	[EmailAddress]
	[Display(Name = "邮箱")]
	public string Email { get; set; }
}
