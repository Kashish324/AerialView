//using final_aerialview.Data;
//using Microsoft.AspNetCore.Mvc;

//namespace final_aerialview.Controllers
//{

//    public class FilterController : BaseController
//    {

//        public FilterController(DataAccess dataAccess) : base(dataAccess)
//        {
//            //_dataAccess = dataAccess;
//        }

//        [HttpPost]
//        public IActionResult FilterView(string option, string selectedValue, string fromDate, string toDate)
//        {

//            if (option == "Standard")
//            {
//                // Implement your logic for handling the "Standard" option
//                //var filteredData = YourDataService.GetStandardFilteredData(selectedValue);
//                //return View(filteredData);

//                if (selectedValue == null)
//                {
//                    return BadRequest("Select one of the option");
//                }

//            }
//            else if (option == "Date")
//            {
//                // Implement your logic for handling the "Date" option
//                //var filteredData = YourDataService.GetDateFilteredData(fromDate, toDate);
//                //return View(filteredData);
//            }

//            return View();
//        }
//    }
//}


using final_aerialview.Data;
using final_aerialview.Models;
using final_aerialview.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace final_aerialview.Controllers
{
    public class FilterController : BaseController
    {
        public FilterController(DataAccess dataAccess) : base(dataAccess)
        {
            //_dataAccess = dataAccess;
        }

        [HttpPost]
        public IActionResult FilterView(string option, string selectedValue, string fromDate, string toDate, string tableName)
        {

            UpdateModel.DatagridTableName = tableName;
            
            if (option == "Standard")
            {
                if (selectedValue == null)
                {
                    return BadRequest("Select one of the option");
                }

                DateTime filterDate = CalculateFilterDate(selectedValue);
                string formattedFilterDate = filterDate.ToString("yyyy-MM-ddTHH:mm:ss");
                string whereClause = $"WHERE DateAndTime >= '{formattedFilterDate}'";
                var filteredData = _dataAccess.GetFilteredData(whereClause);

                return View(filteredData);
            }
            else if (option == "Date")
            {
                string whereClause = $"WHERE DateAndTime >= '{fromDate}' AND DateAndTime <= '{toDate}'";
                var filteredData = _dataAccess.GetFilteredData(whereClause);
                return View(filteredData);
            }
            return View();
        }

        private DateTime CalculateFilterDate(string selectedValue)
        {
            DateTime currentDate = DateTime.Now;
            DateTime filterDate = DateTime.MinValue;

            switch (selectedValue)
            {
                case "Previous Day":
                    filterDate = currentDate.AddDays(-1);
                    break;
                case "1 Week":
                    filterDate = currentDate.AddDays(-7);
                    break;
                case "1 Month":
                    filterDate = currentDate.AddMonths(-1);
                    break;
                case "3 Months":
                    filterDate = currentDate.AddMonths(-3);
                    break;
                case "6 Months":
                    filterDate = currentDate.AddMonths(-6);
                    break;
                case "1 Year":
                    filterDate = currentDate.AddYears(-1);
                    break;
                case "All":
                    // Return an arbitrary date in the past to disable filtering
                    filterDate = DateTime.MinValue;
                    break;
                default:
                    throw new ArgumentException("Invalid selectedValue for standard filter.");
            }

            return filterDate;
        }
    }
}
