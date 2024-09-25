using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class SubmenuDashViewerController : Controller
    {
        private readonly DataAccess _dataAccess;

        public SubmenuDashViewerController(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        #region shows us the saved dashboard from the menu under menu parent "Dashboards"
        public IActionResult ViewDashboard(int dashId)
        {
            var dashboardListData = _dataAccess.GetDashboardMasterData();

            var dashboard = dashboardListData.FirstOrDefault(d => d.DashId == dashId);

            if (dashboard == null || string.IsNullOrEmpty(dashboard.DashPath))
            {
                return NotFound("Dashboard not found.");
            }

            return View(dashboard);
        }
        #endregion
    }
}
