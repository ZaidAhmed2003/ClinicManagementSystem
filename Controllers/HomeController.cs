using ClinicManagementSystem.Data;
using ClinicManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ClinicManagementSystem.Controllers
{
	public class HomeController(ApplicationDbContext context) : Controller
	{
		private readonly ApplicationDbContext _context = context;

		public  IActionResult Index()
		{
			return View();
		}
		public  IActionResult Shop()
		{
			return View();
		}



		public IActionResult Cart()
		{
			return RedirectToAction("Index", "Cart");
		}

		public IActionResult Products()
		{
			return RedirectToAction("Index" , "Products");
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
