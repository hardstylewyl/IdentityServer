using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.AccountViewModels;

public class LoginViewModel
{
	[Required(ErrorMessage = "账号是必须的")]
	public string Username { get; set; }

	[DataType(DataType.Password)]
	[Required(ErrorMessage = "密码是必须的")]
	public string Password { get; set; }

	[Display(Name = "记住我")]
	public bool RememberMe { get; set; }
}
