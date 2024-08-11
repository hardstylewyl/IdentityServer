using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models.ManageViewModels;

public class AddPhoneNumberViewModel
{
	[Required]
	[Phone]
	[Display(Name = "手机号")]
	public string PhoneNumber { get; set; }
}
