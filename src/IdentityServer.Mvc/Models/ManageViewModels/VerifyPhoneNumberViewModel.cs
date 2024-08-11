using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.ManageViewModels;

public class VerifyPhoneNumberViewModel
{
	[Required]
	public string Code { get; set; }

	[Required]
	[Phone]
	[Display(Name = "手机号")]
	public string PhoneNumber { get; set; }
}
