using Dapper;
using DevExpress.CodeParser;
using DevExpress.XtraRichEdit.Model;
using final_aerialview.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;


namespace final_aerialview.Data
{
    public class DataAccess
    {
        private readonly IConfiguration _configuration;

        public DataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region creating connection with the default connection string saved in appsetting.json 
        private IDbConnection CreateConnection()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "Connection String not found";
            return new SqlConnection(connectionString);
        }
        #endregion

        #region executing non-query commands (e.g., INSERT, UPDATE, DELETE)
        public int ExecuteNonQuery(string query, object parameters = null)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                return connection.Execute(query, parameters); // Executes the query and returns the number of affected rows
            }
        }
        #endregion


        #region opening the default connection
        public IEnumerable<T> ExecuteQuery<T>(string query)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                return connection.Query<T>(query);
            }
        }
        #endregion

        #region for login
        public async Task<UserModel> GetUserByUsernameAsync(string userId)
        {
            using (var connection = CreateConnection())
            {
                string query = "SELECT * FROM UserMaster WHERE UserId = @UserId";
                return await connection.QuerySingleOrDefaultAsync<UserModel>(query, new { UserId = userId });
            }
        }
        #endregion

        #region Segregate the sidebar navigation based on whether a user or an admin is logged in 
        public async Task<IEnumerable<UserMasterModel>> GetAccessibleControlsForUserAsync(string role)
        {
            using (var connection = CreateConnection())
            {
                string query = "SELECT * FROM UserControlMaster WHERE UserName = @UserName AND Status = 'True'";
                return await connection.QueryAsync<UserMasterModel>(query, new { UserName = role });
            }
        }
        #endregion

        #region main maenu parent data
        public IEnumerable<ListDataModel> GetMenuParentData()
        {
            string query = "SELECT * FROM Menu_Parent";
            return ExecuteQuery<ListDataModel>(query);
        }
        #endregion

        #region sub menu data
        public IEnumerable<SubMenuModel> GetSubMenuData()
        {
            string query = "SELECT * FROM Menu_Child_New";
            return ExecuteQuery<SubMenuModel>(query);
        }
        #endregion

        #region child menu data
        public IEnumerable<ChildMenuModel> GetChildMenuData()
        {
            string query = "SELECT * FROM ReportDATA_View";
            return ExecuteQuery<ChildMenuModel>(query);
        }
        #endregion

        #region project setting table which includes pdf image, client name, project name, module key etc.
        public IEnumerable<PdfImageModel> GetPdfImageData()
        {
            string query = "SELECT CONVERT(VARCHAR(MAX), Logo, 2) AS Logo, ClientName, ProjectName, ModuleKey FROM ProjectSettings";
            return ExecuteQuery<PdfImageModel>(query);
        }
        #endregion

        #region my reports table
        public IEnumerable<LocalReportModel> GetReportData()
        {
            string query = "SELECT * FROM MyReports";
            return ExecuteQuery<LocalReportModel>(query);
        }
        #endregion

        #region to update the saved report path in database
        public void UpdateReportPath(string url)
        {
            var rptid = UpdateModel.DatagridRptid;
            string query = "UPDATE MyReports SET WebReportPath = @reportName WHERE RptId = @DatagridRptid";

            CreateConnection().Execute(query, new { reportName = url, DatagridRptid = rptid });
        }
        #endregion

        #region to fetch the saved report from the database
        public ReportDataModel? GetReportFromDatabase()
        {
            try
            {
                var reportId = UpdateModel.DatagridRptid;
                string query = "SELECT ReportName, WebReportPath FROM MyReports WHERE RptId = @ReportId";
                return CreateConnection().QueryFirstOrDefault<ReportDataModel>(query, new { ReportId = reportId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving report from the database: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region creating dynamic sql query for reports table with dynamic connection string according to user selection 
        //to connect with dynamic connection string 
        public IEnumerable<dynamic> DynamicConnString(string connectionString, string tableName, string option, string selectedValue, string fromDate, string toDate)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string? whereClause = GenerateWhereClause(option, selectedValue, fromDate, toDate);
                string sqlQuery = $"SELECT TOP 1000 * FROM {tableName} {whereClause} order by DateAndTime desc ";

                return connection.Query<dynamic>(sqlQuery);
            }
        }
        #endregion

        #region conditional table for the datagrid to show only those columns for which the required is true in report config according to rptid
        public IEnumerable<ConditionalTableModel> ConditionalTable(int rptId)
        {
            string query = $"SELECT * from Report_Config where RptId = '{rptId}' and Required = 'true'";
            return ExecuteQuery<ConditionalTableModel>(query);
        }
        #endregion

        #region generating where clause according to datagrid filter
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
                    if (selectedValue != "All")
                    {
                        whereClause = $"WHERE DateAndTime >= '{filterDate:yyyy-MM-ddTHH:mm:ss}'";
                    }
                }
            }

            return whereClause;
        }
        #endregion

        #region dashboard master (dash config) table
        public IEnumerable<DashboardDataModel> GetDashboardMasterData()
        {
            string query = "Select DashId,DashName,DashPath, (case when DashStatus ='' then 'False' else DashStatus end) as 'DashStatus',(case when DashDefault ='' then 'False' else DashDefault end) as 'DashDefault' from DashboardMaster order by DashId asc";
            return ExecuteQuery<DashboardDataModel>(query);
        }
        #endregion

        #region update the existing rows of dashboard config table
        public List<string> UpdateDashboardData(IEnumerable<DashboardDataModel> updatedData)
        {
            var errorMessages = new List<string>();

            using (var connection = CreateConnection())
            {
                connection.Open();

                var checkExistsSql = "SELECT COUNT(*) FROM DashboardMaster WHERE DashName = @DashName AND DashId <> @DashId";
                var sql = "UPDATE DashboardMaster SET DashName = @DashName, DashPath = @DashPath, DashStatus = CASE WHEN @DashStatus = 1 THEN 'true' ELSE 'false' END, DashDefault = CASE WHEN @DashDefault = 1 THEN 'true' ELSE 'false' END WHERE DashId = @DashId";

                foreach (var data in updatedData)
                {
                    // Check if DashName has changed
                    var currentDashNameSql = "SELECT DashName FROM DashboardMaster WHERE DashId = @DashId";
                    var currentDashName = connection.ExecuteScalar<string>(currentDashNameSql, new { data.DashId });

                    if (currentDashName != data.DashName)
                    {
                        // If DashName has changed, check for duplicates
                        var exists = connection.ExecuteScalar<int>(checkExistsSql, new { data.DashName, data.DashId });

                        if (exists > 0)
                        {
                            errorMessages.Add($"DashName '{data.DashName}' already exists.");
                            continue;
                        }
                    }

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

                    // Updating related tables like Menu_Child_New and UserControlMaster if needed
                    foreach (var item in updatedData)
                    {
                        if (item.DashDefault == true)
                        {
                            var test = updatedData.Select(s => s.DashId).FirstOrDefault();
                            var sql1 = $"UPDATE DashboardMaster SET DashDefault = 'false' WHERE DashId <> {test}";
                            connection.Execute(sql1);
                        }
                    }

                    string c2 = $"UPDATE Menu_Child_New SET SubMenuName = @SubMenuName WHERE MainMenuCode = 5 AND DashId = @DashId";
                    string c3 = $"  UPDATE UserControlMaster SET ControlName = @SubMenuName, Status = CASE WHEN @DashStatus = 1 THEN 'true' ELSE 'false' END WHERE refId = @DashId";

                    var c2Parameters = new
                    {
                        SubMenuName = data.DashName,
                        DashId = data.DashId,
                        DashStatus = data.DashStatus
                    };

                    var appendedString = c2 + c3;
                    connection.Execute(appendedString, c2Parameters);
                }
            }
            return errorMessages;
        }
        #endregion

        #region insert new rows in dash config table 
        public List<string> InsertDashboardData(IEnumerable<DashboardDataModel> newData)
        {

            var errorMessages = new List<string>();

            using (var connection = CreateConnection())
            {
                connection.Open();

                var checkExistsSql = "Select count(*) from DashboardMaster WHERE DashName = @DashName";
                var sql = "INSERT INTO DashboardMaster (DashName, DashPath, DashStatus, DashDefault) VALUES (@DashName, @DashPath, CASE WHEN @DashStatus = 1 THEN 'true' ELSE 'false' END, CASE WHEN @DashDefault = 1 THEN 'true' ELSE 'false' END)";

                foreach (var data in newData)
                {

                    var exists = connection.ExecuteScalar<int>(checkExistsSql, new { data.DashName });

                    if (exists > 0)
                    {
                        errorMessages.Add($"DashName '{data.DashName}' already exists.");
                        continue;
                    }

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
                                    + " Select Top (1) * from DashboardMaster order by DashId desc";
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
                    DashName = ds1.Tables[1].Rows[0]["DashName"].ToString() ?? string.Empty;
                }

                var sql2 = $" insert into Menu_Child_New (MainMenuCode, SubMenuCode, SubMenuName, Status, Frm_Level, RptId, Link_FormName, DashId) VALUES (5, {submenucode}, '{DashName}', 0, 0, 0, 'xDashboardReport', {DashId} )";
                connection.Execute(sql2);
            }
            return errorMessages;
        }
        #endregion

        #region to delete the dash config by id
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

                    string query3 = "DELETE FROM UserControlMaster WHERE refId = @DashId";
                    connection.Execute(query3, new { DashId = dashId }, transaction);

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
        #endregion

        #region to get the connection string for report/dashboard designer and adding xpoProvider 
        public IEnumerable<ReportConnectionModel> GetReportConnectionData()
        {
            string query = "SELECT * FROM Report_Connection";
            var connections = ExecuteQuery<ReportConnectionModel>(query);

            foreach (var connection in connections)
            {
                //checking the connection type and adding the xpo provider as needed
                if (connection.ConType == "Sql Server")
                {
                    connection.stringName = $"XpoProvider=MSSqlServer;{connection.stringName};TrustServerCertificate=true";
                }
                else if (connection.ConType == "Windows")
                {
                    // Use Integrated Security for Windows authentication
                    connection.stringName = $"XpoProvider=MSSqlServer;{connection.stringName}Persist Security Info=true;TrustServerCertificate=true";
                }
                //Add other data types as needed
            }

            return connections;
        }
        #endregion

        #region report config whole table
        public IEnumerable<ReportConfigModel> ReportConfigData()
        {
            string query = "SELECT * FROM Report_Config";
            return ExecuteQuery<ReportConfigModel>(query);
        }
        #endregion

        #region report config selected table according to rptid
        public IEnumerable<ReportConfigModel> SelectedReportConfigData(int datagridRptid)
        {
            string query = $"SELECT * FROM Report_Config where RptId = {datagridRptid}";
            return ExecuteQuery<ReportConfigModel>(query);
        }
        #endregion

        #region update report config table in the database
        public void BatchUpdateReportConfig(IEnumerable<ReportConfigModel> changes)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Prepare the SQL command for updating
                        string updateQuery = @"
                    UPDATE Report_Config
                    SET 
                        Required = CASE WHEN @Required = 1 THEN 'True' ELSE 'False' END,
                        DisplayName = @DisplayName,
                        EditableColumn = CASE WHEN @EditableColumn = 1 THEN 'True' ELSE 'False' END,
