using IdentityServer.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Mvc.Controllers;

public class HomeController(ILogger<HomeController> logger) : Controller
{
	[HttpGet]
	public IActionResult Index()
	{
		// return RedirectToAction("Login","Account");
		return View();
	}

	[HttpPost]
	public IActionResult Index(int id, string redirect = "/")
	{
		
		switch (id)
		{
			case 1:
				TempData[AlertModel.Key] = AlertModel.Success("成功提示");
				break;
			case 2:
				TempData[AlertModel.Key] = AlertModel.Error("错误提示");
				break;
			case 3:
				TempData[AlertModel.Key] = AlertModel.Warning("警告提示");
				break;
			case 4:
				TempData[AlertModel.Key] = AlertModel.Toast("吐司");
				break;
			case 5:
				TempData[AlertModel.Key] = AlertModel.Success("准备跳转");
				TempData[ActionModel.Key] = ActionModel.Redirect(redirect);
				break;
		}

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
