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
            ViewData["Mode"] = "Historical";
            return View("ReportList");
        }

        //Live Charts
        public IActionResult LiveCharts()
        {
            var reportNameData = _dataAccess.GetReportData();
            var reportConnData = _dataAccess.GetReportConnectionData();

            ViewData["ReportsConn"] = reportConnData;
            ViewData["ReportData"] = reportNameData;
            ViewData["Mode"] = "Live";

            return View("ReportList");
        }

        //Show Charts form view
        public IActionResult ChartConfiguration(int id, string mode = "Historical")
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

            ViewBag.Mode = mode;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RenderLiveChart(string connString, string tableName, string xColumn, List<string> yColumns, int refreshRateSeconds)
        {
           
            var dataSeries = _dataAccess.GetSplineChartSeries(connString, tableName, xColumn, yColumns, DateTime.Now.AddMinutes(-5), DateTime.Now);

            ViewBag.DataSeries = JsonConvert.SerializeObject(dataSeries);
            ViewBag.TableName = tableName;
            ViewBag.XColumn = xColumn;
            ViewBag.YColumns = yColumns;
            ViewBag.RefreshRateSeconds = refreshRateSeconds;
            ViewBag.ConnectionString = connString;

            return PartialView("RenderLiveChart");
        }


        ////this endpoint is for live updates
        [HttpGet]
        public IActionResult GetLiveChartData(string connString, string tableName, string xColumn, List<string> yColumns)
        {
            if (string.IsNullOrWhiteSpace(connString))
                return BadRequest("Missing connection string.");

            var startTime = DateTime.Now.AddMinutes(-5); 
            var endTime = DateTime.Now;

            //Console.WriteLine($"[GetLiveChartData] Fetching data from {startTime} to {endTime}");

            //Console.WriteLine("xColumn: " + xColumn);
            //Console.WriteLine("yColumns: " + (yColumns == null ? "null" : string.Join(", ", yColumns)));


            var dataSeries = _dataAccess.GetSplineChartSeries(connString, tableName, xColumn, yColumns, startTime, endTime);

            //Console.WriteLine($"[GetLiveChartData] Returning {dataSeries?.Count ?? 0} series");

            return Json(dataSeries);
            //return NoContent();
        }
    }
}
