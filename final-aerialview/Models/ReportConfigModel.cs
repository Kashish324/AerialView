namespace final_aerialview.Models
{
    public class ReportConfigModel
    {
        public int Rid { get; set; }
        public string? ReportName { get; set; }
        public string? ConnName { get; set; }
        public string? TableName { get; set; }
        public string? ColumnName { get; set; }
        public string? DataType { get; set; }
        public bool? Required { get; set; }
        public string? DisplayName { get; set; }
        public int RptId { get; set; }
        public bool? EditableColumn { get; set; }
        public bool? CalculatedField { get; set; }
    }
}
