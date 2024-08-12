using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.AccountViewModels;

public class UseRecoveryCodeViewModel
{
	[Required]
	public string Code { get; set; }

	public string ReturnUrl { get; set; }
}
