namespace final_aerialview.Models
{
    public class EventConfigViewModel
    {
        public int Id { get; set; }
        public string? ConnString { get; set; }
        public string? TableName { get; set; }
        public string? ColumnName { get; set; }
        public int AlarmLow { get; set; }
        public int AlarmHigh { get; set; }
        public int WarningLow { get; set; }
        public int WarningHigh { get; set; }
        //public DateTime? CreatedAt { get; set; }
        //public DateTime? UpdatedAt { get; set; }

        public string? CreatedAt { get; set; }
        public string? UpdatedAt { get; set; }


        public bool? Status { get; set; }
        public int CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public int UpdateById { get; set; }
        public string? UpdatedByName { get; set; }
        public int RptId { get; set; }
        public string? ReportName { get; set; }
        public string? Emails { get; set; }

    }
}
