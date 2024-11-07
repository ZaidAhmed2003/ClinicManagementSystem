using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers
{
    public class TransactionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
