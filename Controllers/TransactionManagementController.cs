using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
	[Authorize]
	public class TransactionManagementController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public TransactionManagementController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			// Get the current user
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Unauthorized("User not found.");

			// Check if the user is an admin
			bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

			IQueryable<TransactionModel> transactionQuery;

			if (isAdmin)
			{
				// Admin can see all transactions, include related Order
				transactionQuery = _context.Transactions.Include(t => t.Order);
			}
			else
			{
				// Regular user can see only their transactions
				transactionQuery = _context.Transactions.Include(t => t.Order)
														 .Where(t => t.Order.UserId == user.Id);
			}

			// Execute the query
			var transactions = await transactionQuery.ToListAsync();

			return View(transactions);
		}
	}
}
