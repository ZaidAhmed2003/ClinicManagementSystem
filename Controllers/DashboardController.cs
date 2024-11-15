﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ClinicManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
	
	public class DashboardController : Controller
    {
		private readonly UserManager<ApplicationUser> _userManager;

		public DashboardController(UserManager<ApplicationUser> userManager)
		{
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
