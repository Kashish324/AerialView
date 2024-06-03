namespace final_aerialview.Models
{
    public class LocalReportModel
    {
        public int RptId { get; set; }

        public string? ReportName { get; set; }

        public string? DataTableName { get; set; }

        public string? WebReportPath { get; set; }

        public string? ReportType { get; set;}
        public string? ConnName { get; set; }
        public string? Status { get;set; }
    }
}
