using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class FilterController : Controller
    {
        [HttpPost]
        public IActionResult FilterView(string selectedRadioOption, string selectedFilterValue)
        {
            return View();
        }
    }
}
