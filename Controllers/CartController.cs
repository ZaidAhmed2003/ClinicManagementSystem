using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers
{
	public class CartController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
