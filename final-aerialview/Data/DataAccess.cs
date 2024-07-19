using Dapper;
using final_aerialview.Models;
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
                string sqlQuery = $"SELECT TOP 1000 * FROM {tableName} {whereClause} order by DateAndTime desc ";

                return connection.Query<dynamic>(sqlQuery);
            }
        }

        //conditional table 
        public IEnumerable<ConditionalTableModel> ConditionalTable(int rptId)
        {
            string query = $"SELECT * from Report_Config where RptId = '{rptId}' and Required = 'true'";
            return ExecuteQuery<ConditionalTableModel>(query);
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

                //return null;
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
                            //return "";
                            whereClause = ""; 
                            break;

                            //top 1000 option where clause is going null so it is by default giving select * top 1000 from table
                        default:
                            return null;
                    }
                    if(selectedValue != "All")
                    {
                    whereClause = $"WHERE DateAndTime >= '{filterDate:yyyy-MM-ddTHH:mm:ss}'";
                    }
                }
            }

            return whereClause;
        }

        //dashboard master table
        public IEnumerable<DashboardDataModel> GetDashboardMasterData()
        {
            string query = "Select DashId,DashName,DashPath, (case when DashStatus ='' then 'False' else DashStatus end) as 'DashStatus',(case when DashDefault ='' then 'False' else DashDefault end) as 'DashDefault' from DashboardMaster order by DashId asc";
            return ExecuteQuery<DashboardDataModel>(query);
        }

        public void UpdateDashboardData(IEnumerable<DashboardDataModel> updatedData)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                var sql = "UPDATE DashboardMaster SET DashName = @DashName, DashPath = @DashPath, DashStatus = CASE WHEN @DashStatus = 1 THEN 'true' ELSE 'false' END, DashDefault = CASE WHEN @DashDefault = 1 THEN 'true' ELSE 'false' END WHERE DashId = @DashId";

                foreach (var data in updatedData)
                {
                    // Convert DashStatus and DashDefault to bool explicitly
                    var parameters = new
                    {
                        data.DashId,
                        data.DashName,
                        data.DashPath,
                        DashStatus = data.DashStatus ? true : false,
                        DashDefault = data.DashDefault ? true : false,
                    };

                    connection.Execute(sql, parameters);


                    foreach(var item in updatedData)
                    {
                       if(item.DashDefault == true)
                        {
                            var test = updatedData.Select(s => s.DashId).FirstOrDefault();

                            var sql1 = $"update DashboardMaster set DashDefault = 'false' where DashId <> {test}";
                            connection.Execute(sql1);
                        }
                    }
                    

                    string c2 = $"UPDATE Menu_Child_New SET SubMenuName = @SubMenuName WHERE MainMenuCode = 5 AND DashId = @DashId";

                    var c2Parameters = new
                    {
                        SubMenuName = data.DashName,
                        DashId = data.DashId
                    };

                    connection.Execute(c2, c2Parameters);
                }

            }
        }

        public void InsertDashboardData(IEnumerable<DashboardDataModel> newData)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                var sql = "INSERT INTO DashboardMaster (DashName, DashPath, DashStatus, DashDefault) VALUES (@DashName, @DashPath, CASE WHEN @DashStatus = 1 THEN 'true' ELSE 'false' END, CASE WHEN @DashDefault = 1 THEN 'true' ELSE 'false' END)";


                foreach (var data in newData)
                {
                    // Convert DashStatus and DashDefault to bool explicitly
                    var parameters = new
                    {
                        data.DashName,
                        data.DashPath,
                        DashStatus = data.DashStatus ? true : false, // Ensure it's a boolean value
                        DashDefault = data.DashDefault ? true : false, // Ensure it's a boolean value
                    };

                    connection.Execute(sql, parameters);
                }

                SqlConnection con = new SqlConnection(connection.ConnectionString);


                string s1 = "select Top (1) * from Menu_Child_New  where MainMenuCode=5 order by SubMenuCode desc"
                                    + " Select Top (1) *from DashboardMaster order by DashId desc";
                SqlDataAdapter Adpt = new SqlDataAdapter(s1, con);
                DataSet ds1 = new DataSet();
                Adpt.Fill(ds1);


                int submenucode = 0;
                int DashId = 0;
                string DashName = null;


                if (ds1.Tables[0].Rows.Count > 0)
                {
                    submenucode = (Convert.ToInt32(ds1.Tables[0].Rows[0][1].ToString()) + 1);
                }

                if (ds1.Tables[1].Rows.Count > 0)
                {
                    DashId = (Convert.ToInt32(ds1.Tables[1].Rows[0][0].ToString()));
                    DashName = ds1.Tables[1].Rows[0]["DashName"].ToString();
                }

                var sql2 = $" insert into Menu_Child_New (MainMenuCode, SubMenuCode, SubMenuName, Status, Frm_Level, RptId, Link_FormName, DashId) VALUES (5, {submenucode}, '{DashName}', 0, 0, 0, 'xDashboardReport', {DashId} )";
                connection.Execute(sql2);
            }
        }

        //to delete the dash config by id
        public void DeleteDashById(int dashId)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                try
                {
                    string query1 = "DELETE FROM DashboardMaster WHERE DashId = @DashId";
                    connection.Execute(query1, new { DashId = dashId }, transaction);

                    string query2 = "DELETE FROM Menu_Child_New WHERE DashId = @DashId";
                    connection.Execute(query2, new { DashId = dashId }, transaction);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error deleting dashboard with ID {dashId}: {ex.Message}");
                    throw; // Optionally handle or log the exception as needed
                }
            }
        }

        public IEnumerable<SettingProfileModel> GetSettingProfileData()
        {
            string query = "SELECT * FROM EmailSettings";
            return ExecuteQuery<SettingProfileModel>(query);
        }

        public IEnumerable<SettingContactModel> GetSettingContactData()
        {
            string query = "SELECT * FROM ContactMaster";
            return ExecuteQuery<SettingContactModel>(query);
        }

    }
}

