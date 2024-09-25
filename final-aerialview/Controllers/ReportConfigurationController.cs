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

        #region shows the report configuration table from the database, and they were sorted on the basis of rptid so that one rptid will show up only one time for the list
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
        #endregion


        #region shows us the selected configuration table according to user selection
        public IActionResult SelectedConfigTable(int datagridRptid, string selectedReport)
        {
            ViewData["DatagridRptid"] = datagridRptid;
            ViewData["selectedReport"] = selectedReport;

            var selectedReportConfigTable = _dataAccess.SelectedReportConfigData(datagridRptid);

            return View(selectedReportConfigTable);
        }
        #endregion


        #region Update the edited values in the report configuration table in the database
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
        #endregion

    }
}

