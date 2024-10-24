using final_aerialview.Models;

namespace final_aerialview.ViewModels
{
    public class ListDataViewModel
    {
        public IEnumerable<ChildMenuModel>? ChildMenuData { get; set; }
        public string? ChildMenuName { get; set; }
        public string? ConnectionString { get; set; }
        public string? TableName { get; set; }
        public int RptId { get; set; }
        //IEnumerable -- it is a list/container which holds some items and you can iterate over it but cannot modify anything in it 
        public IEnumerable<dynamic>? TableData { get; set; }
        public IEnumerable<dynamic>? ConditionalTableData { get; set; }
        public IEnumerable<PdfImageModel>? PdfImageData { get; set; }
        public IEnumerable<CalculatedFieldModel>? CalculatedFields { get; set; }

        // New properties for filter values
        public string? SelectedOption { get; set; }
        public string? SelectedValue { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }
}
