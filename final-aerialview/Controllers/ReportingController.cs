using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class ReportingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
