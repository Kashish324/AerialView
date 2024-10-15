using DevExpress.ReportServer.ServiceModel.DataContracts;
using DevExpress.Xpo.DB.Helpers;
using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class EventConfigurationController(DataAccess dataAccess) : BaseController(dataAccess)
    {
        public IActionResult EventConfig()
        {
            var configurations = _dataAccess.GetEventConfigData();
            var connTableData = _dataAccess.GetChildMenuData();

            ViewData["Reports"] = connTableData;
           

            ViewData["EventConfigurations"] = configurations.ToList();

            return View();
        }

        [HttpGet]
        public IActionResult GetTableNames(string connectionString)
        {
            // Validate the connection string if necessary
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return BadRequest("Connection string cannot be empty.");
            }

            var tableNames = _dataAccess.FetchTableNamesByConnectionString(connectionString);

            


            // Return the table names as JSON
            return Json(tableNames);
        }


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
    }

}
