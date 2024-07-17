using final_aerialview.Data;
using final_aerialview.Models;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class ReportController(DataAccess dataAccess) : BaseController(dataAccess)
    {
        [HttpGet]
        public IActionResult ReportDesigner(int datagridRptid, string selectedReport)
        {
            UpdateModel.DatagridRptid = datagridRptid;
            ViewData["DatagridRptid"] = datagridRptid;
            ViewData["selectedReport"]= selectedReport;

            string reportName = _dataAccess.GetReportFromDatabase();
            ViewData["ReportName"] = reportName ?? string.Empty;

            return View();
        }

        public IActionResult DocumentViewer(int datagridRptid)
        {
            UpdateModel.DatagridRptid = datagridRptid;

            ViewData["DatagridRptid"] = datagridRptid;

            string reportName = _dataAccess.GetReportFromDatabase();
            ViewData["ReportName"] = reportName;

            return View();
        }

    }
}
