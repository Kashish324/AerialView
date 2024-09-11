using final_aerialview.Models;

namespace final_aerialview.ViewModels
{
    public class EmailSettingViewModel
    {
        public SettingProfileModel ? Profile { get; set; }
        public List<SettingContactModel> ? Contacts { get; set; }
    }
}