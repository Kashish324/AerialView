namespace final_aerialview.Models
{
    public class ConditionalTableModel
    {
        public string? TableName {  get; set; }
        public string? ColumnName { get; set;}
        public bool Required {  get; set; }
        public string? DisplayName {  get; set; }
        public bool? EditableColumn { get; set; }
        public string? DataType { get; set; }
        public string? GrandTotal { get; set; }
    }
}
