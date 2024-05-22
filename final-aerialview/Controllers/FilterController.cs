using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class FilterController : Controller
    {
        public IActionResult FilterView()
        {
            return View();
        }
    }
}
