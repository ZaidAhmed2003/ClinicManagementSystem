using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ClinicManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;

namespace ClinicManagementSystem.Controllers
{
	
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
			var user = await _userManager.Users.ToListAsync();
			

			ViewBag.totalusers = user.Count;

            return View();
        }
    }
}
