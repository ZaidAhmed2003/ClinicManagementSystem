
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
	[Authorize]
    public class OrderManagementController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
    {
		private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;


		[HttpGet]
		public async Task<IActionResult> Index()
		{
			
			// Get the logged-in user
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Unauthorized("User not found.");

			// Check if the user is an admin
			bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

			// Get orders based on the user's role
			IQueryable<OrderModel> ordersQuery;

			if (isAdmin)
			{
				// Admin can see all orders, include related User and PaymentDetail
				ordersQuery = _context.Orders.Include(o => o.User)
											  .Include(o => o.PaymentDetail)
											  .Include(o => o.OrderItems)
											  .ThenInclude(oi => oi.Product);
			}
			else
			{
				// Regular user can see only their orders, include related User and PaymentDetail
				ordersQuery = _context.Orders.Include(o => o.User)
											  .Include(o => o.PaymentDetail)
											  .Include(o => o.OrderItems)
											  .ThenInclude(oi => oi.Product)
											  .Where(o => o.UserId == user.Id);
			}

			// Fetch orders
			var orders = await ordersQuery.ToListAsync();

			return View(orders); // Or return Json(orders) if you're using an API
		}



	}
}
