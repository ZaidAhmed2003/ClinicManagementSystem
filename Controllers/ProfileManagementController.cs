using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.ViewModels.ProfileManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


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

		// Display profile page
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return RedirectToAction("Login", "Account"); // Redirect if user not found
			}

			var userAddress = await _context.UserAddresses.FirstOrDefaultAsync(a => a.UserId == user.Id);

			var model = new UserProfileViewModel
			{
				Email = User.Identity.Name,
				FirstName = user.FirstName,
				LastName = user.LastName,
				AddressLine1 = userAddress?.AddressLine1,
				AddressLine2 = userAddress?.AddressLine2,
				PostalCode = userAddress?.PostalCode,
				Country = userAddress?.Country,
				City = userAddress?.City,
				Mobile = userAddress?.Mobile
			};

			return View(model);

		}

		// GET: Profile/Edit
		public async Task<IActionResult> EditProfile()
		{
			var user = await _userManager.GetUserAsync(User);
		
			if (user == null)
			{
				return RedirectToAction("Login", "Account"); // Redirect if user not found
			}
			var userId = await _context.Users.FindAsync(user.Id); // Find user by ID

			// Get the user's address from the UserAddressModel
			var address = await _context.UserAddresses.FirstOrDefaultAsync(a => a.UserId == user.Id);

			// Map the data to the EditProfileViewModel
			var model = new EditProfileViewModel
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = User.Identity.Name,
				AddressLine1 = address?.AddressLine1,
				AddressLine2 = address?.AddressLine2,
				PostalCode = address?.PostalCode,
				Country = address?.Country,
				City = address?.City,
				Mobile = address?.Mobile
			};

			return View(model); 
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditProfile(EditProfileViewModel model)
		{
			
			// Get the logged-in user
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound(); // User not found
			}

			// Update user details
			user.FirstName = model.FirstName;
			user.LastName = model.LastName;

			// Update address details
			var address = await _context.UserAddresses.FirstOrDefaultAsync(a => a.UserId == user.Id);
			if (address == null)
			{
				address = new UserAddressModel
				{
					UserId = user.Id,
					AddressLine1 = model?.AddressLine1,
					AddressLine2 = model?.AddressLine2,
					PostalCode = model?.PostalCode,
					Country = model?.Country,
					City = model?.City,
					Mobile = model?.Mobile,
				};
				_context.UserAddresses.Add(address);
			}
			else
			{
				address.AddressLine1 = model.AddressLine1;
				address.AddressLine2 = model.AddressLine2;
				address.PostalCode = model.PostalCode;
				address.Country = model.Country;
				address.City = model.City;
				address.Mobile = model.Mobile;
				_context.UserAddresses.Update(address);
			}

			// Save address updates
			await _context.SaveChangesAsync();

			return RedirectToAction("Index"); 
		}
	}
}

