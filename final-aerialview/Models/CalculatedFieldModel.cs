using System.ComponentModel.DataAnnotations;

namespace final_aerialview.Models
{
    public class CalculatedFieldModel
    {
        public int? Id { get; set; }
        public int RptId { get; set; }

        [Required]
        public string? ColumnName { get; set; }

        [Required]
        public string? Formula { get; set; }
    }
}
