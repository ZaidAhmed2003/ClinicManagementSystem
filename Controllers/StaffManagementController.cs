using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
	public class StaffManagementController : Controller
	{
		private readonly ApplicationDbContext _context;

		public StaffManagementController(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index()
		{
			var staff = await _context.Staff.ToListAsync();
			return View(staff);
		}
		public IActionResult AddStaff()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> AddStaff(StaffModel staff)
		{

			_context.Add(staff);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> EditStaff(Guid Id)
		{
			var staff = await _context.Staff.FindAsync(Id);
			if (staff == null)
			{
				return NotFound();
			}
			return View(staff);
		}

		[HttpPost]
		public async Task<IActionResult> EditStaff(StaffModel staff)
		{
			
				var existingStaff = await _context.Staff.FindAsync(staff.StaffId);
				if (existingStaff == null)
				{
					return NotFound();
				}

				existingStaff.FirstName = staff.FirstName;
				existingStaff.LastName = staff.LastName;
				existingStaff.Email = staff.Email;
				existingStaff.Position = staff.Position;
				existingStaff.PhoneNumber = staff.PhoneNumber;
				

				_context.Staff.Update(existingStaff);
				await _context.SaveChangesAsync();

				return RedirectToAction("Index");
		}



		[HttpGet]
		public async Task<IActionResult> DeleteStaff(Guid Id)
		{
			var staff = await _context.Staff.FirstOrDefaultAsync(s => s.StaffId == Id);
			if (staff == null)
			{
				return NotFound();
			}

			_context.Staff.Remove(staff);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

	}
}
