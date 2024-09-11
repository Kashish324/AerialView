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
            ViewData["selectedReport"] = selectedReport;

            var reportData = _dataAccess.GetReportFromDatabase();


            if(reportData != null)
            {
            string reportName = reportData.WebReportPath ?? string.Empty;
            string reportTitle = reportData.ReportName ?? string.Empty;

            ViewData["ReportName"] = reportName;
            ViewData["ReportTitle"] = reportTitle;
            }
            else
            {
                ViewData["ReportName"] = string.Empty;
                ViewData["ReportTitle"] = string.Empty;
            }

            return View();
        }

        public IActionResult DocumentViewer(int datagridRptid)
        {
            UpdateModel.DatagridRptid = datagridRptid;

            ViewData["DatagridRptid"] = datagridRptid;

            var reportData = _dataAccess.GetReportFromDatabase();
            if (reportData != null)
            {
                string reportName = reportData.WebReportPath ?? string.Empty;
                string reportTitle = reportData.ReportName ?? string.Empty;

                ViewData["ReportName"] = reportName;
                ViewData["ReportTitle"] = reportTitle;
            }
            else
            {
                ViewData["ReportName"] = string.Empty;
                ViewData["ReportTitle"] = string.Empty;
            }

            return View();
        }

    }
}
