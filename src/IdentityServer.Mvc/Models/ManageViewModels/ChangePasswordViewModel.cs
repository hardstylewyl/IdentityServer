using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.ManageViewModels;

public sealed class ChangePasswordViewModel
{
	[Required]
	[DataType(DataType.Password)]
	[Display(Name = "当前密码")]
	public string OldPassword { get; set; }

	[Required]
	[StringLength(100, ErrorMessage = "{0} 的长度必须至少为 {2} 个字符", MinimumLength = 6)]
	[DataType(DataType.Password)]
	[Display(Name = "新密码")]
	public string NewPassword { get; set; }

	[DataType(DataType.Password)]
	[Display(Name = "确认新密码")]
	[Compare("NewPassword", ErrorMessage = "新密码和确认密码不匹配")]
	public string ConfirmPassword { get; set; }
}
