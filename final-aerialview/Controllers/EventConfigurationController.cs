using DevExpress.ReportServer.ServiceModel.DataContracts;
using DevExpress.Xpo.DB.Helpers;
using final_aerialview.Data;
using final_aerialview.Models;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class EventConfigurationController(DataAccess dataAccess) : BaseController(dataAccess)
    {
        public IActionResult EventConfig()
        {
            var configurations = _dataAccess.GetEventConfigData();
            var connTableData = _dataAccess.GetChildMenuData();
            var reportNameData = _dataAccess.GetReportData();

            ViewData["Reports"] = connTableData;
            ViewData["ReportData"] = reportNameData;


            ViewData["EventConfigurations"] = configurations.ToList();

            return View();
        }



        #region get all the integer column names acc. to the selected report name
        [HttpGet]
        public IActionResult GetColumnNames(string connectionString, string tableName)
        {
            // Validate the connection string if necessary
            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(tableName))
            {
                return BadRequest("Connection string or table name cannot be empty.");
            }

            var columnNames = _dataAccess.FetchColNamesForEventConfig(connectionString, tableName);

            // Return the table names as JSON
            return Json(columnNames);
        }
        #endregion

        #region inserting or updating the configuration to the sql 
        [HttpPost]
        public IActionResult SaveConfiguration([FromBody] EventConfigViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest("Invalid Data.");
            }

            // Handle your model and save it to the database
            if (ModelState.IsValid)
            {
                bool status = viewModel.Status ?? false; // Default to false if null

                string createdAt = viewModel.CreatedAt ?? DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"); // Default to current time in ISO format
                string updatedAt = viewModel.UpdatedAt ?? DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                var result = _dataAccess.InsertOrUpdateEventConfigMasterData(viewModel.Id, viewModel.ConnString, viewModel.TableName, viewModel.ColumnName, viewModel.AlarmLow, viewModel.AlarmHigh, viewModel.WarningHigh, viewModel.WarningLow, createdAt, updatedAt, status, viewModel.RptId, viewModel.CreatedById, viewModel.UpdateById, viewModel.Emails);


                if (result) // Check if the operation was successful
                {
                    return Json(new { success = true, message = "Configuration saved successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to save configuration." });
                }
            }

            return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }
        #endregion

        #region Get configuration by ID
        [HttpGet]
        public IActionResult GetConfigurationById(int id)
        {
            // Fetch the configuration based on the ID
            var configuration = _dataAccess.GetEventConfigDataById(id);

            return Json(new { success = true, data = configuration });
        }
        #endregion


        #region to delete configuration 
        [HttpPost]
        public IActionResult DeleteConfiguration(int Id)
        {
            if (Id == 0)
            {
                return BadRequest("Invalid Data.");
            }

            try
            {
                var result = _dataAccess.DeleteEventConfigData(Id); // Remove await here
                if (result) // Check if the operation was successful
                {
                    return Json(new { success = true, message = "Data deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to delete data." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        #endregion
    }

}
