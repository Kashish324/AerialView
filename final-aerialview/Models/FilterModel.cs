using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Models
{
    public class FilterModel
    {
        [BindProperty]
        public string SelectedPriority { get; set; } = "Standard";
        public string[] Priorities { get; } = new[] { "Standard", "Date" };

        public string? FilterType { get; set; }
        public string? StandardFilter { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
    }
}
