using final_aerialview.Data;
using final_aerialview.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using final_aerialview.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace final_aerialview.Controllers
{
    public class HomeController(ILogger<HomeController> logger, DataAccess dataAccess) : BaseController(dataAccess)
    {
        private readonly ILogger<HomeController> _logger = logger;

        [Authorize]
        public IActionResult Index()
        {
            var menuParentData = _dataAccess.GetMenuParentData();
            var subMenuData = _dataAccess.GetSubMenuData();
            var childMenuData = _dataAccess.GetChildMenuData();



            //var roleClaim = User.FindFirst(ClaimTypes.Role);
            //string role = roleClaim?.Value ?? string.Empty;

            //ViewBag.UserMasterData = _dataAccess.GetAccessibleControlsForUserAsync(role).Result;


            ViewData["MenuParentData"] = menuParentData;
            ViewData["SubMenuData"] = subMenuData;
            ViewData["ChildMenuData"] = childMenuData;

            //var viewModel = new MenuViewModel
            //{
            //    MenuParentData = menuParentData,
            //    SubMenuData = subMenuData,
            //    ChildMenuData = childMenuData
            //};

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
