using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class SubmenuController : BaseController
    {

        public SubmenuController(DataAccess dataAccess) : base(dataAccess)
        {
            //_dataAccess = dataAccess;
        }

        public IActionResult ReportConfiguration(string parentMenu, string submenu)
        {
            var reportData = _dataAccess.GetReportData();
            var uniqueReportTypes = reportData
                .GroupBy(r => r.ReportType)
                .Select(g => g.First())
                .ToList();
            ViewData["ReportData"] = reportData;
            ViewData["ReportType"] = uniqueReportTypes;



            ViewData["MenuName"] = submenu;

            switch (submenu.ToLower())
            {
                case "report configuration":
                    return View("ReportConfiguration");
                // Add more cases as needed
                default:
                    return View("DefaultView");
            }
        }
    }
}
