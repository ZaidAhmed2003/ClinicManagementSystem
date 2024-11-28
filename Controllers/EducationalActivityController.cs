using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Controllers
{
	public class EducationalActivityController : Controller
	{
		private readonly ApplicationDbContext _context;

		public EducationalActivityController(ApplicationDbContext context) => _context = context;

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var activities = await _context.Educational_Activity
										   .Include(a => a.Staff) // Include staff details
										   .ToListAsync();
			return View(activities);
		}

		[HttpGet]
		public IActionResult AddEducationalActivity()
		{
			ViewBag.StaffList = new SelectList(_context.Staff.ToList(), "StaffId", "FirstName");
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> AddEducationalActivity(EducationalActivityModel active)
		{
		
				var activity = new EducationalActivityModel
				{
					ActivityId = Guid.NewGuid(),
					Title = active.Title,
					Description = active.Description,
					Date = active.Date,
					ActivityType = active.ActivityType,
					StaffId = active.StaffId // Assign selected staff ID (can be null)
				};

				await _context.Educational_Activity.AddAsync(activity);
				await _context.SaveChangesAsync();

				return RedirectToAction("Index");
		
		}
	}
}
