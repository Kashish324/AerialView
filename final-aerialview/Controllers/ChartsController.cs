using DevExpress.Xpo.DB.Helpers;
using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class ChartsController : BaseController
    {

        private readonly IConfiguration _configuration;

        public ChartsController(DataAccess dataAccess, IConfiguration configuration)
            : base(dataAccess)
        {
            _configuration = configuration;
        }

        //Historical Charts
        public IActionResult HistoricalCharts()
        {
            var reportNameData = _dataAccess.GetReportData();
            var reportConnData = _dataAccess.GetReportConnectionData();

            ViewData["ReportsConn"] = reportConnData;
            ViewData["ReportData"] = reportNameData;

            return View();
        }

        //Live Charts
        public IActionResult LiveCharts()
        {
            return View();
        }

        //Show Charts form view
        public IActionResult ViewReport(int id)
        {
            var reports = _dataAccess.GetReportData();
            var report = reports.FirstOrDefault(r => r.RptId == id);
            if (report == null)
                return NotFound("Report not found");

            // Get connection info
            var connections = _dataAccess.GetReportConnectionData();
            var conn = connections.FirstOrDefault(c => c.ConNo == report.ConnNo && c.ConName == report.ConnName);
            if (conn == null)
                return RedirectToAction("Error", "Home");

            // Get connection string
            string? connStr = _configuration.GetConnectionString(conn.ConName);
            if (string.IsNullOrEmpty(connStr))
                return RedirectToAction("Error", "Home");

            if (string.IsNullOrWhiteSpace(report.DataTableName))
                return RedirectToAction("Error", "Home");

            // Pass only table name and connection string for now
            ViewData["TableName"] = report.DataTableName;
            ViewData["ConnectionString"] = connStr;


            ViewBag.Columns = _dataAccess.GetColumnsOfTable(connStr, report.DataTableName);

            return View("ReportView");
        }

        [HttpGet]
        public JsonResult GetColumns(string tableName)
        {
            var columns = _dataAccess.GetColumnsWithTypes(tableName);
            return Json(columns);
        }

    }
}
