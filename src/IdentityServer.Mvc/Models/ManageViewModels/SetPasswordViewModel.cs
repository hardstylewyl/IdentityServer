using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.ManageViewModels;

public sealed class SetPasswordViewModel
{
	[Required]
	[StringLength(100, ErrorMessage = "{0} 的长度必须至少为 {2} 个字符", MinimumLength = 6)]
	[DataType(DataType.Password)]
	[Display(Name = "New password")]
	public string NewPassword { get; set; }

	[DataType(DataType.Password)]
	[Display(Name = "Confirm new password")]
	[Compare("NewPassword", ErrorMessage = "新密码和确认密码不匹配")]
	public string ConfirmPassword { get; set; }
}
