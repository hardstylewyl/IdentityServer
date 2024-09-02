using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.AccountViewModels;

public sealed class ResetPasswordViewModel
{
	[Required]
	[EmailAddress]
	public string Email { get; set; }

	[Required]
	[StringLength(100, ErrorMessage = "密码长度至少为6", MinimumLength = 6)]
	[DataType(DataType.Password)]
	public string Password { get; set; }

	[DataType(DataType.Password)]
	[Display(Name = "Confirm password")]
	[Compare("Password", ErrorMessage = "两次密码输入不一致")]
	public string ConfirmPassword { get; set; }

	public string Code { get; set; }
}
