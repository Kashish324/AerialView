using final_aerialview.Data;
using final_aerialview.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class FilterController : BaseController
    {
        public FilterController(DataAccess dataAccess) : base(dataAccess)
        {
            //_dataAccess = dataAccess;
        }

        public IActionResult FilterView()
        {
            return View();
        }

    }
}
