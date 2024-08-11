using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Mvc.Models;

public class ErrorViewModel
{
	[Display(Name = "Error")]
	public string Error { get; set; }

	[Display(Name = "Description")]
	public string ErrorDescription { get; set; }
}
