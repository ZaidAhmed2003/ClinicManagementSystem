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

		public  IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Shop()
		{
			var products = await _context.Products.Include(c => c.Category)
												  .Include(d => d.Discount)
												  .Where(p => p.DeletedAt == null)
												  .ToListAsync();
			return View(products);
		}
		 

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