GrandTotal = @GrandTotal
                    WHERE Rid = @Rid";
                        foreach (var change in changes)
                        {
                            connection.Execute(updateQuery, change, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw; // rethrow the exception to handle it in the controller
                    }
                }
            }
        }
        #endregion


        #region update the main reports datagrid in the database after editing 
        public void UpdatedDataGrid(List<Dictionary<string, object>> updates, string tableName, string connString)
        {
            using (var connection = new SqlConnection(connString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var data in updates)
                        {
                            var setClause = new List<string>();
                            string dateAndTime = null;

                            // Build the SET clause and get DateAndTime separately
                            foreach (var kvp in data)
                            {
                                // Skip keys "Id" or "ID" when building the SET clause
                                if (kvp.Key.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                                    kvp.Key.Equals("ID", StringComparison.OrdinalIgnoreCase))
                                {
                                    continue; // Skip this key
                                }

                                if (kvp.Key == "DateAndTime")
                                {
                                    // Ensure proper handling of DateAndTime
                                    dateAndTime = formatDateToSQL((DateTime)kvp.Value); // Ensure kvp.Value is a DateTime
                                }
                                else
                                {
                                    // Check if value is a JsonElement
                                    if (kvp.Value is JsonElement jsonElement)
                                    {
                                        if (jsonElement.ValueKind == JsonValueKind.String)
                                        {
                                            setClause.Add($"{kvp.Key} = @p{kvp.Key}");
                                        }
                                        else if (jsonElement.ValueKind == JsonValueKind.Number)
                                        {
                                            setClause.Add($"{kvp.Key} = @p{kvp.Key}");
                                        }
                                    }
                                    else
                                    {
                                        setClause.Add($"{kvp.Key} = @p{kvp.Key}");
                                    }
                                }
                            }

                            if (setClause.Count > 0 && dateAndTime != null)
                            {
                                // Create the SQL update command
                                var sqlCommand = $"UPDATE {tableName} SET {string.Join(", ", setClause)} WHERE DateAndTime = @DateAndTime";

                                using (var command = new SqlCommand(sqlCommand, connection, transaction)) // Declare command here
                                {
                                    // Add parameters for each column
                                    foreach (var kvp in data)
                                    {
                                        if (kvp.Key != "DateAndTime" && !kvp.Key.Equals("Id", StringComparison.OrdinalIgnoreCase) &&
                                    !kvp.Key.Equals("ID", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (kvp.Value is JsonElement jsonElement)
                                            {
                                                if (jsonElement.ValueKind == JsonValueKind.String)
                                                {
                                                    command.Parameters.AddWithValue($"@p{kvp.Key}", jsonElement.GetString());
                                                }
                                                else if (jsonElement.ValueKind == JsonValueKind.Number)
                                                {
                                                    command.Parameters.AddWithValue($"@p{kvp.Key}", jsonElement.GetDouble());
                                                }
                                                // Handle other types as necessary
                                            }
                                            else
                                            {
                                                command.Parameters.AddWithValue($"@p{kvp.Key}", kvp.Value);
                                            }
                                        }
                                    }
                                    command.Parameters.AddWithValue("@DateAndTime", dateAndTime); // Add formatted date

                                    // Execute the update command
                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        // Commit the transaction if all updates are successful
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if any update fails
                        transaction.Rollback();
                        throw; // Optionally rethrow the exception for further handling
                    }
                }
            }
        }
        #endregion

        #region converting date and time value into string to maintain consistency for UpdateDataGrid
        private string formatDateToSQL(DateTime date)
        {
            const string format = "yyyy-MM-dd HH:mm:ss"; // SQL format
            return date.ToString(format);
        }
        #endregion


        #region Calculated Field Master
        public IEnumerable<CalculatedFieldModel> GetCalculatedFieldData(int rptId)
        {
            string query = $"SELECT * FROM CalculatedFieldMaster where RptId = {rptId}";
            return ExecuteQuery<CalculatedFieldModel>(query);
        }
        #endregion


        #region Delete from Calculated Field Master
        //public IEnumerable<CalculatedFieldModel> DeleteCalculatedFieldData(int? id)
        //{
        //    string query = $"DELETE FROM CalculatedFieldMaster WHERE Id = {id}";
        //    return ExecuteQuery<CalculatedFieldModel>(query);
        //}

        public bool DeleteCalculatedFieldData(int? id)
        {
            string query = $"DELETE FROM CalculatedFieldMaster WHERE Id = @Id";
            int rowsAffected = ExecuteNonQuery(query, new { Id = id });
            return rowsAffected > 0;
        }
        #endregion

        #region Insert/Update of Calculated field - update the calculated field master if the id already exists or insert if it does not 
        //public IEnumerable<CalculatedFieldModel> UpdateOrInsertCalculatedFieldData(string columnName, string formula, int rptId, int? Id)
        //{
        //    string query = $"if exists (select * from CalculatedFieldMaster where Id = {Id})\r\n" +
        //        $"Begin \r\n" +
        //        $"update CalculatedFieldMaster set ColumnName = '{columnName}', Formula = '{formula}' where Id = {Id} \r\n" +
        //        $"End\r\n" +
        //        $"else\r\n" +
        //        $"begin \r\n" +
        //        $"insert into  CalculatedFieldMaster  (RptId, ColumnName, Formula) values ({rptId} , '{columnName}' , '{formula}') \r\n" +
        //        $"end";
        //    return ExecuteQuery<CalculatedFieldModel>(query);
        //}
        public bool UpdateOrInsertCalculatedFieldData(string columnName, string formula, int rptId, int? Id)
        {
            string query = $"if exists (select * from CalculatedFieldMaster where Id = {Id})\r\n" +
                $"Begin \r\n" +
                $"update CalculatedFieldMaster set ColumnName = '{columnName}', Formula = '{formula}' where Id = {Id} \r\n" +
                $"End\r\n" +
                $"else\r\n" +
                $"begin \r\n" +
                $"insert into  CalculatedFieldMaster  (RptId, ColumnName, Formula) values ({rptId} , '{columnName}' , '{formula}') \r\n" +
                $"end";
            int rowsAffected = ExecuteNonQuery(query);
            return rowsAffected > 0;
        }
        #endregion

        #region event configuration view table data
        public IEnumerable<EventConfigViewModel> GetEventConfigData()
        {
            string query = "SELECT * FROM EventConfig_View ORDER BY Id ASC";
            return ExecuteQuery<EventConfigViewModel>(query);
        }
        #endregion

        #region inserting and updating data in event config master table
        //public IEnumerable<EventConfigViewModel> InsertOrUpdateEventConfigMasterData(int Id, string ConnString, string TableName, string ColumnName, int AlarmLow, int AlarmHigh, int WarningHigh, int WarningLow, string CreatedAt, string UpdatedAt, bool Status, int RptId, int CreatedById, int UpdateById, string Emails)
        public bool InsertOrUpdateEventConfigMasterData(int Id, string ConnString, string TableName, string ColumnName, int AlarmLow, int AlarmHigh, int WarningHigh, int WarningLow, string CreatedAt, string UpdatedAt, bool Status, int RptId, int CreatedById, int UpdateById, string Emails)
        {
            string query = $"if exists (select * from EventConfigurationMaster where Id = {Id})\r\n" +
                $"Begin\r\n UPDATE EventConfigurationMaster\r\n" +
                $"SET ConnString = '{ConnString}', TableName = '{TableName}', ColumnName = '{ColumnName}', AlarmLow = {AlarmLow}, AlarmHigh = {AlarmHigh}, WarningLow = {WarningLow}, WarningHigh = {WarningHigh}, UpdatedAt = '{UpdatedAt}', Status = '{Status}', RptId = {RptId}, UpdateById = {UpdateById}, Emails = '{Emails}'\r\n" +
                $"WHERE Id = {Id};" +
                $"\r\nEnd\r\n" +
                $"else\r\n" +
                $"begin\r\n" +
                $"insert into EventConfigurationMaster (ConnString, TableName, ColumnName, AlarmLow, AlarmHigh, WarningLow, WarningHigh, CreatedAt, UpdatedAt, Status, RptId, CreatedById, UpdateById, Emails) values('{ConnString}', '{TableName}', '{ColumnName}', {AlarmLow}, {AlarmHigh}, {WarningLow},  {WarningHigh}, '{CreatedAt}', '{UpdatedAt}', '{Status}', {RptId}, {CreatedById}, {UpdateById}, '{Emails}') \r\n" +
                $"end";
            //return ExecuteQuery<EventConfigViewModel>(query);

            int rowsAffected = ExecuteNonQuery(query);
            return rowsAffected > 0;
        }
        #endregion

        #region getting column names according to the selected connection string and table name for the alarm and event configuration page

        public IEnumerable<string> FetchColNamesForEventConfig(string connectionString, string tableName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Ensure the table name is safely handled
                string sqlQuery = $@"
            SELECT COLUMN_NAME 
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = '{tableName}'AND DATA_TYPE IN ('real', 'int', 'decimal', 'numeric', 'float')"; // condition so that the columns we get are of type (real, int, decimal, numeric, float)

                // Use Dapper to execute the query and fetch the column names
                return connection.Query<string>(sqlQuery);
            }
        }
        #endregion

        #region to populate form on the basis of selected row from the table
        public IEnumerable<EventConfigViewModel> GetEventConfigDataById(int id)
        {
            string query = $"SELECT * FROM EventConfig_View where Id = {id}";
            return ExecuteQuery<EventConfigViewModel>(query);
        }
        #endregion

        #region delete selected data from event config master table
        //public IEnumerable<EventConfigViewModel> DeleteEventConfigData(int? id)
        //{
        //    string query = $"DELETE FROM EventConfigurationMaster WHERE Id = {id}";
        //    return ExecuteQuery<EventConfigViewModel>(query);
        //}

        public bool DeleteEventConfigData(int? id)
        {
            string query = $"DELETE FROM EventConfigurationMaster WHERE Id = @Id";
            int rowsAffected = ExecuteNonQuery(query, new { Id = id }); // Assuming ExecuteNonQuery returns the number of rows affected
            return rowsAffected > 0; // Return true if at least one row was affected, else false
        }

        #endregion
    }
}

