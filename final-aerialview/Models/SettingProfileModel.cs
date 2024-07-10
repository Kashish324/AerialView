namespace final_aerialview.Models
{
    public class SettingProfileModel
    {
        public int EmailProfileId { get; set; }
        public string? EMailProfileName { get; set; }
        public string? SendEmailId { get; set; }
        public string? Password { get; set; }
        public string? SmptName { get; set; }
        public int smtpPort { get; set; }
    }
}
