using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers
{
    public class ProfileManagementController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public ProfileManagementController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public  async Task <IActionResult> Index()
        {
			var currentUser = await _userManager.GetUserAsync(User);

			// If no user is logged in, redirect to login or show an error message
			if (currentUser == null)
			{
				return RedirectToAction("Login", "Account");  // Or show an error message
			}

			return View(currentUser);
        }
    }
}
