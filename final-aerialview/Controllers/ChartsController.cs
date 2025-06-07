using DevExpress.Xpo.DB.Helpers;
using System.Linq;
using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public IActionResult ChartConfiguration(int id)
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

            if (string.IsNullOrWhiteSpace(conn.ConName))
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


            var columnMap = _dataAccess.GetColumnsOfTable(connStr, report.DataTableName);

            var allColumns = columnMap.Keys.ToList();

            ViewBag.Columns = allColumns;

            // Pass only numeric columns to view
            var numericTypes = new[] { "int", "float", "decimal", "real", "double", "numeric", "bigint", "smallint", "money" };

            ViewBag.NumericColumns = columnMap
                .Where(kv => numericTypes.Contains(kv.Value.ToLower()))
                .Select(kv => kv.Key)
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RenderLineChart(string connString, string tableName, string xColumn, List<string> yColumns, DateTime? fromDate, DateTime? toDate)
        {
            var dataSeries = _dataAccess.GetSplineChartSeries(connString, tableName, xColumn, yColumns, fromDate, toDate);

            ViewBag.DataSeries = JsonConvert.SerializeObject(dataSeries);
            ViewBag.TableName = tableName;
            ViewBag.XColumn = xColumn;
            ViewBag.YColumns = yColumns;

            return PartialView("RenderLineChart");
        }

    }
}
