using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers
{
    public class ProfileManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
