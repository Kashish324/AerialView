using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class MainDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
