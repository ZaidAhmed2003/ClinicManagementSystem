using ClinicManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
	public class ProductsController(ApplicationDbContext context) : Controller
	{
		private readonly ApplicationDbContext _context = context;

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var products = await _context.Products.ToListAsync();
			return View(products);
		}
	}
}
