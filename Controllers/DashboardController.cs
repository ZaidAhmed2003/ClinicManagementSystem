using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
