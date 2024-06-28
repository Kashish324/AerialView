using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class SubmenuController(DataAccess dataAccess) : BaseController(dataAccess)
    {
        public IActionResult SubMenuConfiguration(string parentMenu, string submenu)
        {
            var reportData = _dataAccess.GetReportData();
            var uniqueReportTypes = reportData
                .GroupBy(r => r.ReportType)
                .Select(g => g.First())
                .ToList();
            ViewData["ReportData"] = reportData;
            ViewData["ReportType"] = uniqueReportTypes;


            ViewBag.ParentMenu = parentMenu;    
            ViewBag.Submenu = submenu;


            ViewData["MenuName"] = submenu;

            switch (submenu.ToLower())
            {
                case "report configuration":
                    return View("ReportConfiguration");

                case "dash designer":
                    return View("DashDesigner");

                case "dash configuration":
                    return View("DashConfig");
                default:
                    return View("DefaultView");
            }
        }
    }
}
