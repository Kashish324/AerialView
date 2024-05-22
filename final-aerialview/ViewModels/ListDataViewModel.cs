using final_aerialview.Models;

namespace final_aerialview.ViewModels
{
    public class ListDataViewModel
    {
        public IEnumerable<ChildMenuModel>? ChildMenuData { get; set; }
        public string? ChildMenuName { get; set; }
        public string? ConnectionString { get; set; }
        public string? TableName { get; set; }
        public string? DynamicConnectionString { get; set; }

        public int RptId { get; set; }

        //IEnumerable -- it is a list/container which holds some items and you can iterate over it but cannot modify anything in it 
        public IEnumerable<dynamic>? TableData { get; set; }

        public string? DataBaseName { get; set; }
        public string? DataSource { get; set; }
        public string? UserID { get; set; }
        public string? Password { get; set; }

        public IEnumerable<PdfImageModel>? PdfImageData { get; set; }
    }
}
