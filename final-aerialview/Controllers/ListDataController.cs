using Microsoft.AspNetCore.Mvc;
using final_aerialview.Data;
using final_aerialview.Models;
using final_aerialview.ViewModels;
using final_aerialview.Utilities;

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
                RptId = rptId,
                PdfImageData = pdfImageData,
            };


            return View(viewModel);
        }

    }
}

