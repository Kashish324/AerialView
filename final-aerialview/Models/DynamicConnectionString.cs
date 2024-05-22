namespace final_aerialview.Models
{
    public class DynamicConnectionString
    {
        public string? DataSource { get; set; }
        public string? InitialCatalog { get; set; }
        public string? UserID { get; set; }
        public string? Password { get; set; }
        public bool PersistSecurityInfo { get; set; }
        public bool IntegratedSecurity { get; set; }
    }
}
