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


        #region show us the home page(_layout view) after successful login
        [Authorize]
        public IActionResult Index()
        {
            //if (HttpContext.Session.GetString("IsNewTabOpened") == null)
            //{
            //    HttpContext.Session.SetString("IsNewTabOpened", "True");
            //    ViewBag.IsNewTabOpened = "False";
            //}
            //else
            //{
            //    ViewBag.IsNewTabOpened = "True";
            //}
            return View();
        }
        #endregion


        #region shows no report available page/view
        public IActionResult NoReportAvailable(int datagridRptid)
        {
            ViewBag.DatagridRptid = datagridRptid;
            return View();
        }
        #endregion


        #region shows the error page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
    }
}
