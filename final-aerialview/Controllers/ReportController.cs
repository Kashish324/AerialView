using final_aerialview.Data;
using final_aerialview.Models;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class ReportController(DataAccess dataAccess) : BaseController(dataAccess)
    {
        #region shows the report designer according to the selected item
        [HttpGet]
        public IActionResult ReportDesigner(int datagridRptid, string selectedReport)
        {
            UpdateModel.DatagridRptid = datagridRptid;
            ViewData["DatagridRptid"] = datagridRptid;
            ViewData["selectedReport"] = selectedReport;

            var reportData = _dataAccess.GetReportFromDatabase();

            string reportName = reportData.WebReportPath ?? string.Empty;
            string reportTitle = reportData.ReportName ?? string.Empty;

            ViewData["ReportName"] = reportName;
            ViewData["ReportTitle"] = reportTitle;

            return View();
        }
        #endregion


        #region shows the document viewer according to the selected item
        public IActionResult DocumentViewer(int datagridRptid)
        {
            UpdateModel.DatagridRptid = datagridRptid;

            ViewData["DatagridRptid"] = datagridRptid;

            var reportData = _dataAccess.GetReportFromDatabase();

            string reportName = reportData.WebReportPath ?? string.Empty;

            string reportTitle = reportData.ReportName ?? string.Empty;

            ViewData["ReportName"] = reportName;
            ViewData["ReportTitle"] = reportTitle;

            return View();
        }
        #endregion
    }
}
