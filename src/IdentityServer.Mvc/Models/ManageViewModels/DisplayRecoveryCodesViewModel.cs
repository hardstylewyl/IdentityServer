using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.ManageViewModels;

public class DisplayRecoveryCodesViewModel
{
	[Required]
	public IEnumerable<string> Codes { get; set; }
}
