using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.ManageViewModels;

public sealed class ChangeEmailViewModel
{
	[Required]
	[EmailAddress]
	[Display(Name = "邮箱")]
	public string Email { get; set; }
}
