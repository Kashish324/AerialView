namespace final_aerialview.Models
{
    public class UpdateDataRequestModel
    {
        public string? TableName { get; set; }
        public string? ConnString { get; set; }
        public List<Dictionary<string, object>>? Changes { get; set; }
    }


}
