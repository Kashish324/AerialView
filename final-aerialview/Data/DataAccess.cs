using Dapper;
using DevExpress.XtraReports.Wizards;
using final_aerialview.Models;
using final_aerialview.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using System.Data.SqlClient;

namespace final_aerialview.Data
{
    public class DataAccess
    {
        private readonly IConfiguration _configuration;

        public DataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IDbConnection CreateConnection()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "Connection String not found";
            return new SqlConnection(connectionString);
        }

        public IEnumerable<T> ExecuteQuery<T>(string query)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                return connection.Query<T>(query);
            }
        }

        //for login
        public async Task<UserModel> GetUserByUsernameAsync(string userId)
        {
            using (var connection = CreateConnection())
            {
                string query = "SELECT * FROM UserMaster WHERE UserId = @UserId";
                return await connection.QuerySingleOrDefaultAsync<UserModel>(query, new { UserId = userId });
            }
        }


        //Segregate the sidebar navigation based on whether a user or an admin is logged in 
        public async Task<IEnumerable<UserMasterModel>> GetAccessibleControlsForUserAsync(string role)
        {
            using (var connection = CreateConnection())
            {
                string query = "SELECT * FROM UserControlMaster WHERE UserName = @UserName AND Status = 'True'";
                return await connection.QueryAsync<UserMasterModel>(query, new { UserName = role });
            }
        }

        //full user master table
        public IEnumerable<UserMasterModel> GetUserMasterData()
        {
            string query = "SELECT * FROM UserControlMaster";
            return ExecuteQuery<UserMasterModel>(query);
        }

        //main parent data
        public IEnumerable<ListDataModel> GetMenuParentData()
        {
            string query = "SELECT * FROM Menu_Parent";
            return ExecuteQuery<ListDataModel>(query);
        }

        //sub menu data
        public IEnumerable<SubMenuModel> GetSubMenuData()
        {
            string query = "SELECT * FROM Menu_Child_New";
            return ExecuteQuery<SubMenuModel>(query);
        }

        //child menu data
        public IEnumerable<ChildMenuModel> GetChildMenuData()
        {
            string query = "SELECT * FROM ReportDATA_View";
            return ExecuteQuery<ChildMenuModel>(query);
        }

        //project setting which includes pdf image, client name, project name, etc.
        public IEnumerable<PdfImageModel> GetPdfImageData()
        {
            string query = "SELECT CONVERT(VARCHAR(MAX), Logo, 2) AS Logo, ClientName, ProjectName FROM ProjectSettings";
            return ExecuteQuery<PdfImageModel>(query);


        }

        //my reports table
        public IEnumerable<LocalReportModel> GetReportData()
        {
            string query = "SELECT * FROM MyReports";
            return ExecuteQuery<LocalReportModel>(query);
        }


        //to update the saved report path in database
        public void UpdateReportPath(string url)
        {
            var rptid = UpdateModel.DatagridRptid;
            string query = "UPDATE MyReports SET WebReportPath = @reportName WHERE RptId = @DatagridRptid";

            CreateConnection().Execute(query, new { reportName = url, DatagridRptid = rptid });
        }

        //to fetch the saved report from the database
        public string GetReportFromDatabase()
        {
            try
            {
                var reportId = UpdateModel.DatagridRptid;
                string query = "SELECT WebReportPath FROM MyReports WHERE RptId = @ReportId";
                return CreateConnection().QueryFirstOrDefault<string>(query, new { ReportId = reportId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving report from the database: {ex.Message}");
                return null;
            }
        }

        //to connect with dynamic connection string 
        public IEnumerable<dynamic> DynamicConnString(string connectionString, string tableName, string option, string selectedValue, string fromDate, string toDate)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string whereClause = GenerateWhereClause(option, selectedValue, fromDate, toDate);
                string sqlQuery = $"SELECT * FROM {tableName} {whereClause}";

                return connection.Query<dynamic>(sqlQuery);
            }
        }

        //for generating where clause according to datagrid filter
        private string? GenerateWhereClause(string option, string selectedValue, string fromDate, string toDate)
        {
            string whereClause = "";


            if (option == "Date")
            {

                if (DateTime.TryParse(fromDate, out DateTime fromDateTime) && DateTime.TryParse(toDate, out DateTime toDateTime))
                {
                    return $" WHERE DateAndTime >= '{fromDateTime:yyyy-MM-ddTHH:mm:ss}' AND DateAndTime <= '{toDateTime:yyyy-MM-ddTHH:mm:ss}'";
                }

                return null;
            }
            else if (option == "Standard")
            {

                if (string.IsNullOrEmpty(selectedValue))
                {
                    return null;
                }
                else
                {
                    var currentDate = DateTime.Now;
                    var filterDate = currentDate;

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
                            return "";
                        default:
                            return null;
                    }

                    whereClause = $"WHERE DateAndTime >= '{filterDate:yyyy-MM-ddTHH:mm:ss}'";
                }


            }

            return whereClause;
        }

    }
}