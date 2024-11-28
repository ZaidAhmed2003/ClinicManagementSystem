using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ClinicManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;

namespace ClinicManagementSystem.Controllers
{
	[Authorize(Roles ="Admin , User")]
	public class DashboardController : Controller
    {
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			// Get the total number of users
			var users = await _userManager.Users.ToListAsync();
			ViewBag.TotalUsers = users.Count;

			// Calculate total products sold
			var totalProductsSold = await _context.OrderItems
				.Include(oi => oi.Order) // If you want to filter by specific criteria (like order status), you can join here
				.SumAsync(oi => oi.Quantity);

			// Calculate total revenue (total amount from all orders)
			var totalRevenue = await _context.Orders
				.Where(o => o.Status == OrderStatus.Completed) // Filter only completed orders
				.SumAsync(o => o.TotalAmount);

			// Calculate total COGS (Cost of Goods Sold)
			var totalCOGS = await _context.OrderItems
				.Include(oi => oi.Product)
				.SumAsync(oi => oi.Quantity * oi.Product.CostPrice);

			// Calculate total profit (Revenue - COGS)
			var totalProfit = totalRevenue + totalCOGS;

			ViewBag.TotalRevenue = totalRevenue;
			ViewBag.TotalCOGS = totalCOGS;
			ViewBag.TotalProfit = totalProfit;

			return View();
		}
	}
}
