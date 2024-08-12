using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Mvc.Controllers;

public class HomeController(ILogger<HomeController> logger) : Controller
{
	public IActionResult Index()
	{
		return View();
	}

	public IActionResult Privacy()
	{
		return View();
	}

	public IActionResult Error()
	{
		return View();
	}
}
