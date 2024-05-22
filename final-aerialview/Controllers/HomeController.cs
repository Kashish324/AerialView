using final_aerialview.Data;
using final_aerialview.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace final_aerialview.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DataAccess dataAccess) : base(dataAccess)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var menuParentData = _dataAccess.GetMenuParentData();
            var subMenuData = _dataAccess.GetSubMenuData();
            var childMenuData = _dataAccess.GetChildMenuData();

            ViewData["MenuParentData"] = menuParentData;
            ViewData["SubMenuData"] = subMenuData;
            ViewData["ChildMenuData"] = childMenuData;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
