using System.ComponentModel.DataAnnotations;

namespace final_aerialview.Models
{
    public class CustomProjectModel
    {
        public string? ProjectName { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? ProjectUrl { get; set; }

    }
}
