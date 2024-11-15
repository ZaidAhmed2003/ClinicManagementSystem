using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ClinicManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
	[Authorize]
	public class UserManagementController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public UserManagementController(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}


		[Authorize(Roles = "Admin")]
		public async Task <IActionResult> Index()
		{
			var users = await _userManager.Users.ToListAsync();
			return View(users);
		}
	}
}
