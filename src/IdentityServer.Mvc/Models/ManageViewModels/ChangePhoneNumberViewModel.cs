using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.ManageViewModels;

public sealed class ChangePhoneNumberViewModel
{
	[Required]
	[Phone]
	[Display(Name = "手机号")]
	public string PhoneNumber { get; set; }
}
