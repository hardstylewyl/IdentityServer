using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.AccountViewModels;

public sealed class RegisterViewModel
{
	[Required]
	[StringLength(20, MinimumLength = 6, ErrorMessage = "账号长度6-20位")]
	public string Username { get; set; }

	[Required]
	[StringLength(12, MinimumLength = 1, ErrorMessage = "昵称长度1-12位")]
	public string Nickname { get; set; }

	[Required]
	[EmailAddress(ErrorMessage = "必须为邮箱格式")]
	public string Email { get; set; }

	[DataType(DataType.Password)]
	[Required]
	[MinLength(6, ErrorMessage = "密码长度最小为6位")]
	public string Password { get; set; }

	[DataType(DataType.Password)]
	[Compare(nameof(Password), ErrorMessage = "两次密码不一致")]
	public string ConfirmPassword { get; set; }
}
