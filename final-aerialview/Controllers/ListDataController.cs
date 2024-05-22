using Microsoft.AspNetCore.Mvc;
using final_aerialview.Data;
using final_aerialview.Models;
using final_aerialview.ViewModels;
using System.Data.SqlClient;
using Dapper;
using final_aerialview.Utilities;

namespace final_aerialview.Controllers
{
    public class ListDataController : BaseController
    {
        public ListDataController(DataAccess dataAccess) : base(dataAccess)
        {
            //_dataAccess = dataAccess;
        }

        public IActionResult Index(int? subMenuId, string? subMenuName)
        {
            if (subMenuId != null)
            {
                var menuItems = _dataAccess.GetChildMenuData();
                var reportData = _dataAccess.GetReportData();
                ViewData["ReportData"] = reportData;

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

                IEnumerable<dynamic>? queryResult = null;

                if (selectedChildData != null && !string.IsNullOrEmpty(selectedChildData.stringName))
                {
                    var connectionString = selectedChildData.stringName;
                    var tableName = selectedChildData.DataTableName;
                    var dynamicConnString = ParseDatabaseData(connectionString);

                    if (dynamicConnString != null && tableName != null)
                    {
                        queryResult = ExecuteDynamicQuery(dynamicConnString, tableName);
                    }
                    else
                    {
                        throw new Exception("Can't find Connection String and Table Name");
                    }

                    int rptId = selectedChildData.RptId;

                    var viewModel = new ListDataViewModel
                    {
                        ChildMenuData = menuItems.Where(item => item.SubMenuId == subMenuId),
                        ChildMenuName = childMenuName,
                        ConnectionString = connectionString,
                        TableName = tableName,
                        RptId = rptId,
                        DynamicConnectionString = dynamicConnString.ToString(),
                        DataSource = dynamicConnString.DataSource,
                        DataBaseName = dynamicConnString.InitialCatalog,
                        UserID = dynamicConnString.UserID,
                        Password = dynamicConnString.Password,
                        TableData = queryResult,
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

        private DynamicConnectionString? ParseDatabaseData(string connectionString)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);

                return new DynamicConnectionString
                {
                    DataSource = builder.DataSource,
                    InitialCatalog = builder.InitialCatalog,
                    UserID = builder.ContainsKey("User ID") ? builder.UserID : string.Empty,
                    Password = builder.ContainsKey("Password") ? builder.Password : string.Empty,
                    PersistSecurityInfo = builder.ContainsKey("Persist Security Info") ? builder.PersistSecurityInfo : true,
                    IntegratedSecurity = builder.ContainsKey("Integrated Security") ? builder.IntegratedSecurity : false,
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("error parsing connection string " + e.Message);
                return null;
            }
        }

        // Constructing dynamic connection 
        private IEnumerable<dynamic>? ExecuteDynamicQuery(DynamicConnectionString dynamicConnectionString, string tableName)
        {
            ViewData["DatabaseName"] = dynamicConnectionString.InitialCatalog;

            try
            {
                var connStringBuild = $"Data Source={dynamicConnectionString.DataSource};Initial Catalog={dynamicConnectionString.InitialCatalog};";

                if (!string.IsNullOrEmpty(dynamicConnectionString.UserID))
                {
                    connStringBuild += $"User ID={dynamicConnectionString.UserID};";
                }

                if (!string.IsNullOrEmpty(dynamicConnectionString.Password))
                {
                    connStringBuild += $"Password={dynamicConnectionString.Password};";
                }

                connStringBuild += $"Persist Security Info={dynamicConnectionString.PersistSecurityInfo};Integrated Security={dynamicConnectionString.IntegratedSecurity};";

                using (var connection = new SqlConnection(connStringBuild))
                {
                    connection.Open();

                    var query = $"SELECT * FROM {tableName}";

                    var result = connection.Query(query);

                    connection.Close();

                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

       
    }
}

