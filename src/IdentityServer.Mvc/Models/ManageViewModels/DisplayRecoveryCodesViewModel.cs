using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.ManageViewModels;

public sealed class DisplayRecoveryCodesViewModel
{
	[Required]
	public IEnumerable<string> Codes { get; set; }
}
