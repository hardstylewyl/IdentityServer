using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.AccountViewModels;

public class ForgotPasswordViewModel
{
	[Display(Name = "邮箱地址")]
	[Required]
	[EmailAddress]
	public string Email { get; set; }
}
