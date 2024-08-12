using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.AccountViewModels;

public class ExternalLoginConfirmationViewModel
{
	[Display(Name = "请输入邮箱")]
	[Required]
	[EmailAddress(ErrorMessage = "邮箱格式不正确")]
	public string Email { get; set; }

	[Display(Name = "请输入密码")]
	[DataType(DataType.Password)]
	[Required]
	[MinLength(6, ErrorMessage = "密码长度最小为6位")]
	public string Password { get; set; }

	[Display(Name = "请再次输入密码")]
	[DataType(DataType.Password)]
	[Compare(nameof(Password), ErrorMessage = "两次密码不一致")]
	public string ConfirmPassword { get; set; }
}
