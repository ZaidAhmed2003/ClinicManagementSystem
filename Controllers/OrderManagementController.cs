using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
    public class OrderManagementController : Controller
    {
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public OrderManagementController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		// GET: OrderManagement
		public async Task<IActionResult> Index()
		{
			// Get the current logged-in user
			var currentUser = await _userManager.GetUserAsync(User);

			// If no user is logged in, redirect to login or show an error message
			if (currentUser == null)
			{
				return RedirectToAction("Login", "Account");  // Or show an error message
			}

			// Check if the user is an admin
			if (User.IsInRole("Admin"))
			{
				// Admin sees all orders
				var allOrders = await _context.Orders
											  .Include(o => o.User)  // Assuming you have a User navigation property in Order
											  .ToListAsync();
				return View(allOrders);
			}
			else
			{
				// Regular user sees only their orders
				var userOrders = await _context.Orders
											   .Where(o => o.Id == currentUser.Id)
											   .Include(o => o.User)
											   .ToListAsync();
				return View(userOrders);
			}
		}
	}
}
