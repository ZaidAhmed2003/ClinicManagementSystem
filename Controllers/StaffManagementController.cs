using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers
{
    public class StaffManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
