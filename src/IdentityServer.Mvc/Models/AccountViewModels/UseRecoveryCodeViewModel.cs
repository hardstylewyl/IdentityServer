using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.AccountViewModels;

public sealed class UseRecoveryCodeViewModel
{
	[Required]
	public string Code { get; set; }

	public string ReturnUrl { get; set; }
}
