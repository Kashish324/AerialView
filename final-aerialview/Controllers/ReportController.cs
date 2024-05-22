using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
