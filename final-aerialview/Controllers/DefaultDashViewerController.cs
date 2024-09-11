using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class DefaultDashViewerController : Controller
    {
        private readonly DataAccess _dataAccess;

        public DefaultDashViewerController(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public IActionResult DefaultDashboard()
        {
            // Set session flag to indicate new tab is opened
            HttpContext.Session.SetString("IsNewTabOpened", "True");

            var dashboardListData = _dataAccess.GetDashboardMasterData();

            return View(dashboardListData);
        }
    }
}