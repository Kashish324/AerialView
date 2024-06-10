using final_aerialview.Data;
using final_aerialview.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class FilterController(DataAccess dataAccess) : BaseController(dataAccess)
    {
        public IActionResult FilterView()
        {
            return View();
        }

    }
}
