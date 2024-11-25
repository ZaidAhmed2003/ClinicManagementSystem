using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers
{
    public class TransactionManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
