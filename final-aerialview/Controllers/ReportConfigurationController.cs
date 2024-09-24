using DevExpress.ReportServer.ServiceModel.DataContracts;
using DevExpress.XtraRichEdit.Import.Html;
using final_aerialview.Data;
using final_aerialview.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{


    public class ReportConfigurationController(ILogger<ReportConfigurationController> logger, DataAccess dataAccess) : BaseController(dataAccess)
    {
        private readonly ILogger<ReportConfigurationController> _logger = logger;

        public IActionResult HandleReportConfiguration(string parentMenu, string submenu)
        {
            ViewBag.ParentMenu = parentMenu;
            ViewBag.Submenu = submenu;

            var reportConfigTable = _dataAccess.ReportConfigData();

            var sortByRptId = reportConfigTable
                .GroupBy(r => r.RptId)
                .Select(g => g.First())
                .ToList();

            ViewData["SortedConfigTable"] = sortByRptId;


            return View();
        }

        public IActionResult SelectedConfigTable(int datagridRptid, string selectedReport)
        {
            ViewData["DatagridRptid"] = datagridRptid;
            ViewData["selectedReport"] = selectedReport;

            var selectedReportConfigTable = _dataAccess.SelectedReportConfigData(datagridRptid);

            return View(selectedReportConfigTable);
        }


        [HttpPost]
        public IActionResult UpdateReportConfig([FromBody] List<ReportConfigModel> updateData)
        {

            if (updateData == null || updateData.Count == 0)
            {
                return BadRequest(new { success = false, message = "No changes provided" });
            }

            try
            {
                foreach (var update in updateData)
                {
                    _dataAccess.BatchUpdateReportConfig(updateData);
                }
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return BadRequest(new { success = false, message = ex.Message });
            }
        }




    }
}

