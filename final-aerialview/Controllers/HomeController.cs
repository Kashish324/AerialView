using final_aerialview.Data;
using final_aerialview.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace final_aerialview.Controllers
{
    public class HomeController(ILogger<HomeController> logger, DataAccess dataAccess) : BaseController(dataAccess)
    {
        private readonly ILogger<HomeController> _logger = logger;

        [Authorize]
        public IActionResult Index()
        {

            // Check if session flag is already set
            bool isNewTabOpened = HttpContext.Session.GetString("IsNewTabOpened") == "True";


            // Pass flag to view
            ViewBag.IsNewTabOpened = isNewTabOpened;
            

            return View();
        }

        public IActionResult NoReportAvailable(int datagridRptid)
        {
            ViewBag.DatagridRptid = datagridRptid;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
