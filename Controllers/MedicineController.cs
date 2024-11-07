using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers
{
    public class MedicineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
