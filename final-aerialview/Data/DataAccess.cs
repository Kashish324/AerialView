using Dapper;
using DevExpress.XtraReports.Wizards;
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

        public IEnumerable<ListDataModel> GetMenuParentData()
        {
            string query = "SELECT * FROM Menu_Parent";
            return ExecuteQuery<ListDataModel>(query);
        }

        public IEnumerable<SubMenuModel> GetSubMenuData()
        {
            string query = "SELECT * FROM Menu_Child_New";
            return ExecuteQuery<SubMenuModel>(query);
        }

        public IEnumerable<ChildMenuModel> GetChildMenuData()
        {
            string query = "SELECT * FROM ReportDATA_View";
            return ExecuteQuery<ChildMenuModel>(query);
        }


        public IEnumerable<PdfImageModel> GetPdfImageData()
        {
            //string query = "Select * from ProjectSettings";
            string query = "SELECT CONVERT(VARCHAR(MAX), Logo, 2) AS Logo, ClientName, ProjectName FROM ProjectSettings";
            return ExecuteQuery<PdfImageModel>(query);


        }


        public IEnumerable<LocalReportModel> GetReportData()
        {
            string query = "SELECT * FROM MyReports";
            return ExecuteQuery<LocalReportModel>(query);
        }



        public void UpdateReportPath(string url)
        {
            var rptid = UpdateModel.DatagridRptid;
            string query = "UPDATE MyReports SET WebReportPath = @reportName WHERE RptId = @DatagridRptid";

            //CreateConnection().Execute(query, new { FullReportPath = fullReportPath, DatagridRptid = rptid });
            CreateConnection().Execute(query, new { reportName = url, DatagridRptid = rptid });
        }



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


        public IEnumerable<FilterModel> GetFilteredData(string whereClause)
        {
            var tableName = UpdateModel.DatagridTableName;
            string query = $"SELECT * FROM {tableName} {whereClause}";
            return ExecuteQuery<FilterModel>(query);
        }




    }
}

//select* from T1 where DateAndTime between '2018-03-26 10:28:46.000' and '2018-03-26 10:30:27.000'