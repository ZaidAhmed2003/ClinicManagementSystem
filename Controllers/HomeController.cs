using ClinicManagementSystem.Data;
using ClinicManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ClinicManagementSystem.Controllers
{
    public class HomeController(ApplicationDbContext context) : Controller
    {
		private readonly ApplicationDbContext _context = context;

		public async Task <IActionResult> Index()
        {
			// Fetch products where IsAvailable is false
			var products = await _context.Products
				.Where(o => o.IsAvailable) // Equivalent to o.IsAvailable == false
				.ToListAsync();

			// Check if the user is authenticated and is an admin
			if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
			{
				// Redirect to the dashboard for admins
				return RedirectToAction("Index", "Dashboard");
			}

			// Return the view with the products list
			return View(products);
		}	


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
