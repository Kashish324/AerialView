//using final_aerialview.Data;
//using Microsoft.AspNetCore.Mvc;

//namespace final_aerialview.Controllers
//{
//    public class SubmenuController(DataAccess dataAccess) : BaseController(dataAccess)
//    {
//        public IActionResult SubMenuConfiguration(string parentMenu, string submenu)
//        {
//            var reportData = _dataAccess.GetReportData();
//            var dashboardListData = _dataAccess.GetDashboardMaterData();

//            var uniqueReportTypes = reportData
//                .GroupBy(r => r.ReportType)
//                .Select(g => g.First())
//                .ToList();
//            ViewData["ReportData"] = reportData;
//            ViewData["ReportType"] = uniqueReportTypes;


//            ViewBag.ParentMenu = parentMenu;    
//            ViewBag.Submenu = submenu;


//            ViewData["MenuName"] = submenu;

//            switch (submenu.ToLower())
//            {
//                case "report configuration":
//                    return View("ReportConfiguration");

//                case "dash designer":
//                    return View("DashDesigner");

//                case "dash configuration":
//                    return View("DashConfig");
//                default:
//                    return View("DefaultView");
//            }
//        }
//    }
//}

using final_aerialview.Controllers;
using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;

public class SubmenuController(DataAccess dataAccess) : BaseController(dataAccess)
{
    public IActionResult ReportConfiguration(string parentMenu, string submenu)
    {
        // Example logic specific to ReportConfiguration view
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

        return View();
    }

    public IActionResult DashDesigner(string parentMenu, string submenu)
    {
        ViewBag.ParentMenu = parentMenu;
        ViewBag.Submenu = submenu;
        ViewData["MenuName"] = submenu;

        return View();
    }

    public IActionResult DashConfig(string parentMenu, string submenu)
    {
       
        var dashboardListData = _dataAccess.GetDashboardMaterData();

        ViewBag.ParentMenu = parentMenu;
        ViewBag.Submenu = submenu;
        ViewData["MenuName"] = submenu;

        return View(dashboardListData);
    }

    // Add more actions as needed for each submenu view

    public IActionResult SubMenuConfiguration(string parentMenu, string submenu)
    {
        // Common logic or redirect to default view based on submenu
        switch (submenu.ToLower())
        {
            case "report configuration":
                return RedirectToAction(nameof(ReportConfiguration), new { parentMenu, submenu });

            case "dash designer":
                return RedirectToAction(nameof(DashDesigner), new { parentMenu, submenu });

            case "dash configuration":
                return RedirectToAction(nameof(DashConfig), new { parentMenu, submenu });

            default:
                return View("DefaultView");
        }
    }
}
