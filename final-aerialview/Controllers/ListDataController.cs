using Microsoft.AspNetCore.Mvc;
using final_aerialview.Data;
using final_aerialview.Models;
using final_aerialview.ViewModels;
using final_aerialview.Utilities;
using System.Text.Json;
using Dapper;

namespace final_aerialview.Controllers
{
    public class ListDataController(DataAccess dataAccess) : BaseController(dataAccess)
    {
        public IActionResult Index(int? subMenuId, string? subMenuName)
        {
            if (subMenuId != null)
            {
                var menuItems = _dataAccess.GetChildMenuData();
                IEnumerable<PdfImageModel> pdfImageData = _dataAccess.GetPdfImageData();

                // Convert logo data to Base64
                foreach (var item in pdfImageData)
                {
                    if (!string.IsNullOrEmpty(item.Logo))
                    {
                        item.Logo = ImageConverter.ConvertHexToBase64(item.Logo);
                    }
                }

                var selectedChildData = menuItems.FirstOrDefault(item => item.SubMenuId == subMenuId);
                var childMenuName = subMenuName ?? string.Empty;

                if (selectedChildData != null && !string.IsNullOrEmpty(selectedChildData.stringName))
                {
                    var tableName = selectedChildData.DataTableName;
                    var connectionString = selectedChildData.stringName;

                    ViewBag.TableName = tableName;

                    int rptId = selectedChildData.RptId;

                    var viewModel = new ListDataViewModel
                    {
                        ChildMenuData = menuItems.Where(item => item.SubMenuId == subMenuId),
                        ChildMenuName = childMenuName,
                        ConnectionString = connectionString,
                        TableName = tableName,
                        RptId = rptId,
                        TableData = [],
                        PdfImageData = pdfImageData,
                    };
                    return View(viewModel);
                }
                else
                {
                    return View("Error");
                }
            }
            else
            {
                return View("Error");
            }

        }

        [HttpPost]
        public IActionResult DataGrid(string option, string selectedValue, string fromDate, string toDate, string tableName, string connString, int rptId)
        {
            var dynamicData = _dataAccess.DynamicConnString(connString, tableName, option, selectedValue, fromDate, toDate);
            var condtionalTable = _dataAccess.ConditionalTable(rptId);


            IEnumerable<PdfImageModel> pdfImageData = _dataAccess.GetPdfImageData();

            // Convert logo data to Base64
            foreach (var item in pdfImageData)
            {
                if (!string.IsNullOrEmpty(item.Logo))
                {
                    item.Logo = ImageConverter.ConvertHexToBase64(item.Logo);
                }
            }

            var viewModel = new ListDataViewModel
            {
                TableData = dynamicData,
                ConditionalTableData = condtionalTable,
                RptId = rptId,
                PdfImageData = pdfImageData,
                ConnectionString = connString
            };
            return View(viewModel);
        }


        [HttpPost]
        public IActionResult UpdateDataGrid([FromBody] UpdateDataRequestModel request)
        {
            if (request == null || request.TableName == null || request.ConnString == null || request.Changes == null)
            {
                return BadRequest(new { success = false, message = "No changes provided" });
            }

            try
            {
                // Convert Changes to a more suitable format if necessary
                foreach (var change in request.Changes)
                {
                    if (change.ContainsKey("DateAndTime") && change["DateAndTime"] is JsonElement dateElement)
                    {
                        // Convert JsonElement to string and then parse to DateTime
                        string dateString = dateElement.GetRawText().Trim('"');
                        if (DateTime.TryParse(dateString, out DateTime parsedDate))
                        {
                            change["DateAndTime"] = parsedDate; // Replace JsonElement with parsed DateTime
                        }
                        else
                        {
                            // Handle parsing failure if necessary
                            throw new InvalidOperationException("Invalid DateAndTime format");
                        }
                    }
                }

                // Call your data access layer for the batch update with dynamic parameters
                _dataAccess.UpdatedDataGrid(request.Changes, request.TableName, request.ConnString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}

